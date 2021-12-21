// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: topological sorting 2005-09-09

namespace C5.UserGuideExamples;

internal class Toposort
{
    public static void Main()
    {
        MyNode<string>
          d = new MyNode<string>("d"),
          e = new MyNode<string>("e"),
          c = new MyNode<string>("c", d, e),
          b = new MyNode<string>("b", d),
          a = new MyNode<string>("a", d, b, c);

        foreach (var n in Toposort0(a))
        {
            Console.WriteLine(n);
        }

        Console.WriteLine();

        foreach (var n in Toposort1(a))
        {
            Console.WriteLine(n);
        }

        Console.WriteLine();

        foreach (var n in Toposort2(a))
        {
            Console.WriteLine(n);
        }
    }

    // Toposort 0, adding each node when finished, after its descendants.
    // Classic depth-first search.  Does not terminate on cyclic graphs.
    public static IList<MyNode<T>> Toposort0<T>(params MyNode<T>[] starts)
    {
        var sorted = new HashedLinkedList<MyNode<T>>();

        foreach (var start in starts)
        {
            if (!sorted.Contains(start))
            {
                AddNode0(sorted, start);
            }
        }

        return sorted;
    }

    private static void AddNode0<T>(IList<MyNode<T>> sorted, MyNode<T> node)
    {
        Debug.Assert(!sorted.Contains(node));

        foreach (var child in node.Children)
        {
            if (!sorted.Contains(child))
            {
                AddNode0(sorted, child);
            }
        }

        sorted.InsertLast(node);
    }

    // Toposort 1, using hash index to add each node before its descendants.
    // Terminates also on cyclic graphs.
    public static IList<MyNode<T>> Toposort1<T>(params MyNode<T>[] starts)
    {
        var sorted = new HashedLinkedList<MyNode<T>>();

        foreach (var start in starts)
        {
            if (!sorted.Contains(start))
            {
                sorted.InsertLast(start);
                AddNode1(sorted, start);
            }
        }

        return sorted;
    }

    private static void AddNode1<T>(IList<MyNode<T>> sorted, MyNode<T> node)
    {
        Debug.Assert(sorted.Contains(node));
        foreach (var child in node.Children)
        {
            if (!sorted.Contains(child))
            {
                sorted.ViewOf(node).InsertFirst(child);
                AddNode1(sorted, child);
            }
        }
    }

    // Toposort 2, node rescanning using a view.
    // Uses no method call stack and no extra data structures, but slower.
    public static IList<MyNode<T>> Toposort2<T>(params MyNode<T>[] starts)
    {
        var sorted = new HashedLinkedList<MyNode<T>>();
        foreach (MyNode<T> start in starts)
        {
            if (!sorted.Contains(start))
            {
                sorted.InsertLast(start);
                using (IList<MyNode<T>> cursor = sorted.View(sorted.Count - 1, 1))
                {
                    do
                    {
                        MyNode<T> child;
                        while ((child = PendingChild(sorted, cursor.First)) != null)
                        {
                            cursor.InsertFirst(child);
                            cursor.Slide(0, 1);
                        }
                    }
                    while (cursor.TrySlide(+1));
                }
            }
        }

        return sorted;
    }

    private static MyNode<T> PendingChild<T>(IList<MyNode<T>> sorted, MyNode<T> node)
    {
        foreach (var child in node.Children)
        {
            if (!sorted.Contains(child))
            {
                return child;
            }
        }

        return null;
    }
}

internal class MyNode<T>
{
    public T Id { get; }

    public MyNode<T>[] Children { get; }

    public MyNode(T id, params MyNode<T>[] children)
    {
        Id = id;
        Children = children;
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public MyNode<T> this[int i]
    {
        set => Children[i] = value;
        get => Children[i];
    }
}
