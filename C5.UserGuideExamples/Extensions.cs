// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Experiment with extension methods and C5, 2007-10-31

// Compile with
//   csc /r:netstandard.dll /r:C5.dll Extensions.cs

using System;
using System.Linq.Expressions;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    static class Extension
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

        static void Main()
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
