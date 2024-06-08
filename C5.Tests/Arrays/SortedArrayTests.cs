// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests.arrays.sorted
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            SortedArray<int> factory() { return new SortedArray<int>(TenEqualityComparer.Instance); }
            new Templates.Events.SortedIndexedTester<SortedArray<int>>().Test(factory);
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new SortedArray<T>(); }
    }


    [TestFixture]
    public class Formatting
    {
        private ICollection<int> coll;
        private IFormatProvider rad16;
        [SetUp]
        public void Init() { coll = Factory.New<int>(); rad16 = new RadixFormatProvider(16); }
        [TearDown]
        public void Dispose() { coll = null; rad16 = null; }
        [Test]
        public void Format()
        {
            Assert.That(coll.ToString(), Is.EqualTo("{  }"));
            coll.AddAll([-4, 28, 129, 65530]);
            Assert.Multiple(() =>
            {
                Assert.That(coll.ToString(), Is.EqualTo("{ -4, 28, 129, 65530 }"));
                Assert.That(coll.ToString(null, rad16), Is.EqualTo("{ -4, 1C, 81, FFFA }"));
                Assert.That(coll.ToString("L14", null), Is.EqualTo("{ -4, 28, 129... }"));
                Assert.That(coll.ToString("L14", rad16), Is.EqualTo("{ -4, 1C, 81... }"));
            });
        }
    }

    [TestFixture]
    public class Ranges
    {
        private SortedArray<int> array;

        private SCG.IComparer<int> c;


        [SetUp]
        public void Init()
        {
            c = new IC();
            array = new SortedArray<int>(c);
            for (int i = 1; i <= 10; i++)
            {
                array.Add(i * 2);
            }
        }


        [Test]
        public void Enumerator()
        {
            SCG.IEnumerator<int> e = array.RangeFromTo(5, 17).GetEnumerator();
            int i = 3;

            while (e.MoveNext())
            {
                Assert.That(e.Current, Is.EqualTo(2 * i++));
            }

            Assert.That(i, Is.EqualTo(9));
        }


        [Test]
        public void Enumerator3()
        {
            SCG.IEnumerator<int> e = array.RangeFromTo(5, 17).GetEnumerator();

            e.MoveNext();
            array.Add(67);

            Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
        }


        [Test]
        public void Remove()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];

            array.RemoveRangeFrom(18);
            Assert.That(IC.Eq(array, [2, 4, 6, 8, 10, 12, 14, 16]), Is.True);
            array.RemoveRangeFrom(28);
            Assert.That(IC.Eq(array, [2, 4, 6, 8, 10, 12, 14, 16]), Is.True);
            array.RemoveRangeFrom(13);
            Assert.That(IC.Eq(array, [2, 4, 6, 8, 10, 12]), Is.True);
            array.RemoveRangeFrom(2);
            Assert.That(IC.Eq(array), Is.True);
            foreach (int i in all)
            {
                array.Add(i);
            }

            array.RemoveRangeTo(10);
            Assert.That(IC.Eq(array, [10, 12, 14, 16, 18, 20]), Is.True);
            array.RemoveRangeTo(2);
            Assert.That(IC.Eq(array, [10, 12, 14, 16, 18, 20]), Is.True);
            array.RemoveRangeTo(21);
            Assert.That(IC.Eq(array), Is.True);
            foreach (int i in all)
            {
                array.Add(i);
            }

            array.RemoveRangeFromTo(4, 8);
            Assert.That(IC.Eq(array, 2, 8, 10, 12, 14, 16, 18, 20), Is.True);
            array.RemoveRangeFromTo(14, 28);
            Assert.That(IC.Eq(array, 2, 8, 10, 12), Is.True);
            array.RemoveRangeFromTo(0, 9);
            Assert.That(IC.Eq(array, 10, 12), Is.True);
            array.RemoveRangeFromTo(0, 81);
            Assert.That(IC.Eq(array), Is.True);
        }

        [Test]
        public void Normal()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];

            Assert.Multiple(() =>
            {
                Assert.That(IC.Eq(array, all), Is.True);
                Assert.That(IC.Eq(array.RangeAll(), all), Is.True);
                Assert.That(array.RangeAll(), Has.Count.EqualTo(10));
                Assert.That(IC.Eq(array.RangeFrom(11), [12, 14, 16, 18, 20]), Is.True);
                Assert.That(array.RangeFrom(11), Has.Count.EqualTo(5));
                Assert.That(IC.Eq(array.RangeFrom(12), [12, 14, 16, 18, 20]), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(2), all), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(1), all), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(21), []), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(20), [20]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(8), [2, 4, 6]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(7), [2, 4, 6]), Is.True);
                Assert.That(array.RangeTo(7), Has.Count.EqualTo(3));
                Assert.That(IC.Eq(array.RangeTo(2), []), Is.True);
                Assert.That(IC.Eq(array.RangeTo(1), []), Is.True);
                Assert.That(IC.Eq(array.RangeTo(3), [2]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(20), [2, 4, 6, 8, 10, 12, 14, 16, 18]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(21), all), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(7, 12), [8, 10]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 11), [6, 8, 10]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(1, 12), [2, 4, 6, 8, 10]), Is.True);
                Assert.That(array.RangeFromTo(1, 12), Has.Count.EqualTo(5));
                Assert.That(IC.Eq(array.RangeFromTo(2, 12), [2, 4, 6, 8, 10]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 21), [6, 8, 10, 12, 14, 16, 18, 20]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 20), [6, 8, 10, 12, 14, 16, 18]), Is.True);
            });
        }


        [Test]
        public void Backwards()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];
            int[] lla = [20, 18, 16, 14, 12, 10, 8, 6, 4, 2];

            Assert.Multiple(() =>
            {
                Assert.That(IC.Eq(array, all), Is.True);
                Assert.That(IC.Eq(array.RangeAll().Backwards(), lla), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(11).Backwards(), [20, 18, 16, 14, 12]), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(12).Backwards(), [20, 18, 16, 14, 12]), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(2).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(1).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(21).Backwards(), []), Is.True);
                Assert.That(IC.Eq(array.RangeFrom(20).Backwards(), [20]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(8).Backwards(), [6, 4, 2]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(7).Backwards(), [6, 4, 2]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(2).Backwards(), []), Is.True);
                Assert.That(IC.Eq(array.RangeTo(1).Backwards(), []), Is.True);
                Assert.That(IC.Eq(array.RangeTo(3).Backwards(), [2]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(20).Backwards(), [18, 16, 14, 12, 10, 8, 6, 4, 2]), Is.True);
                Assert.That(IC.Eq(array.RangeTo(21).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(7, 12).Backwards(), [10, 8]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 11).Backwards(), [10, 8, 6]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(1, 12).Backwards(), [10, 8, 6, 4, 2]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(2, 12).Backwards(), [10, 8, 6, 4, 2]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 21).Backwards(), [20, 18, 16, 14, 12, 10, 8, 6]), Is.True);
                Assert.That(IC.Eq(array.RangeFromTo(6, 20).Backwards(), [18, 16, 14, 12, 10, 8, 6]), Is.True);
            });
        }

        [Test]
        public void Direction()
        {
            Assert.Multiple(() =>
            {
                Assert.That(array.Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(array.RangeFrom(20).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(array.RangeTo(7).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(array.RangeFromTo(1, 12).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(array.RangeAll().Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(array.Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(array.RangeFrom(20).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(array.RangeTo(7).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(array.RangeFromTo(1, 12).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(array.RangeAll().Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
            });
        }


        [TearDown]
        public void Dispose()
        {
            array = null;
            c = null;
        }
    }

    [TestFixture]
    public class BagItf
    {
        private SortedArray<int> array;


        [SetUp]
        public void Init()
        {
            array = new SortedArray<int>(new IC());
            for (int i = 10; i < 20; i++)
            {
                array.Add(i);
                array.Add(i + 10);
            }
        }


        [Test]
        public void Both()
        {
            Assert.Multiple(() =>
            {
                Assert.That(array.ContainsCount(7), Is.EqualTo(0));
                Assert.That(array.ContainsCount(10), Is.EqualTo(1));
            });
            array.RemoveAllCopies(10);
            Assert.That(array.ContainsCount(10), Is.EqualTo(0));
            array.RemoveAllCopies(7);
        }


        [TearDown]
        public void Dispose()
        {
            array = null;
        }
    }


    [TestFixture]
    public class Div
    {
        private SortedArray<int> array;


        [SetUp]
        public void Init()
        {
            array = new SortedArray<int>(new IC());
        }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new SortedArray<int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor2()
        {
            Assert.Throws<NullReferenceException>(() => new SortedArray<int>(5, null));
        }

        [Test]
        public void NullEqualityComparerinConstructor3()
        {
            Assert.Throws<NullReferenceException>(() => new SortedArray<int>(5, null, EqualityComparer<int>.Default));
        }

        [Test]
        public void NullEqualityComparerinConstructor4()
        {
            Assert.Throws<NullReferenceException>(() => new SortedArray<int>(5, SCG.Comparer<int>.Default, null));
        }

        [Test]
        public void NullEqualityComparerinConstructor5()
        {
            Assert.Throws<NullReferenceException>(() => new SortedArray<int>(5, null, null));
        }

        [Test]
        public void Choose()
        {
            array.Add(7);
            Assert.That(array.Choose(), Is.EqualTo(7));
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => array.Choose());
        }

        private void loadup()
        {
            for (int i = 10; i < 20; i++)
            {
                array.Add(i);
                array.Add(i + 10);
            }
        }


        [Test]
        public void NoDuplicatesEtc()
        {
            Assert.That(array.AllowsDuplicates, Is.False);
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(array.AllowsDuplicates, Is.False);
                Assert.That(array.ContainsSpeed, Is.EqualTo(Speed.Log));
                Assert.That(array.Comparer.Compare(2, 3), Is.LessThan(0));
                Assert.That(array.Comparer.Compare(4, 3), Is.GreaterThan(0));
                Assert.That(array.Comparer.Compare(3, 3), Is.EqualTo(0));
            });
        }

        [Test]
        public void Add()
        {
            Assert.That(array.Add(17), Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(array.Add(17), Is.False);
                Assert.That(array.Add(18), Is.True);
            });
            Assert.Multiple(() =>
            {
                Assert.That(array.Add(18), Is.False);
                Assert.That(array, Has.Count.EqualTo(2));
            });
        }


        [TearDown]
        public void Dispose()
        {
            array = null;
        }
    }


    [TestFixture]
    public class FindOrAdd
    {
        private SortedArray<SCG.KeyValuePair<int, string>> bag;


        [SetUp]
        public void Init()
        {
            bag = new SortedArray<SCG.KeyValuePair<int, string>>(new KeyValuePairComparer<int, string>(new IC()));
        }


        [TearDown]
        public void Dispose()
        {
            bag = null;
        }


        [Test]
        public void Test()
        {
            SCG.KeyValuePair<int, string> p = new(3, "tre");

            Assert.That(bag.FindOrAdd(ref p), Is.False);
            p = new SCG.KeyValuePair<int, string>(p.Key, "drei");
            Assert.Multiple(() =>
            {
                Assert.That(bag.FindOrAdd(ref p), Is.True);
                Assert.That(p.Value, Is.EqualTo("tre"));
            });
            p = new SCG.KeyValuePair<int, string>(p.Key, "three");
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount(p), Is.EqualTo(1));
                Assert.That(bag[0].Value, Is.EqualTo("tre"));
            });
        }
    }

    [TestFixture]
    public class FindPredicate
    {
        private SortedArray<int> list;
        private Func<int, bool> pred;

        [SetUp]
        public void Init()
        {
            list = new SortedArray<int>(TenEqualityComparer.Instance);
            pred = delegate (int i) { return i % 5 == 0; };
        }

        [TearDown]
        public void Dispose() { list = null; }

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

        [Test]
        public void FindLast()
        {
            Assert.That(list.FindLast(pred, out int i), Is.False);
            list.AddAll([4, 22, 67, 37]);
            Assert.That(list.FindLast(pred, out i), Is.False);
            list.AddAll([45, 122, 675, 137]);
            Assert.Multiple(() =>
            {
                Assert.That(list.FindLast(pred, out i), Is.True);
                Assert.That(i, Is.EqualTo(675));
            });
        }

        [Test]
        public void FindIndex()
        {
            Assert.That(list.FindIndex(pred), Is.LessThanOrEqualTo(0));
            list.AddAll([4, 22, 67, 37]);
            Assert.That(list.FindIndex(pred), Is.LessThanOrEqualTo(0));
            list.AddAll([45, 122, 675, 137]);
            Assert.That(list.FindIndex(pred), Is.EqualTo(3));
        }

        [Test]
        public void FindLastIndex()
        {
            Assert.That(list.FindLastIndex(pred), Is.LessThanOrEqualTo(0));
            list.AddAll([4, 22, 67, 37]);
            Assert.That(list.FindLastIndex(pred), Is.LessThanOrEqualTo(0));
            list.AddAll([45, 122, 675, 137]);
            Assert.That(list.FindLastIndex(pred), Is.EqualTo(7));
        }
    }

    [TestFixture]
    public class UniqueItems
    {
        private SortedArray<int> list;

        [SetUp]
        public void Init() { list = []; }

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
                Assert.That(IC.SetEq(list.ItemMultiplicities(), 7, 1, 9, 1), Is.True);
            });
        }
    }

    [TestFixture]
    public class ArrayTest
    {
        private SortedArray<int> tree;
        private int[] a;


        [SetUp]
        public void Init()
        {
            tree = new SortedArray<int>(new IC());
            a = new int[10];
            for (int i = 0; i < 10; i++)
            {
                a[i] = 1000 + i;
            }
        }


        [TearDown]
        public void Dispose() { tree = null; }


        [Test]
        public void ToArray()
        {
            Assert.That(tree.ToArray(), Is.Empty);
            tree.Add(7);
            tree.Add(4);
            Assert.That(tree.ToArray(), Is.EqualTo(new[] { 4, 7 }));
        }


        [Test]
        public void CopyTo()
        {
            tree.CopyTo(a, 1);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
            tree.Add(6);
            tree.CopyTo(a, 2);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
            tree.Add(4);
            tree.Add(9);
            tree.CopyTo(a, 4);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 4, 6, 9, 1007, 1008, 1009 }));
            tree.Clear();
            tree.Add(7);
            tree.CopyTo(a, 9);
            Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 4, 6, 9, 1007, 1008, 7 }));
        }


        [Test]
        public void CopyToBad()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.CopyTo(a, 11));
        }


        [Test]
        public void CopyToBad2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.CopyTo(a, -1));
        }


        [Test]
        public void CopyToTooFar()
        {
            tree.Add(3);
            tree.Add(4);
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.CopyTo(a, 9));
        }
    }


    [TestFixture]
    public class Combined
    {
        private IIndexedSorted<SCG.KeyValuePair<int, int>> lst;


        [SetUp]
        public void Init()
        {
            lst = new SortedArray<SCG.KeyValuePair<int, int>>(new KeyValuePairComparer<int, int>(new IC()));
            for (int i = 0; i < 10; i++)
            {
                lst.Add(new SCG.KeyValuePair<int, int>(i, i + 30));
            }
        }

        [TearDown]
        public void Dispose() { lst = null; }


        [Test]
        public void Find()
        {
            SCG.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.Find(ref p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
            });
            p = new SCG.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Find(ref p), Is.False);
        }


        [Test]
        public void FindOrAdd()
        {
            SCG.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.FindOrAdd(ref p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
            });
            p = new SCG.KeyValuePair<int, int>(13, 79);
            Assert.Multiple(() =>
            {
                Assert.That(lst.FindOrAdd(ref p), Is.False);
                Assert.That(lst[10].Key, Is.EqualTo(13));
                Assert.That(lst[10].Value, Is.EqualTo(79));
            });
        }


        [Test]
        public void Update()
        {
            SCG.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.Update(p), Is.True);
                Assert.That(lst[3].Key, Is.EqualTo(3));
                Assert.That(lst[3].Value, Is.EqualTo(78));
            });
            p = new SCG.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Update(p), Is.False);
        }


        [Test]
        public void UpdateOrAdd1()
        {
            SCG.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.UpdateOrAdd(p), Is.True);
                Assert.That(lst[3].Key, Is.EqualTo(3));
                Assert.That(lst[3].Value, Is.EqualTo(78));
            });
            p = new SCG.KeyValuePair<int, int>(13, 79);
            Assert.Multiple(() =>
            {
                Assert.That(lst.UpdateOrAdd(p), Is.False);
                Assert.That(lst[10].Key, Is.EqualTo(13));
                Assert.That(lst[10].Value, Is.EqualTo(79));
            });
        }

        [Test]
        public void UpdateOrAdd2()
        {
            ICollection<string> coll = new SortedArray<string>();
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
        public void UpdateOrAddWithExpand()
        {
            // bug20071217
            SortedArray<double> arr = new();
            for (int i = 0; i < 50; i++)
            {
                arr.UpdateOrAdd(i + 0.1);
                arr.Add(i + 0.2);
            }
            Assert.That(arr, Has.Count.EqualTo(100));
        }

        [Test]
        public void FindOrAddWithExpand()
        {
            // bug20071217
            SortedArray<double> arr = new();
            for (int i = 0; i < 50; i++)
            {
                double iVar = i + 0.1;
                arr.FindOrAdd(ref iVar);
                arr.Add(i * 0.2);
            }
            Assert.That(arr, Has.Count.EqualTo(100));
        }

        [Test]
        public void RemoveWithReturn()
        {
            SCG.KeyValuePair<int, int> p = new(3, 78);

            Assert.Multiple(() =>
            {
                Assert.That(lst.Remove(p, out p), Is.True);
                Assert.That(p.Key, Is.EqualTo(3));
                Assert.That(p.Value, Is.EqualTo(33));
                Assert.That(lst[3].Key, Is.EqualTo(4));
                Assert.That(lst[3].Value, Is.EqualTo(34));
            });
            p = new SCG.KeyValuePair<int, int>(13, 78);
            Assert.That(lst.Remove(p, out _), Is.False);
        }
    }


    [TestFixture]
    public class Remove
    {
        private SortedArray<int> array;


        [SetUp]
        public void Init()
        {
            array = new SortedArray<int>(new IC());
            for (int i = 10; i < 20; i++)
            {
                array.Add(i);
                array.Add(i + 10);
            }
        }


        [Test]
        public void SmallTrees()
        {
            array.Clear();
            array.Add(7);
            array.Add(9);
            Assert.Multiple(() =>
            {
                Assert.That(array.Remove(7), Is.True);
                Assert.That(array.Check(), Is.True);
            });
        }


        [Test]
        public void ByIndex()
        {
            //Remove root!
            int n = array.Count;
            int i = array[10];

            array.RemoveAt(10);
            Assert.Multiple(() =>
            {
                Assert.That(array.Check(), Is.True);
                Assert.That(array, Does.Not.Contain(i));
                Assert.That(array, Has.Count.EqualTo(n - 1));
            });

            //Low end
            i = array.FindMin();
            array.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(array.Check(), Is.True);
                Assert.That(array, Does.Not.Contain(i));
                Assert.That(array, Has.Count.EqualTo(n - 2));
            });

            //high end
            i = array.FindMax();
            array.RemoveAt(array.Count - 1);
            Assert.Multiple(() =>
            {
                Assert.That(array.Check(), Is.True);
                Assert.That(array, Does.Not.Contain(i));
                Assert.That(array, Has.Count.EqualTo(n - 3));
            });

            //Some leaf
            i = 18;
            array.RemoveAt(7);
            Assert.Multiple(() =>
            {
                Assert.That(array.Check(), Is.True);
                Assert.That(array, Does.Not.Contain(i));
                Assert.That(array, Has.Count.EqualTo(n - 4));
            });
        }


        [Test]
        public void AlmostEmpty()
        {
            //Almost empty
            array.Clear();
            array.Add(3);
            array.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(array.Check(), Is.True);
                Assert.That(array, Does.Not.Contain(3));
                Assert.That(array, Is.Empty);
            });
        }


        [Test]
        public void Empty()
        {
            array.Clear();

            var exception = Assert.Throws<IndexOutOfRangeException>(() => array.RemoveAt(0));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void HighIndex()
        {
            var exception = Assert.Throws<IndexOutOfRangeException>(() => array.RemoveAt(array.Count));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void LowIndex()
        {
            var exception = Assert.Throws<IndexOutOfRangeException>(() => array.RemoveAt(-1));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void Normal()
        {
            Assert.Multiple(() =>
            {
                Assert.That(array.Remove(-20), Is.False);

                //No demote case, with move_item
                Assert.That(array.Remove(20), Is.True);
                Assert.That(array.Check(), Is.True);
                Assert.That(array.Remove(20), Is.False);

                //plain case 2
                Assert.That(array.Remove(14), Is.True);
                Assert.That(array.Check(), Is.True, "Bad tree");

                //case 1b
                Assert.That(array.Remove(25), Is.True);
                Assert.That(array.Check(), Is.True, "Bad tree");

                //case 1c
                Assert.That(array.Remove(29), Is.True);
                Assert.That(array.Check(), Is.True, "Bad tree");

                //1a (terminating)
                Assert.That(array.Remove(10), Is.True);
                Assert.That(array.Check(), Is.True, "Bad tree");

                //2+1b
                Assert.That(array.Remove(12), Is.True);
                Assert.That(array.Remove(11), Is.True);

                //1a+1b
                Assert.That(array.Remove(18), Is.True);
                Assert.That(array.Remove(13), Is.True);
                Assert.That(array.Remove(15), Is.True);
            });

            //2+1c
            for (int i = 0; i < 10; i++)
            {
                array.Add(50 - 2 * i);
            }

            Assert.Multiple(() =>
            {
                Assert.That(array.Remove(42), Is.True);
                Assert.That(array.Remove(38), Is.True);
                Assert.That(array.Remove(28), Is.True);
                Assert.That(array.Remove(40), Is.True);

                //
                Assert.That(array.Remove(16), Is.True);
                Assert.That(array.Remove(23), Is.True);
                Assert.That(array.Remove(17), Is.True);
                Assert.That(array.Remove(19), Is.True);
                Assert.That(array.Remove(50), Is.True);
                Assert.That(array.Remove(26), Is.True);
                Assert.That(array.Remove(21), Is.True);
                Assert.That(array.Remove(22), Is.True);
                Assert.That(array.Remove(24), Is.True);
            });
            for (int i = 0; i < 48; i++)
            {
                array.Remove(i);
            }

            Assert.Multiple(() =>
            {
                //Almost empty tree:
                Assert.That(array.Remove(26), Is.False);
                Assert.That(array.Remove(48), Is.True);
                Assert.That(array.Check(), Is.True, "Bad tree");

                //Empty tree:
                Assert.That(array.Remove(26), Is.False);
                Assert.That(array.Check(), Is.True, "Bad tree");
            });
        }


        [TearDown]
        public void Dispose()
        {
            array = null;
        }
    }



    [TestFixture]
    public class PredecessorStructure
    {
        private SortedArray<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new SortedArray<int>(new IC());
        }


        private void loadup()
        {
            for (int i = 0; i < 20; i++)
            {
                tree.Add(2 * i);
            }
        }

        [Test]
        public void FindPredecessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryPredecessor(7, out int res) && res == 6, Is.True);
                Assert.That(tree.TryPredecessor(8, out res) && res == 6, Is.True);

                //The bottom
                Assert.That(tree.TryPredecessor(1, out res) && res == 0, Is.True);

                //The top
                Assert.That(tree.TryPredecessor(39, out res) && res == 38, Is.True);
            });
        }

        [Test]
        public void FindPredecessorTooLow1()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryPredecessor(-2, out int res), Is.False);
                Assert.That(res, Is.EqualTo(0));
                Assert.That(tree.TryPredecessor(0, out res), Is.False);
                Assert.That(res, Is.EqualTo(0));
            });
        }

        [Test]
        public void FindWeakPredecessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryWeakPredecessor(7, out int res) && res == 6, Is.True);
                Assert.That(tree.TryWeakPredecessor(8, out res) && res == 8, Is.True);

                //The bottom
                Assert.That(tree.TryWeakPredecessor(1, out res) && res == 0, Is.True);
                Assert.That(tree.TryWeakPredecessor(0, out res) && res == 0, Is.True);

                //The top
                Assert.That(tree.TryWeakPredecessor(39, out res) && res == 38, Is.True);
                Assert.That(tree.TryWeakPredecessor(38, out res) && res == 38, Is.True);
            });
        }

        [Test]
        public void FindWeakPredecessorTooLow1()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryWeakPredecessor(-1, out int res), Is.False);
                Assert.That(res, Is.EqualTo(0));
            });
        }

        [Test]
        public void FindSuccessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.TrySuccessor(7, out int res) && res == 8, Is.True);
                Assert.That(tree.TrySuccessor(8, out res) && res == 10, Is.True);

                //The bottom
                Assert.That(tree.TrySuccessor(0, out res) && res == 2, Is.True);
                Assert.That(tree.TrySuccessor(-1, out res) && res == 0, Is.True);

                //The top
                Assert.That(tree.TrySuccessor(37, out res) && res == 38, Is.True);
            });
        }

        [Test]
        public void FindSuccessorTooHigh()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.TrySuccessor(38, out int res), Is.False);
                Assert.That(res, Is.EqualTo(0));
                Assert.That(tree.TrySuccessor(39, out res), Is.False);
                Assert.That(res, Is.EqualTo(0));
            });
        }

        [Test]
        public void FindWeakSuccessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryWeakSuccessor(6, out int res) && res == 6, Is.True);
                Assert.That(tree.TryWeakSuccessor(7, out res) && res == 8, Is.True);

                //The bottom
                Assert.That(tree.TryWeakSuccessor(-1, out res) && res == 0, Is.True);
                Assert.That(tree.TryWeakSuccessor(0, out res) && res == 0, Is.True);

                //The top
                Assert.That(tree.TryWeakSuccessor(37, out res) && res == 38, Is.True);
                Assert.That(tree.TryWeakSuccessor(38, out res) && res == 38, Is.True);
            });
        }

        [Test]
        public void FindWeakSuccessorTooHigh1()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.TryWeakSuccessor(39, out int res), Is.False);
                Assert.That(res, Is.EqualTo(0));
            });
        }


        [Test]
        public void Predecessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.Predecessor(7), Is.EqualTo(6));
                Assert.That(tree.Predecessor(8), Is.EqualTo(6));

                //The bottom
                Assert.That(tree.Predecessor(1), Is.EqualTo(0));

                //The top
                Assert.That(tree.Predecessor(39), Is.EqualTo(38));
            });
        }

        [Test]
        public void PredecessorTooLow1()
        {
            Assert.Throws<NoSuchItemException>(() => tree.Predecessor(-2));
        }


        [Test]
        public void PredecessorTooLow2()
        {
            Assert.Throws<NoSuchItemException>(() => tree.Predecessor(0));
        }

        [Test]
        public void WeakPredecessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.WeakPredecessor(7), Is.EqualTo(6));
                Assert.That(tree.WeakPredecessor(8), Is.EqualTo(8));

                //The bottom
                Assert.That(tree.WeakPredecessor(1), Is.EqualTo(0));
                Assert.That(tree.WeakPredecessor(0), Is.EqualTo(0));

                //The top
                Assert.That(tree.WeakPredecessor(39), Is.EqualTo(38));
                Assert.That(tree.WeakPredecessor(38), Is.EqualTo(38));
            });
        }

        [Test]
        public void WeakPredecessorTooLow1()
        {
            Assert.Throws<NoSuchItemException>(() => tree.WeakPredecessor(-1));
        }


        [Test]
        public void Successor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.Successor(7), Is.EqualTo(8));
                Assert.That(tree.Successor(8), Is.EqualTo(10));

                //The bottom
                Assert.That(tree.Successor(0), Is.EqualTo(2));
                Assert.That(tree.Successor(-1), Is.EqualTo(0));

                //The top
                Assert.That(tree.Successor(37), Is.EqualTo(38));
            });
        }


        [Test]
        public void SuccessorTooHigh1()
        {
            Assert.Throws<NoSuchItemException>(() => tree.Successor(38));
        }


        [Test]
        public void SuccessorTooHigh2()
        {
            Assert.Throws<NoSuchItemException>(() => tree.Successor(39));
        }


        [Test]
        public void WeakSuccessor()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.WeakSuccessor(6), Is.EqualTo(6));
                Assert.That(tree.WeakSuccessor(7), Is.EqualTo(8));

                //The bottom
                Assert.That(tree.WeakSuccessor(-1), Is.EqualTo(0));
                Assert.That(tree.WeakSuccessor(0), Is.EqualTo(0));

                //The top
                Assert.That(tree.WeakSuccessor(37), Is.EqualTo(38));
                Assert.That(tree.WeakSuccessor(38), Is.EqualTo(38));
            });
        }

        [Test]
        public void WeakSuccessorTooHigh1()
        {
            Assert.Throws<NoSuchItemException>(() => tree.WeakSuccessor(39));
        }


        [TearDown]
        public void Dispose()
        {
            tree = null;
        }
    }



    [TestFixture]
    public class PriorityQueue
    {
        private SortedArray<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new SortedArray<int>(new IC());
        }


        private void loadup()
        {
            foreach (int i in new int[] { 1, 2, 3, 4 })
            {
                tree.Add(i);
            }
        }


        [Test]
        public void Normal()
        {
            loadup();
            Assert.Multiple(() =>
            {
                Assert.That(tree.FindMin(), Is.EqualTo(1));
                Assert.That(tree.FindMax(), Is.EqualTo(4));
                Assert.That(tree.DeleteMin(), Is.EqualTo(1));
                Assert.That(tree.DeleteMax(), Is.EqualTo(4));
                Assert.That(tree.Check(), Is.True, "Bad tree");
                Assert.That(tree.FindMin(), Is.EqualTo(2));
                Assert.That(tree.FindMax(), Is.EqualTo(3));
                Assert.That(tree.DeleteMin(), Is.EqualTo(2));
                Assert.That(tree.DeleteMax(), Is.EqualTo(3));
                Assert.That(tree.Check(), Is.True, "Bad tree");
            });
        }


        [Test]
        public void Empty1()
        {
            Assert.Throws<NoSuchItemException>(() => tree.FindMin());
        }


        [Test]
        public void Empty2()
        {
            Assert.Throws<NoSuchItemException>(() => tree.FindMax());
        }


        [Test]
        public void Empty3()
        {
            Assert.Throws<NoSuchItemException>(() => tree.DeleteMin());
        }


        [Test]
        public void Empty4()
        {
            Assert.Throws<NoSuchItemException>(() => tree.DeleteMax());
        }


        [TearDown]
        public void Dispose()
        {
            tree = null;
        }
    }



    [TestFixture]
    public class IndexingAndCounting
    {
        private SortedArray<int> array;


        [SetUp]
        public void Init()
        {
            array = new SortedArray<int>(new IC());
        }


        private void populate()
        {
            array.Add(30);
            array.Add(50);
            array.Add(10);
            array.Add(70);
        }


        [Test]
        public void ToArray()
        {
            populate();

            int[] a = [.. array];

            Assert.Multiple(() =>
            {
                Assert.That(a, Has.Length.EqualTo(4));
                Assert.That(a[0], Is.EqualTo(10));
                Assert.That(a[1], Is.EqualTo(30));
                Assert.That(a[2], Is.EqualTo(50));
                Assert.That(a[3], Is.EqualTo(70));
            });
        }


        [Test]
        public void GoodIndex()
        {
            Assert.Multiple(() =>
            {
                Assert.That(array.IndexOf(20), Is.EqualTo(~0));
                Assert.That(array.LastIndexOf(20), Is.EqualTo(~0));
            });
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(array[0], Is.EqualTo(10));
                Assert.That(array[1], Is.EqualTo(30));
                Assert.That(array[2], Is.EqualTo(50));
                Assert.That(array[3], Is.EqualTo(70));
                Assert.That(array.IndexOf(10), Is.EqualTo(0));
                Assert.That(array.IndexOf(30), Is.EqualTo(1));
                Assert.That(array.IndexOf(50), Is.EqualTo(2));
                Assert.That(array.IndexOf(70), Is.EqualTo(3));
                Assert.That(array.IndexOf(20), Is.EqualTo(~1));
                Assert.That(array.IndexOf(0), Is.EqualTo(~0));
                Assert.That(array.IndexOf(90), Is.EqualTo(~4));
                Assert.That(array.LastIndexOf(10), Is.EqualTo(0));
                Assert.That(array.LastIndexOf(30), Is.EqualTo(1));
                Assert.That(array.LastIndexOf(50), Is.EqualTo(2));
                Assert.That(array.LastIndexOf(70), Is.EqualTo(3));
                Assert.That(array.LastIndexOf(20), Is.EqualTo(~1));
                Assert.That(array.LastIndexOf(0), Is.EqualTo(~0));
                Assert.That(array.LastIndexOf(90), Is.EqualTo(~4));
            });
        }


        [Test]
        public void IndexTooLarge()
        {
            populate();
            Assert.Throws<IndexOutOfRangeException>(() => Console.WriteLine(array[4]));
        }


        [Test]
        public void IndexTooSmall()
        {
            populate();
            Assert.Throws<IndexOutOfRangeException>(() => Console.WriteLine(array[-1]));
        }


        [Test]
        public void FilledTreeOutsideInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(array.CountFrom(90), Is.EqualTo(0));
                Assert.That(array.CountFromTo(-20, 0), Is.EqualTo(0));
                Assert.That(array.CountFromTo(80, 100), Is.EqualTo(0));
                Assert.That(array.CountTo(0), Is.EqualTo(0));
                Assert.That(array.CountTo(90), Is.EqualTo(4));
                Assert.That(array.CountFromTo(-20, 90), Is.EqualTo(4));
                Assert.That(array.CountFrom(0), Is.EqualTo(4));
            });
        }


        [Test]
        public void FilledTreeIntermediateInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(array.CountFrom(20), Is.EqualTo(3));
                Assert.That(array.CountFromTo(20, 40), Is.EqualTo(1));
                Assert.That(array.CountTo(40), Is.EqualTo(2));
            });
        }


        [Test]
        public void FilledTreeMatchingInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(array.CountFrom(30), Is.EqualTo(3));
                Assert.That(array.CountFromTo(30, 70), Is.EqualTo(2));
                Assert.That(array.CountFromTo(50, 30), Is.EqualTo(0));
                Assert.That(array.CountFromTo(50, 50), Is.EqualTo(0));
                Assert.That(array.CountTo(10), Is.EqualTo(0));
                Assert.That(array.CountTo(50), Is.EqualTo(2));
            });
        }


        [Test]
        public void CountEmptyTree()
        {
            Assert.Multiple(() =>
            {
                Assert.That(array.CountFrom(20), Is.EqualTo(0));
                Assert.That(array.CountFromTo(20, 40), Is.EqualTo(0));
                Assert.That(array.CountTo(40), Is.EqualTo(0));
            });
        }


        [TearDown]
        public void Dispose()
        {
            array = null;
        }
    }




    namespace ModificationCheck
    {
        [TestFixture]
        public class Enumerator
        {
            private SortedArray<int> tree;

            private SCG.IEnumerator<int> e;


            [SetUp]
            public void Init()
            {
                tree = new SortedArray<int>(new IC());
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }
                e = tree.GetEnumerator();
            }


            [Test]
            public void CurrentAfterModification()
            {
                e.MoveNext();
                tree.Add(34);
                Assert.That(e.Current, Is.EqualTo(0));
            }


            [Test]
            public void MoveNextAfterAdd()
            {
                e.MoveNext();
                tree.Add(34);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }

            [Test]
            public void MoveNextAfterRemove()
            {
                e.MoveNext();
                tree.Remove(34);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }


            [Test]
            public void MoveNextAfterClear()
            {
                e.MoveNext();
                tree.Clear();

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }


            [TearDown]
            public void Dispose()
            {
                tree = null;
                e.Dispose();
            }
        }

        [TestFixture]
        public class RangeEnumerator
        {
            private SortedArray<int> tree;

            private SCG.IEnumerator<int> e;


            [SetUp]
            public void Init()
            {
                tree = new SortedArray<int>(new IC());
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                e = tree.RangeFromTo(3, 7).GetEnumerator();
            }


            [Test]
            public void CurrentAfterModification()
            {
                e.MoveNext();
                tree.Add(34);
                Assert.That(e.Current, Is.EqualTo(3));
            }


            [Test]
            public void MoveNextAfterAdd()
            {
                tree.Add(34);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }




            [Test]
            public void MoveNextAfterRemove()
            {
                tree.Remove(34);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }


            [Test]
            public void MoveNextAfterClear()
            {
                tree.Clear();

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }


            [TearDown]
            public void Dispose()
            {
                tree = null;
                e.Dispose();
            }
        }
    }

    namespace HigherOrder
    {
        internal class CubeRoot : IComparable<int>
        {
            private readonly int c;


            internal CubeRoot(int c) { this.c = c; }


            public int CompareTo(int that) { return c - that * that * that; }

            public bool Equals(int that) { return c == that * that * that; }

        }

        internal class Interval : IComparable<int>
        {
            private readonly int b, t;


            internal Interval(int b, int t) { this.b = b; this.t = t; }


            public int CompareTo(int that) { return that < b ? 1 : that > t ? -1 : 0; }

            public bool Equals(int that) { return that >= b && that <= t; }
        }



        [TestFixture]
        public class Simple
        {
            private SortedArray<int> array;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IC();
                array = new SortedArray<int>(ic);
            }


            private bool never(int i) { return false; }


            private bool always(int i) { return true; }


            private bool even(int i) { return i % 2 == 0; }


            private string themap(int i) { return string.Format("AA {0,4} BB", i); }


            private string badmap(int i) { return string.Format("AA {0} BB", i); }


            private int appfield1;

            private int appfield2;


            private void apply(int i) { appfield1++; appfield2 += i * i; }


            [Test]
            public void Apply()
            {
                Simple simple1 = new();

                array.Apply(new Action<int>(simple1.apply));
                Assert.Multiple(() =>
                {
                    Assert.That(simple1.appfield1, Is.EqualTo(0));
                    Assert.That(simple1.appfield2, Is.EqualTo(0));
                });

                Simple simple2 = new();

                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                array.Apply(new Action<int>(simple2.apply));
                Assert.Multiple(() =>
                {
                    Assert.That(simple2.appfield1, Is.EqualTo(10));
                    Assert.That(simple2.appfield2, Is.EqualTo(285));
                });
            }


            [Test]
            public void All()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(array.All(new Func<int, bool>(never)), Is.True);
                    Assert.That(array.All(new Func<int, bool>(even)), Is.True);
                    Assert.That(array.All(new Func<int, bool>(always)), Is.True);
                });
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.All(new Func<int, bool>(even)), Is.False);
                    Assert.That(array.All(new Func<int, bool>(always)), Is.True);
                });
                array.Clear();
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i * 2);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.All(new Func<int, bool>(even)), Is.True);
                    Assert.That(array.All(new Func<int, bool>(always)), Is.True);
                });
                array.Clear();
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i * 2 + 1);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.All(new Func<int, bool>(even)), Is.False);
                    Assert.That(array.All(new Func<int, bool>(always)), Is.True);
                });
            }


            [Test]
            public void Exists()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(array.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(even)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(always)), Is.False);
                });
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(even)), Is.True);
                    Assert.That(array.Exists(new Func<int, bool>(always)), Is.True);
                });
                array.Clear();
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i * 2);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(even)), Is.True);
                    Assert.That(array.Exists(new Func<int, bool>(always)), Is.True);
                });
                array.Clear();
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i * 2 + 1);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(even)), Is.False);
                    Assert.That(array.Exists(new Func<int, bool>(always)), Is.True);
                });
            }


            [Test]
            public void FindAll()
            {
                Assert.That(array.FindAll(new Func<int, bool>(never)), Is.Empty);
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.FindAll(new Func<int, bool>(never)), Is.Empty);
                    Assert.That(array.FindAll(new Func<int, bool>(always)), Has.Count.EqualTo(10));
                    Assert.That(array.FindAll(new Func<int, bool>(even)), Has.Count.EqualTo(5));
                    Assert.That(((SortedArray<int>)array.FindAll(new Func<int, bool>(even))).Check(), Is.True);
                });
            }


            [Test]
            public void Map()
            {
                Assert.That(array.Map(new Func<int, string>(themap), StringComparer.InvariantCulture), Is.Empty);
                for (int i = 0; i < 11; i++)
                {
                    array.Add(i * i * i);
                }

                var res = array.Map(new Func<int, string>(themap), StringComparer.InvariantCulture);

                Assert.Multiple(() =>
                {
                    Assert.That(((SortedArray<string>)res).Check(), Is.True);
                    Assert.That(res, Has.Count.EqualTo(11));
                    Assert.That(res[0], Is.EqualTo("AA    0 BB"));
                    Assert.That(res[3], Is.EqualTo("AA   27 BB"));
                    Assert.That(res[5], Is.EqualTo("AA  125 BB"));
                    Assert.That(res[10], Is.EqualTo("AA 1000 BB"));
                });
            }

            [Test]
            public void BadMap()
            {
                for (int i = 0; i < 11; i++)
                {
                    array.Add(i * i * i);
                }

                var exception = Assert.Throws<ArgumentException>(() => { ISorted<string> res = array.Map(new Func<int, string>(badmap), StringComparer.InvariantCulture); });
                Assert.That(exception.Message, Is.EqualTo("mapper not monotonic"));
            }


            [Test]
            public void Cut()
            {
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Cut(new CubeRoot(27), out int low, out bool lval, out int high, out bool hval), Is.True);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(2));
                    Assert.That(array.Cut(new CubeRoot(30), out low, out lval, out high, out hval), Is.False);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(3));
                });
            }


            [Test]
            public void CutInt()
            {
                for (int i = 0; i < 10; i++)
                {
                    array.Add(2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Cut(new IC(3), out int low, out bool lval, out int high, out bool hval), Is.False);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(2));
                    Assert.That(array.Cut(new IC(6), out low, out lval, out high, out hval), Is.True);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(8));
                    Assert.That(low, Is.EqualTo(4));
                });
            }


            [Test]
            public void CutInterval()
            {
                for (int i = 0; i < 10; i++)
                {
                    array.Add(2 * i);
                }

                Assert.That(array.Cut(new Interval(5, 9), out int lo, out bool lv, out int hi, out bool hv), Is.True);
                Assert.That(lv && hv, Is.True);
                Assert.That(hi, Is.EqualTo(10));
                Assert.That(lo, Is.EqualTo(4));
                Assert.That(array.Cut(new Interval(6, 10), out lo, out lv, out hi, out hv), Is.True);
                Assert.That(lv && hv, Is.True);
                Assert.That(hi, Is.EqualTo(12));
                Assert.That(lo, Is.EqualTo(4));

                for (int i = 0; i < 100; i++)
                {
                    array.Add(2 * i);
                }

                array.Cut(new Interval(77, 105), out lo, out lv, out hi, out hv);
                Assert.Multiple(() =>
                {
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(106));
                    Assert.That(lo, Is.EqualTo(76));
                });
                array.Cut(new Interval(5, 7), out lo, out lv, out hi, out hv);
                Assert.Multiple(() =>
                {
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(8));
                    Assert.That(lo, Is.EqualTo(4));
                });
                array.Cut(new Interval(80, 110), out lo, out lv, out hi, out hv);
                Assert.Multiple(() =>
                {
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(112));
                    Assert.That(lo, Is.EqualTo(78));
                });
            }


            [Test]
            public void UpperCut()
            {
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(array.Cut(new CubeRoot(1000), out int l, out bool lv, out _, out bool hv), Is.False);
                    Assert.That(lv && !hv, Is.True);
                    Assert.That(l, Is.EqualTo(9));
                    Assert.That(array.Cut(new CubeRoot(-50), out _, out lv, out int h, out hv), Is.False);
                    Assert.That(!lv && hv, Is.True);
                    Assert.That(h, Is.EqualTo(0));
                });
            }


            [TearDown]
            public void Dispose() { ic = null; array = null; }
        }
    }




    namespace MultiOps
    {
        [TestFixture]
        public class AddAll
        {
            private int sqr(int i) { return i * i; }

            private SortedArray<int> array;


            [SetUp]
            public void Init() { array = new SortedArray<int>(new IC()); }


            [Test]
            public void EmptyEmpty()
            {
                array.AddAll(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(array, Is.Empty);
                Assert.That(array.Check(), Is.True);
            }


            [Test]
            public void SomeEmpty()
            {
                for (int i = 4; i < 9; i++)
                {
                    array.Add(i);
                }

                array.AddAll(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(array, Has.Count.EqualTo(5));
                Assert.That(array.Check(), Is.True);
            }


            [Test]
            public void EmptySome()
            {
                array.AddAll(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Has.Count.EqualTo(4));
                    Assert.That(array.Check(), Is.True);
                    Assert.That(array[0], Is.EqualTo(0));
                    Assert.That(array[1], Is.EqualTo(1));
                    Assert.That(array[2], Is.EqualTo(4));
                    Assert.That(array[3], Is.EqualTo(9));
                });
            }


            [Test]
            public void SomeSome()
            {
                for (int i = 3; i < 9; i++)
                {
                    array.Add(i);
                }

                array.AddAll(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(array, Has.Count.EqualTo(9));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 3, 4, 5, 6, 7, 8, 9), Is.True);
                });
            }


            [TearDown]
            public void Dispose() { array = null; }
        }



        [TestFixture]
        public class AddSorted
        {
            private int sqr(int i) { return i * i; }


            private int bad(int i) { return i * (5 - i); }

            private SortedArray<int> array;


            [SetUp]
            public void Init() { array = new SortedArray<int>(new IC()); }


            [Test]
            public void EmptyEmpty()
            {
                array.AddSorted(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(array, Is.Empty);
                Assert.That(array.Check(), Is.True);
            }



            [Test]
            public void SomeEmpty()
            {
                for (int i = 4; i < 9; i++)
                {
                    array.Add(i);
                }

                array.AddSorted(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(array, Has.Count.EqualTo(5));
                Assert.That(array.Check(), Is.True);
            }



            [Test]
            public void EmptySome()
            {
                array.AddSorted(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(array, Has.Count.EqualTo(4));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(array[0], Is.EqualTo(0));
                    Assert.That(array[1], Is.EqualTo(1));
                    Assert.That(array[2], Is.EqualTo(4));
                    Assert.That(array[3], Is.EqualTo(9));
                });
            }



            [Test]
            public void SomeSome()
            {
                for (int i = 3; i < 9; i++)
                {
                    array.Add(i);
                }

                array.AddSorted(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(array, Has.Count.EqualTo(9));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 3, 4, 5, 6, 7, 8, 9), Is.True);
                });
            }

            [Test]
            public void EmptyBad()
            {
                var exception = Assert.Throws<ArgumentException>(() => array.AddSorted(new FunEnumerable(9, new Func<int, int>(bad))));
                Assert.That(exception.Message, Is.EqualTo("Argument not sorted"));
            }


            [TearDown]
            public void Dispose() { array = null; }
        }

        [TestFixture]
        public class Rest
        {
            private SortedArray<int> array, array2;


            [SetUp]
            public void Init()
            {
                array = new SortedArray<int>(new IC());
                array2 = new SortedArray<int>(new IC());
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                for (int i = 0; i < 10; i++)
                {
                    array2.Add(2 * i);
                }
            }


            [Test]
            public void RemoveAll()
            {
                array.RemoveAll(array2.RangeFromTo(3, 7));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Has.Count.EqualTo(8));
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 2, 3, 5, 7, 8, 9), Is.True);
                });
                array.RemoveAll(array2.RangeFromTo(3, 7));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Has.Count.EqualTo(8));
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 2, 3, 5, 7, 8, 9), Is.True);
                });
                array.RemoveAll(array2.RangeFromTo(13, 17));
                Assert.That(array, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 2, 3, 5, 7, 8, 9), Is.True);
                });
                array.RemoveAll(array2.RangeFromTo(3, 17));
                Assert.That(array, Has.Count.EqualTo(7));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 0, 1, 2, 3, 5, 7, 9), Is.True);
                });
                for (int i = 0; i < 10; i++)
                {
                    array2.Add(i);
                }

                array.RemoveAll(array2.RangeFromTo(-1, 10));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Is.Empty);
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array), Is.True);
                });
            }


            [Test]
            public void RetainAll()
            {
                array.RetainAll(array2.RangeFromTo(3, 17));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Has.Count.EqualTo(3));
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 4, 6, 8), Is.True);
                });
                array.RetainAll(array2.RangeFromTo(1, 17));
                Assert.That(array, Has.Count.EqualTo(3));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 4, 6, 8), Is.True);
                });
                array.RetainAll(array2.RangeFromTo(3, 5));
                Assert.That(array, Has.Count.EqualTo(1));
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array, 4), Is.True);
                });
                array.RetainAll(array2.RangeFromTo(7, 17));
                Assert.That(array, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array), Is.True);
                });
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                array.RetainAll(array2.RangeFromTo(5, 5));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Is.Empty);
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array), Is.True);
                });
                for (int i = 0; i < 10; i++)
                {
                    array.Add(i);
                }

                array.RetainAll(array2.RangeFromTo(15, 25));
                Assert.Multiple(() =>
                {
                    Assert.That(array, Is.Empty);
                    Assert.That(array.Check(), Is.True);
                    Assert.That(IC.Eq(array), Is.True);
                });
            }


            [Test]
            public void ContainsAll()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(array.ContainsAll(array2), Is.False);
                    Assert.That(array.ContainsAll(array), Is.True);
                });
                array2.Clear();
                Assert.That(array.ContainsAll(array2), Is.True);
                array.Clear();
                Assert.That(array.ContainsAll(array2), Is.True);
                array2.Add(8);
                Assert.That(array.ContainsAll(array2), Is.False);
            }


            [Test]
            public void RemoveInterval()
            {
                array.RemoveInterval(3, 4);
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(array, Has.Count.EqualTo(6));
                    Assert.That(IC.Eq(array, 0, 1, 2, 7, 8, 9), Is.True);
                });
                array.RemoveInterval(2, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(array, Has.Count.EqualTo(3));
                    Assert.That(IC.Eq(array, 0, 1, 9), Is.True);
                });
                array.RemoveInterval(0, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(array.Check(), Is.True);
                    Assert.That(array, Is.Empty);
                    Assert.That(IC.Eq(array), Is.True);
                });
            }


            [Test]
            public void RemoveRangeBad1()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveInterval(-3, 8));
            }


            [Test]
            public void RemoveRangeBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveInterval(3, -8));
            }


            [Test]
            public void RemoveRangeBad3()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveInterval(3, 8));
            }


            [Test]
            public void GetRange()
            {
                SCG.IEnumerable<int> e = array[3, 3];

                Assert.That(IC.Eq(e, 3, 4, 5), Is.True);
                e = array[3, 0];
                Assert.That(IC.Eq(e), Is.True);
            }

            [Test]
            public void GetRangeBad1()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { object foo = array[-3, 0]; });
            }

            [Test]
            public void GetRangeBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { object foo = array[3, -1]; });
            }

            [Test]
            public void GetRangeBad3()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { object foo = array[3, 8]; });
            }

            [TearDown]
            public void Dispose() { array = null; array2 = null; }
        }
    }




    namespace Sync
    {
        [TestFixture]
        public class SyncRoot
        {
            private SortedArray<int> tree;
            private readonly object mySyncRoot = new();
            private readonly int sz = 5000;


            [Test]
            public void Safe()
            {
                System.Threading.Thread t1 = new(new System.Threading.ThreadStart(safe1));
                System.Threading.Thread t2 = new(new System.Threading.ThreadStart(safe2));

                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
                Assert.That(tree, Has.Count.EqualTo(2 * sz + 1));
                Assert.That(tree.Check(), Is.True);
            }


            //[Test]
            public void UnSafe()
            {
                bool bad = false;

                for (int i = 0; i < 10; i++)
                {
                    System.Threading.Thread t1 = new(new System.Threading.ThreadStart(unsafe1));
                    System.Threading.Thread t2 = new(new System.Threading.ThreadStart(unsafe2));

                    t1.Start();
                    t2.Start();
                    t1.Join();
                    t2.Join();
                    if (bad = 2 * sz + 1 != tree.Count)
                    {
                        Console.WriteLine("{0}::Unsafe(): bad at {1}", GetType(), i);
                        break;
                    }
                }

                Assert.That(bad, Is.True, "No sync problems!");
            }


            [Test]
            public void SafeUnSafe()
            {
                System.Threading.Thread t1 = new(new System.Threading.ThreadStart(unsafe1));
                System.Threading.Thread t2 = new(new System.Threading.ThreadStart(unsafe2));

                t1.Start();
                t1.Join();
                t2.Start();
                t2.Join();
                Assert.That(tree, Has.Count.EqualTo(2 * sz + 1));
            }


            [SetUp]
            public void Init() { tree = new SortedArray<int>(new IC()); }


            private void unsafe1()
            {
                for (int i = 0; i < 2 * sz; i++)
                {
                    tree.Add(i * 2);
                }

                for (int i = 1; i < sz; i++)
                {
                    tree.Remove(i * 4);
                }
            }


            private void safe1()
            {
                for (int i = 0; i < 2 * sz; i++)
                {
                    lock (mySyncRoot)
                    {
                        tree.Add(i * 2);
                    }
                }

                for (int i = 1; i < sz; i++)
                {
                    lock (mySyncRoot)
                    {
                        tree.Remove(i * 4);
                    }
                }
            }


            private void unsafe2()
            {
                for (int i = 2 * sz; i > 0; i--)
                {
                    tree.Add(i * 2 + 1);
                }

                for (int i = sz; i > 0; i--)
                {
                    tree.Remove(i * 4 + 1);
                }
            }


            private void safe2()
            {
                for (int i = 2 * sz; i > 0; i--)
                {
                    lock (mySyncRoot)
                    {
                        tree.Add(i * 2 + 1);
                    }
                }

                for (int i = sz; i > 0; i--)
                {
                    lock (mySyncRoot)
                    {
                        tree.Remove(i * 4 + 1);
                    }
                }
            }


            [TearDown]
            public void Dispose() { tree = null; }
        }



        //[TestFixture]
        public class ConcurrentQueries
        {
            private SortedArray<int> tree;
            private readonly int sz = 500000;


            [SetUp]
            public void Init()
            {
                tree = new SortedArray<int>(new IC());
                for (int i = 0; i < sz; i++)
                {
                    tree.Add(i);
                }
            }

            private class A
            {
                public int count = 0;
                private readonly SortedArray<int> t;


                public A(SortedArray<int> t) { this.t = t; }


                public void a(int i) { count++; }


                public void traverse() { t.Apply(new Action<int>(a)); }
            }




            [Test]
            public void Safe()
            {
                A a = new(tree);

                a.traverse();
                Assert.That(a.count, Is.EqualTo(sz));
            }


            [Test]
            public void RegrettablyUnsafe()
            {
                System.Threading.Thread[] t = new System.Threading.Thread[10];
                A[] a = new A[10];
                for (int i = 0; i < 10; i++)
                {
                    a[i] = new A(tree);
                    t[i] = new System.Threading.Thread(new System.Threading.ThreadStart(a[i].traverse));
                }

                for (int i = 0; i < 10; i++)
                {
                    t[i].Start();
                }

                for (int i = 0; i < 10; i++)
                {
                    t[i].Join();
                }

                for (int i = 0; i < 10; i++)
                {
                    Assert.That(a[i].count, Is.EqualTo(sz));
                }
            }


            [TearDown]
            public void Dispose() { tree = null; }
        }
    }




    namespace Hashing
    {
        [TestFixture]
        public class ISequenced
        {
            private ISequenced<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new SortedArray<int>(8, SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new SortedArray<int>(8, SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new SortedArray<int>(8, new ReverseIntegerComparer(), EqualityComparer<int>.Default);
            }


            [Test]
            public void EmptyEmpty()
            {
                Assert.That(dit.SequencedEquals(dat), Is.True);
            }


            [Test]
            public void EmptyNonEmpty()
            {
                dit.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dat), Is.False);
                    Assert.That(dat.SequencedEquals(dit), Is.False);
                });
            }

            [Test]
            public void HashVal()
            {
                Assert.That(dit.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode()));
                dit.Add(3);
                Assert.That(dit.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(3)));
                dit.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(3, 7)));
                    Assert.That(dut.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode()));
                });
                dut.Add(3);
                Assert.That(dut.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(3)));
                dut.Add(7);
                Assert.That(dut.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(7, 3)));
            }


            [Test]
            public void Normal()
            {
                dit.Add(3);
                dit.Add(7);
                dat.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dat), Is.False);
                    Assert.That(dat.SequencedEquals(dit), Is.False);
                });
                dat.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dat), Is.True);
                    Assert.That(dat.SequencedEquals(dit), Is.True);
                });
            }


            [Test]
            public void WrongOrder()
            {
                dit.Add(3);
                dut.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dut), Is.True);
                    Assert.That(dut.SequencedEquals(dit), Is.True);
                });
                dit.Add(7);
                dut.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dut), Is.False);
                    Assert.That(dut.SequencedEquals(dit), Is.False);
                });
            }


            [Test]
            public void Reflexive()
            {
                Assert.That(dit.SequencedEquals(dit), Is.True);
                dit.Add(3);
                Assert.That(dit.SequencedEquals(dit), Is.True);
                dit.Add(7);
                Assert.That(dit.SequencedEquals(dit), Is.True);
            }


            [TearDown]
            public void Dispose()
            {
                dit = null;
                dat = null;
                dut = null;
            }
        }



        [TestFixture]
        public class IEditableCollection
        {
            private ICollection<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new SortedArray<int>(8, SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new SortedArray<int>(8, SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new SortedArray<int>(8, new ReverseIntegerComparer(), EqualityComparer<int>.Default);
            }


            [Test]
            public void EmptyEmpty()
            {
                Assert.That(dit.UnsequencedEquals(dat), Is.True);
            }


            [Test]
            public void EmptyNonEmpty()
            {
                dit.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dat), Is.False);
                    Assert.That(dat.UnsequencedEquals(dit), Is.False);
                });
            }


            [Test]
            public void HashVal()
            {
                Assert.That(dit.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode()));
                dit.Add(3);
                Assert.That(dit.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(3)));
                dit.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(3, 7)));
                    Assert.That(dut.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode()));
                });
                dut.Add(3);
                Assert.That(dut.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(3)));
                dut.Add(7);
                Assert.That(dut.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(7, 3)));
            }


            [Test]
            public void Normal()
            {
                dit.Add(3);
                dit.Add(7);
                dat.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dat), Is.False);
                    Assert.That(dat.UnsequencedEquals(dit), Is.False);
                });
                dat.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dat), Is.True);
                    Assert.That(dat.UnsequencedEquals(dit), Is.True);
                });
            }


            [Test]
            public void WrongOrder()
            {
                dit.Add(3);
                dut.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dut), Is.True);
                    Assert.That(dut.UnsequencedEquals(dit), Is.True);
                });
                dit.Add(7);
                dut.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dut), Is.True);
                    Assert.That(dut.UnsequencedEquals(dit), Is.True);
                });
            }


            [Test]
            public void Reflexive()
            {
                Assert.That(dit.UnsequencedEquals(dit), Is.True);
                dit.Add(3);
                Assert.That(dit.UnsequencedEquals(dit), Is.True);
                dit.Add(7);
                Assert.That(dit.UnsequencedEquals(dit), Is.True);
            }


            [TearDown]
            public void Dispose()
            {
                dit = null;
                dat = null;
                dut = null;
            }
        }

    }
}