// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: File index: read a text file, build and print a list of
// words and the line numbers (without duplicates) on which they occur.

// Compile with 
//   csc /r:C5.dll Fileindex.cs 

using System;                           // Console
using System.IO;                        // StreamReader, TextReader
using System.Text.RegularExpressions;   // Regex
using C5;                               // IDictionary, TreeDictionary, TreeSet

namespace FileIndex
{
  class Fileindex
  {
    static void Main(String[] args)
    {
      if (args.Length != 1)
        Console.WriteLine("Usage: Fileindex <filename>\n");
      else
      {
        IDictionary<String, TreeSet<int>> index = IndexFile(args[0]);
        PrintIndex(index);
      }
    }

    static IDictionary<String, TreeSet<int>> IndexFile(String filename)
    {
      IDictionary<String, TreeSet<int>> index = new TreeDictionary<String, TreeSet<int>>();
      Regex delim = new Regex("[^a-zA-Z0-9]+");
      using (TextReader rd = new StreamReader(filename))
      {
        int lineno = 0;
        for (String line = rd.ReadLine(); line != null; line = rd.ReadLine())
        {
          String[] res = delim.Split(line);
          lineno++;
          foreach (String s in res)
            if (s != "")
            {
              if (!index.Contains(s))
                index[s] = new TreeSet<int>();
              index[s].Add(lineno);
            }
        }
      }
      return index;
    }

    static void PrintIndex(IDictionary<String, TreeSet<int>> index)
    {
      foreach (String word in index.Keys)
      {
        Console.Write("{0}: ", word);
        foreach (int ln in index[word])
          Console.Write("{0} ", ln);
        Console.WriteLine();
      }
    }
  }
}