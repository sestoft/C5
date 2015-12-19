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
