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

// C5 example: 2006-06-17

// Compile with 
//   csc /r:C5.dll Anagrams.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace MyTest {
  class MyTest {
    public static void Main(String[] args) {
      PrintEvents("CircularQueue", new CircularQueue<int>());
      PrintEvents("ArrayList", new ArrayList<int>());
      PrintEvents("LinkedList", new LinkedList<int>());
      PrintEvents("HashedArrayList", new HashedArrayList<int>());
      PrintEvents("HashedLinkedList", new HashedLinkedList<int>());
      PrintEvents("SortedArray", new SortedArray<int>());
      PrintEvents("WrappedArray", new WrappedArray<int>(new int[0]));
      PrintEvents("TreeSet", new TreeSet<int>());
      PrintEvents("TreeBag", new TreeBag<int>());
      PrintEvents("HashSet", new HashSet<int>());
      PrintEvents("HashBag", new HashBag<int>());
      PrintEvents("IntervalHeap", new IntervalHeap<int>());

      PrintEvents("HashDictionary", new HashDictionary<int,int>());
      PrintEvents("TreeDictionary", new TreeDictionary<int,int>());
    }

    public static void PrintEvents<T>(String kind, ICollectionValue<T> coll) {
      Console.WriteLine("{0,25} {1}", kind, coll.ListenableEvents);
    }
  }
}
