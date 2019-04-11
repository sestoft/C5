/*
 Copyright (c) 2003-2019 Niels Kokholm, Peter Sestoft, and Rasmus Lystrøm
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

// C5 example 2004-09-01

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.BugHashedArray
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