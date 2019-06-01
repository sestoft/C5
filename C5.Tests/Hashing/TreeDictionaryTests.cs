// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

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

        [TestCase(1, 10, Description = "Once")]
        [TestCase(2, 20, Description = "Twice")]
        [TestCase(3, 30, Description = "Thrice")]
        [TestCase(4, 40, Description = "Four Times")]
        [TestCase(5, 50, Description = "Five Times")]
        public void Values_works_multiple_times(int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<string, int>() { { "One", 1 }, { "Two", 2 }, { "Three", 3 }, { "Four", 4 } };

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

        [TestCase(1, 10, Description = "Once")]
        [TestCase(2, 20, Description = "Twice")]
        [TestCase(3, 30, Description = "Thrice")]
        [TestCase(4, 40, Description = "Four Times")]
        [TestCase(5, 50, Description = "Five Times")]
        public void Keys_works_multiple_times(int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<int, string>() { { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" } };

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

        [TestCase(1, 10, Description = "Once")]
        [TestCase(2, 20, Description = "Twice")]
        [TestCase(3, 30, Description = "Thrice")]
        [TestCase(4, 40, Description = "Four Times")]
        [TestCase(5, 50, Description = "Five Times")]
        public void KeyValuePairs_works_multiple_times(int numberOfIterations, int expectedResult)
        {
            var dictionary = new TreeDictionary<int, int>() { { 1, -1 }, { 2, -2 }, { 3, -3 }, { 4, -4 } };

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
