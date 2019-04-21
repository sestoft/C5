// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// IndexedObjects.cs sketch 2007-07-26 

// Other approaches: Define an Indexed<T> class to which indexers can
// be attached.  An object can add (or remove) itself to the indexers
// that are attached to the Indexed class at the time of addition (or
// removal).

// Maintaining multiple indices on objects, each index defined by a
// delegate.

// Compile with 
//   csc /r:C5.dll IndexedObjects.cs 

using System;                           // Console
using System.Text;                      // StringBuilder
using C5; 
using SCG = System.Collections.Generic;

namespace IndexedObjects {
  static class IndexedObjectsMain {
    static void Main(String[] args) {
      Indexed<Person> persons 
        = new Indexed<Person>
        (new IndexMaker<Person,String>
         ("name", delegate(Person p) { return p.name; }),
         new IndexMaker<Person,int>
         ("year", delegate(Person p) { return p.date/10000; }),
         new IndexMaker<Person,int>
         ("day", delegate(Person p) { return p.date%100; }),
         new IndexMaker<Person,String>
         ("month", delegate(Person p) { return months[p.date/100%100-1]; })
         );
      persons.Add(new Person("Niels",   19470206));
      persons.Add(new Person("Lone",    19600810));
      persons.Add(new Person("Peter",   19620625));
      persons.Add(new Person("Carsten", 19640627));
      persons.Add(new Person("Hanne",   19641209));
      persons.Add(new Person("Dorte",   19660930));
      persons.Add(new Person("Dorte",   19610312));
      persons.Add(new Person("Jørgen",  19340930));
      persons.Add(new Person("Kirsten", 19360114));
      persons.Add(new Person("Henrik",  19360630));
      persons.Add(new Person("Lars",    19640625));
      persons.Add(new Person("Thora",   19091129));
      Console.WriteLine("Born in 1964:");
      foreach (Person p in persons["year"][1964]) 
        Console.WriteLine(p);
      Console.WriteLine("Born in June:");
      foreach (Person p in persons["month"]["Jun"]) 
        Console.WriteLine(p);
      Console.WriteLine("Named Dorte:");
      foreach (Person p in persons["name"]["Dorte"]) 
        Console.WriteLine(p);
      Console.WriteLine(persons);
    }
    
    private static readonly String[] months = { 
      "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
      "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" 
    };
  }

  public interface IIndexer<Q,R> { 
    ICollectionValue<R> this[Q x] { get; }
  }

  // An index maker has a name, it supports adding and removing items
  // from the index, and looking up items by key (here of type
  // Object).

  public abstract class IndexMaker<T> : IIndexer<Object, T> {
    public readonly String name;
    public abstract bool Add(T item);
    public abstract bool Remove(T item);
    public abstract ICollectionValue<T> this[Object key] { get; }

    public IndexMaker(String name) {
      this.name = name;
    }
  }

  // The implementation of an index maker consists of a function to
  // map an item of type T to the index key type Q, and a dictionary
  // that maps an index key (of type Q) to a set of items (each of
  // type T).

  public class IndexMaker<T,Q> : IndexMaker<T> 
    where T : class 
    where Q : IComparable<Q> 
  {
    private readonly Fun<T,Q> fun;
    private TreeDictionary<Q, HashSet<T>> dict;

    public IndexMaker(String name, Fun<T,Q> fun) : base(name) {
      this.fun = fun;
      dict = new TreeDictionary<Q, HashSet<T>>();
    }

    public override bool Add(T item) { 
      Q key = fun(item);
      if (!dict.Contains(key))
        dict.Add(key, new HashSet<T>(ReferenceEqualityComparer<T>.Default));
      return dict[key].Add(item);
    }

    public override bool Remove(T item) { 
      Q key = fun(item);
      if (!dict.Contains(key))
        return false;
      return dict[key].Remove(item);
    }

    public ICollectionValue<T> this[Q key] {
      get { 
        return dict[key];
      }
    }

    public override ICollectionValue<T> this[Object key] {
      get { 
        return dict[(Q)key];
      }
    }

    public override String ToString() {
      return dict.ToString();
    }
  }

  // Weakly typed implementation of multiple indexers on a class T.  

  // The implementation is an array of index makers, each consisting
  // of the index's name and its implementation which supports adding
  // and removing T objects, and looking up T objects by the key
  // relevant for that index.

  public class Indexed<T> where T : class {
    IndexMaker<T>[] indexMakers;

    public Indexed(params IndexMaker<T>[] indexMakers) {
      this.indexMakers = indexMakers;
    }
    
    public bool Add(T item) { 
      bool result = false;
      foreach (IndexMaker<T> indexMaker in indexMakers)
        result |= indexMaker.Add(item);
      return result;    
    }

    public bool Remove(T item) { 
      bool result = false;
      foreach (IndexMaker<T> indexMaker in indexMakers)
        result |= indexMaker.Remove(item);
      return result;    
    }

    public IIndexer<Object, T> this[String name] {
      get {
        foreach (IndexMaker<T> indexMaker in indexMakers)
          if (indexMaker.name == name) 
            return indexMaker;
        throw new Exception("Unknown index");
      }
    }

    // For debugging

    public override String ToString() {
      StringBuilder sb = new StringBuilder();
      foreach (IndexMaker<T> indexMaker in indexMakers) {
        sb.Append("\n----- ").Append(indexMaker.name).Append("-----\n");
        sb.Append(indexMaker);
      }
      return sb.ToString();
    }
  }

  // Sample class with two fields but many possible indexes
  
  public class Person {
    public readonly String name;
    public readonly int date; // YYYYMMDD as in 20070725

    public Person(String name, int date) {
      this.name = name;
      this.date = date;
    }

    public override String ToString() {
      return name + " (" + date + ")";
    }
  }

  // The interface of a strongly typed indexing of Person objects:
  // (Not yet used)

  public interface PersonIndexers {
    IIndexer<String, Person> Name { get; }
    IIndexer<int,    Person> Year { get; }
    IIndexer<int,    Person> Day { get; }
    IIndexer<String, Person> Month { get; }
  }
}
