// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

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

