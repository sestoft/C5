// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Find and print the most common words in a text file. 
// Programming pearl by D.E. Knuth in CACM 29 (June 1986) 471-483.

// Compile with 
//   csc /r:C5.dll Commonwords.cs 

using System;                           // Console
using System.IO;                        // StreamReader, TextReader
using System.Text.RegularExpressions;   // Regex
using C5;                               // IDictionary, TreeDictionary, TreeSet
using SCG = System.Collections.Generic; // IComparer

namespace Commonwords {
  class Commonwords {
    static void Main(String[] args) {
      if (args.Length != 2)
        Console.WriteLine("Usage: Commonwords <maxwords> <filename>\n");
      else 
        PrintMostCommon(int.Parse(args[0]), args[1]);
    }

    static void PrintMostCommon(int maxWords, String filename) {
      ICollection<String> wordbag = new HashBag<String>();
      Regex delim = new Regex("[^a-zA-Z0-9]+");
      using (TextReader rd = new StreamReader(filename)) {
        for (String line = rd.ReadLine(); line != null; line = rd.ReadLine()) 
          foreach (String s in delim.Split(line)) 
            if (s != "")
              wordbag.Add(s);
      }
      KeyValuePair<String,int>[] frequency 
        = wordbag.ItemMultiplicities().ToArray();
      // Sorting.IntroSort(frequency, 0, frequency.Length, new FreqOrder());
      Sorting.IntroSort(frequency, 0, frequency.Length, 
          new DelegateComparer<KeyValuePair<String,int>>
                        ((p1, p2) => 
                        { 
                          int major = p2.Value.CompareTo(p1.Value);
                          return major != 0 ? major : p1.Key.CompareTo(p2.Key);
                        }));
      int stop = Math.Min(frequency.Length, maxWords);
      for (int i=0; i<stop; i++) {
        KeyValuePair<String,int> p = frequency[i];
        Console.WriteLine("{0,4} occurrences of {1}", p.Value, p.Key);
      }
    }

    // Lexicographic ordering: decreasing frequency, then increasing string
    
    class FreqOrder : SCG.IComparer<KeyValuePair<String,int>> { 
      public int Compare(KeyValuePair<String,int> p1, 
                         KeyValuePair<String,int> p2) {
        int major = p2.Value.CompareTo(p1.Value);
        return major != 0 ? major : p1.Key.CompareTo(p2.Key);
      }
    }
  }
}
