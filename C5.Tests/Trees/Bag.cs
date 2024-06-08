// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;


namespace C5.Tests.trees.TreeBag
{
    [TestFixture]
    public class NewTest
    {
        // Repro for bug20091113:
        [Test]
        public void A()
        {
            var list = new TreeBag<long>
            {
                // Sequence generated in FindNodeRandomTest
                // Manually pruned by sestoft 2009-11-14
                553284,
                155203,
                316201,
                263469,
                263469
            };

            //list.dump();   // OK
            list.Remove(316201);
            //list.dump();   // Not OK
            Assert.That(list.Check(), Is.True);
        }
        [Test]
        public void B()
        {
            var l = 100;
            for (int r = 0; r < l; r++)
            {
                var list = new TreeBag<int>();
                for (int i = 0; i < l; i++)
                {
                    list.Add(l - i);
                    list.Add(l - i);
                    list.Add(l - i);
                }
                list.Remove(r);
                list.Remove(r);
                //list.dump();
                list.Remove(r);
                Assert.That(list.Check("removing" + r), Is.True);
            }
        }
    }

    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            TreeBag<int> factory() { return new TreeBag<int>(TenEqualityComparer.Default); }
            new C5.Tests.Templates.Events.SortedIndexedTester<TreeBag<int>>().Test(factory);
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new TreeBag<T>(); }
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
            Assert.That(coll.ToString(), Is.EqualTo("{{  }}"));
            coll.AddAll([-4, 28, 129, 65530, -4, 28]);
            Assert.Multiple(() =>
            {
                Assert.That(coll.ToString(), Is.EqualTo("{{ -4(*2), 28(*2), 129(*1), 65530(*1) }}"));
                Assert.That(coll.ToString(null, rad16), Is.EqualTo("{{ -4(*2), 1C(*2), 81(*1), FFFA(*1) }}"));
                Assert.That(coll.ToString("L18", null), Is.EqualTo("{{ -4(*2), 28(*2)... }}"));
                Assert.That(coll.ToString("L18", rad16), Is.EqualTo("{{ -4(*2), 1C(*2)... }}"));
            });
        }
    }

    [TestFixture]
    public class Combined
    {
        private TreeBag<SCG.KeyValuePair<int, int>> lst;


        [SetUp]
        public void Init()
        {
            lst = new TreeBag<SCG.KeyValuePair<int, int>>(new KeyValuePairComparer<int, int>(new IC()));
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
                Assert.That(lst[11].Key, Is.EqualTo(13));
                Assert.That(lst[11].Value, Is.EqualTo(79));
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
            var coll = new TreeBag<string>();
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
    public class Simple
    {
        private TreeBag<string> bag;


        [SetUp]
        public void Init()
        {
            bag = new TreeBag<string>(StringComparer.InvariantCulture);
        }


        [TearDown]
        public void Dispose()
        {
            bag.Dispose();
        }

        [Test]
        public void Initial()
        {
            Assert.Multiple(() =>
            {
                Assert.That(bag.IsReadOnly, Is.False);
                Assert.That(bag, Is.Empty, "new bag should be empty");
            });
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(0));
                Assert.That(bag.Contains("A"), Is.False);
                Assert.That(bag.Contains("B"), Is.False);
                Assert.That(bag.Contains("C"), Is.False);
            });
            bag.Add("A");
            Assert.That(bag, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(0));
                Assert.That(bag.Contains("A"), Is.True);
                Assert.That(bag.Contains("B"), Is.False);
                Assert.That(bag.Contains("C"), Is.False);
            });
            bag.Add("C");
            Assert.That(bag, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(1));
                Assert.That(bag.Contains("A"), Is.True);
                Assert.That(bag.Contains("B"), Is.False);
                Assert.That(bag.Contains("C"), Is.True);
            });
            bag.Add("C");
            Assert.That(bag, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(2));
                Assert.That(bag.Contains("A"), Is.True);
                Assert.That(bag.Contains("B"), Is.False);
                Assert.That(bag.Contains("C"), Is.True);
            });
            bag.Add("B");
            bag.Add("C");
            Assert.That(bag, Has.Count.EqualTo(5));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(3));
                Assert.That(bag.Contains("A"), Is.True);
                Assert.That(bag.Contains("B"), Is.True);
                Assert.That(bag.Contains("C"), Is.True);
            });
            _ = bag.Remove("C");
            Assert.That(bag, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(2));
                Assert.That(bag.Contains("A"), Is.True);
                Assert.That(bag.Contains("B"), Is.True);
                Assert.That(bag.Contains("C"), Is.True);
            });
            _ = bag.Remove("A");
            Assert.That(bag, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(2));
                Assert.That(bag.Contains("A"), Is.False);
                Assert.That(bag.Contains("B"), Is.True);
                Assert.That(bag.Contains("C"), Is.True);
            });
            bag.RemoveAllCopies("C");
            Assert.That(bag, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount("A"), Is.EqualTo(0));
                Assert.That(bag.ContainsCount("B"), Is.EqualTo(1));
                Assert.That(bag.ContainsCount("C"), Is.EqualTo(0));
                Assert.That(bag.Contains("A"), Is.False);
                Assert.That(bag.Contains("B"), Is.True);
                Assert.That(bag.Contains("C"), Is.False);
                Assert.That(bag.Contains("Z"), Is.False);
                Assert.That(bag.Remove("Z"), Is.False);
            });
            bag.RemoveAllCopies("Z");
            Assert.That(bag, Has.Count.EqualTo(1));
            Assert.That(bag.ContainsCount("A"), Is.EqualTo(0));
            Assert.That(bag.ContainsCount("B"), Is.EqualTo(1));
            Assert.That(bag.ContainsCount("C"), Is.EqualTo(0));
            Assert.That(bag.Contains("A"), Is.False);
            Assert.That(bag.Contains("B"), Is.True);
            Assert.That(bag.Contains("C"), Is.False);
            Assert.That(bag.Contains("Z"), Is.False);
        }
    }

    [TestFixture]
    public class FindOrAdd
    {
        private TreeBag<SCG.KeyValuePair<int, string>> bag;


        [SetUp]
        public void Init()
        {
            bag = new TreeBag<SCG.KeyValuePair<int, string>>(new KeyValuePairComparer<int, string>(new IC()));
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
            Assert.Multiple(() =>
            {
                Assert.That(bag.FindOrAdd(ref p), Is.True);
                Assert.That(p.Value, Is.EqualTo("tre"));
            });
            p = new SCG.KeyValuePair<int, string>(p.Key, "three");
            Assert.Multiple(() =>
            {
                Assert.That(bag.ContainsCount(p), Is.EqualTo(2));
                Assert.That(bag[0].Value, Is.EqualTo("tre"));
            });
        }
    }


    [TestFixture]
    public class Enumerators
    {
        private TreeBag<string> bag;

        private SCG.IEnumerator<string> bagenum;


        [SetUp]
        public void Init()
        {
            bag = new TreeBag<string>(StringComparer.InvariantCulture);
            foreach (string s in new string[] { "A", "B", "A", "A", "B", "C", "D", "B" })
            {
                bag.Add(s);
            }

            bagenum = bag.GetEnumerator();
        }


        [TearDown]
        public void Dispose()
        {
            bagenum.Dispose();
            bag.Dispose();
        }


        [Test]
        public void MoveNextOnModified()
        {
            //TODO: also problem before first MoveNext!!!!!!!!!!
            bagenum.MoveNext();
            bag.Add("T");
            Assert.Throws<CollectionModifiedException>(() => bagenum.MoveNext());
        }


        [Test]
        public void NormalUse()
        {
            Assert.Multiple(() =>
            {
                Assert.That(bagenum.MoveNext(), Is.True);
                Assert.That(bagenum.Current, Is.EqualTo("A"));
            });
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("A"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("A"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("B"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("B"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("B"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("C"));
            Assert.That(bagenum.MoveNext(), Is.True);
            Assert.That(bagenum.Current, Is.EqualTo("D"));
            Assert.That(bagenum.MoveNext(), Is.False);
        }
    }

    [TestFixture]
    public class Ranges
    {
        private TreeBag<int> tree;

        private SCG.IComparer<int> c;


        [SetUp]
        public void Init()
        {
            c = new IC();
            tree = new TreeBag<int>(c);
            for (int i = 1; i <= 10; i++)
            {
                tree.Add(i * 2); tree.Add(i);
            }
        }


        [Test]
        public void Enumerator()
        {
            SCG.IEnumerator<int> e = tree.RangeFromTo(5, 17).GetEnumerator();
            int i = 0;
            int[] all = [5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16];
            while (e.MoveNext())
            {
                Assert.That(e.Current, Is.EqualTo(all[i++]));
            }

            Assert.That(i, Is.EqualTo(12));
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
            int[] all = [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18, 20];

            tree.RemoveRangeFrom(18);
            Assert.That(IC.Eq(tree, [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16]), Is.True);
            tree.RemoveRangeFrom(28);
            Assert.That(IC.Eq(tree, [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16]), Is.True);
            tree.RemoveRangeFrom(1);
            Assert.That(IC.Eq(tree), Is.True);
            foreach (int i in all)
            {
                tree.Add(i);
            }

            tree.RemoveRangeTo(10);
            Assert.That(IC.Eq(tree, [10, 10, 12, 14, 16, 18, 20]), Is.True);
            tree.RemoveRangeTo(2);
            Assert.That(IC.Eq(tree, [10, 10, 12, 14, 16, 18, 20]), Is.True);
            tree.RemoveRangeTo(21);
            Assert.That(IC.Eq(tree), Is.True);
            foreach (int i in all)
            {
                tree.Add(i);
            }

            tree.RemoveRangeFromTo(4, 8);
            Assert.That(IC.Eq(tree, 1, 2, 2, 3, 8, 8, 9, 10, 10, 12, 14, 16, 18, 20), Is.True);
            tree.RemoveRangeFromTo(14, 28);
            Assert.That(IC.Eq(tree, 1, 2, 2, 3, 8, 8, 9, 10, 10, 12), Is.True);
            tree.RemoveRangeFromTo(0, 9);
            Assert.That(IC.Eq(tree, 9, 10, 10, 12), Is.True);
            tree.RemoveRangeFromTo(0, 81);
            Assert.That(IC.Eq(tree), Is.True);
        }


        [Test]
        public void Normal()
        {
            int[] all = [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18, 20];

            Assert.Multiple(() =>
            {
                Assert.That(IC.Eq(tree, all), Is.True);
                Assert.That(IC.Eq(tree.RangeAll(), all), Is.True);
                Assert.That(tree.RangeAll(), Has.Count.EqualTo(20));
                Assert.That(IC.Eq(tree.RangeFrom(11), [12, 14, 16, 18, 20]), Is.True);
                Assert.That(tree.RangeFrom(11), Has.Count.EqualTo(5));
                Assert.That(IC.Eq(tree.RangeFrom(12), [12, 14, 16, 18, 20]), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(1), all), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(0), all), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(21), []), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(20), [20]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(8), [1, 2, 2, 3, 4, 4, 5, 6, 6, 7]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(7), [1, 2, 2, 3, 4, 4, 5, 6, 6]), Is.True);
                Assert.That(tree.RangeTo(7), Has.Count.EqualTo(9));
                Assert.That(IC.Eq(tree.RangeTo(1), []), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(0), []), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(3), [1, 2, 2]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(20), [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(21), all), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(7, 12), [7, 8, 8, 9, 10, 10]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 11), [6, 6, 7, 8, 8, 9, 10, 10]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(1, 12), [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10]), Is.True);
                Assert.That(tree.RangeFromTo(1, 12), Has.Count.EqualTo(15));
                Assert.That(IC.Eq(tree.RangeFromTo(2, 12), [2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 21), [6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18, 20]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 20), [6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18]), Is.True);
            });
        }


        [Test]
        public void Backwards()
        {
            int[] all = [1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 8, 8, 9, 10, 10, 12, 14, 16, 18, 20];
            int[] lla = [20, 18, 16, 14, 12, 10, 10, 9, 8, 8, 7, 6, 6, 5, 4, 4, 3, 2, 2, 1];

            Assert.Multiple(() =>
            {
                Assert.That(IC.Eq(tree, all), Is.True);
                Assert.That(IC.Eq(tree.RangeAll().Backwards(), lla), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(11).Backwards(), [20, 18, 16, 14, 12]), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(12).Backwards(), [20, 18, 16, 14, 12]), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(1).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(0).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(21).Backwards(), []), Is.True);
                Assert.That(IC.Eq(tree.RangeFrom(20).Backwards(), [20]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(8).Backwards(), [7, 6, 6, 5, 4, 4, 3, 2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(7).Backwards(), [6, 6, 5, 4, 4, 3, 2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(1).Backwards(), []), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(0).Backwards(), []), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(3).Backwards(), [2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(20).Backwards(), [18, 16, 14, 12, 10, 10, 9, 8, 8, 7, 6, 6, 5, 4, 4, 3, 2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeTo(21).Backwards(), lla), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(7, 12).Backwards(), [10, 10, 9, 8, 8, 7]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 11).Backwards(), [10, 10, 9, 8, 8, 7, 6, 6]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(0, 12).Backwards(), [10, 10, 9, 8, 8, 7, 6, 6, 5, 4, 4, 3, 2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(1, 12).Backwards(), [10, 10, 9, 8, 8, 7, 6, 6, 5, 4, 4, 3, 2, 2, 1]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 21).Backwards(), [20, 18, 16, 14, 12, 10, 10, 9, 8, 8, 7, 6, 6]), Is.True);
                Assert.That(IC.Eq(tree.RangeFromTo(6, 20).Backwards(), [18, 16, 14, 12, 10, 10, 9, 8, 8, 7, 6, 6]), Is.True);
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
        private TreeBag<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 5);
            }
        }


        [Test]
        public void Both()
        {
            Assert.Multiple(() =>
            {
                Assert.That(tree.ContainsCount(7), Is.EqualTo(0));
                Assert.That(tree.ContainsCount(10), Is.EqualTo(1));
                Assert.That(tree.ContainsCount(17), Is.EqualTo(2));
            });
            tree.RemoveAllCopies(17);
            Assert.That(tree.ContainsCount(17), Is.EqualTo(0));
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
        private TreeBag<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
        }


        private void loadup()
        {
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 5);
            }
        }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new TreeBag<int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor3()
        {
            Assert.Throws<NullReferenceException>(() => new TreeBag<int>(null, EqualityComparer<int>.Default));
        }

        [Test]
        public void NullEqualityComparerinConstructor4()
        {
            Assert.Throws<NullReferenceException>(() => new TreeBag<int>(SCG.Comparer<int>.Default, null));
        }

        [Test]
        public void NullEqualityComparerinConstructor5()
        {
            Assert.Throws<NullReferenceException>(() => new TreeBag<int>(null, null));
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
            Assert.That(tree.AllowsDuplicates, Is.True);
            loadup();
            Assert.That(tree.AllowsDuplicates, Is.True);
        }


        [Test]
        public void Add()
        {
            Assert.That(tree.Add(17), Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Add(17), Is.True);
                Assert.That(tree.Add(18), Is.True);
            });
            Assert.That(tree.Add(18), Is.True);
            Assert.That(tree, Has.Count.EqualTo(4));
            Assert.That(IC.Eq(tree, 17, 17, 18, 18), Is.True);
        }


        [TearDown]
        public void Dispose()
        {
            tree.Dispose();
        }
    }

    [TestFixture]
    public class FindPredicate
    {
        private TreeBag<int> list;
        private Func<int, bool> pred;

        [SetUp]
        public void Init()
        {
            list = new TreeBag<int>(TenEqualityComparer.Default);
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
        private TreeBag<int> list;

        [SetUp]
        public void Init() { list = []; }

        [TearDown]
        public void Dispose() { list.Dispose(); }

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
    public class UniqueItemCount
    {
        private TreeBag<int> list;

        [SetUp]
        public void Init() { list = []; }

        [TearDown]
        public void Dispose() { list.Dispose(); }

        [Test]
        public void Test1()
        {
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
            list.AddAll([7, 9, 7]);
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.Remove(7);
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.Remove(7);
            Assert.That(list.UniqueCount, Is.EqualTo(1));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test2()
        {
            list.AddSorted([7, 8, 9, 10, 10]);
            Assert.That(list.UniqueCount, Is.EqualTo(4));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test3()
        {
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
            list.Add(7);
            list.Add(9);
            list.Add(7);
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.Remove(7);
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.Remove(7);
            Assert.That(list.UniqueCount, Is.EqualTo(1));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test_FindOrAdd()
        {
            list.AddSorted([7, 8, 9, 10, 10]);
            int tmp = 7;
            list.FindOrAdd(ref tmp);
            Assert.That(list.UniqueCount, Is.EqualTo(4));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            tmp = 11;
            list.FindOrAdd(ref tmp);
            Assert.That(list.UniqueCount, Is.EqualTo(5));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test_RemoveAllCopies()
        {
            list.AddSorted([7, 8, 8, 9, 10, 10]);
            list.RemoveAllCopies(10);
            Assert.That(list.UniqueCount, Is.EqualTo(3));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test_DeleteMinMax()
        {
            list.AddAll([7, 9, 9, 7]);
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.DeleteMin();
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.DeleteMax();
            Assert.That(list.UniqueCount, Is.EqualTo(2));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            list.DeleteMax();
            Assert.That(list.UniqueCount, Is.EqualTo(1));
            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));
        }

        [Test]
        public void Test_ByIndex()
        {
            int i;
            list = new TreeBag<int>(new IC());
            for (i = 10; i < 20; i++)
            {
                list.Add(i);
                list.Add(i + 5);
            }
            //10,11,12,13,14,15,15,16,16,17,17,18,18,19,19,20,21,22,23,24

            //Remove root!
            int n = list.Count;
            i = list[10];

            Assert.Multiple(() =>
            {
                Assert.That(list.RemoveAt(10), Is.EqualTo(17));
                Assert.That(list.Check(""), Is.True);
                Assert.That(list.Contains(i), Is.True);
                Assert.That(list, Has.Count.EqualTo(n - 1));
            });
            Assert.That(list.RemoveAt(9), Is.EqualTo(17));
            Assert.That(list.Check(""), Is.True);
            Assert.That(list.Contains(i), Is.False);
            Assert.That(list, Has.Count.EqualTo(n - 2));

            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            //Low end
            i = list.FindMin();
            list.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(list.Check(""), Is.True);
                Assert.That(list.Contains(i), Is.False);
                Assert.That(list, Has.Count.EqualTo(n - 3));
            });

            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            //high end
            i = list.FindMax();
            list.RemoveAt(list.Count - 1);
            Assert.Multiple(() =>
            {
                Assert.That(list.Check(""), Is.True);
                Assert.That(list.Contains(i), Is.False);
                Assert.That(list, Has.Count.EqualTo(n - 4));
            });

            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

            //Some leaf
            //list.dump();
            i = 18;
            Assert.Multiple(() =>
            {
                Assert.That(list.RemoveAt(9), Is.EqualTo(i));
                Assert.That(list.Check(""), Is.True);
                Assert.That(list.RemoveAt(8), Is.EqualTo(i));
            });
            Assert.That(list.Check(""), Is.True);
            Assert.That(list.Contains(i), Is.False);
            Assert.That(list, Has.Count.EqualTo(n - 6));

            Assert.That(list.UniqueCount, Is.EqualTo(list.UniqueItems().Count));
            Assert.That(list.UniqueCount, Is.EqualTo(list.ItemMultiplicities().Count));

        }
    }


    [TestFixture]
    public class ArrayTest
    {
        private TreeBag<int> tree;
        private int[] a;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
            a = new int[10];
            for (int i = 0; i < 10; i++)
            {
                a[i] = 1000 + i;
            }
        }


        [TearDown]
        public void Dispose() { tree.Dispose(); }


        private static string Aeq(int[] a, params int[] b)
        {
            if (a.Length != b.Length)
            {
                return "Lengths differ: " + a.Length + " != " + b.Length;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return $"{i}'th elements differ: {a[i]} != {b[i]}";
                }
            }

            return "Alles klar";
        }


        [Test]
        public void ToArray()
        {
            Assert.That(Aeq(tree.ToArray()), Is.EqualTo("Alles klar"));
            tree.Add(4);
            tree.Add(7);
            tree.Add(4);
            Assert.That(Aeq(tree.ToArray(), 4, 4, 7), Is.EqualTo("Alles klar"));
        }


        [Test]
        public void CopyTo()
        {
            tree.CopyTo(a, 1);
            Assert.That(Aeq(a, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009), Is.EqualTo("Alles klar"));
            tree.Add(6);
            tree.Add(6);
            tree.CopyTo(a, 2);
            Assert.That(Aeq(a, 1000, 1001, 6, 6, 1004, 1005, 1006, 1007, 1008, 1009), Is.EqualTo("Alles klar"));
            tree.Add(4);
            tree.Add(9);
            tree.CopyTo(a, 4);
            Assert.That(Aeq(a, 1000, 1001, 6, 6, 4, 6, 6, 9, 1008, 1009), Is.EqualTo("Alles klar"));
            tree.Clear();
            tree.Add(7);
            tree.CopyTo(a, 9);
            Assert.That(Aeq(a, 1000, 1001, 6, 6, 4, 6, 6, 9, 1008, 7), Is.EqualTo("Alles klar"));
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
        private TreeBag<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
            for (int i = 10; i < 20; i++)
            {
                tree.Add(i);
                tree.Add(i + 5);
            }
            //10,11,12,13,14,15,15,16,16,17,17,18,18,19,19,20,21,22,23,24
        }


        [Test]
        public void SmallTrees()
        {
            tree.Clear();
            tree.Add(9);
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

            Assert.Multiple(() =>
            {
                Assert.That(tree.RemoveAt(10), Is.EqualTo(17));
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.True);
                Assert.That(tree, Has.Count.EqualTo(n - 1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(tree.RemoveAt(9), Is.EqualTo(17));
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 2));
            });

            //Low end
            i = tree.FindMin();
            tree.RemoveAt(0);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 3));
            });

            //high end
            i = tree.FindMax();
            tree.RemoveAt(tree.Count - 1);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.Contains(i), Is.False);
                Assert.That(tree, Has.Count.EqualTo(n - 4));
            });

            //Some leaf
            //tree.dump();
            i = 18;
            Assert.Multiple(() =>
            {
                Assert.That(tree.RemoveAt(9), Is.EqualTo(i));
                Assert.That(tree.Check(""), Is.True);
                Assert.That(tree.RemoveAt(8), Is.EqualTo(i));
            });
            Assert.That(tree.Check(""), Is.True);
            Assert.That(tree.Contains(i), Is.False);
            Assert.That(tree, Has.Count.EqualTo(n - 6));
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
                //Note: ids does not match for bag
                Assert.That(tree.Remove(-20), Is.False);

                //1b
                Assert.That(tree.Remove(20), Is.True);
                Assert.That(tree.Check("T1"), Is.True);
            });
            Assert.That(tree.Remove(20), Is.False);

            //1b
            Assert.That(tree.Remove(10), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //case 1c
            Assert.That(tree.Remove(24), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //1a (terminating)
            Assert.That(tree.Remove(16), Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(16), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

                //2
                Assert.That(tree.Remove(18), Is.True);
                Assert.That(tree.Remove(17), Is.True);
            });
            Assert.That(tree.Remove(18), Is.True);
            Assert.That(tree.Remove(17), Is.True);
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //2+1b
            Assert.That(tree.Remove(15), Is.True);
            Assert.That(tree.Remove(15), Is.True);
            for (int i = 0; i < 5; i++)
            {
                tree.Add(17 + i);
            }

            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(23), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

                //1a+1b
                Assert.That(tree.Remove(11), Is.True);
            });
            Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");

            //2+1c
            for (int i = 0; i < 10; i++)
            {
                tree.Add(50 - 2 * i);
            }

            Assert.Multiple(() =>
            {
                Assert.That(tree.Remove(42), Is.True);
                Assert.That(tree.Remove(38), Is.True);
                Assert.That(tree.Remove(22), Is.True);
                Assert.That(tree.Remove(40), Is.True);
            });

            //
            for (int i = 0; i < 48; i++)
            {
                tree.Remove(i);
            }

            Assert.Multiple(() =>
            {
                //Almost empty tree:*
                Assert.That(tree.Remove(26), Is.False);
                Assert.That(tree.Remove(48), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
            });

            //Empty tree:*
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
        private TreeBag<int> tree;

        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
        }

        private void loadup()
        {
            for (int i = 0; i < 20; i++)
            {
                tree.Add(2 * i);
            }

            for (int i = 0; i < 10; i++)
            {
                tree.Add(4 * i);
            }
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
        private TreeBag<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
        }

        private void loadup()
        {
            foreach (int i in new int[] { 1, 2, 3, 4 })
            {
                tree.Add(i);
            }
            tree.Add(1);
            tree.Add(3);
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
            Assert.That(tree.FindMin(), Is.EqualTo(1));
            Assert.That(tree.FindMax(), Is.EqualTo(3));
            Assert.That(tree.DeleteMin(), Is.EqualTo(1));
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
        private TreeBag<int> tree;


        [SetUp]
        public void Init()
        {
            tree = new TreeBag<int>(new IC());
        }


        private void populate()
        {
            tree.Add(30);
            tree.Add(30);
            tree.Add(50);
            tree.Add(10);
            tree.Add(70);
            tree.Add(70);
        }


        [Test]
        public void ToArray()
        {
            populate();

            int[] a = tree.ToArray();

            Assert.That(a.Length, Is.EqualTo(6));
            Assert.Multiple(() =>
            {
                Assert.That(a[0], Is.EqualTo(10));
                Assert.That(a[1], Is.EqualTo(30));
                Assert.That(a[2], Is.EqualTo(30));
                Assert.That(a[3], Is.EqualTo(50));
                Assert.That(a[4], Is.EqualTo(70));
                Assert.That(a[5], Is.EqualTo(70));
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
                Assert.That(tree[2], Is.EqualTo(30));
                Assert.That(tree[3], Is.EqualTo(50));
                Assert.That(tree[4], Is.EqualTo(70));
                Assert.That(tree[5], Is.EqualTo(70));
                Assert.That(tree.IndexOf(10), Is.EqualTo(0));
                Assert.That(tree.IndexOf(30), Is.EqualTo(1));
                Assert.That(tree.IndexOf(50), Is.EqualTo(3));
                Assert.That(tree.IndexOf(70), Is.EqualTo(4));
                Assert.That(tree.IndexOf(20), Is.EqualTo(~1));
                Assert.That(tree.IndexOf(0), Is.EqualTo(~0));
                Assert.That(tree.IndexOf(90), Is.EqualTo(~6));
                Assert.That(tree.LastIndexOf(10), Is.EqualTo(0));
                Assert.That(tree.LastIndexOf(30), Is.EqualTo(2));
                Assert.That(tree.LastIndexOf(50), Is.EqualTo(3));
                Assert.That(tree.LastIndexOf(70), Is.EqualTo(5));
                Assert.That(tree.LastIndexOf(20), Is.EqualTo(~1));
                Assert.That(tree.LastIndexOf(0), Is.EqualTo(~0));
                Assert.That(tree.LastIndexOf(90), Is.EqualTo(~6));
            });
        }


        [Test]
        public void IndexTooLarge()
        {
            populate();
            Assert.Throws<IndexOutOfRangeException>(() => Console.WriteLine(tree[6]));
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
                Assert.That(tree.CountTo(90), Is.EqualTo(6));
                Assert.That(tree.CountFromTo(-20, 90), Is.EqualTo(6));
                Assert.That(tree.CountFrom(0), Is.EqualTo(6));
            });
        }


        [Test]
        public void FilledTreeIntermediateInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(20), Is.EqualTo(5));
                Assert.That(tree.CountFromTo(20, 40), Is.EqualTo(2));
                Assert.That(tree.CountTo(40), Is.EqualTo(3));
            });
        }


        [Test]
        public void FilledTreeMatchingInput()
        {
            populate();
            Assert.Multiple(() =>
            {
                Assert.That(tree.CountFrom(30), Is.EqualTo(5));
                Assert.That(tree.CountFromTo(30, 70), Is.EqualTo(3));
                Assert.That(tree.CountFromTo(50, 30), Is.EqualTo(0));
                Assert.That(tree.CountFromTo(50, 50), Is.EqualTo(0));
                Assert.That(tree.CountTo(10), Is.EqualTo(0));
                Assert.That(tree.CountTo(50), Is.EqualTo(3));
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
            private TreeBag<int> tree;

            private SCG.IEnumerator<int> e;


            [SetUp]
            public void Init()
            {
                tree = new TreeBag<int>(new IC());
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                tree.Add(3);
                tree.Add(7);
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
            private TreeBag<int> tree;

            private SCG.IEnumerator<int> e;


            [SetUp]
            public void Init()
            {
                tree = new TreeBag<int>(new IC());
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
            private TreeBag<int> tree, snap;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IC();
                tree = new TreeBag<int>(ic);
                for (int i = 0; i <= 20; i++)
                {
                    tree.Add(2 * i + 1);
                }

                tree.Add(13);
                snap = (TreeBag<int>)tree.Snapshot();
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
                Assert.That(IC.Eq(snap.FindAll(new Func<int, bool>(twomodeleven)), 13, 13, 35), Is.True);
            }


            public void MoreCut()
            {
                //TODO: Assert.Fail("more tests of Cut needed");
            }


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
                    Assert.That(IC.Eq(snap.RangeFromTo(5, 16), 5, 7, 9, 11, 13, 13, 15), Is.True);
                    Assert.That(IC.Eq(snap.RangeFromTo(5, 17), 5, 7, 9, 11, 13, 13, 15), Is.True);
                    Assert.That(IC.Eq(snap.RangeFromTo(6, 16), 7, 9, 11, 13, 13, 15), Is.True);
                });
            }


            [Test]
            public void Contains()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Contains(5), Is.True);
                    Assert.That(snap.Contains(13), Is.True);
                    Assert.That(snap.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(snap.ContainsCount(13), Is.EqualTo(2));
                });
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
            private TreeBag<int> tree;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IC();
                tree = new TreeBag<int>(ic);
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(2 * i + 1);
                }
            }


            [Test]
            public void Bug20120422_1()
            {
                var coll = new C5.TreeBag<string>() { "C" };
                _ = coll.Snapshot();
                coll.Add("C");
                Assert.That(coll.ContainsCount("C"), Is.EqualTo(2));
            }

            [Test]
            public void Bug20120422_2()
            {
                var coll = new C5.TreeBag<string>
                {
                    "B",
                    "A",
                    "C"
                };
                _ = coll.Snapshot();
                coll.Add("C");
                Assert.Multiple(() =>
                {
                    Assert.That(coll.ContainsCount("C"), Is.EqualTo(2));
                    Assert.That(coll.Check(), Is.True);
                });
                coll.Add("A");
                Assert.Multiple(() =>
                {
                    Assert.That(coll.ContainsCount("A"), Is.EqualTo(2));
                    Assert.That(coll.ContainsCount("C"), Is.EqualTo(2));
                    Assert.That(coll.Check(), Is.True);
                });
                coll.Add("B");
                Assert.Multiple(() =>
                {
                    Assert.That(coll.Check(), Is.True);
                    Assert.That(coll.ContainsCount("B"), Is.EqualTo(2));
                    Assert.That(coll.ContainsCount("A"), Is.EqualTo(2));
                    Assert.That(coll.ContainsCount("C"), Is.EqualTo(2));
                });
                _ = coll.Snapshot();
                coll.Add("C");
                Assert.That(coll.ContainsCount("C"), Is.EqualTo(3));
                Assert.That(coll.Check(), Is.True);
                coll.Add("A");
                Assert.Multiple(() =>
                {
                    Assert.That(coll.ContainsCount("A"), Is.EqualTo(3));
                    Assert.That(coll.ContainsCount("C"), Is.EqualTo(3));
                    Assert.That(coll.Check(), Is.True);
                });
                coll.Add("B");
                Assert.Multiple(() =>
                {
                    Assert.That(coll.Check(), Is.True);
                    Assert.That(coll.ContainsCount("B"), Is.EqualTo(3));
                    Assert.That(coll.ContainsCount("A"), Is.EqualTo(3));
                    Assert.That(coll.ContainsCount("C"), Is.EqualTo(3));
                });
                coll.RemoveAt(8);
                Assert.That(coll.ContainsCount("C"), Is.EqualTo(2));
            }

            [Test]
            public void EnumerationWithAdd()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                int i = 0;
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

                foreach (int j in snap)
                {
                    Assert.That(j, Is.EqualTo(1 + 2 * i++));
                    tree.Add(21 - j);
                    Assert.Multiple(() =>
                    {
                        Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                        Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                        Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    });
                }
            }


            [Test]
            public void Remove()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                });
                tree.Remove(19);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
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
                tree.Add(15);

                int[] orig = [10, 11, 12, 13, 14, 15, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29];
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(-20), Is.False);
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");


                    //decrease items case
                    Assert.That(tree.Remove(15), Is.True);
                    Assert.That(tree.Check("T1"), Is.True);
                });
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                //snap.dump();
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                //No demote case, with move_item
                Assert.That(tree.Remove(20), Is.True);
                Assert.That(tree.Check("T1"), Is.True);
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                Assert.That(tree.Remove(20), Is.False);

                //plain case 2
                tree.Snapshot();
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(14), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                    //case 1b
                    Assert.That(tree.Remove(25), Is.True);
                });
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                //case 1c
                Assert.That(tree.Remove(29), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                //1a (terminating)
                Assert.That(tree.Remove(10), Is.True);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                //2+1b
                Assert.That(tree.Remove(12), Is.True);
                tree.Snapshot();
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Remove(11), Is.True);
                    Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

                    //1a+1b
                    Assert.That(tree.Remove(18), Is.True);
                    Assert.That(tree.Remove(13), Is.True);
                    Assert.That(tree.Remove(15), Is.True);
                });
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

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
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");

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
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                });

                //Empty tree:
                Assert.That(tree.Remove(26), Is.False);
                Assert.That(tree.Check("Normal test 1"), Is.True, "Bad tree");
                Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
            }


            [Test]
            public void Add()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
                tree.Add(10);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
                tree.Add(16);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });

                tree.Add(9);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                });

                //Promote+zigzig
                tree.Add(40);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                });
                for (int i = 1; i < 4; i++)
                {
                    tree.Add(40 - 2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });

                //Zigzag:
                tree.Add(32);
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("M"), Is.True, "Bad snap!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                });
            }


            [Test]
            public void Clear()
            {
                int[] orig = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19];
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

                tree.Clear();
                Assert.Multiple(() =>
                {
                    Assert.That(snap.Check("Snap"), Is.True, "Bad snap!");
                    Assert.That(tree.Check("Tree"), Is.True, "Bad tree!");
                    Assert.That(IC.Eq(snap, orig), Is.True, "Snap was changed!");
                    Assert.That(tree, Is.Empty);
                });
            }


            [Test]
            public void SnapSnap()
            {
                TreeBag<int> snap = (TreeBag<int>)tree.Snapshot();

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
            private TreeBag<int> tree;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

            private SCG.IComparer<int> ic;

            [SetUp]
            public void Init()
            {
                ic = new IC();
                tree = new TreeBag<int>(ic);
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(2 * i + 1);
                }
            }


            [Test]
            public void First()
            {
                TreeBag<int>[] snaps = new TreeBag<int>[10];

                for (int i = 0; i < 10; i++)
                {
                    snaps[i] = (TreeBag<int>)(tree.Snapshot());
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
                    Assert.That(IC.Eq(snaps[3], snap3), Is.True, "Snap 3 was changed!");
                    Assert.That(IC.Eq(snaps[7], snap7), Is.True, "Snap 7 was changed!");
                    Assert.That(IC.Eq(tree, res), Is.True);
                    Assert.That(tree.Check("B"), Is.True);
                    Assert.That(snaps[3].Check("B"), Is.True);
                    Assert.That(snaps[7].Check("B"), Is.True);
                });
            }


            [Test]
            public void CollectingTheMaster()
            {
                TreeBag<int>[] snaps = new TreeBag<int>[10];

                for (int i = 0; i < 10; i++)
                {
                    snaps[i] = (TreeBag<int>)(tree.Snapshot());
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
                    Assert.That(IC.Eq(snaps[3], snap3), Is.True, "Snap 3 was changed!");
                    Assert.That(IC.Eq(snaps[7], snap7), Is.True, "Snap 7 was changed!");
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
            private TreeBag<int> tree;

            private SCG.IComparer<int> ic;


            [SetUp]
            public void Init()
            {
                ic = new IC();
                tree = new TreeBag<int>(ic);
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

                tree.Add(2);

                tree.Apply(new Action<int>(simple2.apply));
                Assert.Multiple(() =>
                {
                    Assert.That(simple2.appfield1, Is.EqualTo(11));
                    Assert.That(simple2.appfield2, Is.EqualTo(289));
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

                tree.Add(2);

                Assert.Multiple(() =>
                {
                    Assert.That(tree.FindAll(new Func<int, bool>(never)), Is.Empty);
                    Assert.That(tree.FindAll(new Func<int, bool>(always)), Has.Count.EqualTo(11));
                    Assert.That(tree.FindAll(new Func<int, bool>(even)), Has.Count.EqualTo(6));
                    Assert.That(((TreeBag<int>)tree.FindAll(new Func<int, bool>(even))).Check("R"), Is.True);
                });
            }


            [Test]
            public void Map()
            {
                Assert.That(tree.Map(new Func<int, string>(themap), StringComparer.InvariantCulture), Is.Empty);
                for (int i = 0; i < 14; i++)
                {
                    tree.Add(i * i * i);
                }

                tree.Add(1);

                IIndexedSorted<string> res = tree.Map(new Func<int, string>(themap), StringComparer.InvariantCulture);

                Assert.Multiple(() =>
                {
                    Assert.That(((TreeBag<string>)res).Check("R"), Is.True);
                    Assert.That(res, Has.Count.EqualTo(15));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(res[0], Is.EqualTo("AA    0 BB"));
                    Assert.That(res[1], Is.EqualTo("AA    1 BB"));
                    Assert.That(res[2], Is.EqualTo("AA    1 BB"));
                    Assert.That(res[3], Is.EqualTo("AA    8 BB"));
                    Assert.That(res[4], Is.EqualTo("AA   27 BB"));
                    Assert.That(res[6], Is.EqualTo("AA  125 BB"));
                    Assert.That(res[11], Is.EqualTo("AA 1000 BB"));
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

                tree.Add(3);

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
                    Assert.That(tree.Cut(new IC(3), out int low, out bool lval, out int high, out bool hval), Is.False);
                    Assert.That(lval && hval, Is.True);
                    Assert.That(high, Is.EqualTo(4));
                    Assert.That(low, Is.EqualTo(2));
                    Assert.That(tree.Cut(new IC(6), out low, out lval, out high, out hval), Is.True);
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

            private TreeBag<int> tree;


            [SetUp]
            public void Init() { tree = new TreeBag<int>(new IC()); }


            [Test]
            public void EmptyEmpty()
            {
                tree.AddAll(new FunEnumerable(0, new Func<int, int>(sqr)));
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

                tree.AddAll(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(5));
                Assert.That(tree.Check(), Is.True);
            }


            [Test]
            public void EmptySome()
            {
                tree.AddAll(new FunEnumerable(4, new Func<int, int>(sqr)));
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

                tree.Add(1);

                tree.AddAll(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(9));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 1, 4, 5, 6, 7, 8, 9), Is.True);
                });
            }


            [TearDown]
            public void Dispose() { tree.Dispose(); }
        }



        [TestFixture]
        public class AddSorted
        {
            private int sqr(int i) { return i * i; }

            private int step(int i) { return i / 3; }


            private int bad(int i) { return i * (5 - i); }

            private TreeBag<int> tree;


            [SetUp]
            public void Init() { tree = new TreeBag<int>(new IC()); }


            [Test]
            public void EmptyEmpty()
            {
                tree.AddSorted(new FunEnumerable(0, new Func<int, int>(sqr)));
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

                tree.AddSorted(new FunEnumerable(0, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(5));
                Assert.That(tree.Check(), Is.True);
            }


            [Test]
            public void EmptySome()
            {
                tree.AddSorted(new FunEnumerable(4, new Func<int, int>(sqr)));
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
            public void EmptySome2()
            {
                tree.AddSorted(new FunEnumerable(4, new Func<int, int>(step)));
                //tree.dump();
                Assert.That(tree, Has.Count.EqualTo(4));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree[0], Is.EqualTo(0));
                    Assert.That(tree[1], Is.EqualTo(0));
                    Assert.That(tree[2], Is.EqualTo(0));
                    Assert.That(tree[3], Is.EqualTo(1));
                });
            }


            [Test]
            public void SomeSome()
            {
                for (int i = 5; i < 9; i++)
                {
                    tree.Add(i);
                }

                tree.Add(1);

                tree.AddSorted(new FunEnumerable(4, new Func<int, int>(sqr)));
                Assert.That(tree, Has.Count.EqualTo(9));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 1, 4, 5, 6, 7, 8, 9), Is.True);
                });
            }


            [Test]
            public void EmptyBad()
            {
                var exception = Assert.Throws<ArgumentException>(() => tree.AddSorted(new FunEnumerable(9, new Func<int, int>(bad))));
                Assert.That(exception.Message, Is.EqualTo("Argument not sorted"));
            }


            [TearDown]
            public void Dispose() { tree.Dispose(); }
        }



        [TestFixture]
        public class Rest
        {
            private TreeBag<int> tree, tree2;


            [SetUp]
            public void Init()
            {
                tree = new TreeBag<int>(new IC());
                tree2 = new TreeBag<int>(new IC());
                for (int i = 0; i < 10; i++)
                {
                    tree.Add(i);
                }

                tree.Add(4);

                for (int i = 0; i < 10; i++)
                {
                    tree2.Add(2 * i);
                }
            }


            [Test]
            public void RemoveAll()
            {
                tree.RemoveAll(tree2.RangeFromTo(3, 7));
                Assert.That(tree, Has.Count.EqualTo(9));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 2, 3, 4, 5, 7, 8, 9), Is.True);
                });
                tree.RemoveAll(tree2.RangeFromTo(3, 7));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 2, 3, 5, 7, 8, 9), Is.True);
                });
                tree.RemoveAll(tree2.RangeFromTo(13, 17));
                Assert.That(tree, Has.Count.EqualTo(8));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 2, 3, 5, 7, 8, 9), Is.True);
                });
                tree.RemoveAll(tree2.RangeFromTo(3, 17));
                Assert.That(tree, Has.Count.EqualTo(7));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 0, 1, 2, 3, 5, 7, 9), Is.True);
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
                    Assert.That(IC.Eq(tree), Is.True);
                });
            }

            [Test]
            public void RetainAll()
            {
                tree.Add(8); tree2.Add(6);
                tree.RetainAll(tree2.RangeFromTo(3, 17));
                Assert.That(tree, Has.Count.EqualTo(3));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 4, 6, 8), Is.True);
                });
                tree.RetainAll(tree2.RangeFromTo(1, 17));
                Assert.That(tree, Has.Count.EqualTo(3));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 4, 6, 8), Is.True);
                });
                tree.RetainAll(tree2.RangeFromTo(3, 5));
                Assert.That(tree, Has.Count.EqualTo(1));
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree, 4), Is.True);
                });
                tree.RetainAll(tree2.RangeFromTo(7, 17));
                Assert.That(tree, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(IC.Eq(tree), Is.True);
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
                    Assert.That(IC.Eq(tree), Is.True);
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
                    Assert.That(IC.Eq(tree), Is.True);
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
                    Assert.That(tree, Has.Count.EqualTo(7));
                    Assert.That(IC.Eq(tree, 0, 1, 2, 6, 7, 8, 9), Is.True);
                });
                tree.RemoveInterval(2, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Has.Count.EqualTo(4));
                    Assert.That(IC.Eq(tree, 0, 1, 8, 9), Is.True);
                });
                tree.RemoveInterval(0, 4);
                Assert.Multiple(() =>
                {
                    Assert.That(tree.Check(), Is.True);
                    Assert.That(tree, Is.Empty);
                    Assert.That(IC.Eq(tree), Is.True);
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
                Assert.Throws<ArgumentOutOfRangeException>(() => tree.RemoveInterval(3, 9));
            }


            [Test]
            public void GetRange()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(IC.Eq(tree[3, 0]), Is.True);
                    Assert.That(IC.Eq(tree[3, 1], 3), Is.True);
                    Assert.That(IC.Eq(tree[3, 2], 3, 4), Is.True);
                    Assert.That(IC.Eq(tree[3, 3], 3, 4, 4), Is.True);
                    Assert.That(IC.Eq(tree[3, 4], 3, 4, 4, 5), Is.True);
                    Assert.That(IC.Eq(tree[4, 0]), Is.True);
                    Assert.That(IC.Eq(tree[4, 1], 4), Is.True);
                    Assert.That(IC.Eq(tree[4, 2], 4, 4), Is.True);
                    Assert.That(IC.Eq(tree[4, 3], 4, 4, 5), Is.True);
                    Assert.That(IC.Eq(tree[4, 4], 4, 4, 5, 6), Is.True);
                    Assert.That(IC.Eq(tree[5, 0]), Is.True);
                    Assert.That(IC.Eq(tree[5, 1], 4), Is.True);
                    Assert.That(IC.Eq(tree[5, 2], 4, 5), Is.True);
                    Assert.That(IC.Eq(tree[5, 3], 4, 5, 6), Is.True);
                    Assert.That(IC.Eq(tree[5, 4], 4, 5, 6, 7), Is.True);
                    Assert.That(IC.Eq(tree[5, 6], 4, 5, 6, 7, 8, 9), Is.True);
                });
            }

            [Test]
            public void GetRangeBug20090616()
            {
                TreeBag<double> tree = [
          0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 3.0, 3.0, 4.0 ];
                for (int start = 0; start <= tree.Count - 2; start++)
                {
                    double[] range = tree[start, 2].ToArray();
                    Assert.Multiple(() =>
                    {
                        Assert.That(tree[start], Is.EqualTo(range[0]));
                        Assert.That(tree[start + 1], Is.EqualTo(range[1]));
                    });
                }
            }

            [Test]
            public void GetRangeBackwards()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(IC.Eq(tree[3, 0].Backwards()), Is.True);
                    Assert.That(IC.Eq(tree[3, 1].Backwards(), 3), Is.True);
                    Assert.That(IC.Eq(tree[3, 2].Backwards(), 4, 3), Is.True);
                    Assert.That(IC.Eq(tree[3, 3].Backwards(), 4, 4, 3), Is.True);
                    Assert.That(IC.Eq(tree[3, 4].Backwards(), 5, 4, 4, 3), Is.True);
                    Assert.That(IC.Eq(tree[4, 0].Backwards()), Is.True);
                    Assert.That(IC.Eq(tree[4, 1].Backwards(), 4), Is.True);
                    Assert.That(IC.Eq(tree[4, 2].Backwards(), 4, 4), Is.True);
                    Assert.That(IC.Eq(tree[4, 3].Backwards(), 5, 4, 4), Is.True);
                    Assert.That(IC.Eq(tree[4, 4].Backwards(), 6, 5, 4, 4), Is.True);
                    Assert.That(IC.Eq(tree[5, 0].Backwards()), Is.True);
                    Assert.That(IC.Eq(tree[5, 1].Backwards(), 4), Is.True);
                    Assert.That(IC.Eq(tree[5, 2].Backwards(), 5, 4), Is.True);
                    Assert.That(IC.Eq(tree[5, 3].Backwards(), 6, 5, 4), Is.True);
                    Assert.That(IC.Eq(tree[5, 4].Backwards(), 7, 6, 5, 4), Is.True);
                });
            }

            [Test]
            public void GetRangeBackwardsBug20090616()
            {
                TreeBag<double> tree = [
          0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 3.0, 3.0, 4.0 ];
                for (int start = 0; start <= tree.Count - 2; start++)
                {
                    double[] range = tree[start, 2].Backwards().ToArray();
                    Assert.Multiple(() =>
                    {
                        Assert.That(tree[start], Is.EqualTo(range[1]));
                        Assert.That(tree[start + 1], Is.EqualTo(range[0]));
                    });
                }
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
                    object foo = tree[3, 9];
                });
            }


            [TearDown]
            public void Dispose() { tree.Dispose(); tree2.Dispose(); }
        }
    }


    namespace Hashing
    {
        [TestFixture]
        public class ISequenced
        {
            private TreeBag<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new TreeBag<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new TreeBag<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new TreeBag<int>(new RevIC(), EqualityComparer<int>.Default);
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
        }



        [TestFixture]
        public class IEditableCollection
        {
            private TreeBag<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new TreeBag<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new TreeBag<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new TreeBag<int>(new RevIC(), EqualityComparer<int>.Default);
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
