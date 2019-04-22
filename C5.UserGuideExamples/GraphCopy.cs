// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: graph copying 2005-11-08

// Compile with
//   csc /r:netstandard.dll /r:C5.dll GraphCopy.cs

using System;
using System.Text;

namespace C5.UserGuideExamples
{
    class GraphCopy
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Graphcopy <nodecount>\n");
            }
            else
            {
                var count = int.Parse(args[0]);
                var d = new Node<string>("d");
                var e = new Node<string>("e");
                var c = new Node<string>("c", d, e);
                var b = new Node<string>("b", d);
                var a = new Node<string>("a", d, b, c);
                a[1] = a;
                var newstart = CopyGraph1(a);
                Console.WriteLine($"Copy has same structure: {Isomorphic1(newstart, a)}");
                Console.WriteLine($"newstart = {newstart}");
                foreach (var node1 in newstart.children)
                {
                    Console.WriteLine(node1);
                }

                var node = new Node<string>("last");
                for (var i = 0; i < count; i++)
                {
                    node = new Node<string>(i.ToString(), node);
                }

                newstart = CopyGraph1(node);
                Console.WriteLine($"Copy has same structure: {Isomorphic1(newstart, node)}");
            }
        }

        // Graph copying 0
        public static Node<T> CopyGraph0<T>(Node<T> start)
        {
            var iso = new HashDictionary<Node<T>, Node<T>>(EqualityComparer<Node<T>>.Default);

            return CopyNode0(iso, start);
        }

        private static Node<T> CopyNode0<T>(IDictionary<Node<T>, Node<T>> iso, Node<T> old)
        {
            if (!iso.Find(ref old, out var copy))
            {
                copy = new Node<T>(old);
                iso[old] = copy;
                for (var i = 0; i < copy.children.Length; i++)
                {
                    copy.children[i] = CopyNode0(iso, old.children[i]);
                }
            }
            return copy;
        }

        // Graph copying 1, using an explicit stack of pending nodes.
        // Every new node is added to the stack exactly once.  Every node
        // in the stack is also in the key set of the dictionary.
        public static Node<T> CopyGraph1<T>(Node<T> start)
        {
            var iso = new HashDictionary<Node<T>, Node<T>>(EqualityComparer<Node<T>>.Default);
            IStack<Node<T>> work = new ArrayList<Node<T>>();
            iso[start] = new Node<T>(start);
            work.Push(start);
            while (!work.IsEmpty)
            {
                var node = work.Pop();
                var copy = iso[node];
                for (var i = 0; i < node.children.Length; i++)
                {
                    var child = node.children[i];
                    if (!iso.Find(ref child, out var childCopy))
                    {
                        iso[child] = childCopy = new Node<T>(child);
                        work.Push(child);
                    }
                    copy.children[i] = childCopy;
                }
            }
            return iso[start];
        }

        // Graph equality 0
        public static bool Isomorphic0<T>(Node<T> start1, Node<T> start2)
        {
            var iso = new HashDictionary<Node<T>, Node<T>>(EqualityComparer<Node<T>>.Default);

            return NodeEquals0(iso, start1, start2);
        }

        private static bool NodeEquals0<T>(IDictionary<Node<T>, Node<T>> iso, Node<T> left, Node<T> rght)
        {
            if (iso.Find(ref left, out var node))
            {
                return ReferenceEquals(node, rght);
            }
            else if (left.children.Length != rght.children.Length)
            {
                return false;
            }
            else
            {
                iso[left] = rght;
                for (var i = 0; i < left.children.Length; i++)
                {
                    if (!NodeEquals0(iso, left.children[i], rght.children[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        // Graph equality 1, using an explicit stack of pending nodes.
        // If iso[left] = rght, we are trying to prove that left and rght
        // are the roots of isomorphic subgraphs.  The work stack contains
        // the assumptions needed to prove this.  Whenever node is on the
        // work stack, then node is also in the key set of iso.  
        public static bool Isomorphic1<T>(Node<T> start1, Node<T> start2)
        {
            var iso = new HashDictionary<Node<T>, Node<T>>(EqualityComparer<Node<T>>.Default)
            {
                [start1] = start2
            };
            IStack<Node<T>> work = new ArrayList<Node<T>>();
            work.Push(start1);
            while (!work.IsEmpty)
            {
                var left = work.Pop();
                var rght = iso[left];
                if (left.children.Length != rght.children.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < left.children.Length; i++)
                    {
                        var lchild = left.children[i];
                        var rchild = rght.children[i];
                        if (iso.Find(ref lchild, out var node))
                        {
                            if (!ReferenceEquals(node, rchild))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            iso[lchild] = rchild;
                            work.Push(lchild);
                        }
                    }
                }
            }
            return true;
        }
    }

    public class Node<T>
    {
        public readonly T id;
        public readonly Node<T>[] children;

        public Node(T id, params Node<T>[] children)
        {
            this.id = id; this.children = children;
        }

        public Node(Node<T> node) : this(node.id, new Node<T>[node.children.Length])
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(id).Append("[ ");

            foreach (var child in children)
            {
                sb.Append(child.id).Append(" ");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public Node<T> this[int i]
        {
            set { children[i] = value; }
            get { return children[i]; }
        }
    }
}
