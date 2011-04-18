/*
 Copyright (c) 2003-2007 Niels Kokholm and Peter Sestoft
*/

// IndexedObjects2.cs sketch 2007-07-28

// Maintaining multiple type safe indices on objects using the C5
// library.

// Compile with 
//   csc /r:C5.dll IndexedObjects2.cs 

using System;                           // Console
using System.Text;                      // StringBuilder
using C5; 
using SCG = System.Collections.Generic;

namespace IndexedObjects {
  static class IndexedObjectsMain {
    static void Main(String[] args) {
      // Create some Person objects:
      new Person("Niels",   19470206);
      new Person("Lone",    19600810);
      new Person("Peter",   19620625);
      new Person("Carsten", 19640627);
      new Person("Hanne",   19641209);
      new Person("Dorte",   19660930);
      new Person("Dorte",   19610312);
      new Person("Jørgen",  19340930);
      new Person("Kirsten", 19360114);
      new Person("Henrik",  19360630);
      new Person("Lars",    19640625);
      new Person("Thora",   19091129);
      // Try the indexers:
      Console.WriteLine("\nBorn in 1964:");
      foreach (Person p in Person.Year[1964]) 
        Console.WriteLine(p);
      Console.WriteLine("\nNamed Dorte:");
      foreach (Person p in Person.Name["Dorte"]) 
        Console.WriteLine(p);
      Console.WriteLine("\nBorn on the 30th:");
      foreach (Person p in Person.Day[30]) 
        Console.WriteLine(p);
      Console.WriteLine("\nBorn June-October:");
      foreach (Person p in Person.Month.RangeFromTo(Month.Jun, Month.Nov)) 
        Console.WriteLine(p);
    }
  }

  // The interface IIndex<Q,T> describes an index with keys of type Q
  // on items of type T:

  public interface IIndex<Q,T> { 
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

  public class HashIndex<Q,T> : IIndex<Q,T> where T : class {
    private readonly Fun<T,Q> toKey;
    private HashDictionary<Q, HashSet<T>> dict;

    public HashIndex(Fun<T,Q> toKey) {
      this.toKey = toKey;
      dict = new HashDictionary<Q, HashSet<T>>();
    }

    public void Add(T item) { 
      Q key = toKey(item);
      if (!dict.Contains(key))
        dict.Add(key, new HashSet<T>(ReferenceEqualityComparer<T>.Default));
      dict[key].Add(item);
    }

    public ICollection<T> this[Q key] {
      get { 
        return new GuardedCollection<T>(dict[key]);
      }
    }

    public override String ToString() {
      return dict.ToString();
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

  public class TreeIndex<Q,T> : IIndex<Q,T> where T : class {
    private readonly Fun<T,Q> toKey;
    private TreeDictionary<Q, HashSet<T>> dict;

    public TreeIndex(Fun<T,Q> toKey) {
      this.toKey = toKey;
      dict = new TreeDictionary<Q, HashSet<T>>();
    }

    public void Add(T item) { 
      Q key = toKey(item);
      if (!dict.Contains(key))
        dict.Add(key, new HashSet<T>(ReferenceEqualityComparer<T>.Default));
      dict[key].Add(item);
    }

    public ICollection<T> this[Q key] {
      get { 
        return new GuardedCollection<T>(dict[key]);
      }
    }

    // Bad implementation from a performance point of view: it may be
    // costly to build the result collection and possibly only its
    // size is ever used:

    public ICollection<T> RangeFromTo(Q x, Q y) {
      HashSet<T> result = new HashSet<T>(ReferenceEqualityComparer<T>.Default);
      foreach (KeyValuePair<Q,HashSet<T>> kv in dict.RangeFromTo(x, y))
	result.AddAll(kv.Value);
      return result;
    }

    public override String ToString() {
      return dict.ToString();
    }
  }

  public enum Month { 
    Jan=1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec 
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

  public class Person {
    public readonly String name;
    public readonly int date; // YYYYMMDD as in 20070725

    // Defining the indexes
    public static readonly IIndex<String, Person> Name 
      = new HashIndex<String, Person>(delegate(Person p) { return p.name; });
    public static readonly IIndex<int, Person> Year 
      = new HashIndex<int, Person>(delegate(Person p) { return p.date/10000; });
    public static readonly IIndex<int, Person> Day 
      = new HashIndex<int, Person>(delegate(Person p) { return p.date%100; });
    public static readonly TreeIndex<Month, Person> Month 
      = new TreeIndex<Month, Person>
        (delegate(Person p) { return (Month)(p.date/100%100); });

    // Defining and initializing the indexing delegate
    private static event Act<Person> Index;
    static Person() { 
      Index += Name.Add;
      Index += Year.Add;
      Index += Day.Add;
      Index += Month.Add;
    }

    public Person(String name, int date) {
      this.name = name;
      this.date = date;
      Index(this);
    }

    public override String ToString() {
      return name + " (" + date + ")";
    }
  }
}
