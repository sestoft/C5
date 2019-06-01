// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Compile with
//   csc /r:netstandard.dll /r:C5.dll GConvexHull.cs

using System;

namespace C5.UserGuideExamples
{
    // Find the convex hull of a point set in the plane

    // An implementation of Graham's (1972) point elimination algorithm,
    // as modified by Andrew (1979) to find lower and upper hull separately.

    // This implementation correctly handle duplicate points, and
    // multiple points with the same x-coordinate.

    // 1. Sort the points lexicographically by increasing (x,y), thus 
    //    finding also a leftmost point L and a rightmost point R.
    // 2. Partition the point set into two lists, upper and lower, according as 
    //    point is above or below the segment LR.  The upper list begins with 
    //    L and ends with R; the lower list begins with R and ends with L.
    // 3. Traverse the point lists clockwise, eliminating all but the extreme
    //    points (thus eliminating also duplicate points).
    // 4. Join the point lists (in clockwise order) in an array, 
    //    leaving out L from lower and R from upper.

    public class Hull
    {
        public static Point[] ConvexHull(Point[] pts)
        {
            // 1. Sort points lexicographically by increasing (x, y)
            var n = pts.Length;
            Array.Sort(pts);
            var left = pts[0];
            var right = pts[n - 1];

            // 2. Partition into lower hull and upper hull
            var lower = new LinkedList<Point>();
            var upper = new LinkedList<Point>();
            lower.InsertFirst(left); upper.InsertLast(left);
            for (var i = 0; i < n; i++)
            {
                var det = Point.Area2(left, right, pts[i]);
                if (det < 0)
                {
                    lower.InsertFirst(pts[i]);
                }
                else if (det > 0)
                {
                    upper.InsertLast(pts[i]);
                }
            }
            lower.InsertFirst(right);
            upper.InsertLast(right);

            // 3. Eliminate points not on the hull
            Eliminate(lower);
            Eliminate(upper);

            // 4. Join the lower and upper hull, leaving out lower.Last and upper.Last
            var res = new Point[lower.Count + upper.Count - 2];
            lower[0, lower.Count - 1].CopyTo(res, 0);
            upper[0, upper.Count - 1].CopyTo(res, lower.Count - 1);

            return res;
        }

        // Graham's scan
        public static void Eliminate(IList<Point> lst)
        {
            var view = lst.View(0, 0);
            var slide = 0;
            while (view.TrySlide(slide, 3))
            {
                if (Point.Area2(view[0], view[1], view[2]) < 0)   // right turn
                {
                    slide = 1;
                }
                else
                {                                                 // left or straight
                    view.RemoveAt(1);
                    slide = view.Offset != 0 ? -1 : 0;
                }
            }
        }
    }

    // ------------------------------------------------------------
    // Points in the plane
    public class Point : IComparable<Point>
    {
        private static readonly C5Random rnd = new C5Random(42);

        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static Point Random(int w, int h)
        {
            return new Point(rnd.Next(w), rnd.Next(h));
        }

        public bool Equals(Point p2)
        {
            return X == p2.X && Y == p2.Y;
        }

        public int CompareTo(Point p2)
        {
            var major = X.CompareTo(p2.X);
            return major != 0 ? major : Y.CompareTo(p2.Y);
        }

        // Twice the signed area of the triangle (p0, p1, p2)
        public static double Area2(Point p0, Point p1, Point p2)
        {
            return p0.X * (p1.Y - p2.Y) + p1.X * (p2.Y - p0.Y) + p2.X * (p0.Y - p1.Y);
        }
    }

    // ------------------------------------------------------------
    internal class GConvexHull
    {
        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var arg = args[0];
                var n = int.Parse(arg);
                var pts = new Point[n];
                for (var i = 0; i < n; i++)
                {
                    pts[i] = Point.Random(500, 500);
                }
                var chpts = Hull.ConvexHull(pts);
                Console.WriteLine($"Area is {Area(chpts)}");
                Print(chpts);
            }
            else
            {
                Console.WriteLine("Usage: GConvexHull <pointcount>\n");
            }
        }

        // The area of a polygon (represented by an array of ordered vertices)
        public static double Area(Point[] pts)
        {
            var n = pts.Length;
            var origo = new Point(0, 0);
            var area2 = 0.0;
            for (int i = 0; i < n; i++)
            {
                area2 += Point.Area2(origo, pts[i], pts[(i + 1) % n]);
            }
            return Math.Abs(area2 / 2);
        }

        public static void Print(Point[] pts)
        {
            var n = pts.Length;
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(pts[i]);
            }
        }
    }
}
