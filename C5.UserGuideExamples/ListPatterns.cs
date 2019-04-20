// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: ListPatterns.cs for pattern chapter

// Compile with 
//   csc /r:C5.dll ListPatterns.cs 

using System;
using C5;

namespace ListPatterns
{
    class ListPatterns
    {
        public static void Main(String[] args)
        {
            IList<int> list = new ArrayList<int>();
            list.AddAll(new[] { 23, 29, 31, 37, 41, 43, 47, 53 });
            // Reversing and swapping
            Console.WriteLine(list);
            list.Reverse();
            Console.WriteLine(list);
            ReverseInterval(list, 2, 3);
            Console.WriteLine(list);
            SwapInitialFinal(list, 2);
            Console.WriteLine(list);
            // Clearing all or part of list
            list.CollectionCleared
          += delegate(Object c, ClearedEventArgs eargs)
            {
                if (eargs is ClearedRangeEventArgs ceargs)
                    Console.WriteLine("Cleared [{0}..{1}]",
                          ceargs.Start, ceargs.Start + ceargs.Count - 1);
            };
            RemoveSublist1(list, 1, 2);
            Console.WriteLine(list);
            RemoveSublist2(list, 1, 2);
            Console.WriteLine(list);
            RemoveTail1(list, 3);
            Console.WriteLine(list);
            RemoveTail2(list, 2);
            Console.WriteLine(list);
        }

        // Reverse list[i..i+n-1]

        public static void ReverseInterval<T>(IList<T> list, int i, int n)
        {
            list.View(i, n).Reverse();
        }

        // Swap list[0..i-1] with list[i..Count-1]

        public static void SwapInitialFinal<T>(IList<T> list, int i)
        {
            list.View(0, i).Reverse();
            list.View(i, list.Count - i).Reverse();
            list.Reverse();
        }

        // Remove sublist of a list

        public static void RemoveSublist1<T>(IList<T> list, int i, int n)
        {
            list.RemoveInterval(i, n);
        }

        public static void RemoveSublist2<T>(IList<T> list, int i, int n)
        {
            list.View(i, n).Clear();
        }


        // Remove tail of a list

        public static void RemoveTail1<T>(IList<T> list, int i)
        {
            list.RemoveInterval(i, list.Count - i);
        }

        public static void RemoveTail2<T>(IList<T> list, int i)
        {
            list.View(i, list.Count - i).Clear();
        }

        // Pattern for finding and using first (leftmost) x in list

        private static void PatFirst<T>(IList<T> list, T x)
        {
            int j = list.IndexOf(x);
            if (j >= 0)
            {
                // x is a position j in list
            }
            else
            {
                // x is not in list
            }
        }

        // Pattern for finding and using last (rightmost) x in list

        private static void PatLast<T>(IList<T> list, T x)
        {
            int j = list.LastIndexOf(x);
            if (j >= 0)
            {
                // x is at position j in list
            }
            else
            {
                // x is not in list
            }
        }

        // Pattern for finding and using first (leftmost) x in list[i..i+n-1]

        private static void PatFirstSublist<T>(IList<T> list, T x, int i, int n)
        {
            int j = list.View(i, n).IndexOf(x);
            if (j >= 0)
            {
                // x is at position j+i in list
            }
            else
            {
                // x is not in list[i..i+n-1]
            }
        }

        // Pattern for finding and using last (rightmost) x in list[i..i+n-1]

        private static void PatLastSublist<T>(IList<T> list, T x, int i, int n)
        {
            int j = list.View(i, n).LastIndexOf(x);
            if (j >= 0)
            {
                // x is at position j+i in list
            }
            else
            {
                // x is not in list[i..i+n-1]
            }
        }
    }
}
