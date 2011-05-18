/*
 Copyright (c) 2003-2008 Niels Kokholm and Peter Sestoft
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

// C5 example: union find structure 2007-11-23

// Compile with 
//   csc /r:C5.dll UnionFind.cs 

/*

The union-find algorithm (Tarjan 1983) is a fast way to build and
maintain a partition of a set into disjoint subsets.  It supports the
following operations on a collection of objects:

 * Create a new one-element subset consisting of a single object.

 * Union (join) two subsets into one.

 * Find the canonical representative of the subset containing a given
   object.

Idea: A subset of values of type T, disjoint from other such subsets ,
is represented by an object Subset<T>.  A HashDictionary<T, Subset<T>>
maps each value to its subset.  

 */


using System;
using System.Text;			// Encoding
using C5;
using SCG = System.Collections.Generic;

namespace UnionFind {
  class MyTest {
    public static void Main(String[] args) {
      Eqclass<int> x = Eqclass<int>.Make(3), 
	y = Eqclass<int>.Make(4), 
	z = Eqclass<int>.Make(5);
      x.Union(y);
      y.Union(z);
      Console.WriteLine(x.Find().Item);
      Console.WriteLine(y.Find().Item);
      Console.WriteLine(z.Find().Item);
    }
  }


  // Problem: The link and rank fields are never used at the same
  // time.  Hence it is tempting to make Eqclass<T> abstract and then
  // have two subclasses, one being canonical and containing only a
  // rank, the other being a non-canonical and containing only a link.
  // However, every subset is born as a Canonical and then may turns
  // into a NonCanonical because of Union operations, and this switch
  // of class while retaining object identity is not possible (Find
  // operations just update the link field; the object remains
  // NonCanonical).  

  // The right thing is to leave Eqclass<T> alone and introduce another
  // class hierarchy Node, Canonical : Node, NonCanonical : Node, and
  // let Eqclass<T> have a field of type Node that can be updated from
  // Canonical to NonCanonical.  But this wastes the space that would
  // be saved by the subclassing idea.

  // Note: Also, this would inadvertently allow two Eqclass<T> objects
  // to share a common Node.  Java and C# confuse things by tying
  // variants (subclasses) together with references (classes).  Since
  // the languages admit (side) effects, sharing is observable and
  // sharability should be decoupled from specialization.

  public class Eqclass<T> {
    private static HashDictionary<T,Eqclass<T>> dict 
      = new HashDictionary<T,Eqclass<T>>();
    private readonly T item;
    private Eqclass<T> link = null;
    private int rank = 0;

    private Eqclass(T item) {
      this.item = item;
    }

    public static Eqclass<T> Make(T item) {
      Eqclass<T> result;
      if (!dict.Find(item, out result)) 
	dict[item] = result = new Eqclass<T>(item);
      return result;
    }

    public void Union(Eqclass<T> that) {
      Eqclass<T> thatRep = that.Find(), thisRep = this.Find();
      if (thatRep != thisRep) {
	if (thatRep.rank == thisRep.rank) {
	  thisRep.link = thatRep;
	  thatRep.rank++;
	} else if (thatRep.rank > thisRep.rank)
	  thisRep.link = thatRep;
	else
	  thatRep.link = thisRep;
      }
    }

    // Find, with path halving: avoids recursion

    public Eqclass<T> Find() {
      if (link == null) 
	return this;
      else if (link.link == null) 
	return link;
      else {
	// Grandparent, parent, and link
	Eqclass<T> pp = this, p = link, plink = p.link;
	while (plink != null) {
	  // Invariant: pp -> p -> plink
	  pp.link = plink;
	  pp = p;
	  p = plink;
	  plink = p.link;
	}
	return p;
      }
    }

    public T Item { 
      get { return item; } 
    }
  }
}
