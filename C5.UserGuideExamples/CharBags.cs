// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example 2004-09-01

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.CharBags
//  dotnet run

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    class CharBags
    {
        private static readonly IHasher<TreeBag<char>> sequencedTreeBagHasher = new C5.HasherBuilder.SequencedHasher<TreeBag<char>, char>();
        private static readonly IHasher<TreeBag<char>> unsequencedTreeBagHasher = new C5.HasherBuilder.UnsequencedHasher<TreeBag<char>, char>();
        private static readonly IHasher<HashBag<char>> unsequencedHashBagHasher = new C5.HasherBuilder.UnsequencedHasher<HashBag<char>, char>();

        public static void Main(string[] args)
        {
        }

        public static void FindCollisions(SCG.IEnumerable<string> ss)
        {
            var occurrences = new HashBag<int>();
            foreach (string s in ss)
            {
                var tb = TreeBag(s);
                // HashBag<char> hb = HashBag(s);
                occurrences.Add(sequencedTreeBagHasher.GetHashCode(tb));
                // unsequencedTreeBagHasher.GetHashCode(tb);
                // unsequencedHashBagHasher.GetHashCode(hb);
            }
        }

        public static TreeBag<char> TreeBag(string s)
        {
            var anagram = new TreeBag<char>();
            foreach (char c in s)
            {
                anagram.Add(c);
            }
            return anagram;
        }

        public static HashBag<char> HashBag(string s)
        {
            var anagram = new HashBag<char>();
            foreach (char c in s)
            {
                anagram.Add(c);
            }
            return anagram;
        }
    }
}