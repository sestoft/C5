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

// C5 example: Find and print the most common words in a text file. 
// Programming pearl by D.E. Knuth in CACM 29 (June 1986) 471-483.

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.CommonWords
//  dotnet run

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace C5.UserGuideExamples
{
    class CommonWords
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Commonwords <maxwords> <filename>\n");
            }
            else
            {
                PrintMostCommon(int.Parse(args[0]), args[1]);
            }
        }

        static void PrintMostCommon(int maxWords, string filename)
        {
            var wordbag = new HashBag<string>();
            var delimiter = new Regex("[^a-zA-Z0-9]+");
            using (var reader = File.OpenText(filename))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    foreach (var s in delimiter.Split(line))
                    {
                        if (s != "")
                        {
                            wordbag.Add(s);
                        }
                    }
                }
            }
            var frequency = wordbag.ItemMultiplicities().ToArray();

            // Lexicographic ordering: decreasing frequency, then increasing string
            var comparer = ComparerFactory<KeyValuePair<string, int>>.CreateComparer(
                    (p1, p2) =>
                    {
                        var major = p2.Value.CompareTo(p1.Value);
                        return major != 0 ? major : p1.Key.CompareTo(p2.Key);
                    });
            Sorting.IntroSort(frequency, 0, frequency.Length, comparer);

            var stop = Math.Min(frequency.Length, maxWords);

            for (var i = 0; i < stop; i++)
            {
                var p = frequency[i];
                Console.WriteLine("{0,4} occurrences of {1}", p.Value, p.Key);
            }
        }
    }
}
