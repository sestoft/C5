// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
namespace C5.Tests.arrays.circularqueue
{
    [TestFixture]
    public class GenericTesters
    {
        [Test]
        public void TestEvents()
        {
            CircularQueue<int> factory() { return new CircularQueue<int>(); }
            new C5.Tests.Templates.Events.QueueTester<CircularQueue<int>>().Test(factory);
            new C5.Tests.Templates.Events.StackTester<CircularQueue<int>>().Test(factory);
        }

        //[Test]
        //public void Extensible()
        //{
        //  TODO: Test Circular Queue for Clone(?) and Serializable
        //  C5.Tests.Templates.Extensible.Clone.Tester<CircularQueue<int>>();
        //  C5.Tests.Templates.Extensible.Serialization.Tester<CircularQueue<int>>();
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
            Assert.That(coll.ToString(), Is.EqualTo("{  }"));
            foreach (int i in new int[] { -4, 28, 129, 65530 })
            {
                coll.Enqueue(i);
            }

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
                Assert.That(queue.Check(), Is.True);
            }
            for (int i = 0; i < 14; i++)
            {
                Assert.That(queue.Check(), Is.True);
                queue.Dequeue();
            }
        }

        [Test]
        public void Expand()
        {
            Assert.That(queue.Check(), Is.True);
            loadup3();
            Assert.That(IC.Eq(queue, 14, 15, 16, 17), Is.True);
        }

        [Test]
        public void Simple()
        {
            loadup1();
            Assert.Multiple(() =>
            {
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue, Has.Count.EqualTo(5));
                Assert.That(IC.Eq(queue, 12, 13, 103, 14, 15), Is.True);
            });
            Assert.That(queue.Choose(), Is.EqualTo(12));
        }

        [Test]
        public void Stack()
        {
            queue.Push(1);
            Assert.That(queue.Check(), Is.True);
            queue.Push(2);
            Assert.That(queue.Check(), Is.True);
            queue.Push(3);
            Assert.Multiple(() =>
            {
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue.Pop(), Is.EqualTo(3));
            });
            Assert.That(queue.Check(), Is.True);
            Assert.That(queue.Pop(), Is.EqualTo(2));
            Assert.That(queue.Check(), Is.True);
            Assert.That(queue.Pop(), Is.EqualTo(1));
            Assert.That(queue.Check(), Is.True);
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
            Assert.Multiple(() =>
            {
                Assert.That(queue.Check(), Is.True);
                Assert.That(queue, Has.Count.EqualTo(5));
                Assert.That(IC.Eq(queue, 15, 1000, 1001, 1002, 1003), Is.True);
            });
            Assert.That(queue.Choose(), Is.EqualTo(15));
        }

        [Test]
        public void Counting()
        {
            Assert.Multiple(() =>
            {
                Assert.That(queue.IsEmpty, Is.True);
                Assert.That(queue, Is.Empty);
            });
            Assert.That(queue.CountSpeed, Is.EqualTo(Speed.Constant));
            queue.Enqueue(11);
            Assert.That(queue.IsEmpty, Is.False);
            queue.Enqueue(12);
            Assert.That(queue, Has.Count.EqualTo(2));
        }

        //This test by Steve Wallace uncovered a bug in the indexing.
        [Test]
        public void SW200602()
        {
            C5.CircularQueue<int> list = new(8);
            for (int count = 0; count <= 7; count++)
            {
                list.Enqueue(count);
            }
            int end = list.Count;
            for (int index = 0; index < end; index++)
            {
                Assert.That(list[0], Is.EqualTo(index));
                list.Dequeue();
            }
        }

        [TearDown]
        public void Dispose() { queue = null; }

    }
}