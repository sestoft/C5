// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Antipatterns 2004-12-29

// Compile with 
//   csc /r:C5.dll Antipatterns.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace Antipatterns
{
    class Antipatterns
    {
        public static void Main(String[] args)
        {
            ModifyInner();
            DontModifyInner();
        }

        // Anti-pattern: modifying an inner collection while it is a
        // member of an outer one may cause it to be lost from the outer
        // collection.

        private static void ModifyInner()
        {
            Console.WriteLine("\nAnti-pattern: Add to outer, modify, lose");
            ICollection<ISequenced<int>> outer = new HashSet<ISequenced<int>>();
            for (int i = 0; i < 100; i++)
            {
                ISequenced<int> inner = new TreeSet<int>();
                inner.Add(i); inner.Add(i + 1);
                outer.Add(inner);
            }
            ISequenced<int>
              inner1 = new TreeSet<int>(),
              inner2 = new TreeSet<int>(),
              inner3 = new TreeSet<int>();
            inner1.AddAll(new[] { 2, 3, 5, 7, 11 });
            inner2.AddAll(inner1); inner2.Add(13);
            inner3.AddAll(inner1);
            outer.Add(inner1);
            Console.WriteLine("inner1 in outer: {0}", outer.Contains(inner1));
            Console.WriteLine("inner2 in outer: {0}", outer.Contains(inner2));
            Console.WriteLine("inner3 in outer: {0}", outer.Contains(inner3));
            inner1.Add(13);
            Console.WriteLine("inner1 equals inner2: {0}",
                              outer.EqualityComparer.Equals(inner1, inner2));
            Console.WriteLine("inner1 equals inner3: {0}",
                              outer.EqualityComparer.Equals(inner1, inner3));
            Console.WriteLine("inner1 in outer: {0}", outer.Contains(inner1));
            Console.WriteLine("inner2 in outer: {0}", outer.Contains(inner2));
            Console.WriteLine("inner3 in outer: {0}", outer.Contains(inner3));
            Console.WriteLine("outer.Count: {0}", outer.Count);
        }

        private static void DontModifyInner()
        {
            Console.WriteLine("\nMake a snapshot and add it to outer");
            ICollection<ISequenced<int>> outer = new HashSet<ISequenced<int>>();
            for (int i = 0; i < 100; i++)
            {
                ISequenced<int> inner = new TreeSet<int>();
                inner.Add(i); inner.Add(i + 1);
                outer.Add(inner);
            }
            IPersistentSorted<int>
              inner1 = new TreeSet<int>(),
              inner2 = new TreeSet<int>(),
              inner3 = new TreeSet<int>();
            inner1.AddAll(new[] { 2, 3, 5, 7, 11 });
            inner2.AddAll(inner1); inner2.Add(13);
            inner3.AddAll(inner1);
            // Take a snapshot and add it to outer:
            outer.Add(inner1.Snapshot());
            Console.WriteLine("inner1 in outer: {0}", outer.Contains(inner1));
            Console.WriteLine("inner2 in outer: {0}", outer.Contains(inner2));
            Console.WriteLine("inner3 in outer: {0}", outer.Contains(inner3));
            inner1.Add(13);
            Console.WriteLine("inner1 equals inner2: {0}",
                              outer.EqualityComparer.Equals(inner1, inner2));
            Console.WriteLine("inner1 equals inner3: {0}",
                              outer.EqualityComparer.Equals(inner1, inner3));
            Console.WriteLine("inner1 in outer: {0}", outer.Contains(inner1));
            Console.WriteLine("inner2 in outer: {0}", outer.Contains(inner2));
            Console.WriteLine("inner3 in outer: {0}", outer.Contains(inner3));
            Console.WriteLine("outer.Count: {0}", outer.Count);
        }
    }
}
