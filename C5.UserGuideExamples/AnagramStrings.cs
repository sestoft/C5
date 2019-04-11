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

// C5 example: anagrams represented as sorted strings 2004-08-26

// To represent an anagram class, use a string containing the sorted
// characters of a word.  

// This is faster than a TreeBag<char> because the words and hence
// bags are small.  Takes 15 CPU seconds and 138 MB RAM to find the
// 26,058 anagram classes among 347,000 distinct words.

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.AnagramStrings
//  dotnet run

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    class AnagramStrings
    {
        static void Main(string[] args)
        {
            var ss = args.Length == 1 ? ReadFileWords(args[0]) : args;

            var sw = Stopwatch.StartNew();
            var classes = AnagramClasses(ss);
            var count = 0;

            foreach (var anagramClass in classes)
            {
                count++;
                foreach (string s in anagramClass)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"{count} anagram classes");
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        // Read words from a file
        static SCG.IEnumerable<string> ReadFileWords(string filename)
        {
            var delimiter = new Regex("[^a-zæøåA-ZÆØÅ0-9-]+");
            using (var reader = File.OpenText(filename))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    foreach (var s in delimiter.Split(line))
                    {
                        if (s != "")
                        {
                            yield return s.ToLower();
                        }
                    }
                }
            }
        }

        // From an anagram point of view, a word is just a bag of characters.
        static CharBag AnagramClass(string s)
        {
            return new CharBag(s);
        }

        // Given a sequence of strings, return all non-trivial anagram classes   

        static SCG.IEnumerable<SCG.IEnumerable<string>> AnagramClasses(SCG.IEnumerable<string> ss)
        {
            var classes = new TreeDictionary<CharBag, HashSet<string>>();
            foreach (string s in ss)
            {
                var anagram = AnagramClass(s);
                if (!classes.Find(ref anagram, out HashSet<string> anagramClass))
                {
                    classes[anagram] = anagramClass = new HashSet<string>();
                }
                anagramClass.Add(s);
            }
            foreach (HashSet<string> anagramClass in classes.Values)
            {
                if (anagramClass.Count > 1) // && anagramClass.Exists(delegate(string s) { return !s.EndsWith("s"); }))
                {
                    yield return anagramClass;
                }
            }
        }
    }

    // A bag of characters is represented as a sorted string of the
    // characters, with multiplicity.  Since natural language words are
    // short, the bags are small, so this is vastly better than
    // representing character bags using HashBag<char> or TreeBag<char>
    class CharBag : IComparable<CharBag>
    {
        private readonly string _contents; // The bag's characters, sorted, with multiplicity

        public CharBag(string s)
        {
            var chars = s.ToCharArray();
            Array.Sort(chars);
            _contents = new string(chars);
        }

        public override int GetHashCode()
        {
            return _contents.GetHashCode();
        }

        public bool Equals(CharBag that)
        {
            return _contents.Equals(that._contents);
        }

        public int CompareTo(CharBag that)
        {
            return _contents.CompareTo(that._contents);
        }
    }
}