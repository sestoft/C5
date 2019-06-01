// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;
using System.Collections.Generic;

namespace C5.Tests
{
    [TestFixture]
    public class IReadOnlyListTests
    {
        [Test]
        public void ArrayList_Implements_IReadOnlyList()
        {
            var list = new ArrayList<int> { 0, 1, 2, 3, 4 } as IReadOnlyList<int>;

            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(2, list[2]);
        }
    }
}
