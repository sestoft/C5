// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: Comparer patterns

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.ComparerPatterns
//  dotnet run

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    class ComparerPatterns
    {
        public static void Main()
        {
            var lexico1 = Lexico();
            var lexico2 = Lexico2();

            var r1 = new Rec<string, int>("Carsten", 1962);
            var r2 = new Rec<string, int>("Carsten", 1964);
            var r3 = new Rec<string, int>("Christian", 1932);

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

        static SCG.IComparer<Rec<string, int>> Lexico() => ComparerFactory<Rec<string, int>>.CreateComparer((item1, item2) =>
        {
            var major = item1.X1.CompareTo(item2.X1);
            return major != 0 ? major : item1.X2.CompareTo(item2.X2);
        });

        static SCG.IComparer<Rec<string, int>> Lexico2() => ComparerFactory<Rec<string, int>>.CreateComparer((item1, item2) =>
        {
            var major = item1.X1.CompareTo(item2.X1);
            return major != 0 ? major : item1.X2.CompareTo(item2.X2);
        });

        static SCG.IComparer<T> ReverseComparer<T>(SCG.IComparer<T> cmp) => ComparerFactory<T>.CreateComparer((item1, item2) => cmp.Compare(item2, item1));
    }
}
