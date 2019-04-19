// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Comparer patterns

// Compile with 
//   csc /r:C5.dll ComparerPatterns.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

class MyTest {
  public static void Main(String[] args) {
    SCG.IComparer<Rec<string,int>> lexico1 = new Lexico();
    SCG.IComparer<Rec<string,int>> lexico2 =
      new DelegateComparer<Rec<string,int>>(
          delegate(Rec<string,int> item1, Rec<string,int> item2) { 
            int major = item1.X1.CompareTo(item2.X1);
            return major != 0 ? major : item1.X2.CompareTo(item2.X2);
          });
    Rec<String,int> r1 = new Rec<String,int>("Carsten", 1962);
    Rec<String,int> r2 = new Rec<String,int>("Carsten", 1964);
    Rec<String,int> r3 = new Rec<String,int>("Christian", 1932);
    Console.WriteLine(lexico1.Compare(r1, r1) == 0);
    Console.WriteLine(lexico1.Compare(r1, r2) < 0);
    Console.WriteLine(lexico1.Compare(r2, r3) < 0);
    Console.WriteLine(lexico2.Compare(r1, r1) == 0);
    Console.WriteLine(lexico2.Compare(r1, r2) < 0);
    Console.WriteLine(lexico2.Compare(r2, r3) < 0);
    
    SCG.IComparer<String> rev 
      = ReverseComparer<String>(Comparer<String>.Default);
    Console.WriteLine(rev.Compare("A", "A") == 0);
    Console.WriteLine(rev.Compare("A", "B") > 0);
    Console.WriteLine(rev.Compare("B", "A") < 0);
  }

  class Lexico : SCG.IComparer<Rec<String,int>> {
    public int Compare(Rec<String,int> item1, Rec<String,int> item2) {
      int major = item1.X1.CompareTo(item2.X1);
      return major != 0 ? major : item1.X2.CompareTo(item2.X2);
     }
  }

  class Lexico3 : SCG.IComparer<Rec<String,int,double>> {
    public int Compare(Rec<String,int,double> item1, Rec<String,int,double> item2) {
      int major1 = item1.X1.CompareTo(item2.X1);
      int major2 = major1 != 0 ? major1 : item1.X2.CompareTo(item2.X2);
      return major2 != 0 ? major2 : item1.X2.CompareTo(item2.X2);
     }
  }

  public static SCG.IComparer<T> ReverseComparer<T>(SCG.IComparer<T> cmp) {
    return new DelegateComparer<T>(
       delegate(T item1, T item2) { return cmp.Compare(item2, item1); });
  }
}
