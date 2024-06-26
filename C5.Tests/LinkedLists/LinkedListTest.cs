// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;


namespace C5.Tests.linkedlists.plain
{
    using CollectionOfInt = LinkedList<int>;

    [TestFixture]
    public class GenericTesters
    {

        [Test]
        public void TestEvents()
        {
            CollectionOfInt factory() { return new CollectionOfInt(TenEqualityComparer.Default); }
            new C5.Tests.Templates.Events.ListTester<CollectionOfInt>().Test(factory);
            new C5.Tests.Templates.Events.QueueTester<CollectionOfInt>().Test(factory);
            new C5.Tests.Templates.Events.StackTester<CollectionOfInt>().Test(factory);
        }

        //[Test]
        //public void Extensible()
        //{
        //    C5.Tests.Templates.Extensible.Clone.Tester<CollectionOfInt>();
        //    C5.Tests.Templates.Extensible.Clone.ViewTester<CollectionOfInt>();
        //    C5.Tests.Templates.Extensible.Serialization.Tester<CollectionOfInt>();
        //    C5.Tests.Templates.Extensible.Serialization.ViewTester<CollectionOfInt>();
        //}

        [Test]
        public void List()
        {
            C5.Tests.Templates.List.Dispose.Tester<CollectionOfInt>();
            C5.Tests.Templates.List.SCG_IList.Tester<CollectionOfInt>();
        }
    }

    internal static class Factory
    {
        public static ICollection<T> New<T>() { return new LinkedList<T>(); }
    }

    namespace Enumerable
    {
        [TestFixture]
        public class Multiops
        {
            private LinkedList<int> list;

            private Func<int, bool> always, never, even;


            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>();
                always = delegate { return true; };
                never = delegate { return false; };
                even = delegate (int i) { return i % 2 == 0; };
            }


            [Test]
            public void All()
            {
                Assert.IsTrue(list.All(always));
                Assert.IsTrue(list.All(never));
                Assert.IsTrue(list.All(even));
                list.Add(8);
                Assert.IsTrue(list.All(always));
                Assert.IsFalse(list.All(never));
                Assert.IsTrue(list.All(even));
                list.Add(5);
                Assert.IsTrue(list.All(always));
                Assert.IsFalse(list.All(never));
                Assert.IsFalse(list.All(even));
            }


            [Test]
            public void Exists()
            {
                Assert.IsFalse(list.Exists(always));
                Assert.IsFalse(list.Exists(never));
                Assert.IsFalse(list.Exists(even));
                list.Add(5);
                Assert.IsTrue(list.Exists(always));
                Assert.IsFalse(list.Exists(never));
                Assert.IsFalse(list.Exists(even));
                list.Add(8);
                Assert.IsTrue(list.Exists(always));
                Assert.IsFalse(list.Exists(never));
                Assert.IsTrue(list.Exists(even));
            }


            [Test]
            public void Apply()
            {
                int sum = 0;
                void a(int i) { sum = i + 10 * sum; }

                list.Apply(a);
                Assert.AreEqual(0, sum);
                sum = 0;
                list.Add(5); list.Add(8); list.Add(7); list.Add(5);
                list.Apply(a);
                Assert.AreEqual(5875, sum);
            }


            [TearDown]
            public void Dispose() { list = null; }
        }



        [TestFixture]
        public class GetEnumerator
        {
            private LinkedList<int> list;


            [SetUp]
            public void Init() { list = new LinkedList<int>(); }


            [Test]
            public void Empty()
            {
                SCG.IEnumerator<int> e = list.GetEnumerator();

                Assert.IsFalse(e.MoveNext());
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

                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(5, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(8, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(5, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(5, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(10, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(1, e.Current);
                Assert.IsFalse(e.MoveNext());
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
            public void Dispose() { list = null; }
        }
    }

    namespace CollectionOrExtensible
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
                Assert.AreEqual("[  ]", coll.ToString());
                coll.AddAll(new int[] { -4, 28, 129, 65530 });
                Assert.AreEqual("[ -4, 28, 129, 65530 ]", coll.ToString());
                Assert.AreEqual("[ -4, 1C, 81, FFFA ]", coll.ToString(null, rad16));
                Assert.AreEqual("[ -4, 28, 129... ]", coll.ToString("L14", null));
                Assert.AreEqual("[ -4, 1C, 81... ]", coll.ToString("L14", rad16));
            }
        }

        [TestFixture]
        public class CollectionOrSink
        {
            private LinkedList<int> list;


            [SetUp]
            public void Init() { list = new LinkedList<int>(); }

            [Test]
            public void NullEqualityComparerinConstructor1()
            {
                Assert.Throws<NullReferenceException>(() => new LinkedList<int>(null));
            }

            [Test]
            public void Choose()
            {
                list.Add(7);
                Assert.AreEqual(7, list.Choose());
            }

            [Test]
            public void BadChoose()
            {
                Assert.Throws<NoSuchItemException>(() => list.Choose());
            }

            [Test]
            public void CountEtAl()
            {
                Assert.AreEqual(0, list.Count);
                Assert.IsTrue(list.IsEmpty);
                Assert.IsTrue(list.AllowsDuplicates);
                list.Add(5);
                Assert.AreEqual(1, list.Count);
                Assert.IsFalse(list.IsEmpty);
                list.Add(5);
                Assert.AreEqual(2, list.Count);
                Assert.IsFalse(list.IsEmpty);
                list.Add(8);
                Assert.AreEqual(3, list.Count);
            }


            [Test]
            public void AddAll()
            {
                list.Add(3); list.Add(4); list.Add(5);

                LinkedList<int> list2 = new LinkedList<int>();

                list2.AddAll(list);
                Assert.IsTrue(IC.Eq(list2, 3, 4, 5));
                list.AddAll(list2);
                Assert.IsTrue(IC.Eq(list2, 3, 4, 5));
                Assert.IsTrue(IC.Eq(list, 3, 4, 5, 3, 4, 5));
            }


            [TearDown]
            public void Dispose() { list = null; }
        }

        [TestFixture]
        public class FindPredicate
        {
            private LinkedList<int> list;
            private Func<int, bool> pred;

            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>(TenEqualityComparer.Default);
                pred = delegate (int i) { return i % 5 == 0; };
            }

            [TearDown]
            public void Dispose() { list = null; }

            [Test]
            public void Find()
            {
                Assert.IsFalse(list.Find(pred, out int i));
                list.AddAll(new int[] { 4, 22, 67, 37 });
                Assert.IsFalse(list.Find(pred, out i));
                list.AddAll(new int[] { 45, 122, 675, 137 });
                Assert.IsTrue(list.Find(pred, out i));
                Assert.AreEqual(45, i);
            }

            [Test]
            public void FindLast()
            {
                Assert.IsFalse(list.FindLast(pred, out int i));
                list.AddAll(new int[] { 4, 22, 67, 37 });
                Assert.IsFalse(list.FindLast(pred, out i));
                list.AddAll(new int[] { 45, 122, 675, 137 });
                Assert.IsTrue(list.FindLast(pred, out i));
                Assert.AreEqual(675, i);
            }

            [Test]
            public void FindIndex()
            {
                Assert.IsFalse(0 <= list.FindIndex(pred));
                list.AddAll(new int[] { 4, 22, 67, 37 });
                Assert.IsFalse(0 <= list.FindIndex(pred));
                list.AddAll(new int[] { 45, 122, 675, 137 });
                Assert.AreEqual(4, list.FindIndex(pred));
            }

            [Test]
            public void FindLastIndex()
            {
                Assert.IsFalse(0 <= list.FindLastIndex(pred));
                list.AddAll(new int[] { 4, 22, 67, 37 });
                Assert.IsFalse(0 <= list.FindLastIndex(pred));
                list.AddAll(new int[] { 45, 122, 675, 137 });
                Assert.AreEqual(6, list.FindLastIndex(pred));
            }
        }

        [TestFixture]
        public class UniqueItems
        {
            private LinkedList<int> list;

            [SetUp]
            public void Init() { list = new LinkedList<int>(); }

            [TearDown]
            public void Dispose() { list = null; }

            [Test]
            public void Test()
            {
                Assert.IsTrue(IC.SetEq(list.UniqueItems()));
                Assert.IsTrue(IC.SetEq(list.ItemMultiplicities()));
                list.AddAll(new int[] { 7, 9, 7 });
                Assert.IsTrue(IC.SetEq(list.UniqueItems(), 7, 9));
                Assert.IsTrue(IC.SetEq(list.ItemMultiplicities(), 7, 2, 9, 1));
            }
        }

        [TestFixture]
        public class ArrayTest
        {
            private LinkedList<int> list;
            private int[] a;


            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>();
                a = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    a[i] = 1000 + i;
                }
            }


            [TearDown]
            public void Dispose() { list = null; }


            private string aeq(int[] a, params int[] b)
            {
                if (a.Length != b.Length)
                {
                    return "Lengths differ: " + a.Length + " != " + b.Length;
                }

                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return string.Format("{0}'th elements differ: {1} != {2}", i, a[i], b[i]);
                    }
                }

                return "Alles klar";
            }


            [Test]
            public void ToArray()
            {
                Assert.AreEqual("Alles klar", aeq(list.ToArray()));
                list.Add(7);
                list.Add(7);
                Assert.AreEqual("Alles klar", aeq(list.ToArray(), 7, 7));
            }


            [Test]
            public void CopyTo()
            {
                list.CopyTo(a, 1);
                Assert.AreEqual("Alles klar", aeq(a, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009));
                list.Add(6);
                list.CopyTo(a, 2);
                Assert.AreEqual("Alles klar", aeq(a, 1000, 1001, 6, 1003, 1004, 1005, 1006, 1007, 1008, 1009));
                list.Add(4);
                list.Add(4);
                list.Add(9);
                list.CopyTo(a, 4);
                Assert.AreEqual("Alles klar", aeq(a, 1000, 1001, 6, 1003, 6, 4, 4, 9, 1008, 1009));
                list.Clear();
                list.Add(7);
                list.CopyTo(a, 9);
                Assert.AreEqual("Alles klar", aeq(a, 1000, 1001, 6, 1003, 6, 4, 4, 9, 1008, 7));
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
                list.Add(3);

                Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(a, 9));
            }
        }

        [TestFixture]
        public class Sync
        {
            private LinkedList<int> list;

            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>();
            }

            [TearDown]
            public void Dispose() { list = null; }

            [Test]
            public void Get()
            {
                Assert.IsNotNull(((System.Collections.IList)list).SyncRoot);
            }
        }
    }

    namespace EditableCollection
    {
        [TestFixture]
        public class Searching
        {
            private LinkedList<int> list;

            [SetUp]
            public void Init() { list = new LinkedList<int>(); }

            [Test]
            public void Contains()
            {
                Assert.IsFalse(list.Contains(5));
                list.Add(5);
                Assert.IsTrue(list.Contains(5));
                Assert.IsFalse(list.Contains(7));
                list.Add(8);
                list.Add(10);
                Assert.IsTrue(list.Contains(5));
                Assert.IsFalse(list.Contains(7));
                Assert.IsTrue(list.Contains(8));
                Assert.IsTrue(list.Contains(10));
                list.Remove(8);
                Assert.IsTrue(list.Contains(5));
                Assert.IsFalse(list.Contains(7));
                Assert.IsFalse(list.Contains(8));
                Assert.IsTrue(list.Contains(10));
            }

            [Test]
            public void ContainsCount()
            {
                Assert.AreEqual(0, list.ContainsCount(5));
                list.Add(5);
                Assert.AreEqual(1, list.ContainsCount(5));
                Assert.AreEqual(0, list.ContainsCount(7));
                list.Add(8);
                Assert.AreEqual(1, list.ContainsCount(5));
                Assert.AreEqual(0, list.ContainsCount(7));
                Assert.AreEqual(1, list.ContainsCount(8));
                list.Add(5);
                Assert.AreEqual(2, list.ContainsCount(5));
                Assert.AreEqual(0, list.ContainsCount(7));
                Assert.AreEqual(1, list.ContainsCount(8));
            }


            [Test]
            public void RemoveAllCopies()
            {
                list.Add(5); list.Add(7); list.Add(5);
                Assert.AreEqual(2, list.ContainsCount(5));
                Assert.AreEqual(1, list.ContainsCount(7));
                list.RemoveAllCopies(5);
                Assert.AreEqual(0, list.ContainsCount(5));
                Assert.AreEqual(1, list.ContainsCount(7));
                list.Add(5); list.Add(8); list.Add(5);
                list.RemoveAllCopies(8);
                Assert.IsTrue(IC.Eq(list, 7, 5, 5));
            }


            [Test]
            public void FindAll()
            {
                bool f(int i) { return i % 2 == 0; }

                Assert.IsTrue(list.FindAll(f).IsEmpty);
                list.Add(5); list.Add(8); list.Add(5); list.Add(10); list.Add(8);
                Assert.IsTrue(((LinkedList<int>)list.FindAll(f)).Check());
                Assert.IsTrue(IC.Eq(list.FindAll(f), 8, 10, 8));
            }


            [Test]
            public void ContainsAll()
            {
                LinkedList<int> list2 = new LinkedList<int>();

                Assert.IsTrue(list.ContainsAll(list2));
                list2.Add(4);
                Assert.IsFalse(list.ContainsAll(list2));
                list.Add(4);
                Assert.IsTrue(list.ContainsAll(list2));
                list.Add(5);
                Assert.IsTrue(list.ContainsAll(list2));
                list2.Add(4);
                Assert.IsFalse(list.ContainsAll(list2));
                list.Add(4);
                Assert.IsTrue(list.ContainsAll(list2));
            }


            [Test]
            public void RetainAll()
            {
                LinkedList<int> list2 = new LinkedList<int>();

                list.Add(4); list.Add(4); list.Add(5); list.Add(4); list.Add(6);
                list2.Add(5); list2.Add(4); list2.Add(7); list2.Add(7); list2.Add(4);
                list.RetainAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 4, 5));
                list.Add(5); list.Add(4); list.Add(6);
                list2.Clear();
                list2.Add(5); list2.Add(5); list2.Add(6);
                list.RetainAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 5, 5, 6));
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                list.RetainAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list));
            }


            [Test]
            public void RemoveAll()
            {
                LinkedList<int> list2 = new LinkedList<int>();

                list.Add(4); list.Add(4); list.Add(5); list.Add(4); list.Add(6);
                list2.Add(5); list2.Add(4); list2.Add(7); list2.Add(7); list2.Add(4);
                list.RemoveAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 6));
                list.Add(5); list.Add(4); list.Add(6);
                list2.Clear();
                list2.Add(6); list2.Add(5); list2.Add(5); list2.Add(6);
                list.RemoveAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 4));
                list2.Clear();
                list2.Add(7); list2.Add(8); list2.Add(9);
                list.RemoveAll(list2);
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 4));
            }


            [Test]
            public void Remove()
            {

                Assert.IsTrue(list.FIFO);
                list.Add(4); list.Add(4); list.Add(5); list.Add(4); list.Add(6);
                Assert.IsFalse(list.Remove(2));
                Assert.IsTrue(list.Check());
                Assert.IsTrue(list.Remove(4));
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 5, 4, 6));
                Assert.AreEqual(6, list.RemoveLast());
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 5, 4));
                list.Add(7);
                Assert.AreEqual(4, list.RemoveFirst());
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 5, 4, 7));

                list.FIFO = false;
                list.Clear();
                list.Add(4); list.Add(4); list.Add(5); list.Add(4); list.Add(6);
                Assert.IsFalse(list.Remove(2));
                Assert.IsTrue(list.Check());
                Assert.IsTrue(list.Remove(4));
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 4, 5, 6));
                Assert.AreEqual(6, list.RemoveLast());
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 4, 5));
                list.Add(7);
                Assert.AreEqual(4, list.RemoveFirst());
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 4, 5, 7));
            }


            [Test]
            public void Clear()
            {
                list.Add(7); list.Add(7);
                list.Clear();
                Assert.IsTrue(list.IsEmpty);
            }


            [TearDown]
            public void Dispose() { list = null; }
        }
    }




    namespace IIndexed
    {
        [TestFixture]
        public class Searching
        {
            private IIndexed<int> dit;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
            }


            [Test]
            public void IndexOf()
            {
                Assert.AreEqual(~0, dit.IndexOf(6));
                dit.Add(7);
                Assert.AreEqual(~1, dit.IndexOf(6));
                Assert.AreEqual(~1, dit.LastIndexOf(6));
                Assert.AreEqual(0, dit.IndexOf(7));
                dit.Add(5); dit.Add(7); dit.Add(8); dit.Add(7);
                Assert.AreEqual(~5, dit.IndexOf(6));
                Assert.AreEqual(0, dit.IndexOf(7));
                Assert.AreEqual(4, dit.LastIndexOf(7));
                Assert.AreEqual(3, dit.IndexOf(8));
                Assert.AreEqual(1, dit.LastIndexOf(5));
            }


            [TearDown]
            public void Dispose()
            {
                dit = null;
            }
        }



        [TestFixture]
        public class Removing
        {
            private IIndexed<int> dit;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
            }


            [Test]
            public void RemoveAt()
            {
                dit.Add(5); dit.Add(7); dit.Add(9); dit.Add(1); dit.Add(2);
                Assert.AreEqual(7, dit.RemoveAt(1));
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 5, 9, 1, 2));
                Assert.AreEqual(5, dit.RemoveAt(0));
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 9, 1, 2));
                Assert.AreEqual(2, dit.RemoveAt(2));
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 9, 1));
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
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 10, 20, 30, 40, 50, 60));
                dit.RemoveInterval(3, 1);
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 10, 20, 30, 50, 60));
                dit.RemoveInterval(1, 3);
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 10, 60));
                dit.RemoveInterval(0, 2);
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit));
                dit.Add(10); dit.Add(20); dit.Add(30); dit.Add(40); dit.Add(50); dit.Add(60);
                dit.RemoveInterval(0, 2);
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 30, 40, 50, 60));
                dit.RemoveInterval(2, 2);
                Assert.IsTrue(((LinkedList<int>)dit).Check());
                Assert.IsTrue(IC.Eq(dit, 30, 40));
            }


            [TearDown]
            public void Dispose()
            {
                dit = null;
            }
        }
    }




    namespace IList_
    {
        [TestFixture]
        public class Searching
        {
            private IList<int> lst;


            [SetUp]
            public void Init()
            {
                lst = new LinkedList<int>();
            }

            [TearDown]
            public void Dispose()
            {
                lst = null;
            }

            [Test]
            public void FirstBad()
            {
                Assert.Throws<NoSuchItemException>(() => { int f = lst.First; });
            }

            [Test]
            public void LastBad()
            {
                Assert.Throws<NoSuchItemException>(() => { int f = lst.Last; });
            }

            [Test]
            public void FirstLast()
            {
                lst.Add(19);
                Assert.AreEqual(19, lst.First);
                Assert.AreEqual(19, lst.Last);
                lst.Add(34); lst.InsertFirst(12);
                Assert.AreEqual(12, lst.First);
                Assert.AreEqual(34, lst.Last);
            }


            [Test]
            public void This()
            {
                lst.Add(34);
                Assert.AreEqual(34, lst[0]);
                lst[0] = 56;
                Assert.AreEqual(56, lst.First);
                lst.Add(7); lst.Add(7); lst.Add(7); lst.Add(7);
                lst[0] = 45; lst[2] = 78; lst[4] = 101;
                Assert.IsTrue(IC.Eq(lst, 45, 7, 78, 7, 101));
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
        public class Combined
        {
            private IList<System.Collections.Generic.KeyValuePair<int, int>> lst;


            [SetUp]
            public void Init()
            {
                lst = new LinkedList<System.Collections.Generic.KeyValuePair<int, int>>(new KeyValuePairEqualityComparer<int, int>());
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
                System.Collections.Generic.KeyValuePair<int, int> p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);

                Assert.IsTrue(lst.Find(ref p));
                Assert.AreEqual(3, p.Key);
                Assert.AreEqual(33, p.Value);
                p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
                Assert.IsFalse(lst.Find(ref p));
            }


            [Test]
            public void FindOrAdd()
            {
                System.Collections.Generic.KeyValuePair<int, int> p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);

                Assert.IsTrue(lst.FindOrAdd(ref p));
                Assert.AreEqual(3, p.Key);
                Assert.AreEqual(33, p.Value);
                p = new System.Collections.Generic.KeyValuePair<int, int>(13, 79);
                Assert.IsFalse(lst.FindOrAdd(ref p));
                Assert.AreEqual(13, lst[10].Key);
                Assert.AreEqual(79, lst[10].Value);
            }


            [Test]
            public void Update()
            {
                System.Collections.Generic.KeyValuePair<int, int> p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);

                Assert.IsTrue(lst.Update(p));
                Assert.AreEqual(3, lst[3].Key);
                Assert.AreEqual(78, lst[3].Value);
                p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
                Assert.IsFalse(lst.Update(p));
            }


            [Test]
            public void UpdateOrAdd1()
            {
                System.Collections.Generic.KeyValuePair<int, int> p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);

                Assert.IsTrue(lst.UpdateOrAdd(p));
                Assert.AreEqual(3, lst[3].Key);
                Assert.AreEqual(78, lst[3].Value);
                p = new System.Collections.Generic.KeyValuePair<int, int>(13, 79);
                Assert.IsFalse(lst.UpdateOrAdd(p));
                Assert.AreEqual(13, lst[10].Key);
                Assert.AreEqual(79, lst[10].Value);
            }

            [Test]
            public void UpdateOrAdd2()
            {
                ICollection<String> coll = new LinkedList<String>();
                // s1 and s2 are distinct objects but contain the same text:
                String s1 = "abc", s2 = ("def" + s1).Substring(3);
                Assert.IsFalse(coll.UpdateOrAdd(s1, out string old));
                Assert.AreEqual(null, old);
                Assert.IsTrue(coll.UpdateOrAdd(s2, out old));
                Assert.IsTrue(Object.ReferenceEquals(s1, old));
                Assert.IsFalse(Object.ReferenceEquals(s2, old));
            }


            [Test]
            public void RemoveWithReturn()
            {
                System.Collections.Generic.KeyValuePair<int, int> p = new System.Collections.Generic.KeyValuePair<int, int>(3, 78);

                Assert.IsTrue(lst.Remove(p, out p));
                Assert.AreEqual(3, p.Key);
                Assert.AreEqual(33, p.Value);
                Assert.AreEqual(4, lst[3].Key);
                Assert.AreEqual(34, lst[3].Value);
                p = new System.Collections.Generic.KeyValuePair<int, int>(13, 78);
                Assert.IsFalse(lst.Remove(p, out _));
            }
        }


        [TestFixture]
        public class Inserting
        {
            private IList<int>? lst;

            [SetUp]
            public void Init() { lst = new LinkedList<int>(); }


            [TearDown]
            public void Dispose() { lst = null; }


            [Test]
            public void Insert()
            {
                lst.Insert(0, 5);
                Assert.IsTrue(IC.Eq(lst, 5));
                lst.Insert(0, 7);
                Assert.IsTrue(IC.Eq(lst, 7, 5));
                lst.Insert(1, 4);
                Assert.IsTrue(IC.Eq(lst, 7, 4, 5));
                lst.Insert(3, 2);
                Assert.IsTrue(IC.Eq(lst, 7, 4, 5, 2));
            }

            [Test]
            public void InsertDuplicate()
            {
                lst.Insert(0, 5);
                Assert.IsTrue(IC.Eq(lst, 5));
                lst.Insert(0, 7);
                Assert.IsTrue(IC.Eq(lst, 7, 5));
                lst.Insert(1, 5);
                Assert.IsTrue(IC.Eq(lst, 7, 5, 5));
            }

            [Test]
            public void InsertAllDuplicate1()
            {
                lst.Insert(0, 3);
                Assert.IsTrue(IC.Eq(lst, 3));
                lst.Insert(0, 7);
                Assert.IsTrue(IC.Eq(lst, 7, 3));
                lst.InsertAll(1, new int[] { 1, 2, 3, 4 });
                Assert.IsTrue(IC.Eq(lst, 7, 1, 2, 3, 4, 3));
                Assert.IsTrue(lst.Check());
            }
            [Test]
            public void InsertAllDuplicate2()
            {
                lst.Insert(0, 3);
                Assert.IsTrue(IC.Eq(lst, 3));
                lst.Insert(0, 7);
                Assert.IsTrue(IC.Eq(lst, 7, 3));
                lst.InsertAll(1, new int[] { 5, 6, 5, 8 });
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 5, 6, 5, 8, 3));
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

                Assert.IsTrue(lst.FIFO);
                Assert.AreEqual(0, lst.Remove());
                Assert.AreEqual(2, lst.Remove());
                lst.FIFO = false;
                Assert.AreEqual(12, lst.Remove());
                Assert.AreEqual(10, lst.Remove());
                lst.FIFO = true;
                Assert.AreEqual(4, lst.Remove());
                Assert.AreEqual(6, lst.Remove());
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
                Assert.IsTrue(IC.Eq(lst, 34, 24, 14, 4, 5, 15, 25, 55));
            }


            [Test]
            public void InsertFirst()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(2);
                lst.Add(5);
                lst.ViewOf(2).InsertFirst(7);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 2, 3, 2, 5));
                lst.ViewOf(3).InsertFirst(8);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 2, 8, 3, 2, 5));
                lst.ViewOf(5).InsertFirst(9);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 2, 8, 3, 2, 9, 5));
            }


            [Test]
            public void BadInsertFirst()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(2);
                lst.Add(5);
                Assert.IsNull(lst.ViewOf(4));
            }


            [Test]
            public void InsertAfter()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(2);
                lst.Add(5);
                lst.LastViewOf(2).InsertLast(7);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 1, 2, 3, 2, 7, 5));
                lst.LastViewOf(1).InsertLast(8);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 1, 8, 2, 3, 2, 7, 5));
                lst.LastViewOf(5).InsertLast(9);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 1, 8, 2, 3, 2, 7, 5, 9));
            }


            [Test]
            public void BadInsertAfter()
            {
                lst.Add(2);
                lst.Add(3);
                lst.Add(2);
                lst.Add(5);
                Assert.IsNull(lst.ViewOf(4));
            }


            [Test]
            public void InsertAll()
            {
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);

                IList<int> lst2 = new LinkedList<int>() { 7, 8, 9 };

                lst.InsertAll(0, lst2);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 1, 2, 3, 4));
                lst.InsertAll(7, lst2);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 1, 2, 3, 4, 7, 8, 9));
                lst.InsertAll(5, lst2);
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 1, 2, 7, 8, 9, 3, 4, 7, 8, 9));
            }


            [Test]
            public void Map()
            {
                string m(int i) { return "<<" + i + ">>"; }
                IList<string> r = lst.Map(m);

                Assert.IsTrue(r.Check());
                Assert.IsTrue(r.IsEmpty);
                lst.Add(1);
                lst.Add(2);
                lst.Add(3);
                lst.Add(4);
                r = lst.Map(m);
                Assert.IsTrue(r.Check());
                Assert.AreEqual(4, r.Count);
                for (int i = 0; i < 4; i++)
                {
                    Assert.AreEqual("<<" + (i + 1) + ">>", r[i]);
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
                Assert.AreEqual(1, lst.RemoveFirst());
                Assert.AreEqual(4, lst.RemoveLast());
                Assert.AreEqual(2, lst.RemoveFirst());
                Assert.AreEqual(3, lst.RemoveLast());
                Assert.IsTrue(lst.IsEmpty);
            }

            [Test]
            public void RemoveFirstEmpty()
            {
                Assert.Throws<NoSuchItemException>(() => lst.RemoveFirst());
            }

            [Test]
            public void RemoveLastEmpty()
            {
                Assert.Throws<NoSuchItemException>(() => lst.RemoveLast());
            }

            [Test]
            public void Reverse()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                lst.Reverse();
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0));
                lst.View(0, 3).Reverse();
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 6, 5, 4, 3, 2, 1, 0));
                lst.View(7, 0).Reverse();
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 6, 5, 4, 3, 2, 1, 0));
                lst.View(7, 3).Reverse();
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 6, 5, 4, 3, 0, 1, 2));
                lst.View(5, 1).Reverse();
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(IC.Eq(lst, 7, 8, 9, 6, 5, 4, 3, 0, 1, 2));
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
        public class SortingTests
        {
            private IList<int> lst;


            [SetUp]
            public void Init() { lst = new LinkedList<int>(); }


            [TearDown]
            public void Dispose() { lst = null; }


            [Test]
            public void Sort()
            {
                lst.Add(5); lst.Add(6); lst.Add(5); lst.Add(7); lst.Add(3);
                Assert.IsFalse(lst.IsSorted(new IC()));
                lst.Sort(new IC());
                Assert.IsTrue(lst.Check());
                Assert.IsTrue(lst.IsSorted());
                Assert.IsTrue(lst.IsSorted(new IC()));
                Assert.IsTrue(IC.Eq(lst, 3, 5, 5, 6, 7));
            }


            [Test]
            public void Stability()
            {
                IList<System.Collections.Generic.KeyValuePair<int, string>> lst2 = new LinkedList<System.Collections.Generic.KeyValuePair<int, string>>();
                SCG.IComparer<System.Collections.Generic.KeyValuePair<int, string>> c = new KeyValuePairComparer<int, string>(new IC());

                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(5, "a"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(5, "b"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(6, "c"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(4, "d"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(3, "e"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(4, "f"));
                lst2.Add(new System.Collections.Generic.KeyValuePair<int, string>(5, "handle"));
                Assert.IsFalse(lst2.IsSorted(c));
                lst2.Sort(c);
                Assert.IsTrue(lst2.IsSorted(c));

                System.Collections.Generic.KeyValuePair<int, string> p = lst2.RemoveFirst();

                Assert.AreEqual(3, p.Key);
                Assert.AreEqual("e", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(4, p.Key);
                Assert.AreEqual("d", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(4, p.Key);
                Assert.AreEqual("f", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(5, p.Key);
                Assert.AreEqual("a", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(5, p.Key);
                Assert.AreEqual("b", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(5, p.Key);
                Assert.AreEqual("handle", p.Value);
                p = lst2.RemoveFirst();
                Assert.AreEqual(6, p.Key);
                Assert.AreEqual("c", p.Value);
                Assert.IsTrue(lst2.IsEmpty);
            }
        }
        [TestFixture]
        public class ShuffleTests
        {
            private IList<int> lst;


            [SetUp]
            public void Init() { lst = new LinkedList<int>(); }


            [TearDown]
            public void Dispose() { lst = null; }


            [Test]
            public void Shuffle()
            {
                lst.Add(5); lst.Add(6); lst.Add(5); lst.Add(7); lst.Add(3);
                for (int i = 0; i < 100; i++)
                {
                    lst.Shuffle(new C5Random(i + 1));
                    Assert.IsTrue(lst.Check(), "Check " + i);
                    int[] lst2 = lst.ToArray();
                    Sorting.IntroSort<int>(lst2);
                    Assert.IsTrue(IC.Eq(lst2, 3, 5, 5, 6, 7), "Contents " + i);
                }
            }
        }
    }


    namespace IStackQueue
    {
        [TestFixture]
        public class Stack
        {
            private IStack<int> list;


            [SetUp]
            public void Init() { list = new LinkedList<int>(); }


            [Test]
            public void Normal()
            {
                list.Push(7);
                list.Push(5);
                list.Push(7);
                list.Push(8);
                list.Push(9);
                Assert.AreEqual(9, list.Pop());
                Assert.AreEqual(8, list.Pop());
                Assert.AreEqual(7, list.Pop());
                Assert.AreEqual(5, list.Pop());
                Assert.AreEqual(7, list.Pop());
            }

            [Test]
            public void PopEmpty()
            {
                list.Push(5);
                Assert.AreEqual(5, list.Pop());
                Assert.Throws<NoSuchItemException>(() => list.Pop());
            }

            [TearDown]
            public void Dispose() { list = null; }
        }
        [TestFixture]
        public class Queue
        {
            private IQueue<int> list;


            [SetUp]
            public void Init() { list = new LinkedList<int>(); }


            [Test]
            public void Normal()
            {
                list.Enqueue(7);
                list.Enqueue(5);
                list.Enqueue(7);
                list.Enqueue(8);
                list.Enqueue(9);
                Assert.AreEqual(7, list.Dequeue());
                Assert.AreEqual(5, list.Dequeue());
                Assert.AreEqual(7, list.Dequeue());
                Assert.AreEqual(8, list.Dequeue());
                Assert.AreEqual(9, list.Dequeue());
            }

            [Test]
            public void DeQueueEmpty()
            {
                list.Enqueue(5);
                Assert.AreEqual(5, list.Dequeue());
                Assert.Throws<NoSuchItemException>(() => list.Dequeue());
            }

            [TearDown]
            public void Dispose() { list = null; }
        }
    }

    namespace Range
    {
        [TestFixture]
        public class Range
        {
            private IList<int> lst;


            [SetUp]
            public void Init() { lst = new LinkedList<int>(); }


            [TearDown]
            public void Dispose() { lst = null; }


            [Test]
            public void GetRange()
            {
                //Assert.IsTrue(IC.eq(lst[0, 0)));
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.IsTrue(IC.Eq(lst[0, 3], 0, 1, 2));
                Assert.IsTrue(IC.Eq(lst[3, 4], 3, 4, 5, 6));
                Assert.IsTrue(IC.Eq(lst[6, 4], 6, 7, 8, 9));
            }

            [Test]
            public void BadGetRange()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    object foo = lst[0, 11];
                });
            }

            [Test]
            public void Backwards()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.IsTrue(IC.Eq(lst.Backwards(), 9, 8, 7, 6, 5, 4, 3, 2, 1, 0));
                Assert.IsTrue(IC.Eq(lst[0, 4].Backwards(), 3, 2, 1, 0));
                Assert.IsTrue(IC.Eq(lst[3, 4].Backwards(), 6, 5, 4, 3));
                Assert.IsTrue(IC.Eq(lst[6, 4].Backwards(), 9, 8, 7, 6));
            }

            [Test]
            public void DirectionAndCount()
            {
                for (int i = 0; i < 10; i++)
                {
                    lst.Add(i);
                }

                Assert.AreEqual(Direction.Forwards, lst.Direction);
                Assert.AreEqual(Direction.Forwards, lst[3, 7].Direction);
                Assert.AreEqual(Direction.Backwards, lst[3, 7].Backwards().Direction);
                Assert.AreEqual(Direction.Backwards, lst.Backwards().Direction);
                Assert.AreEqual(4, lst[3, 4].Count);
                Assert.AreEqual(4, lst[3, 4].Backwards().Count);
                Assert.AreEqual(10, lst.Backwards().Count);
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
            private LinkedList<int> list, view;


            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>() { 0, 1, 2, 3 };
                view = (LinkedList<int>)list.View(1, 2);
            }


            [TearDown]
            public void Dispose()
            {
                list = view = null;
            }

            private void check()
            {
                Assert.IsTrue(list.Check());
                Assert.IsTrue(view.Check());
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
                Assert.IsTrue(IC.Eq(list, 0, 11, 1, 9, 7, 2, 10, 3, 8));
                Assert.IsTrue(IC.Eq(view, 11, 1, 9, 7, 2, 10));
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
                Assert.IsTrue(span.Check());
                Assert.AreEqual(1, span.Offset);
                Assert.AreEqual(1, span.Count);
                span = list.View(0, 2).Span(list.View(2, 2));
                Assert.IsTrue(span.Check());
                Assert.AreEqual(0, span.Offset);
                Assert.AreEqual(4, span.Count);
                span = list.View(3, 1).Span(list.View(1, 1));
                Assert.IsNull(span);
            }

            [Test]
            public void ViewOf()
            {
                for (int i = 0; i < 4; i++)
                {
                    list.Add(i);
                }

                IList<int> v = view.ViewOf(2);
                Assert.IsTrue(v.Check());
                Assert.IsTrue(IC.Eq(v, 2));
                Assert.AreEqual(2, v.Offset);
                v = list.ViewOf(2);
                Assert.IsTrue(v.Check());
                Assert.IsTrue(IC.Eq(v, 2));
                Assert.AreEqual(2, v.Offset);
                v = list.LastViewOf(2);
                Assert.IsTrue(v.Check());
                Assert.IsTrue(IC.Eq(v, 2));
                Assert.AreEqual(6, v.Offset);
            }

            [Test]
            public void BadViewOf()
            {
                Assert.IsNull(view.ViewOf(5));
                Assert.IsNull(view.LastViewOf(5));
                Assert.IsNull(view.ViewOf(3));
                Assert.IsNull(view.LastViewOf(3));
                Assert.IsNull(view.ViewOf(0));
                Assert.IsNull(view.LastViewOf(0));
            }

            [Test]
            public void ArrayStuff()
            {
                Assert.IsTrue(IC.Eq(view.ToArray(), 1, 2));
                int[] extarray = new int[5];
                view.CopyTo(extarray, 2);
                Assert.IsTrue(IC.Eq(extarray, 0, 0, 1, 2, 0));
            }


            [Test]
            public void Add()
            {
                check();
                Assert.IsTrue(IC.Eq(list, 0, 1, 2, 3));
                Assert.IsTrue(IC.Eq(view, 1, 2));
                view.InsertFirst(10);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 10, 1, 2, 3));
                Assert.IsTrue(IC.Eq(view, 10, 1, 2));
                view.Clear();
                Assert.IsFalse(view.IsReadOnly);
                Assert.IsTrue(view.AllowsDuplicates);
                Assert.IsTrue(view.IsEmpty);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 3));
                Assert.IsTrue(IC.Eq(view));
                view.Add(8);
                Assert.IsFalse(view.IsEmpty);
                Assert.IsTrue(view.AllowsDuplicates);
                Assert.IsFalse(view.IsReadOnly);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 3));
                Assert.IsTrue(IC.Eq(view, 8));
                view.Add(12);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 12, 3));
                Assert.IsTrue(IC.Eq(view, 8, 12));
                view./*ViewOf(12).*/InsertLast(15);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 12, 15, 3));
                Assert.IsTrue(IC.Eq(view, 8, 12, 15));
                view.ViewOf(12).InsertFirst(18);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 18, 12, 15, 3));
                Assert.IsTrue(IC.Eq(view, 8, 18, 12, 15));

                LinkedList<int> lst2 = new LinkedList<int>() { 90, 92 };
                view.AddAll(lst2);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 18, 12, 15, 90, 92, 3));
                Assert.IsTrue(IC.Eq(view, 8, 18, 12, 15, 90, 92));
                view.InsertLast(66);
                check();
                Assert.IsTrue(IC.Eq(list, 0, 8, 18, 12, 15, 90, 92, 66, 3));
                Assert.IsTrue(IC.Eq(view, 8, 18, 12, 15, 90, 92, 66));
            }


            [Test]
            public void Bxxx()
            {
                Assert.IsTrue(IC.Eq(view.Backwards(), 2, 1));
                Assert.AreSame(list, view.Underlying);
                Assert.IsNull(list.Underlying);
                Assert.AreEqual(Direction.Forwards, view.Direction);
                Assert.AreEqual(Direction.Backwards, view.Backwards().Direction);
                Assert.AreEqual(0, list.Offset);
                Assert.AreEqual(1, view.Offset);
            }


            [Test]
            public void Contains()
            {
                Assert.IsTrue(view.Contains(1));
                Assert.IsFalse(view.Contains(0));

                LinkedList<int> lst2 = new LinkedList<int>() { 2 };

                Assert.IsTrue(view.ContainsAll(lst2));
                lst2.Add(3);
                Assert.IsFalse(view.ContainsAll(lst2));
                Assert.AreEqual(Speed.Linear, view.ContainsSpeed);
                Assert.AreEqual(2, view.Count);
                view.Add(1);
                Assert.AreEqual(1, view.ContainsCount(2));
                Assert.AreEqual(2, view.ContainsCount(1));
                Assert.AreEqual(3, view.Count);
            }


            [Test]
            public void CreateView()
            {
                LinkedList<int> view2 = (LinkedList<int>)view.View(1, 0);

                Assert.AreSame(list, view2.Underlying);
            }


            [Test]
            public void FIFO()
            {
                Assert.IsTrue(view.FIFO);
                view.Add(23); view.Add(24); view.Add(25);
                check();
                Assert.IsTrue(IC.Eq(view, 1, 2, 23, 24, 25));
                Assert.AreEqual(1, view.Remove());
                check();
                Assert.IsTrue(IC.Eq(view, 2, 23, 24, 25));
                view.FIFO = false;
                Assert.IsFalse(view.FIFO);
                Assert.AreEqual(25, view.Remove());
                check();
                Assert.IsTrue(IC.Eq(view, 2, 23, 24));
            }


            [Test]
            public void MapEtc()
            {
                LinkedList<double> dbl = (LinkedList<double>)view.Map(new Func<int, double>(delegate (int i) { return i / 10.0; }));

                Assert.IsTrue(dbl.Check());
                Assert.AreEqual(0.1, dbl[0]);
                Assert.AreEqual(0.2, dbl[1]);
                for (int i = 0; i < 10; i++)
                {
                    view.Add(i);
                }

                list = (LinkedList<int>)view.FindAll(new Func<int, bool>(delegate (int i) { return i % 4 == 1; }));
                Assert.IsTrue(list.Check());
                Assert.IsTrue(IC.Eq(list, 1, 1, 5, 9));
            }


            [Test]
            public void FL()
            {
                Assert.AreEqual(1, view.First);
                Assert.AreEqual(2, view.Last);
            }


            [Test]
            public void Indexing()
            {
                list.Clear();
                for (int i = 0; i < 20; i++)
                {
                    list.Add(i);
                }

                view = (LinkedList<int>)list.View(5, 7);
                for (int i = 0; i < 7; i++)
                {
                    Assert.AreEqual(i + 5, view[i]);
                }

                for (int i = 0; i < 7; i++)
                {
                    Assert.AreEqual(i, view.IndexOf(i + 5));
                }

                for (int i = 0; i < 7; i++)
                {
                    Assert.AreEqual(i, view.LastIndexOf(i + 5));
                }
            }


            [Test]
            public void INsert()
            {
                view.Insert(0, 34);
                view.Insert(1, 35);
                view.Insert(4, 36);
                Assert.IsTrue(view.Check());
                Assert.IsTrue(IC.Eq(view, 34, 35, 1, 2, 36));

                IList<int> list2 = new LinkedList<int>();

                list2.AddAll(view);
                view.InsertAll(3, list2);
                Assert.IsTrue(view.Check());
                Assert.IsTrue(IC.Eq(view, 34, 35, 1, 34, 35, 1, 2, 36, 2, 36));
            }


            [Test]
            public void Sort()
            {
                view.Add(45); view.Add(47); view.Add(46); view.Add(48);
                Assert.IsFalse(view.IsSorted(new IC()));
                view.Sort(new IC());
                check();
                Assert.IsTrue(IC.Eq(list, 0, 1, 2, 45, 46, 47, 48, 3));
                Assert.IsTrue(IC.Eq(view, 1, 2, 45, 46, 47, 48));
            }


            [Test]
            public void Remove()
            {
                view.FIFO = false;
                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.IsTrue(IC.Eq(view, 1, 2, 1, 5, 3, 1, 3, 0));
                Assert.IsTrue(view.Remove(1));
                check();
                Assert.IsTrue(IC.Eq(view, 1, 2, 1, 5, 3, 3, 0));
                Assert.IsTrue(view.Remove(1));
                check();
                Assert.IsTrue(IC.Eq(view, 1, 2, 5, 3, 3, 0));
                Assert.IsTrue(view.Remove(0));
                check();
                Assert.IsTrue(IC.Eq(view, 1, 2, 5, 3, 3));
                view.RemoveAllCopies(3);
                check();
                Assert.IsTrue(IC.Eq(view, 1, 2, 5));
                Assert.IsTrue(IC.Eq(list, 0, 1, 2, 5, 3));
                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.IsTrue(IC.Eq(view, 1, 2, 5, 1, 5, 3, 1, 3, 0));

                view.FIFO = true;
                view.Clear(); view.Add(1); view.Add(2);

                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.IsTrue(IC.Eq(view, 1, 2, 1, 5, 3, 1, 3, 0));
                Assert.IsTrue(view.Remove(1));
                check();
                Assert.IsTrue(IC.Eq(view, 2, 1, 5, 3, 1, 3, 0));
                Assert.IsTrue(view.Remove(1));
                check();
                Assert.IsTrue(IC.Eq(view, 2, 5, 3, 1, 3, 0));
                Assert.IsTrue(view.Remove(0));
                check();
                Assert.IsTrue(IC.Eq(view, 2, 5, 3, 1, 3));
                view.RemoveAllCopies(3);
                check();
                Assert.IsTrue(IC.Eq(view, 2, 5, 1));
                Assert.IsTrue(IC.Eq(list, 0, 2, 5, 1, 3));
                view.Add(1); view.Add(5); view.Add(3); view.Add(1); view.Add(3); view.Add(0);
                Assert.IsTrue(IC.Eq(view, 2, 5, 1, 1, 5, 3, 1, 3, 0));

                LinkedList<int> l2 = new LinkedList<int>() { 1, 2, 2, 3, 1 };

                view.RemoveAll(l2);
                check();
                Assert.IsTrue(IC.Eq(view, 5, 5, 1, 3, 0));
                view.RetainAll(l2);
                check();
                Assert.IsTrue(IC.Eq(view, 1, 3));
                view.Add(2); view.Add(4); view.Add(5);
                Assert.AreEqual(1, view.RemoveAt(0));
                Assert.AreEqual(5, view.RemoveAt(3));
                Assert.AreEqual(2, view.RemoveAt(1));
                check();
                Assert.IsTrue(IC.Eq(view, 3, 4));
                view.Add(8);
                Assert.AreEqual(3, view.RemoveFirst());
                Assert.AreEqual(8, view.RemoveLast());
                view.Add(2); view.Add(5); view.Add(3); view.Add(1);
                view.RemoveInterval(1, 2);
                check();
                Assert.IsTrue(IC.Eq(view, 4, 3, 1));
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
                Assert.IsTrue(IC.Eq(view, 10, 11, 12, 16, 15, 14, 13, 17, 18, 19));
                view.Reverse();
                Assert.IsTrue(IC.Eq(view, 19, 18, 17, 13, 14, 15, 16, 12, 11, 10));
                Assert.IsTrue(IC.Eq(list, 0, 19, 18, 17, 13, 14, 15, 16, 12, 11, 10, 3));
            }


            [Test]
            public void Slide()
            {
                view.Slide(1);
                check();
                Assert.IsTrue(IC.Eq(view, 2, 3));
                view.Slide(-2);
                check();
                Assert.IsTrue(IC.Eq(view, 0, 1));
                view.Slide(0, 3);
                check();
                Assert.IsTrue(IC.Eq(view, 0, 1, 2));
                view.Slide(2, 1);
                check();
                Assert.IsTrue(IC.Eq(view, 2));
                Assert.AreEqual(view, view.Slide(-1, 0));
                check();
                Assert.IsTrue(IC.Eq(view));
                view.Add(28);
                Assert.IsTrue(IC.Eq(list, 0, 28, 1, 2, 3));
            }
            [Test]
            public void Iterate()
            {
                list.Clear();
                view = null;
                foreach (int i in new int[] { 2, 4, 8, 13, 6, 1, 2, 7 })
                {
                    list.Add(i);
                }

                view = (LinkedList<int>)list.View(list.Count - 2, 2);
                while (true)
                {
                    //Console.WriteLine("View: {0}:  {1} --> {2}", view.Count, view.First, view.Last);
                    if ((view.Last - view.First) % 2 == 1)
                    {
                        view.Insert(1, 666);
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
                Assert.IsTrue(IC.Eq(list, 2, 4, 8, 666, 13, 6, 1, 666, 2, 666, 7));
            }


            [Test]
            public void SyncRoot()
            {
                Assert.AreSame(((System.Collections.IList)view).SyncRoot, ((System.Collections.IList)list).SyncRoot);
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
                list = new LinkedList<int>();
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
                list = null;
                views = null;
            }
            [Test]
            public void Insert()
            {
                Assert.IsTrue(list.Check(), "list check before insert");
                list.Insert(3, 777);
                Assert.IsTrue(list.Check(), "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i < 3 || (i == 3 && j == 0) ? i : i + 1, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i < 3 && i + j > 3 ? j + 1 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }
            [Test]
            public void RemoveAt()
            {
                Assert.IsTrue(list.Check(), "list check before remove");
                list.RemoveAt(3);
                Assert.IsTrue(list.Check(), "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i <= 3 ? i : i - 1, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i <= 3 && i + j > 3 ? j - 1 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }

            [Test]
            public void RemoveInterval()
            {
                Assert.IsTrue(list.Check(), "list check before remove");
                list.RemoveInterval(3, 2);
                Assert.IsTrue(list.Check(), "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i <= 3 ? i : i <= 5 ? 3 : i - 2, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(j == 0 ? 0 : i <= 3 && i + j > 4 ? j - 2 : i > 4 || i + j <= 3 ? j : j - 1, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }


            [Test]
            public void InsertAtEnd()
            {
                Assert.IsTrue(list.Check(), "list check before insert");
                list.InsertLast(777);
                Assert.IsTrue(list.Check(), "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }
            [Test]
            public void RemoveAtEnd()
            {
                Assert.IsTrue(list.Check(), "list check before remove");
                list.RemoveAt(5);
                Assert.IsTrue(list.Check(), "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i <= 5 ? i : i - 1, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i <= 5 && i + j > 5 ? j - 1 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }
            [Test]
            public void InsertAtStart()
            {
                Assert.IsTrue(list.Check(), "list check before insert");
                list.Insert(0, 777);
                Assert.IsTrue(list.Check(), "list check after insert");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i == 0 && j == 0 ? 0 : i + 1, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }
            [Test]
            public void RemoveAtStart()
            {
                Assert.IsTrue(list.Check(), "list check before remove");
                list.RemoveAt(0);
                Assert.IsTrue(list.Check(), "list check after remove");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i == 0 ? i : i - 1, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i == 0 && j > 0 ? j - 1 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }
            [Test]
            public void Clear()
            {
                Assert.IsTrue(list.Check(), "list check before clear");
                views[2][3].Clear();
                Assert.IsTrue(list.Check(), "list check after clear");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i < 2 ? i : i < 6 ? 2 : i - 3, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(s(i, j), views[i][j].Count, "view[" + i + "][" + j + "] count");
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
                LinkedList<int> list2 = new LinkedList<int>();
                for (int i = 0; i < 5; i++) { list2.Add(100 + i); }
                Assert.IsTrue(list.Check(), "list check before insertAll");
                list.InsertAll(3, list2);
                Assert.IsTrue(list.Check(), "list check after insertAll");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i < 3 || (i == 3 && j == 0) ? i : i + 5, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i < 3 && i + j > 3 ? j + 5 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }

            [Test]
            public void AddAll()
            {
                LinkedList<int> list2 = new LinkedList<int>();
                for (int i = 0; i < 5; i++) { list2.Add(100 + i); }
                Assert.IsTrue(list.Check(), "list check before AddAll");
                list.View(1, 2).AddAll(list2);
                Assert.IsTrue(list.Check(), "list check after AddAll");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        Assert.AreEqual(i < 3 || (i == 3 && j == 0) ? i : i + 5, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
                        Assert.AreEqual(i < 3 && i + j > 3 ? j + 5 : j, views[i][j].Count, "view[" + i + "][" + j + "] count");
                    }
                }
            }

            [Test]
            public void RemoveAll1()
            {
                LinkedList<int> list2 = new LinkedList<int>() { 1, 3, 4 };

                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        list = new LinkedList<int>();
                        for (int k = 0; k < 6; k++)
                        {
                            list.Add(k);
                        }

                        _ = (LinkedList<int>)list.View(i, j);
                        list.RemoveAll(list2);
                        Assert.IsTrue(list.Check(), "list check after RemoveAll, i=" + i + ", j=" + j);
                    }
                }
            }
            [Test]
            public void RemoveAll2()
            {
                LinkedList<int> list2 = new LinkedList<int>() { 1, 3, 4 };
                Assert.IsTrue(list.Check(), "list check before RemoveAll");
                list.RemoveAll(list2);

                Assert.AreEqual(0, views[0][0].Offset, "view [0][0] offset");
                Assert.AreEqual(0, views[0][1].Offset, "view [0][1] offset");
                Assert.AreEqual(0, views[0][2].Offset, "view [0][2] offset");
                Assert.AreEqual(0, views[0][3].Offset, "view [0][3] offset");
                Assert.AreEqual(0, views[0][4].Offset, "view [0][4] offset");
                Assert.AreEqual(0, views[0][5].Offset, "view [0][5] offset");
                Assert.AreEqual(0, views[0][6].Offset, "view [0][6] offset");
                Assert.AreEqual(1, views[1][0].Offset, "view [1][0] offset");
                Assert.AreEqual(1, views[1][1].Offset, "view [1][1] offset");
                Assert.AreEqual(1, views[1][2].Offset, "view [1][2] offset");
                Assert.AreEqual(1, views[1][3].Offset, "view [1][3] offset");
                Assert.AreEqual(1, views[1][4].Offset, "view [1][4] offset");
                Assert.AreEqual(1, views[1][5].Offset, "view [1][5] offset");
                Assert.AreEqual(1, views[2][0].Offset, "view [2][0] offset");
                Assert.AreEqual(1, views[2][1].Offset, "view [2][1] offset");
                Assert.AreEqual(1, views[2][2].Offset, "view [2][2] offset");
                Assert.AreEqual(1, views[2][3].Offset, "view [2][3] offset");
                Assert.AreEqual(1, views[2][4].Offset, "view [2][4] offset");
                Assert.AreEqual(2, views[3][0].Offset, "view [3][0] offset");
                Assert.AreEqual(2, views[3][1].Offset, "view [3][1] offset");
                Assert.AreEqual(2, views[3][2].Offset, "view [3][2] offset");
                Assert.AreEqual(2, views[3][3].Offset, "view [3][3] offset");
                Assert.AreEqual(2, views[4][0].Offset, "view [4][0] offset");
                Assert.AreEqual(2, views[4][1].Offset, "view [4][1] offset");
                Assert.AreEqual(2, views[4][2].Offset, "view [4][2] offset");
                Assert.AreEqual(2, views[5][0].Offset, "view [5][0] offset");
                Assert.AreEqual(2, views[5][1].Offset, "view [5][1] offset");
                Assert.AreEqual(3, views[6][0].Offset, "view [6][0] offset");

                Assert.AreEqual(0, views[0][0].Count, "view [0][0] count");
                Assert.AreEqual(1, views[0][1].Count, "view [0][1] count");
                Assert.AreEqual(1, views[0][2].Count, "view [0][2] count");
                Assert.AreEqual(2, views[0][3].Count, "view [0][3] count");
                Assert.AreEqual(2, views[0][4].Count, "view [0][4] count");
                Assert.AreEqual(2, views[0][5].Count, "view [0][5] count");
                Assert.AreEqual(3, views[0][6].Count, "view [0][6] count");
                Assert.AreEqual(0, views[1][0].Count, "view [1][0] count");
                Assert.AreEqual(0, views[1][1].Count, "view [1][1] count");
                Assert.AreEqual(1, views[1][2].Count, "view [1][2] count");
                Assert.AreEqual(1, views[1][3].Count, "view [1][3] count");
                Assert.AreEqual(1, views[1][4].Count, "view [1][4] count");
                Assert.AreEqual(2, views[1][5].Count, "view [1][5] count");
                Assert.AreEqual(0, views[2][0].Count, "view [2][0] count");
                Assert.AreEqual(1, views[2][1].Count, "view [2][1] count");
                Assert.AreEqual(1, views[2][2].Count, "view [2][2] count");
                Assert.AreEqual(1, views[2][3].Count, "view [2][3] count");
                Assert.AreEqual(2, views[2][4].Count, "view [2][4] count");
                Assert.AreEqual(0, views[3][0].Count, "view [3][0] count");
                Assert.AreEqual(0, views[3][1].Count, "view [3][1] count");
                Assert.AreEqual(0, views[3][2].Count, "view [3][2] count");
                Assert.AreEqual(1, views[3][3].Count, "view [3][3] count");
                Assert.AreEqual(0, views[4][0].Count, "view [4][0] count");
                Assert.AreEqual(0, views[4][1].Count, "view [4][1] count");
                Assert.AreEqual(1, views[4][2].Count, "view [4][2] count");
                Assert.AreEqual(0, views[5][0].Count, "view [5][0] count");
                Assert.AreEqual(1, views[5][1].Count, "view [5][1] count");
                Assert.AreEqual(0, views[6][0].Count, "view [6][0] count");

                Assert.IsTrue(list.Check(), "list check after RemoveAll");
            }

            [Test]
            public void RetainAll()
            {
                LinkedList<int> list2 = new LinkedList<int>() { 2, 4, 5 };
                Assert.IsTrue(list.Check(), "list check before RetainAll");
                list.RetainAll(list2);
                Assert.AreEqual(0, views[0][0].Offset, "view [0][0] offset");
                Assert.AreEqual(0, views[0][1].Offset, "view [0][1] offset");
                Assert.AreEqual(0, views[0][2].Offset, "view [0][2] offset");
                Assert.AreEqual(0, views[0][3].Offset, "view [0][3] offset");
                Assert.AreEqual(0, views[0][4].Offset, "view [0][4] offset");
                Assert.AreEqual(0, views[0][5].Offset, "view [0][5] offset");
                Assert.AreEqual(0, views[0][6].Offset, "view [0][6] offset");
                Assert.AreEqual(0, views[1][0].Offset, "view [1][0] offset");
                Assert.AreEqual(0, views[1][1].Offset, "view [1][1] offset");
                Assert.AreEqual(0, views[1][2].Offset, "view [1][2] offset");
                Assert.AreEqual(0, views[1][3].Offset, "view [1][3] offset");
                Assert.AreEqual(0, views[1][4].Offset, "view [1][4] offset");
                Assert.AreEqual(0, views[1][5].Offset, "view [1][5] offset");
                Assert.AreEqual(0, views[2][0].Offset, "view [2][0] offset");
                Assert.AreEqual(0, views[2][1].Offset, "view [2][1] offset");
                Assert.AreEqual(0, views[2][2].Offset, "view [2][2] offset");
                Assert.AreEqual(0, views[2][3].Offset, "view [2][3] offset");
                Assert.AreEqual(0, views[2][4].Offset, "view [2][4] offset");
                Assert.AreEqual(1, views[3][0].Offset, "view [3][0] offset");
                Assert.AreEqual(1, views[3][1].Offset, "view [3][1] offset");
                Assert.AreEqual(1, views[3][2].Offset, "view [3][2] offset");
                Assert.AreEqual(1, views[3][3].Offset, "view [3][3] offset");
                Assert.AreEqual(1, views[4][0].Offset, "view [4][0] offset");
                Assert.AreEqual(1, views[4][1].Offset, "view [4][1] offset");
                Assert.AreEqual(1, views[4][2].Offset, "view [4][2] offset");
                Assert.AreEqual(2, views[5][0].Offset, "view [5][0] offset");
                Assert.AreEqual(2, views[5][1].Offset, "view [5][1] offset");
                Assert.AreEqual(3, views[6][0].Offset, "view [6][0] offset");

                Assert.AreEqual(0, views[0][0].Count, "view [0][0] count");
                Assert.AreEqual(0, views[0][1].Count, "view [0][1] count");
                Assert.AreEqual(0, views[0][2].Count, "view [0][2] count");
                Assert.AreEqual(1, views[0][3].Count, "view [0][3] count");
                Assert.AreEqual(1, views[0][4].Count, "view [0][4] count");
                Assert.AreEqual(2, views[0][5].Count, "view [0][5] count");
                Assert.AreEqual(3, views[0][6].Count, "view [0][6] count");
                Assert.AreEqual(0, views[1][0].Count, "view [1][0] count");
                Assert.AreEqual(0, views[1][1].Count, "view [1][1] count");
                Assert.AreEqual(1, views[1][2].Count, "view [1][2] count");
                Assert.AreEqual(1, views[1][3].Count, "view [1][3] count");
                Assert.AreEqual(2, views[1][4].Count, "view [1][4] count");
                Assert.AreEqual(3, views[1][5].Count, "view [1][5] count");
                Assert.AreEqual(0, views[2][0].Count, "view [2][0] count");
                Assert.AreEqual(1, views[2][1].Count, "view [2][1] count");
                Assert.AreEqual(1, views[2][2].Count, "view [2][2] count");
                Assert.AreEqual(2, views[2][3].Count, "view [2][3] count");
                Assert.AreEqual(3, views[2][4].Count, "view [2][4] count");
                Assert.AreEqual(0, views[3][0].Count, "view [3][0] count");
                Assert.AreEqual(0, views[3][1].Count, "view [3][1] count");
                Assert.AreEqual(1, views[3][2].Count, "view [3][2] count");
                Assert.AreEqual(2, views[3][3].Count, "view [3][3] count");
                Assert.AreEqual(0, views[4][0].Count, "view [4][0] count");
                Assert.AreEqual(1, views[4][1].Count, "view [4][1] count");
                Assert.AreEqual(2, views[4][2].Count, "view [4][2] count");
                Assert.AreEqual(0, views[5][0].Count, "view [5][0] count");
                Assert.AreEqual(1, views[5][1].Count, "view [5][1] count");
                Assert.AreEqual(0, views[6][0].Count, "view [6][0] count");


                Assert.IsTrue(list.Check(), "list check after RetainAll");
            }

            [Test]
            public void RemoveAllCopies()
            {
                LinkedList<int> list2 = new LinkedList<int>() { 0, 2, 2, 2, 5, 2, 1 };
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7 - i; j++)
                    {
                        list = new LinkedList<int>();
                        list.AddAll(list2);
                        LinkedList<int> v = (LinkedList<int>)list.View(i, j);
                        list.RemoveAllCopies(2);
                        Assert.AreEqual(i == 0 ? 0 : i <= 4 ? 1 : i <= 6 ? 2 : 3, v.Offset, "v.Offset, i=" + i + ", j=" + j);
                        Assert.AreEqual((i == 0 && j > 0 ? 1 : 0) + (i <= 4 && i + j > 4 ? 1 : 0) + (i <= 6 && i + j > 6 ? 1 : 0), v.Count, "v.Count, i=" + i + ", j=" + j);
                        Assert.IsTrue(list.Check(), "list check after RemoveAllCopies, i=" + i + ", j=" + j);
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
                            Assert.AreEqual(j, views[i][j].Count, "view[" + i + "][" + j + "] size");
                            if (reverse && ((j > 0 && start <= i && start + count >= i + j) || (j == 0 && start < i && start + count > i)))
                            {
                                Assert.AreEqual(start + (start + count - i - j), views[i][j].Offset, "view[" + i + "][" + j + "] offset (mirrored)");
                            }
                            else
                            {
                                Assert.AreEqual(i, views[i][j].Offset, "view[" + i + "][" + j + "] offset");
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
                Assert.IsTrue(list.Check(), "list check before Reverse");
                list2.Reverse();
                Assert.IsTrue(list.Check(), "list check after Reverse");
                checkDisposed(true, start, count);
            }
            [Test]
            public void Sort()
            {
                int start = 2, count = 3;
                IList<int> list2 = list.View(start, count);
                Assert.IsTrue(list.Check(), "list check before Sort");
                list2.Sort();
                Assert.IsTrue(list.Check(), "list check after Sort");
                checkDisposed(false, start, count);
            }
            [Test]
            public void Shuffle()
            {
                int start = 2, count = 3;
                IList<int> list2 = list.View(start, count);
                Assert.IsTrue(list.Check(), "list check before Shuffle");
                list2.Shuffle();
                Assert.IsTrue(list.Check(), "list check after Shuffle");
                checkDisposed(false, start, count);
            }


        }


        [TestFixture]
        public class Validity
        {
            private IList<int> list;
            private IList<int> view;
            [SetUp]
            public void Init()
            {
                list = new LinkedList<int>();
                for (int i = 0; i < 6; i++)
                {
                    list.Add(i);
                }

                view = list.View(2, 3);
                view.Dispose();
            }
            [TearDown]
            public void Dispose()
            {
                list = null;
                view = null;
            }

            //Properties

            //
            /*ActiveEvents,
      AllowsDuplicates,
      ContainsSpeed,
      Count,
      CountSpeed,
      Direction,
      DuplicatesByCounting,
      FIFO,
      First,
      EqualityComparer,
      IsEmpty,
      IsReadOnly,
      this[int index],
      this[int start, int count],
      Last,
      Offset,
      SyncRoot,
      Underlying
      */
            [Test]
            public void Add()
            {
                Assert.Throws<ViewDisposedException>(() => view.Add(5));
            }

            [Test]
            public void AddAll_int_()
            {
                Assert.Throws<ViewDisposedException>(() => view.AddAll(new int[] { }));
            }

            [Test]
            public void AddAll()
            {
                Assert.Throws<ViewDisposedException>(() => view.AddAll(new int[] { }));
            }

            [Test]
            public void All()
            {
                Assert.Throws<ViewDisposedException>(() => view.All(delegate (int i) { return false; }));
            }

            [Test]
            public void Apply()
            {
                Assert.Throws<ViewDisposedException>(() => view.Apply(delegate (int i) { }));
            }

            [Test]
            public void Backwards()
            {
                Assert.Throws<ViewDisposedException>(() => view.Backwards());
            }

            [Test]
            public void Choose()
            {
                Assert.Throws<ViewDisposedException>(() => view.Choose());
            }

            [Test]
            public void Contains()
            {
                Assert.Throws<ViewDisposedException>(() => view.Contains(0));
            }

            [Test]
            public void Clear()
            {
                Assert.Throws<ViewDisposedException>(() => view.Clear());
            }

            [Test]
            public void ContainsAll()
            {
                Assert.Throws<ViewDisposedException>(() => view.ContainsAll(new int[] { }));
            }

            [Test]
            public void ContainsCount()
            {
                Assert.Throws<ViewDisposedException>(() => view.ContainsCount(0));
            }

            [Test]
            public void CopyTo()
            {
                Assert.Throws<ViewDisposedException>(() => view.CopyTo(new int[1], 0));
            }

            [Test]
            public void Dequeue()
            {
                Assert.Throws<ViewDisposedException>(() => ((LinkedList<int>)view).Dequeue());
            }

            [Test]
            public void Enqueue()
            {
                Assert.Throws<ViewDisposedException>(() => ((LinkedList<int>)view).Enqueue(0));
            }

            [Test]
            public void Exists()
            {
                Assert.Throws<ViewDisposedException>(() => view.Exists(delegate (int i) { return false; }));
            }

            [Test]
            public void Filter()
            {
                Assert.Throws<ViewDisposedException>(() => view.Filter(delegate (int i) { return true; }));
            }

            [Test]
            public void Find()
            {
                int i = 0;

                Assert.Throws<ViewDisposedException>(() => view.Find(ref i));
            }

            [Test]
            public void FindAll()
            {
                Assert.Throws<ViewDisposedException>(() => view.FindAll(delegate (int i) { return false; }));
            }

            [Test]
            public void FindOrAdd()
            {
                int i = 0;

                Assert.Throws<ViewDisposedException>(() => view.FindOrAdd(ref i));
            }

            //TODO: wonder if it is allright to wait with the exception till the enumerator is actually used?
            /*    [Test]
            [ExpectedException(typeof(ListDisposedException))]
            public void GetEnumerator()
            {
              view.GetEnumerator();
            }
          */
            /*Method overview
            Check(),
            checkRange(int start, int count),
            Dispose(),
            Equals(object obj),
            Finalize(),
            fireBagItemsAdded(T item, int count),
            fireBagItemsRemoved(T item, int count),
            fireCollectionChanged(),
            fireCollectionCleared(int start, int count),
            fireItemAdded(T item),
            fireItemInserted(T item, int index),
            fireItemRemoved(T item),
            fireItemRemovedAt(T item, int index),
            GetEnumerator(),
            GetHashCode(),
            GetSequencedHashCode(),
            GetType(),
            GetUnsequencedHashCode(),
            IndexOf(T item),
            Insert(int i, T item),
            ViewOf().InsertLast(T item, T target),
            InsertAll(int i, IEnumerable<T> items),
            ViewOf().InsertFirst(T item, T target),
            InsertFirst(T item),
            InsertLast(T item),
            IsSorted(SCG.IComparer<T> c),
            LastIndexOf(T item),
            LastViewOf(T item),
            Map<V>(Func<T,V> mapper),
            Map<V>(Func<T,V> mapper, SCG.IEqualityComparer<V> equalityComparer),
            MemberwiseClone(),
            modifycheck(int stamp),
            Pop(),
            Push(T item),
            Remove(),
            Remove(T item),
            Remove(T item, out T removeditem),
            RemoveAll(IEnumerable<T> items),
            RemoveAllCopies(T item),
            RemoveAt(int i),
            RemoveFirst(),
            RemoveInterval(int start, int count),
            RemoveLast(),
            RetainAll(IEnumerable<T> items),
            Reverse(),
            Reverse(int start, int count),
            SequencedEquals(ISequenced<T> that),
            Shuffle(),
            Shuffle(System.Random rnd),
            Slide(int offset),
            Slide(int offset, int size),
            Sort(),
            Sort(SCG.IComparer<T> c),
            ToArray(),
            ToString(),
            UnsequencedEquals(ICollection<T> that),
            Update(T item),
            Update(T item, out T olditem),
            updatecheck(),
            UpdateOrAdd(T item),
            UpdateOrAdd(T item, out T olditem),
            View(int start, int count),
            ViewOf(T item)
                  */
        }
    }




    namespace LinkedListOfTreesORLists
    {
        [TestFixture]
        public class MultiLevelUnorderedOfUnOrdered
        {
            private ICollection<int> dit, dat, dut;

            private ICollection<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
                dat = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new LinkedList<int>();
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
                Assert.IsTrue(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dit.UnsequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Assert.IsTrue(Dit.UnsequencedEquals(Dat));
                Assert.IsFalse(Dit.UnsequencedEquals(Dut));
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
            private ICollection<int> dit, dat, dut;

            private ISequenced<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
                dat = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dut = new LinkedList<int>();
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
                Assert.IsTrue(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dit.UnsequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dit); Dut.Add(dut); Dut.Add(dat);
                Assert.IsFalse(Dit.SequencedEquals(Dat));
                Assert.IsTrue(Dit.SequencedEquals(Dut));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = null;
                Dit = Dat = Dut = null;
            }
        }



        [TestFixture]
        public class MultiLevelUnOrderedOfOrdered
        {
            private ISequenced<int> dit, dat, dut, dot;

            private ICollection<ISequenced<int>> Dit, Dat, Dut, Dot;


            [SetUp]
            public void Init()
            {
                dit = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
                dot = new LinkedList<int>();
                dit.Add(2); dit.Add(1);
                dat.Add(2); dat.Add(1);
                dut.Add(3);
                dot.Add(1); dot.Add(2);
                Dit = new LinkedList<ISequenced<int>>();
                Dat = new LinkedList<ISequenced<int>>();
                Dut = new LinkedList<ISequenced<int>>();
                Dot = new LinkedList<ISequenced<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsTrue(dit.SequencedEquals(dot));
                Assert.IsFalse(dit.SequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.IsTrue(Dit.UnsequencedEquals(Dut));
                Assert.IsFalse(Dit.UnsequencedEquals(Dat));
                Assert.IsTrue(Dit.UnsequencedEquals(Dot));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = dot = null;
                Dit = Dat = Dut = Dot = null;
            }
        }



        [TestFixture]
        public class MultiLevelOrderedOfOrdered
        {
            private ISequenced<int> dit, dat, dut, dot;

            private ISequenced<ISequenced<int>> Dit, Dat, Dut, Dot;


            [SetUp]
            public void Init()
            {
                dit = new TreeSet<int>(SCG.Comparer<int>.Default, EqualityComparer<int>.Default);
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
                dot = new LinkedList<int>();
                dit.Add(2); dit.Add(1);
                dat.Add(2); dat.Add(1);
                dut.Add(3);
                dot.Add(1); dot.Add(2);
                Dit = new LinkedList<ISequenced<int>>();
                Dat = new LinkedList<ISequenced<int>>();
                Dut = new LinkedList<ISequenced<int>>();
                Dot = new LinkedList<ISequenced<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsTrue(dit.SequencedEquals(dot));
                Assert.IsFalse(dit.SequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.IsTrue(Dit.SequencedEquals(Dut));
                Assert.IsFalse(Dit.SequencedEquals(Dat));
                Assert.IsFalse(Dit.SequencedEquals(Dot));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = dot = null;
                Dit = Dat = Dut = Dot = null;
            }
        }
    }




    namespace HashingAndEquals
    {
        [TestFixture]
        public class ISequenced
        {
            private ISequenced<int> dit, dat, dut;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
            }


            [Test]
            public void EmptyEmpty()
            {
                Assert.IsTrue(dit.SequencedEquals(dat));
            }


            [Test]
            public void EmptyNonEmpty()
            {
                dit.Add(3);
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsFalse(dat.SequencedEquals(dit));
            }

            [Test]
            public void HashVal()
            {
                Assert.AreEqual(CHC.SequencedHashCode(), dit.GetSequencedHashCode());
                dit.Add(3);
                Assert.AreEqual(CHC.SequencedHashCode(3), dit.GetSequencedHashCode());
                dit.Add(7);
                Assert.AreEqual(CHC.SequencedHashCode(3, 7), dit.GetSequencedHashCode());
                Assert.AreEqual(CHC.SequencedHashCode(), dut.GetSequencedHashCode());
                dut.Add(7);
                Assert.AreEqual(CHC.SequencedHashCode(7), dut.GetSequencedHashCode());
                dut.Add(3);
                Assert.AreEqual(CHC.SequencedHashCode(7, 3), dut.GetSequencedHashCode());
            }


            [Test]
            public void EqualHashButDifferent()
            {
                dit.Add(0); dit.Add(31);
                dat.Add(1); dat.Add(0);
                Assert.AreEqual(dit.GetSequencedHashCode(), dat.GetSequencedHashCode());
                Assert.IsFalse(dit.SequencedEquals(dat));
            }


            [Test]
            public void Normal()
            {
                dit.Add(3);
                dit.Add(7);
                dat.Add(3);
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsFalse(dat.SequencedEquals(dit));
                dat.Add(7);
                Assert.IsTrue(dit.SequencedEquals(dat));
                Assert.IsTrue(dat.SequencedEquals(dit));
            }


            [Test]
            public void WrongOrder()
            {
                dit.Add(3);
                dut.Add(3);
                Assert.IsTrue(dit.SequencedEquals(dut));
                Assert.IsTrue(dut.SequencedEquals(dit));
                dit.Add(7);
                ((LinkedList<int>)dut).InsertFirst(7);
                Assert.IsFalse(dit.SequencedEquals(dut));
                Assert.IsFalse(dut.SequencedEquals(dit));
            }


            [Test]
            public void Reflexive()
            {
                Assert.IsTrue(dit.SequencedEquals(dit));
                dit.Add(3);
                Assert.IsTrue(dit.SequencedEquals(dit));
                dit.Add(7);
                Assert.IsTrue(dit.SequencedEquals(dit));
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
                dit = new LinkedList<int>();
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
            }


            [Test]
            public void EmptyEmpty()
            {
                Assert.IsTrue(dit.UnsequencedEquals(dat));
            }


            [Test]
            public void EmptyNonEmpty()
            {
                dit.Add(3);
                Assert.IsFalse(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dat.UnsequencedEquals(dit));
            }


            [Test]
            public void HashVal()
            {
                Assert.AreEqual(CHC.UnsequencedHashCode(), dit.GetUnsequencedHashCode());
                dit.Add(3);
                Assert.AreEqual(CHC.UnsequencedHashCode(3), dit.GetUnsequencedHashCode());
                dit.Add(7);
                Assert.AreEqual(CHC.UnsequencedHashCode(3, 7), dit.GetUnsequencedHashCode());
                Assert.AreEqual(CHC.UnsequencedHashCode(), dut.GetUnsequencedHashCode());
                dut.Add(3);
                Assert.AreEqual(CHC.UnsequencedHashCode(3), dut.GetUnsequencedHashCode());
                dut.Add(7);
                Assert.AreEqual(CHC.UnsequencedHashCode(7, 3), dut.GetUnsequencedHashCode());
            }


            [Test]
            public void EqualHashButDifferent()
            {
                dit.Add(-1657792980); dit.Add(-1570288808);
                dat.Add(1862883298); dat.Add(-272461342);
                Assert.AreEqual(dit.GetUnsequencedHashCode(), dat.GetUnsequencedHashCode());
                Assert.IsFalse(dit.UnsequencedEquals(dat));
            }


            [Test]
            public void Normal()
            {
                dit.Add(3);
                dit.Add(7);
                dat.Add(3);
                Assert.IsFalse(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dat.UnsequencedEquals(dit));
                dat.Add(7);
                Assert.IsTrue(dit.UnsequencedEquals(dat));
                Assert.IsTrue(dat.UnsequencedEquals(dit));
            }


            [Test]
            public void WrongOrder()
            {
                dit.Add(3);
                dut.Add(3);
                Assert.IsTrue(dit.UnsequencedEquals(dut));
                Assert.IsTrue(dut.UnsequencedEquals(dit));
                dit.Add(7);
                dut.Add(7);
                Assert.IsTrue(dit.UnsequencedEquals(dut));
                Assert.IsTrue(dut.UnsequencedEquals(dit));
            }


            [Test]
            public void Reflexive()
            {
                Assert.IsTrue(dit.UnsequencedEquals(dit));
                dit.Add(3);
                Assert.IsTrue(dit.UnsequencedEquals(dit));
                dit.Add(7);
                Assert.IsTrue(dit.UnsequencedEquals(dit));
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
                dit = new LinkedList<int>();
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
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
                Assert.IsTrue(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dit.UnsequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Assert.IsTrue(Dit.UnsequencedEquals(Dat));
                Assert.IsFalse(Dit.UnsequencedEquals(Dut));
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
            private ICollection<int> dit, dat, dut;

            private ISequenced<ICollection<int>> Dit, Dat, Dut;


            [SetUp]
            public void Init()
            {
                dit = new LinkedList<int>();
                dat = new LinkedList<int>();
                dut = new LinkedList<int>();
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
                Assert.IsTrue(dit.UnsequencedEquals(dat));
                Assert.IsFalse(dit.UnsequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dit); Dut.Add(dut); Dut.Add(dat);
                Assert.IsFalse(Dit.SequencedEquals(Dat));
                Assert.IsTrue(Dit.SequencedEquals(Dut));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = null;
                Dit = Dat = Dut = null;
            }
        }



        [TestFixture]
        public class MultiLevelUnOrderedOfOrdered
        {
            private ISequenced<int> dit, dat, dut, dot;

            private ICollection<ISequenced<int>> Dit, Dat, Dut, Dot;


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
                Dit = new LinkedList<ISequenced<int>>();
                Dat = new LinkedList<ISequenced<int>>();
                Dut = new LinkedList<ISequenced<int>>();
                Dot = new LinkedList<ISequenced<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsTrue(dit.SequencedEquals(dot));
                Assert.IsFalse(dit.SequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.IsTrue(Dit.UnsequencedEquals(Dut));
                Assert.IsFalse(Dit.UnsequencedEquals(Dat));
                Assert.IsTrue(Dit.UnsequencedEquals(Dot));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = dot = null;
                Dit = Dat = Dut = Dot = null;
            }
        }



        [TestFixture]
        public class MultiLevelOrderedOfOrdered
        {
            private ISequenced<int> dit, dat, dut, dot;

            private ISequenced<ISequenced<int>> Dit, Dat, Dut, Dot;


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
                Dit = new LinkedList<ISequenced<int>>();
                Dat = new LinkedList<ISequenced<int>>();
                Dut = new LinkedList<ISequenced<int>>();
                Dot = new LinkedList<ISequenced<int>>();
            }


            [Test]
            public void Check()
            {
                Assert.IsFalse(dit.SequencedEquals(dat));
                Assert.IsTrue(dit.SequencedEquals(dot));
                Assert.IsFalse(dit.SequencedEquals(dut));
            }


            [Test]
            public void Multi()
            {
                Dit.Add(dit); Dit.Add(dut); Dit.Add(dit);
                Dat.Add(dut); Dat.Add(dit); Dat.Add(dat);
                Dut.Add(dot); Dut.Add(dut); Dut.Add(dit);
                Dot.Add(dit); Dot.Add(dit); Dot.Add(dut);
                Assert.IsTrue(Dit.SequencedEquals(Dut));
                Assert.IsFalse(Dit.SequencedEquals(Dat));
                Assert.IsFalse(Dit.SequencedEquals(Dot));
            }


            [TearDown]
            public void Dispose()
            {
                dit = dat = dut = dot = null;
                Dit = Dat = Dut = Dot = null;
            }
        }
    }
}