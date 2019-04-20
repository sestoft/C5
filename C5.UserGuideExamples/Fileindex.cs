// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

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
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
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
                foreach (var line in index[word])
                {
                    Console.Write($"{line} ");
                }
                Console.WriteLine();
            }
        }
    }
}