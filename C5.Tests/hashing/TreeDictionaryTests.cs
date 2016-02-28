using NUnit.Framework;
using System.Linq;

namespace C5.Tests.hashing
{
    [TestFixture]
    public class TreeDictionaryTests
    {
        [Test]
        public void TreeDictionaryBug_no_42()
        {
            var tree = new TreeDictionary<int, int>();

            tree.Values.LastOrDefault();
            tree.UpdateOrAdd(0, 0);
            var last = tree.Values.LastOrDefault();

            Assert.AreEqual(0, last);
        }

        [TestCase(MemoryType.Normal, 1, 10, Description = "Once")]
        [TestCase(MemoryType.Normal, 2, 20, Description = "Twice")]
        [TestCase(MemoryType.Normal, 3, 30, Description = "Thrice")]
        [TestCase(MemoryType.Normal, 4, 40, Description = "Four Times")]
        [TestCase(MemoryType.Normal, 5, 50, Description = "Five Times")]
        public void Values_works_multiple_times(MemoryType memoryType, int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<string, int>(memoryType) { { "One", 1 }, { "Two", 2 }, { "Three", 3 }, { "Four", 4 } };

            var sum = 0;
            for (var i = 0; i < numberOfIterations; i++)
            {
                foreach (var value in dictionary.Values)
                {
                    sum += value;
                }
            }

            Assert.AreEqual(expectedResult, sum);
        }

        [TestCase(MemoryType.Normal, 1, 10, Description = "Once")]
        [TestCase(MemoryType.Normal, 2, 20, Description = "Twice")]
        [TestCase(MemoryType.Normal, 3, 30, Description = "Thrice")]
        [TestCase(MemoryType.Normal, 4, 40, Description = "Four Times")]
        [TestCase(MemoryType.Normal, 5, 50, Description = "Five Times")]
        public void Keys_works_multiple_times(MemoryType memoryType, int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<int, string>(memoryType) { { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" } };

            var sum = 0;
            for (var i = 0; i < numberOfIterations; i++)
            {
                foreach (var value in dictionary.Keys)
                {
                    sum += value;
                }
            }

            Assert.AreEqual(expectedResult, sum);
        }

        [TestCase(MemoryType.Normal, 1, 10, Description = "Once")]
        [TestCase(MemoryType.Normal, 2, 20, Description = "Twice")]
        [TestCase(MemoryType.Normal, 3, 30, Description = "Thrice")]
        [TestCase(MemoryType.Normal, 4, 40, Description = "Four Times")]
        [TestCase(MemoryType.Normal, 5, 50, Description = "Five Times")]
        public void KeyValuePairs_works_multiple_times(MemoryType memoryType, int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<int, int>(memoryType) { { 1, -1 }, { 2, -2 }, { 3, -3 }, { 4, -4 } };

            var keys = 0;
            var values = 0;
            for (var i = 0; i < numberOfIterations; i++)
            {
                foreach (var kv in dictionary)
                {
                    keys += kv.Key;
                    values += kv.Value;
                }
            }

            Assert.AreEqual(expectedResult, keys);
            Assert.AreEqual(-expectedResult, values);
        }
    }
}
