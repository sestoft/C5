
// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A list collection based on a plain dynamic array data structure.
/// Expansion of the internal array is performed by doubling on demand.
/// The internal array is only shrinked by the Clear method.
///
/// <i>When the FIFO property is set to false this class works fine as a stack of T.
/// When the FIFO property is set to true the class will function as a (FIFO) queue
/// but very inefficiently, use a LinkedList (<see cref="T:C5.LinkedList`1"/>) instead.</i>
/// </summary>
public class HashedArrayList<T> : ArrayBase<T>, IList<T>, SCG.IList<T>
{
    #region Fields

    /// <summary>
    /// Has this list or view not been invalidated by some operation (by someone calling Dispose())
    /// </summary>
    private bool isValid = true;

    //TODO: wonder if we should save some memory on none-view situations by
    //      putting these three fields into a single ref field?
    /// <summary>
    /// The underlying list if we are a view, null else.
    /// </summary>
    private HashedArrayList<T>? underlying;
    private WeakViewList<HashedArrayList<T>>? views;
    private WeakViewList<HashedArrayList<T>>.Node? myWeakReference;

    /// <summary>
    /// The size of the underlying list.
    /// </summary>
    private int UnderlyingSize => (underlying ?? this).size;

    /// <summary>
    /// The underlying field of the FIFO property
    /// </summary>
    private bool fIFO = false;
    private readonly HashSet<System.Collections.Generic.KeyValuePair<T, int>> itemIndex;
    #endregion
    #region Events

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public override EventType ListenableEvents => underlying == null ? EventType.All : EventType.None;

    /*
        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event CollectionChangedHandler<T> CollectionChanged
        {
          add
          {
            if (underlying == null)
              base.CollectionChanged += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.CollectionChanged -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event CollectionClearedHandler<T> CollectionCleared
        {
          add
          {
            if (underlying == null)
              base.CollectionCleared += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.CollectionCleared -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event ItemsAddedHandler<T> ItemsAdded
        {
          add
          {
            if (underlying == null)
              base.ItemsAdded += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.ItemsAdded -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event ItemInsertedHandler<T> ItemInserted
        {
          add
          {
            if (underlying == null)
              base.ItemInserted += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.ItemInserted -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event ItemsRemovedHandler<T> ItemsRemoved
        {
          add
          {
            if (underlying == null)
              base.ItemsRemoved += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.ItemsRemoved -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override event ItemRemovedAtHandler<T> ItemRemovedAt
        {
          add
          {
            if (underlying == null)
              base.ItemRemovedAt += value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
          remove
          {
            if (underlying == null)
              base.ItemRemovedAt -= value;
            else
              throw new UnlistenableEventException("Can't listen to a view");
          }
        }

          */

    #endregion
    #region Util

    private bool Equals(T i1, T i2) { return itemEqualityComparer.Equals(i1, i2); }

    /// <summary>
    /// Increment or decrement the private size fields
    /// </summary>
    /// <param name="delta">Increment (with sign)</param>
    private void AddToSize(int delta)
    {
        size += delta;
        if (underlying != null)
        {
            underlying.size += delta;
        }
    }

    #region Array handling
    /// <summary>
    /// Double the size of the internal array.
    /// </summary>
    protected override void Expand()
    { Expand(2 * array.Length, UnderlyingSize); }


    /// <summary>
    /// Expand the internal array, resetting the index of the first unused element.
    /// </summary>
    /// <param name="newcapacity">The new capacity (will be rounded upwards to a power of 2).</param>
    /// <param name="newsize">The new count of </param>
    protected override void Expand(int newcapacity, int newsize)
    {
        if (underlying != null)
        {
            underlying.Expand(newcapacity, newsize);
        }
        else
        {
            base.Expand(newcapacity, newsize);
            if (views != null)
            {
                foreach (HashedArrayList<T> v in views)
                {
                    v.array = array;
                }
            }
        }
    }

    #endregion

    #region Checks
    /// <summary>
    /// Check if it is valid to perform updates and increment stamp if so.
    /// </summary>
    /// <exception cref="ViewDisposedException"> If check fails by this list being a disposed view.</exception>
    /// <exception cref="ReadOnlyCollectionException"> If check fails by this being a read only list.</exception>
    protected override void UpdateCheck()
    {
        ValidityCheck();
        base.UpdateCheck();
        if (underlying != null)
        {
            underlying.stamp++;
        }
    }


    /// <summary>
    /// Check if we are a view that the underlying list has only been updated through us.
    /// <para>This method should be called from enumerators etc to guard against
    /// modification of the base collection.</para>
    /// </summary>
    /// <exception cref="ViewDisposedException"> if check fails.</exception>
    private void ValidityCheck()
    {
        if (!isValid)
        {
            throw new ViewDisposedException();
        }
    }


    /// <summary>
    /// Check that the list has not been updated since a particular time.
    /// <para>To be used by enumerators and range </para>
    /// </summary>
    /// <exception cref="ViewDisposedException"> If check fails by this list being a disposed view.</exception>
    /// <exception cref="CollectionModifiedException">If the list *has* been updated since that  time..</exception>
    /// <param name="stamp">The stamp indicating the time.</param>
    protected override void ModifyCheck(int stamp)
    {
        ValidityCheck();
        if (this.stamp != stamp)
        {
            throw new CollectionModifiedException();
        }
    }

    #endregion

    #region Searching

    /// <summary>
    /// Internal version of IndexOf without modification checks.
    /// </summary>
    /// <param name="item">Item to look for</param>
    /// <returns>The index of first occurrence</returns>
    private int IndexOfInner(T item)
    {
        System.Collections.Generic.KeyValuePair<T, int> p = new(item, default);
        if (itemIndex.Find(ref p) && p.Value >= offsetField && p.Value < offsetField + size)
        {
            return p.Value - offsetField;
        }

        return ~size;
    }

    /// <summary>
    /// Internal version of LastIndexOf without modification checks.
    /// </summary>
    /// <param name="item">Item to look for</param>
    /// <returns>The index of last occurrence</returns>
    private int LastIndexOfInner(T item)
    {
        return IndexOfInner(item);
    }
    #endregion

    #region Inserting

    /// <summary>
    /// Internal version of Insert with no modification checks.
    /// </summary>
    /// <exception cref="DuplicateNotAllowedException"> if item already in list.</exception>
    /// <param name="i">Index to insert at</param>
    /// <param name="item">Item to insert</param>
    protected override void InsertProtected(int i, T item)
    {
        System.Collections.Generic.KeyValuePair<T, int> p = new(item, offsetField + i);
        if (itemIndex.FindOrAdd(ref p))
        {
            throw new DuplicateNotAllowedException("Item already in indexed list: " + item);
        }

        InsertBase(i, item);
        ReIndex(i + offsetField + 1);
    }

    private void InsertBase(int i, T item)
    {
        if (UnderlyingSize == array.Length)
        {
            Expand();
        }

        i += offsetField;
        if (i < UnderlyingSize)
        {
            Array.Copy(array, i, array, i + 1, UnderlyingSize - i);
        }

        array[i] = item;
        AddToSize(1);
        FixViewsAfterInsert(1, i);
    }
    #endregion

    #region Removing

    /// <summary>
    /// Internal version of RemoveAt with no modification checks.
    /// </summary>
    /// <param name="i">Index to remove at</param>
    /// <returns>The removed item</returns>
    private T RemoveAtInternal(int i)
    {
        i += offsetField;
        FixViewsBeforeSingleRemove(i);
        T retval = array[i];
        AddToSize(-1);
        if (UnderlyingSize > i)
        {
            Array.Copy(array, i + 1, array, i, UnderlyingSize - i);
        }

        array[UnderlyingSize] = default;
        itemIndex.Remove(new System.Collections.Generic.KeyValuePair<T, int>(retval, default));
        ReIndex(i);
        return retval;
    }
    #endregion

    #region Indexing

    private void ReIndex(int start) { ReIndex(start, UnderlyingSize); }

    private void ReIndex(int start, int end)
    {
        for (int j = start; j < end; j++)
        {
            itemIndex.UpdateOrAdd(new System.Collections.Generic.KeyValuePair<T, int>(array[j], j));
        }
    }
    #endregion

    #region fixView utilities

    /// <summary>
    ///
    /// </summary>
    /// <param name="added">The actual number of inserted nodes</param>
    /// <param name="realInsertionIndex"></param>
    private void FixViewsAfterInsert(int added, int realInsertionIndex)
    {
        if (views != null)
        {
            foreach (HashedArrayList<T> view in views)
            {
                if (view != this)
                {
                    if (view.offsetField < realInsertionIndex && view.offsetField + view.size > realInsertionIndex)
                    {
                        view.size += added;
                    }

                    if (view.offsetField > realInsertionIndex || (view.offsetField == realInsertionIndex && view.size > 0))
                    {
                        view.offsetField += added;
                    }
                }
            }
        }
    }

    private void FixViewsBeforeSingleRemove(int realRemovalIndex)
    {
        if (views != null)
        {
            foreach (HashedArrayList<T> view in views)
            {
                if (view != this)
                {
                    if (view.offsetField <= realRemovalIndex && view.offsetField + view.size > realRemovalIndex)
                    {
                        view.size--;
                    }

                    if (view.offsetField > realRemovalIndex)
                    {
                        view.offsetField--;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fix offsets and sizes of other views before removing an interval from this
    /// </summary>
    /// <param name="start">the start of the interval relative to the array/underlying</param>
    /// <param name="count"></param>
    private void FixViewsBeforeRemove(int start, int count)
    {
        int clearend = start + count - 1;
        if (views != null)
        {
            foreach (HashedArrayList<T> view in views)
            {
                if (view == this)
                {
                    continue;
                }

                int viewoffset = view.offsetField, viewend = viewoffset + view.size - 1;
                if (start < viewoffset)
                {
                    if (clearend < viewoffset)
                    {
                        view.offsetField = viewoffset - count;
                    }
                    else
                    {
                        view.offsetField = start;
                        view.size = clearend < viewend ? viewend - clearend : 0;
                    }
                }
                else if (start <= viewend)
                {
                    view.size = clearend <= viewend ? view.size - count : start - viewoffset;
                }
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="otherOffset"></param>
    /// <param name="otherSize"></param>
    /// <returns>The position of View(otherOffset, otherSize) wrt. this view</returns>
    private MutualViewPosition ViewPosition(int otherOffset, int otherSize)
    {
        int end = offsetField + size, otherEnd = otherOffset + otherSize;
        if (otherOffset >= end || otherEnd <= offsetField)
        {
            return MutualViewPosition.NonOverlapping;
        }

        if (size == 0 || (otherOffset <= offsetField && end <= otherEnd))
        {
            return MutualViewPosition.Contains;
        }

        if (otherSize == 0 || (offsetField <= otherOffset && otherEnd <= end))
        {
            return MutualViewPosition.ContainedIn;
        }

        return MutualViewPosition.Overlapping;
    }

    //TODO: make version that fits the new, more forgiving rules for disposing
    private void DisposeOverlappingViews(bool reverse)
    {
        if (views != null)
        {
            foreach (HashedArrayList<T> view in views)
            {
                if (view != this)
                {
                    switch (ViewPosition(view.offsetField, view.size))
                    {
                        case MutualViewPosition.ContainedIn:
                            if (reverse)
                            {
                                view.offsetField = 2 * offsetField + size - view.size - view.offsetField;
                            }
                            else
                            {
                                view.Dispose();
                            }

                            break;
                        case MutualViewPosition.Overlapping:
                            view.Dispose();
                            break;
                        case MutualViewPosition.Contains:
                        case MutualViewPosition.NonOverlapping:
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #endregion

    #region Position, PositionComparer and ViewHandler nested types
    private class PositionComparer : SCG.IComparer<Position>
    {
        public int Compare(Position a, Position b)
        {
            return a.Index.CompareTo(b.Index);
        }
    }

    /// <summary>
    /// During RemoveAll, we need to cache the original endpoint indices of views (??? also for HashedArrayList?)
    /// </summary>
    private struct Position
    {
        public HashedArrayList<T>? View { get; }

        public int Index { get; }

        public Position(HashedArrayList<T> view, bool left)
        {
            View = view;
            Index = left ? view.offsetField : view.offsetField + view.size - 1;
        }

        public Position(int index)
        {
            Index = index;
            View = null;
        }
    }

    /// <summary>
    /// Handle the update of (other) views during a multi-remove operation.
    /// </summary>
    private struct ViewHandler
    {
        private readonly HashedArrayList<Position>? leftEnds;
        private readonly HashedArrayList<Position>? rightEnds;
        private int leftEndIndex, rightEndIndex;
        internal readonly int viewCount;
        internal ViewHandler(HashedArrayList<T> list)
        {
            leftEndIndex = rightEndIndex = viewCount = 0;
            leftEnds = rightEnds = null;
            if (list.views != null)
            {
                foreach (HashedArrayList<T> v in list.views)
                {
                    if (v != list)
                    {
                        if (leftEnds == null || rightEnds == null)
                        {
                            leftEnds = new HashedArrayList<Position>();
                            rightEnds = new HashedArrayList<Position>();
                        }
                        leftEnds.Add(new Position(v, true));
                        rightEnds.Add(new Position(v, false));
                    }
                }
            }

            if (leftEnds == null || rightEnds == null)
            {
                return;
            }

            viewCount = leftEnds.Count;
            leftEnds.Sort(new PositionComparer());
            rightEnds.Sort(new PositionComparer());
        }
        /// <summary>
        /// This is to be called with realindex pointing to the first node to be removed after a (stretch of) node that was not removed
        /// </summary>
        /// <param name="removed"></param>
        /// <param name="realindex"></param>
        internal void SkipEndpoints(int removed, int realindex)
        {
            if (viewCount > 0)
            {
                Position endpoint;
                while (leftEndIndex < viewCount && (endpoint = leftEnds![leftEndIndex]).Index <= realindex)
                {
                    HashedArrayList<T> view = endpoint.View!;
                    view.offsetField -= removed;
                    view.size += removed;
                    leftEndIndex++;
                }
                while (rightEndIndex < viewCount && (endpoint = rightEnds![rightEndIndex]).Index < realindex)
                {
                    endpoint.View!.size -= removed;
                    rightEndIndex++;
                }
            }
        }
        internal void UpdateViewSizesAndCounts(int removed, int realindex)
        {
            if (viewCount > 0)
            {
                Position endpoint;
                while (leftEndIndex < viewCount && (endpoint = leftEnds![leftEndIndex]).Index <= realindex)
                {
                    HashedArrayList<T> view = endpoint.View!;
                    view!.offsetField = view.Offset - removed;
                    view.size += removed;
                    leftEndIndex++;
                }
                while (rightEndIndex < viewCount && (endpoint = rightEnds![rightEndIndex]).Index < realindex)
                {
                    endpoint.View!.size -= removed;
                    rightEndIndex++;
                }
            }
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Create an array list with default item equalityComparer and initial capacity 8 items.
    /// </summary>
    public HashedArrayList() : this(8) { }


    /// <summary>
    /// Create an array list with external item equalityComparer and initial capacity 8 items.
    /// </summary>
    /// <param name="itemequalityComparer">The external item equalitySCG.Comparer</param>
    public HashedArrayList(SCG.IEqualityComparer<T> itemequalityComparer) : this(8, itemequalityComparer) { }


    /// <summary>
    /// Create an array list with default item equalityComparer and prescribed initial capacity.
    /// </summary>
    /// <param name="capacity">The prescribed capacity</param>
    public HashedArrayList(int capacity) : this(capacity, EqualityComparer<T>.Default) { }


    /// <summary>
    /// Create an array list with external item equalityComparer and prescribed initial capacity.
    /// </summary>
    /// <param name="capacity">The prescribed capacity</param>
    /// <param name="itemequalityComparer">The external item equalitySCG.Comparer</param>
    public HashedArrayList(int capacity, SCG.IEqualityComparer<T> itemequalityComparer)
        : base(capacity, itemequalityComparer)
    {
        itemIndex = new HashSet<System.Collections.Generic.KeyValuePair<T, int>>(new KeyValuePairEqualityComparer<T, int>(itemequalityComparer));
    }

    #endregion

    #region IList<T> Members

    /// <summary>
    /// </summary>
    /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
    /// <value>The first item in this list.</value>
    public virtual T First
    {
        get
        {
            ValidityCheck();
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            return array[offsetField];
        }
    }


    /// <summary>
    /// </summary>
    /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
    /// <value>The last item in this list.</value>
    public virtual T Last
    {
        get
        {
            ValidityCheck();
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            return array[offsetField + size - 1];
        }
    }


    /// <summary>
    /// Since <code>Add(T item)</code> always add at the end of the list,
    /// this describes if list has FIFO or LIFO semantics.
    /// </summary>
    /// <value>True if the <code>Remove()</code> operation removes from the
    /// start of the list, false if it removes from the end. The default for a new array list is false.</value>
    public virtual bool FIFO
    {
        get { ValidityCheck(); return fIFO; }
        set { UpdateCheck(); fIFO = value; }
    }

    /// <summary>
    ///
    /// </summary>
    public virtual bool IsFixedSize
    {
        get { ValidityCheck(); return false; }
    }


    /// <summary>
    /// On this list, this indexer is read/write.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"> if index is negative or
    /// &gt;= the size of the collection.</exception>
    /// <exception cref="DuplicateNotAllowedException"> By the get operation
    /// if the item already is present somewhere else in the list.</exception>
    /// <value>The index'th item of this list.</value>
    /// <param name="index">The index of the item to fetch or store.</param>
    public virtual T this[int index]
    {
        get
        {
            ValidityCheck();
            if (index < 0 || index >= size)
            {
                throw new IndexOutOfRangeException();
            }

            return array[offsetField + index];
        }
        set
        {
            UpdateCheck();
            if (index < 0 || index >= size)
            {
                throw new IndexOutOfRangeException();
            }

            index += offsetField;
            T item = array[index];

            System.Collections.Generic.KeyValuePair<T, int> p = new(value, index);
            if (itemEqualityComparer.Equals(value, item))
            {
                array[index] = value;
                itemIndex.Update(p);
            }
            else if (!itemIndex.FindOrAdd(ref p))
            {
                itemIndex.Remove(new System.Collections.Generic.KeyValuePair<T, int>(item, default));
                array[index] = value;
            }
            else
            {
                throw new DuplicateNotAllowedException("Item already in indexed list");
            } (underlying ?? this).RaiseForSetThis(index, value, item);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public virtual Speed IndexingSpeed => Speed.Constant;


    /// <summary>
    /// Insert an item at a specific index location in this list.
    ///</summary>
    /// <exception cref="IndexOutOfRangeException"> if index is negative or
    /// &gt; the size of the collection. </exception>
    /// <exception cref="DuplicateNotAllowedException">
    /// If the item is already present in the list.</exception>
    /// <param name="index">The index at which to insert.</param>
    /// <param name="item">The item to insert.</param>
    public virtual void Insert(int index, T item)
    {
        UpdateCheck();
        if (index < 0 || index > size)
        {
            throw new IndexOutOfRangeException();
        }

        InsertProtected(index, item);
        (underlying ?? this).RaiseForInsert(index + offsetField, item);
    }

    /// <summary>
    /// Insert an item at the end of a compatible view, used as a pointer.
    /// <para>The <code>pointer</code> must be a view on the same list as
    /// <code>this</code> and the endpoint of <code>pointer</code> must be
    /// a valid insertion point of <code>this</code></para>
    /// </summary>
    /// <exception cref="IncompatibleViewException">If <code>pointer</code>
    /// is not a view on or the same list as <code>this</code></exception>
    /// <exception cref="IndexOutOfRangeException"><b>??????</b> if the endpoint of
    ///  <code>pointer</code> is not inside <code>this</code></exception>
    /// <exception cref="DuplicateNotAllowedException"> if the list has
    /// <code>AllowsDuplicates==false</code> and the item is
    /// already in the list.</exception>
    /// <param name="pointer"></param>
    /// <param name="item"></param>
    public void Insert(IList<T> pointer, T item)
    {
        if ((pointer == null) || ((pointer.Underlying ?? pointer) != (underlying ?? this)))
        {
            throw new IncompatibleViewException();
        }

        Insert(pointer.Offset + pointer.Count - Offset, item);
    }

    /// <summary>
    /// Insert into this list all items from an enumerable collection starting
    /// at a particular index.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"> if index is negative or
    /// &gt; the size of the collection.</exception>
    /// <exception cref="DuplicateNotAllowedException"> If <code>items</code>
    /// contains duplicates or some item already  present in the list.</exception>
    /// <param name="index">Index to start inserting at</param>
    /// <param name="items">Items to insert</param>
    public virtual void InsertAll(int index, SCG.IEnumerable<T> items)
    {
        UpdateCheck();
        if (index < 0 || index > size)
        {
            throw new IndexOutOfRangeException();
        }

        index += offsetField;
        int toadd = CountItems(items);
        if (toadd == 0)
        {
            return;
        }

        if (toadd + UnderlyingSize > array.Length)
        {
            Expand(toadd + UnderlyingSize, UnderlyingSize);
        }

        if (UnderlyingSize > index)
        {
            Array.Copy(array, index, array, index + toadd, UnderlyingSize - index);
        }

        int i = index;
        try
        {

            foreach (T item in items)
            {
                System.Collections.Generic.KeyValuePair<T, int> p = new(item, i);
                if (itemIndex.FindOrAdd(ref p))
                {
                    throw new DuplicateNotAllowedException("Item already in indexed list");
                }

                array[i++] = item;
            }
        }
        finally
        {
            int added = i - index;
            if (added < toadd)
            {
                Array.Copy(array, index + toadd, array, i, UnderlyingSize - index);
                Array.Clear(array, UnderlyingSize + added, toadd - added);
            }
            if (added > 0)
            {
                AddToSize(added);
                ReIndex(i);
                FixViewsAfterInsert(added, index);
                (underlying ?? this).RaiseForInsertAll(index, added);
            }
        }
    }
    private void RaiseForInsertAll(int index, int added)
    {
        if (ActiveEvents != 0)
        {
            if ((ActiveEvents & (EventType.Added | EventType.Inserted)) != 0)
            {
                for (int j = index; j < index + added; j++)
                {
                    RaiseItemInserted(array[j], j);
                    RaiseItemsAdded(array[j], 1);
                }
            }

            RaiseCollectionChanged();
        }
    }

    /// <summary>
    /// Insert an item at the front of this list;
    /// </summary>
    /// <exception cref="DuplicateNotAllowedException">If the item is already in the list</exception>
    /// <param name="item">The item to insert.</param>
    public virtual void InsertFirst(T item)
    {
        UpdateCheck();
        InsertProtected(0, item);
        (underlying ?? this).RaiseForInsert(offsetField, item);
    }


    /// <summary>
    /// Insert an item at the back of this list.
    /// </summary>
    /// <exception cref="DuplicateNotAllowedException">If the item is already in the list</exception>
    /// <param name="item">The item to insert.</param>
    public virtual void InsertLast(T item)
    {
        UpdateCheck();
        InsertProtected(size, item);
        (underlying ?? this).RaiseForInsert(size - 1 + offsetField, item);
    }


    //NOTE: if the filter throws an exception, no result will be returned.
    /// <summary>
    /// Create a new list consisting of the items of this list satisfying a
    /// certain predicate.
    /// <para>The new list will be of type HashedArrayList</para>
    /// </summary>
    /// <param name="filter">The filter delegate defining the predicate.</param>
    /// <returns>The new list.</returns>
    public virtual IList<T> FindAll(Func<T, bool> filter)
    {
        ValidityCheck();
        int stamp = this.stamp;
        HashedArrayList<T> res = new(itemEqualityComparer);
        int j = 0, rescap = res.array.Length;
        for (int i = 0; i < size; i++)
        {
            T a = array[offsetField + i];
            bool found = filter(a);
            ModifyCheck(stamp);
            if (found)
            {
                if (j == rescap)
                {
                    res.Expand(rescap = 2 * rescap, j);
                }

                res.array[j++] = a;
            }
        }
        res.size = j;
        res.ReIndex(0);
        return res;
    }


    /// <summary>
    /// Create a new list consisting of the results of mapping all items of this
    /// list. The new list will use the default item equalityComparer for the item type V.
    /// <para>The new list will be of type HashedArrayList</para>
    /// </summary>
    /// <exception cref="DuplicateNotAllowedException">If <code>mapper</code>
    /// creates duplicates</exception>
    /// <typeparam name="V">The type of items of the new list</typeparam>
    /// <param name="mapper">The delegate defining the map.</param>
    /// <returns>The new list.</returns>
    public virtual IList<V> Map<V>(Func<T, V> mapper)
    {
        ValidityCheck();

        HashedArrayList<V> res = new(size);

        return Map<V>(mapper, res);
    }

    /// <summary>
    /// Create a new list consisting of the results of mapping all items of this
    /// list. The new list will use a specified item equalityComparer for the item type.
    /// <para>The new list will be of type HashedArrayList</para>
    /// </summary>
    /// <exception cref="DuplicateNotAllowedException">If <code>mapper</code>
    /// creates duplicates</exception>
    /// <typeparam name="V">The type of items of the new list</typeparam>
    /// <param name="mapper">The delegate defining the map.</param>
    /// <param name="itemequalityComparer">The item equalityComparer to use for the new list</param>
    /// <returns>The new list.</returns>
    public virtual IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> itemequalityComparer)
    {
        ValidityCheck();

        HashedArrayList<V> res = new(size, itemequalityComparer);

        return Map<V>(mapper, res);
    }

    private IList<V> Map<V>(Func<T, V> mapper, HashedArrayList<V> res)
    {
        int stamp = this.stamp;
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                V mappeditem = mapper(array[offsetField + i]);
                ModifyCheck(stamp);
                System.Collections.Generic.KeyValuePair<V, int> p = new(mappeditem, i);
                if (res.itemIndex.FindOrAdd(ref p))
                {
                    throw new ArgumentException("Mapped item already in indexed list");
                }

                res.array[i] = mappeditem;
            }
        }

        res.size = size;
        return res;
    }

    /// <summary>
    /// Remove one item from the list: from the front if <code>FIFO</code>
    /// is true, else from the back.
    /// </summary>
    /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
    /// <returns>The removed item.</returns>
    public virtual T Remove()
    {
        UpdateCheck();
        if (size == 0)
        {
            throw new NoSuchItemException("List is empty");
        }

        T item = RemoveAtInternal(fIFO ? 0 : size - 1);
        (underlying ?? this).RaiseForRemove(item);
        return item;
    }

    /// <summary>
    /// Remove one item from the front of the list.
    /// </summary>
    /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
    /// <returns>The removed item.</returns>
    public virtual T RemoveFirst()
    {
        UpdateCheck();
        if (size == 0)
        {
            throw new NoSuchItemException("List is empty");
        }

        T item = RemoveAtInternal(0);
        (underlying ?? this).RaiseForRemoveAt(offsetField, item);
        return item;
    }


    /// <summary>
    /// Remove one item from the back of the list.
    /// </summary>
    /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
    /// <returns>The removed item.</returns>
    public virtual T RemoveLast()
    {
        UpdateCheck();
        if (size == 0)
        {
            throw new NoSuchItemException("List is empty");
        }

        T item = RemoveAtInternal(size - 1);
        (underlying ?? this).RaiseForRemoveAt(size + offsetField, item);
        return item;
    }

    /// <summary>
    /// Create a list view on this list.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> if the start or count is negative
    /// or the range does not fit within list.</exception>
    /// <param name="start">The index in this list of the start of the view.</param>
    /// <param name="count">The size of the view.</param>
    /// <returns>The new list view.</returns>
    public virtual IList<T>? View(int start, int count)
    {
        ValidityCheck();
        CheckRange(start, count);
        views ??= new WeakViewList<HashedArrayList<T>>();

        HashedArrayList<T> retval = (HashedArrayList<T>)MemberwiseClone();


        retval.underlying = underlying ?? (this);
        retval.offsetField = start + offsetField;
        retval.size = count;
        retval.myWeakReference = views.Add(retval);
        return retval;
    }

    /// <summary>
    /// Create a list view on this list containing the (first) occurrence of a particular item.
    /// <para>Returns <code>null</code> if the item is not in this list.</para>
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The new list view.</returns>
    public virtual IList<T>? ViewOf(T item)
    {
        int index = IndexOfInner(item);
        if (index < 0)
        {
            return null;
        }

        return View(index, 1);
    }


    /// <summary>
    /// Create a list view on this list containing the last occurrence of a particular item.
    /// <para>Returns <code>null</code> if the item is not in this list.</para>
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The new list view.</returns>
    public virtual IList<T>? LastViewOf(T item)
    {
        int index = LastIndexOfInner(item);
        if (index < 0)
        {
            return null;
        }

        return View(index, 1);
    }

    /// <summary>
    /// Null if this list is not a view.
    /// </summary>
    /// <value>Underlying list for view.</value>
    public virtual IList<T>? Underlying => underlying;


    /// <summary>
    /// </summary>
    /// <value>Offset for this list view or 0 for an underlying list.</value>
    public virtual int Offset => offsetField;

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public virtual bool IsValid => isValid;

    /// <summary>
    /// Slide this list view along the underlying list.
    /// </summary>
    /// <exception cref="NotAViewException"> if this list is not a view.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> if the operation
    /// would bring either end of the view outside the underlying list.</exception>
    /// <param name="offset">The signed amount to slide: positive to slide
    /// towards the end.</param>
    public virtual IList<T> Slide(int offset)
    {
        if (!TrySlide(offset, size))
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        return this;
    }


    /// <summary>
    /// Slide this list view along the underlying list, changing its size.
    /// </summary>
    /// <exception cref="NotAViewException"> if this list is not a view.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> if the operation
    /// would bring either end of the view outside the underlying list.</exception>
    /// <param name="offset">The signed amount to slide: positive to slide
    /// towards the end.</param>
    /// <param name="size">The new size of the view.</param>
    public virtual IList<T> Slide(int offset, int size)
    {
        if (!TrySlide(offset, size))
        {
            throw new ArgumentOutOfRangeException();
        }

        return this;
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="NotAViewException"> if this list is not a view.</exception>
    /// <param name="offset"></param>
    /// <returns></returns>
    public virtual bool TrySlide(int offset)
    {
        return TrySlide(offset, size);
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="NotAViewException"> if this list is not a view.</exception>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public virtual bool TrySlide(int offset, int size)
    {
        UpdateCheck();
        if (underlying == null)
        {
            throw new NotAViewException("Not a view");
        }

        int newoffset = offsetField + offset;
        int newsize = size;

        if (newoffset < 0 || newsize < 0 || newoffset + newsize > UnderlyingSize)
        {
            return false;
        }

        offsetField = newoffset;
        this.size = newsize;
        return true;
    }

    /// <summary>
    ///
    /// <para>Returns null if <code>otherView</code> is strictly to the left of this view</para>
    /// </summary>
    /// <param name="otherView"></param>
    /// <exception cref="IncompatibleViewException">If otherView does not have the same underlying list as this</exception>
    /// <returns></returns>
    public virtual IList<T>? Span(IList<T> otherView)
    {
        if ((otherView == null) || ((otherView.Underlying ?? otherView) != (underlying ?? this)))
        {
            throw new IncompatibleViewException();
        }

        if (otherView.Offset + otherView.Count - Offset < 0)
        {
            return null;
        }

        return (underlying ?? this).View(Offset, otherView.Offset + otherView.Count - Offset);
    }

    /// <summary>
    /// Reverse the list so the items are in the opposite sequence order.
    /// </summary>
    public virtual void Reverse()
    {
        UpdateCheck();
        if (size == 0)
        {
            return;
        }

        for (int i = 0, length = size / 2, end = offsetField + size - 1; i < length; i++)
        {
            T swap = array[offsetField + i];

            array[offsetField + i] = array[end - i];
            array[end - i] = swap;
        }
        ReIndex(offsetField, offsetField + size);
        //TODO: be more forgiving wrt. disposing
        DisposeOverlappingViews(true);
        (underlying ?? this).RaiseCollectionChanged();
    }

    /// <summary>
    /// Check if this list is sorted according to the default sorting order
    /// for the item type T, as defined by the <see cref="T:C5.Comparer`1"/> class
    /// </summary>
    /// <exception cref="NotComparableException">if T is not comparable</exception>
    /// <returns>True if the list is sorted, else false.</returns>
    public bool IsSorted() { return IsSorted(SCG.Comparer<T>.Default); }

    /// <summary>
    /// Check if this list is sorted according to a specific sorting order.
    /// </summary>
    /// <param name="c">The comparer defining the sorting order.</param>
    /// <returns>True if the list is sorted, else false.</returns>
    public virtual bool IsSorted(SCG.IComparer<T> c)
    {
        ValidityCheck();
        for (int i = offsetField + 1, end = offsetField + size; i < end; i++)
        {
            if (c.Compare(array[i - 1], array[i]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Sort the items of the list according to the default sorting order
    /// for the item type T, as defined by the Comparer[T] class
    /// (<see cref="T:C5.Comparer`1"/>).
    /// </summary>
    /// <exception cref="InvalidOperationException">if T is not comparable</exception>
    public virtual void Sort()
    {
        Sort(SCG.Comparer<T>.Default);
    }


    /// <summary>
    /// Sort the items of the list according to a specific sorting order.
    /// </summary>
    /// <param name="comparer">The comparer defining the sorting order.</param>
    public virtual void Sort(SCG.IComparer<T> comparer)
    {
        UpdateCheck();
        if (size == 0)
        {
            return;
        }

        Sorting.IntroSort<T>(array, offsetField, size, comparer);
        DisposeOverlappingViews(false);
        ReIndex(offsetField, offsetField + size);
        (underlying ?? this).RaiseCollectionChanged();
    }


    /// <summary>
    /// Randomly shuffle the items of this list.
    /// </summary>
    public virtual void Shuffle() { Shuffle(new C5Random()); }


    /// <summary>
    /// Shuffle the items of this list according to a specific random source.
    /// </summary>
    /// <param name="rnd">The random source.</param>
    public virtual void Shuffle(Random rnd)
    {
        UpdateCheck();
        if (size == 0)
        {
            return;
        }

        for (int i = offsetField, top = offsetField + size, end = top - 1; i < end; i++)
        {
            int j = rnd.Next(i, top);
            if (j != i)
            {
                T tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }
        DisposeOverlappingViews(false);
        ReIndex(offsetField, offsetField + size);
        (underlying ?? this).RaiseCollectionChanged();
    }
    #endregion

    #region IIndexed<T> Members

    /// <summary>
    /// Search for an item in the list going forwards from the start.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>Index of item from start.</returns>
    public virtual int IndexOf(T item) { ValidityCheck(); return IndexOfInner(item); }


    /// <summary>
    /// Search for an item in the list going backwards from the end.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>Index of item from the end.</returns>
    public virtual int LastIndexOf(T item) { ValidityCheck(); return LastIndexOfInner(item); }


    /// <summary>
    /// Remove the item at a specific position of the list.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"> if index is negative or
    /// &gt;= the size of the collection.</exception>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>The removed item.</returns>
    public virtual T RemoveAt(int index)
    {
        UpdateCheck();
        if (index < 0 || index >= size)
        {
            throw new IndexOutOfRangeException("Index out of range for sequenced collection");
        }

        T item = RemoveAtInternal(index);
        (underlying ?? this).RaiseForRemoveAt(offsetField + index, item);
        return item;
    }


    /// <summary>
    /// Remove all items in an index interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If <code>start</code>
    /// and <code>count</code> does not describe a valid interval in the list</exception>
    /// <param name="start">The index of the first item to remove.</param>
    /// <param name="count">The number of items to remove.</param>
    public virtual void RemoveInterval(int start, int count)
    {
        UpdateCheck();
        if (count == 0)
        {
            return;
        }

        CheckRange(start, count);
        start += offsetField;
        FixViewsBeforeRemove(start, count);
        var p = new SCG.KeyValuePair<T, int>();
        for (int i = start, end = start + count; i < end; i++)
        {
            p = new SCG.KeyValuePair<T, int>(array[i], p.Value);
            itemIndex.Remove(p);
        }
        Array.Copy(array, start + count, array, start, UnderlyingSize - start - count);
        AddToSize(-count);
        Array.Clear(array, UnderlyingSize, count);
        ReIndex(start);
        (underlying ?? this).RaiseForRemoveInterval(start, count);
    }

    private void RaiseForRemoveInterval(int start, int count)
    {
        if (ActiveEvents != 0)
        {
            RaiseCollectionCleared(size == 0, count, start);
            RaiseCollectionChanged();
        }
    }
    #endregion

    #region ICollection<T> Members

    /// <summary>
    /// The value is symbolic indicating the type of asymptotic complexity
    /// in terms of the size of this collection (worst-case or amortized as
    /// relevant).
    /// </summary>
    /// <value>Speed.Linear</value>
    public virtual Speed ContainsSpeed => Speed.Constant;

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override int GetUnsequencedHashCode()
    { ValidityCheck(); return base.GetUnsequencedHashCode(); }

    /// <summary>
    ///
    /// </summary>
    /// <param name="that"></param>
    /// <returns></returns>
    public override bool UnsequencedEquals(ICollection<T> that)
    { ValidityCheck(); return base.UnsequencedEquals(that); }

    /// <summary>
    /// Check if this collection contains (an item equivalent to according to the
    /// itemequalityComparer) a particular value.
    /// </summary>
    /// <param name="item">The value to check for.</param>
    /// <returns>True if the items is in this collection.</returns>
    public virtual bool Contains(T item)
    { ValidityCheck(); return IndexOfInner(item) >= 0; }


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, return in the ref argument (a
    /// binary copy of) the actual value found.
    /// </summary>
    /// <param name="item">The value to look for.</param>
    /// <returns>True if the items is in this collection.</returns>
    public virtual bool Find(ref T item)
    {
        ValidityCheck();

        int i;

        if ((i = IndexOfInner(item)) >= 0)
        {
            item = array[offsetField + i];
            return true;
        }

        return false;
    }


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// to with a binary copy of the supplied value. This will only update the first
    /// mathching item.
    /// </summary>
    /// <param name="item">Value to update.</param>
    /// <returns>True if the item was found and hence updated.</returns>
    public virtual bool Update(T item)
    {
        return Update(item, out _);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    /// <param name="olditem"></param>
    /// <returns></returns>
    public virtual bool Update(T item, out T olditem)
    {
        UpdateCheck();
        int i;

        if ((i = IndexOfInner(item)) >= 0)
        {
            olditem = array[offsetField + i];
            array[offsetField + i] = item;
            itemIndex.Update(new System.Collections.Generic.KeyValuePair<T, int>(item, offsetField + i));
            (underlying ?? this).RaiseForUpdate(item, olditem);
            return true;
        }

        olditem = default;
        return false;
    }

    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, return in the ref argument (a
    /// binary copy of) the actual value found. Else, add the item to the collection.
    /// </summary>
    /// <param name="item">The value to look for.</param>
    /// <returns>True if the item was found (hence not added).</returns>
    public virtual bool FindOrAdd(ref T item)
    {
        UpdateCheck();
        if (Find(ref item))
        {
            return true;
        }

        Add(item);
        return false;
    }


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// to with a binary copy of the supplied value. This will only update the first
    /// matching item.
    /// </summary>
    /// <param name="item">Value to update.</param>
    /// <returns>True if the item was found and hence updated.</returns>
    public virtual bool UpdateOrAdd(T item)
    {
        UpdateCheck();
        if (Update(item))
        {
            return true;
        }

        Add(item);
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    /// <param name="olditem"></param>
    /// <returns></returns>
    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
        UpdateCheck();
        if (Update(item, out olditem))
        {
            return true;
        }

        Add(item);
        olditem = default;
        return false;
    }

    /// <summary>
    /// Remove a particular item from this list. The item will be searched
    /// for from the end of the list if <code>FIFO == false</code> (the default),
    /// else from the start.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <returns>True if the item was found (and removed).</returns>
    public virtual bool Remove(T item)
    {
        UpdateCheck();

        int i = fIFO ? IndexOfInner(item) : LastIndexOfInner(item);

        if (i < 0)
        {
            return false;
        }

        T removeditem = RemoveAtInternal(i);
        (underlying ?? this).RaiseForRemove(removeditem);
        return true;
    }


    /// <summary>
    /// Remove the first copy of a particular item from this collection if found.
    /// If an item was removed, report a binary copy of the actual item removed in
    /// the argument. The item will be searched
    /// for from the end of the list if <code>FIFO == false</code> (the default),
    /// else from the start.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <param name="removeditem">The removed value.</param>
    /// <returns>True if the item was found (and removed).</returns>
    public virtual bool Remove(T item, out T removeditem)
    {
        UpdateCheck();

        int i = fIFO ? IndexOfInner(item) : LastIndexOfInner(item);

        if (i < 0)
        {
            removeditem = default;
            return false;
        }

        removeditem = RemoveAtInternal(i);
        (underlying ?? this).RaiseForRemove(removeditem);
        return true;
    }


    //TODO: remove from end or according to FIFO?
    /// <summary>
    /// Remove all items in another collection from this one, taking multiplicities into account.
    /// Matching items will be removed from the front. Current implementation is not optimal.
    /// </summary>
    /// <param name="items">The items to remove.</param>
    public virtual void RemoveAll(SCG.IEnumerable<T> items)
    {
        UpdateCheck();
        if (size == 0)
        {
            return;
        }
        //TODO: reactivate the old code for small sizes
        HashBag<T> toremove = new(itemEqualityComparer);
        toremove.AddAll(items);
        if (toremove.Count == 0)
        {
            return;
        }

        RaiseForRemoveAllHandler raiseHandler = new(underlying ?? this);
        bool mustFire = raiseHandler.MustFire;
        ViewHandler viewHandler = new(this);
        int j = offsetField;
        int removed = 0;
        int i = offsetField, end = offsetField + size;
        var p = new SCG.KeyValuePair<T, int>();
        while (i < end)
        {
            T item;
            //pass by a stretch of nodes
            while (i < end && !toremove.Contains(item = array[i]))
            {
                if (j < i)
                {
                    p = new SCG.KeyValuePair<T, int>(item, j);
                    itemIndex.Update(p);
                }
                //if (j<i)
                array[j] = item;
                i++; j++;
            }
            viewHandler.SkipEndpoints(removed, i);
            //Remove a stretch of nodes
            while (i < end && toremove.Remove(item = array[i]))
            {
                p = new SCG.KeyValuePair<T, int>(item, p.Value);
                itemIndex.Remove(p);
                if (mustFire)
                {
                    raiseHandler.Remove(item);
                }

                removed++;
                i++;
                viewHandler.UpdateViewSizesAndCounts(removed, i);
            }
        }
        if (removed == 0)
        {
            return;
        }

        viewHandler.UpdateViewSizesAndCounts(removed, UnderlyingSize);
        Array.Copy(array, offsetField + size, array, j, UnderlyingSize - offsetField - size);
        AddToSize(-removed);
        Array.Clear(array, UnderlyingSize, removed);
        ReIndex(j);
        if (mustFire)
        {
            raiseHandler.Raise();
        }
    }

    /*
    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    void RemoveAll(Func<T, bool> predicate)
    {
        updatecheck();
        if (size == 0)
            return;
        RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
        bool mustFire = raiseHandler.MustFire;
        ViewHandler viewHandler = new ViewHandler(this);
        int j = offsetField;
        int removed = 0;
        int i = offsetField, end = offsetField + size;
        System.Collections.Generic.KeyValuePair<T, int> p = new System.Collections.Generic.KeyValuePair<T, int>();
        while (i < end)
        {
            T item;
            //pass by a stretch of nodes
            while (i < end && !predicate(item = array[i]))
            {
                updatecheck();
                if (j < i)
                {
                    p.Key = item;
                    p.Value = j;
                    itemIndex.Update(p);
                }
                //if (j<i)
                array[j] = item;
                i++; j++;
            }
            updatecheck();
            viewHandler.skipEndpoints(removed, i);
            //Remove a stretch of nodes
            while (i < end && predicate(item = array[i]))
            {
                updatecheck();
                p.Key = item;
                itemIndex.Remove(p);
                if (mustFire)
                    raiseHandler.Remove(item);
                removed++;
                i++;
                viewHandler.updateViewSizesAndCounts(removed, i);
            }
            updatecheck();
        }
        if (removed == 0)
            return;
        viewHandler.updateViewSizesAndCounts(removed, underlyingsize);
        Array.Copy(array, offsetField + size, array, j, underlyingsize - offsetField - size);
        addtosize(-removed);
        Array.Clear(array, underlyingsize, removed);
        reindex(j);
        if (mustFire)
            raiseHandler.Raise();
    } */

    /// <summary>
    /// Remove all items from this collection, resetting internal array size.
    /// </summary>
    public override void Clear()
    {
        if (underlying == null)
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            int oldsize = size;
            FixViewsBeforeRemove(0, size);
            itemIndex.Clear();
            array = new T[8];
            size = 0;
            (underlying ?? this).RaiseForRemoveInterval(offsetField, oldsize);
        }
        else
        {
            RemoveInterval(0, size);
        }
    }

    /// <summary>
    /// Remove all items not in some other collection from this one, taking multiplicities into account.
    /// Items are retained front first.
    /// </summary>
    /// <param name="items">The items to retain.</param>
    public virtual void RetainAll(SCG.IEnumerable<T> items)
    {
        UpdateCheck();
        if (size == 0)
        {
            return;
        }

        HashBag<T> toretain = new(itemEqualityComparer);
        toretain.AddAll(items);
        if (toretain.Count == 0)
        {
            Clear();
            return;
        }
        RaiseForRemoveAllHandler raiseHandler = new(underlying ?? this);
        bool mustFire = raiseHandler.MustFire;
        ViewHandler viewHandler = new(this);
        int j = offsetField;
        int removed = 0;
        int i = offsetField, end = offsetField + size;
        System.Collections.Generic.KeyValuePair<T, int> p = new();
        while (i < end)
        {
            T item;
            //pass by a stretch of nodes
            while (i < end && toretain.Remove(item = array[i]))
            {
                if (j < i)
                {
                    p = new SCG.KeyValuePair<T, int>(item, j);
                    itemIndex.Update(p);
                }
                //if (j<i)
                array[j] = item;
                i++; j++;
            }
            viewHandler.SkipEndpoints(removed, i);
            //Remove a stretch of nodes
            while (i < end && !toretain.Contains(item = array[i]))
            {
                p = new SCG.KeyValuePair<T, int>(item, p.Value);
                itemIndex.Remove(p);
                if (mustFire)
                {
                    raiseHandler.Remove(item);
                }

                removed++;
                i++;
                viewHandler.UpdateViewSizesAndCounts(removed, i);
            }
        }
        if (removed == 0)
        {
            return;
        }

        viewHandler.UpdateViewSizesAndCounts(removed, UnderlyingSize);
        Array.Copy(array, offsetField + size, array, j, UnderlyingSize - offsetField - size);
        AddToSize(-removed);
        Array.Clear(array, UnderlyingSize, removed);
        ReIndex(j);
        raiseHandler.Raise();
    }

    /*
    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    void RetainAll(Func<T, bool> predicate)
    {
        updatecheck();
        if (size == 0)
            return;
        RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
        bool mustFire = raiseHandler.MustFire;
        ViewHandler viewHandler = new ViewHandler(this);
        int j = offsetField;
        int removed = 0;
        int i = offsetField, end = offsetField + size;
        System.Collections.Generic.KeyValuePair<T, int> p = new System.Collections.Generic.KeyValuePair<T, int>();
        while (i < end)
        {
            T item;
            //pass by a stretch of nodes
            while (i < end && predicate(item = array[i]))
            {
                updatecheck();
                if (j < i)
                {
                    p.Key = item;
                    p.Value = j;
                    itemIndex.Update(p);
                }
                //if (j<i)
                array[j] = item;
                i++; j++;
            }
            updatecheck();
            viewHandler.skipEndpoints(removed, i);
            //Remove a stretch of nodes
            while (i < end && !predicate(item = array[i]))
            {
                updatecheck();
                p.Key = item;
                itemIndex.Remove(p);
                if (mustFire)
                    raiseHandler.Remove(item);
                removed++;
                i++;
                viewHandler.updateViewSizesAndCounts(removed, i);
            }
            updatecheck();
        }
        if (removed == 0)
            return;
        viewHandler.updateViewSizesAndCounts(removed, underlyingsize);
        Array.Copy(array, offsetField + size, array, j, underlyingsize - offsetField - size);
        addtosize(-removed);
        Array.Clear(array, underlyingsize, removed);
        reindex(j);
        raiseHandler.Raise();
    } */


    /// <summary>
    /// Check if this collection contains all the values in another collection,
    /// taking multiplicities into account.
    /// Current implementation is not optimal.
    /// </summary>
    /// <param name="items">The </param>
    /// <returns>True if all values in <code>items</code>is in this collection.</returns>
    public virtual bool ContainsAll(SCG.IEnumerable<T> items)
    {
        ValidityCheck();
        foreach (T item in items)
        {
            if (IndexOfInner(item) < 0)
            {
                return false;
            }
        }

        return true;

    }


    /// <summary>
    /// Count the number of items of the collection equal to a particular value.
    /// Returns 0 if and only if the value is not in the collection.
    /// </summary>
    /// <param name="item">The value to count.</param>
    /// <returns>The number of copies found.</returns>
    public virtual int ContainsCount(T item)
    {
        ValidityCheck();
        return IndexOfInner(item) >= 0 ? 1 : 0;

    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual ICollectionValue<T> UniqueItems()
    {
        return this;

    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities()
    {
        return new MultiplicityOne<T>(this);

    }





    /// <summary>
    /// Remove all items equal to a given one.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    public virtual void RemoveAllCopies(T item)
    {
        Remove(item);

    }


    //TODO: check views
    /// <summary>
    /// Check the integrity of the internal data structures of this array list.
    /// </summary>
    /// <returns>True if check does not fail.</returns>
    public override bool Check()
    {
        bool retval = true;

        if (UnderlyingSize > array.Length)
        {
            Logger.Log(string.Format("underlyingsize ({0}) > array.Length ({1})", size, array.Length));
            return false;
        }

        if (offsetField + size > UnderlyingSize)
        {
            Logger.Log(string.Format("offset({0})+size({1}) > underlyingsize ({2})", offsetField, size, UnderlyingSize));
            return false;
        }

        if (offsetField < 0)
        {
            Logger.Log(string.Format("offset({0}) < 0", offsetField));
            return false;
        }

        for (int i = 0; i < UnderlyingSize; i++)
        {
            if (array[i] == null)
            {
                Logger.Log(string.Format("Bad element: null at (base)index {0}", i));
                retval = false;
            }
        }

        for (int i = UnderlyingSize, length = array.Length; i < length; i++)
        {
            if (!Equals(array[i], default))
            {
                Logger.Log(string.Format("Bad element: != default(T) at (base)index {0}", i));
                retval = false;
            }
        }

        {
            HashedArrayList<T> u = underlying ?? this;
            if (u.views != null)
            {
                foreach (HashedArrayList<T> v in u.views)
                {
                    if (u.array != v.array)
                    {
                        Logger.Log(string.Format("View from {0} of length {1} has different base array than the underlying list", v.offsetField, v.size));
                        retval = false;
                    }
                }
            }
        }


        if (UnderlyingSize != itemIndex.Count)
        {
            Logger.Log(string.Format("size ({0})!= index.Count ({1})", size, itemIndex.Count));
            retval = false;
        }

        for (int i = 0; i < UnderlyingSize; i++)
        {
            var p = new SCG.KeyValuePair<T, int>(array[i], default);

            if (!itemIndex.Find(ref p))
            {
                Logger.Log(string.Format("Item {1} at {0} not in hashindex", i, array[i]));
                retval = false;
            }

            if (p.Value != i)
            {
                Logger.Log(string.Format("Item {1} at {0} has hashindex {2}", i, array[i], p.Value));
                retval = false;
            }
        }
        return retval;
    }

    #endregion

    #region IExtensible<T> Members

    /// <summary>
    ///
    /// </summary>
    /// <value>True, indicating array list has bag semantics.</value>
    public virtual bool AllowsDuplicates => false;

    /// <summary>
    /// By convention this is true for any collection with set semantics.
    /// </summary>
    /// <value>True if only one representative of a group of equal items
    /// is kept in the collection together with the total count.</value>
    public virtual bool DuplicatesByCounting => true;

    /// <summary>
    /// Add an item to end of this list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True</returns>
    public virtual bool Add(T item)
    {
        UpdateCheck();
        System.Collections.Generic.KeyValuePair<T, int> p = new(item, size + offsetField);
        if (itemIndex.FindOrAdd(ref p))
        {
            return false;
        }

        InsertBase(size, item);
        ReIndex(size + offsetField);
        (underlying ?? this).RaiseForAdd(item);
        return true;
    }


    /// <summary>
    /// Add the elements from another collection to this collection.
    /// </summary>
    /// <param name="items"></param>
    public virtual void AddAll(SCG.IEnumerable<T> items)
    {
        UpdateCheck();
        int toadd = CountItems(items);
        if (toadd == 0)
        {
            return;
        }

        if (toadd + UnderlyingSize > array.Length)
        {
            Expand(toadd + UnderlyingSize, UnderlyingSize);
        }

        int i = size + offsetField;
        if (UnderlyingSize > i)
        {
            Array.Copy(array, i, array, i + toadd, UnderlyingSize - i);
        }

        try
        {
            foreach (T item in items)
            {
                System.Collections.Generic.KeyValuePair<T, int> p = new(item, i);
                if (itemIndex.FindOrAdd(ref p))
                {
                    continue;
                }

                array[i++] = item;
            }
        }
        finally
        {
            int added = i - size - offsetField;
            if (added < toadd)
            {
                Array.Copy(array, size + offsetField + toadd, array, i, UnderlyingSize - size - offsetField);
                Array.Clear(array, UnderlyingSize + added, toadd - added);
            }
            if (added > 0)
            {
                AddToSize(added);
                ReIndex(i);
                FixViewsAfterInsert(added, i - added);
                (underlying ?? this).RaiseForAddAll(i - added, added);
            }
        }
    }
    private void RaiseForAddAll(int start, int added)
    {
        if ((ActiveEvents & EventType.Added) != 0)
        {
            for (int i = start, end = start + added; i < end; i++)
            {
                RaiseItemsAdded(array[i], 1);
            }
        }

        RaiseCollectionChanged();
    }

    #endregion

    #region IDirectedEnumerable<T> Members

    /// <summary>
    /// Create a collection containing the same items as this collection, but
    /// whose enumerator will enumerate the items backwards. The new collection
    /// will become invalid if the original is modified. Method typically used as in
    /// <code>foreach (T x in coll.Backwards()) {...}</code>
    /// </summary>
    /// <returns>The backwards collection.</returns>
    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

    #endregion

    #region ICollectionValue<T> Members
    /// <summary>
    ///
    /// </summary>
    /// <value>The number of items in this collection</value>
    public override int Count { get { ValidityCheck(); return size; } }
    #endregion

    #region IEnumerable<T> Members
    //TODO: make tests of all calls on a disposed view throws the right exception! (Which should be C5.InvalidListViewException)
    /// <summary>
    /// Create an enumerator for the collection
    /// </summary>
    /// <returns>The enumerator</returns>
    public override SCG.IEnumerator<T> GetEnumerator()
    {
        ValidityCheck();
        return base.GetEnumerator();
    }
    #endregion

    #region IDisposable Members

    /// <summary>
    /// Invalidate this list. If a view, just invalidate the view.
    /// If not a view, invalidate the list and all views on it.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(false);
    }

    private void Dispose(bool disposingUnderlying)
    {
        if (isValid)
        {
            if (underlying != null)
            {
                isValid = false;
                if (!disposingUnderlying && views != null)
                {
                    views.Remove(myWeakReference!);
                }

                underlying = null;
                views = null;
                myWeakReference = null;
            }
            else
            {
                //isValid = false;
                if (views != null)
                {
                    foreach (HashedArrayList<T> view in views)
                    {
                        view.Dispose(true);
                    }
                }

                Clear();
            }
        }
    }

    #endregion

    #region ISerializable Members
    /*
/// <summary>
///
/// </summary>
/// <param name="info"></param>
/// <param name="context"></param>
public HashedArrayList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :
  this(info.GetInt32("sz"),(SCG.IEqualityComparer<T>)(info.GetValue("eq",typeof(SCG.IEqualityComparer<T>))))
{
  size = info.GetInt32("sz");
  for (int i = 0; i < size; i++)
  {
    array[i] = (T)(info.GetValue("elem" + i,typeof(T)));
  }
  reindex(0);
}

/// <summary>
///
/// </summary>
/// <param name="info"></param>
/// <param name="context"></param>
public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
{
  info.AddValue("sz", size);
  info.AddValue("eq", EqualityComparer);
  for (int i = 0; i < size; i++)
			{
    info.AddValue("elem" + i, array[i + offset]);
  }
}
*/
    #endregion

    #region System.Collections.Generic.IList<T> Members

    void System.Collections.Generic.IList<T>.RemoveAt(int index)
    {
        RemoveAt(index);
    }

    void System.Collections.Generic.ICollection<T>.Add(T item)
    {
        Add(item);
    }

    #endregion

    #region System.Collections.ICollection Members

    bool System.Collections.ICollection.IsSynchronized => false;

    [Obsolete]
    object System.Collections.ICollection.SyncRoot => underlying != null ? ((System.Collections.ICollection)underlying).SyncRoot : array;

    void System.Collections.ICollection.CopyTo(Array arr, int index)
    {
        if (index < 0 || index + Count > arr.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        foreach (T item in this)
        {
            arr.SetValue(item, index++);
        }
    }

    #endregion

    #region System.Collections.IList Members

    object? System.Collections.IList.this[int index]
    {
        get => this[index]!;
        set
        {
            if (value is null && default(T) is not null) throw new ArgumentNullException(nameof(value));
            if (value is not T) throw new ArgumentException("Wrong type", nameof(value));

            this[index] = (T)value;
        }
    }

    int System.Collections.IList.Add(object? value)
    {
        if (value is null && default(T) is not null) throw new ArgumentNullException(nameof(value));
        if (value is not T) throw new ArgumentException("Wrong type", nameof(value));

        bool added = Add((T)value);
        // What position to report if item not added? SC.IList.Add doesn't say
        return added ? Count - 1 : -1;
    }

    bool System.Collections.IList.Contains(object? value)
    {
        if (value is null && default(T) is not null) return false;
        if (value is not T) return false;

        return Contains((T)value);
    }

    int System.Collections.IList.IndexOf(object? value)
    {
        if (value is null && default(T) is not null) return -1;
        if (value is not T) return -1;

        return Math.Max(-1, IndexOf((T)value));
    }

    void System.Collections.IList.Insert(int index, object? value)
    {
        if (value is null && default(T) is not null) throw new ArgumentNullException(nameof(value));
        if (value is not T) throw new ArgumentException("Wrong type", nameof(value));

        Insert(index, (T)value);
    }

    void System.Collections.IList.Remove(object? value)
    {
        if (value is null && default(T) is not null) throw new ArgumentNullException(nameof(value));
        if (value is not T) throw new ArgumentException("Wrong type", nameof(value));

        Remove((T)value);
    }

    void System.Collections.IList.RemoveAt(int index)
    {
        RemoveAt(index);
    }

    #endregion
}
