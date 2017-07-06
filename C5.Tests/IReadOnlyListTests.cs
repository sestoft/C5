using NUnit.Framework;
using System.Collections.Generic;

namespace C5.NET45.Tests
{
#if NET45
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
#endif
}
