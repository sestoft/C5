// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Comparer patterns

// Compile with
//   csc /r:netstandard.dll /r:C5.dll ComparerPatterns.cs

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    internal class ComparerPatterns
    {
        public static void Main()
        {
            var lexico1 = Lexico();
            var lexico2 = Lexico2();

            var r1 = ("Carsten", 1962);
            var r2 = ("Carsten", 1964);
            var r3 = ("Christian", 1932);

            Console.WriteLine(lexico1.Compare(r1, r1) == 0);
            Console.WriteLine(lexico1.Compare(r1, r2) < 0);
            Console.WriteLine(lexico1.Compare(r2, r3) < 0);

            Console.WriteLine(lexico2.Compare(r1, r1) == 0);
            Console.WriteLine(lexico2.Compare(r1, r2) < 0);
            Console.WriteLine(lexico2.Compare(r2, r3) < 0);

            var rev = ReverseComparer(SCG.Comparer<string>.Default);
            Console.WriteLine(rev.Compare("A", "A") == 0);
            Console.WriteLine(rev.Compare("A", "B") > 0);
            Console.WriteLine(rev.Compare("B", "A") < 0);
        }

        private static SCG.IComparer<(string, int)> Lexico()
        {
            return ComparerFactory<(string, int)>.CreateComparer((item1, item2) =>
{
    var major = item1.Item1.CompareTo(item2.Item1);
    return major != 0 ? major : item1.Item2.CompareTo(item2.Item2);
});
        }

        private static SCG.IComparer<(string, int)> Lexico2()
        {
            return ComparerFactory<(string, int)>.CreateComparer((item1, item2) =>
{
    var major = item1.Item1.CompareTo(item2.Item1);
    return major != 0 ? major : item1.Item2.CompareTo(item2.Item2);
});
        }

        private static SCG.IComparer<T> ReverseComparer<T>(SCG.IComparer<T> cmp)
        {
            return ComparerFactory<T>.CreateComparer((item1, item2) => cmp.Compare(item2, item1));
        }
    }
}
