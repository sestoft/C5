// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 bugs 2007-09-16

// Compile with 
//   csc /r:C5.dll BugHashArray.cs 

using System;
using C5;

namespace Anagrams {
  class MyTest {
    public static void Main(String[] args) {
      HashedLinkedList<int> test = new HashedLinkedList<int>();
      for (int i = 0; i < 33; i++) {
        test.Add(i);
      } // for

      // Fails after 7 removals
      for (int i = 0; i < 33; i++) {
        Console.WriteLine(i);
        test.Remove(i);
      } // for

      // Fails after 23 removals
//        for (int i = 32; i >= 0; i--) {
//          Console.WriteLine(i);
//          test.Remove(i);
//        } // for
    }
  }
}
