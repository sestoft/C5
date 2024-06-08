// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests.trees.TreeSet
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            TreeSet<int> factory() { return new TreeSet<int>(TenEqualityComparer.Instance); }
            new Templates.Events.SortedIndexedTester<TreeSet<int>>().Test(factory);
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new TreeSet<T>(); }
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
    public class Combined
    {
        private TreeSet<SCG.KeyValuePair<int, int>> lst;


        [SetUp]
        public void Init()
        {
            lst = new TreeSet<SCG.KeyValuePair<int, int>>(new KeyValuePairComparer<int, int>(new IntegerComparer()));
            for (int i = 0; i < 10; i++)
            {
                lst.Add(new SCG.KeyValuePair<int, int>(i, i + 30));
            }
        }


        [TearDown]
        public void Dispose() { lst.Dispose(); }


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
            ICollection<string> coll = new TreeSet<string>();
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
    public class Ranges
    {
        private TreeSet<int> tree;

        private SCG.IComparer<int> c;


        [SetUp]
        public void Init()
        {
            c = new IntegerComparer();
            tree = new TreeSet<int>(c);
            for (int i = 1; i <= 10; i++)
            {
                tree.Add(i * 2);
            }
        }


        [Test]
        public void Enumerator()
        {
            SCG.IEnumerator<int> e = tree.RangeFromTo(5, 17).GetEnumerator();
            int i = 3;

            while (e.MoveNext())
            {
                Assert.That(e.Current, Is.EqualTo(2 * i++));
            }

            Assert.That(i, Is.EqualTo(9));
        }


        [Test]
        public void Enumerator2()
        {
            SCG.IEnumerator<int> e = tree.RangeFromTo(5, 17).GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => { int i = e.Current; });
        }


        [Test]
        public void Enumerator3()
        {
            SCG.IEnumerator<int> e = tree.RangeFromTo(5, 17).GetEnumerator();

            while (e.MoveNext())
            {
                ;
            }

            Assert.Throws<InvalidOperationException>(() => { int i = e.Current; });
        }


        [Test]
        public void Remove()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];

            tree.RemoveRangeFrom(18);
            Assert.That(tree, Is.EqualTo(new[] { 2, 4, 6, 8, 10, 12, 14, 16 }));
            tree.RemoveRangeFrom(28);
            Assert.That(tree, Is.EqualTo(new[] { 2, 4, 6, 8, 10, 12, 14, 16 }));
            tree.RemoveRangeFrom(2);
            Assert.That(tree, Is.Empty);
            foreach (int i in all)
            {
                tree.Add(i);
            }

            tree.RemoveRangeTo(10);
            Assert.That(tree, Is.EqualTo(new[] { 10, 12, 14, 16, 18, 20 }));
            tree.RemoveRangeTo(2);
            Assert.That(tree, Is.EqualTo(new[] { 10, 12, 14, 16, 18, 20 }));
            tree.RemoveRangeTo(21);
            Assert.That(tree, Is.Empty);
            foreach (int i in all)
            {
                tree.Add(i);
            }

            tree.RemoveRangeFromTo(4, 8);
            Assert.That(tree, Is.EqualTo(new[] { 2, 8, 10, 12, 14, 16, 18, 20 }));
            tree.RemoveRangeFromTo(14, 28);
            Assert.That(tree, Is.EqualTo(new[] { 2, 8, 10, 12 }));
            tree.RemoveRangeFromTo(0, 9);
            Assert.That(tree, Is.EqualTo(new[] { 10, 12 }));
            tree.RemoveRangeFromTo(0, 81);
            Assert.That(tree, Is.Empty);
        }

        [Test]
        public void Normal()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];

            Assert.Multiple(() =>
            {
                Assert.That(tree, Is.EqualTo(all));
                Assert.That(tree.RangeAll(), Is.EqualTo(all));
                Assert.That(tree.RangeAll(), Has.Count.EqualTo(10));
                Assert.That(tree.RangeFrom(11), Is.EqualTo(new[] { 12, 14, 16, 18, 20 }));
                Assert.That(tree.RangeFrom(11), Has.Count.EqualTo(5));
                Assert.That(tree.RangeFrom(12), Is.EqualTo(new[] { 12, 14, 16, 18, 20 }));
                Assert.That(tree.RangeFrom(2), Is.EqualTo(all));
                Assert.That(tree.RangeFrom(1), Is.EqualTo(all));
                Assert.That(tree.RangeFrom(21), Is.Empty);
                Assert.That(tree.RangeFrom(20), Is.EqualTo(new[] { 20 }));
                Assert.That(tree.RangeTo(8), Is.EqualTo(new[] { 2, 4, 6 }));
                Assert.That(tree.RangeTo(7), Is.EqualTo(new[] { 2, 4, 6 }));
                Assert.That(tree.RangeTo(7), Has.Count.EqualTo(3));
                Assert.That(tree.RangeTo(2), Is.Empty);
                Assert.That(tree.RangeTo(1), Is.Empty);
                Assert.That(tree.RangeTo(3), Is.EqualTo(new[] { 2 }));
                Assert.That(tree.RangeTo(20), Is.EqualTo(new[] { 2, 4, 6, 8, 10, 12, 14, 16, 18 }));
                Assert.That(tree.RangeTo(21), Is.EqualTo(all));
                Assert.That(tree.RangeFromTo(7, 12), Is.EqualTo(new[] { 8, 10 }));
                Assert.That(tree.RangeFromTo(6, 11), Is.EqualTo(new[] { 6, 8, 10 }));
                Assert.That(tree.RangeFromTo(1, 12), Is.EqualTo(new[] { 2, 4, 6, 8, 10 }));
                Assert.That(tree.RangeFromTo(1, 12), Has.Count.EqualTo(5));
                Assert.That(tree.RangeFromTo(2, 12), Is.EqualTo(new[] { 2, 4, 6, 8, 10 }));
                Assert.That(tree.RangeFromTo(6, 21), Is.EqualTo(new[] { 6, 8, 10, 12, 14, 16, 18, 20 }));
                Assert.That(tree.RangeFromTo(6, 20), Is.EqualTo(new[] { 6, 8, 10, 12, 14, 16, 18 }));
            });
        }

        [Test]
        public void Backwards()
        {
            int[] all = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];
            int[] lla = [20, 18, 16, 14, 12, 10, 8, 6, 4, 2];

            Assert.Multiple(() =>
            {
                Assert.That(tree, Is.EqualTo(all));
                Assert.That(tree.RangeAll().Backwards(), Is.EqualTo(lla));
                Assert.That(tree.RangeFrom(11).Backwards(), Is.EqualTo(new[] { 20, 18, 16, 14, 12 }));
                Assert.That(tree.RangeFrom(12).Backwards(), Is.EqualTo(new[] { 20, 18, 16, 14, 12 }));
                Assert.That(tree.RangeFrom(2).Backwards(), Is.EqualTo(lla));
                Assert.That(tree.RangeFrom(1).Backwards(), Is.EqualTo(lla));
                Assert.That(tree.RangeFrom(21).Backwards(), Is.Empty);
                Assert.That(tree.RangeFrom(20).Backwards(), Is.EqualTo(new[] { 20 }));
                Assert.That(tree.RangeTo(8).Backwards(), Is.EqualTo(new[] { 6, 4, 2 }));
                Assert.That(tree.RangeTo(7).Backwards(), Is.EqualTo(new[] { 6, 4, 2 }));
                Assert.That(tree.RangeTo(2).Backwards(), Is.Empty);
                Assert.That(tree.RangeTo(1).Backwards(), Is.Empty);
                Assert.That(tree.RangeTo(3).Backwards(), Is.EqualTo(new[] { 2 }));
                Assert.That(tree.RangeTo(20).Backwards(), Is.EqualTo(new[] { 18, 16, 14, 12, 10, 8, 6, 4, 2 }));
                Assert.That(tree.RangeTo(21).Backwards(), Is.EqualTo(lla));
                Assert.That(tree.RangeFromTo(7, 12).Backwards(), Is.EqualTo(new[] { 10, 8 }));
                Assert.That(tree.RangeFromTo(6, 11).Backwards(), Is.EqualTo(new[] { 10, 8, 6 }));
                Assert.That(tree.RangeFromTo(1, 12).Backwards(), Is.EqualTo(new[] { 10, 8, 6, 4, 2 }));
                Assert.That(tree.RangeFromTo(2, 12).Backwards(), Is.EqualTo(new[] { 10, 8, 6, 4, 2 }));
                Assert.That(tree.RangeFromTo(6, 21).Backwards(), Is.EqualTo(new[] { 20, 18, 16, 14, 12, 10, 8, 6 }));
                Assert.That(tree.RangeFromTo(6, 20).Backwards(), Is.EqualTo(new[] { 18, 16, 14, 12, 10, 8, 6 }));
            });
        }

        [Test]
        public void Direction()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(tree.RangeFrom(20).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(tree.RangeTo(7).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(tree.RangeFromTo(1, 12).Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(tree.RangeAll().Direction, Is.EqualTo(C5.Direction.Forwards));
                Assert.That(tree.Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(tree.RangeFrom(20).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(tree.RangeTo(7).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(tree.RangeFromTo(1, 12).Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
                Assert.That(tree.RangeAll().Backwards().Direction, Is.EqualTo(C5.Direction.Backwards));
            });
        }


        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
            c = null;
        }
    }

    [TestFixture]
    public class BagItf
    {
        private TreeSet<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 10);
            }
        }


        [Test]
        public void Both()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.ContainsCount(7), Is.EqualTo(0));
                Assert.That(tree.ContainsCount(10), Is.EqualTo(1));
            });
            tree.RemoveAllCopies(10);
            Assert.That(tree.ContainsCount(10), Is.EqualTo(0));
            tree.RemoveAllCopies(7);
        }


        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
        }
    }


    [TestFixture]
    public class Div
    {
        private TreeSet<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
        }


        private void loadup()
        {
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 10);
            }
        }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new TreeSet<int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor3()
        {
            Assert.Throws<NullReferenceException>(() => new TreeSet<int>(null, EqualityComparer<int>.Default));
        }

        [Test]
        public void NullEqualityComparerinConstructor4()
        {
            Assert.Throws<NullReferenceException>(() => new TreeSet<int>(SCG.Comparer<int>.Default, null));
        }

        [Test]
        public void NullEqualityComparerinConstructor5()
        {
            Assert.Throws<NullReferenceException>(() => new TreeSet<int>(null, null));
        }

        [Test]
        public void Choose()
        {
            tree.Add(7);
            Assert.That(tree.Choose(), Is.EqualTo(7));
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => tree.Choose());
        }


        [Test]
        public void NoDuplicates()
        {
            Assert.That(tree.AllowsDuplicates, Is.False);
            loadup();
            Assert.That(tree.AllowsDuplicates, Is.False);
        }

        [Test]
        public void Add()
        {
            Assert.That(tree.Add(17), Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Add(17), Is.False);
                Assert.That(tree.Add(18), Is.True);
            });
            Assert.Multiple(() =>
            {
                Assert.That(tree.Add(18), Is.False);
                Assert.That(tree, Has.Count.EqualTo(2));
            });
        }


        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
        }
    }


    [TestFixture]
    public class FindOrAdd
    {
        private TreeSet<SCG.KeyValuePair<int, string>> bag;


        [SetUp]
        public void Init()
        {
            bag = new TreeSet<SCG.KeyValuePair<int, string>>(new KeyValuePairComparer<int, string>(new IntegerComparer()));
        }


        [TearDown]
        public void Dispose()
        {
            bag.Dispose();
        }


        [Test]
        public void Test()
        {
            var p = new SCG.KeyValuePair<int, string>(3, "tre");

            Assert.That(bag.FindOrAdd(ref p), Is.False);
            p = new SCG.KeyValuePair<int, string>(p.Key, "drei");
            Assert.That(bag.FindOrAdd(ref p), Is.True);
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
        private TreeSet<int> list;
        private Func<int, bool> pred;

        [SetUp]
        public void Init()
        {
            list = new TreeSet<int>(TenEqualityComparer.Instance);
            pred = delegate (int i) { return i % 5 == 0; };
        }

        [TearDown]
        public void Dispose() { list.Dispose(); }

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
            Assert.That(0 <= list.FindIndex(pred), Is.False);
            list.AddAll([4, 22, 67, 37]);
            Assert.That(0 <= list.FindIndex(pred), Is.False);
            list.AddAll([45, 122, 675, 137]);
            Assert.That(list.FindIndex(pred), Is.EqualTo(3));
        }

        [Test]
        public void FindLastIndex()
        {
            Assert.That(0 <= list.FindLastIndex(pred), Is.False);
            list.AddAll([4, 22, 67, 37]);
            Assert.That(0 <= list.FindLastIndex(pred), Is.False);
            list.AddAll([45, 122, 675, 137]);
            Assert.That(list.FindLastIndex(pred), Is.EqualTo(7));
        }
    }

    [TestFixture]
    public class UniqueItems
    {
        private TreeSet<int> list;

        [SetUp]
        public void Init() { list = []; }

        [TearDown]
        public void Dispose() { list.Dispose(); }

        [Test]
        public void Test()
        {
            Assert.Multiple(() =>
            {
                Assert.That(list.UniqueItems(), Is.Empty);
                Assert.That(list.ItemMultiplicities(), Is.Empty);
            });
            list.AddAll([7, 9, 7]);
            Assert.Multiple(() =>
            {
                Assert.That(list.UniqueItems(), Is.EquivalentTo(new[] { 7, 9 }));
                Assert.That(list.ItemMultiplicities(), Is.EquivalentTo(new[] { SCG.KeyValuePair.Create(7, 1), SCG.KeyValuePair.Create(9, 1) }));
            });
        }
    }

    [TestFixture]
    public class ArrayTest
    {
        private TreeSet<int> tree;
        private int[] a;

        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
            a = new int[10];
            for (int i = 0; i < 10; i++)
            {
                a[i] = 1000 + i;
            }
        }

        [TearDown]
        public void Dispose() { tree.Dispose(); }

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
    public class Remove
    {
        private TreeSet<int> tree;

        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 10);
            }
        }


        [Test]
        public void SmallTrees()
        {
            tree.Clear();
            tree.Add(7);
            tree.Add(9);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(7), Is.True);
                Assert.That(tree.Check(""), Is.True);
            });
        }


        [Test]
        public void ByIndex()
        {
            //Remove root!
            int n = tree.Count;
            int i = tree[10];

            tree.RemoveAt(10);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 1));
            });

            //Low end
            i = tree.FindMin();
            tree.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 2));
            });

            //high end
            i = tree.FindMax();
            tree.RemoveAt(tree.Count - 1);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 3));
            });

            //Some leaf
            i = 18;
            tree.RemoveAt(7);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 4));
            });
        }


        [Test]
        public void AlmostEmpty()
        {
            //Almost empty
            tree.Clear();
            tree.Add(3);
            tree.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(3), Is.False);
                Assert.That(tree, Is.Empty);
            });
        }


        [Test]
        public void Empty()
        {
            tree.Clear();
            var exception = Assert.Throws<IndexOutOfRangeException>(() => tree.RemoveAt(0));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void HighIndex()
        {
            var exception = Assert.Throws<IndexOutOfRangeException>(() => tree.RemoveAt(tree.Count));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void LowIndex()
        {
            var exception = Assert.Throws<IndexOutOfRangeException>(() => tree.RemoveAt(-1));
            Assert.That(exception.Message, Is.EqualTo("Index out of range for sequenced collectionvalue"));
        }


        [Test]
        public void Normal()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(-20), Is.False);

                //No demote case, with move_item
                Assert.That(tree.Remove(20), Is.True);
                Assert.That(tree.Check("T1"), Is.True);
            });
            Assert.That(tree.Remove(20), Is.False);

            //plain case 2
            Assert.That(tree.Remove(14), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //case 1b
            Assert.That(tree.Remove(25), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //case 1c
            Assert.That(tree.Remove(29), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //1a (terminating)
            Assert.That(tree.Remove(10), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //2+1b
            Assert.That(tree.Remove(12), Is.True);
            Assert.That(tree.Remove(11), Is.True);

            //1a+1b
            Assert.That(tree.Remove(18), Is.True);
            Assert.That(tree.Remove(13), Is.True);
            Assert.That(tree.Remove(15), Is.True);

            //2+1c
            for (int i = 0; i < 10; i++)
            {
                tree.Add(50 - 2 * i);
            }

            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(42), Is.True);
                Assert.That(tree.Remove(38), Is.True);
                Assert.That(tree.Remove(28), Is.True);
                Assert.That(tree.Remove(40), Is.True);

                //
                Assert.That(tree.Remove(16), Is.True);
                Assert.That(tree.Remove(23), Is.True);
                Assert.That(tree.Remove(17), Is.True);
                Assert.That(tree.Remove(19), Is.True);
                Assert.That(tree.Remove(50), Is.True);
                Assert.That(tree.Remove(26), Is.True);
                Assert.That(tree.Remove(21), Is.True);
                Assert.That(tree.Remove(22), Is.True);
                Assert.That(tree.Remove(24), Is.True);
            });
            for (int i = 0; i < 48; i++)
            {
                tree.Remove(i);
            }

            Assert.Multiple(() =>
            {
                //Almost empty tree:
                Assert.That(tree.Remove(26), Is.False);
                Assert.That(tree.Remove(48), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
            });

            //Empty tree:
            Assert.That(tree.Remove(26), Is.False);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
        }


        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
        }
    }



    [TestFixture]
    public class PredecessorStructure
    {
        private TreeSet<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
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
            Assert.Throws<NoSuchItemException>(() => tree.WeakPredecessor(-2));
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
            tree.Dispose();
        }
    }

    [TestFixture]
    public class PriorityQueue
    {
        private TreeSet<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
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
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
            });
            Assert.That(tree.FindMin(), Is.EqualTo(2));
            Assert.That(tree.FindMax(), Is.EqualTo(3));
            Assert.That(tree.DeleteMin(), Is.EqualTo(2));
            Assert.That(tree.DeleteMax(), Is.EqualTo(3));
            Assert.That(tree.Check("Normal test 2"), Is.True, "Bad tree");
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
            tree.Dispose();
        }
    }

    [TestFixture]
    public class IndexingAndCounting
    {
        private TreeSet<int> tree;

        [SetUp]
        public void Init()
        {
            tree = new TreeSet<int>(new IntegerComparer());
        }

        private void populate()
        {
            tree.Add(30);
            tree.Add(50);
            tree.Add(10);
            tree.Add(70);
        }

        [Test]
        public void ToArray()
        {
            populate();

            int[] a = tree.ToArray();

            Assert.That(a.Length, Is.EqualTo(4));
            Assert.Multiple(() =>
            {
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
                Assert.That(tree.IndexOf(20), Is.EqualTo(-1));
                Assert.That(tree.LastIndexOf(20), Is.EqualTo(-1));
            });
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree[0], Is.EqualTo(10));
                Assert.That(tree[1], Is.EqualTo(30));
                Assert.That(tree[2], Is.EqualTo(50));
                Assert.That(tree[3], Is.EqualTo(70));
                Assert.That(tree.IndexOf(10), Is.EqualTo(0));
                Assert.That(tree.IndexOf(30), Is.EqualTo(1));
                Assert.That(tree.IndexOf(50), Is.EqualTo(2));
                Assert.That(tree.IndexOf(70), Is.EqualTo(3));
                Assert.That(tree.IndexOf(20), Is.EqualTo(~1));
                Assert.That(tree.IndexOf(0), Is.EqualTo(~0));
                Assert.That(tree.IndexOf(90), Is.EqualTo(~4));
                Assert.That(tree.LastIndexOf(10), Is.EqualTo(0));
                Assert.That(tree.LastIndexOf(30), Is.EqualTo(1));
                Assert.That(tree.LastIndexOf(50), Is.EqualTo(2));
                Assert.That(tree.LastIndexOf(70), Is.EqualTo(3));
                Assert.That(tree.LastIndexOf(20), Is.EqualTo(~1));
                Assert.That(tree.LastIndexOf(0), Is.EqualTo(~0));
                Assert.That(tree.LastIndexOf(90), Is.EqualTo(~4));
            });
        }


        [Test]
        public void IndexTooLarge()
        {
            populate();
            Assert.Throws<IndexOutOfRangeException>(() => Console.WriteLine(tree[4]));
        }


        [Test]
        public void IndexTooSmall()
        {
            populate();
            Assert.Throws<IndexOutOfRangeException>(() => Console.WriteLine(tree[-1]));
        }


        [Test]
        public void FilledTreeOutsideInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(90), Is.EqualTo(0));
                Assert.That(tree.CountFromTo(-20, 0), Is.EqualTo(0));
                Assert.That(tree.CountFromTo(80, 100), Is.EqualTo(0));
                Assert.That(tree.CountTo(0), Is.EqualTo(0));
                Assert.That(tree.CountTo(90), Is.EqualTo(4));
                Assert.That(tree.CountFromTo(-20, 90), Is.EqualTo(4));
                Assert.That(tree.CountFrom(0), Is.EqualTo(4));
            });
        }


        [Test]
        public void FilledTreeIntermediateInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(20), Is.EqualTo(3));
                Assert.That(tree.CountFromTo(20, 40), Is.EqualTo(1));
                Assert.That(tree.CountTo(40), Is.EqualTo(2));
            });
        }


        [Test]
        public void FilledTreeMatchingInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(30), Is.EqualTo(3));
                Assert.That(tree.CountFromTo(30, 70), Is.EqualTo(2));
                Assert.That(tree.CountFromTo(50, 30), Is.EqualTo(0));
                Assert.That(tree.CountFromTo(50, 50), Is.EqualTo(0));
                Assert.That(tree.CountTo(10), Is.EqualTo(0));
                Assert.That(tree.CountTo(50), Is.EqualTo(2));
            });
        }

        [Test]
        public void CountEmptyTree()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(20), Is.EqualTo(0));
                Assert.That(tree.CountFromTo(20, 40), Is.EqualTo(0));
                Assert.That(tree.CountTo(40), Is.EqualTo(0));
            });
        }

        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
        }
    }

    namespace ModificationCheck
    {
        [TestFixture]
        public class Enumerator
        {
            private TreeSet<int> tree;

            private SCG.IEnumerator<int> e;


            [SetUp]
            public void Init()
            {
                tree = new TreeSet<int>(new IntegerComparer());
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
                tree.Dispose();
                e.Dispose();
            }
        }

        [TestFixture]
        public class RangeEnumerator
        {
            private TreeSet<int> tree;

            private SCG.IEnumerator<int> e;

            [SetUp]
            public void Init()
            {
                tree = new TreeSet<int>(new IntegerComparer());
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
                tree.Dispose();
                e.Dispose();
            }
        }
    }

    namespace PathcopyPersistence
    {
        [TestFixture]
        public class Navigation
        {
            private TreeSet<int> tree, snap;

            private SCG.IComparer<int> ic;

            [SetUp]
            public void Init()
            {
                ic = new IntegerComparer();
                tree = new TreeSet<int>(ic);
                for (int i = 0; i <= 20; i++)
                {
                    tree.Add(2 * i + 1);
                }

                snap = (TreeSet<int>)tree.Snapshot();
                for (int i = 0; i <= 10; i++)
                {
                    tree.Remove(4 * i + 1);
                }
            }

            private bool twomodeleven(int i)
            {
                return i % 11 == 2;
            }

            [Test]
            public void InternalEnum()
            {
                Assert.That(snap.FindAll(new Func<int, bool>(twomodeleven)), Is.EqualTo(new[] { 13, 35 }));
            }

            public void MoreCut() { }

            [Test]
            public void Cut()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Cut(new HigherOrder.CubeRoot(64), out int lo, out bool lv, out int hi, out bool hv), Is.False);
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(5));
                    Assert.That(lo, Is.EqualTo(3));
                    Assert.That(snap.Cut(new HigherOrder.CubeRoot(125), out lo, out lv, out hi, out hv), Is.True);
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(7));
                    Assert.That(lo, Is.EqualTo(3));
                    Assert.That(snap.Cut(new HigherOrder.CubeRoot(125000), out lo, out lv, out _, out hv), Is.False);
                    Assert.That(lv && !hv, Is.True);
                    Assert.That(lo, Is.EqualTo(41));
                    Assert.That(snap.Cut(new HigherOrder.CubeRoot(-27), out _, out lv, out hi, out hv), Is.False);
                    Assert.That(!lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(1));
                });
            }


            [Test]
            public void Range()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.RangeFromTo(5, 16), Is.EqualTo(new[] { 5, 7, 9, 11, 13, 15 }));
                    Assert.That(snap.RangeFromTo(5, 17), Is.EqualTo(new[] { 5, 7, 9, 11, 13, 15 }));
                    Assert.That(snap.RangeFromTo(6, 16), Is.EqualTo(new[] { 7, 9, 11, 13, 15 }));
                });
                //Assert.AreEqual(snap.RangeFromTo(6, 16).Count, 5);
            }

            [Test]
            public void Contains()
            {
                Assert.That(snap.Contains(5), Is.True);
            }

            [Test]
            public void FindMin()
            {
                Assert.That(snap.FindMin(), Is.EqualTo(1));
            }

            [Test]
            public void FindMax()
            {
                Assert.That(snap.FindMax(), Is.EqualTo(41));
            }

            [Test]
            public void FindPredecessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.TryPredecessor(15, out int res) && res == 13, Is.True);
                    Assert.That(snap.TryPredecessor(16, out res) && res == 15, Is.True);
                    Assert.That(snap.TryPredecessor(17, out res) && res == 15, Is.True);
                    Assert.That(snap.TryPredecessor(18, out res) && res == 17, Is.True);

                    Assert.That(snap.TryPredecessor(2, out res) && res == 1, Is.True);

                    Assert.That(snap.TryPredecessor(1, out res), Is.False);
                    Assert.That(res, Is.EqualTo(0));
                });
            }


            [Test]
            public void FindSuccessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.TrySuccessor(15, out int res) && res == 17, Is.True);
                    Assert.That(snap.TrySuccessor(16, out res) && res == 17, Is.True);
                    Assert.That(snap.TrySuccessor(17, out res) && res == 19, Is.True);
                    Assert.That(snap.TrySuccessor(18, out res) && res == 19, Is.True);

                    Assert.That(snap.TrySuccessor(40, out res) && res == 41, Is.True);

                    Assert.That(snap.TrySuccessor(41, out res), Is.False);
                    Assert.That(res, Is.EqualTo(0));
                });
            }


            [Test]
            public void FindWeakPredecessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.TryWeakPredecessor(15, out int res) && res == 15, Is.True);
                    Assert.That(snap.TryWeakPredecessor(16, out res) && res == 15, Is.True);
                    Assert.That(snap.TryWeakPredecessor(17, out res) && res == 17, Is.True);
                    Assert.That(snap.TryWeakPredecessor(18, out res) && res == 17, Is.True);

                    Assert.That(snap.TryWeakPredecessor(1, out res) && res == 1, Is.True);

                    Assert.That(snap.TryWeakPredecessor(0, out res), Is.False);
                    Assert.That(res, Is.EqualTo(0));
                });
            }


            [Test]
            public void FindWeakSuccessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.TryWeakSuccessor(15, out int res) && res == 15, Is.True);
                    Assert.That(snap.TryWeakSuccessor(16, out res) && res == 17, Is.True);
                    Assert.That(snap.TryWeakSuccessor(17, out res) && res == 17, Is.True);
                    Assert.That(snap.TryWeakSuccessor(18, out res) && res == 19, Is.True);

                    Assert.That(snap.TryWeakSuccessor(41, out res) && res == 41, Is.True);

                    Assert.That(snap.TryWeakSuccessor(42, out res), Is.False);
                    Assert.That(res, Is.EqualTo(0));
                });
            }


            [Test]
            public void Predecessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Predecessor(15), Is.EqualTo(13));
                    Assert.That(snap.Predecessor(16), Is.EqualTo(15));
                    Assert.That(snap.Predecessor(17), Is.EqualTo(15));
                    Assert.That(snap.Predecessor(18), Is.EqualTo(17));
                });
            }


            [Test]
            public void Successor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Successor(15), Is.EqualTo(17));
                    Assert.That(snap.Successor(16), Is.EqualTo(17));
                    Assert.That(snap.Successor(17), Is.EqualTo(19));
                    Assert.That(snap.Successor(18), Is.EqualTo(19));
                });
            }


            [Test]
            public void WeakPredecessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.WeakPredecessor(15), Is.EqualTo(15));
                    Assert.That(snap.WeakPredecessor(16), Is.EqualTo(15));
                    Assert.That(snap.WeakPredecessor(17), Is.EqualTo(17));
                    Assert.That(snap.WeakPredecessor(18), Is.EqualTo(17));
                });
            }


            [Test]
            public void WeakSuccessor()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.WeakSuccessor(15), Is.EqualTo(15));
                    Assert.That(snap.WeakSuccessor(16), Is.EqualTo(17));
                    Assert.That(snap.WeakSuccessor(17), Is.EqualTo(17));
                    Assert.That(snap.WeakSuccessor(18), Is.EqualTo(19));
                });
            }


            [Test]
            public void CountTo()
            {
                var exception = Assert.Throws<NotSupportedException>(() =>
                {
                    int j = snap.CountTo(15);
                });
                Assert.That(exception.Message, Is.EqualTo("Indexing not supported for snapshots"));
            }

            [Test]
            public void Indexing()
            {
                var exception = Assert.Throws<NotSupportedException>(() =>
                {
                    int j = snap[4];
                });
                Assert.That(exception.Message, Is.EqualTo("Indexing not supported for snapshots"));
            }

            [Test]
            public void Indexing2()
            {
                var exception = Assert.Throws<NotSupportedException>(() =>
                {
                    int j = snap.IndexOf(5);
                });
                Assert.That(exception.Message, Is.EqualTo("Indexing not supported for snapshots"));
            }

            [TearDown]
            public void Dispose()
            {
                tree.Dispose();
                ic = null;
            }
        }

        [TestFixture]
        public class Single
        {
            private TreeSet<int> tree;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IntegerComparer();
                tree = new TreeSet<int>(ic);
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(2 * i + 1);
                }
            }


            [Test]
            public void EnumerationWithAdd()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                int i = 0;
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                foreach (int j in snap)
                {
                    Assert.That(j, Is.EqualTo(1 + 2 * i++));
                    tree.Add(21 - j);
                    Assert.Multiple(() =>
                    {
                        Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                        Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                        Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    });
                }
            }


            [Test]
            public void Remove()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                });
                tree.Remove(19);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                });
            }


            [Test]
            public void RemoveNormal()
            {
                tree.Clear();
                for (int i = 10; i < 20; i++)
                {
                    tree.Add(i);
                    tree.Add(i + 10);
                }

                int[] orig = [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29];
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(-20), Is.False);
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                    //No demote case, with move_item
                    Assert.That(tree.Remove(20), Is.True);
                    Assert.That(tree.Check("T1"), Is.True);
                });
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                Assert.That(tree.Remove(20), Is.False);

                //plain case 2
                tree.Snapshot();
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(14), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                    //case 1b
                    Assert.That(tree.Remove(25), Is.True);
                });
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                //case 1c
                Assert.That(tree.Remove(29), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                //1a (terminating)
                Assert.That(tree.Remove(10), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                //2+1b
                Assert.That(tree.Remove(12), Is.True);
                tree.Snapshot();
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(11), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                    //1a+1b
                    Assert.That(tree.Remove(18), Is.True);
                    Assert.That(tree.Remove(13), Is.True);
                    Assert.That(tree.Remove(15), Is.True);
                });
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                //2+1c
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(50 - 2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(42), Is.True);
                    Assert.That(tree.Remove(38), Is.True);
                    Assert.That(tree.Remove(28), Is.True);
                    Assert.That(tree.Remove(40), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");

                    //
                    Assert.That(tree.Remove(16), Is.True);
                    Assert.That(tree.Remove(23), Is.True);
                    Assert.That(tree.Remove(17), Is.True);
                    Assert.That(tree.Remove(19), Is.True);
                    Assert.That(tree.Remove(50), Is.True);
                    Assert.That(tree.Remove(26), Is.True);
                    Assert.That(tree.Remove(21), Is.True);
                    Assert.That(tree.Remove(22), Is.True);
                    Assert.That(tree.Remove(24), Is.True);
                });
                for (int i = 0; i < 48; i++)
                {
                    tree.Remove(i);
                }

                Assert.Multiple(() =>
                {
                    //Almost empty tree:
                    Assert.That(tree.Remove(26), Is.False);
                    Assert.That(tree.Remove(48), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                });

                //Empty tree:
                Assert.That(tree.Remove(26), Is.False);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
            }


            [Test]
            public void Add()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
                tree.Add(10);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
                tree.Add(16);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });

                //Promote+zigzig
                tree.Add(40);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                });
                for (int i = 1; i < 4; i++)
                {
                    tree.Add(40 - 2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });

                //Zigzag:
                tree.Add(32);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
            }

            [Test]
            public void Clear()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                tree.Clear();
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(snap, Is.EqualTo(orig), "Snap was changed!");
                    Assert.That(tree, Is.Empty);
                });
            }


            [Test]
            public void SnapSnap()
            {
                TreeSet<int> snap = (TreeSet<int>)tree.Snapshot();

                var exception = Assert.Throws<InvalidOperationException>(() => snap.Snapshot());
                Assert.That(exception.Message, Is.EqualTo("Cannot snapshot a snapshot"));
            }

            [TearDown]
            public void Dispose()
            {
                tree.Dispose();
                ic = null;
            }
        }

        [TestFixture]
        public class Multiple
        {
#pragma warning disable NUnit1032 // TODO: Breaks tests
            private TreeSet<int> tree;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

            private SCG.IComparer<int> ic;

            /* private bool eq(SCG.IEnumerable<int> me, int[] that)
            {
                int i = 0, maxind = that.Length - 1;

                foreach (int item in me)
                    if (i > maxind || ic.Compare(item, that[i++]) != 0)
                        return false;

                return true;
            } */

            [SetUp]
            public void Init()
            {
                ic = new IntegerComparer();
                tree = new TreeSet<int>(ic);
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(2 * i + 1);
                }
            }

            [Test]
            public void First()
            {
                TreeSet<int>[] snaps = new TreeSet<int>[10];

                for (int i = 0; i < 10; i++)
                {
                    snaps[i] = (TreeSet<int>)(tree.Snapshot());
                    tree.Add(2 * i);
                }

                for (int i = 0; i < 10; i++)
                {
                    Assert.That(snaps[i], Has.Count.EqualTo(i + 10));
                }

                snaps[5] = null;
                snaps[9] = null;
                GC.Collect();
                snaps[8].Dispose();
                tree.Remove(14);

                int[] res = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19];
                int[] snap7 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 17, 19];
                int[] snap3 = [0, 1, 2, 3, 4, 5, 7, 9, 11, 13, 15, 17, 19];

                Assert.Multiple(() =>
                {
                    Assert.That(snaps[3], Is.EqualTo(snap3), "Snap 3 was changed!");
                    Assert.That(snaps[7], Is.EqualTo(snap7), "Snap 7 was changed!");
                    Assert.That(tree, Is.EqualTo(res));
                    Assert.That(tree.Check("B"), Is.True);
                    Assert.That(snaps[3].Check("B"), Is.True);
                    Assert.That(snaps[7].Check("B"), Is.True);
                });
            }


            [Test]
            public void CollectingTheMaster()
            {
                TreeSet<int>[] snaps = new TreeSet<int>[10];

                for (int i = 0; i < 10; i++)
                {
                    snaps[i] = (TreeSet<int>)(tree.Snapshot());
                    tree.Add(2 * i);
                }

                tree = null;
                GC.Collect();
                for (int i = 0; i < 10; i++)
                {
                    Assert.That(snaps[i], Has.Count.EqualTo(i + 10));
                }

                snaps[5] = null;
                snaps[9] = null;
                GC.Collect();
                snaps[8].Dispose();

                int[] snap7 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 17, 19];
                int[] snap3 = [0, 1, 2, 3, 4, 5, 7, 9, 11, 13, 15, 17, 19];

                Assert.Multiple(() =>
                {
                    Assert.That(snaps[3], Is.EqualTo(snap3), "Snap 3 was changed!");
                    Assert.That(snaps[7], Is.EqualTo(snap7), "Snap 7 was changed!");
                    Assert.That(snaps[3].Check("B"), Is.True);
                    Assert.That(snaps[7].Check("B"), Is.True);
                });
            }


            [TearDown]
            public void Dispose()
            {
                //tree.Dispose();
                ic = null;
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
            private TreeSet<int> tree;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IntegerComparer();
                tree = new TreeSet<int>(ic);
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

                tree.Apply(new Action<int>(simple1.apply));
                Assert.Multiple(() =>
                {
                    Assert.That(simple1.appfield1, Is.EqualTo(0));
                    Assert.That(simple1.appfield2, Is.EqualTo(0));
                });

                Simple simple2 = new();

                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                tree.Apply(new Action<int>(simple2.apply));
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
                    Assert.That(tree.All(new Func<int, bool>(never)), Is.True);
                    Assert.That(tree.All(new Func<int, bool>(even)), Is.True);
                    Assert.That(tree.All(new Func<int, bool>(always)), Is.True);
                });
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.All(new Func<int, bool>(even)), Is.False);
                    Assert.That(tree.All(new Func<int, bool>(always)), Is.True);
                });
                tree.Clear();
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i * 2);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.All(new Func<int, bool>(even)), Is.True);
                    Assert.That(tree.All(new Func<int, bool>(always)), Is.True);
                });
                tree.Clear();
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i * 2 + 1);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.All(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.All(new Func<int, bool>(even)), Is.False);
                    Assert.That(tree.All(new Func<int, bool>(always)), Is.True);
                });
            }


            [Test]
            public void Exists()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(even)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(always)), Is.False);
                });
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(even)), Is.True);
                    Assert.That(tree.Exists(new Func<int, bool>(always)), Is.True);
                });
                tree.Clear();
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i * 2);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(even)), Is.True);
                    Assert.That(tree.Exists(new Func<int, bool>(always)), Is.True);
                });
                tree.Clear();
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i * 2 + 1);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Exists(new Func<int, bool>(never)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(even)), Is.False);
                    Assert.That(tree.Exists(new Func<int, bool>(always)), Is.True);
                });
            }


            [Test]
            public void FindAll()
            {
                Assert.That(tree.FindAll(new Func<int, bool>(never)), Is.Empty);
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.FindAll(new Func<int, bool>(never)), Is.Empty);
                    Assert.That(tree.FindAll(new Func<int, bool>(always)), Has.Count.EqualTo(10));
                    Assert.That(tree.FindAll(new Func<int, bool>(even)), Has.Count.EqualTo(5));
                    Assert.That(((TreeSet<int>)tree.FindAll(new Func<int, bool>(even))).Check("R"), Is.True);
                });
            }


            [Test]
            public void Map()
            {
                Assert.That(tree.Map(new Func<int, string>(themap), StringComparer.InvariantCulture), Is.Empty);
                for (int i = 0; i < 11; i++)
                {
                    tree.Add(i * i * i);
                }

                IIndexedSorted<string> res = tree.Map(new Func<int, string>(themap), StringComparer.InvariantCulture);

                Assert.Multiple(() =>
                {
                    Assert.That(((TreeSet<string>)res).Check("R"), Is.True);
                    Assert.That(res, Has.Count.EqualTo(11));
                });
                Assert.Multiple(() =>
                {
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
                    tree.Add(i * i * i);
                }

                var exception = Assert.Throws<ArgumentException>(() =>
                {
                    ISorted<string> res = tree.Map(new Func<int, string>(badmap), StringComparer.InvariantCulture);
                });
                Assert.That(exception.Message, Is.EqualTo("mapper not monotonic"));
            }


            [Test]
            public void Cut()
            {
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Cut(new CubeRoot(27), out int low, out bool lval, out int high, out bool hval), Is.True);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(2));
                    Assert.That(tree.Cut(new CubeRoot(30), out low, out lval, out high, out hval), Is.False);
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
                    tree.Add(2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Cut(new IntegerComparer(3), out int low, out bool lval, out int high, out bool hval), Is.False);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(2));
                    Assert.That(tree.Cut(new IntegerComparer(6), out low, out lval, out high, out hval), Is.True);
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
                    tree.Add(2 * i);
                }

                Assert.That(tree.Cut(new Interval(5, 9), out int lo, out bool lv, out int hi, out bool hv), Is.True);
                Assert.That(lv && hv, Is.True);
                Assert.That(hi, Is.EqualTo(10));
                Assert.That(lo, Is.EqualTo(4));
                Assert.That(tree.Cut(new Interval(6, 10), out lo, out lv, out hi, out hv), Is.True);
                Assert.That(lv && hv, Is.True);
                Assert.That(hi, Is.EqualTo(12));
                Assert.That(lo, Is.EqualTo(4));

                for (int i = 0; i < 100; i++)
                {
                    tree.Add(2 * i);
                }

                tree.Cut(new Interval(77, 105), out lo, out lv, out hi, out hv);
                Assert.Multiple(() =>
                {
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(106));
                    Assert.That(lo, Is.EqualTo(76));
                });
                tree.Cut(new Interval(5, 7), out lo, out lv, out hi, out hv);
                Assert.Multiple(() =>
                {
                    Assert.That(lv && hv, Is.True);
                    Assert.That(hi, Is.EqualTo(8));
                    Assert.That(lo, Is.EqualTo(4));
                });
                tree.Cut(new Interval(80, 110), out lo, out lv, out hi, out hv);
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
                    tree.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Cut(new CubeRoot(1000), out int l, out bool lv, out _, out bool hv), Is.False);
                    Assert.That(lv && !hv, Is.True);
                    Assert.That(l, Is.EqualTo(9));
                    Assert.That(tree.Cut(new CubeRoot(-50), out _, out lv, out int h, out hv), Is.False);
                    Assert.That(!lv && hv, Is.True);
                    Assert.That(h, Is.EqualTo(0));
                });
            }


            [TearDown]
            public void Dispose() { ic = null; tree.Dispose(); }
        }
    }




    namespace MultiOps
    {
        [TestFixture]
        public class AddAll
        {
            private int sqr(int i) { return i * i; }

            private TreeSet<int> tree;


            [SetUp]
            public void Init() { tree = new TreeSet<int>(new IntegerComparer()); }


            [Test]
            public void EmptyEmpty()
            {
                tree.AddAll(new FuncEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Is.Empty);
                Assert.That(tree.Check(), Is.True);
            }


            [Test]
            public void SomeEmpty()
            {
                for (int i = 4; i < 9; i++)
                {
                    tree.Add(i);
                }

                tree.AddAll(new FuncEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(5));
                Assert.That(tree.Check(), Is.True);
            }


            [Test]
            public void EmptySome()
            {
                tree.AddAll(new FuncEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(4));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree[0], Is.EqualTo(0));
                    Assert.That(tree[1], Is.EqualTo(1));
                    Assert.That(tree[2], Is.EqualTo(4));
                    Assert.That(tree[3], Is.EqualTo(9));
                });
            }


            [Test]
            public void SomeSome()
            {
                for (int i = 5; i < 9; i++)
                {
                    tree.Add(i);
                }

                tree.AddAll(new FuncEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 4, 5, 6, 7, 8, 9 }));
                });
            }


            [TearDown]
            public void Dispose() { tree.Dispose(); }
        }



        [TestFixture]
        public class AddSorted
        {
            private int sqr(int i) { return i * i; }


            private int bad(int i) { return i * (5 - i); }

            private TreeSet<int> tree;


            [SetUp]
            public void Init() { tree = new TreeSet<int>(new IntegerComparer()); }


            [Test]
            public void EmptyEmpty()
            {
                tree.AddSorted(new FuncEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Is.Empty);
                Assert.That(tree.Check(), Is.True);
            }



            [Test]
            public void SomeEmpty()
            {
                for (int i = 4; i < 9; i++)
                {
                    tree.Add(i);
                }

                tree.AddSorted(new FuncEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(5));
                Assert.That(tree.Check(), Is.True);
            }



            [Test]
            public void EmptySome()
            {
                tree.AddSorted(new FuncEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(4));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree[0], Is.EqualTo(0));
                    Assert.That(tree[1], Is.EqualTo(1));
                    Assert.That(tree[2], Is.EqualTo(4));
                    Assert.That(tree[3], Is.EqualTo(9));
                });
            }



            [Test]
            public void SomeSome()
            {
                for (int i = 5; i < 9; i++)
                {
                    tree.Add(i);
                }

                tree.AddSorted(new FuncEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 4, 5, 6, 7, 8, 9 }));
                });
            }

            [Test]
            public void EmptyBad()
            {
                var exception = Assert.Throws<ArgumentException>(() => tree.AddSorted(new FuncEnumerable(9, new Func<int, int>(bad))));
                Assert.That(exception.Message, Is.EqualTo("Argument not sorted"));
            }


            [TearDown]
            public void Dispose() { tree.Dispose(); }
        }

        [TestFixture]
        public class Rest
        {
            private TreeSet<int> tree, tree2;


            [SetUp]
            public void Init()
            {
                tree = new TreeSet<int>(new IntegerComparer());
                tree2 = new TreeSet<int>(new IntegerComparer());
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                for (int i = 0; i < 10; i++)
                {
                    tree2.Add(2 * i);
                }
            }


            [Test]
            public void RemoveAll()
            {
                tree.RemoveAll(tree2.RangeFromTo(3, 7));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 2, 3, 5, 7, 8, 9 }));
                });
                tree.RemoveAll(tree2.RangeFromTo(3, 7));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 2, 3, 5, 7, 8, 9 }));
                });
                tree.RemoveAll(tree2.RangeFromTo(13, 17));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 2, 3, 5, 7, 8, 9 }));
                });
                tree.RemoveAll(tree2.RangeFromTo(3, 17));
                Assert.That(tree, Has.Count.EqualTo(7));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 2, 3, 5, 7, 9 }));
                });
                for (int i = 0; i < 10; i++)
                {
                    tree2.Add(i);
                }

                tree.RemoveAll(tree2.RangeFromTo(-1, 10));
                Assert.That(tree, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                });
            }


            [Test]
            public void RetainAll()
            {
                tree.RetainAll(tree2.RangeFromTo(3, 17));
                Assert.That(tree, Has.Count.EqualTo(3));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 4, 6, 8 }));
                });
                tree.RetainAll(tree2.RangeFromTo(1, 17));
                Assert.That(tree, Has.Count.EqualTo(3));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 4, 6, 8 }));
                });
                tree.RetainAll(tree2.RangeFromTo(3, 5));
                Assert.That(tree, Has.Count.EqualTo(1));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.EqualTo(new[] { 4 }));
                });
                tree.RetainAll(tree2.RangeFromTo(7, 17));
                Assert.That(tree, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                });
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                tree.RetainAll(tree2.RangeFromTo(5, 5));
                Assert.That(tree, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                });
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                tree.RetainAll(tree2.RangeFromTo(15, 25));
                Assert.That(tree, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                });
            }


            [Test]
            public void ContainsAll()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tree.ContainsAll(tree2), Is.False);
                    Assert.That(tree.ContainsAll(tree), Is.True);
                });
                tree2.Clear();
                Assert.That(tree.ContainsAll(tree2), Is.True);
                tree.Clear();
                Assert.That(tree.ContainsAll(tree2), Is.True);
                tree2.Add(8);
                Assert.That(tree.ContainsAll(tree2), Is.False);
            }


            [Test]
            public void RemoveInterval()
            {
                tree.RemoveInterval(3, 4);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Has.Count.EqualTo(6));
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 2, 7, 8, 9 }));
                });
                tree.RemoveInterval(2, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Has.Count.EqualTo(3));
                    Assert.That(tree, Is.EqualTo(new[] { 0, 1, 9 }));
                });
                tree.RemoveInterval(0, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                    Assert.That(tree, Is.Empty);
                });
            }


            [Test]
            public void RemoveRangeBad1()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => tree.RemoveInterval(-3, 8));
            }


            [Test]
            public void RemoveRangeBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => tree.RemoveInterval(3, -8));
            }


            [Test]
            public void RemoveRangeBad3()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => tree.RemoveInterval(3, 8));
            }


            [Test]
            public void GetRange()
            {
                SCG.IEnumerable<int> e = tree[3, 3];
                Assert.That(e, Is.EqualTo(new[] { 3, 4, 5 }));
                e = tree[3, 0];
                Assert.That(e, Is.Empty);
            }

            [Test]
            public void GetRangeBad1()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    object foo = tree[-3, 0];
                });
            }

            [Test]
            public void GetRangeBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    object foo = tree[3, -1];
                });
            }

            [Test]
            public void GetRangeBad3()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    object foo = tree[3, 8];
                });
            }

            [TearDown]
            public void Dispose() { tree.Dispose(); tree2.Dispose(); }
        }
    }




    namespace Sync
    {
        [TestFixture]
        public class SyncRoot
        {
            private TreeSet<int> tree;
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
            public void Init() { tree = new TreeSet<int>(new IntegerComparer()); }


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
            public void Dispose() { tree.Dispose(); }
        }



        //[TestFixture]
        public class ConcurrentQueries
        {
            private TreeSet<int> tree;
            private readonly int sz = 500000;


            [SetUp]
            public void Init()
            {
                tree = new TreeSet<int>(new IntegerComparer());
                for (int i = 0; i < sz; i++)
                {
                    tree.Add(i);
                }
            }

            private class A
            {
                public int count = 0;
                private readonly TreeSet<int> t;


                public A(TreeSet<int> t) { this.t = t; }


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
            public void Dispose() { tree.Dispose(); }
        }
    }




    namespace Hashing
    {
        [TestFixture]
        public class ISequenced
        {
            private TreeSet<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new TreeSet<int>(new ReverseIntegerComparer(), EqualityComparer<int>.Default);
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
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
            }



            [TestFixture]
            public class IEditableCollection
            {
                private TreeSet<int> dit, dat, dut;


                [SetUp]
                public void Init()
                {
                    dit = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                    dat = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                    dut = new TreeSet<int>(new ReverseIntegerComparer(), EqualityComparer<int>.Default);
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
                    dit.Dispose();
                    dat.Dispose();
                    dut.Dispose();
                }
            }
        }
    }
}