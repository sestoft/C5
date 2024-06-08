// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
namespace C5.Tests.hashtable.bag
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            HashBag<int> factory() { return new HashBag<int>(TenEqualityComparer.Instance); }
            new Templates.Events.CollectionTester<HashBag<int>>().Test(factory);
        }

        //[Test]
        //public void Extensible()
        //{
        //    C5.Tests.Templates.Extensible.Clone.Tester<HashBag<int>>();
        //    C5.Tests.Templates.Extensible.Serialization.Tester<HashBag<int>>();
        //}
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new HashBag<T>(); }
    }


    [TestFixture]
    public class Formatting
    {
        private ICollection<int> coll;
        private IFormatProvider rad16;
        [SetUp]
        public void Init()
        {
            Debug.UseDeterministicHashing = true;
            coll = Factory.New<int>(); rad16 = new RadixFormatProvider(16);
        }
        [TearDown]
        public void Dispose()
        {
            Debug.UseDeterministicHashing = false;
            coll = null; rad16 = null;
        }
        [Test]
        public void Format()
        {
            Assert.That(coll.ToString(), Is.EqualTo("{{  }}"));
            coll.AddAll([-4, 28, 129, 65530, -4, 28]);
            Assert.Multiple(() =>
            {
                Assert.That(coll.ToString(), Is.EqualTo("{{ 65530(*1), -4(*2), 28(*2), 129(*1) }}"));
                Assert.That(coll.ToString(null, rad16), Is.EqualTo("{{ FFFA(*1), -4(*2), 1C(*2), 81(*1) }}"));
                Assert.That(coll.ToString("L18", null), Is.EqualTo("{{ 65530(*1), -4(*2)... }}"));
                Assert.That(coll.ToString("L18", rad16), Is.EqualTo("{{ FFFA(*1), -4(*2)... }}"));
            });
        }
    }

    [TestFixture]
    public class Combined
    {
        private ICollection<System.Collections.Generic.KeyValuePair<int, int>> lst;


        [SetUp]
        public void Init()
        {
            lst = new HashBag<System.Collections.Generic.KeyValuePair<int, int>>(new KeyValuePairEqualityComparer<int, int>());
            for (int i = 0; i < 10; i++)
            {
                lst.Add(new System.Collections.Generic.KeyValuePair<int, int>(i, i + 30));
            }
        }


        [TearDown]
        public void Dispose() { lst = null; }


        [Test]
        public void Find()
        {
            System.Collections.Generic.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
            });
            p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Find(ref p), Is.False);
        }


        [Test]
        public void FindOrAdd()
        {
            System.Collections.Generic.KeyValuePair<int, int> p = new(3, 78);
            System.Collections.Generic.KeyValuePair<int, int> q = new();

            Assert.Multiple(() =>
            {
                Assert.That(lst.FindOrAdd(ref p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
            });
            p = new System.Collections.Generic.KeyValuePair<int, int>(13, 79);
            Assert.That(lst.FindOrAdd(ref p), Is.False);
            q = new System.Collections.Generic.KeyValuePair<int, int>(13, q.Value);
            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref q), Is.True);
                Assert.That(q.Key, Is.EqualTo(13));
                Assert.That(q.Value, Is.EqualTo(79));
            });
        }


        [Test]
        public void Update()
        {
            System.Collections.Generic.KeyValuePair<int, int> p = new(3, 78);
            System.Collections.Generic.KeyValuePair<int, int> q = new();

            Assert.That(lst.Update(p), Is.True);
            q = new System.Collections.Generic.KeyValuePair<int, int>(3, q.Value);
            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref q), Is.True);
                Assert.That(q.Key, Is.EqualTo(3));
                Assert.That(q.Value, Is.EqualTo(78));
            });
            p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Update(p), Is.False);
        }


        [Test]
        public void UpdateOrAdd1()
        {
            var p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);
            var q = new System.Collections.Generic.KeyValuePair<int, int>();

            Assert.That(lst.UpdateOrAdd(p), Is.True);
            q = new System.Collections.Generic.KeyValuePair<int, int>(3, q.Value);
            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref q), Is.True);
                Assert.That(q.Key, Is.EqualTo(3));
                Assert.That(q.Value, Is.EqualTo(78));
            });
            p = new System.Collections.Generic.KeyValuePair<int, int>(13, 79);
            Assert.That(lst.UpdateOrAdd(p), Is.False);
            q = new System.Collections.Generic.KeyValuePair<int, int>(13, q.Value);
            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref q), Is.True);
                Assert.That(q.Key, Is.EqualTo(13));
                Assert.That(q.Value, Is.EqualTo(79));
            });
        }

        [Test]
        public void UpdateOrAdd2()
        {
            ICollection<string> coll = new HashBag<string>();
            // s1 and s2 are distinct objects but contain the same text:
            string s1 = "abc", s2 = ("def" + s1).Substring(3);
            Assert.Multiple(() =>
            {
                Assert.That(coll.UpdateOrAdd(s1, out string old), Is.False);
                Assert.That(old, Is.EqualTo(null));
                Assert.That(coll.UpdateOrAdd(s2, out old), Is.True);
                Assert.That(ReferenceEquals(s1, old), Is.True);
                Assert.That(ReferenceEquals(s2, old), Is.False);
            });
        }

        [Test]
        public void RemoveWithReturn()
        {
            System.Collections.Generic.KeyValuePair<int, int> p = new(3, 78);
            Assert.Multiple(() =>
            {
                //System.Collections.Generic.KeyValuePair<int, int> q = new System.Collections.Generic.KeyValuePair<int, int>();

                Assert.That(lst.Remove(p, out p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
            });
            p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Remove(p, out _), Is.False);
        }
    }



    [TestFixture]
    public class CollectionOrSink
    {
        private HashBag<int> hashbag;


        [SetUp]
        public void Init()
        {
            Debug.UseDeterministicHashing = true;
            hashbag = new HashBag<int>();
        }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new HashBag<int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor2()
        {
            Assert.Throws<NullReferenceException>(() => new HashBag<int>(5, null));
        }

        [Test]
        public void NullEqualityComparerinConstructor3()
        {
            Assert.Throws<NullReferenceException>(() => new HashBag<int>(5, 0.5, null));
        }

        [Test]
        public void Choose()
        {
            hashbag.Add(7);
            Assert.That(hashbag.Choose(), Is.EqualTo(7));
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => hashbag.Choose());
        }

        [Test]
        public void CountEtAl()
        {
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.IsReadOnly, Is.False);
                // Assert.IsFalse(hashbag.SyncRoot == null);
                Assert.That(hashbag, Is.Empty);
            });
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.IsEmpty, Is.True);
                Assert.That(hashbag.AllowsDuplicates, Is.True);
                Assert.That(hashbag.Add(0), Is.True);
                Assert.That(hashbag, Has.Count.EqualTo(1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.IsEmpty, Is.False);
                Assert.That(hashbag.Add(5), Is.True);
                Assert.That(hashbag, Has.Count.EqualTo(2));
            });
            Assert.That(hashbag.Add(5), Is.True);
            Assert.That(hashbag, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.IsEmpty, Is.False);
                Assert.That(hashbag.Add(8), Is.True);
                Assert.That(hashbag, Has.Count.EqualTo(4));
            });
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.ContainsCount(5), Is.EqualTo(2));
                Assert.That(hashbag.ContainsCount(8), Is.EqualTo(1));
                Assert.That(hashbag.ContainsCount(0), Is.EqualTo(1));
            });
        }


        [Test]
        public void AddAll()
        {
            hashbag.Add(3); hashbag.Add(4); hashbag.Add(4); hashbag.Add(5); hashbag.Add(4);

            HashBag<int> hashbag2 = new();

            hashbag2.AddAll(hashbag);
            Assert.That(IC.SetEq(hashbag2, 3, 4, 4, 4, 5), Is.True);
            hashbag.Add(9);
            hashbag.AddAll(hashbag2);
            Assert.Multiple(() =>
            {
                Assert.That(IC.SetEq(hashbag2, 3, 4, 4, 4, 5), Is.True);
                Assert.That(IC.SetEq(hashbag, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 9), Is.True);
            });
        }


        [Test]
        public void ContainsCount()
        {
            Assert.That(hashbag.ContainsCount(5), Is.EqualTo(0));
            hashbag.Add(5);
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.ContainsCount(5), Is.EqualTo(1));
                Assert.That(hashbag.ContainsCount(7), Is.EqualTo(0));
            });
            hashbag.Add(8);
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.ContainsCount(5), Is.EqualTo(1));
                Assert.That(hashbag.ContainsCount(7), Is.EqualTo(0));
                Assert.That(hashbag.ContainsCount(8), Is.EqualTo(1));
            });
            hashbag.Add(5);
            Assert.That(hashbag.ContainsCount(5), Is.EqualTo(2));
            Assert.That(hashbag.ContainsCount(7), Is.EqualTo(0));
            Assert.That(hashbag.ContainsCount(8), Is.EqualTo(1));
        }


        [Test]
        public void RemoveAllCopies()
        {
            hashbag.Add(5); hashbag.Add(7); hashbag.Add(5);
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.ContainsCount(5), Is.EqualTo(2));
                Assert.That(hashbag.ContainsCount(7), Is.EqualTo(1));
            });
            hashbag.RemoveAllCopies(5);
            Assert.That(hashbag.ContainsCount(5), Is.EqualTo(0));
            Assert.That(hashbag.ContainsCount(7), Is.EqualTo(1));
            hashbag.Add(5); hashbag.Add(8); hashbag.Add(5);
            hashbag.RemoveAllCopies(8);
            Assert.That(IC.Eq(hashbag, 7, 5, 5), Is.True);
        }


        [Test]
        public void ContainsAll()
        {
            HashBag<int> list2 = new();

            Assert.That(hashbag.ContainsAll(list2), Is.True);
            list2.Add(4);
            Assert.That(hashbag.ContainsAll(list2), Is.False);
            hashbag.Add(4);
            Assert.That(hashbag.ContainsAll(list2), Is.True);
            hashbag.Add(5);
            Assert.That(hashbag.ContainsAll(list2), Is.True);
            list2.Add(20);
            Assert.That(hashbag.ContainsAll(list2), Is.False);
            hashbag.Add(20);
            Assert.That(hashbag.ContainsAll(list2), Is.True);
            list2.Add(4);
            Assert.That(hashbag.ContainsAll(list2), Is.False);
            hashbag.Add(4);
            Assert.That(hashbag.ContainsAll(list2), Is.True);
        }


        [Test]
        public void RetainAll()
        {
            HashBag<int> list2 = new();

            hashbag.Add(4); hashbag.Add(5); hashbag.Add(4); hashbag.Add(6); hashbag.Add(4);
            list2.Add(5); list2.Add(4); list2.Add(7); list2.Add(4);
            hashbag.RetainAll(list2);
            Assert.That(IC.SetEq(hashbag, 4, 4, 5), Is.True);
            hashbag.Add(6);
            list2.Clear();
            list2.Add(7); list2.Add(8); list2.Add(9);
            hashbag.RetainAll(list2);
            Assert.That(IC.Eq(hashbag), Is.True);
        }


        [Test]
        public void RemoveAll()
        {
            HashBag<int> list2 = new();

            hashbag.Add(4); hashbag.Add(5); hashbag.Add(6); hashbag.Add(4); hashbag.Add(5);
            list2.Add(5); list2.Add(4); list2.Add(7); list2.Add(4);
            hashbag.RemoveAll(list2);
            Assert.That(IC.SetEq(hashbag, 5, 6), Is.True);
            hashbag.Add(5); hashbag.Add(4);
            list2.Clear();
            list2.Add(6); list2.Add(5);
            hashbag.RemoveAll(list2);
            Assert.That(IC.SetEq(hashbag, 4, 5), Is.True);
            list2.Clear();
            list2.Add(7); list2.Add(8); list2.Add(9);
            hashbag.RemoveAll(list2);
            Assert.That(IC.SetEq(hashbag, 4, 5), Is.True);
        }


        [Test]
        public void Remove()
        {
            hashbag.Add(4); hashbag.Add(4); hashbag.Add(5); hashbag.Add(4); hashbag.Add(6);
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.Remove(2), Is.False);
                Assert.That(hashbag.Remove(4), Is.True);
                Assert.That(IC.SetEq(hashbag, 4, 4, 5, 6), Is.True);
            });
            hashbag.Add(7);
            hashbag.Add(21); hashbag.Add(37); hashbag.Add(53); hashbag.Add(69); hashbag.Add(53); hashbag.Add(85);
            Assert.Multiple(() =>
            {
                Assert.That(hashbag.Remove(5), Is.True);
                Assert.That(IC.SetEq(hashbag, 4, 4, 6, 7, 21, 37, 53, 53, 69, 85), Is.True);
                Assert.That(hashbag.Remove(165), Is.False);
                Assert.That(hashbag.Check(), Is.True);
            });
            Assert.That(IC.SetEq(hashbag, 4, 4, 6, 7, 21, 37, 53, 53, 69, 85), Is.True);
            Assert.That(hashbag.Remove(53), Is.True);
            Assert.That(IC.SetEq(hashbag, 4, 4, 6, 7, 21, 37, 53, 69, 85), Is.True);
            Assert.That(hashbag.Remove(37), Is.True);
            Assert.That(IC.SetEq(hashbag, 4, 4, 6, 7, 21, 53, 69, 85), Is.True);
            Assert.That(hashbag.Remove(85), Is.True);
            Assert.That(IC.SetEq(hashbag, 4, 4, 6, 7, 21, 53, 69), Is.True);
        }


        [TearDown]
        public void Dispose()
        {
            Debug.UseDeterministicHashing = false;
            hashbag = null;
        }
    }

    [TestFixture]
    public class FindPredicate
    {
        private HashBag<int> list;
        private Func<int, bool> pred;

        [SetUp]
        public void Init()
        {
            Debug.UseDeterministicHashing = true;
            list = new HashBag<int>(TenEqualityComparer.Instance);
            pred = delegate (int i) { return i % 5 == 0; };
        }

        [TearDown]
        public void Dispose()
        {
            Debug.UseDeterministicHashing = false;
            list = null;
        }

        [Test]
        public void Find()
        {
            Assert.That(list.Find(pred, out int i), Is.False);
            list.AddAll([4, 22, 67, 37]);
            Assert.That(list.Find(pred, out i), Is.False);
            list.AddAll([45, 122, 675, 137]);
            Assert.Multiple(() =>
            {
                Assert.That(list.Find(pred, out i), Is.True);
                Assert.That(i, Is.EqualTo(45));
            });
        }

    }

    [TestFixture]
    public class UniqueItems
    {
        private HashBag<int> list;

        [SetUp]
        public void Init() { list = new HashBag<int>(); }

        [TearDown]
        public void Dispose() { list = null; }

        [Test]
        public void Test()
        {
            Assert.Multiple(() =>
            {
                Assert.That(IC.SetEq(list.UniqueItems()), Is.True);
                Assert.That(IC.SetEq(list.ItemMultiplicities()), Is.True);
            });
            list.AddAll([7, 9, 7]);
            Assert.Multiple(() =>
            {
                Assert.That(IC.SetEq(list.UniqueItems(), 7, 9), Is.True);
                Assert.That(IC.SetEq(list.ItemMultiplicities(), 7, 2, 9, 1), Is.True);
            });
        }
    }

    [TestFixture]
    public class ArrayTest
    {
        private HashBag<int> hashbag;
        private int[] a;


        [SetUp]
        public void Init()
        {
            Debug.UseDeterministicHashing = true;
            hashbag = new HashBag<int>();
            a = new int[10];
            for (int i = 0; i < 10; i++)
            {
                a[i] = 1000 + i;
            }
        }


        [TearDown]
        public void Dispose()
        {
            Debug.UseDeterministicHashing = false;
            hashbag = null;
        }

        [Test]
        public void ToArray()
        {
            Assert.That(hashbag.ToArray(), Is.Empty);
            hashbag.Add(7);
            hashbag.Add(3);
            hashbag.Add(10);
            hashbag.Add(3);

            int[] r = hashbag.ToArray();

            Array.Sort(r);
            Assert.That(r, Is.EqualTo(new[] { 3, 3, 7, 10 }));
        }

        [Test]
        public void CopyTo()
        {
            //Note: for small ints the itemequalityComparer is the identity!
            hashbag.CopyTo(a, 1);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
            hashbag.Add(6);
            hashbag.CopyTo(a, 2);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
            hashbag.Add(4);
            hashbag.Add(6);
            hashbag.Add(9);
            hashbag.CopyTo(a, 4);

            //TODO: make independent of interequalityComparer
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 6, 9, 4, 1008, 1009 }));
            hashbag.Clear();
            hashbag.Add(7);
            hashbag.CopyTo(a, 9);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 6, 9, 4, 1008, 7 }));
        }


        [Test]
        public void CopyToBad()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => hashbag.CopyTo(a, 11));
        }


        [Test]
        public void CopyToBad2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => hashbag.CopyTo(a, -1));
        }


        [Test]
        public void CopyToTooFar()
        {
            hashbag.Add(3);
            hashbag.Add(8);

            Assert.Throws<ArgumentOutOfRangeException>(() => hashbag.CopyTo(a, 9));
        }
    }

    [TestFixture]
    public class HashingEquals
    {
        private HashBag<int> h1;
        private LinkedList<int> h2;


        [SetUp]
        public void Init()
        {
            h1 = new HashBag<int>();
            h2 = new LinkedList<int>();
        }

        [TearDown]
        public void Dispose()
        {
            h1 = null;
            h2.Dispose();
        }

        [Test]
        public void Hashing()
        {
            Assert.That(h2.GetUnsequencedHashCode(), Is.EqualTo(h1.GetUnsequencedHashCode()));
            h1.Add(7);
            h2.Add(9);
            Assert.That(h1.GetUnsequencedHashCode() != h2.GetUnsequencedHashCode(), Is.True);
            h2.Add(7);
            h1.Add(9);
            Assert.That(h1.GetUnsequencedHashCode(), Is.EqualTo(h2.GetUnsequencedHashCode()));
        }

        [Test]
        public void Equals()
        {
            Assert.That(h1.UnsequencedEquals(h2), Is.True);
            //            Code 1550734257, Pair (3 x 1602896434, 2 x 1186320090) number 169185 matched oth
            //er pair (3 x -1615223932, 2 x 1019546595)
            h1.Add(1602896434);
            h1.Add(1186320090);
            h1.Add(1602896434);
            h1.Add(1186320090);
            h1.Add(1602896434);
            h2.Add(-1615223932);
            h2.Add(1019546595);
            h2.Add(-1615223932);
            h2.Add(1019546595);
            h2.Add(-1615223932);
            Assert.Multiple(() =>
            {
                Assert.That(h1.GetUnsequencedHashCode(), Is.EqualTo(h2.GetUnsequencedHashCode()));
                Assert.That(!h1.UnsequencedEquals(h2), Is.True);
            });
            h1.Clear();
            h2.Clear();
            h1.Add(1);
            h1.Add(2);
            h2.Add(2);
            h2.Add(1);
            Assert.That(h1.GetUnsequencedHashCode(), Is.EqualTo(h2.GetUnsequencedHashCode()));
            Assert.That(h1.UnsequencedEquals(h2), Is.True);
        }
    }
}
