// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example
// 2004-11-09

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll TreeTraversal.cs 

using System;

namespace C5.UserGuideExamples
{
    internal class TreeTraversal
    {
        public static void Main()
        {
            var t = MakeTree(1, 15);
            void action(int val) => Console.Write("{0} ", val);
            Console.WriteLine("Depth-first:");
            Tree<int>.DepthFirst(t, action);
            Console.WriteLine("\nBreadth-first:");
            Tree<int>.BreadthFirst(t, action);
            Console.WriteLine("\nDepth-first:");
            Tree<int>.Traverse(t, action, new ArrayList<Tree<int>>());
            Console.WriteLine("\nBreadth-first:");
            Tree<int>.Traverse(t, action, new LinkedList<Tree<int>>());
            Console.WriteLine();
        }

        // Build n-node tree with root numbered b and other nodes numbered b+1..b+n
        public static Tree<int> MakeTree(int b, int n)
        {
            if (n == 0)
            {
                return null;
            }
            else
            {
                var k = n / 2;
                var t1 = MakeTree(b + 1, k);
                var t2 = MakeTree(b + k + 1, n - 1 - k);
                return new Tree<int>(b, t1, t2);
            }
        }
    }

    internal class Tree<T>
    {
        private readonly T _val;
        private readonly Tree<T> _t1;
        private readonly Tree<T> _t2;

        public Tree(T val) : this(val, null, null) { }
        public Tree(T val, Tree<T> t1, Tree<T> t2)
        {
            _val = val;
            _t1 = t1;
            _t2 = t2;
        }

        public static void DepthFirst(Tree<T> t, Action<T> action)
        {
            IStack<Tree<T>> work = new ArrayList<Tree<T>>();
            work.Push(t);
            while (!work.IsEmpty)
            {
                var cur = work.Pop();
                if (cur != null)
                {
                    work.Push(cur._t2);
                    work.Push(cur._t1);
                    action(cur._val);
                }
            }
        }

        public static void BreadthFirst(Tree<T> t, Action<T> action)
        {
            IQueue<Tree<T>> work = new CircularQueue<Tree<T>>();
            work.Enqueue(t);
            while (!work.IsEmpty)
            {
                var cur = work.Dequeue();
                if (cur != null)
                {
                    work.Enqueue(cur._t1);
                    work.Enqueue(cur._t2);
                    action(cur._val);
                }
            }
        }

        public static void Traverse(Tree<T> t, Action<T> action, IList<Tree<T>> work)
        {
            work.Clear();
            work.Add(t);
            while (!work.IsEmpty)
            {
                var cur = work.Remove();
                if (cur != null)
                {
                    if (work.FIFO)
                    {
                        work.Add(cur._t1);
                        work.Add(cur._t2);
                    }
                    else
                    {
                        work.Add(cur._t2);
                        work.Add(cur._t1);
                    }
                    action(cur._val);
                }
            }
        }
    }
}