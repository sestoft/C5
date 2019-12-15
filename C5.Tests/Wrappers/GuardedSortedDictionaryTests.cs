using NUnit.Framework;

namespace C5.Tests.Wrappers
{
    [TestFixture]
    public class GuardedSortedDictionaryTests
    {
        [Test]
        public void Keys_returns_GuardedSorted()
        {
            var reverse = ComparerFactory<int>.CreateComparer((a, b) => a > b ? -1 : 1);

            var source = new SortedArrayDictionary<int, string>(reverse)
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedSortedDictionary<int, string>(source);

            Assert.IsAssignableFrom<GuardedSorted<int>>(guarded.Keys);
        }

        [Test]
        public void Keys_returns_Keys()
        {
            var reverse = ComparerFactory<int>.CreateComparer((a, b) => a > b ? -1 : 1);

            var source = new SortedArrayDictionary<int, string>(reverse)
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedSortedDictionary<int, string>(source);

            CollectionAssert.AreEquivalent(new[] { 3, 2, 1 }, guarded.Keys);
        }

        [Test]
        public void Values_returns_GuardedCollectionValue()
        {
            var reverse = ComparerFactory<int>.CreateComparer((a, b) => a > b ? -1 : 1);

            var source = new SortedArrayDictionary<int, string>(reverse)
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedSortedDictionary<int, string>(source);

            Assert.IsAssignableFrom<GuardedCollectionValue<string>>(guarded.Values);
        }

        [Test]
        public void Values_returns_Values()
        {
            var reverse = ComparerFactory<int>.CreateComparer((a, b) => a > b ? -1 : 1);

            var source = new SortedArrayDictionary<int, string>(reverse)
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var guarded = new GuardedSortedDictionary<int, string>(source);

            CollectionAssert.AreEquivalent(new[] { "one", "two", "three" }, guarded.Values);
        }
    }
}
