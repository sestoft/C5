// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: EventPatterns.cs for pattern chapter

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.EventPatterns
//  dotnet run

using System;

//#pragma warning disable IDE0059 // Value assigned to symbol is never used
namespace C5.UserGuideExamples
{
    class EventPatterns
    {
        public static void Main(string[] args)
        {
            UnindexedCollectionEvents();
            Console.WriteLine("--------------------");
            IndexedCollectionEvents();
            Console.WriteLine("--------------------");
            UpdateEvent();
        }

        public static void UnindexedCollectionEvents()
        {
            ICollection<int> coll = new ArrayList<int>();
            ICollection<int> bag1 = new HashBag<int>();
            bag1.AddAll(new[] { 3, 2, 5, 5, 7, 7, 5, 3, 7, 7 });
            // Add change handler
            coll.CollectionChanged += o => Console.WriteLine("Collection changed");

            // Add cleared handler
            coll.CollectionCleared += (o, a) => Console.WriteLine("Collection cleared");

            // Add added handler
            coll.ItemsAdded += (o, args) => Console.WriteLine($"Item {args.Item} added");

            // Add item count handler
            AddItemsAddedCounter(coll);
            AddItemsRemovedCounter(coll);

            coll.AddAll(bag1);
            coll.RemoveAll(new[] { 2, 5, 6, 3, 7, 2 });
            coll.Clear();

            ICollection<int> bag2 = new HashBag<int>();

            // Add added handler with multiplicity
            bag2.ItemsAdded += (o, args) => Console.WriteLine($"{args.Count} copies of {args.Item} added");
            bag2.AddAll(bag1);

            // Add removed handler with multiplicity
            bag2.ItemsRemoved += (o, args) => Console.WriteLine($"{args.Count} copies of {args.Item} removed");
            bag2.RemoveAllCopies(7);
        }

        // This works for all kinds of collections, also those with bag
        // semantics and representing duplicates by counting:
        private static void AddItemsAddedCounter<T>(ICollection<T> coll)
        {

            var addedCount = 0;
            coll.ItemsAdded += (o, args) => addedCount += args.Count;
            coll.CollectionChanged += o =>
            {
                if (addedCount > 0)
                {
                    Console.WriteLine("{0} items were added", addedCount);
                }
                addedCount = 0;
            };
        }

        // This works for all kinds of collections, also those with bag
        // semantics and representing duplicates by counting:
        private static void AddItemsRemovedCounter<T>(ICollection<T> coll)
        {
            var removedCount = 0;
            coll.ItemsRemoved += (o, args) => removedCount += args.Count;
            coll.CollectionChanged += o =>
            {
                if (removedCount > 0)
                {
                    Console.WriteLine("{0} items were removed", removedCount);
                }
                removedCount = 0;
            };
        }

        // Event patterns on indexed collections
        public static void IndexedCollectionEvents()
        {
            var coll = new ArrayList<int>();
            var bag = new HashBag<int>();
            bag.AddAll(new[] { 3, 2, 5, 5, 7, 7, 5, 3, 7, 7 });

            // Add item inserted handler
            coll.ItemInserted += (o, args) => Console.WriteLine($"Item {args.Item} inserted at {args.Index}");
            coll.InsertAll(0, bag);

            // Add item removed-at handler
            coll.ItemRemovedAt += (o, args) => Console.WriteLine($"Item {args.Item} removed at {args.Index}");
            coll.RemoveLast();
            coll.RemoveFirst();
            coll.RemoveAt(1);
        }

        // Recognizing Update event as a Removed-Added-Changed sequence
        private enum State { Before, Removed, Updated };

        private static void AddItemUpdatedHandler<T>(ICollection<T> coll)
        {
            var state = State.Before;
            T removed = default;
            T added = default;

            coll.ItemsRemoved += (o, args) =>
            {
                if (state == State.Before)
                {
                    state = State.Removed;
                    removed = args.Item;
                }
                else
                {
                    state = State.Before;
                }
            };

            coll.ItemsAdded += (o, args) =>
            {
                if (state == State.Removed)
                {
                    state = State.Updated;
                    added = args.Item;
                }
                else
                {
                    state = State.Before;
                }
            };

            coll.CollectionChanged += o =>
            {
                if (state == State.Updated)
                {
                    Console.WriteLine($"Item {removed} was updated to {added}");
                }
                state = State.Before;
            };
        }

        public static void UpdateEvent()
        {
            var coll = new HashSet<Teacher>();
            AddItemUpdatedHandler(coll);
            var kristian = new Teacher("Kristian", "physics");
            coll.Add(kristian);
            coll.Add(new Teacher("Poul Einer", "mathematics"));
            // This should be caught by the update handler:
            coll.Update(new Teacher("Thomas", "mathematics"));
            // This should not be caught by the update handler:
            coll.Remove(kristian);
            coll.Add(new Teacher("Jens", "physics"));
            // The update handler is activated also by indexed updates
            var list = new ArrayList<int>();
            list.AddAll(new[] { 7, 11, 13 });
            AddItemUpdatedHandler(list);
            list[1] = 9;
        }
    }

    // Example class where objects may be equal yet display differently
    class Teacher : IEquatable<Teacher>
    {
        public string Name { get; }
        public string Subject { get; }

        public Teacher(string name, string subject)
        {
            Name = name;
            Subject = subject;
        }

        public bool Equals(Teacher that) => Subject == that.Subject;

        public override int GetHashCode() => Subject.GetHashCode();

        public override string ToString() => $"{Name} [{Subject}]";
    }
}
