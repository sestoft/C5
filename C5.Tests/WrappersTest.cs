// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;


namespace C5.Tests.wrappers
{
    namespace events
    {
        [TestFixture]
        public class IList_
        {
            private ArrayList<int> list;
            private GuardedList<int> guarded;
            private CollectionEventList<int> seen;

            [SetUp]
            public void Init()
            {
                list = new ArrayList<int>(TenEqualityComparer.Instance);
                guarded = new GuardedList<int>(list);
                seen = new CollectionEventList<int>(System.Collections.Generic.EqualityComparer<int>.Default);
            }

            private void listen() { seen.Listen(guarded, EventType.All); }

            [Test]
            public void Listenable()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(guarded.ListenableEvents, Is.EqualTo(EventType.All));
                    Assert.That(guarded.ActiveEvents, Is.EqualTo(EventType.None));
                });
                listen();
                Assert.That(guarded.ActiveEvents, Is.EqualTo(EventType.All));
            }

            [Test]
            public void SetThis()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list[1] = 45;
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), guarded),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
          ]);
            }

            [Test]
            public void Insert()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Insert(1, 45);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(666,1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(666, 1), guarded),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(777,2), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(777, 1), guarded),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(888,3), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(888, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
          ]);
                list.InsertAll(1, []);
                seen.Check([]);
            }

            [Test]
            public void InsertFirstLast()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.InsertFirst(45);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,0), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
          ]);
                list.InsertLast(88);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(88,4), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(88, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
          ]);
            }

            [Test]
            public void Remove()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Remove();
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(8, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void RemoveFirst()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveFirst();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(4,0), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(4, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void RemoveLast()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveLast();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(8,2), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(8, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void Reverse()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.Reverse();
                seen.Check([
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void RemoveInterval()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.RemoveInterval(1, 2);
                seen.Check([
           new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,2,1), guarded),
         new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
                val = 67;
                list.UpdateOrAdd(val);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
                list.UpdateOrAdd(51, out _);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(53, 1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(51, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
                // val = 67;
                list.UpdateOrAdd(81, out _);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(81, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
            }

            [Test]
            public void RemoveItem()
            {
                list.Add(4); list.Add(56); list.Add(18);
                listen();
                list.Remove(53);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Remove(11);
                seen.Check([
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(18, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
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
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(35, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(45, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.RemoveAll([200, 300]);
                seen.Check([]);
            }

            [Test]
            public void Clear()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.View(1, 1).Clear();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,1,1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
                list.Clear();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,2,0), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
        ]);
                list.Clear();
                seen.Check([]);
            }

            [Test]
            public void ListDispose()
            {
                list.Add(4); list.Add(56); list.Add(8);
                listen();
                list.View(1, 1).Dispose();
                seen.Check([]);
                list.Dispose();
                seen.Check([
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,3,0), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)
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
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(15, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(25, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(55, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(75, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
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
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(11, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(14, 1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(17, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
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
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void AddAll()
            {
                for (int i = 0; i < 10; i++)
                {
                    list.Add(10 * i + 5);
                }
                listen();
                list.AddAll([45, 56, 67]);
                seen.Check([
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(56, 1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.AddAll([]);
                seen.Check([]);
            }

            [TearDown]
            public void Dispose() { list.Dispose(); guarded.Dispose(); seen = null; }

            [Test]
            public void ViewChanged()
            {
                IList<int> w = list.View(0, 0);
                Assert.Throws<UnlistenableEventException>(() => w.CollectionChanged += new CollectionChangedHandler<int>(w_CollectionChanged));
            }

            [Test]
            public void ViewCleared()
            {
                IList<int> w = list.View(0, 0);
                Assert.Throws<UnlistenableEventException>(() => w.CollectionCleared += new CollectionClearedHandler<int>(w_CollectionCleared));
            }

            [Test]
            public void ViewAdded()
            {
                IList<int> w = list.View(0, 0);
                Assert.Throws<UnlistenableEventException>(() => w.ItemsAdded += new ItemsAddedHandler<int>(w_ItemAdded));
            }

            [Test]
            public void ViewInserted()
            {
                IList<int> w = list.View(0, 0);
                Assert.Throws<UnlistenableEventException>(() => w.ItemInserted += new ItemInsertedHandler<int>(w_ItemInserted));
            }

            [Test]
            public void ViewRemoved()
            {
                IList<int> w = list.View(0, 0);
                Assert.Throws<UnlistenableEventException>(() => w.ItemsRemoved += new ItemsRemovedHandler<int>(w_ItemRemoved));
            }

            [Test]
            public void ViewRemovedAt()
            {
                IList<int> w = list.View(0, 0);

                Assert.Throws<UnlistenableEventException>(() => w.ItemRemovedAt += new ItemRemovedAtHandler<int>(w_ItemRemovedAt));
            }

            private void w_CollectionChanged(object sender)
            {
                throw new NotImplementedException();
            }

            private void w_CollectionCleared(object sender, ClearedEventArgs eventArgs)
            {
                throw new NotImplementedException();
            }

            private void w_ItemAdded(object sender, ItemCountEventArgs<int> eventArgs)
            {
                throw new NotImplementedException();
            }

            private void w_ItemInserted(object sender, ItemAtEventArgs<int> eventArgs)
            {
                throw new NotImplementedException();
            }

            private void w_ItemRemoved(object sender, ItemCountEventArgs<int> eventArgs)
            {
                throw new NotImplementedException();
            }

            private void w_ItemRemovedAt(object sender, ItemAtEventArgs<int> eventArgs)
            {
                throw new NotImplementedException();
            }
        }

        [TestFixture]
        public class StackQueue
        {
            private ArrayList<int> list;
            private GuardedList<int> guarded;
            private CollectionEventList<int> seen;

            [SetUp]
            public void Init()
            {
                list = new ArrayList<int>(TenEqualityComparer.Instance);
                guarded = new GuardedList<int>(list);
                seen = new CollectionEventList<int>(System.Collections.Generic.EqualityComparer<int>.Default);
            }

            private void listen() { seen.Listen(guarded, EventType.All); }


            [Test]
            public void EnqueueDequeue()
            {
                listen();
                list.Enqueue(67);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(67,0), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Enqueue(2);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(2,1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(2, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Dequeue();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(67,0), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(67, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Dequeue();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(2,0), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(2, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [Test]
            public void PushPop()
            {
                listen();
                seen.Check([]);
                list.Push(23);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(23,0), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Push(-12);
                seen.Check([
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(-12,1), guarded),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(-12, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Pop();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(-12,1), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(-12, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
                list.Pop();
                seen.Check([
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(23,0), guarded),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(23, 1), guarded),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), guarded)]);
            }

            [TearDown]
            public void Dispose() { list.Dispose(); guarded.Dispose(); seen = null; }
        }


    }

    namespace wrappedarray
    {
        [TestFixture]
        public class Basic
        {

            [SetUp]
            public void Init()
            {
            }

            [TearDown]
            public void Dispose()
            {
            }

            [Test]
            public void NoExc()
            {
                WrappedArray<int> wrapped = new([4, 6, 5]);
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped[1], Is.EqualTo(6));
                    Assert.That(wrapped[1, 2], Is.EqualTo(new[] { 6, 5 }));
                });
                //
                bool is4(int i) { return i == 4; }

                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.ActiveEvents, Is.EqualTo(EventType.None));
                    Assert.That(wrapped.All(is4), Is.EqualTo(false));
                    Assert.That(wrapped.AllowsDuplicates, Is.EqualTo(true));
                });
                wrapped.Apply(delegate (int i) { });
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Backwards().ToString(), Is.EqualTo("{ 5, 6, 4 }"));
                    Assert.That(wrapped.Check(), Is.EqualTo(true));
                });
                wrapped.Choose();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Contains(4), Is.EqualTo(true));
                    Assert.That(wrapped.ContainsAll(new ArrayList<int>()), Is.EqualTo(true));
                    Assert.That(wrapped.ContainsCount(4), Is.EqualTo(1));
                    Assert.That(wrapped.ContainsSpeed, Is.EqualTo(Speed.Linear));
                });
                int[] extarray = new int[5];
                wrapped.CopyTo(extarray, 1);
                Assert.Multiple(() =>
                {
                    Assert.That(extarray, Is.EqualTo(new[] { 0, 4, 6, 5, 0 }));
                    Assert.That(wrapped, Has.Count.EqualTo(3));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.CountSpeed, Is.EqualTo(Speed.Constant));
                    Assert.That(wrapped.Direction, Is.EqualTo(Direction.Forwards));
                    Assert.That(wrapped.DuplicatesByCounting, Is.EqualTo(false));
                    Assert.That(wrapped.EqualityComparer, Is.EqualTo(System.Collections.Generic.EqualityComparer<int>.Default));
                    Assert.That(wrapped.Exists(is4), Is.EqualTo(true));
                    Assert.That(wrapped.Filter(is4), Is.EqualTo(new[] { 4 }));
                });
                int j = 5;
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Find(ref j), Is.EqualTo(true));
                    Assert.That(wrapped.Find(is4, out j), Is.EqualTo(true));
                    Assert.That(wrapped.FindAll(is4).ToString(), Is.EqualTo("[ 0:4 ]"));
                    Assert.That(wrapped.FindIndex(is4), Is.EqualTo(0));
                    Assert.That(wrapped.FindLast(is4, out j), Is.EqualTo(true));
                    Assert.That(wrapped.FindLastIndex(is4), Is.EqualTo(0));
                    Assert.That(wrapped.First, Is.EqualTo(4));
                });
                wrapped.GetEnumerator();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(4, 6, 5)));
                    Assert.That(wrapped.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(4, 6, 5)));
                    Assert.That(wrapped.IndexingSpeed, Is.EqualTo(Speed.Constant));
                    Assert.That(wrapped.IndexOf(5), Is.EqualTo(2));
                    Assert.That(wrapped.IsEmpty, Is.EqualTo(false));
                    Assert.That(wrapped.IsReadOnly, Is.EqualTo(true));
                    Assert.That(wrapped.IsSorted(), Is.EqualTo(false));
                    Assert.That(wrapped.IsValid, Is.EqualTo(true));
                    Assert.That(wrapped.Last, Is.EqualTo(5));
                    Assert.That(wrapped.LastIndexOf(5), Is.EqualTo(2));
                    Assert.That(wrapped.ListenableEvents, Is.EqualTo(EventType.None));
                });
                string i2s(int i) { return string.Format("T{0}", i); }
                Assert.That(wrapped.Map(i2s).ToString(), Is.EqualTo("[ 0:T4, 1:T6, 2:T5 ]"));
                Assert.That(wrapped.Offset, Is.EqualTo(0));
                wrapped.Reverse();
                Assert.That(wrapped.ToString(), Is.EqualTo("[ 0:5, 1:6, 2:4 ]"));
                IList<int> other = new ArrayList<int>(); other.AddAll([4, 5, 6]);
                Assert.That(wrapped.SequencedEquals(other), Is.False);
                j = 30;
                Assert.That(wrapped.Show(new System.Text.StringBuilder(), ref j, null), Is.EqualTo(true));
                wrapped.Sort();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.ToString(), Is.EqualTo("[ 0:4, 1:5, 2:6 ]"));
                    Assert.That(wrapped.ToArray(), Is.EquivalentTo(new[] { 4, 5, 6 }));
                    Assert.That(wrapped.ToString("L4", null), Is.EqualTo("[ ... ]"));
                    Assert.That(wrapped.Underlying, Is.EqualTo(null));
                    Assert.That(wrapped.UniqueItems(), Is.EquivalentTo(new[] { 4, 5, 6 }));
                    Assert.That(wrapped.UnsequencedEquals(other), Is.True);
                });
                wrapped.Shuffle();
                Assert.That(wrapped.UniqueItems(), Is.EquivalentTo(new[] { 4, 5, 6 }));
            }

            [Test]
            public void WithExc()
            {
                WrappedArray<int> wrapped = new([3, 4, 6, 5, 7]);
                //
                try { wrapped.Add(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.AddAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Clear(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Dispose(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                int j = 1;
                try { wrapped.FindOrAdd(ref j); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Insert(1, 1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Insert(wrapped.View(0, 0), 1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertAll(1, null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertFirst(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertLast(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Remove(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Remove(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAllCopies(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAt(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveFirst(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveInterval(0, 0); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveLast(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RetainAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Update(1, out j); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.UpdateOrAdd(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
            }

            [Test]
            public void View()
            {
                int[] inner = [3, 4, 6, 5, 7];
                WrappedArray<int> outerwrapped = new(inner);
                WrappedArray<int> wrapped = (WrappedArray<int>)outerwrapped.View(1, 3);
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped[1], Is.EqualTo(6));
                    Assert.That(wrapped[1, 2], Is.EqualTo(new[] { 6, 5 }));
                });
                bool is4(int i) { return i == 4; }

                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.ActiveEvents, Is.EqualTo(EventType.None));
                    Assert.That(wrapped.All(is4), Is.EqualTo(false));
                    Assert.That(wrapped.AllowsDuplicates, Is.EqualTo(true));
                });
                wrapped.Apply(delegate (int i) { });
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Backwards().ToString(), Is.EqualTo("{ 5, 6, 4 }"));
                    Assert.That(wrapped.Check(), Is.EqualTo(true));
                });
                wrapped.Choose();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Contains(4), Is.EqualTo(true));
                    Assert.That(wrapped.ContainsAll(new ArrayList<int>()), Is.EqualTo(true));
                    Assert.That(wrapped.ContainsCount(4), Is.EqualTo(1));
                    Assert.That(wrapped.ContainsSpeed, Is.EqualTo(Speed.Linear));
                });
                int[] extarray = new int[5];
                wrapped.CopyTo(extarray, 1);
                Assert.Multiple(() =>
                {
                    Assert.That(extarray, Is.EqualTo(new[] { 0, 4, 6, 5, 0 }));
                    Assert.That(wrapped, Has.Count.EqualTo(3));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.CountSpeed, Is.EqualTo(Speed.Constant));
                    Assert.That(wrapped.Direction, Is.EqualTo(Direction.Forwards));
                    Assert.That(wrapped.DuplicatesByCounting, Is.EqualTo(false));
                    Assert.That(wrapped.EqualityComparer, Is.EqualTo(System.Collections.Generic.EqualityComparer<int>.Default));
                    Assert.That(wrapped.Exists(is4), Is.EqualTo(true));
                    Assert.That(wrapped.Filter(is4), Is.EqualTo(new[] { 4 }));
                });
                int j = 5;
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.Find(ref j), Is.EqualTo(true));
                    Assert.That(wrapped.Find(is4, out j), Is.EqualTo(true));
                    Assert.That(wrapped.FindAll(is4).ToString(), Is.EqualTo("[ 0:4 ]"));
                    Assert.That(wrapped.FindIndex(is4), Is.EqualTo(0));
                    Assert.That(wrapped.FindLast(is4, out j), Is.EqualTo(true));
                    Assert.That(wrapped.FindLastIndex(is4), Is.EqualTo(0));
                    Assert.That(wrapped.First, Is.EqualTo(4));
                });
                wrapped.GetEnumerator();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.GetSequencedHashCode(), Is.EqualTo(CHC.SequencedHashCode(4, 6, 5)));
                    Assert.That(wrapped.GetUnsequencedHashCode(), Is.EqualTo(CHC.UnsequencedHashCode(4, 6, 5)));
                    Assert.That(wrapped.IndexingSpeed, Is.EqualTo(Speed.Constant));
                    Assert.That(wrapped.IndexOf(5), Is.EqualTo(2));
                    Assert.That(wrapped.IsEmpty, Is.EqualTo(false));
                    Assert.That(wrapped.IsReadOnly, Is.EqualTo(true));
                    Assert.That(wrapped.IsSorted(), Is.EqualTo(false));
                    Assert.That(wrapped.IsValid, Is.EqualTo(true));
                    Assert.That(wrapped.Last, Is.EqualTo(5));
                    Assert.That(wrapped.LastIndexOf(5), Is.EqualTo(2));
                    Assert.That(wrapped.ListenableEvents, Is.EqualTo(EventType.None));
                });
                string i2s(int i) { return string.Format("T{0}", i); }
                Assert.That(wrapped.Map(i2s).ToString(), Is.EqualTo("[ 0:T4, 1:T6, 2:T5 ]"));
                Assert.That(wrapped.Offset, Is.EqualTo(1));
                wrapped.Reverse();
                Assert.That(wrapped.ToString(), Is.EqualTo("[ 0:5, 1:6, 2:4 ]"));
                IList<int> other = new ArrayList<int>(); other.AddAll([4, 5, 6]);
                Assert.That(wrapped.SequencedEquals(other), Is.False);
                j = 30;
                Assert.That(wrapped.Show(new System.Text.StringBuilder(), ref j, null), Is.EqualTo(true));
                wrapped.Sort();
                Assert.Multiple(() =>
                {
                    Assert.That(wrapped.ToString(), Is.EqualTo("[ 0:4, 1:5, 2:6 ]"));
                    Assert.That(wrapped.ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
                    Assert.That(wrapped.ToString("L4", null), Is.EqualTo("[ ... ]"));
                    Assert.That(outerwrapped, Is.EqualTo(wrapped.Underlying));
                    Assert.That(wrapped.UniqueItems(), Is.EquivalentTo(new[] { 4, 5, 6 }));
                    Assert.That(wrapped.UnsequencedEquals(other), Is.True);
                    Assert.That(wrapped.TrySlide(1), Is.True);
                    Assert.That(wrapped.ToArray(), Is.EqualTo(new[] { 5, 6, 7 }));
                    Assert.That(wrapped.TrySlide(-1, 2), Is.True);
                    Assert.That(wrapped.ToArray(), Is.EqualTo(new[] { 4, 5 }));
                    Assert.That(wrapped.TrySlide(-2), Is.False);
                    Assert.That(wrapped.Span(outerwrapped.ViewOf(7)), Is.EquivalentTo(new[] { 4, 5, 6, 7 }));
                });
                //
                wrapped.Shuffle();
                Assert.That(wrapped.UniqueItems(), Is.EquivalentTo(new[] { 4, 5 }));
                Assert.That(wrapped.IsValid, Is.True);
                wrapped.Dispose();
                Assert.That(wrapped.IsValid, Is.False);
            }

            [Test]
            public void ViewWithExc()
            {
                int[] inner = [3, 4, 6, 5, 7];
                WrappedArray<int> outerwrapped = new(inner);
                WrappedArray<int> wrapped = (WrappedArray<int>)outerwrapped.View(1, 3);
                //
                try { wrapped.Add(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.AddAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Clear(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                //Should not throw
                //try { wrapped.Dispose(); Assert.Fail("No throw"); }
                //catch (FixedSizeCollectionException) { }
                int j = 1;
                try { wrapped.FindOrAdd(ref j); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Insert(1, 1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Insert(wrapped.View(0, 0), 1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertAll(1, null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertFirst(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.InsertLast(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Remove(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Remove(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAllCopies(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveAt(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveFirst(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveInterval(0, 0); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RemoveLast(); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.RetainAll(null); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.Update(1, out j); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
                try { wrapped.UpdateOrAdd(1); Assert.Fail("No throw"); }
                catch (FixedSizeCollectionException) { }
            }
        }
    }
}
