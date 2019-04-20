// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: collections of collections 2004-11-16

// Compile with 
//   csc /r:C5.dll CollectionCollection.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace CollectionCollection
{
    class MyTest
    {
        private static IList<int> col1 = new LinkedList<int>(),
          col2 = new LinkedList<int>(), col3 = new LinkedList<int>();

        public static void Main(String[] args)
        {
            ListEqualityComparers();
            IntSetSet();
            CharBagSet();
        }

        public static void ListEqualityComparers()
        {
            col1.AddAll(new[] { 7, 9, 13 });
            col2.AddAll(new[] { 7, 9, 13 });
            col3.AddAll(new[] { 9, 7, 13 });

            // Default equality and hasher == sequenced equality and hasher
            HashSet<IList<int>> hs1 = new HashSet<IList<int>>();
            Equalities("Default equality (sequenced equality)", hs1.EqualityComparer);
            hs1.Add(col1); hs1.Add(col2); hs1.Add(col3);
            Console.WriteLine("hs1.Count = {0}", hs1.Count);

            // Sequenced equality and hasher 
            SCG.IEqualityComparer<IList<int>> seqEqualityComparer
              = SequencedCollectionEqualityComparer<IList<int>, int>.Default;
            HashSet<IList<int>> hs2 = new HashSet<IList<int>>(seqEqualityComparer);
            Equalities("Sequenced equality", hs2.EqualityComparer);
            hs2.Add(col1); hs2.Add(col2); hs2.Add(col3);
            Console.WriteLine("hs2.Count = {0}", hs2.Count);

            // Unsequenced equality and hasher
            SCG.IEqualityComparer<IList<int>> unseqEqualityComparer
              = UnsequencedCollectionEqualityComparer<IList<int>, int>.Default;
            HashSet<IList<int>> hs3 = new HashSet<IList<int>>(unseqEqualityComparer);
            Equalities("Unsequenced equality", hs3.EqualityComparer);
            hs3.Add(col1); hs3.Add(col2); hs3.Add(col3);
            Console.WriteLine("hs3.Count = {0}", hs3.Count);
        }

        public static void Equalities(String msg, SCG.IEqualityComparer<IList<int>> equalityComparer)
        {
            Console.WriteLine("\n{0}:", msg);
            Console.Write("Equals(col1,col2)={0,-5}; ", equalityComparer.Equals(col1, col2));
            Console.Write("Equals(col1,col3)={0,-5}; ", equalityComparer.Equals(col1, col3));
            Console.WriteLine("Equals(col2,col3)={0,-5}", equalityComparer.Equals(col2, col3));
        }

        public static void IntSetSet()
        {
            ICollection<ISequenced<int>> outer = new HashSet<ISequenced<int>>();
            int[] ss = { 2, 3, 5, 7 };
            TreeSet<int> inner = new TreeSet<int>();
            outer.Add(inner.Snapshot());
            foreach (int i in ss)
            {
                inner.Add(i);
                outer.Add(inner.Snapshot());
            }
            foreach (ISequenced<int> s in outer)
            {
                int sum = 0;
                s.Apply(delegate(int x) { sum += x; });
                Console.WriteLine("Set has {0} elements and sum {1}", s.Count, sum);
            }
        }

        public static void CharBagSet()
        {
            String text =
              @"three sorted streams aligned by leading masters are stored 
          there; an integral triangle ends, and stable tables keep;
          being alert, they later reread the logarithm, peek at the
          recent center, then begin to send their reader algorithm.";
            String[] words = text.Split(' ', '\n', '\r', ';', ',', '.');
            ICollection<ICollection<char>> anagrams
              = new HashSet<ICollection<char>>();
            int count = 0;
            foreach (String word in words)
            {
                if (word != "")
                {
                    count++;
                    HashBag<char> anagram = new HashBag<char>();
                    anagram.AddAll(word.ToCharArray());
                    anagrams.Add(anagram);
                }
            }
            Console.WriteLine("Found {0} anagrams", count - anagrams.Count);
        }
    }
}
