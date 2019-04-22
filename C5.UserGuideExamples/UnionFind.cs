// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: union find structure 2007-11-23

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll UnionFind.cs 

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

namespace C5.UserGuideExamples
{
    class UnionFind
    {
        public static void Main()
        {
            var x = Eqclass<int>.Make(3);
            var y = Eqclass<int>.Make(4);
            var z = Eqclass<int>.Make(5);
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

    public class Eqclass<T>
    {
        private static HashDictionary<T, Eqclass<T>> dict = new HashDictionary<T, Eqclass<T>>();

        private Eqclass<T> _link;
        private int _rank;

        public T Item { get; }

        private Eqclass(T item)
        {
            Item = item;
        }

        public static Eqclass<T> Make(T item)
        {
            if (!dict.Find(ref item, out var result))
            {
                dict[item] = result = new Eqclass<T>(item);
            }

            return result;
        }

        public void Union(Eqclass<T> that)
        {
            var thatRep = that.Find();
            var thisRep = Find();

            if (thatRep != thisRep)
            {
                if (thatRep._rank == thisRep._rank)
                {
                    thisRep._link = thatRep;
                    thatRep._rank++;
                }
                else if (thatRep._rank > thisRep._rank)
                {
                    thisRep._link = thatRep;
                }
                else
                {
                    thatRep._link = thisRep;
                }
            }
        }

        // Find, with path halving: avoids recursion
        public Eqclass<T> Find()
        {
            if (_link == null)
            {
                return this;
            }
            else if (_link._link == null)
            {
                return _link;
            }
            else
            {
                // Grandparent, parent, and link
                var pp = this;
                var p = _link;
                var plink = p._link;

                while (plink != null)
                {
                    // Invariant: pp -> p -> plink
                    pp._link = plink;
                    pp = p;
                    p = plink;
                    plink = p._link;
                }

                return p;
            }
        }
    }
}
