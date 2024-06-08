// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests.arrays.hashed
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            HashedArrayList<int> factory() { return new HashedArrayList<int>(TenEqualityComparer.Instance); }
            new Templates.Events.ListTester<HashedArrayList<int>>().Test(factory);
        }

        [Test]
        public void List()
        {
            Templates.List.Dispose.Tester<HashedArrayList<int>>();
            Templates.List.SCG_IList.Tester<HashedArrayList<int>>();
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new HashedArrayList<T>(); }
    }

    namespace Events
    {
        internal class TenEqualityComparer : SCG.IEqualityComparer<int>
        {
            private TenEqualityComparer() { }
            public static TenEqualityComparer Default => new();
            public int GetHashCode(int item) { return (item / 10).GetHashCode(); }
            public bool Equals(int item1, int item2) { return item1 / 10 == item2 / 10; }
        }

        [TestFixture]
        public class IList_
        {
            private HashedArrayList<int> list;
            private CollectionEventList<int> seen;

            [SetUp]
            public void Init()
            {
                list = new HashedArrayList<int>(TenEqualityComparer.Default);
                seen = new CollectionEventList<int>(SCG.EqualityComparer<int>.Default);
            }

            private void listen() { seen.Listen(list, EventType.All); }

            [Test]
            public void SetThis()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list[1] = 45;
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), list),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
            }

            [Test]
            public void Insert()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Insert(1, 45);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
            }

            [Test]
            public void InsertAll()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.InsertAll(1, [666, 777, 888]);
                //seen.Print(Console.Error);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(666,1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(666, 1), list),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(777,2), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(777, 1), list),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(888,3), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(888, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
                list.InsertAll(1, []);
                seen.Check([]);
            }

            [Test]
            public void InsertFirstLast()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.InsertFirst(45);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,0), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
                list.InsertLast(88);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(88,4), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(88, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
            }

            [Test]
            public void Remove()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Remove();
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void RemoveFirst()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveFirst();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(4,0), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(4, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void RemoveLast()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveLast();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void Reverse()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Reverse();
                seen.Check([
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.View(1, 0).Reverse();
                seen.Check([]);
            }


            [Test]
            public void Sort()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Sort();
                seen.Check([
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.View(1, 0).Sort();
                seen.Check([]);
            }

            [Test]
            public void Shuffle()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Shuffle();
                seen.Check([
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.View(1, 0).Shuffle();
                seen.Check([]);
            }

            [Test]
            public void RemoveAt()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveAt(1);
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void RemoveInterval()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.RemoveInterval(1, 2);
                seen.Check([
           new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,2,1), list),
         new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.RemoveInterval(1, 0);
                seen.Check([]);
            }

            [Test]
            public void Update()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Update(53);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
          ]);
                list.Update(67);
                seen.Check([]);
            }

            [Test]
            public void FindOrAdd()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                int val = 53;
                list.FindOrAdd(ref val);
                seen.Check([]);
                val = 67;
                list.FindOrAdd(ref val);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
            }

            [Test]
            public void UpdateOrAdd()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                int val = 53;
                list.UpdateOrAdd(val);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                val = 67;
                list.UpdateOrAdd(val);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.UpdateOrAdd(51, out _);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(53, 1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(51, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                // val = 67;
                list.UpdateOrAdd(81, out _);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(81, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
            }

            [Test]
            public void RemoveItem()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.Remove(53);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Remove(11);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(18, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void RemoveAll()
            {
                for (int i = 0; i < 10; i++)
                {
                    list.Add(10 * i + 5);
                }
                listen();
                list.RemoveAll([32, 187, 45]);
                //TODO: the order depends on internals of the HashSet
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(35, 1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(45, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.RemoveAll([200, 300]);
                seen.Check([]);
            }

            [Test]
            public void Clear()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.View(1, 1).Clear();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,1,1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.Clear();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,2,0), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.Clear();
                seen.Check([]);
            }

            [Test]
            public void ListDispose()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.View(1, 1).Dispose();
                seen.Check([]);
                list.Dispose();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,3,0), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)
        ]);
                list.Dispose();
                seen.Check([]);
            }


            [Test]
            public void RetainAll()
            {
                for (int i = 0; i < 10; i++)
                {
                    list.Add(10 * i + 5);
                }
                listen();
                list.RetainAll([32, 187, 45, 62, 82, 95, 2]);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(15, 1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(25, 1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(55, 1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(75, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.RetainAll([32, 187, 45, 62, 82, 95, 2]);
                seen.Check([]);
            }

            [Test]
            public void RemoveAllCopies()
            {
                for (int i = 0; i < 10; i++)
                {
                    list.Add(3 * i + 5);
                }
                listen();
                list.RemoveAllCopies(14);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(11, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.RemoveAllCopies(14);
                seen.Check([]);
            }

            [Test]
            public void Add()
            {
                listen();
                seen.Check([]);
                list.Add(23);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void AddAll()
            {
                for (int i = 0; i < 10; i++)
                {
                    list.Add(10 * i + 5);
                }
                listen();
                list.AddAll([145, 56, 167]);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(145, 1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(167, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.AddAll([]);
                seen.Check([]);
            }

            [TearDown]
            public void Dispose() { list.Dispose(); seen = null; }
        }

        [TestFixture]
        public class StackQueue
        {

            private ArrayList<int> list;
            private CollectionEventList<int> seen;

            [SetUp]
            public void Init()
            {
                list = new ArrayList<int>(TenEqualityComparer.Default);
                seen = new CollectionEventList<int>(SCG.EqualityComparer<int>.Default);
            }

            private void listen() { seen.Listen(list, EventType.All); }

            [Test]
            public void EnqueueDequeue()
            {
                listen();
                list.Enqueue(67);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(67,0), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Enqueue(2);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(2,1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(2, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Dequeue();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(67,0), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(67, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Dequeue();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(2,0), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(2, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [Test]
            public void PushPop()
            {
                listen();
                seen.Check([]);
                list.Push(23);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(23,0), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Push(-12);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(-12,1), list),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(-12, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Pop();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(-12,1), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(-12, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
                list.Pop();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(23,0), list),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(23, 1), list),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), list)]);
            }

            [TearDown]
            public void Dispose() { list.Dispose(); seen = null; }
        }


    }

    namespace Safety
    {
        /// <summary>
        /// Tests to see if the collection classes are robust for enumerable arguments that throw exceptions.
        /// </summary>
        [TestFixture]
        public class BadEnumerable
        {
            private HashedArrayList<int> list;

            [SetUp]
            public void Init()
            {
                list = [];
            }

            [Test]
            public void InsertAll()
            {
                list.Add(4); list.Add(56); list.Add(18);
                try
                {
                    list.InsertAll(1, new BadEnumerable<int>(new BadEnumerableException(), 91, 81, 71));
                    Assert.Fail("Should not get here");
                }
                catch (BadEnumerableException) { }

                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 91, 81, 71, 56, 18 }));
                });
            }

            [Test]
            public void AddAll()
            {
                list.Add(4); list.Add(56); list.Add(18);
                try
                {
                    list.View(0, 1).AddAll(new BadEnumerable<int>(new BadEnumerableException(), 91, 81, 71));
                    Assert.Fail("Should not get here");
                }
                catch (BadEnumerableException) { }

                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 91, 81, 71, 56, 18 }));
                });
            }

            [Test]
            public void RemoveAll()
            {
                list.Add(4); list.Add(56); list.Add(18);
                try
                {
                    list.RemoveAll(new BadEnumerable<int>(new BadEnumerableException(), 9, 8, 7));
                    Assert.Fail("Should not get here");
                }
                catch (BadEnumerableException) { }

                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 56, 18 }));
                });
            }

            [Test]
            public void RetainAll()
            {
                list.Add(4); list.Add(56); list.Add(18);
                try
                {
                    list.RetainAll(new BadEnumerable<int>(new BadEnumerableException(), 9, 8, 7));
                    Assert.Fail("Should not get here");
                }
                catch (BadEnumerableException) { }

                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 56, 18 }));
                });
            }


            [Test]
            public void ContainsAll()
            {
                list.Add(4); list.Add(56); list.Add(18);
                try
                {
                    list.ContainsAll(new BadEnumerable<int>(new BadEnumerableException(), 4, 18));
                    Assert.Fail("Should not get here");
                }
                catch (BadEnumerableException) { }

                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 56, 18 }));
                });
            }


            [TearDown]
            public void Dispose() { list.Dispose(); }
        }

        /// <summary>
        /// Tests to see if the collection classes are robust for delegate arguments that throw exceptions.
        /// </summary>
        [TestFixture]
        public class BadFun
        {

            [SetUp]
            public void Init()
            {
            }

            [Test]
            public void NoTests() { }

            [TearDown]
            public void Dispose() { }
        }
    }


    namespace Enumerable
    {
        [TestFixture]
        public class Multiops
        {
            private HashedArrayList<int> list;

            private Func<int, bool> always, never, even;


            [SetUp]
            public void Init()
            {
                list = [];
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
                list.Add(8);
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
                Assert.That(sum, Is.EqualTo(587));
            }


            [TearDown]
            public void Dispose() { list.Dispose(); }
        }



        [TestFixture]
        public class GetEnumerator
        {
            private HashedArrayList<int> list;


            [SetUp]
            public void Init() { list = []; }


            [Test]
            public void Empty()
            {
                SCG.IEnumerator<int> e = list.GetEnumerator();

                Assert.That(e.MoveNext(), Is.False);
            }


            [Test]
            public void Normal()
            {
                list.Add(5);
                list.Add(8);
                list.Add(5);
                list.Add(5);
                list.Add(10);
                list.Add(1);

                SCG.IEnumerator<int> e = list.GetEnumerator();

                Assert.Multiple(() =>
                {
                    Assert.That(e.MoveNext(), Is.True);
                    Assert.That(e.Current, Is.EqualTo(5));
                });
                Assert.That(e.MoveNext(), Is.True);
                Assert.That(e.Current, Is.EqualTo(8));
                Assert.That(e.MoveNext(), Is.True);
                Assert.That(e.Current, Is.EqualTo(10));
                Assert.That(e.MoveNext(), Is.True);
                Assert.That(e.Current, Is.EqualTo(1));
                Assert.That(e.MoveNext(), Is.False);
            }


            [Test]
            public void DoDispose()
            {
                list.Add(5);
                list.Add(8);
                list.Add(5);

                SCG.IEnumerator<int> e = list.GetEnumerator();

                e.MoveNext();
                e.MoveNext();
                e.Dispose();
            }


            [Test]
            public void MoveNextAfterUpdate()
            {
                list.Add(5);
                list.Add(8);
                list.Add(5);

                SCG.IEnumerator<int> e = list.GetEnumerator();

                e.MoveNext();
                list.Add(99);

                Assert.Throws<CollectionModifiedException>(() => e.MoveNext());
            }

            [TearDown]
            public void Dispose() { list.Dispose(); }
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
            public void Init() { coll = Factory.New<int>(); rad16 = new RadixFormatProvider(16); }
            [TearDown]
            public void Dispose() { coll = null; rad16 = null; }
            [Test]
            public void Format()
            {
                Assert.That(coll.ToString(), Is.EqualTo("[  ]"));
                coll.AddAll([-4, 28, 129, 65530]);
                Assert.Multiple(() =>
                {
                    Assert.That(coll.ToString(), Is.EqualTo("[ 0:-4, 1:28, 2:129, 3:65530 ]"));
                    Assert.That(coll.ToString(null, rad16), Is.EqualTo("[ 0:-4, 1:1C, 2:81, 3:FFFA ]"));
                    Assert.That(coll.ToString("L14", null), Is.EqualTo("[ 0:-4, 1:28... ]"));
                    Assert.That(coll.ToString("L14", rad16), Is.EqualTo("[ 0:-4, 1:1C... ]"));
                });
            }
        }

        [TestFixture]
        public class CollectionOrSink
        {
            private HashedArrayList<int> list;


            [SetUp]
            public void Init() { list = []; }

            [Test]
            public void Choose()
            {
                list.Add(7);
                Assert.That(list.Choose(), Is.EqualTo(7));
            }

            [Test]
            public void BadChoose()
            {
                Assert.Throws<NoSuchItemException>(() => list.Choose());
            }

            [Test]
            public void CountEtAl()
            {
                Assert.That(list, Is.Empty);
                Assert.Multiple(() =>
                {
                    Assert.That(list.IsEmpty, Is.True);
                    Assert.That(list.AllowsDuplicates, Is.False);
                    Assert.That(list.Add(5), Is.True);
                    Assert.That(list, Has.Count.EqualTo(1));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(list.IsEmpty, Is.False);
                    Assert.That(list.Add(5), Is.False);
                    Assert.That(list, Has.Count.EqualTo(1));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(list.IsEmpty, Is.False);
                    Assert.That(list.Add(8), Is.True);
                    Assert.That(list, Has.Count.EqualTo(2));
                });
            }


            [Test]
            public void AddAll()
            {
                list.Add(3); list.Add(4); list.Add(5);

                HashedArrayList<int> list2 = [];

                list2.AddAll(list);
                Assert.That(list2, Is.EqualTo(new[] { 3, 4, 5 }));
                list.AddAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(list2, Is.EqualTo(new[] { 3, 4, 5 }));
                    Assert.That(list, Is.EqualTo(new[] { 3, 4, 5 }));
                });
            }


            [TearDown]
            public void Dispose() { list.Dispose(); }
        }

        [TestFixture]
        public class FindPredicate
        {
            private HashedArrayList<int> list;
            private Func<int, bool> pred;

            [SetUp]
            public void Init()
            {
                list = new HashedArrayList<int>(TenEqualityComparer.Instance);
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
                Assert.That(list.FindIndex(pred), Is.EqualTo(4));
            }

            [Test]
            public void FindLastIndex()
            {
                Assert.That(0 <= list.FindLastIndex(pred), Is.False);
                list.AddAll([4, 22, 67, 37]);
                Assert.That(0 <= list.FindLastIndex(pred), Is.False);
                list.AddAll([45, 122, 675, 137]);
                Assert.That(list.FindLastIndex(pred), Is.EqualTo(6));
            }
        }

        [TestFixture]
        public class UniqueItems
        {
            private HashedArrayList<int> list;

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
            private HashedArrayList<int> list;
            private int[] a;


            [SetUp]
            public void Init()
            {
                list = [];
                a = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    a[i] = 1000 + i;
                }
            }


            [TearDown]
            public void Dispose() { list.Dispose(); }

            [Test]
            public void ToArray()
            {
                Assert.That(list.ToArray(), Is.Empty);
                list.Add(7);
                list.Add(8);
                Assert.That(list.ToArray(), Is.EqualTo(new[] { 7, 8 }));
            }

            [Test]
            public void CopyTo()
            {
                list.CopyTo(a, 1);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
                list.Add(6);
                list.CopyTo(a, 2);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 1004, 1005, 1006, 1007, 1008, 1009 }));
                list.Add(4);
                list.Add(5);
                list.Add(9);
                list.CopyTo(a, 4);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 4, 5, 9, 1008, 1009 }));
                list.Clear();
                list.Add(7);
                list.CopyTo(a, 9);
                Assert.That(a, Is.EqualTo(new[] { 1000, 1001, 6, 1003, 6, 4, 5, 9, 1008, 7 }));
            }

            [Test]
            public void CopyToBad()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(a, 11));
            }

            [Test]
            public void CopyToBad2()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(a, -1));
            }

            [Test]
            public void CopyToTooFar()
            {
                list.Add(3);
                list.Add(4);
                Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(a, 9));
            }
        }

        [TestFixture]
        public class Sync
        {
            [Test]
            public void HashedArrayList_has_SyncRoot()
            {
                var list = new HashedArrayList<int>();
                Assert.That(((System.Collections.IList)list).SyncRoot, Is.Not.Null);
            }
        }
    }

    namespace EditableCollection
    {
        [TestFixture]
        public class Searching
        {
            private HashedArrayList<int> list;


            [SetUp]
            public void Init() { list = []; }


            [Test]
            public void NullEqualityComparerinConstructor1()
            {
                Assert.Throws<NullReferenceException>(() => new HashedArrayList<int>(null));
            }

            [Test]
            public void NullEqualityComparerinConstructor2()
            {
                Assert.Throws<NullReferenceException>(() => new HashedArrayList<int>(5, null));
            }

            [Test]
            public void Contains()
            {
                Assert.That(list.Contains(5), Is.False);
                list.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Contains(5), Is.True);
                    Assert.That(list.Contains(7), Is.False);
                });
                list.Add(8);
                list.Add(10);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Contains(5), Is.True);
                    Assert.That(list.Contains(7), Is.False);
                    Assert.That(list.Contains(8), Is.True);
                    Assert.That(list.Contains(10), Is.True);
                });
                list.Remove(8);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Contains(5), Is.True);
                    Assert.That(list.Contains(7), Is.False);
                    Assert.That(list.Contains(8), Is.False);
                    Assert.That(list.Contains(10), Is.True);
                });
            }

            [Test]
            public void BadAdd()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(list.Add(5), Is.True);
                    Assert.That(list.Add(8), Is.True);
                });
                Assert.That(list.Add(5), Is.False);
            }


            [Test]
            public void ContainsCount()
            {
                Assert.That(list.ContainsCount(5), Is.EqualTo(0));
                list.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(list.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(list.ContainsCount(7), Is.EqualTo(0));
                });
                list.Add(8);
                Assert.Multiple(() =>
                {
                    Assert.That(list.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(list.ContainsCount(7), Is.EqualTo(0));
                    Assert.That(list.ContainsCount(8), Is.EqualTo(1));
                });
            }


            [Test]
            public void RemoveAllCopies()
            {
                list.Add(5); list.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(list.ContainsCount(5), Is.EqualTo(1));
                    Assert.That(list.ContainsCount(7), Is.EqualTo(1));
                });
                list.RemoveAllCopies(5);
                Assert.Multiple(() =>
                {
                    Assert.That(list.ContainsCount(5), Is.EqualTo(0));
                    Assert.That(list.ContainsCount(7), Is.EqualTo(1));
                });
                list.Add(5); list.Add(8);
                list.RemoveAllCopies(8);
                Assert.That(list, Is.EqualTo(new[] { 7, 5 }));
            }


            [Test]
            public void FindAll()
            {
                bool f(int i) { return i % 2 == 0; }

                Assert.That(list.FindAll(f).IsEmpty, Is.True);
                list.Add(5); list.Add(8); list.Add(10);
                Assert.Multiple(() =>
                {
                    Assert.That(((HashedArrayList<int>)list.FindAll(f)).Check(), Is.True);
                    Assert.That(list.FindAll(f), Is.EqualTo(new[] { 8, 10 }));
                });
            }


            [Test]
            public void ContainsAll()
            {
                HashedArrayList<int> list2 = [];

                Assert.That(list.ContainsAll(list2), Is.True);
                list2.Add(4);
                Assert.That(list.ContainsAll(list2), Is.False);
                list.Add(4);
                Assert.That(list.ContainsAll(list2), Is.True);
                list.Add(5);
                Assert.That(list.ContainsAll(list2), Is.True);
            }


            [Test]
            public void RetainAll()
            {
                HashedArrayList<int> list2 = [];

                list.Add(4); list.Add(5); list.Add(6);
                list2.Add(5); list2.Add(4); list2.Add(7);
                list.RetainAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4, 5 }));
                });
                list.Add(5); list.Add(4); list.Add(6);
                list2.Clear();
                list2.Add(5); list2.Add(5); list2.Add(6);
                list.RetainAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 5, 6 }));
                });
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                list.RetainAll(list2);
                Assert.That(list.Check(), Is.True);
                Assert.That(IC.Eq(list), Is.True);
            }


            [Test]
            public void RemoveAll()
            {
                HashedArrayList<int> list2 = [];

                list.Add(4); list.Add(5); list.Add(6);
                list2.Add(5); list2.Add(4); list2.Add(7);
                list.RemoveAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 6 }));
                });
                list.Add(5); list.Add(4); list.Add(6);
                list2.Clear();
                list2.Add(6); list2.Add(5);
                list.RemoveAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 4 }));
                });
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                list.RemoveAll(list2);
                Assert.That(list.Check(), Is.True);
                Assert.That(list, Is.EqualTo(new[] { 4 }));
            }


            [Test]
            public void Remove()
            {
                list.Add(4); list.Add(5); list.Add(6);
                Assert.Multiple(() =>
                {
                    Assert.That(list.Remove(2), Is.False);
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list.Remove(4), Is.True);
                });
                Assert.That(list.Check(), Is.True);
                Assert.That(list, Is.EqualTo(new[] { 5, 6 }));
                Assert.That(list.RemoveLast(), Is.EqualTo(6));
                Assert.That(list.Check(), Is.True);
                Assert.That(list, Is.EqualTo(new[] { 5 }));
                list.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(list.RemoveFirst(), Is.EqualTo(5));
                    Assert.That(list.Check(), Is.True);
                    Assert.That(list, Is.EqualTo(new[] { 7 }));
                });
            }


            [Test]
            public void Clear()
            {
                list.Add(7); list.Add(6);
                list.Clear();
                Assert.That(list.IsEmpty, Is.True);
            }


            [TearDown]
            public void Dispose() { list.Dispose(); }
        }
    }




    namespace IIndexed
    {
        [TestFixture]
        public class Searching
        {
            private HashedArrayList<int> dit;


            [SetUp]
            public void Init()
            {
                dit = [];
            }


            [Test]
            public void IndexOf()
            {
                Assert.That(dit.IndexOf(6), Is.EqualTo(~0));
                dit.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.IndexOf(6), Is.EqualTo(~1));
                    Assert.That(dit.LastIndexOf(6), Is.EqualTo(~1));
                    Assert.That(dit.IndexOf(7), Is.EqualTo(0));
                });
                dit.Add(5); dit.Add(7); dit.Add(8); dit.Add(7);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.IndexOf(6), Is.EqualTo(~3));
                    Assert.That(dit.IndexOf(7), Is.EqualTo(0));
                    Assert.That(dit.LastIndexOf(7), Is.EqualTo(0));
                    Assert.That(dit.IndexOf(8), Is.EqualTo(2));
                    Assert.That(dit.LastIndexOf(5), Is.EqualTo(1));
                });
            }

            [TearDown]
            public void Dispose()
            {
                dit.Dispose();
            }
        }

        [TestFixture]
        public class Removing
        {
            private HashedArrayList<int> dit;

            [SetUp]
            public void Init()
            {
                dit = [];
            }

            [Test]
            public void RemoveAt()
            {
                dit.Add(5); dit.Add(7); dit.Add(9); dit.Add(1); dit.Add(2);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.RemoveAt(1), Is.EqualTo(7));
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 5, 9, 1, 2 }));
                    Assert.That(dit.RemoveAt(0), Is.EqualTo(5));
                });
                Assert.That(dit.Check(), Is.True);
                Assert.That(dit, Is.EqualTo(new[] { 9, 1, 2 }));
                Assert.That(dit.RemoveAt(2), Is.EqualTo(2));
                Assert.That(dit.Check(), Is.True);
                Assert.That(dit, Is.EqualTo(new[] { 9, 1 }));
            }

            [Test]
            public void RemoveAtBad0()
            {
                Assert.Throws<IndexOutOfRangeException>(() => dit.RemoveAt(0));
            }

            [Test]
            public void RemoveAtBadM1()
            {
                Assert.Throws<IndexOutOfRangeException>(() => dit.RemoveAt(-1));
            }

            [Test]
            public void RemoveAtBad1()
            {
                dit.Add(8);
                Assert.Throws<IndexOutOfRangeException>(() => dit.RemoveAt(1));
            }

            [Test]
            public void RemoveInterval()
            {
                dit.RemoveInterval(0, 0);
                dit.Add(10); dit.Add(20); dit.Add(30); dit.Add(40); dit.Add(50); dit.Add(60);
                dit.RemoveInterval(3, 0);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 10, 20, 30, 40, 50, 60 }));
                });
                dit.RemoveInterval(3, 1);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 10, 20, 30, 50, 60 }));
                });
                dit.RemoveInterval(1, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 10, 60 }));
                });
                dit.RemoveInterval(0, 2);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(IC.Eq(dit), Is.True);
                });
                dit.Add(10); dit.Add(20); dit.Add(30); dit.Add(40); dit.Add(50); dit.Add(60);
                dit.RemoveInterval(0, 2);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 30, 40, 50, 60 }));
                });
                dit.RemoveInterval(2, 2);
                Assert.Multiple(() =>
                {
                    Assert.That(dit.Check(), Is.True);
                    Assert.That(dit, Is.EqualTo(new[] { 30, 40 }));
                });
            }

            [TearDown]
            public void Dispose()
            {
                dit.Dispose();
            }
        }
    }

    namespace IList
    {
        [TestFixture]
        public class Searching
        {
            private IList<int> lst;

            [SetUp]
            public void Init() { lst = new HashedArrayList<int>(); }


            [TearDown]
            public void Dispose() { lst.Dispose(); }

            [Test]
            public void FirstBad()
            {
                Assert.Throws<NoSuchItemException>(() =>
                {
                    int f = lst.First;
                });
            }

            [Test]
            public void LastBad()
            {
                Assert.Throws<NoSuchItemException>(() =>
                {
                    int f = lst.Last;
                });
            }

            [Test]
            public void FirstLast()
            {
                lst.Add(19);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.First, Is.EqualTo(19));
                    Assert.That(lst.Last, Is.EqualTo(19));
                });
                lst.Add(34); lst.InsertFirst(12);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.First, Is.EqualTo(12));
                    Assert.That(lst.Last, Is.EqualTo(34));
                });
            }


            [Test]
            public void This()
            {
                lst.Add(34);
                Assert.That(lst[0], Is.EqualTo(34));
                lst[0] = 56;
                Assert.That(lst.First, Is.EqualTo(56));
                lst.Add(7); lst.Add(77); lst.Add(777); lst.Add(7777);
                lst[0] = 45; lst[2] = 78; lst[4] = 101;
                Assert.That(lst, Is.EqualTo(new[] { 45, 7, 78, 777, 101 }));
            }

            [Test]
            public void ThisWithUpdates()
            {
                HashedArrayList<SCG.KeyValuePair<int, int>> pairlist = new(new KeyValuePairEqualityComparer<int, int>())
                {
                    new SCG.KeyValuePair<int, int>(10, 50),
                    new SCG.KeyValuePair<int, int>(11, 51),
                    new SCG.KeyValuePair<int, int>(12, 52),
                    new SCG.KeyValuePair<int, int>(13, 53)
                };
                pairlist[2] = new SCG.KeyValuePair<int, int>(12, 102);
                Assert.Multiple(() =>
                {
                    Assert.That(pairlist.Check(), Is.True);
                    Assert.That(pairlist[2], Is.EqualTo(new SCG.KeyValuePair<int, int>(12, 102)));
                });
                pairlist[2] = new SCG.KeyValuePair<int, int>(22, 202);
                Assert.Multiple(() =>
                {
                    Assert.That(pairlist.Check(), Is.True);
                    Assert.That(pairlist[2], Is.EqualTo(new SCG.KeyValuePair<int, int>(22, 202)));
                });
                pairlist[1] = new SCG.KeyValuePair<int, int>(12, 303);
                Assert.Multiple(() =>
                {
                    Assert.That(pairlist.Check(), Is.True);
                    Assert.That(pairlist[1], Is.EqualTo(new SCG.KeyValuePair<int, int>(12, 303)));
                    Assert.That(pairlist[2], Is.EqualTo(new SCG.KeyValuePair<int, int>(22, 202)));
                });
            }

            [Test]
            public void ThisWithUpdatesBad()
            {
                HashedArrayList<SCG.KeyValuePair<int, int>> pairlist = new(new KeyValuePairEqualityComparer<int, int>())
                {
                    new SCG.KeyValuePair<int, int>(10, 50),
                    new SCG.KeyValuePair<int, int>(11, 51),
                    new SCG.KeyValuePair<int, int>(12, 52),
                    new SCG.KeyValuePair<int, int>(13, 53)
                };

                Assert.Throws<DuplicateNotAllowedException>(() => pairlist[2] = new SCG.KeyValuePair<int, int>(11, 102));
            }

            [Test]
            public void ThisBadEmptyGet()
            {
                Assert.Throws<IndexOutOfRangeException>(() =>
                {
                    int f = lst[0];
                });
            }

            [Test]
            public void ThisBadLowGet()
            {
                lst.Add(7);
                Assert.Throws<IndexOutOfRangeException>(() =>
                {
                    int f = lst[-1];
                });
            }

            [Test]
            public void ThisBadHiGet()
            {
                lst.Add(6);

                Assert.Throws<IndexOutOfRangeException>(() =>
                {
                    int f = lst[1];
                });
            }

            [Test]
            public void ThisBadEmptySet()
            {
                Assert.Throws<IndexOutOfRangeException>(() => lst[0] = 4);
            }

            [Test]
            public void ThisBadLowSet()
            {
                lst.Add(7);
                Assert.Throws<IndexOutOfRangeException>(() => lst[-1] = 9);
            }

            [Test]
            public void ThisBadHiSet()
            {
                lst.Add(6);
                Assert.Throws<IndexOutOfRangeException>(() => lst[1] = 11);
            }
        }

        [TestFixture]
        public class Inserting
        {
            private IList<int> lst;

            [SetUp]
            public void Init() { lst = new HashedArrayList<int>(); }

            [TearDown]
            public void Dispose() { lst.Dispose(); }

            [Test]
            public void Insert()
            {
                lst.Insert(0, 5);
                Assert.That(lst, Is.EqualTo(new[] { 5 }));
                lst.Insert(0, 7);
                Assert.That(lst, Is.EqualTo(new[] { 7, 5 }));
                lst.Insert(1, 4);
                Assert.That(lst, Is.EqualTo(new[] { 7, 4, 5 }));
                lst.Insert(3, 2);
                Assert.That(lst, Is.EqualTo(new[] { 7, 4, 5, 2 }));
            }

            [Test]
            public void InsertDuplicate()
            {
                lst.Insert(0, 5);
                Assert.That(lst, Is.EqualTo(new[] { 5 }));
                lst.Insert(0, 7);
                Assert.That(lst, Is.EqualTo(new[] { 7, 5 }));

                Assert.Throws<DuplicateNotAllowedException>(() => lst.Insert(1, 5));
            }

            [Test]
            public void InsertAllDuplicate1()
            {
                lst.Insert(0, 3);
                Assert.That(lst, Is.EqualTo(new[] { 3 }));
                lst.Insert(0, 7);
                Assert.That(lst, Is.EqualTo(new[] { 7, 3 }));
                try
                {
                    lst.InsertAll(1, [1, 2, 3, 4]);
                }
                catch (DuplicateNotAllowedException)
                {
                }

                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 1, 2, 3 }));
                });
            }

            [Test]
            public void InsertAllDuplicate2()
            {
                lst.Insert(0, 3);
                Assert.That(lst, Is.EqualTo(new[] { 3 }));
                lst.Insert(0, 7);
                Assert.That(lst, Is.EqualTo(new[] { 7, 3 }));
                try
                {
                    lst.InsertAll(1, [5, 6, 5, 8]);
                }
                catch (DuplicateNotAllowedException)
                {
                }

                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 5, 6, 3 }));
                });
            }

            [Test]
            public void BadInsertLow()
            {
                lst.Add(7);

                Assert.Throws<IndexOutOfRangeException>(() => lst.Insert(-1, 9));
            }

            [Test]
            public void BadInsertHi()
            {
                lst.Add(6);
                Assert.Throws<IndexOutOfRangeException>(() => lst.Insert(2, 11));
            }

            [Test]
            public void FIFO()
            {
                for (int i = 0; i < 7; i++)
                {
                    lst.Add(2 * i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(lst.FIFO, Is.False);
                    Assert.That(lst.Remove(), Is.EqualTo(12));
                });
                Assert.That(lst.Remove(), Is.EqualTo(10));
                lst.FIFO = true;
                Assert.That(lst.Remove(), Is.EqualTo(0));
                Assert.That(lst.Remove(), Is.EqualTo(2));
                lst.FIFO = false;
                Assert.That(lst.Remove(), Is.EqualTo(8));
                Assert.That(lst.Remove(), Is.EqualTo(6));
            }

            [Test]
            public void InsertFirstLast()
            {
                lst.InsertFirst(4);
                lst.InsertLast(5);
                lst.InsertFirst(14);
                lst.InsertLast(15);
                lst.InsertFirst(24);
                lst.InsertLast(25);
                lst.InsertFirst(34);
                lst.InsertLast(55);
                Assert.That(lst, Is.EqualTo(new[] { 34, 24, 14, 4, 5, 15, 25, 55 }));
            }


            [Test]
            public void InsertFirst()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);
                lst.Add(5);
                lst.ViewOf(2).InsertFirst(7);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 2, 3, 4, 5 }));
                });
                lst.ViewOf(3).InsertFirst(8);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 2, 8, 3, 4, 5 }));
                });
                lst.ViewOf(5).InsertFirst(9);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 2, 8, 3, 4, 9, 5 }));
                });
            }

            [Test]
            public void BadFirst()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(2);
                lst.Add(5);
                Assert.That(lst.ViewOf(4), Is.Null);
            }

            [Test]
            public void InsertAfter()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);
                lst.Add(5);
                lst.LastViewOf(2).InsertLast(7);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 1, 2, 7, 3, 4, 5 }));
                });
                lst.LastViewOf(1).InsertLast(8);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 1, 8, 2, 7, 3, 4, 5 }));
                });
                lst.LastViewOf(5).InsertLast(9);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 1, 8, 2, 7, 3, 4, 5, 9 }));
                });
            }

            [Test]
            public void BadInsertAfter()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(6);
                lst.Add(5);
                Assert.That(lst.ViewOf(4), Is.Null);
            }

            [Test]
            public void InsertAll()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);

                IList<int> lst2 = new HashedArrayList<int>
                {
                    7,
                    8,
                    9
                };
                lst.InsertAll(0, lst2);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 8, 9, 1, 2, 3, 4 }));
                });
                lst.RemoveAll(lst2);
                lst.InsertAll(4, lst2);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 1, 2, 3, 4, 7, 8, 9 }));
                });
                lst.RemoveAll(lst2);
                lst.InsertAll(2, lst2);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 1, 2, 7, 8, 9, 3, 4 }));
                });
            }

            [Test]
            public void InsertAllBad()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);

                IList<int> lst2 = new HashedArrayList<int>
                {
                    5,
                    2,
                    9
                };

                Assert.Throws<DuplicateNotAllowedException>(() => lst.InsertAll(0, lst2));
            }

            [Test]
            public void Map()
            {
                string m(int i) { return "<<" + i + ">>"; }
                IList<string> r = lst.Map(m);

                Assert.Multiple(() =>
                {
                    Assert.That(((HashedArrayList<string>)r).Check(), Is.True);
                    Assert.That(r.IsEmpty, Is.True);
                });
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);
                r = lst.Map(m);
                Assert.Multiple(() =>
                {
                    Assert.That(((HashedArrayList<string>)r).Check(), Is.True);
                    Assert.That(r, Has.Count.EqualTo(4));
                });
                for (int i = 0; i < 4; i++)
                {
                    Assert.That(r[i], Is.EqualTo("<<" + (i + 1) + ">>"));
                }
            }
            [Test]
            public void BadMapper()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                bool m(int i) { if (i == 2) { lst.Add(7); } return true; }

                Assert.Throws<CollectionModifiedException>(() => lst.Map(m));
            }

            [Test]
            public void ModifyingFindAll()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                bool m(int i) { if (i == 2) { lst.Add(7); } return true; }

                Assert.Throws<CollectionModifiedException>(() => lst.FindAll(m));
            }

            [Test]
            public void BadMapperView()
            {
                lst = lst.View(0, 0);
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                bool m(int i) { if (i == 2) { lst.Add(7); } return true; }

                Assert.Throws<CollectionModifiedException>(() => lst.Map(m));
            }

            [Test]
            public void ModifyingFindAllView()
            {
                lst = lst.View(0, 0);
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                bool m(int i) { if (i == 2) { lst.Add(7); } return true; }

                Assert.Throws<CollectionModifiedException>(() => lst.FindAll(m));
            }


            [Test]
            public void BadRemove()
            {
                Assert.Throws<NoSuchItemException>(() => lst.Remove());
            }

            [Test]
            public void BadRemoveFirst()
            {
                Assert.Throws<NoSuchItemException>(() => lst.RemoveFirst());
            }

            [Test]
            public void BadRemoveLast()
            {
                Assert.Throws<NoSuchItemException>(() => lst.RemoveLast());
            }

            [Test]
            public void RemoveFirstLast()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);
                Assert.Multiple(() =>
                {
                    Assert.That(lst.RemoveFirst(), Is.EqualTo(1));
                    Assert.That(lst.RemoveLast(), Is.EqualTo(4));
                });
                Assert.That(lst.RemoveFirst(), Is.EqualTo(2));
                Assert.That(lst.RemoveLast(), Is.EqualTo(3));
                Assert.That(lst.IsEmpty, Is.True);
            }


            [Test]
            public void Reverse()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                lst.Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }));
                });
                lst.View(0, 3).Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 8, 9, 6, 5, 4, 3, 2, 1, 0 }));
                });
                lst.View(7, 0).Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 8, 9, 6, 5, 4, 3, 2, 1, 0 }));
                });
                lst.View(7, 3).Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 8, 9, 6, 5, 4, 3, 0, 1, 2 }));
                });
                lst.View(5, 1).Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(lst.Check(), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 7, 8, 9, 6, 5, 4, 3, 0, 1, 2 }));
                });
            }


            [Test]
            public void BadReverse()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => lst.View(8, 3).Reverse());
            }
        }

        [TestFixture]
        public class Combined
        {
            private IList<SCG.KeyValuePair<int, int>> lst;


            [SetUp]
            public void Init()
            {
                lst = new HashedArrayList<SCG.KeyValuePair<int, int>>(new KeyValuePairEqualityComparer<int, int>());
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(new SCG.KeyValuePair<int, int>(i, i + 30));
                }
            }

            [TearDown]
            public void Dispose()
            {
                lst.Dispose();
            }

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
                ICollection<string> coll = new HashedArrayList<string>();
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
        public class Sorting
        {
            private IList<int> lst;


            [SetUp]
            public void Init() { lst = new HashedArrayList<int>(); }


            [TearDown]
            public void Dispose() { lst.Dispose(); }


            [Test]
            public void Sort()
            {
                lst.Add(5); lst.Add(6); lst.Add(55); lst.Add(7); lst.Add(3);
                Assert.That(lst.IsSorted(new IC()), Is.False);
                lst.Sort(new IC());
                Assert.Multiple(() =>
                {
                    Assert.That(lst.IsSorted(), Is.True);
                    Assert.That(lst.IsSorted(new IC()), Is.True);
                    Assert.That(lst, Is.EqualTo(new[] { 3, 5, 6, 7, 55 }));
                });
            }
        }
    }




    namespace Range
    {
        [TestFixture]
        public class Range
        {
            private HashedArrayList<int> lst;


            [SetUp]
            public void Init() { lst = []; }


            [TearDown]
            public void Dispose() { lst.Dispose(); }


            [Test]
            public void GetRange()
            {
                //Assert.IsTrue(IC.eq(lst[0, 0)));
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(IC.Eq(lst[0, 3], 0, 1, 2), Is.True);
                    Assert.That(IC.Eq(lst[3, 3], 3, 4, 5), Is.True);
                    Assert.That(IC.Eq(lst[6, 3], 6, 7, 8), Is.True);
                    Assert.That(IC.Eq(lst[6, 4], 6, 7, 8, 9), Is.True);
                });
            }


            [Test]
            public void Backwards()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(lst.Backwards(), Is.EqualTo(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }));
                    Assert.That(IC.Eq(lst[0, 3].Backwards(), 2, 1, 0), Is.True);
                    Assert.That(IC.Eq(lst[3, 3].Backwards(), 5, 4, 3), Is.True);
                    Assert.That(IC.Eq(lst[6, 4].Backwards(), 9, 8, 7, 6), Is.True);
                });
            }


            [Test]
            public void DirectionAndCount()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.Multiple(() =>
                {
                    Assert.That(lst.Direction, Is.EqualTo(Direction.Forwards));
                    Assert.That(lst[3, 4].Direction, Is.EqualTo(Direction.Forwards));
                    Assert.That(lst[3, 4].Backwards().Direction, Is.EqualTo(Direction.Backwards));
                    Assert.That(lst.Backwards().Direction, Is.EqualTo(Direction.Backwards));
                    Assert.That(lst[3, 4], Has.Count.EqualTo(4));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(lst[3, 4].Backwards(), Has.Count.EqualTo(4));
                    Assert.That(lst.Backwards(), Has.Count.EqualTo(10));
                });
            }

            [Test]
            public void MoveNextAfterUpdate()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.Throws<CollectionModifiedException>(() =>
                {
                    foreach (int i in lst)
                    {
                        lst.Add(45 + i);
                    }
                });
            }
        }
    }

    namespace View
    {
        [TestFixture]
        public class Simple
        {
            private HashedArrayList<int> list;
            private HashedArrayList<int> view;


            [SetUp]
            public void Init()
            {
                list =
                [
                    0,
                    1,
                    2,
                    3
                ];
                view = (HashedArrayList<int>)list.View(1, 2);
            }


            [TearDown]
            public void Dispose()
            {
                list.Dispose();
                view.Dispose();
            }

            private void check()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(list.Check(), Is.True);
                    Assert.That(view.Check(), Is.True);
                });
            }

            [Test]
            public void InsertPointer()
            {
                IList<int> view2 = list.View(2, 0);
                list.Insert(view2, 7);
                check();
                list.Insert(list, 8);
                check();
                view.Insert(view2, 9);
                check();
                view.Insert(list.View(3, 2), 10);
                check();
                view.Insert(list.ViewOf(0), 11);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 11, 1, 9, 7, 2, 10, 3, 8 }));
                    Assert.That(view, Is.EqualTo(new[] { 11, 1, 9, 7, 2, 10 }));
                });
            }

            [Test]
            public void InsertPointerBad1()
            {
                Assert.Throws<IndexOutOfRangeException>(() => view.Insert(list.View(0, 0), 7));
            }

            [Test]
            public void InsertPointerBad2()
            {
                Assert.Throws<IndexOutOfRangeException>(() => view.Insert(list, 7));
            }

            [Test]
            public void InsertPointerBad3()
            {
                Assert.Throws<IncompatibleViewException>(() => list.Insert(new ArrayList<int>(), 7));
            }

            [Test]
            public void InsertPointerBad4()
            {
                Assert.Throws<IncompatibleViewException>(() => list.Insert(new ArrayList<int>().View(0, 0), 7));
            }

            [Test]
            public void Span()
            {
                IList<int> span = list.View(1, 0).Span(list.View(2, 0));
                Assert.Multiple(() =>
                {
                    Assert.That(span.Check(), Is.True);
                    Assert.That(span.Offset, Is.EqualTo(1));
                    Assert.That(span, Has.Count.EqualTo(1));
                });
                span = list.View(0, 2).Span(list.View(2, 2));
                Assert.Multiple(() =>
                {
                    Assert.That(span.Check(), Is.True);
                    Assert.That(span.Offset, Is.EqualTo(0));
                    Assert.That(span, Has.Count.EqualTo(4));
                });
                span = list.View(3, 1).Span(list.View(1, 1));
                Assert.That(span, Is.Null);
            }

            [Test]
            public void ViewOf()
            {
                for (int i = 0; i < 4; i++)
                {
                    list.Add(i);
                }

                IList<int> v = view.ViewOf(2);
                Assert.Multiple(() =>
                {
                    Assert.That(v.Check(), Is.True);
                    Assert.That(v, Is.EqualTo(new[] { 2 }));
                    Assert.That(v.Offset, Is.EqualTo(2));
                });
                v = list.ViewOf(2);
                Assert.Multiple(() =>
                {
                    Assert.That(v.Check(), Is.True);
                    Assert.That(v, Is.EqualTo(new[] { 2 }));
                    Assert.That(v.Offset, Is.EqualTo(2));
                });
                v = list.LastViewOf(2);
                Assert.Multiple(() =>
                {
                    Assert.That(v.Check(), Is.True);
                    Assert.That(v, Is.EqualTo(new[] { 2 }));
                    Assert.That(v.Offset, Is.EqualTo(2));
                });
            }

            [Test]
            public void BadViewOf()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(view.ViewOf(5), Is.Null);
                    Assert.That(view.LastViewOf(5), Is.Null);
                    Assert.That(view.ViewOf(3), Is.Null);
                    Assert.That(view.LastViewOf(3), Is.Null);
                    Assert.That(view.ViewOf(0), Is.Null);
                    Assert.That(view.LastViewOf(0), Is.Null);
                });
            }


            [Test]
            public void ArrayStuff()
            {
                Assert.That(view.ToArray(), Is.EqualTo(new[] { 1, 2 }));
                int[] extarray = new int[5];
                view.CopyTo(extarray, 2);
                Assert.That(extarray, Is.EqualTo(new[] { 0, 0, 1, 2, 0 }));
            }


            [Test]
            public void Add()
            {
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 1, 2, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 1, 2 }));
                });
                view.InsertFirst(10);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 10, 1, 2, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 10, 1, 2 }));
                });
                view.Clear();
                Assert.Multiple(() =>
                {
                    Assert.That(view.IsReadOnly, Is.False);
                    Assert.That(view.AllowsDuplicates, Is.False);
                    Assert.That(view.IsEmpty, Is.True);
                });
                check();
                Assert.That(list, Is.EqualTo(new[] { 0, 3 }));
                Assert.That(IC.Eq(view), Is.True);
                view.Add(8);
                Assert.That(view.IsEmpty, Is.False);
                Assert.That(view.AllowsDuplicates, Is.False);
                Assert.That(view.IsReadOnly, Is.False);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8 }));
                });
                view.Add(12);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 12, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8, 12 }));
                });
                view./*ViewOf(12).*/InsertLast(15);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 12, 15, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8, 12, 15 }));
                });
                view.ViewOf(12).InsertFirst(18);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 18, 12, 15, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8, 18, 12, 15 }));
                });

                HashedArrayList<int> lst2 =
                [
                    90,
                    92
                ];
                view.AddAll(lst2);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 18, 12, 15, 90, 92, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8, 18, 12, 15, 90, 92 }));
                });
                view.InsertLast(66);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 8, 18, 12, 15, 90, 92, 66, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 8, 18, 12, 15, 90, 92, 66 }));
                });
            }


            [Test]
            public void Bxxx()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(view.Backwards(), Is.EqualTo(new[] { 2, 1 }));
                    Assert.That(view.Underlying, Is.SameAs(list));
                    Assert.That(list.Underlying, Is.Null);
                    Assert.That(view.Direction, Is.EqualTo(Direction.Forwards));
                    Assert.That(view.Backwards().Direction, Is.EqualTo(Direction.Backwards));
                    Assert.That(list.Offset, Is.EqualTo(0));
                    Assert.That(view.Offset, Is.EqualTo(1));
                });
            }


            [Test]
            public void Contains()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(view.Contains(1), Is.True);
                    Assert.That(view.Contains(0), Is.False);
                });

                HashedArrayList<int> lst2 =
                [
                    2
                ];
                Assert.That(view.ContainsAll(lst2), Is.True);
                lst2.Add(3);
                Assert.Multiple(() =>
                {
                    Assert.That(view.ContainsAll(lst2), Is.False);
                    Assert.That(view.ContainsSpeed, Is.EqualTo(Speed.Constant));
                    Assert.That(view, Has.Count.EqualTo(2));
                });
                view.Add(1);
                Assert.Multiple(() =>
                {
                    Assert.That(view.ContainsCount(2), Is.EqualTo(1));
                    Assert.That(view.ContainsCount(1), Is.EqualTo(1));
                    Assert.That(view, Has.Count.EqualTo(2));
                });
            }


            [Test]
            public void CreateView()
            {
                HashedArrayList<int> view2 = (HashedArrayList<int>)view.View(1, 0);

                Assert.That(view2.Underlying, Is.SameAs(list));
            }


            [Test]
            public void FIFO()
            {
                Assert.That(view.FIFO, Is.False);
                view.FIFO = true;
                view.Add(23); view.Add(24); view.Add(25);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 1, 2, 23, 24, 25 }));
                    Assert.That(view.Remove(), Is.EqualTo(1));
                });
                check();
                Assert.That(view, Is.EqualTo(new[] { 2, 23, 24, 25 }));
                view.FIFO = false;
                Assert.Multiple(() =>
                {
                    Assert.That(view.FIFO, Is.False);
                    Assert.That(view.Remove(), Is.EqualTo(25));
                });
                check();
                Assert.That(view, Is.EqualTo(new[] { 2, 23, 24 }));
            }


            [Test]
            public void MapEtc()
            {
                HashedArrayList<double> dbl = (HashedArrayList<double>)view.Map(new Func<int, double>(delegate (int i) { return i / 10.0; }));

                Assert.Multiple(() =>
                {
                    Assert.That(dbl.Check(), Is.True);
                    Assert.That(dbl[0], Is.EqualTo(0.1));
                    Assert.That(dbl[1], Is.EqualTo(0.2));
                });
                for (int i = 0; i < 10; i++)
                {
                    view.Add(i);
                }

                HashedArrayList<int> list2 = (HashedArrayList<int>)view.FindAll(new Func<int, bool>(delegate (int i) { return i % 4 == 1; }));

                Assert.Multiple(() =>
                {
                    Assert.That(list2.Check(), Is.True);
                    Assert.That(list2, Is.EqualTo(new[] { 1, 5, 9 }));
                });
            }


            [Test]
            public void FL()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(view.First, Is.EqualTo(1));
                    Assert.That(view.Last, Is.EqualTo(2));
                });
            }


            [Test]
            public void Indexing()
            {
                list.Clear();
                for (int i = 0; i < 20; i++)
                {
                    list.Add(i);
                }

                view = (HashedArrayList<int>)list.View(5, 7);
                for (int i = 0; i < 7; i++)
                {
                    Assert.That(view[i], Is.EqualTo(i + 5));
                }

                for (int i = 0; i < 7; i++)
                {
                    Assert.That(view.IndexOf(i + 5), Is.EqualTo(i));
                }

                for (int i = 0; i < 7; i++)
                {
                    Assert.That(view.LastIndexOf(i + 5), Is.EqualTo(i));
                }
            }


            [Test]
            public void Insert()
            {
                view.Insert(0, 34);
                view.Insert(1, 35);
                view.Insert(4, 36);
                Assert.Multiple(() =>
                {
                    Assert.That(view.Check(), Is.True);
                    Assert.That(view, Is.EqualTo(new[] { 34, 35, 1, 2, 36 }));
                });

                IList<int> list2 = new HashedArrayList<int>
                {
                    40,
                    41
                };
                view.InsertAll(3, list2);
                Assert.Multiple(() =>
                {
                    Assert.That(view.Check(), Is.True);
                    Assert.That(view, Is.EqualTo(new[] { 34, 35, 1, 40, 41, 2, 36 }));
                });
            }


            [Test]
            public void Sort()
            {
                view.Add(45); view.Add(47); view.Add(46); view.Add(48);
                Assert.That(view.IsSorted(new IC()), Is.False);
                view.Sort(new IC());
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(list, Is.EqualTo(new[] { 0, 1, 2, 45, 46, 47, 48, 3 }));
                    Assert.That(view, Is.EqualTo(new[] { 1, 2, 45, 46, 47, 48 }));
                });
            }


            [Test]
            public void Remove()
            {
                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 1, 2, 5 }));
                    Assert.That(view.Remove(1), Is.True);
                });
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 2, 5 }));
                    Assert.That(view.Remove(1), Is.False);
                });
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 2, 5 }));
                    Assert.That(view.Remove(0), Is.False);
                });
                check();
                Assert.That(view, Is.EqualTo(new[] { 2, 5 }));
                view.RemoveAllCopies(3);
                check();
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 2, 5 }));
                    Assert.That(list, Is.EqualTo(new[] { 0, 2, 5, 3 }));
                });
                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.That(view, Is.EqualTo(new[] { 2, 5, 1 }));

                HashedArrayList<int> l2 =
                [
                    1,
                    2,
                    2,
                    3,
                    1
                ];
                view.RemoveAll(l2);
                check();
                Assert.That(view, Is.EqualTo(new[] { 5 }));
                view.RetainAll(l2);
                check();
                Assert.That(IC.Eq(view), Is.True);
                view.Add(2); view.Add(4); view.Add(5);
                Assert.Multiple(() =>
                {
                    Assert.That(view.RemoveAt(0), Is.EqualTo(2));
                    Assert.That(view.RemoveAt(1), Is.EqualTo(5));
                });
                Assert.That(view.RemoveAt(0), Is.EqualTo(4));
                check();
                Assert.That(IC.Eq(view), Is.True);
                view.Add(8); view.Add(6); view.Add(78);
                Assert.Multiple(() =>
                {
                    Assert.That(view.RemoveFirst(), Is.EqualTo(8));
                    Assert.That(view.RemoveLast(), Is.EqualTo(78));
                });
                view.Add(2); view.Add(5); view.Add(3); view.Add(1);
                view.RemoveInterval(1, 2);
                check();
                Assert.That(view, Is.EqualTo(new[] { 6, 1 }));
            }


            [Test]
            public void Reverse()
            {
                view.Clear();
                for (int i = 0; i < 10; i++)
                {
                    view.Add(10 + i);
                }

                view.View(3, 4).Reverse();
                check();
                Assert.That(view, Is.EqualTo(new[] { 10, 11, 12, 16, 15, 14, 13, 17, 18, 19 }));
                view.Reverse();
                Assert.Multiple(() =>
                {
                    Assert.That(view, Is.EqualTo(new[] { 19, 18, 17, 13, 14, 15, 16, 12, 11, 10 }));
                    Assert.That(list, Is.EqualTo(new[] { 0, 19, 18, 17, 13, 14, 15, 16, 12, 11, 10, 3 }));
                });
            }


            [Test]
            public void Slide()
            {
                view.Slide(1);
                check();
                Assert.That(view, Is.EqualTo(new[] { 2, 3 }));
                view.Slide(-2);
                check();
                Assert.That(view, Is.EqualTo(new[] { 0, 1 }));
                view.Slide(0, 3);
                check();
                Assert.That(view, Is.EqualTo(new[] { 0, 1, 2 }));
                view.Slide(2, 1);
                check();
                Assert.That(view, Is.EqualTo(new[] { 2 }));
                view.Slide(-1, 0);
                check();
                Assert.That(IC.Eq(view), Is.True);
                view.Add(28);
                Assert.That(list, Is.EqualTo(new[] { 0, 28, 1, 2, 3 }));
            }
            [Test]
            public void Iterate()
            {
                list.Clear();
                view = null;
                foreach (int i in new int[] { 2, 4, 8, 13, 6, 1, 10, 11 })
                {
                    list.Add(i);
                }

                view = (HashedArrayList<int>)list.View(list.Count - 2, 2);
                int j = 666;
                while (true)
                {
                    //Console.WriteLine("View: {0}:  {1} --> {2}", view.Count, view.First, view.Last);
                    if ((view.Last - view.First) % 2 == 1)
                    {
                        view.Insert(1, j++);
                    }

                    check();
                    if (view.Offset == 0)
                    {
                        break;
                    }
                    else
                    {
                        view.Slide(-1, 2);
                    }
                }
                //foreach (int cell in list) Console.Write(" " + cell);
                //Assert.IsTrue(list.Check());
                Assert.That(list, Is.EqualTo(new[] { 2, 4, 8, 668, 13, 6, 1, 667, 10, 666, 11 }));
            }


            [Test]
            public void SyncRoot()
            {
                Assert.That(((System.Collections.IList)list).SyncRoot, Is.SameAs(((System.Collections.IList)view).SyncRoot));
            }
        }

        [TestFixture]
        public class MulipleViews
        {
            private IList<int> list;
            private IList<int>[][] views;
            [SetUp]
            public void Init()
            {
                list = new HashedArrayList<int>();
                for (int i = 0; i < 6; i++)
                {
                    list.Add(i);
                }

                views = new IList<int>[7][];
                for (int i = 0; i < 7; i++)
                {
                    views[i] = new IList<int>[7 - i];
                    for (int j = 0; j < 7 - i; j++)
                    {
                        views[i][j] = list.View(i, j);
                    }
                }
            }
            [TearDown]
            public void Dispose()
            {
                list.Dispose();
                views = null;
            }
            [Test]
            public void Insert()
            {
                Assert.That(list.Check(), Is.True, "list check before insert");
                list.Insert(3, 777);
                Assert.That(list.Check(), Is.True, "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i < 3 || (i == 3 && j == 0) ? i : i + 1), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i < 3 && i + j > 3 ? j + 1 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }
            [Test]
            public void RemoveAt()
            {
                Assert.That(list.Check(), Is.True, "list check before remove");
                list.RemoveAt(3);
                Assert.That(list.Check(), Is.True, "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i <= 3 ? i : i - 1), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i <= 3 && i + j > 3 ? j - 1 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }

            [Test]
            public void RemoveInterval()
            {
                Assert.That(list.Check(), Is.True, "list check before remove");
                list.RemoveInterval(3, 2);
                Assert.That(list.Check(), Is.True, "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i <= 3 ? i : i <= 5 ? 3 : i - 2), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(j == 0 ? 0 : i <= 3 && i + j > 4 ? j - 2 : i > 4 || i + j <= 3 ? j : j - 1), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }

            [Test]
            public void InsertAtEnd()
            {
                Assert.That(list.Check(), Is.True, "list check before insert");
                list.InsertLast(777);
                Assert.That(list.Check(), Is.True, "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }
            [Test]
            public void RemoveAtEnd()
            {
                Assert.That(list.Check(), Is.True, "list check before remove");
                list.RemoveAt(5);
                Assert.That(list.Check(), Is.True, "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i <= 5 ? i : i - 1), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i <= 5 && i + j > 5 ? j - 1 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }
            [Test]
            public void InsertAtStart()
            {
                Assert.That(list.Check(), Is.True, "list check before insert");
                list.Insert(0, 777);
                Assert.That(list.Check(), Is.True, "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i == 0 && j == 0 ? 0 : i + 1), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }
            [Test]
            public void RemoveAtStart()
            {
                Assert.That(list.Check(), Is.True, "list check before remove");
                list.RemoveAt(0);
                Assert.That(list.Check(), Is.True, "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i == 0 ? i : i - 1), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i == 0 && j > 0 ? j - 1 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }
            [Test]
            public void Clear()
            {
                Assert.That(list.Check(), Is.True, "list check before clear");
                //for (int i = 0; i < 7; i++)
                //for (int j = 0; j < 7 - i; j++)
                //Console.WriteLine("// view[{0}][{1}] : {2}", i, j, ((HashedArrayList<int>) views[i][j]).GetHashCode());
                views[2][3].Clear();
                Assert.That(list.Check(), Is.True, "list check after clear");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i < 2 ? i : i < 6 ? 2 : i - 3), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(s(i, j)), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }

            private int s(int i, int j)
            {
                if (j == 0)
                {
                    return 0;
                }

                int k = i + j - 1; //end
                if (i > 4 || k <= 1)
                {
                    return j;
                }

                if (i >= 2)
                {
                    return k > 4 ? k - 4 : 0;
                }

                if (i <= 2)
                {
                    return k >= 4 ? j - 3 : 2 - i;
                }

                return -1;
            }
            [Test]
            public void InsertAll()
            {
                IList<int> list2 = new HashedArrayList<int>();
                for (int i = 0; i < 5; i++) { list2.Add(100 + i); }
                Assert.That(list.Check(), Is.True, "list check before insertAll");
                list.InsertAll(3, list2);
                Assert.That(list.Check(), Is.True, "list check after insertAll");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i < 3 || (i == 3 && j == 0) ? i : i + 5), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i < 3 && i + j > 3 ? j + 5 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }

            [Test]
            public void AddAll()
            {
                IList<int> list2 = new HashedArrayList<int>();
                for (int i = 0; i < 5; i++) { list2.Add(100 + i); }
                Assert.That(list.Check(), Is.True, "list check before AddAll");
                list.View(1, 2).AddAll(list2);
                Assert.That(list.Check(), Is.True, "list check after AddAll");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(views[i][j].Offset, Is.EqualTo(i < 3 || (i == 3 && j == 0) ? i : i + 5), "view[" + i + "][" + j + "] offset");
                            Assert.That(views[i][j], Has.Count.EqualTo(i < 3 && i + j > 3 ? j + 5 : j), "view[" + i + "][" + j + "] count");
                        });
                    }
                }
            }

            [Test]
            public void Remove()
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        list = new HashedArrayList<int>();
                        for (int k = 0; k < 6; k++)
                        {
                            list.Add(k);
                        }

                        _ = (HashedArrayList<int>)list.View(i, j);
                        list.Remove(3);
                        Assert.That(list.Check(), Is.True, "list check after Remove, i=" + i + ", j=" + j);
                    }
                }
            }
            [Test]
            public void RemoveAll1()
            {
                IList<int> list2 = new HashedArrayList<int>
                {
                    1,
                    3,
                    4
                };

                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        list = new HashedArrayList<int>();
                        for (int k = 0; k < 6; k++)
                        {
                            list.Add(k);
                        }

                        _ = (HashedArrayList<int>)list.View(i, j);
                        list.RemoveAll(list2);
                        Assert.That(list.Check(), Is.True, "list check after RemoveAll, i=" + i + ", j=" + j);
                    }
                }
            }
            [Test]
            public void RemoveAll2()
            {
                IList<int> list2 = new HashedArrayList<int>
                {
                    1,
                    3,
                    4
                };
                Assert.That(list.Check(), Is.True, "list check before RemoveAll");
                list.RemoveAll(list2);

                Assert.Multiple(() =>
                {
                    Assert.That(views[0][0].Offset, Is.EqualTo(0), "view [0][0] offset");
                    Assert.That(views[0][1].Offset, Is.EqualTo(0), "view [0][1] offset");
                    Assert.That(views[0][2].Offset, Is.EqualTo(0), "view [0][2] offset");
                    Assert.That(views[0][3].Offset, Is.EqualTo(0), "view [0][3] offset");
                    Assert.That(views[0][4].Offset, Is.EqualTo(0), "view [0][4] offset");
                    Assert.That(views[0][5].Offset, Is.EqualTo(0), "view [0][5] offset");
                    Assert.That(views[0][6].Offset, Is.EqualTo(0), "view [0][6] offset");
                    Assert.That(views[1][0].Offset, Is.EqualTo(1), "view [1][0] offset");
                    Assert.That(views[1][1].Offset, Is.EqualTo(1), "view [1][1] offset");
                    Assert.That(views[1][2].Offset, Is.EqualTo(1), "view [1][2] offset");
                    Assert.That(views[1][3].Offset, Is.EqualTo(1), "view [1][3] offset");
                    Assert.That(views[1][4].Offset, Is.EqualTo(1), "view [1][4] offset");
                    Assert.That(views[1][5].Offset, Is.EqualTo(1), "view [1][5] offset");
                    Assert.That(views[2][0].Offset, Is.EqualTo(1), "view [2][0] offset");
                    Assert.That(views[2][1].Offset, Is.EqualTo(1), "view [2][1] offset");
                    Assert.That(views[2][2].Offset, Is.EqualTo(1), "view [2][2] offset");
                    Assert.That(views[2][3].Offset, Is.EqualTo(1), "view [2][3] offset");
                    Assert.That(views[2][4].Offset, Is.EqualTo(1), "view [2][4] offset");
                    Assert.That(views[3][0].Offset, Is.EqualTo(2), "view [3][0] offset");
                    Assert.That(views[3][1].Offset, Is.EqualTo(2), "view [3][1] offset");
                    Assert.That(views[3][2].Offset, Is.EqualTo(2), "view [3][2] offset");
                    Assert.That(views[3][3].Offset, Is.EqualTo(2), "view [3][3] offset");
                    Assert.That(views[4][0].Offset, Is.EqualTo(2), "view [4][0] offset");
                    Assert.That(views[4][1].Offset, Is.EqualTo(2), "view [4][1] offset");
                    Assert.That(views[4][2].Offset, Is.EqualTo(2), "view [4][2] offset");
                    Assert.That(views[5][0].Offset, Is.EqualTo(2), "view [5][0] offset");
                    Assert.That(views[5][1].Offset, Is.EqualTo(2), "view [5][1] offset");
                    Assert.That(views[6][0].Offset, Is.EqualTo(3), "view [6][0] offset");

                    Assert.That(views[0][0], Is.Empty, "view [0][0] count");
                    Assert.That(views[0][1], Has.Count.EqualTo(1), "view [0][1] count");
                    Assert.That(views[0][2], Has.Count.EqualTo(1), "view [0][2] count");
                    Assert.That(views[0][3], Has.Count.EqualTo(2), "view [0][3] count");
                    Assert.That(views[0][4], Has.Count.EqualTo(2), "view [0][4] count");
                    Assert.That(views[0][5], Has.Count.EqualTo(2), "view [0][5] count");
                    Assert.That(views[0][6], Has.Count.EqualTo(3), "view [0][6] count");
                    Assert.That(views[1][0], Is.Empty, "view [1][0] count");
                    Assert.That(views[1][1], Is.Empty, "view [1][1] count");
                    Assert.That(views[1][2], Has.Count.EqualTo(1), "view [1][2] count");
                    Assert.That(views[1][3], Has.Count.EqualTo(1), "view [1][3] count");
                    Assert.That(views[1][4], Has.Count.EqualTo(1), "view [1][4] count");
                    Assert.That(views[1][5], Has.Count.EqualTo(2), "view [1][5] count");
                    Assert.That(views[2][0], Is.Empty, "view [2][0] count");
                    Assert.That(views[2][1], Has.Count.EqualTo(1), "view [2][1] count");
                    Assert.That(views[2][2], Has.Count.EqualTo(1), "view [2][2] count");
                    Assert.That(views[2][3], Has.Count.EqualTo(1), "view [2][3] count");
                    Assert.That(views[2][4], Has.Count.EqualTo(2), "view [2][4] count");
                    Assert.That(views[3][0], Is.Empty, "view [3][0] count");
                    Assert.That(views[3][1], Is.Empty, "view [3][1] count");
                    Assert.That(views[3][2], Is.Empty, "view [3][2] count");
                    Assert.That(views[3][3], Has.Count.EqualTo(1), "view [3][3] count");
                    Assert.That(views[4][0], Is.Empty, "view [4][0] count");
                    Assert.That(views[4][1], Is.Empty, "view [4][1] count");
                    Assert.That(views[4][2], Has.Count.EqualTo(1), "view [4][2] count");
                    Assert.That(views[5][0], Is.Empty, "view [5][0] count");
                    Assert.That(views[5][1], Has.Count.EqualTo(1), "view [5][1] count");
                    Assert.That(views[6][0], Is.Empty, "view [6][0] count");

                    Assert.That(list.Check(), Is.True, "list check after RemoveAll");
                });
            }

            [Test]
            public void RetainAll()
            {
                IList<int> list2 = new HashedArrayList<int>
                {
                    2,
                    4,
                    5
                };
                Assert.That(list.Check(), Is.True, "list check before RetainAll");
                list.RetainAll(list2);
                Assert.Multiple(() =>
                {
                    Assert.That(views[0][0].Offset, Is.EqualTo(0), "view [0][0] offset");
                    Assert.That(views[0][1].Offset, Is.EqualTo(0), "view [0][1] offset");
                    Assert.That(views[0][2].Offset, Is.EqualTo(0), "view [0][2] offset");
                    Assert.That(views[0][3].Offset, Is.EqualTo(0), "view [0][3] offset");
                    Assert.That(views[0][4].Offset, Is.EqualTo(0), "view [0][4] offset");
                    Assert.That(views[0][5].Offset, Is.EqualTo(0), "view [0][5] offset");
                    Assert.That(views[0][6].Offset, Is.EqualTo(0), "view [0][6] offset");
                    Assert.That(views[1][0].Offset, Is.EqualTo(0), "view [1][0] offset");
                    Assert.That(views[1][1].Offset, Is.EqualTo(0), "view [1][1] offset");
                    Assert.That(views[1][2].Offset, Is.EqualTo(0), "view [1][2] offset");
                    Assert.That(views[1][3].Offset, Is.EqualTo(0), "view [1][3] offset");
                    Assert.That(views[1][4].Offset, Is.EqualTo(0), "view [1][4] offset");
                    Assert.That(views[1][5].Offset, Is.EqualTo(0), "view [1][5] offset");
                    Assert.That(views[2][0].Offset, Is.EqualTo(0), "view [2][0] offset");
                    Assert.That(views[2][1].Offset, Is.EqualTo(0), "view [2][1] offset");
                    Assert.That(views[2][2].Offset, Is.EqualTo(0), "view [2][2] offset");
                    Assert.That(views[2][3].Offset, Is.EqualTo(0), "view [2][3] offset");
                    Assert.That(views[2][4].Offset, Is.EqualTo(0), "view [2][4] offset");
                    Assert.That(views[3][0].Offset, Is.EqualTo(1), "view [3][0] offset");
                    Assert.That(views[3][1].Offset, Is.EqualTo(1), "view [3][1] offset");
                    Assert.That(views[3][2].Offset, Is.EqualTo(1), "view [3][2] offset");
                    Assert.That(views[3][3].Offset, Is.EqualTo(1), "view [3][3] offset");
                    Assert.That(views[4][0].Offset, Is.EqualTo(1), "view [4][0] offset");
                    Assert.That(views[4][1].Offset, Is.EqualTo(1), "view [4][1] offset");
                    Assert.That(views[4][2].Offset, Is.EqualTo(1), "view [4][2] offset");
                    Assert.That(views[5][0].Offset, Is.EqualTo(2), "view [5][0] offset");
                    Assert.That(views[5][1].Offset, Is.EqualTo(2), "view [5][1] offset");
                    Assert.That(views[6][0].Offset, Is.EqualTo(3), "view [6][0] offset");

                    Assert.That(views[0][0], Is.Empty, "view [0][0] count");
                    Assert.That(views[0][1], Is.Empty, "view [0][1] count");
                    Assert.That(views[0][2], Is.Empty, "view [0][2] count");
                    Assert.That(views[0][3], Has.Count.EqualTo(1), "view [0][3] count");
                    Assert.That(views[0][4], Has.Count.EqualTo(1), "view [0][4] count");
                    Assert.That(views[0][5], Has.Count.EqualTo(2), "view [0][5] count");
                    Assert.That(views[0][6], Has.Count.EqualTo(3), "view [0][6] count");
                    Assert.That(views[1][0], Is.Empty, "view [1][0] count");
                    Assert.That(views[1][1], Is.Empty, "view [1][1] count");
                    Assert.That(views[1][2], Has.Count.EqualTo(1), "view [1][2] count");
                    Assert.That(views[1][3], Has.Count.EqualTo(1), "view [1][3] count");
                    Assert.That(views[1][4], Has.Count.EqualTo(2), "view [1][4] count");
                    Assert.That(views[1][5], Has.Count.EqualTo(3), "view [1][5] count");
                    Assert.That(views[2][0], Is.Empty, "view [2][0] count");
                    Assert.That(views[2][1], Has.Count.EqualTo(1), "view [2][1] count");
                    Assert.That(views[2][2], Has.Count.EqualTo(1), "view [2][2] count");
                    Assert.That(views[2][3], Has.Count.EqualTo(2), "view [2][3] count");
                    Assert.That(views[2][4], Has.Count.EqualTo(3), "view [2][4] count");
                    Assert.That(views[3][0], Is.Empty, "view [3][0] count");
                    Assert.That(views[3][1], Is.Empty, "view [3][1] count");
                    Assert.That(views[3][2], Has.Count.EqualTo(1), "view [3][2] count");
                    Assert.That(views[3][3], Has.Count.EqualTo(2), "view [3][3] count");
                    Assert.That(views[4][0], Is.Empty, "view [4][0] count");
                    Assert.That(views[4][1], Has.Count.EqualTo(1), "view [4][1] count");
                    Assert.That(views[4][2], Has.Count.EqualTo(2), "view [4][2] count");
                    Assert.That(views[5][0], Is.Empty, "view [5][0] count");
                    Assert.That(views[5][1], Has.Count.EqualTo(1), "view [5][1] count");
                    Assert.That(views[6][0], Is.Empty, "view [6][0] count");

                    Assert.That(list.Check(), Is.True, "list check after RetainAll");
                });
            }

            [Test]
            public void RemoveAllCopies()
            {
                IList<int> list2 = new HashedArrayList<int>
                {
                    0,
                    2,
                    82,
                    92,
                    5,
                    2,
                    1
                };
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        list = new HashedArrayList<int>();
                        list.AddAll(list2);
                        _ = (HashedArrayList<int>)list.View(i, j);
                        list.RemoveAllCopies(2);
                        Assert.That(list.Check(), Is.True, "list check after RemoveAllCopies, i=" + i + ", j=" + j);
                    }
                }
            }

            private void checkDisposed(bool reverse, int start, int count)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        int k;
                        if (i + j <= start || i >= start + count || (i <= start && i + j >= start + count) || (reverse && start <= i && start + count >= i + j))
                        {
                            try
                            {
                                k = views[i][j].Count;
                            }
                            catch (ViewDisposedException)
                            {
                                Assert.Fail("view[" + i + "][" + j + "] threw");
                            }
                            Assert.That(views[i][j], Has.Count.EqualTo(j), "view[" + i + "][" + j + "] size");
                            if (reverse && ((j > 0 && start <= i && start + count >= i + j) || (j == 0 && start < i && start + count > i)))
                            {
                                Assert.That(views[i][j].Offset, Is.EqualTo(start + (start + count - i - j)), "view[" + i + "][" + j + "] offset (mirrored)");
                            }
                            else
                            {
                                Assert.That(views[i][j].Offset, Is.EqualTo(i), "view[" + i + "][" + j + "] offset");
                            }
                        }
                        else
                        {
                            try
                            {
                                k = views[i][j].Count;
                                Assert.Fail("view[" + i + "][" + j + "] no throw");
                            }
                            catch (ViewDisposedException) { }
                        }
                    }
                }
            }

            [Test]
            public void Reverse()
            {
                int start = 2, count = 3;
                IList<int> list2 = list.View(start, count);
                Assert.That(list.Check(), Is.True, "list check before Reverse");
                list2.Reverse();
                Assert.That(list.Check(), Is.True, "list check after Reverse");
                checkDisposed(true, start, count);
            }

            [Test]
            public void Sort()
            {
                int start = 2, count = 3;
                IList<int> list2 = list.View(start, count);
                Assert.That(list.Check(), Is.True, "list check before Sort");
                list2.Sort();
                Assert.That(list.Check(), Is.True, "list check after Sort");
                checkDisposed(false, start, count);
            }
            [Test]
            public void Shuffle()
            {
                int start = 2, count = 3;
                IList<int> list2 = list.View(start, count);
                Assert.That(list.Check(), Is.True, "list check before Shuffle");
                list2.Shuffle();
                Assert.That(list.Check(), Is.True, "list check after Shuffle");
                checkDisposed(false, start, count);
            }


        }

    }

    namespace HashingAndEquals
    {
        [TestFixture]
        public class IIndexed
        {
            private HashedArrayList<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
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
                dut.Add(7);
                Assert.That(dut.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(7)));
                dut.Add(3);
                Assert.That(dut.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(7, 3)));
            }


            [Test]
            public void EqualHashButDifferent()
            {
                dit.Add(0); dit.Add(31);
                dat.Add(1); dat.Add(0);
                Assert.Multiple(() =>
                {
                    Assert.That(dat.GetSequencedHashCode(), Is.EqualTo(dit.GetSequencedHashCode()));
                    Assert.That(dit.SequencedEquals(dat), Is.False);
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
                dut.InsertFirst(7);
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
            private HashedArrayList<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
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
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
            }
        }

        [TestFixture]
        public class MultiLevelUnorderedOfUnOrdered
        {
            private HashedArrayList<int> dit, dat, dut;

            private HashedArrayList<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                Dit = [];
                Dat = [];
                Dut = [];
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
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
                Dit.Dispose();
                Dat.Dispose();
                Dut.Dispose();
            }
        }



        [TestFixture]
        public class MultiLevelOrderedOfUnOrdered
        {
            private HashedArrayList<int> dit, dat, dut;

            private HashedArrayList<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                Dit = [];
                Dat = [];
                Dut = [];
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
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
                Dit.Dispose();
                Dat.Dispose();
                Dut.Dispose();
            }
        }



        [TestFixture]
        public class MultiLevelUnOrderedOfOrdered
        {
            private HashedArrayList<int> dit, dat, dut, dot;

            private HashedArrayList<ISequenced<int>> Dit, Dat, Dut, Dot;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
                dot = [];
                dit.Add(2); dit.Add(1);
                dat.Add(1); dat.Add(2);
                dut.Add(3);
                dot.Add(2); dot.Add(1);
                Dit = [];
                Dat = [];
                Dut = [];
                Dot = [];
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
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.Multiple(() =>
                {
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
                Dit.Dispose();
                Dat.Dispose();
                Dut.Dispose();
                Dot.Dispose();
            }
        }



        [TestFixture]
        public class MultiLevelOrderedOfOrdered
        {
            private HashedArrayList<int> dit, dat, dut, dot;

            private HashedArrayList<ISequenced<int>> Dit, Dat, Dut, Dot;


            [SetUp]
            public void Init()
            {
                dit = [];
                dat = [];
                dut = [];
                dot = [];
                dit.Add(2); dit.Add(1); //{2,1}
                dat.Add(1); dat.Add(2); //{1,2}
                dut.Add(3);            //{3}
                dot.Add(2); dot.Add(1); //{2,1}
                Dit = [];
                Dat = [];
                Dut = [];
                Dot = [];
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
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit); // {{2,1},{3}}
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat); // {{3},{2,1},{1,2}}
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit); // {{2,1},{3}}
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut); // {{2,1},{3}}
                Assert.Multiple(() =>
                {
                    Assert.That(Dit.SequencedEquals(Dut), Is.True);
                    Assert.That(Dit.SequencedEquals(Dat), Is.False);
                    Assert.That(Dit.SequencedEquals(Dot), Is.True);
                });
            }


            [TearDown]
            public void Dispose()
            {
                dit.Dispose();
                dat.Dispose();
                dut.Dispose();
                dot.Dispose();
                Dit.Dispose();
                Dat.Dispose();
                Dut.Dispose();
                Dot.Dispose();
            }
        }
    }
}