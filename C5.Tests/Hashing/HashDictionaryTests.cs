// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

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
            Assert.That(coll.ToString(), Is.EqualTo("{  }"));
            coll.Add(23, 67); coll.Add(45, 89);
            Assert.Multiple(() =>
            {
                Assert.That(coll.ToString(), Is.EqualTo("{ [45, 89], [23, 67] }"));
                Assert.That(coll.ToString(null, rad16), Is.EqualTo("{ [2D, 59], [17, 43] }"));
                Assert.That(coll.ToString("L14", null), Is.EqualTo("{ [45, 89], ... }"));
                Assert.That(coll.ToString("L14", rad16), Is.EqualTo("{ [2D, 59], ... }"));
            });
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
            Assert.That(dict.Choose(), Is.EqualTo(new System.Collections.Generic.KeyValuePair<string, string>("ER", "FOO")));
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

            Assert.Multiple(() =>
            {
                Assert.That(dict.IsReadOnly, Is.False);
                Assert.That(dict, Is.Empty, "new dict should be empty");
            });
            dict.Add("A", "B");
            Assert.That(dict, Has.Count.EqualTo(1), "bad count");
            Assert.That(dict["A"], Is.EqualTo("B"), "Wrong value for dict[A]");
            dict.Add("C", "D");
            Assert.That(dict, Has.Count.EqualTo(2), "bad count");
            Assert.Multiple(() =>
            {
                Assert.That(dict["A"], Is.EqualTo("B"), "Wrong value");
                Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value");
            });
            res = dict.Remove("A");
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.True, "bad return value from Remove(A)");
                Assert.That(dict, Has.Count.EqualTo(1), "bad count");
            });
            Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value of dict[C]");
            res = dict.Remove("Z");
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.False, "bad return value from Remove(Z)");
                Assert.That(dict, Has.Count.EqualTo(1), "bad count");
            });
            Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value of dict[C] (2)");
        }


        [Test]
        public void Contains()
        {
            dict.Add("C", "D");
            Assert.Multiple(() =>
            {
                Assert.That(dict.Contains("C"), Is.True);
                Assert.That(dict.Contains("D"), Is.False);
            });
        }


        [Test]
        public void IllegalAdd()
        {
            dict.Add("A", "B");

            var exception = Assert.Throws<DuplicateNotAllowedException>(() => dict.Add("A", "B"));
            Assert.That(exception.Message, Is.EqualTo("Key being added: 'A'"));
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
            Assert.That(dict["R"], Is.EqualTo("UYGUY"));
            dict["R"] = "UIII";
            Assert.That(dict["R"], Is.EqualTo("UIII"));
            dict["S"] = "VVV";
            Assert.Multiple(() =>
            {
                Assert.That(dict["R"], Is.EqualTo("UIII"));
                Assert.That(dict["S"], Is.EqualTo("VVV"));
            });
            //dict.dump();
        }

        [Test]
        public void CombinedOps()
        {
            dict["R"] = "UIII";
            dict["S"] = "VVV";
            dict["T"] = "XYZ";

            Assert.That(dict.Remove("S", out string s), Is.True);
            Assert.That(s, Is.EqualTo("VVV"));
            Assert.That(dict.Contains("S"), Is.False);
            Assert.That(dict.Remove("A", out _), Is.False);

            //
            string t = "T", a = "A";
            Assert.Multiple(() =>
            {
                Assert.That(dict.Find(ref t, out s), Is.True);
                Assert.That(s, Is.EqualTo("XYZ"));
                Assert.That(dict.Find(ref a, out _), Is.False);

                //
                Assert.That(dict.Update("R", "UHU"), Is.True);
                Assert.That(dict["R"], Is.EqualTo("UHU"));
                Assert.That(dict.Update("A", "W"), Is.False);
                Assert.That(dict.Contains("A"), Is.False);
            });

            //
            s = "KKK";
            Assert.Multiple(() =>
            {
                Assert.That(dict.FindOrAdd("B", ref s), Is.False);
                Assert.That(dict["B"], Is.EqualTo("KKK"));
                Assert.That(dict.FindOrAdd("T", ref s), Is.True);
                Assert.That(s, Is.EqualTo("XYZ"));
            });

            //
            s = "LLL";
            Assert.Multiple(() =>
            {
                Assert.That(dict.UpdateOrAdd("R", s), Is.True);
                Assert.That(dict["R"], Is.EqualTo("LLL"));
            });
            s = "MMM";
            Assert.Multiple(() =>
            {
                Assert.That(dict.UpdateOrAdd("C", s), Is.False);
                Assert.That(dict["C"], Is.EqualTo("MMM"));
            });

            // bug20071112 fixed 2008-02-03
            s = "NNN";
            Assert.Multiple(() =>
            {
                Assert.That(dict.UpdateOrAdd("R", s, out string old), Is.True);
                Assert.That(dict["R"], Is.EqualTo("NNN"));
                Assert.That(old, Is.EqualTo("LLL"));
            });
            s = "OOO";
            Assert.Multiple(() =>
            {
                Assert.That(dict.UpdateOrAdd("D", s, out _), Is.False);
                Assert.That(dict["D"], Is.EqualTo("OOO"));
            });
            // Unclear which of these is correct:
            // Assert.AreEqual(null, old);
            // Assert.AreEqual("OOO", old);
        }

        [Test]
        public void DeepBucket()
        {
            HashDictionary<int, int> dict2 = new();

            for (int i = 0; i < 5; i++)
            {
                dict2[16 * i] = 5 * i;
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.That(dict2[16 * i], Is.EqualTo(5 * i));
            }

            for (int i = 0; i < 5; i++)
            {
                dict2[16 * i] = 7 * i + 1;
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.That(dict2[16 * i], Is.EqualTo(7 * i + 1));
            }

            Assert.That(dict.Check(), Is.True);
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

            Assert.That(keys.Length, Is.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(keys.Contains("R"), Is.True);
                Assert.That(keys.Contains("S"), Is.True);
                Assert.That(keys.Contains("T"), Is.True);
            });
        }


        [Test]
        public void Values()
        {
            var values = _dict.Values.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(values.Contains("A"), Is.True);
                Assert.That(values.Contains("B"), Is.True);
                Assert.That(values.Contains("C"), Is.True);
            });
        }

        [Test]
        public void Fun()
        {
            Assert.That(_dict.Func("T"), Is.EqualTo("B"));
        }


        [Test]
        public void NormalUse()
        {
            var pairs = _dict.ToDictionary(pair => pair.Key, pair => pair.Value);

            Assert.That(pairs, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(pairs.Contains(new SCG.KeyValuePair<string, string>("R", "C")), Is.True);
                Assert.That(pairs.Contains(new SCG.KeyValuePair<string, string>("S", "A")), Is.True);
                Assert.That(pairs.Contains(new SCG.KeyValuePair<string, string>("T", "B")), Is.True);
            });
        }
    }
}
