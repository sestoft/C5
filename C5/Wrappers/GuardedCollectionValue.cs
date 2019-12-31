using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for an ICollectionValue&lt;T&gt;
    ///
    /// <i>This is mainly interesting as a base of other guard classes</i>
    /// </summary>
    [Serializable]
    public class GuardedCollectionValue<T> : GuardedEnumerable<T>, ICollectionValue<T>
    {
        #region Events
        /// <summary>
        /// The ListenableEvents value of the wrapped collection
        /// </summary>
        /// <value></value>
        public virtual EventType ListenableEvents => collectionvalue.ListenableEvents;

        /// <summary>
        /// The ActiveEvents value of the wrapped collection
        /// </summary>
        /// <value></value>
        public virtual EventType ActiveEvents => collectionvalue.ActiveEvents;

        private ProxyEventBlock<T>? eventBlock;
        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public event CollectionChangedHandler<T> CollectionChanged
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).CollectionChanged += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.CollectionChanged -= value;
                }
            }
        }

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public event CollectionClearedHandler<T> CollectionCleared
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).CollectionCleared += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.CollectionCleared -= value;
                }
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public event ItemsAddedHandler<T> ItemsAdded
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).ItemsAdded += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemsAdded -= value;
                }
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public event ItemInsertedHandler<T> ItemInserted
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).ItemInserted += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemInserted -= value;
                }
            }
        }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public event ItemsRemovedHandler<T> ItemsRemoved
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).ItemsRemoved += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemsRemoved -= value;
                }
            }
        }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<T>(this, collectionvalue))).ItemRemovedAt += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemRemovedAt -= value;
                }
            }
        }
        #endregion

        #region Fields

        private readonly ICollectionValue<T> collectionvalue;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a ICollectionValue&lt;T&gt; in a read-only wrapper
        /// </summary>
        /// <param name="collectionvalue">the collection to wrap</param>
        public GuardedCollectionValue(ICollectionValue<T> collectionvalue)
            : base(collectionvalue)
        { this.collectionvalue = collectionvalue; }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Get the size of the wrapped collection
        /// </summary>
        /// <value>The size</value>
        public virtual bool IsEmpty => collectionvalue.IsEmpty;

        /// <summary>
        /// Get the size of the wrapped collection
        /// </summary>
        /// <value>The size</value>
        public virtual int Count => collectionvalue.Count;

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the 
        /// <code>Count</code> property in this collection.</value>
        public virtual Speed CountSpeed => collectionvalue.CountSpeed;

        /// <summary>
        /// Copy the items of the wrapped collection to an array
        /// </summary>
        /// <param name="a">The array</param>
        /// <param name="i">Starting offset</param>
        public virtual void CopyTo(T[] a, int i) { collectionvalue.CopyTo(a, i); }

        /// <summary>
        /// Create an array from the items of the wrapped collection
        /// </summary>
        /// <returns>The array</returns>
        public virtual T[] ToArray() { return collectionvalue.ToArray(); }

        /// <summary>
        /// Apply a delegate to all items of the wrapped enumerable.
        /// </summary>
        /// <param name="a">The delegate to apply</param>
        //TODO: change this to throw an exception?
        public virtual void Apply(Action<T> a) { collectionvalue.Apply(a); }


        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in the wrapped enumerable.
        /// </summary>
        /// <param name="filter">A filter delegate 
        /// (<see cref="T:C5.Filter`1"/>) defining the predicate</param>
        /// <returns>True is such an item exists</returns>
        public virtual bool Exists(Func<T, bool> filter) { return collectionvalue.Exists(filter); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Find(Func<T, bool> filter, out T item) { return collectionvalue.Find(filter, out item); }

        /// <summary>
        /// Check if all items in the wrapped enumerable satisfies a specific predicate.
        /// </summary>
        /// <param name="filter">A filter delegate 
        /// (<see cref="T:C5.Filter`1"/>) defining the predicate</param>
        /// <returns>True if all items satisfies the predicate</returns>
        public virtual bool All(Func<T, bool> filter) { return collectionvalue.All(filter); }

        /// <summary>
        /// Create an enumerable, enumerating the items of this collection that satisfies 
        /// a certain condition.
        /// </summary>
        /// <param name="filter">The T->bool filter delegate defining the condition</param>
        /// <returns>The filtered enumerable</returns>
        public virtual System.Collections.Generic.IEnumerable<T> Filter(Func<T, bool> filter) { return collectionvalue.Filter(filter); }

        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public virtual T Choose() { return collectionvalue.Choose(); }

        #endregion

        #region SCG.ICollection<T> Members

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection{T}"/> is read-only.
        /// </summary>
        public virtual bool IsReadOnly => collectionvalue.IsReadOnly;

        /// <summary>
        /// Adds an item to the <see cref="ICollectionValue{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollectionValue{T}"/>.</param>
        public virtual bool Add(T item) => collectionvalue.Add(item);

        void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="System.Collections.Generic.ICollection{T}"/>.
        /// </summary>
        public virtual void Clear() => collectionvalue.Clear();

        /// <summary>
        /// Determines whether the <see cref="System.Collections.Generic.ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="System.Collections.Generic.ICollection{T}"/>.</param>
        /// <returns><c>true</c> if item is found in the <see cref="System.Collections.Generic.ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(T item) => collectionvalue.Contains(item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="System.Collections.Generic.ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="System.Collections.Generic.ICollection{T}"/>.</param>
        /// <returns><c>true</c> if item was successfully removed from the <see cref="System.Collections.Generic.ICollection{T}"/>; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if item is not found in the original <see cref="System.Collections.Generic.ICollection{T}"/>.</returns>
        public virtual bool Remove(T item) => collectionvalue.Remove(item);

        #endregion

        #region IShowable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="formatProvider"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            return collectionvalue.Show(stringbuilder, ref rest, formatProvider);
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return collectionvalue.ToString(format, formatProvider);
        }

        #endregion
    }
}