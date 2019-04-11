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
        public static void Main(string[] args)
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
