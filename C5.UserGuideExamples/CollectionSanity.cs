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

// C5 example: anagrams 2004-12-08

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.CollectionCollection
//  dotnet run

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    class CollectionSanity
    {
        public static void Main(string[] args)
        {
            var col1 = new LinkedList<int>();
            var col2 = new LinkedList<int>();
            var col3 = new LinkedList<int>();
            col1.AddAll(new[] { 7, 9, 13 });
            col2.AddAll(new[] { 7, 9, 13 });
            col3.AddAll(new[] { 9, 7, 13 });

            HashSet<IList<int>> hs1 = new HashSet<IList<int>>
            {
                col1,
                col2,
                col3
            };
            Console.WriteLine("hs1 is sane: {0}", EqualityComparerSanity<int, IList<int>>(hs1));
        }

        // When colls is a collection of collections, this method checks
        // that all `inner' collections use the exact same equalityComparer.  Note
        // that two equalityComparer objects may be functionally (extensionally)
        // identical, yet be distinct objects.  However, if the equalityComparers
        // were obtained from EqualityComparer<T>.Default, there will be at most one
        // equalityComparer for each type T.
        public static bool EqualityComparerSanity<T, U>(ICollectionValue<U> colls) where U : IExtensible<T>
        {
            SCG.IEqualityComparer<T> equalityComparer = null;
            foreach (IExtensible<T> coll in colls)
            {
                if (equalityComparer == null)
                {
                    equalityComparer = coll.EqualityComparer;
                }
                if (equalityComparer != coll.EqualityComparer)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
