// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: SortedIterationPatterns.cs for pattern chapter

// Compile with 
//   csc /r:C5.dll SortedIterationPatterns.cs 

using System;
using C5;

namespace SortedIterationPatterns
{
    class SortedIterationPatterns
    {
        public static void Main(String[] args)
        {
            ISorted<int> sorted = new TreeSet<int>();
            sorted.AddAll(new[] { 23, 29, 31, 37, 41, 43, 47, 53 });
            Console.WriteLine(sorted);
            if (args.Length == 1)
            {
                int n = int.Parse(args[0]);
                if (Predecessor(sorted, n, out int res))
                    Console.WriteLine("{0} has predecessor {1}", n, res);
                if (WeakPredecessor(sorted, n, out res))
                    Console.WriteLine("{0} has weak predecessor {1}", n, res);
                if (Successor(sorted, n, out res))
                    Console.WriteLine("{0} has successor {1}", n, res);
                if (WeakSuccessor(sorted, n, out res))
                    Console.WriteLine("{0} has weak successor {1}", n, res);
            }
            IterBeginEnd(sorted);
            IterBeginEndBackwards(sorted);
            IterIncExc(sorted, 29, 47);
            IterIncExcBackwards(sorted, 29, 47);
            IterIncEnd(sorted, 29);
            IterBeginExc(sorted, 47);
            IterIncInc(sorted, 29, 47);
            IterBeginInc(sorted, 47);
            IterExcExc(sorted, 29, 47);
            IterExcEnd(sorted, 29);
            IterExcInc(sorted, 29, 47);
        }

        // --- Predecessor and successor patterns --------------------

        // Find weak successor of y in coll, or return false

        public static bool WeakSuccessor<T>(ISorted<T> coll, T y, out T ySucc)
          where T : IComparable<T>
        {
            bool hasSucc,
              hasY = coll.Cut(y, out _, out _, out ySucc, out hasSucc);
            if (hasY)
                ySucc = y;
            return hasY || hasSucc;
        }

        // Find weak predecessor of y in coll, or return false

        public static bool WeakPredecessor<T>(ISorted<T> coll, T y, out T yPred)
          where T : IComparable<T>
        {
            bool hasPred, hasY = coll.Cut(y, out yPred, out hasPred, out _, out _);
            if (hasY)
                yPred = y;
            return hasY || hasPred;
        }

        // Find (strict) successor of y in coll, or return false

        public static bool Successor<T>(ISorted<T> coll, T y, out T ySucc)
          where T : IComparable<T>
        {
            coll.Cut(y, out _, out _, out ySucc, out bool hasSucc);
            return hasSucc;
        }

        // Find (strict) predecessor of y in coll, or return false

        public static bool Predecessor<T>(ISorted<T> coll, T y, out T yPred)
          where T : IComparable<T>
        {
            coll.Cut(y, out yPred, out bool hasPred, out _, out _);
            return hasPred;
        }

        // --- Sorted iteration patterns -----------------------------

        // Iterate over all items

        public static void IterBeginEnd<T>(ISorted<T> coll)
        {
            foreach (T x in coll)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over all items, backwards

        public static void IterBeginEndBackwards<T>(ISorted<T> coll)
        {
            foreach (T x in coll.Backwards())
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [x1,x2[

        public static void IterIncExc<T>(ISorted<T> coll, T x1, T x2)
        {
            foreach (T x in coll.RangeFromTo(x1, x2))
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [x1,x2[, backwards

        public static void IterIncExcBackwards<T>(ISorted<T> coll, T x1, T x2)
        {
            foreach (T x in coll.RangeFromTo(x1, x2).Backwards())
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [x1...]

        public static void IterIncEnd<T>(ISorted<T> coll, T x1)
        {
            foreach (T x in coll.RangeFrom(x1))
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [...x2[

        public static void IterBeginExc<T>(ISorted<T> coll, T x2)
        {
            foreach (T x in coll.RangeTo(x2))
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [x1...x2]

        public static void IterIncInc<T>(ISorted<T> coll, T x1, T x2)
          where T : IComparable<T>
        {
            bool x2HasSucc = Successor(coll, x2, out T x2Succ);
            IDirectedEnumerable<T> range =
              x2HasSucc ? coll.RangeFromTo(x1, x2Succ) : coll.RangeFrom(x1);
            foreach (T x in range)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over [...x2]

        public static void IterBeginInc<T>(ISorted<T> coll, T x2)
          where T : IComparable<T>
        {
            bool x2HasSucc = Successor(coll, x2, out T x2Succ);
            IDirectedEnumerable<T> range =
              x2HasSucc ? coll.RangeTo(x2Succ) : coll.RangeAll();
            foreach (T x in range)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over ]x1...x2[

        public static void IterExcExc<T>(ISorted<T> coll, T x1, T x2)
          where T : IComparable<T>
        {
            bool x1HasSucc = Successor(coll, x1, out T x1Succ);
            IDirectedEnumerable<T> range =
              x1HasSucc ? coll.RangeFromTo(x1Succ, x2) : new ArrayList<T>();
            foreach (T x in range)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over ]x1...]

        public static void IterExcEnd<T>(ISorted<T> coll, T x1)
          where T : IComparable<T>
        {
            bool x1HasSucc = Successor(coll, x1, out T x1Succ);
            IDirectedEnumerable<T> range =
              x1HasSucc ? coll.RangeFrom(x1Succ) : new ArrayList<T>();
            foreach (T x in range)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

        // Iterate over ]x1...x2]

        public static void IterExcInc<T>(ISorted<T> coll, T x1, T x2)
          where T : IComparable<T>
        {
            bool x1HasSucc = Successor(coll, x1, out T x1Succ),
                 x2HasSucc = Successor(coll, x2, out T x2Succ);
            IDirectedEnumerable<T> range =
              x1HasSucc ? (x2HasSucc ? coll.RangeFromTo(x1Succ, x2Succ)
                                     : coll.RangeFrom(x1Succ))
                        : new ArrayList<T>();
            foreach (T x in range)
            {
                Console.Write("{0} ", x);
            }
            Console.WriteLine();
        }

    }
}
