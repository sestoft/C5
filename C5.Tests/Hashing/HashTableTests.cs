// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;
namespace C5.Tests.hashtable.set
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            HashSet<int> factory() { return new HashSet<int>(TenEqualityComparer.Instance); }
            new Templates.Events.CollectionTester<HashSet<int>>().Test(factory);
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new HashSet<T>(); }
    }


    namespace Enumerable
    {
        [TestFixture]
        public class Multiops
        {
            private HashSet<int> list;

            private Func<int, bool> always, never, even;


            [SetUp]
            public void Init()
            {
                Debug.UseDeterministicHashing = true;
                list = new HashSet<int>();
                always = delegate { return true; };
                never = delegate { return false; };
                even = delegate (int i) { return i % 2 == 0; };
            }


            [Test]
            public void All()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(list.All(always), Is.True);
                    Assert.That(list.All(never), Is.True);
                    Assert.That(list.All(even), Is.True);
                });
                list.Add(0);
                Assert.Multiple(() =>
                {
                    Assert.That(list.All(always), Is.True);
                    Assert.That(list.All(never), Is.False);
                    Assert.That(list.All(even), Is.True);
                });
                list.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(list.All(always), Is.True);
                    Assert.That(list.All(never), Is.False);
                    Assert.That(list.All(even), Is.False);
                });
            }


            [Test]
            public void Exists()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(list.Exists(always), Is.False);
                    Assert.That(list.Exists(never), Is.False);
                    Assert.That(list.Exists(even), Is.False);
                });
                list.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Exists(always), Is.True);
                    Assert.That(list.Exists(never), Is.False);
                    Assert.That(list.Exists(even), Is.False);
                });
                list.Add(8);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Exists(always), Is.True);
                    Assert.That(list.Exists(never), Is.False);
                    Assert.That(list.Exists(even), Is.True);
                });
            }


            [Test]
            public void Apply()
            {
                int sum = 0;
                void a(int i) { sum = i + 10 * sum; }

                list.Apply(a);
                Assert.That(sum, Is.EqualTo(0));
                sum = 0;
                list.Add(5); list.Add(8); list.Add(7); list.Add(5);
                list.Apply(a);
                Assert.That(sum, Is.EqualTo(758));
            }


            [TearDown]
            public void Dispose()
            {
                Debug.UseDeterministicHashing = false;
                list = null;
            }
        }



        [TestFixture]
        public class GetEnumerator
        {
            private HashSet<int> hashset;


            [SetUp]
            public void Init() { hashset = new HashSet<int>(); }


            [Test]
            public void Empty()
            {
                SCG.IEnumerator<int> e = hashset.GetEnumerator();

                Assert.That(e.MoveNext(), Is.False);
            }


            [Test]
            public void Normal()
            {
                hashset.Add(5);
                hashset.Add(8);
                hashset.Add(5);
                hashset.Add(5);
                hashset.Add(10);
                hashset.Add(1);
                hashset.Add(16);
                hashset.Add(18);
                hashset.Add(17);
                hashset.Add(33);
                Assert.That(IC.SetEq(hashset, 1, 5, 8, 10, 16, 17, 18, 33), Is.True);
            }

            [Test]
            public void DoDispose()
            {
                hashset.Add(5);
                hashset.Add(8);
                hashset.Add(5);

                SCG.IEnumerator<int> e = hashset.GetEnumerator();

                e.MoveNext();
                e.MoveNext();
                e.Dispose();
            }


            [Test]
            public void MoveNextAfterUpdate()
            {
                hashset.Add(5);
                hashset.Add(8);
                hashset.Add(5);

                SCG.IEnumerator<int> e = hashset.GetEnumerator();

                e.MoveNext();
                hashset.Add(99);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }

            [TearDown]
            public void Dispose() { hashset = null; }
        }
    }

    namespace CollectionOrSink
    {
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
                coll = null;
                rad16 = null;
            }

            [Test]
            public void Format()
            {
                Assert.That(coll.ToString(), Is.EqualTo("{  }"));
                coll.AddAll([-4, 28, 129, 65530]);
                Assert.Multiple(() =>
                {
                    Assert.That(coll.ToString(), Is.EqualTo("{ 65530, -4, 28, 129 }"));
                    Assert.That(coll.ToString(null, rad16), Is.EqualTo("{ FFFA, -4, 1C, 81 }"));
                    Assert.That(coll.ToString("L14", null), Is.EqualTo("{ 65530, -4, ... }"));
                    Assert.That(coll.ToString("L14", rad16), Is.EqualTo("{ FFFA, -4, ... }"));
                });
            }
        }

        [TestFixture]
        public class CollectionOrSink
        {
            private HashSet<int> hashset;


            [SetUp]
            public void Init() { hashset = new HashSet<int>(); }

            [Test]
            public void Choose()
            {
                hashset.Add(7);
                Assert.That(hashset.Choose(), Is.EqualTo(7));
            }

            [Test]
            public void BadChoose()
            {
                Assert.Throws<NoSuchItemException>(() => hashset.Choose());
            }

            [Test]
            public void CountEtAl()
            {
                Assert.That(hashset, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.IsEmpty, Is.True);
                    Assert.That(hashset.AllowsDuplicates, Is.False);
                    Assert.That(hashset.Add(0), Is.True);
                    Assert.That(hashset, Has.Count.EqualTo(1));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.IsEmpty, Is.False);
                    Assert.That(hashset.Add(5), Is.True);
                    Assert.That(hashset, Has.Count.EqualTo(2));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Add(5), Is.False);
                    Assert.That(hashset, Has.Count.EqualTo(2));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.IsEmpty, Is.False);
                    Assert.That(hashset.Add(8), Is.True);
                    Assert.That(hashset, Has.Count.EqualTo(3));
                });
            }


            [Test]
            public void AddAll()
            {
                hashset.Add(3); hashset.Add(4); hashset.Add(5);

                HashSet<int> hashset2 = new();

                hashset2.AddAll(hashset);
                Assert.That(IC.SetEq(hashset2, 3, 4, 5), Is.True);
                hashset.Add(9);
                hashset.AddAll(hashset2);
                Assert.Multiple(() =>
                {
                    Assert.That(IC.SetEq(hashset2, 3, 4, 5), Is.True);
                    Assert.That(IC.SetEq(hashset, 3, 4, 5, 9), Is.True);
                });
            }


            [TearDown]
            public void Dispose() { hashset = null; }
        }

        [TestFixture]
        public class FindPredicate
        {
            private HashSet<int> list;
            private Func<int, bool> pred;

            [SetUp]
            public void Init()
            {
                Debug.UseDeterministicHashing = true;
                list = new HashSet<int>(TenEqualityComparer.Instance);
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
            private HashSet<int> list;

            [SetUp]
            public void Init() { list = new HashSet<int>(); }

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
            private HashSet<int> hashset;
            private int[] a;


            [SetUp]
            public void Init()
            {
                Debug.UseDeterministicHashing = true;
                hashset = new HashSet<int>();
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
                hashset = null;
            }

            [Test]
            public void ToArray()
            {
                Assert.That(hashset.ToArray(), Is.Empty);
                hashset.Add(7);
                hashset.Add(3);
                hashset.Add(10);

                int[] r = hashset.ToArray();

                Array.Sort(r);
                Assert.That(r, Is.EqualTo(new[] { 3, 7, 10 }));
            }


            [Test]
            public void CopyTo()
            {
                //Note: for small ints the itemequalityComparer is the identity!
                hashset.CopyTo(a, 1);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
                hashset.Add(6);
                hashset.CopyTo(a, 2);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
                hashset.Add(4);
                hashset.Add(9);
                hashset.CopyTo(a, 4);

                //TODO: make test independent on onterequalityComparer
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 9, 4, 1007, 1008, 1009 }));
                hashset.Clear();
                hashset.Add(7);
                hashset.CopyTo(a, 9);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 9, 4, 1007, 1008, 7 }));
            }

            [Test]
            public void CopyToBad()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => hashset.CopyTo(a, 11));
            }


            [Test]
            public void CopyToBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => hashset.CopyTo(a, -1));
            }


            [Test]
            public void CopyToTooFar()
            {
                hashset.Add(3);
                hashset.Add(8);

                Assert.Throws<ArgumentOutOfRangeException>(() => hashset.CopyTo(a, 9));
            }
        }
    }

    namespace EditableCollection
    {
        [TestFixture]
        public class Collision
        {
            private HashSet<int> hashset;


            [SetUp]
            public void Init()
            {
                hashset = new HashSet<int>();
            }


            [Test]
            public void SingleCollision()
            {
                hashset.Add(7);
                hashset.Add(7 - 1503427877);

                //foreach (int cell in hashset) Console.WriteLine("A: {0}", cell);
                hashset.Remove(7);
                Assert.That(hashset.Contains(7 - 1503427877), Is.True);
            }


            [TearDown]
            public void Dispose()
            {
                hashset = null;
            }
        }



        [TestFixture]
        public class Searching
        {
            private HashSet<int> hashset;


            [SetUp]
            public void Init()
            {
                Debug.UseDeterministicHashing = true;
                hashset = new HashSet<int>();
            }


            [Test]
            public void NullEqualityComparerinConstructor1()
            {
                Assert.Throws<NullReferenceException>(() => new HashSet<int>(null));
            }

            [Test]
            public void NullEqualityComparerinConstructor2()
            {
                Assert.Throws<NullReferenceException>(() => new HashSet<int>(5, null));
            }

            [Test]
            public void NullEqualityComparerinConstructor3()
            {
                Assert.Throws<NullReferenceException>(() => new HashSet<int>(5, 0.5, null));
            }

            [Test]
            public void Contains()
            {
                Assert.That(hashset.Contains(5), Is.False);
                hashset.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Contains(5), Is.True);
                    Assert.That(hashset.Contains(7), Is.False);
                });
                hashset.Add(8);
                hashset.Add(10);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Contains(5), Is.True);
                    Assert.That(hashset.Contains(7), Is.False);
                    Assert.That(hashset.Contains(8), Is.True);
                    Assert.That(hashset.Contains(10), Is.True);
                });
                hashset.Remove(8);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Contains(5), Is.True);
                    Assert.That(hashset.Contains(7), Is.False);
                    Assert.That(hashset.Contains(8), Is.False);
                    Assert.That(hashset.Contains(10), Is.True);
                });
                hashset.Add(0); hashset.Add(16); hashset.Add(32); hashset.Add(48); hashset.Add(64);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Contains(0), Is.True);
                    Assert.That(hashset.Contains(16), Is.True);
                    Assert.That(hashset.Contains(32), Is.True);
                    Assert.That(hashset.Contains(48), Is.True);
                    Assert.That(hashset.Contains(64), Is.True);
                    Assert.That(hashset.Check(), Is.True);
                });

                int i = 0, j = i;

                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.True);
                    Assert.That(i, Is.EqualTo(j));
                });
                j = i = 16;
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.True);
                    Assert.That(i, Is.EqualTo(j));
                });
                j = i = 32;
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.True);
                    Assert.That(i, Is.EqualTo(j));
                });
                j = i = 48;
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.True);
                    Assert.That(i, Is.EqualTo(j));
                });
                j = i = 64;
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.True);
                    Assert.That(i, Is.EqualTo(j));
                });
                j = i = 80;
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Find(ref i), Is.False);
                    Assert.That(i, Is.EqualTo(j));
                });
            }


            [Test]
            public void Many()
            {
                int j = 7373;
                int[] a = new int[j];

                for (int i = 0; i < j; i++)
                {
                    hashset.Add(3 * i + 1);
                    a[i] = 3 * i + 1;
                }

                Assert.That(IC.SetEq(hashset, a), Is.True);
            }


            [Test]
            public void ContainsCount()
            {
                Assert.That(hashset.ContainsCount(5), Is.EqualTo(0));
                hashset.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(hashset.ContainsCount(7), Is.EqualTo(0));
                });
                hashset.Add(8);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(hashset.ContainsCount(7), Is.EqualTo(0));
                    Assert.That(hashset.ContainsCount(8), Is.EqualTo(1));
                });
                hashset.Add(5);
                Assert.That(hashset.ContainsCount(5), Is.EqualTo(1));
                Assert.That(hashset.ContainsCount(7), Is.EqualTo(0));
                Assert.That(hashset.ContainsCount(8), Is.EqualTo(1));
            }


            [Test]
            public void RemoveAllCopies()
            {
                hashset.Add(5); hashset.Add(7); hashset.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(hashset.ContainsCount(7), Is.EqualTo(1));
                });
                hashset.RemoveAllCopies(5);
                Assert.That(hashset.ContainsCount(5), Is.EqualTo(0));
                Assert.That(hashset.ContainsCount(7), Is.EqualTo(1));
                hashset.Add(5); hashset.Add(8); hashset.Add(5);
                hashset.RemoveAllCopies(8);
                Assert.That(IC.Eq(hashset, 7, 5), Is.True);
            }


            [Test]
            public void ContainsAll()
            {
                HashSet<int> list2 = new();

                Assert.That(hashset.ContainsAll(list2), Is.True);
                list2.Add(4);
                Assert.That(hashset.ContainsAll(list2), Is.False);
                hashset.Add(4);
                Assert.That(hashset.ContainsAll(list2), Is.True);
                hashset.Add(5);
                Assert.That(hashset.ContainsAll(list2), Is.True);
                list2.Add(20);
                Assert.That(hashset.ContainsAll(list2), Is.False);
                hashset.Add(20);
                Assert.That(hashset.ContainsAll(list2), Is.True);
            }


            [Test]
            public void RetainAll()
            {
                HashSet<int> list2 = new();

                hashset.Add(4); hashset.Add(5); hashset.Add(6);
                list2.Add(5); list2.Add(4); list2.Add(7);
                hashset.RetainAll(list2);
                Assert.That(IC.SetEq(hashset, 4, 5), Is.True);
                hashset.Add(6);
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                hashset.RetainAll(list2);
                Assert.That(IC.SetEq(hashset), Is.True);
            }

            //Bug in RetainAll reported by Chris Fesler
            //The result has different bitsc
            [Test]
            public void RetainAll2()
            {
                int LARGE_ARRAY_SIZE = 30;
                int LARGE_ARRAY_MID = 15;
                string[] _largeArrayOne = new string[LARGE_ARRAY_SIZE];
                string[] _largeArrayTwo = new string[LARGE_ARRAY_SIZE];
                for (int i = 0; i < LARGE_ARRAY_SIZE; i++)
                {
                    _largeArrayOne[i] = "" + i;
                    _largeArrayTwo[i] = "" + (i + LARGE_ARRAY_MID);
                }

                HashSet<string> setOne = new();
                setOne.AddAll(_largeArrayOne);

                HashSet<string> setTwo = new();
                setTwo.AddAll(_largeArrayTwo);

                setOne.RetainAll(setTwo);

                Assert.That(setOne.Check(), Is.True, "setOne check fails");

                for (int i = LARGE_ARRAY_MID; i < LARGE_ARRAY_SIZE; i++)
                {
                    Assert.That(setOne.Contains(_largeArrayOne[i]), Is.True, "missing " + i);
                }
            }

            [Test]
            public void RemoveAll()
            {
                HashSet<int> list2 = new();

                hashset.Add(4); hashset.Add(5); hashset.Add(6);
                list2.Add(5); list2.Add(7); list2.Add(4);
                hashset.RemoveAll(list2);
                Assert.That(IC.Eq(hashset, 6), Is.True);
                hashset.Add(5); hashset.Add(4);
                list2.Clear();
                list2.Add(6); list2.Add(5);
                hashset.RemoveAll(list2);
                Assert.That(IC.Eq(hashset, 4), Is.True);
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                hashset.RemoveAll(list2);
                Assert.That(IC.Eq(hashset, 4), Is.True);
            }


            [Test]
            public void Remove()
            {
                hashset.Add(4); hashset.Add(4); hashset.Add(5); hashset.Add(4); hashset.Add(6);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Remove(2), Is.False);
                    Assert.That(hashset.Remove(4), Is.True);
                    Assert.That(IC.SetEq(hashset, 5, 6), Is.True);
                });
                hashset.Add(7);
                hashset.Add(21); hashset.Add(37); hashset.Add(53); hashset.Add(69); hashset.Add(85);
                Assert.Multiple(() =>
                {
                    Assert.That(hashset.Remove(5), Is.True);
                    Assert.That(IC.SetEq(hashset, 6, 7, 21, 37, 53, 69, 85), Is.True);
                    Assert.That(hashset.Remove(165), Is.False);
                });
                Assert.That(IC.SetEq(hashset, 6, 7, 21, 37, 53, 69, 85), Is.True);
                Assert.That(hashset.Remove(53), Is.True);
                Assert.That(IC.SetEq(hashset, 6, 7, 21, 37, 69, 85), Is.True);
                Assert.That(hashset.Remove(37), Is.True);
                Assert.That(IC.SetEq(hashset, 6, 7, 21, 69, 85), Is.True);
                Assert.That(hashset.Remove(85), Is.True);
                Assert.That(IC.SetEq(hashset, 6, 7, 21, 69), Is.True);
            }


            [Test]
            public void Clear()
            {
                hashset.Add(7); hashset.Add(7);
                hashset.Clear();
                Assert.That(hashset.IsEmpty, Is.True);
            }


            [TearDown]
            public void Dispose()
            {
                Debug.UseDeterministicHashing = false;
                hashset = null;
            }
        }

        [TestFixture]
        public class Combined
        {
            private ICollection<SCG.KeyValuePair<int, int>> lst;


            [SetUp]
            public void Init()
            {
                lst = new HashSet<SCG.KeyValuePair<int, int>>(new KeyValuePairEqualityComparer<int, int>());
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
                var p = new SCG.KeyValuePair<int, int>(3, 78);
                var q = new SCG.KeyValuePair<int, int>();

                Assert.Multiple(() =>
                {
                    Assert.That(lst.FindOrAdd(ref p), Is.True);
                    Assert.That(p.Key, Is.EqualTo(3));
                    Assert.That(p.Value, Is.EqualTo(33));
                });
                p = new SCG.KeyValuePair<int, int>(13, 79);
                Assert.That(lst.FindOrAdd(ref p), Is.False);
                q = new SCG.KeyValuePair<int, int>(13, q.Value);
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
                var p = new SCG.KeyValuePair<int, int>(3, 78);
                var q = new SCG.KeyValuePair<int, int>();

                Assert.That(lst.Update(p), Is.True);
                q = new SCG.KeyValuePair<int, int>(3, q.Value);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Find(ref q), Is.True);
                    Assert.That(q.Key, Is.EqualTo(3));
                    Assert.That(q.Value, Is.EqualTo(78));
                });
                p = new SCG.KeyValuePair<int, int>(13, 78);
                Assert.That(lst.Update(p), Is.False);
            }


            [Test]
            public void UpdateOrAdd1()
            {
                var p = new SCG.KeyValuePair<int, int>(3, 78);
                var q = new SCG.KeyValuePair<int, int>();

                Assert.That(lst.UpdateOrAdd(p), Is.True);
                q = new SCG.KeyValuePair<int, int>(3, q.Value);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Find(ref q), Is.True);
                    Assert.That(q.Key, Is.EqualTo(3));
                    Assert.That(q.Value, Is.EqualTo(78));
                });
                p = new SCG.KeyValuePair<int, int>(13, 79);
                Assert.That(lst.UpdateOrAdd(p), Is.False);
                q = new SCG.KeyValuePair<int, int>(13, q.Value);
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
                ICollection<string> coll = new HashSet<string>();
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
                    //System.Collections.Generic.KeyValuePair<int,int> q = new System.Collections.Generic.KeyValuePair<int,int>();

                    Assert.That(lst.Remove(p, out p), Is.True);
                    Assert.That(p.Key, Is.EqualTo(3));
                    Assert.That(p.Value, Is.EqualTo(33));
                });
                p = new SCG.KeyValuePair<int, int>(13, 78);
                Assert.That(lst.Remove(p, out _), Is.False);
            }
        }


    }




    namespace HashingAndEquals
    {
        [TestFixture]
        public class IEditableCollection
        {
            private ICollection<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new HashSet<int>();
                dat = new HashSet<int>();
                dut = new HashSet<int>();
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
            public void EqualHashButDifferent()
            {
                dit.Add(-1657792980); dit.Add(-1570288808);
                dat.Add(1862883298); dat.Add(-272461342);
                Assert.Multiple(() =>
                {
                    Assert.That(dat.GetUnsequencedHashCode(), Is.EqualTo(dit.GetUnsequencedHashCode()));
                    Assert.That(dit.UnsequencedEquals(dat), Is.False);
                });
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



        [TestFixture]
        public class MultiLevelUnorderedOfUnOrdered
        {
            private ICollection<int> dit, dat, dut;

            private ICollection<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = new HashSet<int>();
                dat = new HashSet<int>();
                dut = new HashSet<int>();
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                Dit = new HashSet<ICollection<int>>();
                Dat = new HashSet<ICollection<int>>();
                Dut = new HashSet<ICollection<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dat), Is.True);
                    Assert.That(dit.UnsequencedEquals(dut), Is.False);
                });
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Assert.Multiple(() =>
                {
                    Assert.That(Dit.UnsequencedEquals(Dat), Is.True);
                    Assert.That(Dit.UnsequencedEquals(Dut), Is.False);
                });
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = null;
                Dit = Dat = Dut = null;
            }
        }



        [TestFixture]
        public class MultiLevelOrderedOfUnOrdered
        {
            private HashSet<int> dit, dat, dut;

            private LinkedList<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = new HashSet<int>();
                dat = new HashSet<int>();
                dut = new HashSet<int>();
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                Dit = new LinkedList<ICollection<int>>();
                Dat = new LinkedList<ICollection<int>>();
                Dut = new LinkedList<ICollection<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(dit.UnsequencedEquals(dat), Is.True);
                    Assert.That(dit.UnsequencedEquals(dut), Is.False);
                });
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dit); Dut.Add(dut); Dut.Add(dat);
                Assert.Multiple(() =>
                {
                    Assert.That(Dit.SequencedEquals(Dat), Is.False);
                    Assert.That(Dit.SequencedEquals(Dut), Is.True);
                });
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = null;
                Dit.Dispose();
                Dat.Dispose();
                Dut.Dispose();
            }
        }

        [TestFixture]
        public class MultiLevelUnOrderedOfOrdered
        {
            private LinkedList<int> dit, dat, dut, dot;

            private HashSet<ISequenced<int>> Dit, Dat, Dut, Dot;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
                dot = new LinkedList<int>();
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                dot.Add(2); dot.Add(1);
                Dit = new HashSet<ISequenced<int>>();
                Dat = new HashSet<ISequenced<int>>();
                Dut = new HashSet<ISequenced<int>>();
                Dot = new HashSet<ISequenced<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(dit.SequencedEquals(dat), Is.False);
                    Assert.That(dit.SequencedEquals(dot), Is.True);
                    Assert.That(dit.SequencedEquals(dut), Is.False);
                });
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut);//Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut);//Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.Multiple(() =>
                {
                    Assert.That(Dit.UnsequencedEquals(Dit), Is.True);
                    Assert.That(Dit.UnsequencedEquals(Dut), Is.True);
                    Assert.That(Dit.UnsequencedEquals(Dat), Is.False);
                    Assert.That(Dit.UnsequencedEquals(Dot), Is.True);
                });
            }


            [TearDown]
            public void Dispose()
            {
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
                dot.Dispose();
                Dit = Dat = Dut = Dot = null;
            }
        }
    }
}