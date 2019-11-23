// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;
using System;
namespace C5.Tests.arrays.circularqueue
{
    using CollectionOfInt = CircularQueue<int>;

    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            CollectionOfInt factory() { return new CollectionOfInt(); }
            new C5.Tests.Templates.Events.QueueTester<CollectionOfInt>().Test(factory);
            new C5.Tests.Templates.Events.StackTester<CollectionOfInt>().Test(factory);
        }

        //[Test]
        //public void Extensible()
        //{
        //  TODO: Test Circular Queue for Clone(?) and Serializable 
        //  C5.Tests.Templates.Extensible.Clone.Tester<CollectionOfInt>();
        //  C5.Tests.Templates.Extensible.Serialization.Tester<CollectionOfInt>();
        //}
    }

    //[TestFixture]
    public class Template
    {

        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void LeTest()
        {
        }

        [TearDown]
        public void Dispose() { }

    }

    [TestFixture]
    public class Formatting
    {
        private CircularQueue<int> coll;
        private IFormatProvider rad16;
        [SetUp]
        public void Init() { coll = new CircularQueue<int>(); rad16 = new RadixFormatProvider(16); }
        [TearDown]
        public void Dispose() { coll = null; rad16 = null; }
        [Test]
        public void Format()
        {
            Assert.AreEqual("{  }", coll.ToString());
            foreach (int i in new int[] { -4, 28, 129, 65530 })
            {
                coll.Enqueue(i);
            }

            Assert.AreEqual("{ -4, 28, 129, 65530 }", coll.ToString());
            Assert.AreEqual("{ -4, 1C, 81, FFFA }", coll.ToString(null, rad16));
            Assert.AreEqual("{ -4, 28, 129... }", coll.ToString("L14", null));
            Assert.AreEqual("{ -4, 1C, 81... }", coll.ToString("L14", rad16));
        }
    }

    [TestFixture]
    public class CircularQueue
    {
        private CircularQueue<int> queue;

        [SetUp]
        public void Init()
        {
            queue = new CircularQueue<int>();
        }

        private void loadup1()
        {
            queue.Enqueue(11);
            queue.Enqueue(12);
            queue.Enqueue(13);
            queue.Dequeue();
            queue.Enqueue(103);
            queue.Enqueue(14);
            queue.Enqueue(15);
        }

        private void loadup2()
        {
            loadup1();
            for (int i = 0; i < 4; i++)
            {
                queue.Dequeue();
                queue.Enqueue(1000 + i);
            }
        }

        private void loadup3()
        {
            for (int i = 0; i < 18; i++)
            {
                queue.Enqueue(i);
                Assert.IsTrue(queue.Check());
            }
            for (int i = 0; i < 14; i++)
            {
                Assert.IsTrue(queue.Check());
                queue.Dequeue();
            }
        }

        [Test]
        public void Expand()
        {
            Assert.IsTrue(queue.Check());
            loadup3();
            Assert.IsTrue(IC.Eq(queue, 14, 15, 16, 17));
        }

        [Test]
        public void Simple()
        {
            loadup1();
            Assert.IsTrue(queue.Check());
            Assert.AreEqual(5, queue.Count);
            Assert.IsTrue(IC.Eq(queue, 12, 13, 103, 14, 15));
            Assert.AreEqual(12, queue.Choose());
        }

        [Test]
        public void Stack()
        {
            queue.Push(1);
            Assert.IsTrue(queue.Check());
            queue.Push(2);
            Assert.IsTrue(queue.Check());
            queue.Push(3);
            Assert.IsTrue(queue.Check());
            Assert.AreEqual(3, queue.Pop());
            Assert.IsTrue(queue.Check());
            Assert.AreEqual(2, queue.Pop());
            Assert.IsTrue(queue.Check());
            Assert.AreEqual(1, queue.Pop());
            Assert.IsTrue(queue.Check());
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => queue.Choose());
        }

        [Test]
        public void BadDequeue()
        {
            Assert.Throws<NoSuchItemException>(() => queue.Dequeue());
        }

        [Test]
        public void Simple2()
        {
            loadup2();
            Assert.IsTrue(queue.Check());
            Assert.AreEqual(5, queue.Count);
            Assert.IsTrue(IC.Eq(queue, 15, 1000, 1001, 1002, 1003));
            Assert.AreEqual(15, queue.Choose());
        }

        [Test]
        public void Counting()
        {
            Assert.IsTrue(queue.IsEmpty);
            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(Speed.Constant, queue.CountSpeed);
            queue.Enqueue(11);
            Assert.IsFalse(queue.IsEmpty);
            queue.Enqueue(12);
            Assert.AreEqual(2, queue.Count);
        }

        //This test by Steve Wallace uncovered a bug in the indexing.
        [Test]
        public void SW200602()
        {
            C5.CircularQueue<int> list = new C5.CircularQueue<int>(8);
            for (int count = 0; count <= 7; count++)
            {
                list.Enqueue(count);
            }
            int end = list.Count;
            for (int index = 0; index < end; index++)
            {
                Assert.AreEqual(index, list[0]);
                list.Dequeue();
            }
        }

        [TearDown]
        public void Dispose() { queue = null; }

    }
}