// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll PointLocation.cs 

using System;
using System.Diagnostics;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    //public enum Site { Cell, Edge, Outside }

    /// <summary>
    /// A line segment with associated data of type T for the cell 
    /// to its right respectively left.
    /// </summary>
    public struct Edge<T> : IComparable<Edge<T>>
    {
        public double Xs { get; }

        public double Ys { get; }

        public double Xe { get; }

        public double Ye { get; }

        public T Right { get; }

        public T Left { get; }

        public Edge(double xs, double ys, double xe, double ye, T right, T left)
        {
            if (xs < xe)
            {
                Xs = xs;
                Ys = ys;
                Xe = xe;
                Ye = ye;
                Right = right;
                Left = left;
            }
            else
            {
                Xs = xe;
                Ys = ye;
                Xe = xs;
                Ye = ys;
                Right = left;
                Left = right;
            }
        }

        public T Cell(bool upper)
        {
            return upper ? Left : Right;
        }

        public override string ToString()
        {
            return string.Format("[({0:G5};{1:G5})->({2:G5};{3:G5})/R:{4} L:{5}]", Xs, Ys, Xe, Ye, Right, Left);
        }

        public int CompareTo(Edge<T> other)
        {
            double dx, dy, thisotherx, thisothery;
            int res = 1;
            if (other.Xs < Xs)
            {
                dx = other.Xe - other.Xs;
                dy = other.Ye - other.Ys;
                thisotherx = Xs - other.Xs;
                thisothery = Ys - other.Ys;
                res = -1;
            }
            else
            {
                dx = Xe - Xs;
                dy = Ye - Ys;
                if (Ys == other.Ys && Xs == other.Xs)
                {
                    thisotherx = other.Xe - Xs;
                    thisothery = other.Ye - Ys;
                }
                else
                {
                    thisotherx = other.Xs - Xs;
                    thisothery = other.Ys - Ys;
                }
            }
            double det = dx * thisothery - dy * thisotherx;
            if (det > 0)
            {
                res = -1 * res;
            }
            //else if (det < 0) res = 1 * res;
            else if (det == 0)
            {
                if (Xs == other.Xs && Xe == other.Xe && Ys == other.Ys && Ye == other.Ye)
                {
                    res = 0;
                }
            }
            return res;
        }
    }

    /// <summary>
    /// A data structure for point location in a plane divided into
    /// cells by edges. This is the classical use of persistent trees
    /// by Sarnak and Tarjan [?]. See de Berg et al for alternatives.
    /// 
    /// The internal data is an outer sorted dictionary that maps each
    /// x coordinate of an endpoint of some edge to an inner sorted set
    /// of the edges crossing or touching the vertical line at that x
    /// coordinate, the edges being ordered by their y coordinates
    /// to the immediate right of x. Lookup of a point (x,y) is done by
    /// finding the predecessor of x cell the outer dictionary and then locating
    /// the edges above and below (x,y) by searching in the inner sorted set.
    /// 
    /// The creation of the inner sorted sets is done by maintaining a
    /// (persistent) tree of edges, inserting and deleting edges according
    /// to a horizontal sweep of the edges while saving a snapshot of the
    /// inner tree in the outer dictionary at each new x coordinate.
    ///
    /// If there are n edges, there will be 2n updates to the inner tree,
    /// and in the worst case, the inner tree will have size Omega(n) for
    /// Omega(n) snapshots. We will use O(n*logn) time and O(n) space for
    /// sorting the endpoints. If we use a nodecopying persistent inner tree,
    /// we will use O(n) space and time for building the data structure proper.
    /// If we use a pathcopy persistent tree, we will use O(n*logn) time and
    /// space for the data structure. Finally, if we use a non-persistent
    /// tree with a full copy for snapshot, we may use up to O(n^2) space and
    /// time for building the datastructure.
    ///
    /// Lookup will take O(logn) time in any case, but taking the memory
    /// hierarchy into consideration, a low space use is very beneficial
    /// for large problems.
    ///
    /// The code assumes that the given set of edges is correct, in particular
    /// that they do not touch at interior points (e.g. cross or coincide). 
    /// </summary>
    public class PointLocator<T>
    {
        private TreeDictionary<double, ISorted<Edge<T>>> htree;

        private readonly TreeDictionary<double, System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>> endpoints;

        private bool built = false;

        public PointLocator()
        {
            //htree = new TreeDictionary<double,TreeSet<Edge<T>>>(dc);
            endpoints = new TreeDictionary<double, System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>>();
        }

        public PointLocator(SCG.IEnumerable<Edge<T>> edges)
        {
            //htree = new TreeDictionary<double,TreeSet<Edge<T>>>(dc);
            endpoints = new TreeDictionary<double, System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>>();
            foreach (Edge<T> edge in edges)
            {
                add(edge);
            }
        }

        private void add(Edge<T> edge)
        {
            if (edge.Xs == edge.Xe)
            {
                return;
            }

            if (!endpoints.Contains(edge.Xs))
            {
                endpoints.Add(edge.Xs, new System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>(new LinkedList<Edge<T>>(), new LinkedList<Edge<T>>()));
            }

            System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>> kv;
            kv = endpoints[edge.Xs];
            kv.Key.Add(edge);
            if (!endpoints.Contains(edge.Xe))
            {
                endpoints.Add(edge.Xe, new System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>(new LinkedList<Edge<T>>(), new LinkedList<Edge<T>>()));
            }

            kv = endpoints[edge.Xe];
            kv.Value.Add(edge);
        }

        public void Add(Edge<T> edge)
        {
            if (built)
            {
                throw new InvalidOperationException("PointLocator static when built");
            }

            add(edge);
        }

        public void AddAll(SCG.IEnumerable<Edge<T>> edges)
        {
            if (built)
            {
                throw new InvalidOperationException("PointLocator static when built");
            }

            foreach (Edge<T> edge in edges)
            {
                add(edge);
            }
        }

        public void Build()
        {
            //htree.Clear();
            htree = new TreeDictionary<double, ISorted<Edge<T>>>();

            TreeSet<Edge<T>> vtree = new TreeSet<Edge<T>>();

            htree[Double.NegativeInfinity] = (ISorted<Edge<T>>)(vtree.Snapshot());

            foreach (System.Collections.Generic.KeyValuePair<double, System.Collections.Generic.KeyValuePair<LinkedList<Edge<T>>, LinkedList<Edge<T>>>> p in endpoints)
            {
                foreach (Edge<T> e in p.Value.Value)
                {
                    Debug.Assert(vtree.Check("C"));

                    bool chk = vtree.Remove(e);
                    Debug.Assert(vtree.Check("D"));

                    Debug.Assert(chk, "edge was not removed!", "" + e);
                }

                foreach (Edge<T> e in p.Value.Key)
                {
                    Debug.Assert(vtree.Check());
                    bool chk = vtree.Add(e);
                    Debug.Assert(vtree.Check());

                    Debug.Assert(chk, "edge was not added!", "" + e);
                }

                htree[p.Key] = (ISorted<Edge<T>>)(vtree.Snapshot());
            }

            built = true;
        }


        /*public void Clear()
          {
          endpoints.Clear();
          htree.Clear();
          }*/
        /// <summary>
        /// Find the cell, if any, containing (x,y).
        /// </summary>
        /// <param name="x">x coordinate of point</param>
        /// <param name="y">y coordinate of point</param>
        /// <param name="cell"></param>
        /// <returns>True if point is inside some cell</returns>
        public bool Place(double x, double y, out T cell)
        {
            if (!built)
            {
                throw new InvalidOperationException("PointLocator must be built first");
            }

            System.Collections.Generic.KeyValuePair<double, ISorted<Edge<T>>> p = htree.WeakPredecessor(x);

            //if (DoubleComparer.StaticCompare(cell.key,x)==0)
            //Just note it, we have thrown away the vertical edges!
            PointComparer<T> c = new PointComparer<T>(x, y);

            //Return value true here means we are at an edge.
            //But note that if x is in htree.Keys, we may be at a
            //vertical edge even if the return value is false here.
            //Therefore we do not attempt to sort out completely the case
            //where (x,y) is on an edge or even on several edges,
            //and just deliver some cell it is in.
            p.Value.Cut(c, out Edge<T> low, out bool lval, out _, out bool hval);
            if (!lval || !hval)
            {
                cell = default;
                return false;
            }
            else
            {
                cell = low.Cell(true);//high.Cell(false);
                return true;
            }
        }

        public void Place(double x, double y, out T upper, out bool hval, out T lower, out bool lval)
        {
            if (!built)
            {
                throw new InvalidOperationException("PointLocator must be built first");
            }

            System.Collections.Generic.KeyValuePair<double, ISorted<Edge<T>>> p = htree.WeakPredecessor(x);

            //if (DoubleComparer.StaticCompare(cell.key,x)==0)
            //Just note it, we have thrown away the vertical edges!
            PointComparer<T> c = new PointComparer<T>(x, y);

            //Return value true here means we are at an edge.
            //But note that if x is in htree.Keys, we may be at a
            //vertical edge even if the return value is false here.
            //Therefore we do not attempt to sort out completely the case
            //where (x,y) is on an edge or even on several edges,
            //and just deliver some cell it is in.
            p.Value.Cut(c, out Edge<T> low, out lval, out Edge<T> high, out hval);
            upper = hval ? high.Cell(false) : default;
            lower = lval ? low.Cell(true) : default;
            return;
        }

        public void Test(double x, double y)
        {

            if (Place(x, y, out T cell))
            {
                Console.WriteLine("({0}; {1}): <- {2} ", x, y, cell);
            }
            else
            {
                Console.WriteLine("({0}; {1}): -", x, y);
            }
        }
    }

    /// <summary>
    /// Compare a given point (x,y) to edges: is the point above, at or below
    /// the edge. Assumes edges not vertical. 
    /// Uses crossproduct to compute the result.
    /// </summary>
    internal class PointComparer<T> : IComparable<Edge<T>>
    {
        private readonly double x, y;

        public PointComparer(double x, double y)
        {
            this.x = x; this.y = y;
        }

        public int CompareTo(Edge<T> a)
        {
            int res = 0;
            double abx = a.Xe - a.Xs;
            double aby = a.Ye - a.Ys;
            double atx = x - a.Xs;
            double aty = y - a.Ys;
            double det = abx * aty - aby * atx;
            if (det > 0)
            {
                res = 1;
            }
            else if (det < 0)
            {
                res = -1;
            }

            return res;
        }

        public bool Equals(Edge<T> a) { return CompareTo(a) == 0; }
    }

    namespace Test
    {
        public class Ugly : EnumerableBase<Edge<int>>, SCG.IEnumerable<Edge<int>>, SCG.IEnumerator<Edge<int>>
        {
            private int level = -1;
            private readonly int maxlevel;
            private bool leftend = false;

            public Ugly(int maxlevel)
            {
                this.maxlevel = maxlevel;
            }

            public override SCG.IEnumerator<Edge<int>> GetEnumerator()
            {
                return (SCG.IEnumerator<Edge<int>>)MemberwiseClone();
            }

            public void Reset()
            {
                level = -1;
                leftend = false;
            }

            public bool MoveNext()
            {
                if (level > maxlevel)
                {
                    throw new InvalidOperationException();
                }

                if (leftend)
                {
                    leftend = false;
                    return true;
                }
                else
                {
                    leftend = true;
                    return ++level <= maxlevel;
                }
            }

            public Edge<int> Current
            {
                get
                {
                    if (level < 0 || level > maxlevel)
                    {
                        throw new InvalidOperationException();
                    }

                    double y = (level * 37) % maxlevel;
                    _ = leftend ? 1 : maxlevel;

                    if (leftend)
                    {
                        return new Edge<int>(0, y, level, y - 0.5, 0, 0);
                    }
                    else
                    {
                        return new Edge<int>(level, y - 0.5, level, y, 0, 0);
                    }
                }
            }


            public void Dispose() { }

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current => throw new NotImplementedException();

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class TestUgly
        {
            private readonly Ugly ugly;

            private readonly int d;

            private PointLocator<int> pointlocator;


            public TestUgly(int d)
            {
                this.d = d;
                ugly = new Ugly(d);
            }


            public double Traverse()
            {
                double xsum = 0;

                foreach (Edge<int> e in ugly)
                {
                    xsum += e.Xe;
                }

                return xsum;
            }

            public bool LookUp(int count, int seed)
            {
                Random random = new Random(seed);
                bool res = false;

                for (int i = 0; i < count; i++)
                {
                    res ^= pointlocator.Place(random.NextDouble() * d, random.NextDouble() * d, out _);
                }

                return res;
            }

            public static void Run(string[] args)
            {
                int d = args.Length >= 2 ? int.Parse(args[1]) : 400;//00;
                _ = args.Length >= 3 ? int.Parse(args[2]) : 10;
                int lookups = args.Length >= 4 ? int.Parse(args[3]) : 500;//00;

                new TestUgly(d).run(lookups);
            }


            public void run(int lookups)
            {
                _ = Traverse();

                pointlocator = new PointLocator<int>(ugly);
                pointlocator.Build();

                LookUp(lookups, 567);
            }
        }

        public class Lattice : EnumerableBase<Edge<string>>, SCG.IEnumerable<Edge<string>>, SCG.IEnumerator<Edge<string>>, System.Collections.IEnumerator
        {
            private int currenti = -1, currentj = 0;

            private bool currenthoriz = true;

            private readonly int maxi, maxj;

            private readonly double a11 = 1, a21 = -1, a12 = 1, a22 = 1;

            public Lattice(int maxi, int maxj, double a11, double a21, double a12, double a22)
            {
                this.maxi = maxi;
                this.maxj = maxj;
                this.a11 = a11;
                this.a12 = a12;
                this.a21 = a21;
                this.a22 = a22;
            }

            public Lattice(int maxi, int maxj)
            {
                this.maxi = maxi;
                this.maxj = maxj;
            }

            public override SCG.IEnumerator<Edge<string>> GetEnumerator()
            {
                return (SCG.IEnumerator<Edge<string>>)MemberwiseClone();
            }

            public void Reset()
            {
                currenti = -1;
                currentj = 0;
                currenthoriz = true;
            }

            public bool MoveNext()
            {
                if (currenthoriz)
                {
                    if (++currenti >= maxi)
                    {
                        if (currentj >= maxj)
                        {
                            return false;
                        }

                        currenti = 0;
                        currenthoriz = false;
                    }

                    return true;
                }
                else
                {
                    if (++currenti > maxi)
                    {
                        currenti = 0;
                        currenthoriz = true;
                        if (++currentj > maxj)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }


            /*
            private string i2l(int i)
            {
                int ls = 0, j = i;

                do { ls++; j = j / 26 - 1; } while (j >= 0);

                char[] res = new char[ls];

                while (ls > 0) { res[--ls] = (char)(65 + i % 26); i = i / 26 - 1; }

                //res[0]--;
                return new string(res);
            } */

            public Edge<string> Current
            {
                get
                {
                    if (currenti >= maxi && currentj >= maxj)
                    {
                        throw new InvalidOperationException();
                    }

                    double xs = currenti * a11 + currentj * a12;
                    double ys = currenti * a21 + currentj * a22;
                    double deltax = currenthoriz ? a11 : a12;
                    double deltay = currenthoriz ? a21 : a22;

                    return new Edge<string>(xs, ys, xs + deltax, ys + deltay, "", "");
                }
            }


            public void Dispose() { }

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current => throw new NotImplementedException();

            bool System.Collections.IEnumerator.MoveNext()
            {
                throw new NotImplementedException();
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class TestLattice
        {
            private readonly Lattice lattice;

            private readonly int d;

            private PointLocator<string> pointlocator;


            public TestLattice(int d)
            {
                this.d = d;
                lattice = new Lattice(d, d, 1, 0, 0, 1);
            }

            public TestLattice(int d, double shear)
            {
                this.d = d;
                lattice = new Lattice(d, d, 1, 0, shear, 1);
            }

            public double Traverse()
            {
                double xsum = 0;

                foreach (Edge<string> e in lattice)
                {
                    xsum += e.Xe;
                }

                return xsum;
            }


            public bool LookUp(int count, int seed)
            {
                Random random = new Random(seed);
                bool res = false;

                for (int i = 0; i < count; i++)
                {
                    res ^= pointlocator.Place(random.NextDouble() * d, random.NextDouble() * d, out _);
                }

                return res;
            }


            public static void Run()
            {
                int d = 200;
                int repeats = 2;
                int lookups = 50000;
                Console.WriteLine("TestLattice Run({0}), means over {1} repeats:", d, repeats);
                TestLattice tl = new TestLattice(d, 0.000001);

                tl.Traverse();

                tl.pointlocator = new PointLocator<string>();

                tl.pointlocator.AddAll(tl.lattice);

                tl.pointlocator.Build();

                tl.LookUp(lookups, 567);
            }


            public void BasicRun()
            {
                pointlocator.Test(-0.5, -0.5);
                pointlocator.Test(-0.5, 0.5);
                pointlocator.Test(-0.5, 1.5);
                pointlocator.Test(0.5, -0.5);
                pointlocator.Test(0.5, 0.5);
                pointlocator.Test(0.5, 1.5);
                pointlocator.Test(1.5, -0.5);
                pointlocator.Test(1.5, 0.5);
                pointlocator.Test(1.5, 1.5);
                pointlocator.Test(1.5, 4.99);
                pointlocator.Test(1.5, 5);
                pointlocator.Test(1.5, 5.01);
                pointlocator.Test(1.99, 4.99);
                pointlocator.Test(1.99, 5);
                pointlocator.Test(1.99, 5.01);
                pointlocator.Test(2, 4.99);
                pointlocator.Test(2, 5);
                pointlocator.Test(2, 5.01);
                pointlocator.Test(2.01, 4.99);
                pointlocator.Test(2.01, 5);
                pointlocator.Test(2.01, 5.01);
            }
        }

        internal class T1
        {
            private static void t1()
            {
                PointLocator<int> pl = new PointLocator<int>();
                pl.Add(new Edge<int>(1, 1, 3, 4, 1, -1));
                pl.Add(new Edge<int>(3, 4, 4, 2, 1, -1));
                pl.Add(new Edge<int>(1, 1, 4, 2, -1, 1));
                pl.Build();

                pl.Place(0, 0, out int x); Debug.Assert(x == 0);
                pl.Place(1, 0, out x); Debug.Assert(x == 0);
                pl.Place(2, 0, out x); Debug.Assert(x == 0);
                pl.Place(3, 0, out x); Debug.Assert(x == 0);
                pl.Place(4, 0, out x); Debug.Assert(x == 0);
                pl.Place(5, 0, out x); Debug.Assert(x == 0);

                pl.Place(0, 1, out x); Debug.Assert(x == 0);
                //pl.Place(1, 1, out x); Debug.Assert(x == 0);
                pl.Place(2, 1, out x); Debug.Assert(x == 0);
                pl.Place(3, 1, out x); Debug.Assert(x == 0);
                pl.Place(4, 1, out x); Debug.Assert(x == 0);
                pl.Place(5, 1, out x); Debug.Assert(x == 0);

                pl.Place(0, 2, out x); Debug.Assert(x == 0);
                pl.Place(1, 2, out x); Debug.Assert(x == 0);
                pl.Place(2, 2, out x); Debug.Assert(x == 1);
                pl.Place(3, 2, out x); Debug.Assert(x == 1);
                //pl.Place(4, 2, out x); Debug.Assert(x == 0);
                pl.Place(5, 2, out x); Debug.Assert(x == 0);

                pl.Place(0, 3, out x); Debug.Assert(x == 0);
                pl.Place(1, 3, out x); Debug.Assert(x == 0);
                pl.Place(2, 3, out x); Debug.Assert(x == 0);
                pl.Place(3, 3, out x); Debug.Assert(x == 1);
                pl.Place(4, 3, out x); Debug.Assert(x == 0);
                pl.Place(5, 3, out x); Debug.Assert(x == 0);

                pl.Place(0, 4, out x); Debug.Assert(x == 0);
                pl.Place(1, 4, out x); Debug.Assert(x == 0);
                pl.Place(2, 4, out x); Debug.Assert(x == 0);
                //pl.Place(3, 4, out x); Debug.Assert(x == 0);
                pl.Place(4, 4, out x); Debug.Assert(x == 0);
                pl.Place(5, 4, out x); Debug.Assert(x == 0);

                pl.Place(0, 5, out x); Debug.Assert(x == 0);
                pl.Place(1, 5, out x); Debug.Assert(x == 0);
                pl.Place(2, 5, out x); Debug.Assert(x == 0);
                pl.Place(3, 5, out x); Debug.Assert(x == 0); //wtf?
                pl.Place(4, 5, out x); Debug.Assert(x == 0);
                pl.Place(5, 5, out x); Debug.Assert(x == 0);

            }

            private static void t2()
            {
                PointLocator<int> pl = new PointLocator<int>();
                //outer triangle
                pl.Add(new Edge<int>(1, 1, 2, 8, 1, -1));
                pl.Add(new Edge<int>(2, 8, 7, 5, 1, -1));
                pl.Add(new Edge<int>(1, 1, 7, 5, -1, 1));
                //inner triangle
                pl.Add(new Edge<int>(2, 4, 3, 6, 2, 1));
                pl.Add(new Edge<int>(3, 6, 4, 5, 2, 1));
                pl.Add(new Edge<int>(2, 4, 4, 5, 1, 2));
                pl.Build();

                pl.Place(0, 0, out int x); Debug.Assert(x == 0);
                pl.Place(1, 0, out x); Debug.Assert(x == 0);
                pl.Place(2, 0, out x); Debug.Assert(x == 0);
                pl.Place(3, 0, out x); Debug.Assert(x == 0);
                pl.Place(4, 0, out x); Debug.Assert(x == 0);
                pl.Place(5, 0, out x); Debug.Assert(x == 0);
                pl.Place(6, 0, out x); Debug.Assert(x == 0);
                pl.Place(7, 0, out x); Debug.Assert(x == 0);
                pl.Place(8, 0, out x); Debug.Assert(x == 0);

                pl.Place(0, 1, out x); Debug.Assert(x == 0);
                //pl.Place(1, 1, out x); Debug.Assert(x == 0);
                pl.Place(2, 1, out x); Debug.Assert(x == 0);
                pl.Place(3, 1, out x); Debug.Assert(x == 0);
                pl.Place(4, 1, out x); Debug.Assert(x == 0);
                pl.Place(5, 1, out x); Debug.Assert(x == 0);
                pl.Place(6, 1, out x); Debug.Assert(x == 0);
                pl.Place(7, 1, out x); Debug.Assert(x == 0);
                pl.Place(8, 1, out x); Debug.Assert(x == 0);

                pl.Place(0, 2, out x); Debug.Assert(x == 0);
                pl.Place(1, 2, out x); Debug.Assert(x == 0);
                pl.Place(2, 2, out x); Debug.Assert(x == 1);
                pl.Place(3, 2, out x); Debug.Assert(x == 0);
                pl.Place(4, 2, out x); Debug.Assert(x == 0);
                pl.Place(5, 2, out x); Debug.Assert(x == 0);
                pl.Place(6, 2, out x); Debug.Assert(x == 0);
                pl.Place(7, 2, out x); Debug.Assert(x == 0);
                pl.Place(8, 2, out x); Debug.Assert(x == 0);

                pl.Place(0, 3, out x); Debug.Assert(x == 0);
                pl.Place(1, 3, out x); Debug.Assert(x == 0);
                pl.Place(2, 3, out x); Debug.Assert(x == 1);
                pl.Place(3, 3, out x); Debug.Assert(x == 1);
                //pl.Place(4, 3, out x); Debug.Assert(x == 0);
                pl.Place(5, 3, out x); Debug.Assert(x == 0);
                pl.Place(6, 3, out x); Debug.Assert(x == 0);
                pl.Place(7, 3, out x); Debug.Assert(x == 0);
                pl.Place(8, 3, out x); Debug.Assert(x == 0);

                pl.Place(0, 4, out x); Debug.Assert(x == 0);
                pl.Place(1, 4, out x); Debug.Assert(x == 0);
                //pl.Place(2, 4, out x); Debug.Assert(x == 0);
                pl.Place(3, 4, out x); Debug.Assert(x == 1);
                pl.Place(4, 4, out x); Debug.Assert(x == 1);
                pl.Place(5, 4, out x); Debug.Assert(x == 1);
                pl.Place(6, 4, out x); Debug.Assert(x == 0);
                pl.Place(7, 4, out x); Debug.Assert(x == 0);
                pl.Place(8, 4, out x); Debug.Assert(x == 0);

                pl.Place(0, 5, out x); Debug.Assert(x == 0);
                pl.Place(1, 5, out x); Debug.Assert(x == 0);
                pl.Place(2, 5, out x); Debug.Assert(x == 1);
                pl.Place(3, 5, out x); Debug.Assert(x == 2);
                //pl.Place(4, 5, out x); Debug.Assert(x == 0);
                pl.Place(5, 5, out x); Debug.Assert(x == 1);
                pl.Place(6, 5, out x); Debug.Assert(x == 1);
                //pl.Place(7, 5, out x); Debug.Assert(x == 0);
                pl.Place(8, 5, out x); Debug.Assert(x == 0);

                pl.Place(0, 6, out x); Debug.Assert(x == 0);
                pl.Place(1, 6, out x); Debug.Assert(x == 0);
                pl.Place(2, 6, out x); Debug.Assert(x == 1);
                //pl.Place(3, 6, out x); Debug.Assert(x == 0);
                pl.Place(4, 6, out x); Debug.Assert(x == 1);
                pl.Place(5, 6, out x); Debug.Assert(x == 1);
                pl.Place(6, 6, out x); Debug.Assert(x == 0);
                pl.Place(7, 6, out x); Debug.Assert(x == 0);
                pl.Place(8, 6, out x); Debug.Assert(x == 0);

                pl.Place(0, 7, out x); Debug.Assert(x == 0);
                pl.Place(1, 7, out x); Debug.Assert(x == 0);
                pl.Place(2, 7, out x); Debug.Assert(x == 1);
                pl.Place(3, 7, out x); Debug.Assert(x == 1);
                pl.Place(4, 7, out x); Debug.Assert(x == 0);
                pl.Place(5, 7, out x); Debug.Assert(x == 0);
                pl.Place(6, 7, out x); Debug.Assert(x == 0);
                pl.Place(7, 7, out x); Debug.Assert(x == 0);
                pl.Place(8, 7, out x); Debug.Assert(x == 0);

                pl.Place(0, 8, out x); Debug.Assert(x == 0);
                pl.Place(1, 8, out x); Debug.Assert(x == 0);
                //pl.Place(2, 8, out x); Debug.Assert(x == 0);
                pl.Place(3, 8, out x); Debug.Assert(x == 0);
                pl.Place(4, 8, out x); Debug.Assert(x == 0);
                pl.Place(5, 8, out x); Debug.Assert(x == 0);
                pl.Place(6, 8, out x); Debug.Assert(x == 0);
                pl.Place(7, 8, out x); Debug.Assert(x == 0);
                pl.Place(8, 8, out x); Debug.Assert(x == 0);

                pl.Place(0, 9, out x); Debug.Assert(x == 0);
                pl.Place(1, 9, out x); Debug.Assert(x == 0);
                pl.Place(2, 9, out x); Debug.Assert(x == 0);
                pl.Place(3, 9, out x); Debug.Assert(x == 0);
                pl.Place(4, 9, out x); Debug.Assert(x == 0);
                pl.Place(5, 9, out x); Debug.Assert(x == 0);
                pl.Place(6, 9, out x); Debug.Assert(x == 0);
                pl.Place(7, 9, out x); Debug.Assert(x == 0);
                pl.Place(8, 9, out x); Debug.Assert(x == 0);
            }

            private static void t3()
            {
                PointLocator<int> pl = new PointLocator<int>();
                pl.Add(new Edge<int>(1, 1, 5, 2, -1, 1));
                pl.Add(new Edge<int>(5, 2, 6, 6, -1, 1));
                pl.Add(new Edge<int>(4, 3, 6, 6, 1, -1));
                pl.Add(new Edge<int>(1, 1, 4, 3, 1, -1));
                pl.Build();

                pl.Place(0, 0, out int x); Debug.Assert(x == 0);
                pl.Place(1, 0, out x); Debug.Assert(x == 0);
                pl.Place(2, 0, out x); Debug.Assert(x == 0);
                pl.Place(3, 0, out x); Debug.Assert(x == 0);
                pl.Place(4, 0, out x); Debug.Assert(x == 0);
                pl.Place(5, 0, out x); Debug.Assert(x == 0);
                pl.Place(6, 0, out x); Debug.Assert(x == 0);
                pl.Place(7, 0, out x); Debug.Assert(x == 0);

                pl.Place(0, 1, out x); Debug.Assert(x == 0);
                //pl.Place(1, 1, out x); Debug.Assert(x == 0);
                pl.Place(2, 1, out x); Debug.Assert(x == 0);
                pl.Place(3, 1, out x); Debug.Assert(x == 0);
                pl.Place(4, 1, out x); Debug.Assert(x == 0);
                pl.Place(5, 1, out x); Debug.Assert(x == 0);
                pl.Place(6, 1, out x); Debug.Assert(x == 0);
                pl.Place(7, 1, out x); Debug.Assert(x == 0);

                pl.Place(0, 2, out x); Debug.Assert(x == 0);
                pl.Place(1, 2, out x); Debug.Assert(x == 0);
                pl.Place(2, 2, out x); Debug.Assert(x == 0);
                pl.Place(3, 2, out x); Debug.Assert(x == 1);
                pl.Place(4, 2, out x); Debug.Assert(x == 1);
                //pl.Place(5, 2, out x); Debug.Assert(x == 0);
                pl.Place(6, 2, out x); Debug.Assert(x == 0);
                pl.Place(7, 2, out x); Debug.Assert(x == 0);

                pl.Place(0, 3, out x); Debug.Assert(x == 0);
                pl.Place(1, 3, out x); Debug.Assert(x == 0);
                pl.Place(2, 3, out x); Debug.Assert(x == 0);
                pl.Place(3, 3, out x); Debug.Assert(x == 0);
                //pl.Place(4, 3, out x); Debug.Assert(x == 0);
                pl.Place(5, 3, out x); Debug.Assert(x == 1);
                pl.Place(6, 3, out x); Debug.Assert(x == 0);
                pl.Place(7, 3, out x); Debug.Assert(x == 0);

                pl.Place(0, 4, out x); Debug.Assert(x == 0);
                pl.Place(1, 4, out x); Debug.Assert(x == 0);
                pl.Place(2, 4, out x); Debug.Assert(x == 0);
                pl.Place(3, 4, out x); Debug.Assert(x == 0);
                pl.Place(4, 4, out x); Debug.Assert(x == 0);
                pl.Place(5, 4, out x); Debug.Assert(x == 1);
                pl.Place(6, 4, out x); Debug.Assert(x == 0);
                pl.Place(7, 4, out x); Debug.Assert(x == 0);

                pl.Place(0, 5, out x); Debug.Assert(x == 0);
                pl.Place(1, 5, out x); Debug.Assert(x == 0);
                pl.Place(2, 5, out x); Debug.Assert(x == 0);
                pl.Place(3, 5, out x); Debug.Assert(x == 0);
                pl.Place(4, 5, out x); Debug.Assert(x == 0);
                pl.Place(5, 5, out x); Debug.Assert(x == 0);
                pl.Place(6, 5, out x); Debug.Assert(x == 0);
                pl.Place(7, 5, out x); Debug.Assert(x == 0);

                pl.Place(0, 6, out x); Debug.Assert(x == 0);
                pl.Place(1, 6, out x); Debug.Assert(x == 0);
                pl.Place(2, 6, out x); Debug.Assert(x == 0);
                pl.Place(3, 6, out x); Debug.Assert(x == 0);
                pl.Place(4, 6, out x); Debug.Assert(x == 0);
                pl.Place(5, 6, out x); Debug.Assert(x == 0);
                //pl.Place(6, 6, out x); Debug.Assert(x == 0);
                pl.Place(7, 6, out x); Debug.Assert(x == 0);

                pl.Place(0, 7, out x); Debug.Assert(x == 0);
                pl.Place(1, 7, out x); Debug.Assert(x == 0);
                pl.Place(2, 7, out x); Debug.Assert(x == 0);
                pl.Place(3, 7, out x); Debug.Assert(x == 0);
                pl.Place(4, 7, out x); Debug.Assert(x == 0);
                pl.Place(5, 7, out x); Debug.Assert(x == 0);
                pl.Place(6, 7, out x); Debug.Assert(x == 0);
                pl.Place(7, 7, out x); Debug.Assert(x == 0);
            }

            private static void t4()
            {
                PointLocator<int> pl = new PointLocator<int>();
                pl.Add(new Edge<int>(2, 4, 5, 1, -1, 1));
                pl.Add(new Edge<int>(5, 1, 11, 2, -1, 2));
                pl.Add(new Edge<int>(5, 1, 9, 4, 2, 1));
                pl.Add(new Edge<int>(2, 4, 9, 4, 1, 7));
                pl.Add(new Edge<int>(9, 4, 10, 7, 5, 7));
                pl.Add(new Edge<int>(11, 2, 13, 6, 3, 5));
                pl.Add(new Edge<int>(11, 2, 14, 1, -1, 3));
                pl.Add(new Edge<int>(14, 1, 15, 4, 4, 3));
                pl.Add(new Edge<int>(14, 1, 19, 2, -1, 4));
                pl.Add(new Edge<int>(15, 4, 19, 2, 4, 6));
                pl.Add(new Edge<int>(19, 2, 21, 6, -1, 6));
                pl.Add(new Edge<int>(15, 4, 17, 9, 6, 8));
                pl.Add(new Edge<int>(13, 6, 15, 4, 3, 8));
                pl.Add(new Edge<int>(17, 9, 21, 6, 6, -1));
                pl.Add(new Edge<int>(14, 11, 17, 9, 8, -1));
                pl.Add(new Edge<int>(13, 6, 14, 11, 8, 9));
                pl.Add(new Edge<int>(10, 7, 13, 6, 5, 9));
                pl.Add(new Edge<int>(10, 7, 14, 11, 9, 10));
                pl.Add(new Edge<int>(7, 9, 10, 7, 7, 10));
                pl.Add(new Edge<int>(4, 8, 7, 9, 7, 11));
                pl.Add(new Edge<int>(2, 4, 4, 8, 7, -1));
                pl.Add(new Edge<int>(1, 9, 4, 8, -1, 11));
                pl.Add(new Edge<int>(1, 9, 3, 13, 11, -1));
                pl.Add(new Edge<int>(3, 13, 7, 9, 11, -1));
                pl.Add(new Edge<int>(7, 9, 12, 13, 10, -1));
                pl.Add(new Edge<int>(12, 13, 14, 11, 10, -1));
                pl.Add(new Edge<int>(9, 4, 11, 2, 2, 5));
                pl.Build();


                pl.Place(0, 0, out int x); Debug.Assert(x == 0);
                pl.Place(1, 0, out x); Debug.Assert(x == 0);
                pl.Place(2, 0, out x); Debug.Assert(x == 0);
                pl.Place(3, 0, out x); Debug.Assert(x == 0);
                pl.Place(4, 0, out x); Debug.Assert(x == 0);
                pl.Place(5, 0, out x); Debug.Assert(x == 0);
                pl.Place(6, 0, out x); Debug.Assert(x == 0);
                pl.Place(7, 0, out x); Debug.Assert(x == 0);
                pl.Place(8, 0, out x); Debug.Assert(x == 0);
                pl.Place(9, 0, out x); Debug.Assert(x == 0);
                pl.Place(10, 0, out x); Debug.Assert(x == 0);
                pl.Place(11, 0, out x); Debug.Assert(x == 0);
                pl.Place(12, 0, out x); Debug.Assert(x == 0);
                pl.Place(13, 0, out x); Debug.Assert(x == 0);
                pl.Place(14, 0, out x); Debug.Assert(x == 0);
                pl.Place(15, 0, out x); Debug.Assert(x == 0);
                pl.Place(16, 0, out x); Debug.Assert(x == 0);
                pl.Place(17, 0, out x); Debug.Assert(x == 0);
                pl.Place(18, 0, out x); Debug.Assert(x == 0);
                pl.Place(19, 0, out x); Debug.Assert(x == 0);
                pl.Place(20, 0, out x); Debug.Assert(x == 0);
                pl.Place(21, 0, out x); Debug.Assert(x == 0);
                pl.Place(22, 0, out x); Debug.Assert(x == 0);

                pl.Place(0, 1, out x); Debug.Assert(x == 0);
                pl.Place(1, 1, out x); Debug.Assert(x == 0);
                pl.Place(2, 1, out x); Debug.Assert(x == 0);
                pl.Place(3, 1, out x); Debug.Assert(x == 0);
                pl.Place(4, 1, out x); Debug.Assert(x == 0);
                pl.Place(6, 1, out x); Debug.Assert(x == 0);
                pl.Place(7, 1, out x); Debug.Assert(x == 0);
                pl.Place(8, 1, out x); Debug.Assert(x == 0);
                pl.Place(9, 1, out x); Debug.Assert(x == 0);
                pl.Place(10, 1, out x); Debug.Assert(x == 0);
                pl.Place(11, 1, out x); Debug.Assert(x == 0);
                pl.Place(12, 1, out x); Debug.Assert(x == 0);
                pl.Place(13, 1, out x); Debug.Assert(x == 0);
                pl.Place(15, 1, out x); Debug.Assert(x == 0);
                pl.Place(16, 1, out x); Debug.Assert(x == 0);
                pl.Place(17, 1, out x); Debug.Assert(x == 0);
                pl.Place(18, 1, out x); Debug.Assert(x == 0);
                pl.Place(19, 1, out x); Debug.Assert(x == 0);
                pl.Place(20, 1, out x); Debug.Assert(x == 0);
                pl.Place(21, 1, out x); Debug.Assert(x == 0);
                pl.Place(22, 1, out x); Debug.Assert(x == 0);

                pl.Place(0, 2, out x); Debug.Assert(x == 0);
                pl.Place(1, 2, out x); Debug.Assert(x == 0);
                pl.Place(2, 2, out x); Debug.Assert(x == 0);
                pl.Place(3, 2, out x); Debug.Assert(x == 0);
                //pl.Place(4, 2, out x); Debug.Assert(x == 0);
                pl.Place(5, 2, out x); Debug.Assert(x == 1);
                pl.Place(6, 2, out x); Debug.Assert(x == 1);
                pl.Place(7, 2, out x); Debug.Assert(x == 2);
                pl.Place(8, 2, out x); Debug.Assert(x == 2);
                pl.Place(9, 2, out x); Debug.Assert(x == 2);
                pl.Place(10, 2, out x); Debug.Assert(x == 2);
                //pl.Place(11, 2, out x); Debug.Assert(x == 0);
                pl.Place(12, 2, out x); Debug.Assert(x == 3);
                pl.Place(13, 2, out x); Debug.Assert(x == 3);
                pl.Place(14, 2, out x); Debug.Assert(x == 3);
                pl.Place(15, 2, out x); Debug.Assert(x == 4);
                pl.Place(16, 2, out x); Debug.Assert(x == 4);
                pl.Place(17, 2, out x); Debug.Assert(x == 4);
                pl.Place(18, 2, out x); Debug.Assert(x == 4);
                //pl.Place(19, 2, out x); Debug.Assert(x == 0);
                pl.Place(20, 2, out x); Debug.Assert(x == 0);
                pl.Place(21, 2, out x); Debug.Assert(x == 0);
                pl.Place(22, 2, out x); Debug.Assert(x == 0);

                pl.Place(0, 3, out x); Debug.Assert(x == 0);
                pl.Place(1, 3, out x); Debug.Assert(x == 0);
                pl.Place(2, 3, out x); Debug.Assert(x == 0);
                //pl.Place(3, 3, out x); Debug.Assert(x == 0);
                pl.Place(4, 3, out x); Debug.Assert(x == 1);
                pl.Place(5, 3, out x); Debug.Assert(x == 1);
                pl.Place(6, 3, out x); Debug.Assert(x == 1);
                pl.Place(7, 3, out x); Debug.Assert(x == 1);
                pl.Place(8, 3, out x); Debug.Assert(x == 2);
                pl.Place(9, 3, out x); Debug.Assert(x == 2);
                //pl.Place(10, 3, out x); Debug.Assert(x == 0);
                pl.Place(11, 3, out x); Debug.Assert(x == 5);
                pl.Place(12, 3, out x); Debug.Assert(x == 3);
                pl.Place(13, 3, out x); Debug.Assert(x == 3);
                pl.Place(14, 3, out x); Debug.Assert(x == 3);
                pl.Place(15, 3, out x); Debug.Assert(x == 4);
                pl.Place(16, 3, out x); Debug.Assert(x == 4);
                //pl.Place(17, 3, out x); Debug.Assert(x == 6);
                pl.Place(18, 3, out x); Debug.Assert(x == 6);
                pl.Place(19, 3, out x); Debug.Assert(x == 6);
                pl.Place(20, 3, out x); Debug.Assert(x == 0);
                pl.Place(21, 3, out x); Debug.Assert(x == 0);
                pl.Place(22, 3, out x); Debug.Assert(x == 0);

                pl.Place(0, 4, out x); Debug.Assert(x == 0);
                pl.Place(1, 4, out x); Debug.Assert(x == 0);
                //pl.Place(2, 4, out x); Debug.Assert(x == 0);
                //pl.Place(3, 4, out x); Debug.Assert(x == 0);
                //pl.Place(4, 4, out x); Debug.Assert(x == 0);
                //pl.Place(5, 4, out x); Debug.Assert(x == 0);
                //pl.Place(6, 4, out x); Debug.Assert(x == 0);
                //pl.Place(7, 4, out x); Debug.Assert(x == 0);
                //pl.Place(8, 4, out x); Debug.Assert(x == 0);
                //pl.Place(9, 4, out x); Debug.Assert(x == 0);
                pl.Place(10, 4, out x); Debug.Assert(x == 5);
                pl.Place(11, 4, out x); Debug.Assert(x == 5);
                //pl.Place(12, 4, out x); Debug.Assert(x == 0);
                pl.Place(13, 4, out x); Debug.Assert(x == 3);
                pl.Place(14, 4, out x); Debug.Assert(x == 3);
                //pl.Place(15, 4, out x); Debug.Assert(x == 0);
                pl.Place(16, 4, out x); Debug.Assert(x == 6);
                pl.Place(17, 4, out x); Debug.Assert(x == 6);
                pl.Place(18, 4, out x); Debug.Assert(x == 6);
                pl.Place(19, 4, out x); Debug.Assert(x == 6);
                //pl.Place(20, 4, out x); Debug.Assert(x == 0);
                pl.Place(21, 4, out x); Debug.Assert(x == 0);
                pl.Place(22, 4, out x); Debug.Assert(x == 0);

                pl.Place(0, 5, out x); Debug.Assert(x == 0);
                pl.Place(1, 5, out x); Debug.Assert(x == 0);
                pl.Place(2, 5, out x); Debug.Assert(x == -1);
                pl.Place(3, 5, out x); Debug.Assert(x == 7);
                pl.Place(4, 5, out x); Debug.Assert(x == 7);
                pl.Place(5, 5, out x); Debug.Assert(x == 7);
                pl.Place(6, 5, out x); Debug.Assert(x == 7);
                pl.Place(7, 5, out x); Debug.Assert(x == 7);
                pl.Place(8, 5, out x); Debug.Assert(x == 7);
                pl.Place(9, 5, out x); Debug.Assert(x == 7);
                pl.Place(10, 5, out x); Debug.Assert(x == 5);
                pl.Place(11, 5, out x); Debug.Assert(x == 5);
                pl.Place(12, 5, out x); Debug.Assert(x == 5);
                pl.Place(13, 5, out x); Debug.Assert(x == 3);
                //pl.Place(14, 5, out x); Debug.Assert(x == 0);
                pl.Place(15, 5, out x); Debug.Assert(x == 8);
                pl.Place(16, 5, out x); Debug.Assert(x == 6);
                pl.Place(17, 5, out x); Debug.Assert(x == 6);
                pl.Place(18, 5, out x); Debug.Assert(x == 6);
                pl.Place(19, 5, out x); Debug.Assert(x == 6);
                pl.Place(20, 5, out x); Debug.Assert(x == 6);
                pl.Place(21, 5, out x); Debug.Assert(x == 0);
                pl.Place(22, 5, out x); Debug.Assert(x == 0);

                pl.Place(0, 6, out x); Debug.Assert(x == 0);
                pl.Place(1, 6, out x); Debug.Assert(x == 0);
                pl.Place(2, 6, out x); Debug.Assert(x == -1);
                //pl.Place(3, 6, out x); Debug.Assert(x == 0);
                pl.Place(4, 6, out x); Debug.Assert(x == 7);
                pl.Place(5, 6, out x); Debug.Assert(x == 7);
                pl.Place(6, 6, out x); Debug.Assert(x == 7);
                pl.Place(7, 6, out x); Debug.Assert(x == 7);
                pl.Place(8, 6, out x); Debug.Assert(x == 7);
                pl.Place(9, 6, out x); Debug.Assert(x == 7);
                pl.Place(10, 6, out x); Debug.Assert(x == 5);
                pl.Place(11, 6, out x); Debug.Assert(x == 5);
                pl.Place(12, 6, out x); Debug.Assert(x == 5);
                //pl.Place(13, 6, out x); Debug.Assert(x == 0);
                pl.Place(14, 6, out x); Debug.Assert(x == 8);
                pl.Place(15, 6, out x); Debug.Assert(x == 8);
                pl.Place(16, 6, out x); Debug.Assert(x == 6);
                pl.Place(17, 6, out x); Debug.Assert(x == 6);
                pl.Place(18, 6, out x); Debug.Assert(x == 6);
                pl.Place(19, 6, out x); Debug.Assert(x == 6);
                pl.Place(20, 6, out x); Debug.Assert(x == 6);
                //pl.Place(21, 6, out x); Debug.Assert(x == 0);
                pl.Place(22, 6, out x); Debug.Assert(x == 0);

                pl.Place(0, 7, out x); Debug.Assert(x == 0);
                pl.Place(1, 7, out x); Debug.Assert(x == 0);
                pl.Place(2, 7, out x); Debug.Assert(x == -1);
                pl.Place(3, 7, out x); Debug.Assert(x == -1);
                pl.Place(4, 7, out x); Debug.Assert(x == 7);
                pl.Place(5, 7, out x); Debug.Assert(x == 7);
                pl.Place(6, 7, out x); Debug.Assert(x == 7);
                pl.Place(7, 7, out x); Debug.Assert(x == 7);
                pl.Place(8, 7, out x); Debug.Assert(x == 7);
                pl.Place(9, 7, out x); Debug.Assert(x == 7);
                //pl.Place(10, 7, out x); Debug.Assert(x == 0);
                pl.Place(11, 7, out x); Debug.Assert(x == 9);
                pl.Place(12, 7, out x); Debug.Assert(x == 9);
                pl.Place(13, 7, out x); Debug.Assert(x == 9);
                pl.Place(14, 7, out x); Debug.Assert(x == 8);
                pl.Place(15, 7, out x); Debug.Assert(x == 8);
                pl.Place(16, 7, out x); Debug.Assert(x == 8);
                pl.Place(17, 7, out x); Debug.Assert(x == 6);
                pl.Place(18, 7, out x); Debug.Assert(x == 6);
                pl.Place(19, 7, out x); Debug.Assert(x == 6);
                pl.Place(20, 7, out x); Debug.Assert(x == 0);
                pl.Place(21, 7, out x); Debug.Assert(x == 0);
                pl.Place(22, 7, out x); Debug.Assert(x == 0);

                pl.Place(0, 8, out x); Debug.Assert(x == 0);
                pl.Place(1, 8, out x); Debug.Assert(x == 0);
                pl.Place(2, 8, out x); Debug.Assert(x == -1);
                pl.Place(3, 8, out x); Debug.Assert(x == -1);
                //pl.Place(4, 8, out x); Debug.Assert(x == 0);
                pl.Place(5, 8, out x); Debug.Assert(x == 7);
                pl.Place(6, 8, out x); Debug.Assert(x == 7);
                pl.Place(7, 8, out x); Debug.Assert(x == 7);
                pl.Place(8, 8, out x); Debug.Assert(x == 7);
                pl.Place(9, 8, out x); Debug.Assert(x == 10);
                pl.Place(10, 8, out x); Debug.Assert(x == 10);
                //pl.Place(11, 8, out x); Debug.Assert(x == 0);
                pl.Place(12, 8, out x); Debug.Assert(x == 9);
                pl.Place(13, 8, out x); Debug.Assert(x == 9);
                pl.Place(14, 8, out x); Debug.Assert(x == 8);
                pl.Place(15, 8, out x); Debug.Assert(x == 8);
                pl.Place(16, 8, out x); Debug.Assert(x == 8);
                pl.Place(17, 8, out x); Debug.Assert(x == 6);
                pl.Place(18, 8, out x); Debug.Assert(x == 6);
                pl.Place(19, 8, out x); Debug.Assert(x == 0);
                pl.Place(20, 8, out x); Debug.Assert(x == 0);
                pl.Place(21, 8, out x); Debug.Assert(x == 0);
                pl.Place(22, 8, out x); Debug.Assert(x == 0);

                pl.Place(0, 9, out x); Debug.Assert(x == 0);
                //pl.Place(1, 9, out x); Debug.Assert(x == 0);
                pl.Place(2, 9, out x); Debug.Assert(x == 11);
                pl.Place(3, 9, out x); Debug.Assert(x == 11);
                pl.Place(4, 9, out x); Debug.Assert(x == 11);
                pl.Place(5, 9, out x); Debug.Assert(x == 11);
                pl.Place(6, 9, out x); Debug.Assert(x == 11);
                //pl.Place(7, 9, out x); Debug.Assert(x == 0);
                pl.Place(8, 9, out x); Debug.Assert(x == 10);
                pl.Place(9, 9, out x); Debug.Assert(x == 10);
                pl.Place(10, 9, out x); Debug.Assert(x == 10);
                pl.Place(11, 9, out x); Debug.Assert(x == 10);
                //pl.Place(12, 9, out x); Debug.Assert(x == 0);
                pl.Place(13, 9, out x); Debug.Assert(x == 9);
                pl.Place(14, 9, out x); Debug.Assert(x == 8);
                pl.Place(15, 9, out x); Debug.Assert(x == 8);
                pl.Place(16, 9, out x); Debug.Assert(x == 8);
                //pl.Place(17, 9, out x); Debug.Assert(x == 0);
                pl.Place(18, 9, out x); Debug.Assert(x == 0);
                pl.Place(19, 9, out x); Debug.Assert(x == 0);
                pl.Place(20, 9, out x); Debug.Assert(x == 0);
                pl.Place(21, 9, out x); Debug.Assert(x == 0);
                pl.Place(22, 9, out x); Debug.Assert(x == 0);

                pl.Place(0, 10, out x); Debug.Assert(x == 0);
                pl.Place(1, 10, out x); Debug.Assert(x == 0);
                pl.Place(2, 10, out x); Debug.Assert(x == 11);
                pl.Place(3, 10, out x); Debug.Assert(x == 11);
                pl.Place(4, 10, out x); Debug.Assert(x == 11);
                pl.Place(5, 10, out x); Debug.Assert(x == 11);
                //pl.Place(6, 10, out x); Debug.Assert(x == 0);
                pl.Place(7, 10, out x); Debug.Assert(x == 0);
                pl.Place(8, 10, out x); Debug.Assert(x == 0);
                pl.Place(9, 10, out x); Debug.Assert(x == 10);
                pl.Place(10, 10, out x); Debug.Assert(x == 10);
                pl.Place(11, 10, out x); Debug.Assert(x == 10);
                pl.Place(12, 10, out x); Debug.Assert(x == 10);
                //pl.Place(13, 10, out x); Debug.Assert(x == 0);
                pl.Place(14, 10, out x); Debug.Assert(x == 8);
                pl.Place(15, 10, out x); Debug.Assert(x == 8);
                pl.Place(16, 10, out x); Debug.Assert(x == 0);
                pl.Place(17, 10, out x); Debug.Assert(x == 0);
                pl.Place(18, 10, out x); Debug.Assert(x == 0);
                pl.Place(19, 10, out x); Debug.Assert(x == 0);
                pl.Place(20, 10, out x); Debug.Assert(x == 0);
                pl.Place(21, 10, out x); Debug.Assert(x == 0);
                pl.Place(22, 10, out x); Debug.Assert(x == 0);

                pl.Place(0, 11, out x); Debug.Assert(x == 0);
                pl.Place(1, 11, out x); Debug.Assert(x == 0);
                //pl.Place(2, 11, out x); Debug.Assert(x == 11);
                pl.Place(3, 11, out x); Debug.Assert(x == 11);
                pl.Place(4, 11, out x); Debug.Assert(x == 11);
                //pl.Place(5, 11, out x); Debug.Assert(x == 0);
                pl.Place(6, 11, out x); Debug.Assert(x == 0);
                pl.Place(7, 11, out x); Debug.Assert(x == 0);
                pl.Place(8, 11, out x); Debug.Assert(x == 0);
                pl.Place(9, 11, out x); Debug.Assert(x == 0);
                pl.Place(10, 11, out x); Debug.Assert(x == 10);
                pl.Place(11, 11, out x); Debug.Assert(x == 10);
                pl.Place(12, 11, out x); Debug.Assert(x == 10);
                pl.Place(13, 11, out x); Debug.Assert(x == 10);
                //pl.Place(14, 11, out x); Debug.Assert(x == 0);
                pl.Place(15, 11, out x); Debug.Assert(x == 0);
                pl.Place(16, 11, out x); Debug.Assert(x == 0);
                pl.Place(17, 11, out x); Debug.Assert(x == 0);
                pl.Place(18, 11, out x); Debug.Assert(x == 0);
                pl.Place(19, 11, out x); Debug.Assert(x == 0);
                pl.Place(20, 11, out x); Debug.Assert(x == 0);
                pl.Place(21, 11, out x); Debug.Assert(x == 0);
                pl.Place(22, 11, out x); Debug.Assert(x == 0);

                pl.Place(0, 12, out x); Debug.Assert(x == 0);
                pl.Place(1, 12, out x); Debug.Assert(x == 0);
                pl.Place(2, 12, out x); Debug.Assert(x == 0);
                pl.Place(3, 12, out x); Debug.Assert(x == 11);
                //pl.Place(4, 12, out x); Debug.Assert(x == 0);
                pl.Place(5, 12, out x); Debug.Assert(x == 0);
                pl.Place(6, 12, out x); Debug.Assert(x == 0);
                pl.Place(7, 12, out x); Debug.Assert(x == 0);
                pl.Place(8, 12, out x); Debug.Assert(x == 0);
                pl.Place(9, 12, out x); Debug.Assert(x == 0);
                pl.Place(10, 12, out x); Debug.Assert(x == 0);
                pl.Place(11, 12, out x); Debug.Assert(x == 10);
                pl.Place(12, 12, out x); Debug.Assert(x == 10);
                //pl.Place(13, 12, out x); Debug.Assert(x == 0);
                pl.Place(14, 12, out x); Debug.Assert(x == 0);
                pl.Place(15, 12, out x); Debug.Assert(x == 0);
                pl.Place(16, 12, out x); Debug.Assert(x == 0);
                pl.Place(17, 12, out x); Debug.Assert(x == 0);
                pl.Place(18, 12, out x); Debug.Assert(x == 0);
                pl.Place(19, 12, out x); Debug.Assert(x == 0);
                pl.Place(20, 12, out x); Debug.Assert(x == 0);
                pl.Place(21, 12, out x); Debug.Assert(x == 0);
                pl.Place(22, 12, out x); Debug.Assert(x == 0);

                pl.Place(0, 13, out x); Debug.Assert(x == 0);
                pl.Place(1, 13, out x); Debug.Assert(x == 0);
                pl.Place(2, 13, out x); Debug.Assert(x == 0);
                //pl.Place(3, 13, out x); Debug.Assert(x == 0);
                pl.Place(4, 13, out x); Debug.Assert(x == 0);
                pl.Place(5, 13, out x); Debug.Assert(x == 0);
                pl.Place(6, 13, out x); Debug.Assert(x == 0);
                pl.Place(7, 13, out x); Debug.Assert(x == 0);
                pl.Place(8, 13, out x); Debug.Assert(x == 0);
                pl.Place(9, 13, out x); Debug.Assert(x == 0);
                pl.Place(10, 13, out x); Debug.Assert(x == 0);
                pl.Place(11, 13, out x); Debug.Assert(x == 0);
                //pl.Place(12, 13, out x); Debug.Assert(x == 0);
                pl.Place(13, 13, out x); Debug.Assert(x == 0);
                pl.Place(14, 13, out x); Debug.Assert(x == 0);
                pl.Place(15, 13, out x); Debug.Assert(x == 0);
                pl.Place(16, 13, out x); Debug.Assert(x == 0);
                pl.Place(17, 13, out x); Debug.Assert(x == 0);
                pl.Place(18, 13, out x); Debug.Assert(x == 0);
                pl.Place(19, 13, out x); Debug.Assert(x == 0);
                pl.Place(20, 13, out x); Debug.Assert(x == 0);
                pl.Place(21, 13, out x); Debug.Assert(x == 0);
                pl.Place(22, 13, out x); Debug.Assert(x == 0);

                pl.Place(0, 14, out x); Debug.Assert(x == 0);
                pl.Place(1, 14, out x); Debug.Assert(x == 0);
                pl.Place(2, 14, out x); Debug.Assert(x == 0);
                pl.Place(3, 14, out x); Debug.Assert(x == 0);
                pl.Place(4, 14, out x); Debug.Assert(x == 0);
                pl.Place(5, 14, out x); Debug.Assert(x == 0);
                pl.Place(6, 14, out x); Debug.Assert(x == 0);
                pl.Place(7, 14, out x); Debug.Assert(x == 0);
                pl.Place(8, 14, out x); Debug.Assert(x == 0);
                pl.Place(9, 14, out x); Debug.Assert(x == 0);
                pl.Place(10, 14, out x); Debug.Assert(x == 0);
                pl.Place(11, 14, out x); Debug.Assert(x == 0);
                pl.Place(12, 14, out x); Debug.Assert(x == 0);
                pl.Place(13, 14, out x); Debug.Assert(x == 0);
                pl.Place(14, 14, out x); Debug.Assert(x == 0);
                pl.Place(15, 14, out x); Debug.Assert(x == 0);
                pl.Place(16, 14, out x); Debug.Assert(x == 0);
                pl.Place(17, 14, out x); Debug.Assert(x == 0);
                pl.Place(18, 14, out x); Debug.Assert(x == 0);
                pl.Place(19, 14, out x); Debug.Assert(x == 0);
                pl.Place(20, 14, out x); Debug.Assert(x == 0);
                pl.Place(21, 14, out x); Debug.Assert(x == 0);
                pl.Place(22, 14, out x); Debug.Assert(x == 0);
            }

            public static void run()
            {
                t1();
                t2();
                t3();
                t4();
            }
        }
    }

    public class PointLocationProgram
    {
        public static void Main()
        {
            Test.T1.run();
            Test.TestUgly.Run(new string[0]);
        }
    }
}

