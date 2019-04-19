// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: EventPatterns.cs for pattern chapter

// Compile with 
//   csc /r:C5.dll EventPatterns.cs 

using System;
using C5;

#pragma warning disable IDE0059 // Value assigned to symbol is never used
namespace EventPatterns
{
    class EventPatterns
    {
        public static void Main(String[] args)
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
            coll.CollectionChanged
              += delegate(Object c)
            {
                Console.WriteLine("Collection changed");
            };
            // Add cleared handler
            coll.CollectionCleared
              += delegate(Object c, ClearedEventArgs args)
            {
                Console.WriteLine("Collection cleared");
            };
            // Add added handler
            coll.ItemsAdded
              += delegate(Object c, ItemCountEventArgs<int> args)
            {
                Console.WriteLine("Item {0} added", args.Item);
            };
            // Add item count handler
            AddItemsAddedCounter(coll);
            AddItemsRemovedCounter(coll);
            coll.AddAll(bag1);
            coll.RemoveAll(new[] { 2, 5, 6, 3, 7, 2 });
            coll.Clear();
            ICollection<int> bag2 = new HashBag<int>();
            // Add added handler with multiplicity
            bag2.ItemsAdded
              += delegate(Object c, ItemCountEventArgs<int> args)
            {
                Console.WriteLine("{0} copies of {1} added",
                                  args.Count, args.Item);
            };
            bag2.AddAll(bag1);
            // Add removed handler with multiplicity
            bag2.ItemsRemoved
              += delegate(Object c, ItemCountEventArgs<int> args)
            {
                Console.WriteLine("{0} copies of {1} removed",
                                  args.Count, args.Item);
            };
            bag2.RemoveAllCopies(7);
        }

        // This works for all kinds of collections, also those with bag
        // semantics and representing duplicates by counting:

        private static void AddItemsAddedCounter<T>(ICollection<T> coll)
        {

            int addedCount = 0;
            coll.ItemsAdded
              += delegate(Object c, ItemCountEventArgs<T> args)
            {
                addedCount += args.Count;
            };
            coll.CollectionChanged
              += delegate(Object c)
            {
                if (addedCount > 0)
                    Console.WriteLine("{0} items were added", addedCount);
                addedCount = 0;
            };
        }

        // This works for all kinds of collections, also those with bag
        // semantics and representing duplicates by counting:

        private static void AddItemsRemovedCounter<T>(ICollection<T> coll)
        {
            int removedCount = 0;
            coll.ItemsRemoved
              += delegate(Object c, ItemCountEventArgs<T> args)
            {
                removedCount += args.Count;
            };
            coll.CollectionChanged
              += delegate(Object c)
            {
                if (removedCount > 0)
                    Console.WriteLine("{0} items were removed", removedCount);
                removedCount = 0;
            };
        }

        // Event patterns on indexed collections

        public static void IndexedCollectionEvents()
        {
            IList<int> coll = new ArrayList<int>();
            ICollection<int> bag = new HashBag<int>();
            bag.AddAll(new[] { 3, 2, 5, 5, 7, 7, 5, 3, 7, 7 });
            // Add item inserted handler
            coll.ItemInserted
              += delegate(Object c, ItemAtEventArgs<int> args)
            {
                Console.WriteLine("Item {0} inserted at {1}",
                                  args.Item, args.Index);
            };
            coll.InsertAll(0, bag);
            // Add item removed-at handler
            coll.ItemRemovedAt
              += delegate(Object c, ItemAtEventArgs<int> args)
            {
                Console.WriteLine("Item {0} removed at {1}",
                                  args.Item, args.Index);
            };
            coll.RemoveLast();
            coll.RemoveFirst();
            coll.RemoveAt(1);
        }

        // Recognizing Update event as a Removed-Added-Changed sequence

        private enum State { Before, Removed, Updated };

        private static void AddItemUpdatedHandler<T>(ICollection<T> coll)
        {
            State state = State.Before;
            T removed = default, added = default;
            coll.ItemsRemoved
              += delegate(Object c, ItemCountEventArgs<T> args)
            {
                if (state == State.Before)
                {
                    state = State.Removed;
                    removed = args.Item;
                }
                else
                    state = State.Before;
            };
            coll.ItemsAdded
              += delegate(Object c, ItemCountEventArgs<T> args)
            {
                if (state == State.Removed)
                {
                    state = State.Updated;
                    added = args.Item;
                }
                else
                    state = State.Before;
            };
            coll.CollectionChanged
              += delegate(Object c)
            {
                if (state == State.Updated)
                    Console.WriteLine("Item {0} was updated to {1}",
                                      removed, added);
                state = State.Before;
            };
        }

        public static void UpdateEvent()
        {
            ICollection<Teacher> coll = new HashSet<Teacher>();
            AddItemUpdatedHandler(coll);
            Teacher kristian = new Teacher("Kristian", "physics");
            coll.Add(kristian);
            coll.Add(new Teacher("Poul Einer", "mathematics"));
            // This should be caught by the update handler:
            coll.Update(new Teacher("Thomas", "mathematics"));
            // This should not be caught by the update handler:
            coll.Remove(kristian);
            coll.Add(new Teacher("Jens", "physics"));
            // The update handler is activated also by indexed updates
            IList<int> list = new ArrayList<int>();
            list.AddAll(new[] { 7, 11, 13 });
            AddItemUpdatedHandler(list);
            list[1] = 9;
        }
    }

    // Example class where objects may be equal yet display differently

    class Teacher : IEquatable<Teacher>
    {
        private readonly String name, subject;

        public Teacher(String name, String subject)
        {
            this.name = name; this.subject = subject;
        }

        public bool Equals(Teacher that)
        {
            return this.subject.Equals(that.subject);
        }

        public override int GetHashCode()
        {
            return subject.GetHashCode();
        }

        public override String ToString()
        {
            return name + "[" + subject + "]";
        }
    }
}
#pragma warning restore IDE0059 // Value assigned to symbol is never used
