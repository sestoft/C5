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

// C5 bugs 2007-09-16

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.BugHashedArray
//  dotnet run

using System;

namespace C5.UserGuideExamples
{
    class BugHashedArray
    {
        public static void Main(string[] args)
        {
            var test = new HashedLinkedList<int>();

            for (var i = 0; i < 33; i++)
            {
                test.Add(i);
            }

            // Fails after 7 removals
            for (var i = 0; i < 33; i++)
            {
                Console.WriteLine(i);
                test.Remove(i);
            }

            // Fails after 23 removals
            for (var i = 32; i >= 0; i--)
            {
                Console.WriteLine(i);
                test.Remove(i);
            }
        }
    }
}
