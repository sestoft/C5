using System;

namespace C5
{
    /// <summary>
    /// Base class for classes implementing ICollectionValue[T]
    /// </summary>
    [Serializable]
    public abstract class CollectionValueBase<T> : EnumerableBase<T>, ICollectionValue<T>, IShowable
    {
        #region Event handling
        private EventBlock<T>? eventBlock;
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual EventType ListenableEvents => 0;

        /// <summary>
        /// A flag bitmap of the events currently subscribed to by this collection.
        /// </summary>
        /// <value></value>
        public virtual EventType ActiveEvents => eventBlock == null ? 0 : eventBlock.events;

        private void CheckWillListen(EventType eventType)
        {
            if ((ListenableEvents & eventType) == 0)
            {
                throw new UnlistenableEventException();
            }
        }

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public virtual event CollectionChangedHandler<T> CollectionChanged
        {
            add { CheckWillListen(EventType.Changed); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionChanged += value; }
            remove
            {
                CheckWillListen(EventType.Changed);
                if (eventBlock != null)
                {
                    eventBlock.CollectionChanged -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }
        /// <summary>
        /// Fire the CollectionChanged event
        /// </summary>
        protected virtual void RaiseCollectionChanged()
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseCollectionChanged(this);
            }
        }

        /// <summary>
        /// The clear event. Will be raised for every Clear operation on the collection.
        /// </summary>
        public virtual event CollectionClearedHandler<T> CollectionCleared
        {
            add { CheckWillListen(EventType.Cleared); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionCleared += value; }
            remove
            {
                CheckWillListen(EventType.Cleared);
                if (eventBlock != null)
                {
                    eventBlock.CollectionCleared -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }
        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void RaiseCollectionCleared(bool full, int count)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseCollectionCleared(this, full, count);
            }
        }

        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void RaiseCollectionCleared(bool full, int count, int? offset)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseCollectionCleared(this, full, count, offset);
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemsAddedHandler<T> ItemsAdded
        {
            add { CheckWillListen(EventType.Added); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsAdded += value; }
            remove
            {
                CheckWillListen(EventType.Added);
                if (eventBlock != null)
                {
                    eventBlock.ItemsAdded -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }
        /// <summary>
        /// Fire the ItemsAdded event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="count"></param>
        protected virtual void RaiseItemsAdded(T item, int count)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseItemsAdded(this, item, count);
            }
        }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemsRemovedHandler<T> ItemsRemoved
        {
            add { CheckWillListen(EventType.Removed); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsRemoved += value; }
            remove
            {
                CheckWillListen(EventType.Removed);
                if (eventBlock != null)
                {
                    eventBlock.ItemsRemoved -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }

        /// <summary>
        /// Fire the ItemsRemoved event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="count"></param>
        protected virtual void RaiseItemsRemoved(T item, int count)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseItemsRemoved(this, item, count);
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemInsertedHandler<T> ItemInserted
        {
            add { CheckWillListen(EventType.Inserted); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemInserted += value; }
            remove
            {
                CheckWillListen(EventType.Inserted);
                if (eventBlock != null)
                {
                    eventBlock.ItemInserted -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }
        /// <summary>
        /// Fire the ItemInserted event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="index"></param>
        protected virtual void RaiseItemInserted(T item, int index)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseItemInserted(this, item, index);
            }
        }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add { CheckWillListen(EventType.RemovedAt); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemRemovedAt += value; }
            remove
            {
                CheckWillListen(EventType.RemovedAt);
                if (eventBlock != null)
                {
                    eventBlock.ItemRemovedAt -= value;
                    if (eventBlock.events == 0)
                    {
                        eventBlock = null;
                    }
                }
            }
        }
        /// <summary> 
        /// Fire the ItemRemovedAt event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="index"></param>
        protected virtual void RaiseItemRemovedAt(T item, int index)
        {
            if (eventBlock != null)
            {
                eventBlock.RaiseItemRemovedAt(this, item, index);
            }
        }

        #region Event support for IList
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        protected virtual void RaiseForSetThis(int index, T value, T item)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(item, 1);
                RaiseItemRemovedAt(item, index);
                RaiseItemsAdded(value, 1);
                RaiseItemInserted(value, index);
                RaiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        protected virtual void RaiseForInsert(int i, T item)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemInserted(item, i);
                RaiseItemsAdded(item, 1);
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected void RaiseForRemove(T item)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(item, 1);
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        protected void RaiseForRemove(T item, int count)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(item, count);
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void RaiseForRemoveAt(int index, T item)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemRemovedAt(item, index);
                RaiseItemsRemoved(item, 1);
                RaiseCollectionChanged();
            }
        }

        #endregion

        #region Event  Support for ICollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        protected virtual void RaiseForUpdate(T newitem, T olditem)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(olditem, 1);
                RaiseItemsAdded(newitem, 1);
                RaiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        /// <param name="count"></param>
        protected virtual void RaiseForUpdate(T newitem, T olditem, int count)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(olditem, count);
                RaiseItemsAdded(newitem, count);
                RaiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected virtual void RaiseForAdd(T item)
        {
            if (ActiveEvents != 0)
            {
                RaiseItemsAdded(item, 1);
                RaiseCollectionChanged();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wasRemoved"></param>
        protected virtual void RaiseForRemoveAll(ICollectionValue<T>? wasRemoved)
        {
            if ((ActiveEvents & EventType.Removed) != 0)
            {
                if (wasRemoved != null)
                {
                    foreach (T item in wasRemoved)
                    {
                        RaiseItemsRemoved(item, 1);
                    }
                }
            }

            if (wasRemoved != null && wasRemoved.Count > 0)
            {
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        protected class RaiseForRemoveAllHandler
        {
            private readonly CollectionValueBase<T> collection;
            private CircularQueue<T>? wasRemoved = null;
            private bool wasChanged = false;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="collection"></param>
            public RaiseForRemoveAllHandler(CollectionValueBase<T> collection)
            {
                this.collection = collection;
                mustFireRemoved = (collection.ActiveEvents & EventType.Removed) != 0;
                MustFire = (collection.ActiveEvents & (EventType.Removed | EventType.Changed)) != 0;
            }

            private readonly bool mustFireRemoved;

            /// <summary>
            /// 
            /// </summary>
            public bool MustFire { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public void Remove(T item)
            {
                if (mustFireRemoved)
                {
                    if (wasRemoved == null)
                    {
                        wasRemoved = new CircularQueue<T>();
                    }

                    wasRemoved.Enqueue(item);
                }
                if (!wasChanged)
                {
                    wasChanged = true;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Raise()
            {
                if (wasRemoved != null)
                {
                    foreach (T item in wasRemoved)
                    {
                        collection.RaiseItemsRemoved(item, 1);
                    }
                }

                if (wasChanged)
                {
                    collection.RaiseCollectionChanged();
                }
            }
        }
        #endregion

        #endregion

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
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (T item in this)
            {
                array[index++] = item;
            }
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

            foreach (T item in this)
            {
                res[i++] = item;
            }

            return res;
        }

        /// <summary>
        /// Apply an single argument action, <see cref="T:Action`1"/> to this enumerable
        /// </summary>
        /// <param name="action">The action delegate</param>
        public virtual void Apply(Action<T> action)
        {
            foreach (T item in this)
            {
                action(item);
            }
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
            {
                if (predicate(item))
                {
                    return true;
                }
            }

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
            {
                if (predicate(jtem))
                {
                    item = jtem;
                    return true;
                }
            }

            item = default;
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
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

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
        public virtual System.Collections.Generic.IEnumerable<T> Filter(Func<T, bool> predicate)
        {
            foreach (T item in this)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public abstract T Choose();


        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public abstract override System.Collections.Generic.IEnumerator<T> GetEnumerator();

        #region IShowable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            return Showing.ShowCollectionValue<T>(this, stringbuilder, ref rest, formatProvider!);
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
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

        #region SCG.ICollection<T> Members

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection{T}"/> is read-only.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Adds an item to the <see cref="System.Collections.Generic.ICollection{T}"/>.
        /// <para/>
        /// The default implementation throws a <see cref="ReadOnlyCollectionException"/>. Override to provide an implementation.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="System.Collections.Generic.ICollection{T}"/>.</param>
        public virtual bool Add(T item) => throw new ReadOnlyCollectionException();

        void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="System.Collections.Generic.ICollection{T}"/>.
        /// <para/>
        /// The default implementation throws a <see cref="ReadOnlyCollectionException"/>. Override to provide an implementation.
        /// </summary>
        public virtual void Clear() => throw new ReadOnlyCollectionException();

        /// <summary>
        /// Determines whether the <see cref="System.Collections.Generic.ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="System.Collections.Generic.ICollection{T}"/>.</param>
        /// <returns><c>true</c> if item is found in the <see cref="System.Collections.Generic.ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(T item) => this.Exists((thisItem) => EqualityComparer<T>.Default.Equals(thisItem, item));

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="System.Collections.Generic.ICollection{T}"/>.
        /// <para/>
        /// The default implementation throws a <see cref="ReadOnlyCollectionException"/>. Override to provide an implementation.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="System.Collections.Generic.ICollection{T}"/>.</param>
        /// <returns><c>true</c> if item was successfully removed from the <see cref="System.Collections.Generic.ICollection{T}"/>; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if item is not found in the original <see cref="System.Collections.Generic.ICollection{T}"/>.</returns>
        public virtual bool Remove(T item) => throw new ReadOnlyCollectionException();

        #endregion
    }
}