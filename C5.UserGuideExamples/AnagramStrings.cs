// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: anagrams represented as sorted strings 2004-08-26

// To represent an anagram class, use a string containing the sorted
// characters of a word.  

// This is faster than a TreeBag<char> because the words and hence
// bags are small.  Takes 15 CPU seconds and 138 MB RAM to find the
// 26,058 anagram classes among 347,000 distinct words.

// Compile with 
//   csc /r:C5.dll Anagrams.cs 

using System;
using System.Diagnostics;
using System.IO;                        // StreamReader, TextReader
using System.Text;			// Encoding
using System.Text.RegularExpressions;   // Regex
using C5;
using SCG = System.Collections.Generic;

namespace AnagramStrings
{
    class MyTest
    {
        public static void Main(String[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("iso-8859-1");
            SCG.IEnumerable<String> ss;
            if (args.Length == 1)
                ss = ReadFileWords(args[0]);
            else
                ss = args;

            var sw = Stopwatch.StartNew();
            SCG.IEnumerable<SCG.IEnumerable<String>> classes = AnagramClasses(ss);
            int count = 0;
            foreach (SCG.IEnumerable<String> anagramClass in classes)
            {
                count++;
                // foreach (String s in anagramClass) 
                //   Console.Write(s + " ");
                // Console.WriteLine();
            }
            Console.WriteLine("{0} anagram classes", count);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        // Read words from a file

        public static SCG.IEnumerable<String> ReadFileWords(String filename)
        {
            Regex delim = new Regex("[^a-zæøåA-ZÆØÅ0-9-]+");
            using (TextReader rd = new StreamReader(filename, Encoding.GetEncoding("iso-8859-1")))
            {
                for (String line = rd.ReadLine(); line != null; line = rd.ReadLine())
                    foreach (String s in delim.Split(line))
                        if (s != "")
                            yield return s.ToLower();
            }
        }

        // From an anagram point of view, a word is just a bag of characters.

        public static CharBag AnagramClass(String s)
        {
            return new CharBag(s);
        }

        // Given a sequence of strings, return all non-trivial anagram classes   

        public static SCG.IEnumerable<SCG.IEnumerable<String>> AnagramClasses(SCG.IEnumerable<String> ss)
        {
            IDictionary<CharBag, HashSet<String>> classes
              = new TreeDictionary<CharBag, HashSet<String>>();
            foreach (String s in ss)
            {
                CharBag anagram = AnagramClass(s);
                if (!classes.Find(ref anagram, out HashSet<string> anagramClass))
                    classes[anagram] = anagramClass = new HashSet<String>();
                anagramClass.Add(s);
            }
            foreach (HashSet<String> anagramClass in classes.Values)
                if (anagramClass.Count > 1
              ) // && anagramClass.Exists(delegate(String s) { return !s.EndsWith("s"); }))
                    yield return anagramClass;
        }
    }

    // A bag of characters is represented as a sorted string of the
    // characters, with multiplicity.  Since natural language words are
    // short, the bags are small, so this is vastly better than
    // representing character bags using HashBag<char> or TreeBag<char>

    class CharBag : IComparable<CharBag>
    {
        private readonly String contents; // The bag's characters, sorted, with multiplicity

        public CharBag(String s)
        {
            char[] chars = s.ToCharArray();
            Array.Sort(chars);
            this.contents = new String(chars);
        }

        public override int GetHashCode()
        {
            return contents.GetHashCode();
        }

        public bool Equals(CharBag that)
        {
            return this.contents.Equals(that.contents);
        }

        public int CompareTo(CharBag that)
        {
            return this.contents.CompareTo(that.contents);
        }
    }
}