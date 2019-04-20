// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: LockstepCollections: A group collections lockstep, so
// that they all contain the same items.  This is done by equipping
// them all with event handlers, so that when one collection is
// updated (item added or removed) then all the others are updated
// too.  The collections must be initially empty.

// The idea is to organize the N collections c0, ..., c(N-1) in a ring
// in which c(i) is followed by c((i+1)%N), and then each collection
// with event handlers that update the next collection.  The update to
// that collection in turn will cause its event listeners to update
// the next next collection, and so on.

// Event cycles are prevented by using a common counter "steps",
// initialized to the number N of collections, and reset to N by
// CollectionChanged events.  All other events decrement the counter
// and performing further updates only if it is positive.  Since the
// CollectionChanged events are fired only after all the other events,
// the counter will be reset only after all the updates have been
// performed, but then it will (superfluously) be reset N times.  None
// of this will work in a multithreaded setting, of course; but then
// even plain updates wouldn't either.

// If the (equality) comparer of one collection identifies more items
// than that of another collection, then the lockstep collections do
// not necessarily have the same number of items, even though they all
// started out empty.  This also means that it matters what collection
// one attempts to add an item to, and in what order the collections
// are chained.  If one adds a new item to a collection that contains
// one deemed equal to it, then nothing happens, the event is not
// raised, and the item will not be added to any of the collections.
// If instead one adds it to a collection where it is not deemed equal
// to an existing item, it will be added to that collection but
// possibly not to others.

// Compile with 
//   csc /r:C5.dll LockstepCollections.cs 

using System;                           // Console
using C5; 
using SCG = System.Collections.Generic;

namespace LockstepCollections
{
  static class LockstepCollections
  {
    static void Main(String[] args)
    {
      ICollection<Person> nameColl 
        = new HashSet<Person>(new Person.NameEqualityComparer());
      ICollection<Person> dateColl 
        = new TreeSet<Person>(new Person.DateComparer());
      MakeLockstep(nameColl, dateColl);
      Person p1 = new Person("Peter", 19620625), 
        p2 = new Person("Carsten", 19640627), 
        p3 = new Person("Carsten", 19640628);
      nameColl.Add(p1);      
      nameColl.Add(p2);
      dateColl.Add(p3);
      Console.WriteLine("dateColl = {0}", dateColl);
      Console.WriteLine("nameColl = {0}", nameColl);
      dateColl.Remove(p1);
      Console.WriteLine("dateColl = {0}", dateColl);
      Console.WriteLine("nameColl = {0}", nameColl);
      dateColl.Clear();
      Console.WriteLine("dateColl = {0}", dateColl);
      Console.WriteLine("nameColl = {0}", nameColl);
    }

    static void MakeLockstep<T>(params ICollection<T>[] colls)
    {
      // These will be captured in the event handler delegates below
      int N = colls.Length;
      int steps = N;
      for (int i=0; i<N; i++) {
        if (!colls[i].IsEmpty) 
          throw new ApplicationException("Non-empty collection");
      }
      for (int i=0; i<N; i++) {
        ICollection<T> thisColl = colls[i];
        ICollection<T> nextColl = colls[(i+1)%N];
        thisColl.CollectionChanged += 
          delegate(Object coll) {
            steps = N;
          };
        thisColl.CollectionCleared += 
          delegate(Object coll, ClearedEventArgs args) {
            // For now ignoring that the clearing may be partial
            if (--steps > 0) {
              nextColl.Clear();
            }
          };
        thisColl.ItemsAdded +=
          delegate(Object coll, ItemCountEventArgs<T> args) {
            // For now ignoring the multiplicity
            if (--steps > 0) {
              T item = args.Item;
              nextColl.FindOrAdd(ref item);
            }
          };
        thisColl.ItemsRemoved +=
          delegate(Object coll, ItemCountEventArgs<T> args) {
            // For now ignoring the multiplicity
            if (--steps > 0) {
              nextColl.Remove(args.Item);
            }
          };
      }
    }
  }

  public class Person {
    String name;
    int date;

    public Person(String name, int date) {
      this.name = name;
      this.date = date;
    }

    public class NameEqualityComparer : SCG.IEqualityComparer<Person> {
      public bool Equals(Person p1, Person p2) {
        return p1.name == p2.name;
      }
      public int GetHashCode(Person p) { 
        return p.name.GetHashCode();
      }
    }

    public class DateComparer : SCG.IComparer<Person> {
      public int Compare(Person p1, Person p2) { 
        return p1.date.CompareTo(p2.date);
      }
    }

    public override String ToString() {
      return name + " (" + date + ")";
    }
  }
}
