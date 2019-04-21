// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example
// 2004-11-09

using System;
using C5;

namespace TreeTraversal
{
  class MyTest
  {
    public static void Main(String[] args)
    {
      Tree<int> t = MakeTree(1, 15);
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
        return null;
      else
      {
        int k = n / 2;
        Tree<int> t1 = MakeTree(b + 1, k), t2 = MakeTree(b + k + 1, n - 1 - k);
        return new Tree<int>(b, t1, t2);
      }
    }
  }

  class Tree<T>
  {
    private T val;
    private Tree<T> t1, t2;
    public Tree(T val) : this(val, null, null) { }
    public Tree(T val, Tree<T> t1, Tree<T> t2)
    {
      this.val = val; this.t1 = t1; this.t2 = t2;
    }

    public static void DepthFirst(Tree<T> t, Action<T> action)
    {
      IStack<Tree<T>> work = new ArrayList<Tree<T>>();
      work.Push(t);
      while (!work.IsEmpty)
      {
        Tree<T> cur = work.Pop();
        if (cur != null)
        {
          work.Push(cur.t2);
          work.Push(cur.t1);
          action(cur.val);
        }
      }
    }

    public static void BreadthFirst(Tree<T> t, Action<T> action)
    {
      IQueue<Tree<T>> work = new CircularQueue<Tree<T>>();
      work.Enqueue(t);
      while (!work.IsEmpty)
      {
        Tree<T> cur = work.Dequeue();
        if (cur != null)
        {
          work.Enqueue(cur.t1);
          work.Enqueue(cur.t2);
          action(cur.val);
        }
      }
    }

    public static void Traverse(Tree<T> t, Action<T> action, IList<Tree<T>> work)
    {
      work.Clear();
      work.Add(t);
      while (!work.IsEmpty)
      {
        Tree<T> cur = work.Remove();
        if (cur != null)
        {
          if (work.FIFO)
          {
            work.Add(cur.t1);
            work.Add(cur.t2);
          }
          else
          {
            work.Add(cur.t2);
            work.Add(cur.t1);
          }
          action(cur.val);
        }
      }
    }
  }
}