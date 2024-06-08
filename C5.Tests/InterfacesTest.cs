// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;


namespace C5.Tests.interfaces
{
    [TestFixture]
    public class ICollectionsTests
    {
        public void TryC5Coll(ICollection<double> coll)
        {
            Assert.That(coll, Is.Empty);
            double[] arr = [];
            coll.CopyTo(arr, 0);
            Assert.That(coll.IsReadOnly, Is.False);
            coll.Add(2.3);
            coll.Add(3.2);
            Assert.That(coll, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(coll.Contains(2.3), Is.True);
                Assert.That(coll.Contains(3.1), Is.False);
                Assert.That(coll.Remove(3.1), Is.False);
                Assert.That(coll.Remove(3.2), Is.True);
            });
            Assert.That(coll.Contains(3.1), Is.False);
            Assert.That(coll, Has.Count.EqualTo(1));
            coll.Clear();
            Assert.That(coll, Is.Empty);
            Assert.That(coll.Remove(3.1), Is.False);
        }

        public void TrySCGColl(SCG.ICollection<double> coll)
        {
            // All members of SCG.ICollection<T>
            Assert.That(coll, Is.Empty);
            double[] arr = [];
            coll.CopyTo(arr, 0);
            Assert.That(coll.IsReadOnly, Is.False);
            coll.Add(2.3);
            coll.Add(3.2);
            Assert.That(coll, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(coll.Contains(2.3), Is.True);
                Assert.That(coll.Contains(3.1), Is.False);
                Assert.That(coll.Remove(3.1), Is.False);
                Assert.That(coll.Remove(3.2), Is.True);
            });
            Assert.That(coll.Contains(3.1), Is.False);
            Assert.That(coll, Has.Count.EqualTo(1));
            coll.Clear();
            Assert.That(coll, Is.Empty);
            Assert.That(coll.Remove(3.1), Is.False);
        }

        public void TryBothColl(ICollection<double> coll)
        {
            TryC5Coll(coll);
            TrySCGColl(coll);
        }


        [Test]
        public void Test1()
        {
            TryBothColl(new HashSet<double>());
            TryBothColl(new HashBag<double>());
            TryBothColl(new TreeSet<double>());
            TryBothColl(new TreeBag<double>());
            TryBothColl(new ArrayList<double>());
            TryBothColl(new LinkedList<double>());
            TryBothColl(new HashedArrayList<double>());
            TryBothColl(new HashedLinkedList<double>());
            TryBothColl(new SortedArray<double>());
        }
    }

    [TestFixture]
    public class SCIListTests
    {
        private class A { }

        private class B : A { }

        private class C : B { }

        public void TrySCIList(System.Collections.IList list)
        {
            // Should be called with a C5.IList<B> which is not a WrappedArray
            Assert.That(list, Is.Empty);
            list.CopyTo(Array.Empty<A>(), 0);
            list.CopyTo(Array.Empty<B>(), 0);
            list.CopyTo(Array.Empty<C>(), 0);
            Assert.Multiple(() =>
            {
                Assert.That(!list.IsFixedSize, Is.True);
                Assert.That(list.IsReadOnly, Is.False);
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
            });
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            Assert.Multiple(() =>
            {
                Assert.That(list.Add(b1), Is.EqualTo(0));
                Assert.That(list.Add(c1), Is.EqualTo(1));
                Assert.That(list, Has.Count.EqualTo(2));
            });
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(c1), Is.True);
                Assert.That(list.Contains(b2), Is.False);
            });
            list[0] = b2;
            Assert.That(list[0], Is.EqualTo(b2));
            list[1] = c2;
            Assert.Multiple(() =>
            {
                Assert.That(list[1], Is.EqualTo(c2));
                Assert.That(list.Contains(b2), Is.True);
                Assert.That(list.Contains(c2), Is.True);
            });
            Array arrA = new A[2], arrB = new B[2];
            list.CopyTo(arrA, 0);
            list.CopyTo(arrB, 0);
            Assert.Multiple(() =>
            {
                Assert.That(arrA.GetValue(0), Is.EqualTo(b2));
                Assert.That(arrB.GetValue(0), Is.EqualTo(b2));
                Assert.That(arrA.GetValue(1), Is.EqualTo(c2));
                Assert.That(arrB.GetValue(1), Is.EqualTo(c2));
                Assert.That(list.IndexOf(b2), Is.EqualTo(0));
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
            });
            list.Remove(b1);
            list.Remove(b2);
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(b2), Is.False);
                Assert.That(list, Has.Count.EqualTo(1)); // Contains c2 only
            });
            list.Insert(0, b2);
            list.Insert(2, b1);
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(b2));
                Assert.That(list[1], Is.EqualTo(c2));
                Assert.That(list[2], Is.EqualTo(b1));
            });
            list.Remove(c2);
            Assert.Multiple(() =>
            {
                Assert.That(list[0], Is.EqualTo(b2));
                Assert.That(list[1], Is.EqualTo(b1));
            });
            list.RemoveAt(1);
            Assert.That(list[0], Is.EqualTo(b2));
            list.Clear();
            Assert.That(list, Is.Empty);
            list.Remove(b1);
        }

        [Test]
        public void Test1()
        {
            TrySCIList(new ArrayList<B>());
            TrySCIList(new HashedArrayList<B>());
            TrySCIList(new LinkedList<B>());
            TrySCIList(new HashedLinkedList<B>());
        }

        [Test]
        public void TryWrappedArrayAsSCIList1()
        {
            B[] myarray = [new B(), new B(), new C()];
            System.Collections.IList list = new WrappedArray<B>(myarray);
            // Should be called with a three-element WrappedArray<B>
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list.IsFixedSize, Is.True);
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
            });
            Assert.That(list.SyncRoot, Is.EqualTo(myarray.SyncRoot));
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            list[0] = b2;
            Assert.That(list[0], Is.EqualTo(b2));
            list[1] = c2;
            Assert.Multiple(() =>
            {
                Assert.That(list[1], Is.EqualTo(c2));
                Assert.That(list.Contains(b2), Is.True);
                Assert.That(list.Contains(c2), Is.True);
            });
            Array arrA = new A[3], arrB = new B[3];
            list.CopyTo(arrA, 0);
            list.CopyTo(arrB, 0);
            Assert.Multiple(() =>
            {
                Assert.That(arrA.GetValue(0), Is.EqualTo(b2));
                Assert.That(arrB.GetValue(0), Is.EqualTo(b2));
                Assert.That(arrA.GetValue(1), Is.EqualTo(c2));
                Assert.That(arrB.GetValue(1), Is.EqualTo(c2));
                Assert.That(list.IndexOf(b2), Is.EqualTo(0));
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
                Assert.That(list.IndexOf(c1), Is.EqualTo(-1));
                Assert.That(list.Contains(b1), Is.False);
                Assert.That(list.Contains(c1), Is.False);
            });
        }

        [Test]
        public void TryWrappedArrayAsSCIList2()
        {
            B[] myarray = [];
            System.Collections.IList list = new WrappedArray<B>(myarray);
            // Should be called with an empty WrappedArray<B>
            Assert.That(list, Is.Empty);
            list.CopyTo(Array.Empty<A>(), 0);
            list.CopyTo(Array.Empty<B>(), 0);
            list.CopyTo(Array.Empty<C>(), 0);
            Assert.Multiple(() =>
            {
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
            });
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(b2), Is.False);
                Assert.That(list.Contains(c2), Is.False);
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
                Assert.That(list.IndexOf(c1), Is.EqualTo(-1));
            });
        }

        [Test]
        public void TryGuardedListAsSCIList1()
        {
            B b1_ = new(), b2_ = new();
            C c1_ = new(), c2_ = new();
            ArrayList<B> mylist = new();
            mylist.AddAll([b1_, b2_, c1_]);
            System.Collections.IList list = new GuardedList<B>(mylist);
            object b1 = b1_, b2 = b2_, c1 = c1_, c2 = c2_;
            // Should be called with a three-element GuardedList<B>
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list.IsFixedSize, Is.True);
                Assert.That(list.IsReadOnly, Is.True);
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
                Assert.That(((System.Collections.IList)mylist).SyncRoot, Is.EqualTo(list.SyncRoot));
                Assert.That(list.Contains(b1), Is.True);
                Assert.That(list.Contains(b2), Is.True);
                Assert.That(list.Contains(c1), Is.True);
                Assert.That(list.Contains(c2), Is.False);
            });
            Array arrA = new A[3], arrB = new B[3];
            list.CopyTo(arrA, 0);
            list.CopyTo(arrB, 0);
            Assert.Multiple(() =>
            {
                Assert.That(arrA.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrB.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrA.GetValue(1), Is.EqualTo(b2));
                Assert.That(arrB.GetValue(1), Is.EqualTo(b2));
                Assert.That(list.IndexOf(b1), Is.EqualTo(0));
                Assert.That(list.IndexOf(c2), Is.EqualTo(-1));
            });
        }

        [Test]
        public void TryGuardedListAsSCIList2()
        {
            System.Collections.IList list = new GuardedList<B>(new ArrayList<B>());
            // Should be called with an empty GuardedList<B>
            Assert.That(list, Is.Empty);
            list.CopyTo(Array.Empty<A>(), 0);
            list.CopyTo(Array.Empty<B>(), 0);
            list.CopyTo(Array.Empty<C>(), 0);
            Assert.Multiple(() =>
            {
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
            });
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(b2), Is.False);
                Assert.That(list.Contains(c2), Is.False);
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
                Assert.That(list.IndexOf(c1), Is.EqualTo(-1));
            });
        }

        [Test]
        public void TryViewOfGuardedListAsSCIList1()
        {
            B b1_ = new(), b2_ = new();
            C c1_ = new(), c2_ = new();
            ArrayList<B> mylist = new();
            mylist.AddAll([new B(), b1_, b2_, c1_, new B()]);
            System.Collections.IList list = new GuardedList<B>(mylist).View(1, 3);
            object b1 = b1_, b2 = b2_, c1 = c1_, c2 = c2_;
            // Should be called with a three-element view of a GuardedList<B>
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list.IsFixedSize, Is.True);
                Assert.That(list.IsReadOnly, Is.True);
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
                Assert.That(((System.Collections.IList)mylist).SyncRoot, Is.EqualTo(list.SyncRoot));
                Assert.That(list.Contains(b1), Is.True);
                Assert.That(list.Contains(b2), Is.True);
                Assert.That(list.Contains(c1), Is.True);
                Assert.That(list.Contains(c2), Is.False);
            });
            Array arrA = new A[3], arrB = new B[3];
            list.CopyTo(arrA, 0);
            list.CopyTo(arrB, 0);
            Assert.Multiple(() =>
            {
                Assert.That(arrA.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrB.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrA.GetValue(1), Is.EqualTo(b2));
                Assert.That(arrB.GetValue(1), Is.EqualTo(b2));
                Assert.That(list.IndexOf(b1), Is.EqualTo(0));
                Assert.That(list.IndexOf(c2), Is.EqualTo(-1));
            });
        }

        [Test]
        public void TryViewOfGuardedListAsSCIList2()
        {
            System.Collections.IList list = new GuardedList<B>(new ArrayList<B>()).View(0, 0);
            Assert.That(list, Is.Empty);
            list.CopyTo(Array.Empty<A>(), 0);
            list.CopyTo(Array.Empty<B>(), 0);
            list.CopyTo(Array.Empty<C>(), 0);
            Assert.Multiple(() =>
            {
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
            });
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(b2), Is.False);
                Assert.That(list.Contains(c2), Is.False);
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
                Assert.That(list.IndexOf(c1), Is.EqualTo(-1));
            });
        }

        private void TryListViewAsSCIList1(IList<B> mylist)
        {
            B b1_ = new(), b2_ = new();
            C c1_ = new(), c2_ = new();
            mylist.AddAll([new B(), b1_, b2_, c1_, new B()]);
            System.Collections.IList list = mylist.View(1, 3);
            object b1 = b1_, b2 = b2_, c1 = c1_, c2 = c2_;
            // Should be called with a three-element view on ArrayList<B>
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
                Assert.That(mylist.SyncRoot, Is.EqualTo(list.SyncRoot));
                Assert.That(list.Contains(b1), Is.True);
                Assert.That(list.Contains(b2), Is.True);
                Assert.That(list.Contains(c1), Is.True);
                Assert.That(list.Contains(c2), Is.False);
            });
            Array arrA = new A[3], arrB = new B[3];
            list.CopyTo(arrA, 0);
            list.CopyTo(arrB, 0);
            Assert.Multiple(() =>
            {
                Assert.That(arrA.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrB.GetValue(0), Is.EqualTo(b1));
                Assert.That(arrA.GetValue(1), Is.EqualTo(b2));
                Assert.That(arrB.GetValue(1), Is.EqualTo(b2));
                Assert.That(list.IndexOf(b1), Is.EqualTo(0));
                Assert.That(list.IndexOf(c2), Is.EqualTo(-1));
            });
        }

        private void TryListViewAsSCIList2(IList<B> mylist)
        {
            System.Collections.IList list = mylist.View(0, 0);
            Assert.That(list, Is.Empty);
            list.CopyTo(Array.Empty<A>(), 0);
            list.CopyTo(Array.Empty<B>(), 0);
            list.CopyTo(Array.Empty<C>(), 0);
            Assert.Multiple(() =>
            {
                Assert.That(list.IsSynchronized, Is.False);
                Assert.That(list.SyncRoot, Is.Not.EqualTo(null));
                Assert.That(mylist.SyncRoot, Is.EqualTo(list.SyncRoot));
            });
            object b1 = new B(), b2 = new B(), c1 = new C(), c2 = new C();
            Assert.Multiple(() =>
            {
                Assert.That(list.Contains(b2), Is.False);
                Assert.That(list.Contains(c2), Is.False);
                Assert.That(list.IndexOf(b1), Is.EqualTo(-1));
                Assert.That(list.IndexOf(c1), Is.EqualTo(-1));
            });
        }

        [Test]
        public void TryArrayListViewAsSCIList()
        {
            TryListViewAsSCIList1(new ArrayList<B>());
            TryListViewAsSCIList2(new ArrayList<B>());
        }

        [Test]
        public void TryLinkedListViewAsSCIList()
        {
            TryListViewAsSCIList1(new LinkedList<B>());
            TryListViewAsSCIList2(new LinkedList<B>());
        }

        [Test]
        public void TryHashedArrayListViewAsSCIList()
        {
            TryListViewAsSCIList1(new HashedArrayList<B>());
            TryListViewAsSCIList2(new HashedArrayList<B>());
        }

        [Test]
        public void TryHashedLinkedListViewAsSCIList()
        {
            TryListViewAsSCIList1(new HashedLinkedList<B>());
            TryListViewAsSCIList2(new HashedLinkedList<B>());
        }

        [Test]
        public void TryGuardedViewAsSCIList()
        {
            ArrayList<B> mylist = new();
            TryListViewAsSCIList2(new GuardedList<B>(mylist));
        }
    }

    [TestFixture]
    public class IDictionaryTests
    {
        public void TryDictionary(IDictionary<string, string> dict)
        {
            Assert.That(dict, Is.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(dict.IsEmpty, Is.True);
                Assert.That(dict.IsReadOnly, Is.False);
            });
            SCG.KeyValuePair<string, string>[] arr = [];
            dict.CopyTo(arr, 0);
            dict["R"] = "A";
            dict["S"] = "B";
            dict["T"] = "C";
            Assert.Multiple(() =>
            {
                Assert.That(dict.Update("R", "A1"), Is.True);
                Assert.That(dict["R"], Is.EqualTo("A1"));

                Assert.That(dict.Update("U", "D1"), Is.False);
                Assert.That(dict.Contains("U"), Is.False);

                Assert.That(dict.Update("R", "A2", out string old), Is.True);
                Assert.That(dict["R"], Is.EqualTo("A2"));
                Assert.That(old, Is.EqualTo("A1"));

                Assert.That(dict.Update("U", "D2", out old), Is.False);
                Assert.That(old, Is.EqualTo(null));
                Assert.That(dict.Contains("U"), Is.False);

                Assert.That(dict.UpdateOrAdd("R", "A3"), Is.True);
                Assert.That(dict["R"], Is.EqualTo("A3"));

                Assert.That(dict.UpdateOrAdd("U", "D3"), Is.False);
                Assert.That(dict.Contains("U"), Is.True);
                Assert.That(dict["U"], Is.EqualTo("D3"));

                Assert.That(dict.UpdateOrAdd("R", "A4", out old), Is.True);
                Assert.That(dict["R"], Is.EqualTo("A4"));
                Assert.That(old, Is.EqualTo("A3"));

                Assert.That(dict.UpdateOrAdd("U", "D4", out old), Is.True);
                Assert.That(dict.Contains("U"), Is.True);
                Assert.That(dict["U"], Is.EqualTo("D4"));
                Assert.That(old, Is.EqualTo("D3"));

                Assert.That(dict.UpdateOrAdd("V", "E1", out old), Is.False);
                Assert.That(dict.Contains("V"), Is.True);
                Assert.That(dict["V"], Is.EqualTo("E1"));
                Assert.That(old, Is.EqualTo(null));
            });
        }

        [Test]
        public void TestHashDictionary()
        {
            TryDictionary(new HashDictionary<string, string>());
        }

        [Test]
        public void TestTreeDictionary()
        {
            TryDictionary(new TreeDictionary<string, string>());
        }
    }
}
