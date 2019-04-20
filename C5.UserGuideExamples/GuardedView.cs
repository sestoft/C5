// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: 2006-06-26

// Compile with 
//   csc /r:C5.dll Anagrams.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace MyTest {
  class MyTest {
    public static void Main(String[] args) {
      IList<int> list1 = new ArrayList<int>();
      list1.AddAll(new int[] { 2, 5, 7, 11, 37 });
      IList<int> list2 = new GuardedList<int>(list1);
      IList<int> 
        gv = new GuardedList<int>(list1.View(1,2)),
        vg = list2.View(1,2);
      IList<int> 
        gvu = gv.Underlying, 
        vgu = vg.Underlying;
      Console.WriteLine(gvu);  // Legal 
      Console.WriteLine(vgu);  // Legal 
      // gv.Slide(+1);         // Illegal: guarded view cannot be slid
      vg.Slide(+1);            // Legal: view of guarded can be slid
      // gvu[1] = 9;           // Illegal: list is guarded
      // vgu[1] = 9;           // Illegal: list is guarded
    }
  }
}
