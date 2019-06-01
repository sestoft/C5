// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: functional sets 2004-12-21

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll Sets.cs 

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    // The class of sets with item type T, implemented as a subclass of
    // HashSet<T> but with functional infix operators * + - that compute
    // intersection, union and difference functionally.  That is, they
    // create a new set object instead of modifying an existing one.
    // The hasher is automatically created so that it is appropriate for
    // T.  In particular, this is true when T has the form Set<W> for
    // some W, since Set<W> implements ICollectionValue<W>.
    public class Set<T> : HashSet<T>
    {
        public Set(SCG.IEnumerable<T> enm) : base()
        {
            AddAll(enm);
        }

        public Set(params T[] elems) : this((SCG.IEnumerable<T>)elems) { }

        // MySet union (+), difference (-), and intersection (*):
        public static Set<T> operator +(Set<T> s1, Set<T> s2)
        {
            if (s1 == null || s2 == null)
            {
                throw new ArgumentNullException("MySet+MySet");
            }
            else
            {
                var res = new Set<T>(s1);
                res.AddAll(s2);
                return res;
            }
        }

        public static Set<T> operator -(Set<T> s1, Set<T> s2)
        {
            if (s1 == null || s2 == null)
            {
                throw new ArgumentNullException("MySet-MySet");
            }
            else
            {
                var res = new Set<T>(s1);
                res.RemoveAll(s2);
                return res;
            }
        }

        public static Set<T> operator *(Set<T> s1, Set<T> s2)
        {
            if (s1 == null || s2 == null)
            {
                throw new ArgumentNullException("MySet*MySet");
            }
            else
            {
                var res = new Set<T>(s1);
                res.RetainAll(s2);
                return res;
            }
        }

        // Equality of sets; take care to avoid infinite loops
        public static bool operator ==(Set<T> s1, Set<T> s2)
        {
            return EqualityComparer<Set<T>>.Default.Equals(s1, s2);
        }

        public static bool operator !=(Set<T> s1, Set<T> s2)
        {
            return !(s1 == s2);
        }

        public override bool Equals(object that)
        {
            return this == (that as Set<T>);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Set<T>>.Default.GetHashCode(this);
        }

        // Subset (<=) and superset (>=) relation:
        public static bool operator <=(Set<T> s1, Set<T> s2)
        {
            if (s1 == null || s2 == null)
            {
                throw new ArgumentNullException("MySet<=MySet");
            }
            else
            {
                return s1.ContainsAll(s2);
            }
        }

        public static bool operator >=(Set<T> s1, Set<T> s2)
        {
            if (s1 == null || s2 == null)
            {
                throw new ArgumentNullException("MySet>=MySet");
            }
            else
            {
                return s2.ContainsAll(s1);
            }
        }

        public override string ToString()
        {
            return $"{{{string.Join(",", this)}}}";
        }
    }

    internal class Sets
    {
        public static void Main(string[] args)
        {
            var s1 = new Set<int>(2, 3, 5, 7, 11);
            var s2 = new Set<int>(2, 4, 6, 8, 10);
            Console.WriteLine("s1 + s2 = {0}", s1 + s2);
            Console.WriteLine("s1 * s2 = {0}", s1 * s2);
            Console.WriteLine("s1 - s2 = {0}", s1 - s2);
            Console.WriteLine("s1 - s1 = {0}", s1 - s1);
            Console.WriteLine("s1 + s1 == s1 is {0}", s1 + s1 == s1);
            Console.WriteLine("s1 * s1 == s1 is {0}", s1 * s1 == s1);
            var ss1 = new Set<Set<int>>(s1, s2, s1 + s2);
            Console.WriteLine("ss1 = {0}", ss1);
            Console.WriteLine("IntersectionClose(ss1) = {0}", IntersectionClose(ss1));
            var ss2 = new Set<Set<int>>(new Set<int>(2, 3), new Set<int>(1, 3), new Set<int>(1, 2));
            Console.WriteLine("ss2 = {0}", ss2);
            Console.WriteLine("IntersectionClose(ss2) = {0}", IntersectionClose(ss2));
        }

        // Given a set SS of sets of Integers, compute its intersection
        // closure, that is, the least set TT such that SS is a subset of TT
        // and such that for any two sets t1 and t2 in TT, their
        // intersection is also in TT.  

        // For instance, if SS is {{2,3}, {1,3}, {1,2}}, 
        // then TT is {{2,3}, {1,3}, {1,2}, {3}, {2}, {1}, {}}.

        // Both the argument and the result is a MySet<MySet<int>>
        private static Set<Set<T>> IntersectionClose<T>(Set<Set<T>> ss)
        {
            IQueue<Set<T>> worklist = new CircularQueue<Set<T>>();

            foreach (Set<T> s in ss)
            {
                worklist.Enqueue(s);
            }

            var tt = new HashSet<Set<T>>();

            while (worklist.Count != 0)
            {
                var s = worklist.Dequeue();

                foreach (Set<T> t in tt)
                {
                    var ts = t * s;

                    if (!tt.Contains(ts))
                    {
                        worklist.Enqueue(ts);
                    }
                }
                tt.Add(s);
            }

            return new Set<Set<T>>(tt);
        }
    }
}
