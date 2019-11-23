// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example
// 2004-11

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll SortingPermutation.cs 

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    internal class SortingPermutation
    {
        public static void Main()
        {
            string[] cities = { "Tokyo", "Beijing", "Hangzhou", "Kyoto", "Beijing", "Copenhagen", "Seattle" };

            IList<string> alst = new ArrayList<string>();
            alst.AddAll(cities);

            Console.WriteLine(string.Join(", ", MySort.GetPermutation1(alst)));
            Console.WriteLine();

            IList<string> llst = new LinkedList<string>();
            llst.AddAll(cities);

            Console.WriteLine(string.Join(", ", MySort.GetPermutation2(llst)));
            Console.WriteLine();

            Console.WriteLine("The rank of the cities:");
            var res = MySort.GetPermutation1(MySort.GetPermutation2(llst));
            Console.WriteLine(string.Join(", ", res));
            Console.WriteLine();
        }
    }

    internal class MySort
    {
        // Fast for array lists and similar, but not stable; slow for linked lists
        public static ArrayList<int> GetPermutation1<T>(IList<T> lst)
            where T : IComparable<T>
        {
            var res = new ArrayList<int>(lst.Count);

            for (var i = 0; i < lst.Count; i++)
            {
                res.Add(i);
            }

            res.Sort(ComparerFactory<int>.CreateComparer((i, j) => lst[i].CompareTo(lst[j])));
            return res;
        }

        // Stable and fairly fast both for array lists and linked lists, 
        // but does copy the collection's items. 
        public static ArrayList<int> GetPermutation2<T>(IList<T> lst)
            where T : IComparable<T>
        {
            var i = 0;
            var zipList = lst.Map(x => new System.Collections.Generic.KeyValuePair<T, int>(x, i++));
            zipList.Sort(new KeyValuePairKeyComparer<T>());
            var res = new ArrayList<int>(lst.Count);

            foreach (var p in zipList)
            {
                res.Add(p.Value);
            }

            return res;
        }

        private class KeyValuePairKeyComparer<T> : SCG.IComparer<System.Collections.Generic.KeyValuePair<T, int>>
            where T : IComparable<T>
        {
            public int Compare(System.Collections.Generic.KeyValuePair<T, int> p1, System.Collections.Generic.KeyValuePair<T, int> p2)
            {
                return p1.Key.CompareTo(p2.Key);
            }
        }
    }
}
