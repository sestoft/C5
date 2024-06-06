using NUnit.Framework;

namespace C5.Tests.Wrappers
{
    [TestFixture]
    public class GuardedDictionaryTests
    {
        [Test]
        public void Keys_returns_GuardedCollectionValue()
        {
            var source = new HashDictionary<int, string>
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedDictionary<int, string>(source);

            Assert.IsAssignableFrom<GuardedCollectionValue<int>>(guarded.Keys);
        }

        [Test]
        public void Keys_returns_Keys()
        {
            var source = new HashDictionary<int, string>
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedDictionary<int, string>(source);

            Assert.That(guarded.Keys, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void Values_returns_GuardedCollectionValue()
        {
            var source = new HashDictionary<int, string>
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedDictionary<int, string>(source);

            Assert.IsAssignableFrom<GuardedCollectionValue<string>>(guarded.Values);
        }

        [Test]
        public void Values_returns_Values()
        {
            var source = new HashDictionary<int, string>
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedDictionary<int, string>(source);

            Assert.That(guarded.Values, Is.EquivalentTo(new[] { "one", "two", "three" }));
        }
    }
}
