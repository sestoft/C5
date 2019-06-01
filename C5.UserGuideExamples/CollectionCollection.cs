// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: collections of collections 2004-11-16

// Compile with
//   csc /r:netstandard.dll /r:C5.dll CollectionCollection.cs

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    internal class CollectionCollection
    {
        private static readonly IList<int> _col1 = new LinkedList<int>();
        private static readonly IList<int> _col2 = new LinkedList<int>();
        private static readonly IList<int> _col3 = new LinkedList<int>();

        public static void Main()
        {
            ListEqualityComparers();
            IntSetSet();
            CharBagSet();
        }

        public static void ListEqualityComparers()
        {
            _col1.AddAll(new[] { 7, 9, 13 });
            _col2.AddAll(new[] { 7, 9, 13 });
            _col3.AddAll(new[] { 9, 7, 13 });

            // Default equality and hasher == sequenced equality and hasher
            var hs1 = new HashSet<IList<int>>();
            Equalities("Default equality (sequenced equality)", hs1.EqualityComparer);
            hs1.Add(_col1); hs1.Add(_col2); hs1.Add(_col3);
            Console.WriteLine("hs1.Count = {0}", hs1.Count);

            // Sequenced equality and hasher 
            var seqEqualityComparer = SequencedCollectionEqualityComparer<IList<int>, int>.Default;
            var hs2 = new HashSet<IList<int>>(seqEqualityComparer);
            Equalities("Sequenced equality", hs2.EqualityComparer);
            hs2.Add(_col1); hs2.Add(_col2); hs2.Add(_col3);
            Console.WriteLine("hs2.Count = {0}", hs2.Count);

            // Unsequenced equality and hasher
            var unseqEqualityComparer = UnsequencedCollectionEqualityComparer<IList<int>, int>.Default;
            var hs3 = new HashSet<IList<int>>(unseqEqualityComparer);
            Equalities("Unsequenced equality", hs3.EqualityComparer);
            hs3.Add(_col1); hs3.Add(_col2); hs3.Add(_col3);
            Console.WriteLine("hs3.Count = {0}", hs3.Count);
        }

        public static void Equalities(string msg, SCG.IEqualityComparer<IList<int>> equalityComparer)
        {
            Console.WriteLine("\n{0}:", msg);
            Console.Write("Equals(col1,col2)={0,-5}; ", equalityComparer.Equals(_col1, _col2));
            Console.Write("Equals(col1,col3)={0,-5}; ", equalityComparer.Equals(_col1, _col3));
            Console.WriteLine("Equals(col2,col3)={0,-5}", equalityComparer.Equals(_col2, _col3));
        }

        public static void IntSetSet()
        {
            var outer = new HashSet<ISequenced<int>>();
            int[] ss = { 2, 3, 5, 7 };
            var inner = new TreeSet<int>();
            outer.Add(inner.Snapshot());

            foreach (int i in ss)
            {
                inner.Add(i);
                outer.Add(inner.Snapshot());
            }
            foreach (ISequenced<int> s in outer)
            {
                int sum = 0;
                s.Apply(delegate (int x) { sum += x; });
                Console.WriteLine("Set has {0} elements and sum {1}", s.Count, sum);
            }
        }

        public static void CharBagSet()
        {
            var text = @"three sorted streams aligned by leading masters are stored 
                         there; an integral triangle ends, and stable tables keep;
                         being alert, they later reread the logarithm, peek at the
                         recent center, then begin to send their reader algorithm.";
            var words = text.Split(' ', '\n', '\r', ';', ',', '.');
            var anagrams = new HashSet<ICollection<char>>();
            int count = 0;
            foreach (var word in words)
            {
                if (word != "")
                {
                    count++;
                    var anagram = new HashBag<char>();
                    anagram.AddAll(word.ToCharArray());
                    anagrams.Add(anagram);
                }
            }
            Console.WriteLine("Found {0} anagrams", count - anagrams.Count);
        }
    }
}
