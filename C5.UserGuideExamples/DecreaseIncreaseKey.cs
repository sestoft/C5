// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Decrease key and increase key pattern chapter

// Compile with
//   csc /r:netstandard.dll /r:C5.dll DecreaseIncreaseKey.cs

using System;

namespace C5.UserGuideExamples
{
    internal class DecreaseIncreaseKey
    {
        public static void Main()
        {
            IPriorityQueue<Prio<string>> pq = new IntervalHeap<Prio<string>>();
            IPriorityQueueHandle<Prio<string>> h1 = null;
            IPriorityQueueHandle<Prio<string>> h2 = null;
            IPriorityQueueHandle<Prio<string>> h3 = null;

            pq.Add(ref h1, new Prio<string>("surfing", 10));
            pq.Add(ref h2, new Prio<string>("shopping", 6));
            pq.Add(ref h3, new Prio<string>("cleaning", 4));

            // The following is legal because +, - are overloaded on (Prio<D>,int):
            pq[h2] -= 5;
            pq[h1] += 5;

            while (!pq.IsEmpty)
            {
                Console.WriteLine(pq.DeleteMin());
            }
        }
    }

    internal struct Prio<D> : IComparable<Prio<D>> where D : class
    {
        public D Data { get; }
        public int Priority { get; }

        public Prio(D data, int priority)
        {
            Data = data;
            Priority = priority;
        }

        public int CompareTo(Prio<D> that) => Priority.CompareTo(that.Priority);

        public bool Equals(Prio<D> that) => Priority == that.Priority;

        public static Prio<D> operator +(Prio<D> tp, int delta) => new Prio<D>(tp.Data, tp.Priority + delta);

        public static Prio<D> operator -(Prio<D> tp, int delta) => new Prio<D>(tp.Data, tp.Priority - delta);

        public override string ToString() => string.Format("{0}[{1}]", Data, Priority);
    }
}
