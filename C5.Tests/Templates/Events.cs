// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests.Templates.Events
{
    public abstract class CollectionValueTester<TCollection, TItem> : GenericCollectionTester<TCollection, EventType>
      where TCollection : ICollectionValue<TItem>
    {
        protected TCollection collection;
        protected CollectionEventList<TItem> seen;
        protected EventType listenTo;
        protected void listen() { seen.Listen(collection, listenTo); }

        public override void SetUp(TCollection list, EventType testSpec)
        {
            collection = list;
            listenTo = testSpec;
            seen = new CollectionEventList<TItem>(EqualityComparer<TItem>.Default);
        }

        public SCG.IEnumerable<EventType> SpecsBasic
        {
            get
            {
                CircularQueue<EventType> specs = new CircularQueue<EventType>();
                //foreach (EventType listenTo in Enum.GetValues(typeof(EventType)))
                //  if ((listenTo & ~EventType.Basic) == 0)
                //    specs.Enqueue(listenTo);
                //specs.Enqueue(EventType.Added | EventType.Removed);
                for (int spec = 0; spec <= (int)EventType.Basic; spec++)
                {
                    specs.Enqueue((EventType)spec);
                }

                return specs;
            }
        }
        public SCG.IEnumerable<EventType> SpecsAll
        {
            get
            {
                CircularQueue<EventType> specs = new CircularQueue<EventType>();
                //foreach (EventType listenTo in Enum.GetValues(typeof(EventType)))
                //  specs.Enqueue(listenTo);
                //specs.Enqueue(EventType.Added | EventType.Removed);

                for (int spec = 0; spec <= (int)EventType.All; spec++)
                {
                    specs.Enqueue((EventType)spec);
                }

                return specs;
            }
        }
    }
    public abstract class CollectionValueTester<U> : CollectionValueTester<U, int> where U : ICollectionValue<int>
    {
    }

    public class ExtensibleTester<U> : CollectionValueTester<U> where U : IExtensible<int>
    {
        public override SCG.IEnumerable<EventType> GetSpecs()
        {
            return SpecsBasic;
        }
        public virtual void Listenable()
        {
            Assert.AreEqual(EventType.Basic, collection.ListenableEvents);
            Assert.AreEqual(EventType.None, collection.ActiveEvents);
            listen();
            Assert.AreEqual(listenTo, collection.ActiveEvents);
        }

        public void Add()
        {
            listen();
            seen.Check(new CollectionEvent<int>[0]);
            collection.Add(23);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void AddAll()
        {
            for (int i = 0; i < 10; i++)
            {
                collection.Add(10 * i + 5);
            }
            listen();
            collection.AddAll(new int[] { 45, 200, 56, 67 });
            seen.Check(collection.AllowsDuplicates ?
              collection.DuplicatesByCounting ?
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(200, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(55, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(65, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
              :
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(200, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(56, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
                :
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(200, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.AddAll(new int[] { });
            seen.Check(new CollectionEvent<int>[] { });
        }

    }

    public class CollectionTester<U> : ExtensibleTester<U> where U : ICollection<int>
    {
        public void Update()
        {
            collection.Add(4); collection.Add(54); collection.Add(56); collection.Add(8);
            listen();
            collection.Update(53);
            seen.Check(
              collection.AllowsDuplicates ?
              collection.DuplicatesByCounting ?
              new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(54, 2), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 2), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          }
              : new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(54, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          }
              : new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(54, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
            collection.Update(67);
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void FindOrAdd()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            int val = 53;
            collection.FindOrAdd(ref val);
            seen.Check(new CollectionEvent<int>[] { });
            val = 67;
            collection.FindOrAdd(ref val);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
        }

        public void UpdateOrAdd()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            int val = 53;
            collection.UpdateOrAdd(val);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(53, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            val = 67;
            collection.UpdateOrAdd(val);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.UpdateOrAdd(51, out _);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(53, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(51, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            // val = 67;
            collection.UpdateOrAdd(81, out _);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(81, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
        }

        public void RemoveItem()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.Remove(53);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Remove(11);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(18, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void RemoveAll()
        {
            for (int i = 0; i < 10; i++)
            {
                collection.Add(10 * i + 5);
            }
            listen();
            collection.RemoveAll(new int[] { 32, 187, 45 });
            //TODO: the order depends on internals of the HashSet
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(35, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RemoveAll(new int[] { 200, 300 });
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void RetainAll()
        {
            for (int i = 0; i < 10; i++)
            {
                collection.Add(10 * i + 5);
            }
            listen();
            collection.RetainAll(new int[] { 32, 187, 45, 62, 75, 82, 95, 2 });
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(15, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(25, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(55, 1), collection),
          //new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(75, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RetainAll(new int[] { 32, 187, 45, 62, 75, 82, 95, 2 });
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void RemoveAllCopies()
        {
            for (int i = 0; i < 10; i++)
            {
                collection.Add(3 * i + 5);
            }
            listen();
            collection.RemoveAllCopies(14);
            seen.Check(
              collection.AllowsDuplicates ?
                collection.DuplicatesByCounting ?
                  new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(11, 3), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
                :
                new CollectionEvent<int>[] {
            new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(11, 1), collection),
            new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(14, 1), collection),
            new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(17, 1), collection),
            new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
              :
              new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(11, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RemoveAllCopies(14);
            seen.Check(new CollectionEvent<int>[] { });
        }

        public virtual void Clear()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.Clear();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Cleared, new ClearedEventArgs(true, collection.AllowsDuplicates ? 3 : 2), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.Clear();
            seen.Check(new CollectionEvent<int>[] { });
        }

    }

    public class IndexedTester<U> : CollectionTester<U> where U : IIndexed<int>
    {
        public void RemoveAt()
        {
            collection.Add(4); collection.Add(16); collection.Add(28);
            listen();
            collection.RemoveAt(1);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(16,1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(16, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void RemoveInterval()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.RemoveInterval(1, 2);
            seen.Check(new CollectionEvent<int>[] {
        collection is IList<int> ?
           new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,2,1), collection):
           new CollectionEvent<int>(EventType.Cleared, new ClearedEventArgs(false,2), collection),
         new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.RemoveInterval(1, 0);
            seen.Check(new CollectionEvent<int>[] { });
        }
    }

    public class SortedIndexedTester<U> : IndexedTester<U> where U : IIndexedSorted<int>
    {
        public void DeleteMinMax()
        {
            collection.Add(34);
            collection.Add(56);
            collection.Add(34);
            collection.Add(12);
            listen();
            collection.DeleteMax();
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.DeleteMin();
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(12, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
        }

        public void AddSorted()
        {
            listen();
            collection.AddSorted(collection.AllowsDuplicates ? new int[] { 31, 62, 63, 93 } : new int[] { 31, 62, 93 });
            seen.Check(collection.AllowsDuplicates ?
              collection.DuplicatesByCounting ?
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(31, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(62, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(62, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(93, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
              :
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(31, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(62, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(63, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(93, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)}
                :
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(31, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(62, 1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(93, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.AddSorted(new int[] { });
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void RemoveRange()
        {
            for (int i = 0; i < 20; i++)
            {
                collection.Add(i * 10 + 5);
            }

            listen();
            collection.RemoveRangeFrom(173);
            //TODO: fix order to remove in:
            seen.Check(
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(195, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(185, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(175, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RemoveRangeFromTo(83, 113);
            seen.Check(
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(105, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(95, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(85, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RemoveRangeTo(33);
            seen.Check(
                new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(5, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(15, 1), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(25, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.RemoveRangeFrom(173);
            seen.Check(new CollectionEvent<int>[] { });
            collection.RemoveRangeFromTo(83, 113);
            seen.Check(new CollectionEvent<int>[] { });
            collection.RemoveRangeTo(33);
            seen.Check(new CollectionEvent<int>[] { });
        }
    }

    public class ListTester<U> : IndexedTester<U> where U : IList<int>
    {
        public override SCG.IEnumerable<EventType> GetSpecs()
        {
            return SpecsAll;
        }

        public override void Listenable()
        {
            Assert.AreEqual(EventType.All, collection.ListenableEvents);
            Assert.AreEqual(EventType.None, collection.ActiveEvents);
            listen();
            Assert.AreEqual(listenTo, collection.ActiveEvents);
        }
        public void SetThis()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection[1] = 45;
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(56,1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
        }

        public void Insert()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.Insert(1, 45);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
        }

        public void InsertAll()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.InsertAll(1, new int[] { 666, 777, 888 });
            //seen.Print(Console.Error);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(666,1), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(666, 1), collection),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(777,2), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(777, 1), collection),
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(888,3), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(888, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
            collection.InsertAll(1, new int[] { });
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void InsertFirstLast()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.InsertFirst(45);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(45,0), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(45, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
            collection.InsertLast(88);
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(88,4), collection),
          new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(88, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
          });
        }

        public void Remove()
        {
            collection.FIFO = false;
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.Remove();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(18, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.FIFO = true;
            collection.Remove();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(4, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void RemoveFirst()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.RemoveFirst();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(4,0), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(4, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void RemoveLast()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.RemoveLast();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(18,2), collection),
          new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(18, 1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

        public void Reverse()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.Reverse();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.View(1, 0).Reverse();
            seen.Check(new CollectionEvent<int>[] { });
        }


        public void Sort()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.Sort();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.View(1, 0).Sort();
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void Shuffle()
        {
            collection.Add(4); collection.Add(56); collection.Add(8);
            listen();
            collection.Shuffle();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.View(1, 0).Shuffle();
            seen.Check(new CollectionEvent<int>[] { });
        }

        public override void Clear()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.View(1, 1).Clear();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(false,1,1), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.Clear();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,2,0), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.Clear();
            seen.Check(new CollectionEvent<int>[] { });
        }

        public void ListDispose()
        {
            collection.Add(4); collection.Add(56); collection.Add(18);
            listen();
            collection.View(1, 1).Dispose();
            seen.Check(new CollectionEvent<int>[] { });
            collection.Dispose();
            seen.Check(new CollectionEvent<int>[] {
          new CollectionEvent<int>(EventType.Cleared, new ClearedRangeEventArgs(true,3,0), collection),
          new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
        });
            collection.Dispose();
            seen.Check(new CollectionEvent<int>[] { });
        }

        /*

         * /
            //[TearDown]
            //public void Dispose() { list = null; seen = null; }
            /*
            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewChanged()
            {
              IList<int> w = collection.View(0, 0);
              w.CollectionChanged += new CollectionChangedHandler<int>(w_CollectionChanged);
            }

            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewCleared()
            {
              IList<int> w = collection.View(0, 0);
              w.CollectionCleared += new CollectionClearedHandler<int>(w_CollectionCleared);
            }

            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewAdded()
            {
              IList<int> w = collection.View(0, 0);
              w.ItemsAdded += new ItemsAddedHandler<int>(w_ItemAdded);
            }

            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewInserted()
            {
              IList<int> w = collection.View(0, 0);
              w.ItemInserted += new ItemInsertedHandler<int>(w_ItemInserted);
            }

            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewRemoved()
            {
              IList<int> w = collection.View(0, 0);
              w.ItemsRemoved += new ItemsRemovedHandler<int>(w_ItemRemoved);
            }

            [Test]
            [ExpectedException(typeof(UnlistenableEventException))]
            public void ViewRemovedAt()
            {
              IList<int> w = collection.View(0, 0);
              w.ItemRemovedAt += new ItemRemovedAtHandler<int>(w_ItemRemovedAt);
            }

            void w_CollectionChanged(object sender)
            {
              throw new NotImplementedException();
            }

            void w_CollectionCleared(object sender, ClearedEventArgs eventArgs)
            {
              throw new NotImplementedException();
            }

            void w_ItemAdded(object sender, ItemCountEventArgs<int> eventArgs)
            {
              throw new NotImplementedException();
            }

            void w_ItemInserted(object sender, ItemAtEventArgs<int> eventArgs)
            {
              throw new NotImplementedException();
            }

            void w_ItemRemoved(object sender, ItemCountEventArgs<int> eventArgs)
            {
              throw new NotImplementedException();
            }

            void w_ItemRemovedAt(object sender, ItemAtEventArgs<int> eventArgs)
            {
              throw new NotImplementedException();
            }*/
    }

    public class StackTester<U> : CollectionValueTester<U> where U : IStack<int>
    {
        public override SCG.IEnumerable<EventType> GetSpecs()
        {
            return SpecsBasic;
        }

        public void PushPop()
        {
            listen();
            seen.Check(new CollectionEvent<int>[0]);
            collection.Push(23);
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(23,0), collection),
              new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(23, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Push(-12);
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(-12,1), collection),
              new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(-12, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Pop();
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(-12,1), collection),
              new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(-12, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Pop();
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(23,0), collection),
              new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(23, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }
    }

    public class QueueTester<U> : CollectionValueTester<U> where U : IQueue<int>
    {
        public override SCG.IEnumerable<EventType> GetSpecs()
        {
            return SpecsBasic;
        }

        public void EnqueueDequeue()
        {
            listen();
            collection.Enqueue(67);
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(67,0), collection),
              new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(67, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Enqueue(2);
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.Inserted, new ItemAtEventArgs<int>(2,1), collection),
              new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(2, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Dequeue();
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(67,0), collection),
              new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(67, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
            collection.Dequeue();
            seen.Check(new CollectionEvent<int>[] {
              new CollectionEvent<int>(EventType.RemovedAt, new ItemAtEventArgs<int>(2,0), collection),
              new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(2, 1), collection),
              new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)});
        }

    }

    public class PriorityQueueTester<U> : ExtensibleTester<U> where U : IPriorityQueue<int>
    {
        public override System.Collections.Generic.IEnumerable<EventType> GetSpecs()
        {
            return SpecsBasic;
        }

        public void Direct()
        {
            listen();
            collection.Add(34);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(34, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.Add(56);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(56, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.AddAll(new int[] { });
            seen.Check(new CollectionEvent<int>[] {
      });
            collection.Add(34);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(34, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.Add(12);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(12, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.DeleteMax();
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.DeleteMin();
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(12, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.AddAll(new int[] { 4, 5, 6, 2 });
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(4, 1), collection),
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(5, 1), collection),
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(6, 1), collection),
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(2, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection)
      });
        }

        public void WithHandles()
        {
            listen();
            IPriorityQueueHandle<int> handle = null;
            collection.Add(34);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(34, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.Add(56);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(56, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.Add(ref handle, 34);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(34, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.Add(12);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(12, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.DeleteMax(out _);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(56, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
            collection.DeleteMin(out _);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(12, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });

            collection.Replace(handle, 117);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(34, 1), collection),
        new CollectionEvent<int>(EventType.Added, new ItemCountEventArgs<int>(117, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });

            collection.Delete(handle);
            seen.Check(new CollectionEvent<int>[] {
        new CollectionEvent<int>(EventType.Removed, new ItemCountEventArgs<int>(117, 1), collection),
        new CollectionEvent<int>(EventType.Changed, new EventArgs(), collection),
      });
        }
    }

    public class DictionaryTester<U> : CollectionValueTester<U, System.Collections.Generic.KeyValuePair<int, int>> where U : IDictionary<int, int>
    {
        public override SCG.IEnumerable<EventType> GetSpecs()
        {
            return SpecsBasic;
        }

        public virtual void Listenable()
        {
            Assert.AreEqual(EventType.Basic, collection.ListenableEvents);
            Assert.AreEqual(EventType.None, collection.ActiveEvents);
            listen();
            Assert.AreEqual(listenTo, collection.ActiveEvents);
        }

        public void AddAndREmove()
        {
            listen();
            seen.Check(new CollectionEvent<System.Collections.Generic.KeyValuePair<int, int>>[0]);
            collection.Add(23, 45);
            seen.Check(new CollectionEvent<System.Collections.Generic.KeyValuePair<int, int>>[] {
          new CollectionEvent<System.Collections.Generic.KeyValuePair<int,int>>(EventType.Added, new ItemCountEventArgs<System.Collections.Generic.KeyValuePair<int,int>>(new System.Collections.Generic.KeyValuePair<int,int>(23,45), 1), collection),
          new CollectionEvent<System.Collections.Generic.KeyValuePair<int,int>>(EventType.Changed, new EventArgs(), collection)});
            collection.Remove(25);
            seen.Check(new CollectionEvent<System.Collections.Generic.KeyValuePair<int, int>>[] {
          new CollectionEvent<System.Collections.Generic.KeyValuePair<int,int>>(EventType.Removed, new ItemCountEventArgs<System.Collections.Generic.KeyValuePair<int,int>>(new System.Collections.Generic.KeyValuePair<int,int>(23,45), 1), collection),
          new CollectionEvent<System.Collections.Generic.KeyValuePair<int,int>>(EventType.Changed, new EventArgs(), collection)});
        }



    }

}
