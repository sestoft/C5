// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: ReadOnlyPatterns.cs for pattern chapter

// Compile with 
//   csc /r:C5.dll ReadOnlyPatterns.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

#pragma warning disable IDE0060

namespace ReadOnlyPatterns {
  class ReadOnlyPatterns {
    public static void Main(String[] args) {
      GuardHashSet<int>();
      GuardTreeSet<int>();
      GuardList<int>();
      GuardHashDictionary<int,int>();
      GuardSortedDictionary<int,int>();
    }

    // Read-only access to a hash-based collection
    
    static void GuardHashSet<T>() {
      ICollection<T> coll = new HashSet<T>();
      DoWork(new GuardedCollection<T>(coll));
    }

 
    static void DoWork<T>(ICollection<T> gcoll) { 
      // Use gcoll ... 
    }

        // Read-only access to an indexed sorted collection

        static void GuardTreeSet<T>() {
      IIndexedSorted<T> coll = new TreeSet<T>(); 
      DoWork(new GuardedIndexedSorted<T>(coll));
    }
    
    static void DoWork<T>(IIndexedSorted<T> gcoll) { 
      // Use gcoll ...
    }

    // Read-only access to a list

    static void GuardList<T>() {
      IList<T> coll = new ArrayList<T>(); 
      DoWork(new GuardedList<T>(coll));
    }
    
    static void DoWork<T>(IList<T> gcoll) { 
      // Use gcoll ...
    }

    // Read-only access to a dictionary

    static void GuardHashDictionary<K,V>() {
      IDictionary<K,V> dict = new HashDictionary<K,V>(); 
      DoWork(new GuardedDictionary<K,V>(dict));
    }
    
    static void DoWork<K,V>(IDictionary<K,V> gdict) { 
      // Use gdict ...
    }

    // Read-only access to a sorted dictionary

    static void GuardSortedDictionary<K,V>() {
      ISortedDictionary<K,V> dict = new TreeDictionary<K,V>(); 
      DoWork(new GuardedSortedDictionary<K,V>(dict));
    }
    
    static void DoWork<K,V>(ISortedDictionary<K,V> gdict) { 
      // Use gdict ...
    }
  }
}

#pragma warning restore IDE0060