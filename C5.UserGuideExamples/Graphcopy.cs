/*
 Copyright (c) 2003-2006 Niels Kokholm <kokholm@itu.dk> and Peter Sestoft <sestoft@dina.kvl.dk>
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

// C5 example: graph copying 2005-11-08

// Compile with 
//   csc /r:C5.dll Graphcopy.cs 

using System;
using System.Text;
using C5;
using SCG = System.Collections.Generic;

namespace Graphcopy {
  class TestGraphcopy {
    public static void Main(String[] args) {
      if (args.Length != 1) 
	Console.WriteLine("Usage: Graphcopy <nodecount>\n");
      else {
	int count = int.Parse(args[0]);
	Node<String> 
	  d = new Node<String>("d"), 
	  e = new Node<String>("e"),
	  c = new Node<String>("c", d, e),
	  b = new Node<String>("b", d),
	  a = new Node<String>("a", d, b, c);
	a[1] = a;
	Node<String> newstart = CopyGraph1(a);
	Console.WriteLine("Copy has same structure: " + Isomorphic1(newstart, a));
	Console.WriteLine("newstart = " + newstart);
	foreach (Node<String> node1 in newstart.children)
	  Console.WriteLine(node1);
	Node<String> node = new Node<String>("last");
	for (int i=0; i<count; i++)
	  node = new Node<String>(i.ToString(), node);
	newstart = CopyGraph1(node);
	Console.WriteLine("Copy has same structure: " 
			  + Isomorphic1(newstart, node));
      }
    }
    
    // Graph copying 0
    
    public static Node<T> CopyGraph0<T>(Node<T> start) {
      IDictionary<Node<T>,Node<T>> iso 
        = new HashDictionary<Node<T>,Node<T>>
              (ReferenceEqualityComparer<Node<T>>.Default);
      return CopyNode0(iso, start);
    }

    private static Node<T> CopyNode0<T>(IDictionary<Node<T>,Node<T>> iso, 
                                        Node<T> old) {
      Node<T> copy;
      if (!iso.Find(old, out copy)) {
        copy = new Node<T>(old);
        iso[old] = copy;
        for (int i=0; i<copy.children.Length; i++)
          copy.children[i] = CopyNode0(iso, old.children[i]);
      }
      return copy;
    }

    // Graph copying 1, using an explicit stack of pending nodes.
    // Every new node is added to the stack exactly once.  Every node
    // in the stack is also in the key set of the dictionary.
    
    public static Node<T> CopyGraph1<T>(Node<T> start) {
      IDictionary<Node<T>,Node<T>> iso 
        = new HashDictionary<Node<T>,Node<T>>
              (ReferenceEqualityComparer<Node<T>>.Default);
      IStack<Node<T>> work = new ArrayList<Node<T>>();
      iso[start] = new Node<T>(start);
      work.Push(start);
      while (!work.IsEmpty) {
        Node<T> node = work.Pop(), copy = iso[node];
        for (int i=0; i<node.children.Length; i++) {
          Node<T> child = node.children[i];
          Node<T> childCopy;
          if (!iso.Find(child, out childCopy)) {
            iso[child] = childCopy = new Node<T>(child);
            work.Push(child);
          }
          copy.children[i] = childCopy;
        }
      }
      return iso[start];
    }

    // Graph equality 0

    public static bool Isomorphic0<T>(Node<T> start1, Node<T> start2) {
      IDictionary<Node<T>,Node<T>> iso
        = new HashDictionary<Node<T>,Node<T>>
              (ReferenceEqualityComparer<Node<T>>.Default);
      return NodeEquals0(iso, start1, start2);
    }

    private static bool NodeEquals0<T>(IDictionary<Node<T>,Node<T>> iso, 
                                       Node<T> left, Node<T> rght) {
      Node<T> node;
      if (iso.Find(left, out node))
        return Object.ReferenceEquals(node, rght);
      else if (left.children.Length != rght.children.Length)
        return false;
      else {
        iso[left] = rght;
        for (int i=0; i<left.children.Length; i++)
          if (!NodeEquals0(iso, left.children[i], rght.children[i]))
            return false;
        return true;
      } 
    }

    // Graph equality 1, using an explicit stack of pending nodes.
    // If iso[left] = rght, we are trying to prove that left and rght
    // are the roots of isomorphic subgraphs.  The work stack contains
    // the assumptions needed to prove this.  Whenever node is on the
    // work stack, then node is also in the key set of iso.  

    public static bool Isomorphic1<T>(Node<T> start1, Node<T> start2) {
      IDictionary<Node<T>,Node<T>> iso
        = new HashDictionary<Node<T>,Node<T>>
              (ReferenceEqualityComparer<Node<T>>.Default);
      IStack<Node<T>> work = new ArrayList<Node<T>>();
      iso[start1] = start2;
      work.Push(start1);
      while (!work.IsEmpty) {
        Node<T> left = work.Pop(), rght = iso[left];
        if (left.children.Length != rght.children.Length)
          return false;
        else {
          for (int i=0; i<left.children.Length; i++) {
            Node<T> lchild = left.children[i], rchild = rght.children[i];
            Node<T> node;
            if (iso.Find(lchild, out node)) {
              if (!Object.ReferenceEquals(node, rchild))
                return false;
            } else {
              iso[lchild] = rchild;
              work.Push(lchild);
            }
          }
        } 
      }
      return true;
    }
  }

  public class Node<T> { 
    public readonly T id;
    public readonly Node<T>[] children;

    public Node(T id, params Node<T>[] children) { 
      this.id = id; this.children = children;
    }

    public Node(Node<T> node) : this(node.id, new Node<T>[node.children.Length])
    { }

    public override String ToString() { 
      StringBuilder sb = new StringBuilder();
      sb.Append(id).Append("[ ");
      foreach (Node<T> child in children)
        sb.Append(child.id).Append(" ");
      sb.Append("]");
      return sb.ToString();
    }

    public Node<T> this[int i] {
      set { children[i] = value; }
      get { return children[i]; }
    }
  }
}
