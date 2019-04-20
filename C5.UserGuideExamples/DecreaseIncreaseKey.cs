// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Decrease key and increase key pattern chapter

// Compile with 
//   csc /r:C5.dll DecreaseIncreaseKey.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

class MyTest {
  public static void Main(String[] args) {
    IPriorityQueue<Prio<String>> pq = new IntervalHeap<Prio<String>>();
    IPriorityQueueHandle<Prio<String>> h1, h2, h3;
    h1 = h2 = h3 = null;
    pq.Add(ref h1, new Prio<String>("surfing", 10));
    pq.Add(ref h2, new Prio<String>("shopping", 6));
    pq.Add(ref h3, new Prio<String>("cleaning", 4));
    // The following is legal because +, - are overloaded on (Prio<D>,int):
    pq[h2] -= 5;
    pq[h1] += 5;
    while (!pq.IsEmpty) 
      Console.WriteLine(pq.DeleteMin());
  }
}

struct Prio<D> : IComparable<Prio<D>> where D : class  {
  public readonly D data;
  private int priority;

  public Prio(D data, int priority) {
    this.data = data; 
    this.priority = priority;
  }
  
  public int CompareTo(Prio<D> that) {
    return this.priority.CompareTo(that.priority);
  }

  public bool Equals(Prio<D> that) {
    return this.priority == that.priority;
  }

  public static Prio<D> operator+(Prio<D> tp, int delta) {
    return new Prio<D>(tp.data, tp.priority + delta);
  }

  public static Prio<D> operator-(Prio<D> tp, int delta) {
    return new Prio<D>(tp.data, tp.priority - delta);
  }

  public override String ToString() {
    return String.Format("{0}[{1}]", data, priority);
  }
}
