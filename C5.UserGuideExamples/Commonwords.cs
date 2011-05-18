/*
 Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
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
