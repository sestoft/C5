// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 and C# 3.0 collection initializers

// Compile with
//   csc /r:netstandard.dll /r:C5.dll Initializers.cs

using System;

namespace C5.UserGuideExamples
{
    class Initializers
    {
        public static void Main()
        {
            var list = new HashSet<int> { 2, 3, 5, 7, 11 };

            foreach (var x in list)
            {
                Console.WriteLine(x);
            }

            var dict = new HashDictionary<int, string>
            {
                { 2, "two" },
                { 3, "three" }
            };

            foreach (var x in dict)
            {
                Console.WriteLine(x.Value);
            }
        }
    }
}
