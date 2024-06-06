// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using SCG = System.Collections.Generic;

namespace C5.Tests.Templates.List
{
    internal class Dispose
    {
        public static void Tester<U>() where U : class, IList<int>, new()
        {
            U extensible = new();
            extensible.Dispose();
        }
    }

    internal class SCG_IList
    {
        public static void Tester<U>() where U : class, IList<int>, SCG.IList<int>, new()
        {
            SCG.IList<int> slist = new U();
            slist.Add(4);
            slist.Add(5);
            slist.Add(6);
            slist.RemoveAt(1);
            Assert.That(slist, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(slist[0], Is.EqualTo(4));
                Assert.That(slist[1], Is.EqualTo(6));
            });
        }
    }
}
