/*
 Copyright (c) 2003-2008 Niels Kokholm and Peter Sestoft
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

// Experiment with extension methods and C5, 2007-10-31

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.AddOn
//  dotnet run

using System;
using System.Linq.Expressions;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    static class AddOn
    {
        public static int Added<T>(this ICollection<T> coll, int x)
        {
            return coll.Count + x;
        }

        public static SCG.IEnumerable<T> Where<T>(this ICollection<T> coll, Expression<Func<T, bool>> predicate)
        {
            Console.WriteLine("hallo");
            // Func<T,bool> p = pred.Compile();
            var p = predicate.Compile();
            foreach (T item in coll)
            {   
                // if (p(item))
                if ((bool)p.DynamicInvoke(item))
                {
                    yield return item;
                }
            }
        }

        static void Main(string[] args)
        {
            var hs = new HashSet<NamedPerson>
            {
                new NamedPerson("Ole"),
                new NamedPerson("Hans")
            };

            foreach (NamedPerson q in (from p in hs where p.Name.Length == 4 select p))
            {
                Console.WriteLine(q);
            }
        }
    }

    class NamedPerson
    {
        public string Name { get; }

        public NamedPerson(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
