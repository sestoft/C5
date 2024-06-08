// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;

namespace C5.Tests.heaps
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            IntervalHeap<int> factory() { return new IntervalHeap<int>(TenEqualityComparer.Instance); }
            new Templates.Events.PriorityQueueTester<IntervalHeap<int>>().Test(factory);
        }
    }

    [TestFixture]
    public class Events
    {
        private IPriorityQueue<int> queue;
        private ArrayList<System.Collections.Generic.KeyValuePair<Acts, int>> events;


        [SetUp]
        public void Init()
        {
            queue = new IntervalHeap<int>();
            events = [];
        }


        [TearDown]
        public void Dispose() { queue = null; events.Dispose(); }

        [Test]
        public void Listenable()
        {
            Assert.That(queue.ListenableEvents, Is.EqualTo(EventType.Basic));
        }

        private enum Acts
        {
            Add, Remove, Changed
        }

        [Test]
        public void Direct()
        {
            CollectionChangedHandler<int> cch;
            ItemsAddedHandler<int> iah;
            ItemsRemovedHandler<int> irh;
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.None));
            queue.CollectionChanged += (cch = new CollectionChangedHandler<int>(queue_CollectionChanged));
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.Changed));
            queue.ItemsAdded += (iah = new ItemsAddedHandler<int>(queue_ItemAdded));
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.Changed | EventType.Added));
            queue.ItemsRemoved += (irh = new ItemsRemovedHandler<int>(queue_ItemRemoved));
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.Changed | EventType.Added | EventType.Removed));
            queue.Add(34);
            queue.Add(56);
            queue.AddAll([]);
            queue.Add(34);
            queue.Add(12);
            queue.DeleteMax();
            queue.DeleteMin();
            queue.AddAll([4, 5, 6, 2]);
            Assert.That(events, Has.Count.EqualTo(17));
            int[] vals = [34, 0, 56, 0, 34, 0, 12, 0, 56, 0, 12, 0, 4, 5, 6, 2, 0];
            Acts[] acts = [ Acts.Add, Acts.Changed, Acts.Add, Acts.Changed, Acts.Add, Acts.Changed, Acts.Add, Acts.Changed,
                Acts.Remove, Acts.Changed, Acts.Remove, Acts.Changed, Acts.Add, Acts.Add, Acts.Add, Acts.Add, Acts.Changed ];
            for (int i = 0; i < vals.Length; i++)
            {
                //Console.WriteLine("{0}", events[cell]);
                Assert.That(events[i].Key, Is.EqualTo(acts[i]), "Action " + i);
                Assert.That(events[i].Value, Is.EqualTo(vals[i]), "Value " + i);
            }
            queue.CollectionChanged -= cch;
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.Added | EventType.Removed));
            queue.ItemsAdded -= iah;
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.Removed));
            queue.ItemsRemoved -= irh;
            Assert.That(queue.ActiveEvents, Is.EqualTo(EventType.None));
        }

        [Test]
        public void Guarded()
        {
            ICollectionValue<int> guarded = new GuardedCollectionValue<int>(queue);
            guarded.CollectionChanged += new CollectionChangedHandler<int>(queue_CollectionChanged);
            guarded.ItemsAdded += new ItemsAddedHandler<int>(queue_ItemAdded);
            guarded.ItemsRemoved += new ItemsRemovedHandler<int>(queue_ItemRemoved);
            queue.Add(34);
            queue.Add(56);
            queue.Add(34);
            queue.Add(12);
            queue.DeleteMax();
            queue.DeleteMin();
            queue.AddAll([4, 5, 6, 2]);
            Assert.That(events, Has.Count.EqualTo(17));
            int[] vals = [34, 0, 56, 0, 34, 0, 12, 0, 56, 0, 12, 0, 4, 5, 6, 2, 0];
            Acts[] acts = [ Acts.Add, Acts.Changed, Acts.Add, Acts.Changed, Acts.Add, Acts.Changed, Acts.Add, Acts.Changed,
                Acts.Remove, Acts.Changed, Acts.Remove, Acts.Changed, Acts.Add, Acts.Add, Acts.Add, Acts.Add, Acts.Changed ];
            for (int i = 0; i < vals.Length; i++)
            {
                //Console.WriteLine("{0}", events[cell]);
                Assert.That(events[i].Value, Is.EqualTo(vals[i]));
                Assert.That(events[i].Key, Is.EqualTo(acts[i]));
            }
        }

        private void queue_CollectionChanged(object sender)
        {
            events.Add(new System.Collections.Generic.KeyValuePair<Acts, int>(Acts.Changed, 0));
        }

        private void queue_ItemAdded(object sender, ItemCountEventArgs<int> e)
        {
            events.Add(new System.Collections.Generic.KeyValuePair<Acts, int>(Acts.Add, e.Item));
        }

        private void queue_ItemRemoved(object sender, ItemCountEventArgs<int> e)
        {
            events.Add(new System.Collections.Generic.KeyValuePair<Acts, int>(Acts.Remove, e.Item));
        }
    }

    [TestFixture]
    public class Formatting
    {
        private IntervalHeap<int> coll;
        private IFormatProvider rad16;
        [SetUp]
        public void Init() { coll = []; rad16 = new RadixFormatProvider(16); }
        [TearDown]
        public void Dispose() { coll = null; rad16 = null; }
        [Test]
        public void Format()
        {
            Assert.That(coll.ToString(), Is.EqualTo("{  }"));
            coll.AddAll([-4, 28, 129, 65530]);
            Assert.That(coll.ToString(), Is.EqualTo("{ -4, 65530, 28, 129 }"));
            Assert.That(coll.ToString(null, rad16), Is.EqualTo("{ -4, FFFA, 1C, 81 }"));
            Assert.That(coll.ToString("L14", null), Is.EqualTo("{ -4, 65530, ... }"));
            Assert.That(coll.ToString("L14", rad16), Is.EqualTo("{ -4, FFFA, ... }"));
        }
    }


    [TestFixture]
    public class IntervalHeapTests
    {
        private IPriorityQueue<int> queue;


        [SetUp]
        public void Init() { queue = new IntervalHeap<int>(); }


        [TearDown]
        public void Dispose() { queue = null; }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new IntervalHeap<int>(null));
        }

        [Test]
        public void NullEqualityComparerinConstructor2()
        {
            Assert.Throws<NullReferenceException>(() => new IntervalHeap<int>(5, null));
        }

        [Test]
        public void Handles()
        {
            IPriorityQueueHandle<int>[] handles = new IPriorityQueueHandle<int>[10];

            queue.Add(ref handles[0], 7);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[1], 72);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[2], 27);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[3], 17);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[4], 70);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[5], 1);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[6], 2);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[7], 7);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[8], 8);
            Assert.That(queue.Check(), Is.True);
            queue.Add(ref handles[9], 9);
            Assert.That(queue.Check(), Is.True);
            queue.Delete(handles[2]);
            Assert.That(queue.Check(), Is.True);
            queue.Delete(handles[0]);
            Assert.That(queue.Check(), Is.True);
            queue.Delete(handles[8]);
            Assert.That(queue.Check(), Is.True);
            queue.Delete(handles[4]);
            Assert.That(queue.Check(), Is.True);
            queue.Delete(handles[6]);
            Assert.That(queue.Check(), Is.True);
            Assert.That(queue, Has.Count.EqualTo(5));
        }

        [Test]
        public void Replace()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(6);
            queue.Add(10);
            queue.Add(ref handle, 7);
            queue.Add(21);
            Assert.That(queue.Replace(handle, 12), Is.EqualTo(7));
            Assert.That(queue.FindMax(), Is.EqualTo(21));
            Assert.That(queue.Replace(handle, 34), Is.EqualTo(12));
            Assert.That(queue.FindMax(), Is.EqualTo(34));
            Assert.That(queue.Check(), Is.True);
            //replace max
            Assert.That(queue.Replace(handle, 60), Is.EqualTo(34));
            Assert.That(queue.FindMax(), Is.EqualTo(60));
            Assert.That(queue.Replace(handle, queue[handle] + 80), Is.EqualTo(60));
            Assert.That(queue.FindMax(), Is.EqualTo(140));
            Assert.That(queue.Check(), Is.True);
        }

        [Test]
        public void Replace2()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(6);
            queue.Add(10);
            queue.Add(ref handle, 7);
            //Replace last item in queue with something large
            Assert.That(queue.Replace(handle, 12), Is.EqualTo(7));
            Assert.That(queue.Check(), Is.True);
        }

        /// <summary>
        /// bug20070504.txt by Viet Yen Nguyen
        /// </summary>
        [Test]
        public void Replace3()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(ref handle, 10);
            Assert.That(queue.Replace(handle, 12), Is.EqualTo(10));
            Assert.That(queue.Check(), Is.True);
        }

        /// <summary>
        /// bug20080222.txt by Thomas Dufour
        /// </summary>
        [Test]
        public void Replace4a()
        {
            IPriorityQueueHandle<int> handle1 = null;
            queue.Add(ref handle1, 4);
            Assert.That(queue.FindMin(), Is.EqualTo(4));
            queue.Add(3);
            Assert.That(queue.FindMin(), Is.EqualTo(3));
            Assert.That(queue.Replace(handle1, 2), Is.EqualTo(4));
            Assert.That(queue.FindMin(), Is.EqualTo(2));
        }

        [Test]
        public void Replace4b()
        {
            IPriorityQueueHandle<int> handle1 = null;
            queue.Add(ref handle1, 2);
            Assert.That(queue.FindMax(), Is.EqualTo(2));
            queue.Add(3);
            Assert.That(queue.FindMax(), Is.EqualTo(3));
            Assert.That(queue.Replace(handle1, 4), Is.EqualTo(2));
            Assert.That(queue.FindMax(), Is.EqualTo(4));
        }

        [Test]
        public void Replace5a()
        {
            for (int size = 0; size < 130; size++)
            {
                IPriorityQueue<double> q = new IntervalHeap<double>();
                IPriorityQueueHandle<double> handle1 = null;
                q.Add(ref handle1, 3.0);
                Assert.That(q.FindMin(), Is.EqualTo(3.0));
                for (int i = 1; i < size; i++)
                {
                    q.Add(i + 3.0);
                }

                Assert.That(q.FindMin(), Is.EqualTo(3.0));
                for (int min = 2; min >= -10; min--)
                {
                    Assert.That(q.Replace(handle1, min), Is.EqualTo(min + 1.0));
                    Assert.That(q.FindMin(), Is.EqualTo(min));
                }
                Assert.That(q.DeleteMin(), Is.EqualTo(-10.0));
                for (int i = 1; i < size; i++)
                {
                    Assert.That(q.DeleteMin(), Is.EqualTo(i + 3.0));
                }

                Assert.That(q.IsEmpty, Is.True);
            }
        }

        [Test]
        public void Replace5b()
        {
            for (int size = 0; size < 130; size++)
            {
                IPriorityQueue<double> q = new IntervalHeap<double>();
                IPriorityQueueHandle<double> handle1 = null;
                q.Add(ref handle1, -3.0);
                Assert.That(q.FindMax(), Is.EqualTo(-3.0));
                for (int i = 1; i < size; i++)
                {
                    q.Add(-i - 3.0);
                }

                Assert.That(q.FindMax(), Is.EqualTo(-3.0));
                for (int max = -2; max <= 10; max++)
                {
                    Assert.That(q.Replace(handle1, max), Is.EqualTo(max - 1.0));
                    Assert.That(q.FindMax(), Is.EqualTo(max));
                }
                Assert.That(q.DeleteMax(), Is.EqualTo(10.0));
                for (int i = 1; i < size; i++)
                {
                    Assert.That(q.DeleteMax(), Is.EqualTo(-i - 3.0));
                }

                Assert.That(q.IsEmpty, Is.True);
            }
        }

        [Test]
        public void Delete1a()
        {
            IPriorityQueueHandle<int> handle1 = null;
            queue.Add(ref handle1, 4);
            Assert.That(queue.FindMin(), Is.EqualTo(4));
            queue.Add(3);
            Assert.That(queue.FindMin(), Is.EqualTo(3));
            queue.Add(2);
            Assert.That(queue.Delete(handle1), Is.EqualTo(4));
            Assert.That(queue.FindMin(), Is.EqualTo(2));
            Assert.That(queue.FindMax(), Is.EqualTo(3));
        }

        [Test]
        public void Delete1b()
        {
            IPriorityQueueHandle<int> handle1 = null;
            queue.Add(ref handle1, 2);
            Assert.That(queue.FindMax(), Is.EqualTo(2));
            queue.Add(3);
            Assert.That(queue.FindMax(), Is.EqualTo(3));
            queue.Add(4);
            Assert.That(queue.Delete(handle1), Is.EqualTo(2));
            Assert.That(queue.FindMin(), Is.EqualTo(3));
            Assert.That(queue.FindMax(), Is.EqualTo(4));
        }

        [Test]
        public void ReuseHandle()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(ref handle, 7);
            queue.Delete(handle);
            queue.Add(ref handle, 8);
        }

        [Test]
        public void ErrorAddValidHandle()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(ref handle, 7);

            Assert.Throws<InvalidPriorityQueueHandleException>(() => queue.Add(ref handle, 8));
        }

        [Test]
        public void ErrorDeleteInvalidHandle()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(ref handle, 7);
            queue.Delete(handle);

            Assert.Throws<InvalidPriorityQueueHandleException>(() => queue.Delete(handle));
        }

        [Test]
        public void ErrorReplaceInvalidHandle()
        {
            IPriorityQueueHandle<int> handle = null;
            queue.Add(ref handle, 7);
            queue.Delete(handle);

            Assert.Throws<InvalidPriorityQueueHandleException>(() => queue.Replace(handle, 13));
        }

        [Test]
        public void Simple()
        {
            Assert.That(queue.AllowsDuplicates, Is.True);
            Assert.That(queue, Is.Empty);
            queue.Add(8); queue.Add(18); queue.Add(8); queue.Add(3);
            Assert.That(queue, Has.Count.EqualTo(4));
            Assert.That(queue.DeleteMax(), Is.EqualTo(18));
            Assert.That(queue, Has.Count.EqualTo(3));
            Assert.That(queue.DeleteMin(), Is.EqualTo(3));
            Assert.That(queue, Has.Count.EqualTo(2));
            Assert.That(queue.FindMax(), Is.EqualTo(8));
            Assert.That(queue.DeleteMax(), Is.EqualTo(8));
            Assert.That(queue.FindMax(), Is.EqualTo(8));
            queue.Add(15);
            Assert.That(queue.FindMax(), Is.EqualTo(15));
            Assert.That(queue.FindMin(), Is.EqualTo(8));
            Assert.That(queue.Comparer.Compare(2, 3), Is.LessThan(0));
            Assert.That(queue.Comparer.Compare(4, 3), Is.GreaterThan(0));
            Assert.That(queue.Comparer.Compare(3, 3), Is.EqualTo(0));
        }


        [Test]
        public void Enumerate()
        {
            int[] a = new int[4];
            int siz = 0;
            foreach (int i in queue)
            {
                siz++;
            }

            Assert.That(siz, Is.EqualTo(0));

            queue.Add(8); queue.Add(18); queue.Add(8); queue.Add(3);

            foreach (int i in queue)
            {
                a[siz++] = i;
            }

            Assert.That(siz, Is.EqualTo(4));
            Array.Sort(a, 0, siz);
            Assert.That(a[0], Is.EqualTo(3));
            Assert.That(a[1], Is.EqualTo(8));
            Assert.That(a[2], Is.EqualTo(8));
            Assert.That(a[3], Is.EqualTo(18));

            siz = 0;
            Assert.That(queue.DeleteMax(), Is.EqualTo(18));
            foreach (int i in queue)
            {
                a[siz++] = i;
            }

            Assert.That(siz, Is.EqualTo(3));
            Array.Sort(a, 0, siz);
            Assert.That(a[0], Is.EqualTo(3));
            Assert.That(a[1], Is.EqualTo(8));
            Assert.That(a[2], Is.EqualTo(8));

            siz = 0;
            Assert.That(queue.DeleteMax(), Is.EqualTo(8));
            foreach (int i in queue)
            {
                a[siz++] = i;
            }

            Assert.That(siz, Is.EqualTo(2));
            Array.Sort(a, 0, siz);
            Assert.That(a[0], Is.EqualTo(3));
            Assert.That(a[1], Is.EqualTo(8));

            siz = 0;
            Assert.That(queue.DeleteMax(), Is.EqualTo(8));
            foreach (int i in queue)
            {
                a[siz++] = i;
            }

            Assert.That(siz, Is.EqualTo(1));
            Assert.That(a[0], Is.EqualTo(3));
        }

        [Test]
        public void Random()
        {
            int length = 1000;
            int[] a = new int[length];
            Random ran = new(6754);

            for (int i = 0; i < length; i++)
            {
                queue.Add(a[i] = ran.Next());
            }

            Assert.That(queue.Check(), Is.True);
            Array.Sort(a);
            for (int i = 0; i < length / 2; i++)
            {
                Assert.That(queue.DeleteMax(), Is.EqualTo(a[length - i - 1]));
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue.DeleteMin(), Is.EqualTo(a[i]));
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public void RandomWithHandles()
        {
            int length = 1000;
            int[] a = new int[length];
            Random ran = new(6754);

            for (int i = 0; i < length; i++)
            {
                IPriorityQueueHandle<int> h = null;
                queue.Add(ref h, a[i] = ran.Next());
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.Check(), Is.True);
            Array.Sort(a);
            for (int i = 0; i < length / 2; i++)
            {
                Assert.That(queue.DeleteMax(), Is.EqualTo(a[length - i - 1]));
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue.DeleteMin(), Is.EqualTo(a[i]));
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public void RandomWithDeleteHandles()
        {
            Random ran = new(6754);
            int length = 1000;
            int[] a = new int[length];
            ArrayList<int> shuffle = new(length);
            IPriorityQueueHandle<int>[] h = new IPriorityQueueHandle<int>[length];

            for (int i = 0; i < length; i++)
            {
                shuffle.Add(i);
                queue.Add(ref h[i], a[i] = ran.Next());
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.Check(), Is.True);
            shuffle.Shuffle(ran);
            for (int i = 0; i < length; i++)
            {
                int j = shuffle[i];
                Assert.That(queue.Delete(h[j]), Is.EqualTo(a[j]));
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.IsEmpty, Is.True);
        }

        [Test]
        public void RandomIndexing()
        {
            Random ran = new(6754);
            int length = 1000;
            int[] a = new int[length];
            int[] b = new int[length];
            ArrayList<int> shuffle = new(length);
            IPriorityQueueHandle<int>[] h = new IPriorityQueueHandle<int>[length];

            for (int i = 0; i < length; i++)
            {
                shuffle.Add(i);
                queue.Add(ref h[i], a[i] = ran.Next());
                b[i] = ran.Next();
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.Check(), Is.True);
            shuffle.Shuffle(ran);
            for (int i = 0; i < length; i++)
            {
                int j = shuffle[i];
                Assert.That(queue[h[j]], Is.EqualTo(a[j]));
                queue[h[j]] = b[j];
                Assert.That(queue[h[j]], Is.EqualTo(b[j]));
                Assert.That(queue.Check(), Is.True);
            }
        }



        [Test]
        public void RandomDuplicates()
        {
            int length = 1000;
            int[] a = new int[length];
            Random ran = new(6754);

            for (int i = 0; i < length; i++)
            {
                queue.Add(a[i] = ran.Next(3, 13));
            }

            Assert.That(queue.Check(), Is.True);

            Array.Sort(a);

            for (int i = 0; i < length / 2; i++)
            {
                Assert.That(queue.DeleteMin(), Is.EqualTo(a[i]));
                Assert.That(queue.Check(), Is.True);
                Assert.That(_ = queue.DeleteMax(), Is.EqualTo(a[length - i - 1]));
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.IsEmpty, Is.True);
        }


        [Test]
        public void AddAll()
        {
            int length = 1000;
            int[] a = new int[length];
            Random ran = new(6754);

            LinkedList<int> lst = [];
            for (int i = 0; i < length; i++)
            {
                lst.Add(a[i] = ran.Next());
            }

            queue.AddAll(lst);
            Assert.That(queue.Check(), Is.True);
            Array.Sort(a);
            for (int i = 0; i < length / 2; i++)
            {
                Assert.That(queue.DeleteMax(), Is.EqualTo(a[length - i - 1]));
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue.DeleteMin(), Is.EqualTo(a[i]));
                Assert.That(queue.Check(), Is.True);
            }

            Assert.That(queue.IsEmpty, Is.True);
        }


        /// <summary>
        /// Cases related to bug20130208 (Iulian Nitescu &lt;iulian@live.co.uk&gt;)
        /// and bug20130505 (perpetual, via Github)
        /// </summary>
        [Test]
        public void Bug20130208()
        {
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> h0 = null, h2 = null, h4 = null, h7 = null, h5 = null;
            // Add(43, 0);
            q.Add(ref h0, 43);
            // Remove();
            q.DeleteMin();
            // XAddMaxReplace(9, 2);
            q.Add(ref h2, double.MaxValue);
            q[h2] = 9;
            // XAddMaxReplace(32, 4);
            q.Add(ref h4, double.MaxValue);
            q[h4] = 32;
            // XAddMaxReplace(44, 7);
            q.Add(ref h7, double.MaxValue);
            q[h7] = 44;
            // Remove();
            q.DeleteMin();
            // XAddMaxReplace(0, 5);
            q.Add(ref h5, double.MaxValue);
            q[h5] = 0;
            // Internally inconsistent data structure already now
            Assert.That(q.Check(), Is.True);
        }

        [Test]
        public void Bug20130208Case3()
        {
            // Case 3: only left.first
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(25);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(25));
        }

        [Test]
        public void Bug20130208Case4()
        {
            // Case 4: both left.first and left.last
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(24);
            q.Add(26);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(26));
        }

        [Test]
        public void Bug20130208Case5a()
        {
            // Case 5a: only right.first, is max
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(24);
            q.Add(26);
            q.Add(28);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(28));
        }

        [Test]
        public void Bug20130208Case5b()
        {
            // Case 5b: only right.first, not max
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(24);
            q.Add(28);
            q.Add(26);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(28));
        }

        [Test]
        public void Bug20130208Case6a()
        {
            // Case 6a: both right.first and right.last, is max
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(24);
            q.Add(26);
            q.Add(23);
            q.Add(28);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(28));
        }

        [Test]
        public void Bug20130208Case6b()
        {
            // Case 6b: both right.first and right.last, not max
            IPriorityQueue<double> q = new IntervalHeap<double>();
            IPriorityQueueHandle<double> topRight = null;
            q.Add(20);
            q.Add(ref topRight, 30);
            q.Add(24);
            q.Add(28);
            q.Add(23);
            q.Add(26);
            q[topRight] = 10;
            Assert.That(q.Check(), Is.True);
            Assert.That(q.FindMax(), Is.EqualTo(28));
        }
    }

}