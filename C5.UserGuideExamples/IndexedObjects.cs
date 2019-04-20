// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// IndexedObjects.cs sketch 2007-07-26 

// Other approaches: Define an Indexed<T> class to which indexers can
// be attached.  An object can add (or remove) itself to the indexers
// that are attached to the Indexed class at the time of addition (or
// removal).

// Maintaining multiple indices on objects, each index defined by a
// delegate.

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.IndexedObjectsProgram
//  dotnet run

using System;
using System.Text;

namespace C5.UserGuideExamples
{
    static class IndexedObjectsProgram
    {
        static void Main(string[] args)
        {
            var persons = new Indexed<PersonIndexedObjects>(
                new IndexMaker<PersonIndexedObjects, string>("name", delegate (PersonIndexedObjects p) { return p.Name; }),
                new IndexMaker<PersonIndexedObjects, int>("year", delegate (PersonIndexedObjects p) { return p.Date / 10000; }),
                new IndexMaker<PersonIndexedObjects, int>("day", delegate (PersonIndexedObjects p) { return p.Date % 100; }),
                new IndexMaker<PersonIndexedObjects, string>("month", delegate (PersonIndexedObjects p) { return _months[p.Date / 100 % 100 - 1]; })
            );
            persons.Add(new PersonIndexedObjects("Niels", 19470206));
            persons.Add(new PersonIndexedObjects("Lone", 19600810));
            persons.Add(new PersonIndexedObjects("Peter", 19620625));
            persons.Add(new PersonIndexedObjects("Carsten", 19640627));
            persons.Add(new PersonIndexedObjects("Hanne", 19641209));
            persons.Add(new PersonIndexedObjects("Dorte", 19660930));
            persons.Add(new PersonIndexedObjects("Dorte", 19610312));
            persons.Add(new PersonIndexedObjects("Jørgen", 19340930));
            persons.Add(new PersonIndexedObjects("Kirsten", 19360114));
            persons.Add(new PersonIndexedObjects("Henrik", 19360630));
            persons.Add(new PersonIndexedObjects("Lars", 19640625));
            persons.Add(new PersonIndexedObjects("Thora", 19091129));

            Console.WriteLine("Born in 1964:");
            foreach (var p in persons["year"][1964])
            {
                Console.WriteLine(p);
            }

            Console.WriteLine("Born in June:");
            foreach (var p in persons["month"]["Jun"])
            {
                Console.WriteLine(p);
            }

            Console.WriteLine("Named Dorte:");
            foreach (var p in persons["name"]["Dorte"])
            {
                Console.WriteLine(p);
            }

            Console.WriteLine(persons);
        }

        private static readonly string[] _months = 
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"
        };
    }

    public interface IIndexer<Q, R>
    {
        ICollectionValue<R> this[Q x] { get; }
    }

    // An index maker has a name, it supports adding and removing items
    // from the index, and looking up items by key (here of type
    // Object).
    public abstract class IndexMaker<T> : IIndexer<object, T>
    {
        public string Name { get; }
        public abstract bool Add(T item);
        public abstract bool Remove(T item);
        public abstract ICollectionValue<T> this[object key] { get; }

        public IndexMaker(string name)
        {
            Name = name;
        }
    }

    // The implementation of an index maker consists of a function to
    // map an item of type T to the index key type Q, and a dictionary
    // that maps an index key (of type Q) to a set of items (each of
    // type T).
    public class IndexMaker<T, Q> : IndexMaker<T>
        where T : class
        where Q : IComparable<Q>
    {
        private readonly Func<T, Q> _fun;
        private readonly TreeDictionary<Q, HashSet<T>> _dictionary;

        public IndexMaker(string name, Func<T, Q> fun) : base(name)
        {
            _fun = fun;
            _dictionary = new TreeDictionary<Q, HashSet<T>>();
        }

        public override bool Add(T item)
        {
            var key = _fun(item);
            if (!_dictionary.Contains(key))
            {
                _dictionary.Add(key, new HashSet<T>(EqualityComparer<T>.Default));
            }

            return _dictionary[key].Add(item);
        }

        public override bool Remove(T item)
        {
            var key = _fun(item);

            return !_dictionary.Contains(key) ? false : _dictionary[key].Remove(item);
        }

        public ICollectionValue<T> this[Q key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        public override ICollectionValue<T> this[object key]
        {
            get
            {
                return _dictionary[(Q)key];
            }
        }

        public override string ToString()
        {
            return _dictionary.ToString();
        }
    }

    // Weakly typed implementation of multiple indexers on a class T.  

    // The implementation is an array of index makers, each consisting
    // of the index's name and its implementation which supports adding
    // and removing T objects, and looking up T objects by the key
    // relevant for that index.
    public class Indexed<T> where T : class
    {
        private readonly IndexMaker<T>[] _indexMakers;

        public Indexed(params IndexMaker<T>[] indexMakers)
        {
            _indexMakers = indexMakers;
        }

        public bool Add(T item)
        {
            bool result = false;

            foreach (var indexMaker in _indexMakers)
            {
                result |= indexMaker.Add(item);
            }

            return result;
        }

        public bool Remove(T item)
        {
            bool result = false;
            foreach (var indexMaker in _indexMakers)
            {
                result |= indexMaker.Remove(item);
            }

            return result;
        }

        public IIndexer<object, T> this[string name]
        {
            get
            {
                foreach (var indexMaker in _indexMakers)
                {
                    if (indexMaker.Name == name)
                    {
                        return indexMaker;
                    }
                }

                throw new Exception("Unknown index");
            }
        }

        // For debugging

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var indexMaker in _indexMakers)
            {
                sb.Append("\n----- ").Append(indexMaker.Name).Append("-----\n");
                sb.Append(indexMaker);
            }
            return sb.ToString();
        }
    }

    // Sample class with two fields but many possible indexes
    public class PersonIndexedObjects
    {
        public string Name { get; }
        public int Date { get; } // YYYYMMDD as in 20070725

        public PersonIndexedObjects(string name, int date)
        {
            Name = name;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Name} ({Date})";
        }
    }

    // The interface of a strongly typed indexing of PersonIndexed objects:
    // (Not yet used)
    public interface IPersonIndexers
    {
        IIndexer<string, PersonIndexedObjects> Name { get; }
        IIndexer<int, PersonIndexedObjects> Year { get; }
        IIndexer<int, PersonIndexedObjects> Day { get; }
        IIndexer<string, PersonIndexedObjects> Month { get; }
    }
}
