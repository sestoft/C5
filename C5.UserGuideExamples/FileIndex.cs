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

// C5 example: File index: read a text file, build and print a list of
// words and the line numbers (without duplicates) on which they occur.

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.FileIndex
//  dotnet run

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace C5.UserGuideExamples
{
    class FileIndex
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Fileindex <filename>");
            }
            else
            {
                var index = IndexFile(args[0]);
                PrintIndex(index);
            }
        }

        static IDictionary<string, TreeSet<int>> IndexFile(string filename)
        {
            var index = new TreeDictionary<string, TreeSet<int>>();
            var delimiter = new Regex("[^a-zA-Z0-9]+");
            using (var reader = File.OpenText(filename))
            {
                var lineNumber = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var res = delimiter.Split(line);
                    lineNumber++;
                    foreach (var s in res)
                    {
                        if (s != "")
                        {
                            if (!index.Contains(s))
                            {
                                index[s] = new TreeSet<int>();
                            }
                            index[s].Add(lineNumber);
                        }
                    }
                }
            }
            return index;
        }

        static void PrintIndex(IDictionary<string, TreeSet<int>> index)
        {
            foreach (var word in index.Keys)
            {
                Console.Write($"{word}: ");
                foreach (var ln in index[word])
                {
                    Console.Write($"{ln} ");
                }
                Console.WriteLine();
            }
        }
    }
}