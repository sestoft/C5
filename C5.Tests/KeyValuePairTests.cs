// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;

namespace C5.Tests
{
    [TestFixture]
    public class KeyValuePairTests
    {
        [Test]
        public void Create()
        {
            var p1 = new KeyValuePair<int, string>(42, "The answer");
            var p2 = KeyValuePair.Create(42, "The answer");

            Assert.AreEqual(p1, p2);
        }
    }
}
