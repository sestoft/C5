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

// C5 example: anagrams 2004-08-08, 2004-11-16

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.AnagramHashBag
//  dotnet run

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    class AnagramHashBag
    {
        static void Main(string[] args)
        {
            var ss = args.Length == 2 ? ReadFileWords(args[0], int.Parse(args[1])) : args;

            foreach (var s in FirstAnagramOnly(ss))
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("===");

            var sw = Stopwatch.StartNew();
            var classes = AnagramClasses(ss);
            var count = 0;

            foreach (var anagramClass in classes)
            {
                count++;
                foreach (var s in anagramClass)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"{count} non-trivial anagram classes");

            sw.Stop();

            Console.WriteLine(sw.Elapsed);
        }

        // Read words at most n words from a file
        static SCG.IEnumerable<string> ReadFileWords(string filename, int n)
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
                        if (--n == 0)
                        {
                            yield break;
                        }
                    }
                }
            }
        }

        // From an anagram point of view, a word is just a bag of
        // characters.  So an anagram class is represented as HashBag<char>
        // which permits fast equality comparison -- we shall use them as
        // elements of hash sets or keys in hash maps.
        static HashBag<char> AnagramClass(string s)
        {
            var anagram = new HashBag<char>();

            foreach (char c in s)
            {
                anagram.Add(c);
            }

            return anagram;
        }

        // Given a sequence of strings, return only the first member of each
        // anagram class.
        static SCG.IEnumerable<string> FirstAnagramOnly(SCG.IEnumerable<string> ss)
        {
            var anagrams = new HashSet<HashBag<char>>();

            foreach (string s in ss)
            {
                var anagram = AnagramClass(s);

                if (!anagrams.Contains(anagram))
                {
                    anagrams.Add(anagram);

                    yield return s;
                }
            }
        }

        // Given a sequence of strings, return all non-trivial anagram
        // classes.  

        // Using HashBag<char> and an unsequenced equalityComparer, this performs as
        // follows on 1600 MHz Mobile P4 and .Net 2.0 beta 1 (wall-clock
        // time):
        //  50 000 words  2 822 classes   2.0 sec
        // 100 000 words  5 593 classes   4.3 sec
        // 200 000 words 11 705 classes   8.8 sec
        // 300 000 words 20 396 classes  52.0 sec includes swapping
        // 347 165 words 24 428 classes 146.0 sec includes swapping

        // The maximal memory consumption is less than 180 MB.
        static SCG.IEnumerable<SCG.IEnumerable<string>> AnagramClasses(SCG.IEnumerable<string> ss)
        {
            var classes = new HashDictionary<HashBag<char>, TreeSet<string>>();

            foreach (var s in ss)
            {
                var anagram = AnagramClass(s);

                if (!classes.Find(ref anagram, out var anagramClass))
                {
                    classes[anagram] = anagramClass = new TreeSet<string>();
                }

                anagramClass.Add(s);
            }

            foreach (var anagramClass in classes.Values)
            {
                if (anagramClass.Count > 1)
                {
                    yield return anagramClass;
                }
            }
        }
    }
}
