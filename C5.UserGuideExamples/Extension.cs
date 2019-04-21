// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Experiment with extension methods and C5, 2007-10-31

// Compile with 
//   csc /r:C5.dll Extension.cs 

using System;
using System.Linq.Expressions;
using C5;
using SCG = System.Collections.Generic;

namespace Extension {
  static class AddOn {
    public static int Added<T>(this ICollection<T> coll, int x) {
      return coll.Count + x;
    }

    public static SCG.IEnumerable<T> Where<T>(this ICollection<T> coll, 
					      Expression<Func<T,bool>> pred) 
    {
      Console.WriteLine("hallo");
      //      Func<T,bool> p = pred.Compile();
      Delegate p = pred.Compile();
      foreach (T item in coll) 
	//  	if (p(item))
  	if ((bool)p.DynamicInvoke(item))
  	  yield return item;
    }

    static void Main(String[] args) {
      HashSet<Person> hs = new HashSet<Person>();
      hs.Add(new Person("Ole"));
      hs.Add(new Person("Hans"));
      foreach (Person q in (from p in hs where p.name.Length == 4 select p))
	Console.WriteLine(q);
    }
  }

  class Person {
    public readonly String name;

    public Person(String name) {
      this.name = name;
    }
    
    public override String ToString() {
      return name;
    }
  }
}
