/*
 Copyright (c) 2003-2016 Niels Kokholm, Peter Sestoft, and Rasmus Lystr�m
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

using System;
using SCG = System.Collections.Generic;
namespace C5
{

    /// <summary>
    /// Base class for classes implementing ICollectionValue[T]
    /// </summary>
    [Serializable]
    public abstract class CollectionValueBase<T> : EnumerableBase<T>, ICollectionValue<T>, IShowable
    {
        #region Event handling
        EventBlock<T> eventBlock;
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual EventTypeEnum ListenableEvents { get { return 0; } }

        /// <summary>
        /// A flag bitmap of the events currently subscribed to by this collection.
        /// </summary>
        /// <value></value>
        public virtual EventTypeEnum ActiveEvents { get { return eventBlock == null ? 0 : eventBlock.events; } }

        private void checkWillListen(EventTypeEnum eventType)
        {
            if ((ListenableEvents & eventType) == 0)
                throw new UnlistenableEventException();
        }

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public virtual event CollectionChangedHandler<T> CollectionChanged
        {
            add { checkWillListen(EventTypeEnum.Changed); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionChanged += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Changed);
                if (eventBlock != null)
                {
                    eventBlock.CollectionChanged -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the CollectionChanged event
        /// </summary>
        protected virtual void raiseCollectionChanged()
        { if (eventBlock != null) eventBlock.raiseCollectionChanged(this); }

        /// <summary>
        /// The clear event. Will be raised for every Clear operation on the collection.
        /// </summary>
        public virtual event CollectionClearedHandler<T> CollectionCleared
        {
            add { checkWillListen(EventTypeEnum.Cleared); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionCleared += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Cleared);
                if (eventBlock != null)
                {
                    eventBlock.CollectionCleared -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void raiseCollectionCleared(bool full, int count)
        { if (eventBlock != null) eventBlock.raiseCollectionCleared(this, full, count); }

        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void raiseCollectionCleared(bool full, int count, int? offset)
        { if (eventBlock != null) eventBlock.raiseCollectionCleared(this, full, count, offset); }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemsAddedHandler<T> ItemsAdded
        {
            add { checkWillListen(EventTypeEnum.Added); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsAdded += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Added);
                if (eventBlock != null)
                {
                    eventBlock.ItemsAdded -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemsAdded event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="count"></param>
        protected virtual void raiseItemsAdded(T item, int count)
        { if (eventBlock != null) eventBlock.raiseItemsAdded(this, item, count); }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemsRemovedHandler<T> ItemsRemoved
        {
            add { checkWillListen(EventTypeEnum.Removed); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsRemoved += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Removed);
                if (eventBlock != null)
                {
                    eventBlock.ItemsRemoved -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemsRemoved event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="count"></param>
        protected virtual void raiseItemsRemoved(T item, int count)
        { if (eventBlock != null) eventBlock.raiseItemsRemoved(this, item, count); }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemInsertedHandler<T> ItemInserted
        {
            add { checkWillListen(EventTypeEnum.Inserted); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemInserted += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Inserted);
                if (eventBlock != null)
                {
                    eventBlock.ItemInserted -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemInserted event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="index"></param>
        protected virtual void raiseItemInserted(T item, int index)
        { if (eventBlock != null) eventBlock.raiseItemInserted(this, item, index); }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add { checkWillListen(EventTypeEnum.RemovedAt); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemRemovedAt += value; }
            remove
            {
                checkWillListen(EventTypeEnum.RemovedAt);
                if (eventBlock != null)
                {
                    eventBlock.ItemRemovedAt -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary> 
        /// Fire the ItemRemovedAt event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="index"></param>
        protected virtual void raiseItemRemovedAt(T item, int index)
        { if (eventBlock != null) eventBlock.raiseItemRemovedAt(this, item, index); }

        #region Event support for IList
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        protected virtual void raiseForSetThis(int index, T value, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, 1);
                raiseItemRemovedAt(item, index);
                raiseItemsAdded(value, 1);
                raiseItemInserted(value, index);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        protected virtual void raiseForInsert(int i, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemInserted(item, i);
                raiseItemsAdded(item, 1);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected void raiseForRemove(T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, 1);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        protected void raiseForRemove(T item, int count)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, count);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void raiseForRemoveAt(int index, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemRemovedAt(item, index);
                raiseItemsRemoved(item, 1);
                raiseCollectionChanged();
            }
        }

        #endregion

        #region Event  Support for ICollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        protected virtual void raiseForUpdate(T newitem, T olditem)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(olditem, 1);
                raiseItemsAdded(newitem, 1);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        /// <param name="count"></param>
        protected virtual void raiseForUpdate(T newitem, T olditem, int count)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(olditem, count);
                raiseItemsAdded(newitem, count);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected virtual void raiseForAdd(T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsAdded(item, 1);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wasRemoved"></param>
        protected virtual void raiseForRemoveAll(ICollectionValue<T> wasRemoved)
        {
            if ((ActiveEvents & EventTypeEnum.Removed) != 0)
                foreach (T item in wasRemoved)
                    raiseItemsRemoved(item, 1);
            if (wasRemoved != null && wasRemoved.Count > 0)
                raiseCollectionChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        protected class RaiseForRemoveAllHandler
        {
            CollectionValueBase<T> collection;
            CircularQueue<T> wasRemoved;
            bool wasChanged = false;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="collection"></param>
            public RaiseForRemoveAllHandler(CollectionValueBase<T> collection)
            {
                this.collection = collection;
                mustFireRemoved = (collection.ActiveEvents & EventTypeEnum.Removed) != 0;
                MustFire = (collection.ActiveEvents & (EventTypeEnum.Removed | EventTypeEnum.Changed)) != 0;
            }

            bool mustFireRemoved;
            /// <summary>
            /// 
            /// </summary>
            public readonly bool MustFire;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public void Remove(T item)
            {
                if (mustFireRemoved)
                {
                    if (wasRemoved == null)
                        wasRemoved = new CircularQueue<T>();
                    wasRemoved.Enqueue(item);
                }
                if (!wasChanged)
                    wasChanged = true;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Raise()
            {
                if (wasRemoved != null)
                    foreach (T item in wasRemoved)
                        collection.raiseItemsRemoved(item, 1);
                if (wasChanged)
                    collection.raiseCollectionChanged();
            }
        }
        #endregion

        #endregion
         
        internal MemoryType MemoryType { get; set; }

        /// <summary>
        /// Check if collection is empty.
        /// </summary>
        /// <value>True if empty</value>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// The number of items in this collection.
        /// </summary>
        /// <value></value>
        public abstract int Count { get; }

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the 
        /// <code>Count</code> property in this collection.</value>
        public abstract Speed CountSpeed { get; }

        /// <summary>
        /// Copy the items of this collection to part of an array.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if <code>index</code> 
        /// is not a valid index
        /// into the array (i.e. negative or greater than the size of the array)
        /// or the array does not have room for the items.</exception>
        /// <param name="array">The array to copy to.</param>
        /// <param name="index">The starting index.</param>
        public virtual void CopyTo(T[] array, int index)
        {
            if (index < 0 || index + Count > array.Length)
                throw new ArgumentOutOfRangeException();

            foreach (T item in this) array[index++] = item;
        }

        /// <summary>
        /// Create an array with the items of this collection (in the same order as an
        /// enumerator would output them).
        /// </summary>
        /// <returns>The array</returns>
        public virtual T[] ToArray()
        {
            T[] res = new T[Count];
            int i = 0;

            foreach (T item in this) res[i++] = item;

            return res;
        }

        /// <summary>
        /// Apply an single argument action, <see cref="T:Action`1"/> to this enumerable
        /// </summary>
        /// <param name="action">The action delegate</param>
        public virtual void Apply(Action<T> action)
        {
            foreach (T item in this)
                action(item);
        }


        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R = bool</code>) 
        /// defining the predicate</param>
        /// <returns>True if such an item exists</returns>
        public virtual bool Exists(Func<T, bool> predicate)
        {
            foreach (T item in this)
                if (predicate(item))
                    return true;

            return false;
        }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the first one in enumeration order.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
        /// <param name="item"></param>
        /// <returns>True is such an item exists</returns>
        public virtual bool Find(Func<T, bool> predicate, out T item)
        {
            foreach (T jtem in this)
                if (predicate(jtem))
                {
                    item = jtem;
                    return true;
                }
            item = default(T);
            return false;
        }

        /// <summary>
        /// Check if all items in this collection satisfies a specific predicate.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R = bool</code>) 
        /// defining the predicate</param>
        /// <returns>True if all items satisfies the predicate</returns>
        public virtual bool All(Func<T, bool> predicate)
        {
            foreach (T item in this)
                if (!predicate(item))
                    return false;

            return true;
        }

        /// <summary>
        /// Create an enumerable, enumerating the items of this collection that satisfies 
        /// a certain condition.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R = bool</code>) 
        /// defining the predicate</param>
        /// <returns>The filtered enumerable</returns>
        public virtual SCG.IEnumerable<T> Filter(Func<T, bool> predicate)
        {
            if (MemoryType == MemoryType.Strict) throw new Exception("This is not a memory safe function and cannot be used in MemoryType.Strict");

            foreach (T item in this)
                if (predicate(item))
                    yield return item;
        }

        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public abstract T Choose();
 

        #region IShowable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            return Showing.ShowCollectionValue<T>(this, stringbuilder, ref rest, formatProvider);
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return Showing.ShowString(this, format, formatProvider);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DirectedCollectionValueBase<T> : CollectionValueBase<T>, IDirectedCollectionValue<T>
    {
        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public virtual EnumerationDirection Direction { get { return EnumerationDirection.Forwards; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDirectedCollectionValue<T> Backwards();

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return this.Backwards(); }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the first one in enumeration order.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
        /// <param name="item"></param>
        /// <returns>True is such an item exists</returns>
        public virtual bool FindLast(Func<T, bool> predicate, out T item)
        {
            foreach (T jtem in Backwards())
                if (predicate(jtem))
                {
                    item = jtem;
                    return true;
                }
            item = default(T);
            return false;
        }
    }

    /// <summary>
    /// Base class (abstract) for ICollection implementations.
    /// </summary>
    [Serializable]
    public abstract class CollectionBase<T> : CollectionValueBase<T>
    {
        #region Fields

        /// <summary>
        /// The underlying field of the ReadOnly property
        /// </summary>
        protected bool isReadOnlyBase = false;

        /// <summary>
        /// The current stamp value
        /// </summary>
        protected int stamp { get; set; }

        /// <summary>
        /// The number of items in the collection
        /// </summary>
        protected int size;

        /// <summary>
        /// The item equalityComparer of the collection
        /// </summary>
        protected readonly SCG.IEqualityComparer<T> itemequalityComparer;

        int iUnSequencedHashCode, iUnSequencedHashCodeStamp = -1;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemequalityComparer"></param>
        /// <param name = "memoryType">The type of memory for the enumerator used to iterate the collection</param>
        protected CollectionBase(SCG.IEqualityComparer<T> itemequalityComparer, MemoryType memoryType)
        {
            if (itemequalityComparer == null)
                throw new NullReferenceException("Item EqualityComparer cannot be null.");
            this.itemequalityComparer = itemequalityComparer;

            MemoryType = memoryType;
        }

        #region Util

        /// <summary>
        /// Utility method for range checking.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if the start or count is negative or
        ///  if the range does not fit within collection size.</exception>
        /// <param name="start">start of range</param>
        /// <param name="count">size of range</param>
        protected void checkRange(int start, int count)
        {
            if (start < 0 || count < 0 || start + count > size)
                throw new ArgumentOutOfRangeException();
        }


        /// <summary>
        /// Compute the unsequenced hash code of a collection
        /// </summary>
        /// <param name="items">The collection to compute hash code for</param>
        /// <param name="itemequalityComparer">The item equalitySCG.Comparer</param>
        /// <returns>The hash code</returns>
        public static int ComputeHashCode(ICollectionValue<T> items, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            int h = 0;

            //But still heuristic: 
            //Note: the three odd factors should really be random, 
            //but there will be a problem with serialization/deserialization!
            //Two products is too few
            foreach (T item in items)
            {
                uint h1 = (uint)itemequalityComparer.GetHashCode(item);

                h += (int)((h1 * 1529784657 + 1) ^ (h1 * 2912831877) ^ (h1 * 1118771817 + 2));
            }

            return h;
            /*
                  The pairs (-1657792980, -1570288808) and (1862883298, -272461342) gives the same
                  unsequenced hashcode with this hashfunction. The pair was found with code like

                  HashDictionary<int, int[]> set = new HashDictionary<int, int[]>();
                  Random rnd = new C5Random(12345);
                  while (true)
                  {
                      int[] a = new int[2];
                      a[0] = rnd.Next(); a[1] = rnd.Next();
                      int h = unsequencedhashcode(a);
                      int[] b = a;
                      if (set.FindOrAdd(h, ref b))
                      {
                          Logger.Log(string.Format("Code {5}, Pair ({1},{2}) number {0} matched other pair ({3},{4})", set.Count, a[0], a[1], b[0], b[1], h));
                      }
                  }
                  */

        }

        static Type isortedtype = typeof(ISorted<T>);

        /// <summary>
        /// Examine if collection1 and collection2 are equal as unsequenced collections
        /// using the specified item equalityComparer (assumed compatible with the two collections).
        /// </summary>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="itemequalityComparer">The item equalityComparer to use for comparison</param>
        /// <returns>True if equal</returns>
        public static bool StaticEquals(ICollection<T> collection1, ICollection<T> collection2, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            if (ReferenceEquals(collection1, collection2))
                return true;

            // bug20070227:
            if (collection1 == null || collection2 == null)
                return false;

            if (collection1.Count != collection2.Count)
                return false;

            //This way we might run through both enumerations twice, but
            //probably not (if the hash codes are good)
            //TODO: check equal equalityComparers, at least here!
            if (collection1.GetUnsequencedHashCode() != collection2.GetUnsequencedHashCode())
                return false;

            //TODO: move this to the sorted implementation classes? 
            //Really depends on speed of InstanceOfType: we could save a cast
            {
                ISorted<T> stit, stat;
                if ((stit = collection1 as ISorted<T>) != null && (stat = collection2 as ISorted<T>) != null && stit.Comparer == stat.Comparer)
                {
                    using (SCG.IEnumerator<T> dat = collection2.GetEnumerator(), dit = collection1.GetEnumerator())
                    {
                        while (dit.MoveNext())
                        {
                            dat.MoveNext();
                            if (!itemequalityComparer.Equals(dit.Current, dat.Current))
                                return false;
                        }
                        return true;
                    }
                }
            }

            if (!collection1.AllowsDuplicates && (collection2.AllowsDuplicates || collection2.ContainsSpeed >= collection1.ContainsSpeed))
            {
                foreach (T x in collection1) if (!collection2.Contains(x)) return false;
            }
            else if (!collection2.AllowsDuplicates)
            {
                foreach (T x in collection2) if (!collection1.Contains(x)) return false;
            }
            // Now tit.AllowsDuplicates && tat.AllowsDuplicates
            else if (collection1.DuplicatesByCounting && collection2.DuplicatesByCounting)
            {
                foreach (T item in collection2) if (collection1.ContainsCount(item) != collection2.ContainsCount(item)) return false;
            }
            else
            {
                // To avoid an O(n^2) algorithm, we make an aux hashtable to hold the count of items
                // bug20101103: HashDictionary<T, int> dict = new HashDictionary<T, int>();
                HashDictionary<T, int> dict = new HashDictionary<T, int>(itemequalityComparer);
                foreach (T item in collection2)
                {
                    int count = 1;
                    if (dict.FindOrAdd(item, ref count))
                        dict[item] = count + 1;
                }
                foreach (T item in collection1)
                {
                    var i = item;
                    int count;
                    if (dict.Find(ref i, out count) && count > 0)
                        dict[item] = count - 1;
                    else
                        return false;
                }
                return true;
            }

            return true;
        }


        /// <summary>
        /// Get the unsequenced collection hash code of this collection: from the cached 
        /// value if present and up to date, else (re)compute.
        /// </summary>
        /// <returns>The hash code</returns>
        public virtual int GetUnsequencedHashCode()
        {
            if (iUnSequencedHashCodeStamp == stamp)
                return iUnSequencedHashCode;

            iUnSequencedHashCode = ComputeHashCode(this, itemequalityComparer);
            iUnSequencedHashCodeStamp = stamp;
            return iUnSequencedHashCode;
        }


        /// <summary>
        /// Check if the contents of otherCollection is equal to the contents of this
        /// in the unsequenced sense.  Uses the item equality comparer of this collection
        /// </summary>
        /// <param name="otherCollection">The collection to compare to.</param>
        /// <returns>True if  equal</returns>
        public virtual bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            return otherCollection != null && StaticEquals((ICollection<T>)this, otherCollection, itemequalityComparer);
        }


        /// <summary>
        /// Check if the collection has been modified since a specified time, expressed as a stamp value.
        /// </summary>
        /// <exception cref="CollectionModifiedException"> if this collection has been updated 
        /// since a target time</exception>
        /// <param name="thestamp">The stamp identifying the target time</param>
        protected virtual void modifycheck(int thestamp)
        {
            if (stamp != thestamp)
                throw new CollectionModifiedException();
        }


        /// <summary>
        /// Check if it is valid to perform update operations, and if so increment stamp.
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException">If collection is read-only</exception>
        protected virtual void updatecheck()
        {
            if (isReadOnlyBase)
                throw new ReadOnlyCollectionException();

            stamp++;
        }

        #endregion

        #region ICollection<T> members

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is read only</value>
        public virtual bool IsReadOnly { get { return isReadOnlyBase; } }

        #endregion

        #region ICollectionValue<T> members
        /// <summary>
        /// 
        /// </summary>
        /// <value>The size of this collection</value>
        public override int Count { get { return size; } }

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the 
        /// <code>Count</code> property in this collection.</value>
        public override Speed CountSpeed { get { return Speed.Constant; } }


        #endregion

        #region IExtensible<T> members

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual SCG.IEqualityComparer<T> EqualityComparer { get { return itemequalityComparer; } }

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is empty</value>
        public override bool IsEmpty { get { return size == 0; } }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DirectedCollectionBase<T> : CollectionBase<T>, IDirectedCollectionValue<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemequalityComparer"></param>
        /// <param name = "memoryType">The type of memory for the enumerator used to iterate the collection</param>
        protected DirectedCollectionBase(SCG.IEqualityComparer<T> itemequalityComparer, MemoryType memoryType) : base(itemequalityComparer, memoryType) { }
        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public virtual EnumerationDirection Direction { get { return EnumerationDirection.Forwards; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDirectedCollectionValue<T> Backwards();

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the first one in enumeration order.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
        /// <param name="item"></param>
        /// <returns>True is such an item exists</returns>
        public virtual bool FindLast(Func<T, bool> predicate, out T item)
        {
            foreach (T jtem in Backwards())
                if (predicate(jtem))
                {
                    item = jtem;
                    return true;
                }
            item = default(T);
            return false;
        }
    }

    /// <summary>
    /// Base class (abstract) for sequenced collection implementations.
    /// </summary>
    [Serializable]
    public abstract class SequencedBase<T> : DirectedCollectionBase<T>, IDirectedCollectionValue<T>
    {
        #region Fields

        int iSequencedHashCode, iSequencedHashCodeStamp = -1;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemequalityComparer"></param>
        /// <param name = "memoryType">The type of memory for the enumerator used to iterate the collection</param>
        protected SequencedBase(SCG.IEqualityComparer<T> itemequalityComparer, MemoryType memoryType) : base(itemequalityComparer, memoryType) { }

        #region Util

        //TODO: make random for release
        const int HASHFACTOR = 31;

        /// <summary>
        /// Compute the unsequenced hash code of a collection
        /// </summary>
        /// <param name="items">The collection to compute hash code for</param>
        /// <param name="itemequalityComparer">The item equalitySCG.Comparer</param>
        /// <returns>The hash code</returns>
        public static int ComputeHashCode(ISequenced<T> items, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            //NOTE: It must be possible to devise a much stronger combined hashcode, 
            //but unfortunately, it has to be universal. OR we could use a (strong)
            //family and initialise its parameter randomly at load time of this class!
            //(We would not want to have yet a flag to check for invalidation?!)
            //NBNBNB: the current hashcode has the very bad property that items with hashcode 0
            // is ignored.
            int iIndexedHashCode = 0;

            foreach (T item in items)
                iIndexedHashCode = iIndexedHashCode * HASHFACTOR + itemequalityComparer.GetHashCode(item);

            return iIndexedHashCode;
        }


        /// <summary>
        /// Examine if tit and tat are equal as sequenced collections
        /// using the specified item equalityComparer (assumed compatible with the two collections).
        /// </summary>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="itemequalityComparer">The item equalityComparer to use for comparison</param>
        /// <returns>True if equal</returns>
        public static bool StaticEquals(ISequenced<T> collection1, ISequenced<T> collection2, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            if (ReferenceEquals(collection1, collection2))
                return true;

            if (collection1.Count != collection2.Count)
                return false;

            //This way we might run through both enumerations twice, but
            //probably not (if the hash codes are good)
            if (collection1.GetSequencedHashCode() != collection2.GetSequencedHashCode())
                return false;

            using (SCG.IEnumerator<T> dat = collection2.GetEnumerator(), dit = collection1.GetEnumerator())
            {
                while (dit.MoveNext())
                {
                    dat.MoveNext();
                    if (!itemequalityComparer.Equals(dit.Current, dat.Current))
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Get the sequenced collection hash code of this collection: from the cached 
        /// value if present and up to date, else (re)compute.
        /// </summary>
        /// <returns>The hash code</returns>
        public virtual int GetSequencedHashCode()
        {
            if (iSequencedHashCodeStamp == stamp)
                return iSequencedHashCode;

            iSequencedHashCode = ComputeHashCode((ISequenced<T>)this, itemequalityComparer);
            iSequencedHashCodeStamp = stamp;
            return iSequencedHashCode;
        }


        /// <summary>
        /// Check if the contents of that is equal to the contents of this
        /// in the sequenced sense. Using the item equalityComparer of this collection.
        /// </summary>
        /// <param name="otherCollection">The collection to compare to.</param>
        /// <returns>True if  equal</returns>
        public virtual bool SequencedEquals(ISequenced<T> otherCollection)
        {
            return StaticEquals((ISequenced<T>)this, otherCollection, itemequalityComparer);
        }


        #endregion
         
        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public override EnumerationDirection Direction { get { return EnumerationDirection.Forwards; } }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the index of the first one.
        /// </summary>
        /// <param name="predicate">A delegate defining the predicate</param>
        /// <returns>the index, if found, a negative value else</returns>
        public int FindIndex(Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in this)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the index of the last one.
        /// </summary>
        /// <param name="predicate">A delegate defining the predicate</param>
        /// <returns>the index, if found, a negative value else</returns>
        public int FindLastIndex(Func<T, bool> predicate)
        {
            int index = Count - 1;
            foreach (T item in Backwards())
            {
                if (predicate(item))
                    return index;
                index--;
            }
            return -1;
        }

    }


    /// <summary>
    /// Base class for collection classes of dynamic array type implementations.
    /// </summary>
    [Serializable]
    public abstract class ArrayBase<T> : SequencedBase<T>
    {
        #region Fields
        /// <summary>
        /// The actual internal array container. Will be extended on demand.
        /// </summary>
        protected T[] array;

        /// <summary>
        /// The offset into the internal array container of the first item. The offset is 0 for a 
        /// base dynamic array and may be positive for an updatable view into a base dynamic array.
        /// </summary>
        protected int offsetField;

        private readonly Enumerator _internalEnumerator;
        #endregion

        #region Util
        /// <summary>
        /// Double the size of the internal array.
        /// </summary>
        protected virtual void expand()
        {
            expand(2 * array.Length, size);
        }


        /// <summary>
        /// Expand the internal array container.
        /// </summary>
        /// <param name="newcapacity">The new size of the internal array - 
        /// will be rounded upwards to a power of 2.</param>
        /// <param name="newsize">The (new) size of the (base) collection.</param>
        protected virtual void expand(int newcapacity, int newsize)
        {
            System.Diagnostics.Debug.Assert(newcapacity >= newsize);

            int newlength = array.Length;

            while (newlength < newcapacity) newlength *= 2;

            T[] newarray = new T[newlength];

            Array.Copy(array, newarray, newsize);
            array = newarray;
        }


        /// <summary>
        /// Insert an item at a specific index, moving items to the right
        /// upwards and expanding the array if necessary.
        /// </summary>
        /// <param name="i">The index at which to insert.</param>
        /// <param name="item">The item to insert.</param>
        protected virtual void InsertProtected(int i, T item)
        {
            if (size == array.Length)
                expand();

            if (i < size)
                Array.Copy(array, i, array, i + 1, size - i);

            array[i] = item;
            size++;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create an empty ArrayBase object.
        /// </summary>
        /// <param name="capacity">The initial capacity of the internal array container.
        /// Will be rounded upwards to the nearest power of 2 greater than or equal to 8.</param>
        /// <param name="itemequalityComparer">The item equalityComparer to use, primarily for item equality</param>
        /// <param name="memoryType">The type of memory for the enumerator used to iterate the collection</param>
        protected ArrayBase(int capacity, SCG.IEqualityComparer<T> itemequalityComparer, MemoryType memoryType)
            : base(itemequalityComparer, memoryType)
        {
            int newlength = 8;
            while (newlength < capacity) newlength *= 2;
            array = new T[newlength];

            _internalEnumerator = new Enumerator(this, memoryType);
        }



        #endregion

        #region IIndexed members
        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the arguments does not describe a 
        /// valid range in the indexed collection, cf. <see cref="M:C5.CollectionBase`1.checkRange(System.Int32,System.Int32)"/>.</exception>
        /// <value>The directed collection of items in a specific index interval.</value>
        /// <param name="start">The low index of the interval (inclusive).</param>
        /// <param name="count">The size of the range.</param>
        public virtual IDirectedCollectionValue<T> this[int start, int count]
        {
            get
            {
                checkRange(start, count);
                return new Range(this, start, count, true);
            }
        }

        #endregion

        #region IEditableCollection members
        /// <summary>
        /// Remove all items and reset size of internal array container.
        /// </summary>
        public virtual void Clear()
        {
            updatecheck();
            array = new T[8];
            size = 0;
        }


        /// <summary>
        /// Create an array containing (copies) of the items of this collection in enumeration order.
        /// </summary>
        /// <returns>The new array</returns>
        public override T[] ToArray()
        {
            T[] res = new T[size];

            Array.Copy(array, offsetField, res, 0, size);
            return res;
        }


        /// <summary>
        /// Perform an internal consistency (invariant) test on the array base.
        /// </summary>
        /// <returns>True if test succeeds.</returns>
        public virtual bool Check()
        {
            bool retval = true;

            if (size > array.Length)
            {
                Logger.Log(string.Format("Bad size ({0}) > array.Length ({1})", size, array.Length));
                return false;
            }

            for (int i = 0; i < size; i++)
            {
                if ((object)(array[i]) == null)
                {
                    Logger.Log(string.Format("Bad element: null at index {0}", i));
                    return false;
                }
            }

            return retval;
        }

        #endregion

        #region IDirectedCollection<T> Members

        /// <summary>
        /// Create a directed collection with the same contents as this one, but 
        /// opposite enumeration sequence.
        /// </summary>
        /// <returns>The mirrored collection.</returns>
        public override IDirectedCollectionValue<T> Backwards() { return this[0, size].Backwards(); }

        #endregion

        /// <summary>
        /// Choose some item of this collection. The result is the last item in the internal array,
        /// making it efficient to remove.
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override T Choose() { if (size > 0) return array[size - 1]; throw new NoSuchItemException(); }


        #region Private Enumerator

        [Serializable]
        private class Enumerator : MemorySafeEnumerator<T>
        {
            private ArrayBase<T> _internalList;

            private int _internalIncrementalIndex;
            private int _theStamp;
            private int _end;
            


            public Enumerator(ArrayBase<T> list, MemoryType memoryType)
                : base(memoryType)
            {
                _internalList = list;

            }

            internal void UpdateReference(ArrayBase<T> list, int start, int end, int theStamp)
            {
                _internalIncrementalIndex = start;
                _end = end;
                _internalList = list;
                Current = default(T);
                _theStamp = theStamp;
            }
             

            public override bool MoveNext()
            {
                ArrayBase<T> list = _internalList;

                if (list.stamp != _theStamp)
                    throw new CollectionModifiedException();

                if (_internalIncrementalIndex < _end)
                {
                    Current = list.array[_internalIncrementalIndex];
                    _internalIncrementalIndex++;

                    return true;
                }

                Current = default(T);
                return false;
            }

            public override void Reset()
            {
                _internalIncrementalIndex = 0;
                Current = default(T);
            }


            protected override MemorySafeEnumerator<T> Clone()
            {
                var enumerator = new Enumerator(_internalList, MemoryType)
                {
                    Current = default(T),

                };
                return enumerator;
            }
        }
        #endregion
        #region IEnumerable<T> Members
        /// <summary>
        /// Create an enumerator for this array based collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public override SCG.IEnumerator<T> GetEnumerator()
        {
            int thestamp = stamp, theend = size + offsetField, thestart = offsetField;

            var enumerator = (Enumerator)_internalEnumerator.GetEnumerator();

            enumerator.UpdateReference(this, thestart, theend, thestamp);

            return enumerator;
        }
        #endregion

        #region Range nested class
        /// <summary>
        /// A helper class for defining results of interval queries on array based collections.
        /// </summary>
        [Serializable]
        protected class Range : DirectedCollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            int start, count, delta, stamp;

            ArrayBase<T> thebase;

            private readonly RangeEnumerator _rangeInternalEnumerator;

            internal Range(ArrayBase<T> thebase, int start, int count, bool forwards, MemoryType memoryType = MemoryType.Normal)
            {

                this.thebase = thebase; stamp = thebase.stamp;
                delta = forwards ? 1 : -1;

                this.start = start + thebase.offsetField; this.count = count;
                _rangeInternalEnumerator = new RangeEnumerator(thebase, memoryType);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>True if this collection is empty.</value>
            public override bool IsEmpty { get { thebase.modifycheck(stamp); return count == 0; } }


            /// <summary>
            /// 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>The number of items in the range</value>
            public override int Count { get { thebase.modifycheck(stamp); return count; } }

            /// <summary>
            /// The value is symbolic indicating the type of asymptotic complexity
            /// in terms of the size of this collection (worst-case or amortized as
            /// relevant).
            /// </summary>
            /// <value>A characterization of the speed of the 
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <code>Count</code> property in this collection.</value>
            public override Speed CountSpeed { get { thebase.modifycheck(stamp); return Speed.Constant; } }

            /// <summary>
            /// Choose some item of this collection. 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <exception cref="NoSuchItemException">if range is empty.</exception>
            /// <returns></returns>
            public override T Choose()
            {
                thebase.modifycheck(stamp);
                if (count == 0)
                    throw new NoSuchItemException();
                return thebase.array[start];
            }


            /// <summary>
            /// Create an enumerator for this range of an array based collection.
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <returns>The enumerator</returns>
            public override SCG.IEnumerator<T> GetEnumerator()
            {
                var enumerator = (RangeEnumerator)_rangeInternalEnumerator.GetEnumerator();

                enumerator.UpdateReference(thebase, start, delta, stamp, count);

                return enumerator;
            }


            /// <summary>
            /// Create an array collection range with the same contents as this one, but 
            /// opposite enumeration sequence.
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <returns>The mirrored collection.</returns>
            public override IDirectedCollectionValue<T> Backwards()
            {
                thebase.modifycheck(stamp);

                Range res = (Range)MemberwiseClone();

                res.delta = -delta;
                res.start = start + (count - 1) * delta;
                return res;
            }


            IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
            {
                return Backwards();
            }

            private sealed class RangeEnumerator : MemorySafeEnumerator<T>
            {
                private ArrayBase<T> _rangeEnumeratorArrayBase;

                private int _start;
                private int _count;
                private int _theStamp;
                private int _delta;
                private int _index;


                public RangeEnumerator(ArrayBase<T> internalList, MemoryType memoryType)
                    : base(memoryType)
                {
                    _rangeEnumeratorArrayBase = internalList;
                    IteratorState = -1;
                    _index = 0;
                }

                internal void UpdateReference(ArrayBase<T> list, int start, int delta, int theStamp, int count)
                {
                    _count = count;
                    _start = start;
                    _delta = delta;
                    _rangeEnumeratorArrayBase = list;
                    Current = default(T);
                    _theStamp = theStamp;
                }


                protected override MemorySafeEnumerator<T> Clone()
                {
                    var enumerator = new RangeEnumerator(_rangeEnumeratorArrayBase, MemoryType)
                    {
                        Current = default(T),

                    };
                    return enumerator;
                }

                public override bool MoveNext()
                {
                    ArrayBase<T> list = _rangeEnumeratorArrayBase;

                    list.modifycheck(_theStamp);

                    if (_index < _count)
                    {
                        Current = list.array[_start + _delta * _index];
                        _index++;
                        return true;
                    }

                    Current = default(T);
                    return false;
                }

                public override void Reset()
                {
                    _index = 0;
                    Current = default(T);
                }
            }

            /// <summary>
            /// <code>Forwards</code> if same, else <code>Backwards</code>
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>The enumeration direction relative to the original collection.</value>
            public override EnumerationDirection Direction
            {
                get
                {
                    thebase.modifycheck(stamp);
                    return delta > 0 ? EnumerationDirection.Forwards : EnumerationDirection.Backwards;
                }
            }
        }
        #endregion
    }
}
