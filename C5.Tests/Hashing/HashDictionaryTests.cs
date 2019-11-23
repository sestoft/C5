// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;
using System;
using System.Linq;
using SCG = System.Collections.Generic;
namespace C5.Tests.hashtable.dictionary
{
    using DictionaryIntToInt = HashDictionary<int, int>;

    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            DictionaryIntToInt factory() { return new DictionaryIntToInt(TenEqualityComparer.Default); }
            new C5.Tests.Templates.Events.DictionaryTester<DictionaryIntToInt>().Test(factory);
        }
    }

    internal static class Factory
    {
        public static IDictionary<K, V> New<K, V>() { return new HashDictionary<K, V>(); }
    }

    [TestFixture]
    public class Formatting
    {
        private IDictionary<int, int> coll;
        private IFormatProvider rad16;
        [SetUp]
        public void Init()
        {
            Debug.UseDeterministicHashing = true;
            coll = Factory.New<int, int>();
            rad16 = new RadixFormatProvider(16);
        }
        [TearDown]
        public void Dispose()
        {
            Debug.UseDeterministicHashing = false;
            coll = null;
            rad16 = null;
        }
        [Test]
        public void Format()
        {
            Assert.AreEqual("{  }", coll.ToString());
            coll.Add(23, 67); coll.Add(45, 89);
            Assert.AreEqual("{ [45, 89], [23, 67] }", coll.ToString());
            Assert.AreEqual("{ [2D, 59], [17, 43] }", coll.ToString(null, rad16));
            Assert.AreEqual("{ [45, 89], ... }", coll.ToString("L14", null));
            Assert.AreEqual("{ [2D, 59], ... }", coll.ToString("L14", rad16));
        }
    }

    [TestFixture]
    public class HashDict
    {
        private HashDictionary<string, string> dict;


        [SetUp]
        public void Init()
        {
            dict = new HashDictionary<string, string>();
            //dict = TreeDictionary<string,string>.MakeNaturalO<string,string>();
        }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new HashDictionary<int, int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor2()
        {
            Assert.Throws<NullReferenceException>(() => new HashDictionary<int, int>(5, 0.5, null));
        }

        [Test]
        public void Choose()
        {
            dict.Add("ER", "FOO");
            Assert.AreEqual(new System.Collections.Generic.KeyValuePair<string, string>("ER", "FOO"), dict.Choose());
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => dict.Choose());
        }



        [TearDown]
        public void Dispose()
        {
            dict = null;
        }


        [Test]
        public void Initial()
        {
            bool res;

            Assert.IsFalse(dict.IsReadOnly);
            Assert.AreEqual(0, dict.Count, "new dict should be empty");
            dict.Add("A", "B");
            Assert.AreEqual(1, dict.Count, "bad count");
            Assert.AreEqual("B", dict["A"], "Wrong value for dict[A]");
            dict.Add("C", "D");
            Assert.AreEqual(2, dict.Count, "bad count");
            Assert.AreEqual("B", dict["A"], "Wrong value");
            Assert.AreEqual("D", dict["C"], "Wrong value");
            res = dict.Remove("A");
            Assert.IsTrue(res, "bad return value from Remove(A)");
            Assert.AreEqual(1, dict.Count, "bad count");
            Assert.AreEqual("D", dict["C"], "Wrong value of dict[C]");
            res = dict.Remove("Z");
            Assert.IsFalse(res, "bad return value from Remove(Z)");
            Assert.AreEqual(1, dict.Count, "bad count");
            Assert.AreEqual("D", dict["C"], "Wrong value of dict[C] (2)");
        }


        [Test]
        public void Contains()
        {
            dict.Add("C", "D");
            Assert.IsTrue(dict.Contains("C"));
            Assert.IsFalse(dict.Contains("D"));
        }


        [Test]
        public void IllegalAdd()
        {
            dict.Add("A", "B");

            var exception = Assert.Throws<DuplicateNotAllowedException>(() => dict.Add("A", "B"));
            Assert.AreEqual("Key being added: 'A'", exception.Message);
        }


        [Test]
        public void GettingNonExisting()
        {
            Assert.Throws<NoSuchItemException>(() => Console.WriteLine(dict["R"]));
        }


        [Test]
        public void Setter()
        {
            dict["R"] = "UYGUY";
            Assert.AreEqual("UYGUY", dict["R"]);
            dict["R"] = "UIII";
            Assert.AreEqual("UIII", dict["R"]);
            dict["S"] = "VVV";
            Assert.AreEqual("UIII", dict["R"]);
            Assert.AreEqual("VVV", dict["S"]);
            //dict.dump();
        }

        [Test]
        public void CombinedOps()
        {
            dict["R"] = "UIII";
            dict["S"] = "VVV";
            dict["T"] = "XYZ";


            Assert.IsTrue(dict.Remove("S", out string s));
            Assert.AreEqual("VVV", s);
            Assert.IsFalse(dict.Contains("S"));
            Assert.IsFalse(dict.Remove("A", out _));

            //
            string t = "T", a = "A";
            Assert.IsTrue(dict.Find(ref t, out s));
            Assert.AreEqual("XYZ", s);
            Assert.IsFalse(dict.Find(ref a, out _));

            //
            Assert.IsTrue(dict.Update("R", "UHU"));
            Assert.AreEqual("UHU", dict["R"]);
            Assert.IsFalse(dict.Update("A", "W"));
            Assert.IsFalse(dict.Contains("A"));

            //
            s = "KKK";
            Assert.IsFalse(dict.FindOrAdd("B", ref s));
            Assert.AreEqual("KKK", dict["B"]);
            Assert.IsTrue(dict.FindOrAdd("T", ref s));
            Assert.AreEqual("XYZ", s);

            //
            s = "LLL";
            Assert.IsTrue(dict.UpdateOrAdd("R", s));
            Assert.AreEqual("LLL", dict["R"]);
            s = "MMM";
            Assert.IsFalse(dict.UpdateOrAdd("C", s));
            Assert.AreEqual("MMM", dict["C"]);

            // bug20071112 fixed 2008-02-03
            s = "NNN";
            Assert.IsTrue(dict.UpdateOrAdd("R", s, out string old));
            Assert.AreEqual("NNN", dict["R"]);
            Assert.AreEqual("LLL", old);
            s = "OOO";
            Assert.IsFalse(dict.UpdateOrAdd("D", s, out _));
            Assert.AreEqual("OOO", dict["D"]);
            // Unclear which of these is correct:
            // Assert.AreEqual(null, old);
            // Assert.AreEqual("OOO", old);
        }

        [Test]
        public void DeepBucket()
        {
            HashDictionary<int, int> dict2 = new HashDictionary<int, int>();

            for (int i = 0; i < 5; i++)
            {
                dict2[16 * i] = 5 * i;
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(5 * i, dict2[16 * i]);
            }

            for (int i = 0; i < 5; i++)
            {
                dict2[16 * i] = 7 * i + 1;
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(7 * i + 1, dict2[16 * i]);
            }

            Assert.IsTrue(dict.Check());
        }
    }

    [TestFixture]
    public class Enumerators
    {
        private HashDictionary<string, string> _dict;

        [SetUp]
        public void Init()
        {
            _dict = new HashDictionary<string, string>
            {
                ["S"] = "A",
                ["T"] = "B",
                ["R"] = "C"
            };
        }

        [TearDown]
        public void Dispose()
        {
            _dict = null;
        }

        [Test]
        public void Keys()
        {
            var keys = _dict.Keys.ToArray();

            Assert.AreEqual(3, keys.Length);
            Assert.IsTrue(keys.Contains("R"));
            Assert.IsTrue(keys.Contains("S"));
            Assert.IsTrue(keys.Contains("T"));
        }


        [Test]
        public void Values()
        {
            var values = _dict.Values.ToArray();

            Assert.AreEqual(3, values.Length);
            Assert.IsTrue(values.Contains("A"));
            Assert.IsTrue(values.Contains("B"));
            Assert.IsTrue(values.Contains("C"));
        }

        [Test]
        public void Fun()
        {
            Assert.AreEqual("B", _dict.Func("T"));
        }


        [Test]
        public void NormalUse()
        {
            var pairs = _dict.ToDictionary(pair => pair.Key, pair => pair.Value);

            Assert.AreEqual(3, pairs.Count);
            Assert.IsTrue(pairs.Contains(new SCG.KeyValuePair<string, string>("R", "C")));
            Assert.IsTrue(pairs.Contains(new SCG.KeyValuePair<string, string>("S", "A")));
            Assert.IsTrue(pairs.Contains(new SCG.KeyValuePair<string, string>("T", "B")));
        }
    }
}





