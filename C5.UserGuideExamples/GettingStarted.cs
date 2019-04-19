// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: GettingStarted 2005-01-18

// Compile with 
//   csc /r:C5.dll GettingStarted.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace GettingStarted {
  class GettingStarted {
    public static void Main(String[] args) {
      IList<String> names = new ArrayList<String>();
      names.AddAll(new String[] { "Hoover", "Roosevelt", 
                                  "Truman", "Eisenhower", "Kennedy" });
      // Print list:
      Console.WriteLine(names);
      // Print item 1 ("Roosevelt") in the list:
      Console.WriteLine(names[1]);
      // Create a list view comprising post-WW2 presidents:
      IList<String> postWWII = names.View(2, 3);
      // Print item 2 ("Kennedy") in the view:
      Console.WriteLine(postWWII[2]);
      // Enumerate and print the list view in reverse chronological order:
      foreach (String name in postWWII.Backwards()) 
        Console.WriteLine(name);
    }
  }
}
