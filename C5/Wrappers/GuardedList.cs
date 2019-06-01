using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a generic list collection
    /// <i>Suitable as a wrapper for LinkedList, HashedLinkedList, ArrayList and HashedArray.
    /// <see cref="T:C5.LinkedList`1"/>, 
    /// <see cref="T:C5.HashedLinkedList`1"/>, 
    /// <see cref="T:C5.ArrayList`1"/> or
    /// <see cref="T:C5.HashedArray`1"/>.
    /// </i>
    /// </summary>
    [Serializable]
    public class GuardedList<T> : GuardedSequenced<T>, IList<T>, System.Collections.Generic.IList<T>
    {
        #region Fields

        private readonly IList<T> innerlist;
        private readonly GuardedList<T>? underlying;
        private readonly bool slidableView = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a list in a read-only wrapper.  A list gets wrapped as read-only,
        /// a list view gets wrapped as read-only and non-slidable.
        /// </summary>
        /// <param name="list">The list</param>
        public GuardedList(IList<T> list)
            : base(list)
        {
            innerlist = list;
            // If wrapping a list view, make innerlist = the view, and make 
            // underlying = a guarded version of the view's underlying list
            if (list.Underlying != null)
            {
                underlying = new GuardedList<T>(list.Underlying, null, false);
            }
        }

        private GuardedList(IList<T> list, GuardedList<T>? underlying, bool slidableView)
            : base(list)
        {
            innerlist = list; this.underlying = underlying; this.slidableView = slidableView;
        }
        #endregion

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value>The first item of the wrapped list</value>
        public T First => innerlist.First;


        /// <summary>
        /// 
        /// </summary>
        /// <value>The last item of the wrapped list</value>
        public T Last => innerlist.Last;


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> if used as setter</exception>
        /// <value>True if wrapped list has FIFO semantics for the Add(T item) and Remove() methods</value>
        public bool FIFO
        {
            get => innerlist.FIFO;
            set => throw new ReadOnlyCollectionException("List is read only");
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsFixedSize => true;


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> if used as setter</exception>
        /// <value>The i'th item of the wrapped list</value>
        public T this[int i]
        {
            get => innerlist[i];
            set => throw new ReadOnlyCollectionException("List is read only");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed IndexingSpeed => innerlist.IndexingSpeed;

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new ReadOnlyCollectionException();
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="pointer"></param>
        /// <param name="item"></param>
        public void Insert(IList<T> pointer, T item)
        {
            throw new ReadOnlyCollectionException();
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="item"></param>
        public void InsertFirst(T item)
        {
            throw new ReadOnlyCollectionException("List is read only");
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="item"></param>
        public void InsertLast(T item)
        {
            throw new ReadOnlyCollectionException("List is read only");
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="item"></param>
        /// <param name="target"></param>
        public void InsertBefore(T item, T target)
        {
            throw new ReadOnlyCollectionException("List is read only");
        }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="item"></param>
        /// <param name="target"></param>
        public void InsertAfter(T item, T target)
        {
            throw new ReadOnlyCollectionException("List is read only");
        }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="i"></param>
        /// <param name="items"></param>
        public void InsertAll(int i, System.Collections.Generic.IEnumerable<T> items)
        { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// Perform FindAll on the wrapped list. The result is <b>not</b> necessarily read-only.
        /// </summary>
        /// <param name="filter">The filter to use</param>
        /// <returns></returns>
        public IList<T> FindAll(Func<T, bool> filter) { return innerlist.FindAll(filter); }


        /// <summary>
        /// Perform Map on the wrapped list. The result is <b>not</b> necessarily read-only.
        /// </summary>
        /// <typeparam name="V">The type of items of the new list</typeparam>
        /// <param name="mapper">The mapper to use.</param>
        /// <returns>The mapped list</returns>
        public IList<V> Map<V>(Func<T, V> mapper) { return innerlist.Map(mapper); }

        /// <summary>
        /// Perform Map on the wrapped list. The result is <b>not</b> necessarily read-only.
        /// </summary>
        /// <typeparam name="V">The type of items of the new list</typeparam>
        /// <param name="mapper">The delegate defining the map.</param>
        /// <param name="itemequalityComparer">The itemequalityComparer to use for the new list</param>
        /// <returns>The new list.</returns>
        public IList<V> Map<V>(Func<T, V> mapper, System.Collections.Generic.IEqualityComparer<V> itemequalityComparer) { return innerlist.Map(mapper, itemequalityComparer); }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public T Remove() { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public T RemoveFirst() { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public T RemoveLast() { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// Create the indicated view on the wrapped list and wrap it read-only.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IList<T>? View(int start, int count)
        {
            IList<T>? view = innerlist.View(start, count);
            return view == null ? null : new GuardedList<T>(view, underlying ?? this, true);
        }

        /// <summary>
        /// Create the indicated view on the wrapped list and wrap it read-only.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<T>? ViewOf(T item)
        {
            IList<T>? view = innerlist.ViewOf(item);
            return view == null ? null : new GuardedList<T>(view, underlying ?? this, true);
        }

        /// <summary>
        /// Create the indicated view on the wrapped list and wrap it read-only.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<T>? LastViewOf(T item)
        {
            IList<T>? view = innerlist.LastViewOf(item);
            return view == null ? null : new GuardedList<T>(view, underlying ?? this, true);
        }


        /// <summary>
        /// </summary>
        /// <value>The wrapped underlying list of the wrapped view </value>
        public IList<T>? Underlying => underlying;


        /// <summary>
        /// 
        /// </summary>
        /// <value>The offset of the wrapped list as a view.</value>
        public int Offset => innerlist.Offset;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual bool IsValid => innerlist.IsValid;

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> if this is a wrapped view and not a view that was made on a wrapper</exception>
        /// <param name="offset"></param>
        public IList<T> Slide(int offset)
        {
            if (slidableView)
            {
                innerlist.Slide(offset);
                return this;
            }
            else
            {
                throw new ReadOnlyCollectionException("List is read only");
            }
        }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public IList<T> Slide(int offset, int size)
        {
            if (slidableView)
            {
                innerlist.Slide(offset, size);
                return this;
            }
            else
            {
                throw new ReadOnlyCollectionException("List is read only");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool TrySlide(int offset)
        {
            if (slidableView)
            {
                return innerlist.TrySlide(offset);
            }
            else
            {
                throw new ReadOnlyCollectionException("List is read only");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool TrySlide(int offset, int size)
        {
            if (slidableView)
            {
                return innerlist.TrySlide(offset, size);
            }
            else
            {
                throw new ReadOnlyCollectionException("List is read only");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherView"></param>
        /// <returns></returns>
        public IList<T>? Span(IList<T> otherView)
        {
            if (!(otherView is GuardedList<T> otherGuardedList))
            {
                throw new IncompatibleViewException();
            }

            IList<T>? span = innerlist.Span(otherGuardedList.innerlist);
            if (span == null)
            {
                return null;
            }

            return new GuardedList<T>(span, underlying ?? otherGuardedList.underlying ?? this, true);
        }

        /// <summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// </summary>
        public void Reverse() { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void Reverse(int start, int count)
        { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// Check if wrapped list is sorted according to the default sorting order
        /// for the item type T, as defined by the <see cref="T:C5.Comparer`1"/> class 
        /// </summary>
        /// <exception cref="NotComparableException">if T is not comparable</exception>
        /// <returns>True if the list is sorted, else false.</returns>
        public bool IsSorted() { return innerlist.IsSorted(System.Collections.Generic.Comparer<T>.Default); }

        /// <summary>
        /// Check if wrapped list is sorted
        /// </summary>
        /// <param name="c">The sorting order to use</param>
        /// <returns>True if sorted</returns>
        public bool IsSorted(System.Collections.Generic.IComparer<T> c) { return innerlist.IsSorted(c); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        public void Sort()
        { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="c"></param>
        public void Sort(System.Collections.Generic.IComparer<T> c)
        { throw new ReadOnlyCollectionException("List is read only"); }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        public void Shuffle()
        { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="rnd"></param>
        public void Shuffle(Random rnd)
        { throw new ReadOnlyCollectionException("List is read only"); }

        #endregion

        #region IIndexed<T> Members

        /// <summary> </summary>
        /// <value>A directed collection of the items in the indicated interval of the wrapped collection</value>
        public IDirectedCollectionValue<T> this[int start, int end] => new GuardedDirectedCollectionValue<T>(innerlist[start, end]);


        /// <summary>
        /// Find the (first) index of an item in the wrapped collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) { return innerlist.IndexOf(item); }


        /// <summary>
        /// Find the last index of an item in the wrapped collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int LastIndexOf(T item) { return innerlist.LastIndexOf(item); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="i"></param>
        /// <returns></returns>
        public T RemoveAt(int i)
        { throw new ReadOnlyCollectionException("List is read only"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void RemoveInterval(int start, int count)
        { throw new ReadOnlyCollectionException("List is read only"); }

        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
        { return Backwards(); }

        #endregion

        #region IStack<T> Members


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public void Push(T item)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public T Pop()
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        #endregion

        #region IQueue<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public void Enqueue(T item)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public T Dequeue()
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Ignore: this may be called by a foreach or using statement.
        /// </summary>
        public void Dispose() { }

        #endregion

        #region System.Collections.Generic.IList<T> Members

        void System.Collections.Generic.IList<T>.RemoveAt(int index)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        void System.Collections.Generic.ICollection<T>.Add(T item)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        #endregion

        #region System.Collections.ICollection Members

        bool System.Collections.ICollection.IsSynchronized => false;

        [Obsolete]
        Object System.Collections.ICollection.SyncRoot => innerlist.SyncRoot;

        void System.Collections.ICollection.CopyTo(Array arr, int index)
        {
            if (index < 0 || index + Count > arr.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (T item in this)
            {
                arr.SetValue(item, index++);
            }
        }

        #endregion

        #region System.Collections.IList Members

        Object System.Collections.IList.this[int index]
        {
            get => this[index]!;
            set => throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        int System.Collections.IList.Add(Object o)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        bool System.Collections.IList.Contains(Object o)
        {
            return Contains((T)o);
        }

        int System.Collections.IList.IndexOf(Object o)
        {
            return Math.Max(-1, IndexOf((T)o));
        }

        void System.Collections.IList.Insert(int index, Object o)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        void System.Collections.IList.Remove(Object o)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
        }

        #endregion
    }
}