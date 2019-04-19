// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Keyword recognition 2004-12-20

// Compile with 
//   csc /r:C5.dll KeywordRecognition.cs 

using System;
using System.Diagnostics;
using C5;
using SCG = System.Collections.Generic;

namespace KeywordRecognition
{

    class KeywordRecognition
    {
        // Array of 77 keywords:

        static readonly String[] keywordArray = 
    { "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
      "char", "checked", "class", "const", "continue", "decimal", "default",
      "delegate", "do", "double", "else", "enum", "event", "explicit",
      "extern", "false", "finally", "fixed", "float", "for", "foreach",
      "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
      "lock", "long", "namespace", "new", "null", "object", "operator",
      "out", "override", "params", "private", "protected", "public",
      "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
      "stackalloc", "static", "string", "struct", "switch", "this", "throw",
      "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
      "ushort", "using", "virtual", "void", "volatile", "while" };

        private static readonly ICollection<String> kw1;

        private static readonly ICollection<String> kw2;

        private static readonly ICollection<String> kw3;

        private static readonly SCG.IDictionary<String, bool> kw4 =
          new SCG.Dictionary<String, bool>();


        class SC : SCG.IComparer<string>
        {
            public int Compare(string a, string b)
            {
                return StringComparer.InvariantCulture.Compare(a, b);
            }
        }

        class SH : SCG.IEqualityComparer<string>
        {
            public int GetHashCode(string item)
            {
                return item.GetHashCode();
            }

            public bool Equals(string i1, string i2)
            {
                return i1 == null ? i2 == null : i1.Equals(i2, StringComparison.InvariantCulture);
            }
        }

        static KeywordRecognition()
        {
            kw1 = new HashSet<String>();
            kw1.AddAll(keywordArray);
            kw2 = new TreeSet<String>(new SC());
            kw2.AddAll(keywordArray);
            kw3 = new SortedArray<String>(new SC());
            kw3.AddAll(keywordArray);
            kw4 = new SCG.Dictionary<String, bool>();
            foreach (String keyword in keywordArray)
                kw4.Add(keyword, false);
        }

        public static bool IsKeyword1(String s)
        {
            return kw1.Contains(s);
        }

        public static bool IsKeyword2(String s)
        {
            return kw2.Contains(s);
        }

        public static bool IsKeyword3(String s)
        {
            return kw3.Contains(s);
        }

        public static bool IsKeyword4(String s)
        {
            return kw4.ContainsKey(s);
        }

        public static bool IsKeyword5(String s)
        {
            return Array.BinarySearch(keywordArray, s) >= 0;
        }

        public static void Main(String[] args)
        {
            if (args.Length != 2)
                Console.WriteLine("Usage: KeywordRecognition <iterations> <word>\n");
            else
            {
                int count = int.Parse(args[0]);
                String id = args[1];

                {
                    Console.Write("HashSet.Contains ");
                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < count; i++)
                        IsKeyword1(id);
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                }

                {
                    Console.Write("TreeSet.Contains ");
                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < count; i++)
                        IsKeyword2(id);
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                }

                {
                    Console.Write("SortedArray.Contains ");
                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < count; i++)
                        IsKeyword3(id);
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                }

                {
                    Console.Write("SCG.Dictionary.ContainsKey ");
                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < count; i++)
                        IsKeyword4(id);
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                }

                {
                    Console.Write("Array.BinarySearch ");
                    var sw = Stopwatch.StartNew();
                    for (int i = 0; i < count; i++)
                        IsKeyword5(id);
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                }
            }
        }
    }
}
