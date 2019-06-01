// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Views 2004-12-29 OBSOLETE

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll Views.cs 

using System;

namespace C5.UserGuideExamples
{
    internal class Views
    {
        public static void Main()
        {
            IList<char> lst = new LinkedList<char>();
            lst.AddAll(new char[] { 'a', 'b', 'c', 'd' });
            IList<char>
              A = lst.View(0, 2),
              B = lst.View(2, 0),
              C = lst.View(2, 1),
              D = lst.View(3, 1),
              E = lst.View(4, 0),
              F = lst.View(1, 2),
              G = lst.View(0, 4);
            IList<char>[] views = { A, B, C, D, E, F, G };
            Console.WriteLine("ABCDEFG overlaps with:");
            foreach (IList<char> u in views)
            {
                foreach (var w in views)
                {
                    Console.Write(Overlap(u, w) ? '+' : '-');
                }

                Console.WriteLine();
            }
            Console.WriteLine("ABCDEFG overlap length:");
            foreach (var u in views)
            {
                foreach (var w in views)
                {
                    var len = OverlapLength(u, w);
                    Console.Write(len >= 0 ? string.Format("{0}", len) : " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("ABCDEFG contained in:");
            foreach (var u in views)
            {
                foreach (var w in views)
                {
                    Console.Write(ContainsView(u, w) ? '+' : '-');
                }

                Console.WriteLine();
            }
        }

        public static int LeftEndIndex<T>(IList<T> u)
        {
            return u.Offset;
        }

        public static int RightEndIndex<T>(IList<T> u)
        {
            return u.Offset + u.Count;
        }

        public static bool Overlap<T>(IList<T> u, IList<T> w)
        {
            if (u.Underlying == null || u.Underlying != w.Underlying)
            {
                throw new ArgumentException("views must have same underlying list");
            }
            else
            {
                return u.Offset < w.Offset + w.Count && w.Offset < u.Offset + u.Count;
            }
        }

        public static int OverlapLength<T>(IList<T> u, IList<T> w)
        {
            if (Overlap(u, w))
            {
                return Math.Min(u.Offset + u.Count, w.Offset + w.Count)
                     - Math.Max(u.Offset, w.Offset);
            }
            else
            {
                return -1; // No overlap
            }
        }

        public static bool ContainsView<T>(IList<T> u, IList<T> w)
        {
            if (u.Underlying == null || u.Underlying != w.Underlying)
            {
                throw new ArgumentException("views must have same underlying list");
            }
            else if (w.Count > 0)
            {
                return u.Offset <= w.Offset && w.Offset + w.Count <= u.Offset + u.Count;
            }
            else
            {
                return u.Offset < w.Offset && w.Offset < u.Offset + u.Count;
            }
        }

        public static bool SameUnderlying<T>(IList<T> u, IList<T> w)
        {
            return (u.Underlying ?? u) == (w.Underlying ?? w);
        }

        // Replace the first occurrence of each x from xs by y in list:
        public static void ReplaceXsByY<T>(HashedLinkedList<T> list, T[] xs, T y)
        {
            foreach (T x in xs)
            {
                using (IList<T> view = list.ViewOf(x))
                {
                    if (view != null)
                    {
                        view.Remove();
                        view.Add(y);
                    }
                }
            }
        }

        // Find first item that satisfies p
        public static bool Find<T>(IList<T> list, Func<T, bool> p, out T res)
        {
            IList<T> view = list.View(0, 0);
            while (view.Offset < list.Count)
            {
                view.Slide(+1, 1);
                if (p(view.First))
                {
                    res = view.First;
                    return true;
                }
            }
            res = default;
            return false;
        }

        // Or, using that the list is enumerable:

        public static bool Find1<T>(IList<T> list, Func<T, bool> p, out T res)
        {
            foreach (T x in list)
            {
                if (p(x))
                {
                    res = x;
                    return true;
                }
            }
            res = default;
            return false;
        }

        // Find last item that satisfies p
        public static bool FindLast<T>(IList<T> list, Func<T, bool> p, out T res)
        {
            IList<T> view = list.View(list.Count, 0);
            while (view.Offset > 0)
            {
                view.Slide(-1, 1);
                if (p(view.First))
                {
                    res = view.First;
                    return true;
                }
            }
            res = default;
            return false;
        }
    }
}
