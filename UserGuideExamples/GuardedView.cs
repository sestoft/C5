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
