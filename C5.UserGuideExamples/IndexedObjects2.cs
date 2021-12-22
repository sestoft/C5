// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// IndexedObjects2.cs sketch 2007-07-28

// Maintaining multiple type safe indices on objects using the C5
// library.

namespace C5.UserGuideExamples;

internal static class IndexedObjects2Program
{
    private static void Main()
    {
        // Create some PersonIndexed2 objects:
        new PersonIndexedObjects2("Niels", 19470206);
        new PersonIndexedObjects2("Lone", 19600810);
        new PersonIndexedObjects2("Peter", 19620625);
        new PersonIndexedObjects2("Carsten", 19640627);
        new PersonIndexedObjects2("Hanne", 19641209);
        new PersonIndexedObjects2("Dorte", 19660930);
        new PersonIndexedObjects2("Dorte", 19610312);
        new PersonIndexedObjects2("J�rgen", 19340930);
        new PersonIndexedObjects2("Kirsten", 19360114);
        new PersonIndexedObjects2("Henrik", 19360630);
        new PersonIndexedObjects2("Lars", 19640625);
        new PersonIndexedObjects2("Thora", 19091129);

        // Try the indexers:
        Console.WriteLine("Born in 1964:");
        foreach (PersonIndexedObjects2 p in PersonIndexedObjects2.YearIndex[1964])
        {
            Console.WriteLine(p);
        }
        Console.WriteLine();

        Console.WriteLine("\nNamed Dorte:");
        foreach (PersonIndexedObjects2 p in PersonIndexedObjects2.NameIndex["Dorte"])
        {
            Console.WriteLine(p);
        }
        Console.WriteLine();

        Console.WriteLine("Born on the 30th:");
        foreach (PersonIndexedObjects2 p in PersonIndexedObjects2.DayIndex[30])
        {
            Console.WriteLine(p);
        }
        Console.WriteLine();

        Console.WriteLine("Born June-October:");
        foreach (PersonIndexedObjects2 p in PersonIndexedObjects2.MonthIndex.RangeFromTo(Month.Jun, Month.Nov))
        {
            Console.WriteLine(p);
        }
    }
}

// The interface IIndex<Q,T> describes an index with keys of type Q
// on items of type T:
public interface IIndex<Q, T>
{
    ICollection<T> this[Q x] { get; }
    void Add(T item);
}

// A hash-based index supports adding items of type T to the index
// and looking up items by key of type Q.  The function toKey
// transforms an item to its index key.

// The hash-based index is implemented by a hash dictionary that
// maps each index key to a hash set of items.  The hash dictionary
// uses the key type's natural equality comparer.  The hash set uses
// a reference equality comparer for the item type -- it might also
// use the item type's natural comparer.
public class HashIndex<Q, T> : IIndex<Q, T> where T : class
{
    private readonly Func<T, Q> _toKey;
    private readonly HashDictionary<Q, HashSet<T>> _dictionary;

    public HashIndex(Func<T, Q> toKey)
    {
        _toKey = toKey;
        _dictionary = new HashDictionary<Q, HashSet<T>>();
    }

    public void Add(T item)
    {
        var key = _toKey(item);

        if (!_dictionary.Contains(key))
        {
            _dictionary.Add(key, new HashSet<T>(EqualityComparer<T>.Default));
        }

        _dictionary[key].Add(item);
    }

    public ICollection<T> this[Q key] => new GuardedCollection<T>(_dictionary[key]);

    public override string ToString()
    {
        return _dictionary.ToString();
    }
}

// A tree-based index supports adding items of type T to the index,
// looking up items by key of type Q, and getting the items within a
// key range.  The function toKey transforms an item to its index
// key.

// The tree-based index is implemented by a tree dictionary that
// maps each index key to a hash set of items.  The tree dictionary
// uses the key type's natural comparer.  The hash set uses a
// reference equality comparer for the item type -- it might also
// use the item type's natural comparer.
public class TreeIndex<Q, T> : IIndex<Q, T> where T : class
{
    private readonly Func<T, Q> _toKey;
    private readonly TreeDictionary<Q, HashSet<T>> _dictionary;

    public TreeIndex(Func<T, Q> toKey)
    {
        _toKey = toKey;
        _dictionary = new TreeDictionary<Q, HashSet<T>>();
    }

    public void Add(T item)
    {
        var key = _toKey(item);

        if (!_dictionary.Contains(key))
        {
            _dictionary.Add(key, new HashSet<T>(EqualityComparer<T>.Default));
        }

        _dictionary[key].Add(item);
    }

    public ICollection<T> this[Q key] => new GuardedCollection<T>(_dictionary[key]);

    // Bad implementation from a performance point of view: it may be
    // costly to build the result collection and possibly only its
    // size is ever used:
    public ICollection<T> RangeFromTo(Q x, Q y)
    {
        var result = new HashSet<T>(EqualityComparer<T>.Default);

        foreach (var kv in _dictionary.RangeFromTo(x, y))
        {
            result.AddAll(kv.Value);
        }

        return result;
    }

    public override string ToString()
    {
        return _dictionary.ToString();
    }
}

public enum Month
{
    Jan = 1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
}

// Sample class with two fields and four different indexes

// All indexes are defined as static fields and are strongly typed.
// The definitions would be considerably shorter and clearer in C#
// 3.0 thanks to lambda notation and better type inference.

// Note all code related to indexing can be kept in one place, due
// to the use of an indexing delegate, except for the call
// Index(this), which must appear at the end of every constructor.
// By using reflection and a naming convention for the indexing
// delegate, one might be able to avoid explicitly adding the
// indexes to the event in the static constructor.  However, the
// present setup can be implemented using a static aspect weaver
// such as yiihaw (Johansen and Spangenberg 2007), avoiding the use
// of runtime reflection.

// For use with LINQ-to-C5, one can use runtime reflection to find
// the indexes on a type, as the public static fields of type
// HashIndex or TreeIndex or IIndex.

public class PersonIndexedObjects2
{
    public string Name { get; }
    public int Date { get; } // YYYYMMDD as in 20070725

    // Defining the indexes
    public static readonly IIndex<string, PersonIndexedObjects2> NameIndex = new HashIndex<string, PersonIndexedObjects2>(p => p.Name);
    public static readonly IIndex<int, PersonIndexedObjects2> YearIndex = new HashIndex<int, PersonIndexedObjects2>(p => p.Date / 10000);
    public static readonly IIndex<int, PersonIndexedObjects2> DayIndex = new HashIndex<int, PersonIndexedObjects2>(p => p.Date % 100);
    public static readonly TreeIndex<Month, PersonIndexedObjects2> MonthIndex = new TreeIndex<Month, PersonIndexedObjects2>(p => (Month)(p.Date / 100 % 100));

    // Defining and initializing the indexing delegate
    private static event Action<PersonIndexedObjects2> Index;

    static PersonIndexedObjects2()
    {
        Index += NameIndex.Add;
        Index += YearIndex.Add;
        Index += DayIndex.Add;
        Index += MonthIndex.Add;
    }

    public PersonIndexedObjects2(string name, int date)
    {
        Name = name;
        Date = date;
        Index(this);
    }

    public override string ToString()
    {
        return $"{Name} ({Date})";
    }
}
