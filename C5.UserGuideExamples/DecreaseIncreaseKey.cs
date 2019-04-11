/*
 Copyright (c) 2003-2019 Niels Kokholm, Peter Sestoft, and Rasmus Lystrøm
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

// C5 example: Decrease key and increase key pattern chapter

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.DecreaseIncreaseKey
//  dotnet run

using System;

namespace C5.UserGuideExamples
{
    class DecreaseIncreaseKey
    {
        public static void Main(string[] args)
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

    struct Prio<D> : IComparable<Prio<D>> where D : class
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
