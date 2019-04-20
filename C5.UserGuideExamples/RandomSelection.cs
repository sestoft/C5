// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: RandomSelection.cs for pattern chapter

// Compile with 
//   csc /r:C5.dll RandomSelection.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace RandomSelection
{
    class RandomSelection
    {
        public static void Main(String[] args)
        {
            ArrayList<int> list = new ArrayList<int>(), copy1, copy2;
            list.AddAll(new[] { 2, 3, 5, 7, 11, 13, 17, 19 });
            copy1 = new ArrayList<int>();
            copy1.AddAll(list);
            copy2 = new ArrayList<int>();
            copy2.AddAll(list);
            const int N = 7;
            Console.WriteLine("-- With replacement:");
            foreach (int x in RandomWith(list, N))
                Console.Write("{0} ", x);
            Console.WriteLine("\n-- Without replacement:");
            foreach (int x in RandomWithout1(copy1, N))
                Console.Write("{0} ", x);
            Console.WriteLine("\n-- Without replacement:");
            foreach (int x in RandomWithout2(copy2, N))
                Console.Write("{0} ", x);
            Console.WriteLine();
        }

        private static readonly C5Random rnd = new C5Random();

        // Select N random items from coll, with replacement.
        // Does not modify the given list.

        public static SCG.IEnumerable<T> RandomWith<T>(IIndexed<T> coll, int N)
        {
            for (int i = N; i > 0; i--)
            {
                T x = coll[rnd.Next(coll.Count)];
                yield return x;
            }
        }

        // Select N random items from list, without replacement.
        // Modifies the given list.

        public static SCG.IEnumerable<T> RandomWithout1<T>(IList<T> list, int N)
        {
            list.Shuffle(rnd);
            foreach (T x in list.View(0, N))
                yield return x;
        }

        // Select N random items from list, without replacement.
        // Faster when list is efficiently indexable and modifiable.
        // Modifies the given list.

        public static SCG.IEnumerable<T> RandomWithout2<T>(ArrayList<T> list, int N)
        {
            for (int i = N; i > 0; i--)
            {
                int j = rnd.Next(list.Count);
                T x = list[j], replacement = list.RemoveLast();
                if (j < list.Count)
                    list[j] = replacement;
                yield return x;
            }
        }
    }
}
