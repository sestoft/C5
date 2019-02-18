/*
 Copyright (c) 2003-2019 Niels Kokholm, Peter Sestoft, and Rasmus Lystr�m
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

// C5 example: hash codes, good and bad 2005-02-28

// Compile with 
//   csc /r:C5.dll HashCodes.cs 

using System;
using System.Diagnostics;
using C5;
using SCG = System.Collections.Generic;

namespace MyHashCodesTest
{
    class MyTest
    {
        public static void Main(String[] args)
        {
            int count = int.Parse(args[0]);
            {
                Console.Write("Good hash function: ");
                var sw = Stopwatch.StartNew();
                HashSet<int> good
                  = MakeRandom(count, new GoodIntegerEqualityComparer());
                sw.Stop();
                Console.WriteLine("({0} sec, {1} items)", sw.Elapsed.TotalSeconds, good.Count);
                ISortedDictionary<int, int> bcd = good.BucketCostDistribution();
                foreach (KeyValuePair<int, int> entry in bcd)
                    Console.WriteLine("{0,7} bucket(s) with cost {1,5}",
                              entry.Value, entry.Key);
            }
            {
                Console.Write("Bad hash function:  ");
                var sw = Stopwatch.StartNew();
                HashSet<int> bad = MakeRandom(count, new BadIntegerEqualityComparer());
                sw.Stop();
                Console.WriteLine("({0} sec, {1} items)", sw.Elapsed.TotalSeconds, bad.Count);
                ISortedDictionary<int, int> bcd = bad.BucketCostDistribution();
                foreach (KeyValuePair<int, int> entry in bcd)
                    Console.WriteLine("{0,7} bucket(s) with cost {1,5}",
                              entry.Value, entry.Key);
            }
        }

        private static readonly C5Random rnd = new C5Random();

        public static HashSet<int> MakeRandom(int count,
                          SCG.IEqualityComparer<int> eqc)
        {
            HashSet<int> res;
            if (eqc == null)
                res = new HashSet<int>();
            else
                res = new HashSet<int>(eqc);
            for (int i = 0; i < count; i++)
                res.Add(rnd.Next(1000000));
            return res;
        }

        private class BadIntegerEqualityComparer : SCG.IEqualityComparer<int>
        {
            public bool Equals(int i1, int i2)
            {
                return i1 == i2;
            }
            public int GetHashCode(int i)
            {
                return i % 7;
            }
        }

        private class GoodIntegerEqualityComparer : SCG.IEqualityComparer<int>
        {
            public bool Equals(int i1, int i2)
            {
                return i1 == i2;
            }
            public int GetHashCode(int i)
            {
                return i;
            }
        }
    }
}

