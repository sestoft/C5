// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: This should fail because C5 does not know how to build
// a comparer for Object.

// Similarly for Rec<string,int>

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll SortedArrayTest.cs 

using System;

namespace C5.UserGuideExamples
{
    class SortedArrayTest
    {
        public static void Main()
        {
            var lexico = ComparerFactory<Rec<string, int>>.CreateComparer(
                (r1, r2) =>
                    {
                        int order = r1.X1.CompareTo(r2.X1);
                        return order == 0 ? r1.X2.CompareTo(r2.X2) : order;
                    }
                );

            SortedArray<Rec<string, int>> sarr = new SortedArray<Rec<string, int>>(lexico)
            {
                new Rec<string, int>("ole", 32),
                new Rec<string, int>("hans", 77),
                new Rec<string, int>("ole", 63)
            };

            foreach (var r in sarr)
            {
                Console.WriteLine(r);
            }
        }
    }
}
