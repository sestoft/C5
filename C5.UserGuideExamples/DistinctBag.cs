/*
 Copyright (c) 2003-2007 Niels Kokholm and Peter Sestoft
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

// C5 example: A bag collection in which items that compare equal may
// be distinct objects or values; that is, a collection with
// AllowsDuplicates=true and DuplicatesByCounting=false.

// The collection may contain distinct objects x1 and x2 for which the
// equality comparer (or comparer) says that x1 and x2 are equal.
// This can be implemented by a dictionary whose keys are the objects
// themselves (using the given equality comparer or comparer), and
// whose values are hashsets that use reference equality for equality
// comparison.

// Such a bag-with-actual-duplicates can be used to index objects, for
// instance Person objects, by name and birthdate.  Several Person 
// objects may have the same name, or the same birthdate, or even the 
// same name and birthdate, yet be distinct Person objects.

// How much of this can be implemented on top of CollectionBase<T>?

// What should the meaning of Contains(x) be?

// Compile with 
//   csc /r:C5.dll DistinctBag.cs 

using System;                           // Console
using System.Text;                      // StringBuilder
using C5; 
using SCG = System.Collections.Generic;

namespace DistinctBag
{
  static class DistinctBagMain
  {
    static void Main(String[] args)
    {
      DistinctHashBag<Person> nameColl 
        = new DistinctHashBag<Person>(new Person.NameEqualityComparer());
      Person p1 = new Person("Peter", 19620625), 
        p2 = new Person("Carsten", 19640627), 
        p3 = new Person("Carsten", 19640628);
      nameColl.Add(p1);      
      nameColl.Add(p2);
      nameColl.Add(p3);
      Console.WriteLine("nameColl = {0}", nameColl);
    }
  }

  public class DistinctHashBag<T> where T : class { 
    private HashDictionary<T, HashSet<T>> dict;

    public DistinctHashBag(SCG.IEqualityComparer<T> eqc) {
      dict = new HashDictionary<T,HashSet<T>>(eqc);
    }

    public DistinctHashBag() : this(EqualityComparer<T>.Default) {
    }
    
    public bool Add(T item) { 
      if (!dict.Contains(item)) 
        dict.Add(item, new HashSet<T>(ReferenceEqualityComparer<T>.Default));
      return dict[item].Add(item);
    }

    public bool Remove(T item) {
      bool result = false;
      if (dict.Contains(item)) {
        HashSet<T> set = dict[item];
        result = set.Remove(item);
        if (set.IsEmpty) 
          dict.Remove(item);
      }
      return result;
    } 

    public SCG.IEnumerator<T> GetEnumerator() { 
      foreach (KeyValuePair<T,HashSet<T>> entry in dict) 
	foreach (T item in entry.Value)
	  yield return item;
    }

    public override String ToString() {
      StringBuilder sb = new StringBuilder();
      foreach (T item in this) 
        sb.Append(item).Append(" ");
      return sb.ToString();
    }
  }

  public class Person {
    String name;
    int date;

    public Person(String name, int date) {
      this.name = name;
      this.date = date;
    }

    public class NameEqualityComparer : SCG.IEqualityComparer<Person> {
      public bool Equals(Person p1, Person p2) {
        return p1.name == p2.name;
      }
      public int GetHashCode(Person p) { 
        return p.name.GetHashCode();
      }
    }

    public class DateComparer : SCG.IComparer<Person> {
      public int Compare(Person p1, Person p2) { 
        return p1.date.CompareTo(p2.date);
      }
    }

    public override String ToString() {
      return name + " (" + date + ")";
    }
  }
}
