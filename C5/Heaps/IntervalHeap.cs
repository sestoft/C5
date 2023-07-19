// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A priority queue class based on an interval heap data structure.
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    [Serializable]
    public class IntervalHeap<T> : CollectionValueBase<T>, IPriorityQueue<T>
    {
        #region Events

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override EventType ListenableEvents => EventType.Basic;

        #endregion

        #region Fields
        private struct Interval
        {
            internal T first, last; internal Handle? firsthandle, lasthandle;


            public override string ToString() { return string.Format("[{0}; {1}]", first, last); }
        }

        private int stamp;
        private readonly SCG.IComparer<T> comparer;
        private readonly SCG.IEqualityComparer<T> itemequalityComparer;
        private Interval[] heap;
        private int size;
        #endregion

        #region Util
        // heapifyMin and heapifyMax and their auxiliaries

        private void SwapFirstWithLast(int cell1, int cell2)
        {
            T first = heap[cell1].first;
            Handle firsthandle = heap[cell1].firsthandle!;
            UpdateFirst(cell1, heap[cell2].last, heap[cell2].lasthandle);
            UpdateLast(cell2, first, firsthandle);
        }

        private void SwapLastWithLast(int cell1, int cell2)
        {
            T last = heap[cell2].last;
            Handle lasthandle = heap[cell2].lasthandle!;
            UpdateLast(cell2, heap[cell1].last, heap[cell1].lasthandle!);
            UpdateLast(cell1, last, lasthandle);
        }

        private void SwapFirstWithFirst(int cell1, int cell2)
        {
            T first = heap[cell2].first;
            Handle firsthandle = heap[cell2].firsthandle!;
            UpdateFirst(cell2, heap[cell1].first, heap[cell1].firsthandle);
            UpdateFirst(cell1, first, firsthandle);
        }

        private bool HeapifyMin(int cell)
        {
            bool swappedroot = false;
            // If first > last, swap them
            if (2 * cell + 1 < size && comparer.Compare(heap[cell].first, heap[cell].last) > 0)
            {
                swappedroot = true;
                SwapFirstWithLast(cell, cell);
            }

            int currentmin = cell, l = 2 * cell + 1, r = l + 1;
            if (2 * l < size && comparer.Compare(heap[l].first, heap[currentmin].first) < 0)
            {
                currentmin = l;
            }

            if (2 * r < size && comparer.Compare(heap[r].first, heap[currentmin].first) < 0)
            {
                currentmin = r;
            }

            if (currentmin != cell)
            {
                // cell has at least one daughter, and it contains the min
                SwapFirstWithFirst(currentmin, cell);
                HeapifyMin(currentmin);
            }
            return swappedroot;
        }

        private bool HeapifyMax(int cell)
        {
            bool swappedroot = false;
            if (2 * cell + 1 < size && comparer.Compare(heap[cell].last, heap[cell].first) < 0)
            {
                swappedroot = true;
                SwapFirstWithLast(cell, cell);
            }

            int currentmax = cell, l = 2 * cell + 1, r = l + 1;
            bool firstmax = false;  // currentmax's first field holds max
            if (2 * l + 1 < size)
            {  // both l.first and l.last exist
                if (comparer.Compare(heap[l].last, heap[currentmax].last) > 0)
                {
                    currentmax = l;
                }
            }
            else if (2 * l + 1 == size)
            {  // only l.first exists
                if (comparer.Compare(heap[l].first, heap[currentmax].last) > 0)
                {
                    currentmax = l;
                    firstmax = true;
                }
            }

            if (2 * r + 1 < size)
            {  // both r.first and r.last exist
                if (comparer.Compare(heap[r].last, heap[currentmax].last) > 0)
                {
                    currentmax = r;
                }
            }
            else if (2 * r + 1 == size)
            {  // only r.first exists
                if (comparer.Compare(heap[r].first, heap[currentmax].last) > 0)
                {
                    currentmax = r;
                    firstmax = true;
                }
            }

            if (currentmax != cell)
            {
                // The cell has at least one daughter, and it contains the max
                if (firstmax)
                {
                    SwapFirstWithLast(currentmax, cell);
                }
                else
                {
                    SwapLastWithLast(currentmax, cell);
                }

                HeapifyMax(currentmax);
            }
            return swappedroot;
        }

        private void BubbleUpMin(int i)
        {
            if (i > 0)
            {
                T min = heap[i].first, iv = min;
                Handle minhandle = heap[i].firsthandle!;
                _ = (i + 1) / 2 - 1;

                while (i > 0)
                {
                    int p;
                    if (comparer.Compare(iv, min = heap[p = (i + 1) / 2 - 1].first) < 0)
                    {
                        UpdateFirst(i, min, heap[p].firsthandle);
                        i = p;
                    }
                    else
                    {
                        break;
                    }
                }

                UpdateFirst(i, iv, minhandle);
            }
        }

        private void BubbleUpMax(int i)
        {
            if (i > 0)
            {
                T max = heap[i].last, iv = max;
                Handle maxhandle = heap[i].lasthandle!;
                _ = (i + 1) / 2 - 1;

                while (i > 0)
                {
                    int p;
                    if (comparer.Compare(iv, max = heap[p = (i + 1) / 2 - 1].last) > 0)
                    {
                        UpdateLast(i, max, heap[p].lasthandle);
                        i = p;
                    }
                    else
                    {
                        break;
                    }
                }

                UpdateLast(i, iv, maxhandle);

            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Create an interval heap with natural item comparer and default initial capacity (16)
        /// </summary>
        public IntervalHeap() : this(16) { }


        /// <summary>
        /// Create an interval heap with external item comparer and default initial capacity (16)
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        public IntervalHeap(SCG.IComparer<T> comparer) : this(16, comparer) { }


        //TODO: maybe remove
        /// <summary>
        /// Create an interval heap with natural item comparer and prescribed initial capacity
        /// </summary>
        /// <param name="capacity">The initial capacity</param>
        public IntervalHeap(int capacity) : this(capacity, SCG.Comparer<T>.Default, EqualityComparer<T>.Default) { }


        /// <summary>
        /// Create an interval heap with external item comparer and prescribed initial capacity
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        /// <param name="capacity">The initial capacity</param>
        public IntervalHeap(int capacity, SCG.IComparer<T> comparer) : this(capacity, comparer, new ComparerZeroHashCodeEqualityComparer<T>(comparer)) { }

        private IntervalHeap(int capacity, SCG.IComparer<T> comparer, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            this.comparer = comparer ?? throw new NullReferenceException("Item comparer cannot be null");
            this.itemequalityComparer = itemequalityComparer ?? throw new NullReferenceException("Item equality comparer cannot be null");
            int length = 1;
            while (length < capacity)
            {
                length <<= 1;
            }

            heap = new Interval[length];
        }

        #endregion

        #region IPriorityQueue<T> Members

        /// <summary>
        /// Find the current least item of this priority queue.
        /// <exception cref="NoSuchItemException"/> if queue is empty
        /// </summary>
        /// <returns>The least item.</returns>
        public T FindMin()
        {
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            return heap[0].first;
        }


        /// <summary>
        /// Remove the least item from this  priority queue.
        /// <exception cref="NoSuchItemException"/> if queue is empty
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMin()
        {
            return DeleteMin(out _);
        }


        /// <summary>
        /// Find the current largest item of this priority queue.
        /// <exception cref="NoSuchItemException"/> if queue is empty
        /// </summary>
        /// <returns>The largest item.</returns>
        public T FindMax()
        {
            if (size == 0)
            {
                throw new NoSuchItemException("Heap is empty");
            }
            else if (size == 1)
            {
                return heap[0].first;
            }
            else
            {
                return heap[0].last;
            }
        }


        /// <summary>
        /// Remove the largest item from this  priority queue.
        /// <exception cref="NoSuchItemException"/> if queue is empty
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMax()
        {
            return DeleteMax(out _);
        }


        /// <summary>
        /// The comparer object supplied at creation time for this collection
        /// </summary>
        /// <value>The comparer</value>
        public SCG.IComparer<T> Comparer => comparer;

        #endregion

        #region IExtensible<T> Members

        /// <summary>
        /// If true any call of an updating operation will throw an
        /// <code>ReadOnlyCollectionException</code>
        /// </summary>
        /// <value>True if this collection is read-only.</value>
        public bool IsReadOnly => false;

        /// <summary>
        ///
        /// </summary>
        /// <value>True since this collection has bag semantics</value>
        public bool AllowsDuplicates => true;

        /// <summary>
        /// Value is null since this collection has no equality concept for its items.
        /// </summary>
        /// <value></value>
        public virtual SCG.IEqualityComparer<T> EqualityComparer => itemequalityComparer;

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting => false;



        /// <summary>
        /// Add an item to this priority queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True</returns>
        public bool Add(T item)
        {
            stamp++;
            if (Add(null, item))
            {
                RaiseItemsAdded(item, 1);
                RaiseCollectionChanged();
                return true;
            }
            return false;
        }

        private bool Add(Handle? itemhandle, T item)
        {
            if (size == 0)
            {
                size = 1;
                UpdateFirst(0, item, itemhandle);
                return true;
            }

            if (size == 2 * heap.Length)
            {
                Interval[] newheap = new Interval[2 * heap.Length];

                Array.Copy(heap, newheap, heap.Length);
                heap = newheap;
            }

            if (size % 2 == 0)
            {
                int i = size / 2, p = (i + 1) / 2 - 1;
                T tmp = heap[p].last;

                if (comparer.Compare(item, tmp) > 0)
                {
                    UpdateFirst(i, tmp, heap[p].lasthandle);
                    UpdateLast(p, item, itemhandle);
                    BubbleUpMax(p);
                }
                else
                {
                    UpdateFirst(i, item, itemhandle);

                    if (comparer.Compare(item, heap[p].first) < 0)
                    {
                        BubbleUpMin(i);
                    }
                }
            }
            else
            {
                int i = size / 2;
                T other = heap[i].first;

                if (comparer.Compare(item, other) < 0)
                {
                    UpdateLast(i, other, heap[i].firsthandle);
                    UpdateFirst(i, item, itemhandle);
                    BubbleUpMin(i);
                }
                else
                {
                    UpdateLast(i, item, itemhandle);
                    BubbleUpMax(i);
                }
            }
            size++;

            return true;
        }

        private void UpdateLast(int cell, T item, Handle? handle)
        {
            heap[cell].last = item;
            if (handle != null)
            {
                handle.index = 2 * cell + 1;
            }

            heap[cell].lasthandle = handle;
        }

        private void UpdateFirst(int cell, T item, Handle? handle)
        {
            heap[cell].first = item;
            if (handle != null)
            {
                handle.index = 2 * cell;
            }

            heap[cell].firsthandle = handle;
        }


        /// <summary>
        /// Add the elements from another collection with a more specialized item type
        /// to this collection.
        /// </summary>
        /// <param name="items">The items to add</param>
        public void AddAll(SCG.IEnumerable<T> items)
        {
            stamp++;
            int oldsize = size;
            foreach (T item in items)
            {
                Add(null, item);
            }

            if (size != oldsize)
            {
                if ((ActiveEvents & EventType.Added) != 0)
                {
                    foreach (T item in items)
                    {
                        RaiseItemsAdded(item, 1);
                    }
                }

                RaiseCollectionChanged();
            }
        }

        #endregion

        #region ICollection<T> members

        /// <summary>
        ///
        /// </summary>
        /// <value>True if this collection is empty.</value>
        public override bool IsEmpty => size == 0;

        /// <summary>
        ///
        /// </summary>
        /// <value>The size of this collection</value>
        public override int Count => size;


        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the
        /// <code>Count</code> property in this collection.</value>
        public override Speed CountSpeed => Speed.Constant;

        /// <summary>
        /// Choose some item of this collection.
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override T Choose()
        {
            if (size == 0)
            {
                throw new NoSuchItemException("Collection is empty");
            }

            return heap[0].first;
        }


        /// <summary>
        /// Create an enumerator for the collection
        /// <para>Note: the enumerator does *not* enumerate the items in sorted order,
        /// but in the internal table order.</para>
        /// </summary>
        /// <returns>The enumerator(SIC)</returns>
        public override SCG.IEnumerator<T> GetEnumerator()
        {
            int mystamp = stamp;
            for (int i = 0; i < size; i++)
            {
                if (mystamp != stamp)
                {
                    throw new CollectionModifiedException();
                }

                yield return i % 2 == 0 ? heap[i >> 1].first : heap[i >> 1].last;
            }
            yield break;
        }


        #endregion

        #region Diagnostics

        // Check invariants:
        // * first <= last in a cell if both are valid
        // * a parent interval (cell) contains both its daughter intervals (cells)
        // * a handle, if non-null, points to the cell it is associated with
        private bool Check(int i, T min, T max)
        {
            bool retval = true;
            Interval interval = heap[i];
            T first = interval.first, last = interval.last;

            if (2 * i + 1 == size)
            {
                if (comparer.Compare(min, first) > 0)
                {
                    Logger.Log(string.Format("Cell {0}: parent.first({1}) > first({2})  [size={3}]", i, min, first, size));
                    retval = false;
                }

                if (comparer.Compare(first, max) > 0)
                {
                    Logger.Log(string.Format("Cell {0}: first({1}) > parent.last({2})  [size={3}]", i, first, max, size));
                    retval = false;
                }
                if (interval.firsthandle != null && interval.firsthandle.index != 2 * i)
                {
                    Logger.Log(string.Format("Cell {0}: firsthandle.index({1}) != 2*cell({2})  [size={3}]", i, interval.firsthandle.index, 2 * i, size));
                    retval = false;
                }

                return retval;
            }
            else
            {
                if (comparer.Compare(min, first) > 0)
                {
                    Logger.Log(string.Format("Cell {0}: parent.first({1}) > first({2})  [size={3}]", i, min, first, size));
                    retval = false;
                }

                if (comparer.Compare(first, last) > 0)
                {
                    Logger.Log(string.Format("Cell {0}: first({1}) > last({2})  [size={3}]", i, first, last, size));
                    retval = false;
                }

                if (comparer.Compare(last, max) > 0)
                {
                    Logger.Log(string.Format("Cell {0}: last({1}) > parent.last({2})  [size={3}]", i, last, max, size));
                    retval = false;
                }
                if (interval.firsthandle != null && interval.firsthandle.index != 2 * i)
                {
                    Logger.Log(string.Format("Cell {0}: firsthandle.index({1}) != 2*cell({2})  [size={3}]", i, interval.firsthandle.index, 2 * i, size));
                    retval = false;
                }
                if (interval.lasthandle != null && interval.lasthandle.index != 2 * i + 1)
                {
                    Logger.Log(string.Format("Cell {0}: lasthandle.index({1}) != 2*cell+1({2})  [size={3}]", i, interval.lasthandle.index, 2 * i + 1, size));
                    retval = false;
                }

                int l = 2 * i + 1, r = l + 1;

                if (2 * l < size)
                {
                    retval = retval && Check(l, first, last);
                }

                if (2 * r < size)
                {
                    retval = retval && Check(r, first, last);
                }
            }

            return retval;
        }


        /// <summary>
        /// Check the integrity of the internal data structures of this collection.
        /// Only available in DEBUG builds???
        /// </summary>
        /// <returns>True if check does not fail.</returns>
        public bool Check()
        {
            if (size == 0)
            {
                return true;
            }

            if (size == 1)
            {
                return heap[0].first != null;
            }

            return Check(0, heap[0].first, heap[0].last);
        }

        #endregion

        #region IPriorityQueue<T> Members

        [Serializable]
        private class Handle : IPriorityQueueHandle<T>
        {
            /// <summary>
            /// To save space, the index is 2*cell for heap[cell].first, and 2*cell+1 for heap[cell].last
            /// </summary>
            internal int index = -1;

            public override string ToString()
            {
                return string.Format("[{0}]", index);
            }

        }

        /// <summary>
        /// Get or set the item corresponding to a handle.
        /// </summary>
        /// <exception cref="InvalidPriorityQueueHandleException">if the handle is invalid for this queue</exception>
        /// <param name="handle">The reference into the heap</param>
        /// <returns></returns>
        public T this[IPriorityQueueHandle<T> handle]
        {
            get
            {
                CheckHandle(handle, out int cell, out bool isfirst);

                return isfirst ? heap[cell].first : heap[cell].last;
            }
            set => Replace(handle, value);
        }


        /// <summary>
        /// Check safely if a handle is valid for this queue and if so, report the corresponding queue item.
        /// </summary>
        /// <param name="handle">The handle to check</param>
        /// <param name="item">If the handle is valid this will contain the corresponding item on output.</param>
        /// <returns>True if the handle is valid.</returns>
        public bool Find(IPriorityQueueHandle<T> handle, out T item)
        {
            if (handle is not Handle myhandle)
            {
                item = default;
                return false;
            }
            int toremove = myhandle.index;
            int cell = toremove / 2;
            bool isfirst = toremove % 2 == 0;
            {
                if (toremove == -1 || toremove >= size)
                {
                    item = default;
                    return false;
                }
                Handle actualhandle = (isfirst ? heap[cell].firsthandle : heap[cell].lasthandle)!;
                if (actualhandle != myhandle)
                {
                    item = default;
                    return false;
                }
            }
            item = isfirst ? heap[cell].first : heap[cell].last;
            return true;
        }


        /// <summary>
        /// Add an item to the priority queue, receiving a
        /// handle for the item in the queue,
        /// or reusing an already existing handle.
        /// </summary>
        /// <param name="handle">On output: a handle for the added item.
        /// On input: null for allocating a new handle, an invalid handle for reuse.
        /// A handle for reuse must be compatible with this priority queue,
        /// by being created by a priority queue of the same runtime type, but not
        /// necessarily the same priority queue object.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>True since item will always be added unless the call throws an exception.</returns>
        public bool Add(ref IPriorityQueueHandle<T> handle, T item)
        {
            stamp++;
            Handle myhandle = (Handle)handle;
            if (myhandle == null)
            {
                handle = myhandle = new Handle();
            }
            else
                if (myhandle.index != -1)
            {
                throw new InvalidPriorityQueueHandleException("Handle not valid for reuse");
            }

            if (Add(myhandle, item))
            {
                RaiseItemsAdded(item, 1);
                RaiseCollectionChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete an item with a handle from a priority queue.
        /// </summary>
        /// <exception cref="InvalidPriorityQueueHandleException">if the handle is invalid</exception>
        /// <param name="handle">The handle for the item. The handle will be invalidated, but reusable.</param>
        /// <returns>The deleted item</returns>
        public T Delete(IPriorityQueueHandle<T> handle)
        {
            stamp++;
            Handle myhandle = CheckHandle(handle, out int cell, out bool isfirst);

            T retval;
            myhandle.index = -1;
            int lastcell = (size - 1) / 2;

            if (cell == lastcell)
            {
                if (isfirst)
                {
                    retval = heap[cell].first;
                    if (size % 2 == 0)
                    {
                        UpdateFirst(cell, heap[cell].last, heap[cell].lasthandle);
                        heap[cell].last = default;
                        heap[cell].lasthandle = null;
                    }
                    else
                    {
                        heap[cell].first = default;
                        heap[cell].firsthandle = null;
                    }
                }
                else
                {
                    retval = heap[cell].last;
                    heap[cell].last = default;
                    heap[cell].lasthandle = null;
                }
                size--;
            }
            else if (isfirst)
            {
                retval = heap[cell].first;

                if (size % 2 == 0)
                {
                    UpdateFirst(cell, heap[lastcell].last, heap[lastcell].lasthandle);
                    heap[lastcell].last = default;
                    heap[lastcell].lasthandle = null;
                }
                else
                {
                    UpdateFirst(cell, heap[lastcell].first, heap[lastcell].firsthandle);
                    heap[lastcell].first = default;
                    heap[lastcell].firsthandle = null;
                }

                size--;
                if (HeapifyMin(cell))
                {
                    BubbleUpMax(cell);
                }
                else
                {
                    BubbleUpMin(cell);
                }
            }
            else
            {
                retval = heap[cell].last;

                if (size % 2 == 0)
                {
                    UpdateLast(cell, heap[lastcell].last, heap[lastcell].lasthandle);
                    heap[lastcell].last = default;
                    heap[lastcell].lasthandle = null;
                }
                else
                {
                    UpdateLast(cell, heap[lastcell].first, heap[lastcell].firsthandle);
                    heap[lastcell].first = default;
                    heap[lastcell].firsthandle = null;
                }

                size--;
                if (HeapifyMax(cell))
                {
                    BubbleUpMin(cell);
                }
                else
                {
                    BubbleUpMax(cell);
                }
            }

            RaiseItemsRemoved(retval, 1);
            RaiseCollectionChanged();

            return retval;
        }

        private Handle CheckHandle(IPriorityQueueHandle<T> handle, out int cell, out bool isfirst)
        {
            Handle myhandle = (Handle)handle;
            int toremove = myhandle.index;
            cell = toremove / 2;
            isfirst = toremove % 2 == 0;
            {
                if (toremove == -1 || toremove >= size)
                {
                    throw new InvalidPriorityQueueHandleException("Invalid handle, index out of range");
                }

                Handle actualhandle = (isfirst ? heap[cell].firsthandle : heap[cell].lasthandle)!;
                if (actualhandle != myhandle)
                {
                    throw new InvalidPriorityQueueHandleException("Invalid handle, doesn't match queue");
                }
            }
            return myhandle;
        }


        /// <summary>
        /// Replace an item with a handle in a priority queue with a new item.
        /// Typically used for changing the priority of some queued object.
        /// </summary>
        /// <param name="handle">The handle for the old item</param>
        /// <param name="item">The new item</param>
        /// <returns>The old item</returns>
        public T Replace(IPriorityQueueHandle<T> handle, T item)
        {
            stamp++;
            CheckHandle(handle, out int cell, out bool isfirst);
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            T retval;

            if (isfirst)
            {
                retval = heap[cell].first;
                heap[cell].first = item;
                if (size == 1)
                {
                }
                else if (size == 2 * cell + 1) // cell == lastcell
                {
                    int p = (cell + 1) / 2 - 1;
                    if (comparer.Compare(item, heap[p].last) > 0)
                    {
                        Handle thehandle = heap[cell].firsthandle!;
                        UpdateFirst(cell, heap[p].last, heap[p].lasthandle);
                        UpdateLast(p, item, thehandle);
                        BubbleUpMax(p);
                    }
                    else
                    {
                        BubbleUpMin(cell);
                    }
                }
                else if (HeapifyMin(cell))
                {
                    BubbleUpMax(cell);
                }
                else
                {
                    BubbleUpMin(cell);
                }
            }
            else
            {
                retval = heap[cell].last;
                heap[cell].last = item;
                if (HeapifyMax(cell))
                {
                    BubbleUpMin(cell);
                }
                else
                {
                    BubbleUpMax(cell);
                }
            }

            RaiseItemsRemoved(retval, 1);
            RaiseItemsAdded(item, 1);
            RaiseCollectionChanged();

            return retval;
        }

        /// <summary>
        /// Find the current least item of this priority queue.
        /// </summary>
        /// <param name="handle">On return: the handle of the item.</param>
        /// <returns>The least item.</returns>
        public T FindMin(out IPriorityQueueHandle<T> handle)
        {
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            handle = heap[0].firsthandle!;

            return heap[0].first;
        }

        /// <summary>
        /// Find the current largest item of this priority queue.
        /// </summary>
        /// <param name="handle">On return: the handle of the item.</param>
        /// <returns>The largest item.</returns>
        public T FindMax(out IPriorityQueueHandle<T> handle)
        {
            if (size == 0)
            {
                throw new NoSuchItemException();
            }
            else if (size == 1)
            {
                handle = heap[0].firsthandle!;
                return heap[0].first;
            }
            else
            {
                handle = heap[0].lasthandle!;
                return heap[0].last;
            }
        }

        /// <summary>
        /// Remove the least item from this priority queue.
        /// </summary>
        /// <param name="handle">On return: the handle of the removed item.</param>
        /// <returns>The removed item.</returns>
        public T DeleteMin(out IPriorityQueueHandle<T> handle)
        {
            stamp++;
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            T retval = heap[0].first;
            Handle myhandle = heap[0].firsthandle!;
            handle = myhandle;
            if (myhandle != null)
            {
                myhandle.index = -1;
            }

            if (size == 1)
            {
                size = 0;
                heap[0].first = default;
                heap[0].firsthandle = null;
            }
            else
            {
                int lastcell = (size - 1) / 2;

                if (size % 2 == 0)
                {
                    UpdateFirst(0, heap[lastcell].last, heap[lastcell].lasthandle);
                    heap[lastcell].last = default;
                    heap[lastcell].lasthandle = null;
                }
                else
                {
                    UpdateFirst(0, heap[lastcell].first, heap[lastcell].firsthandle);
                    heap[lastcell].first = default;
                    heap[lastcell].firsthandle = null;
                }

                size--;
                HeapifyMin(0);
            }

            RaiseItemsRemoved(retval, 1);
            RaiseCollectionChanged();
            return retval;

        }

        /// <summary>
        /// Remove the largest item from this priority queue.
        /// </summary>
        /// <param name="handle">On return: the handle of the removed item.</param>
        /// <returns>The removed item.</returns>
        public T DeleteMax(out IPriorityQueueHandle<T> handle)
        {
            stamp++;
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            T retval;
            Handle myhandle;

            if (size == 1)
            {
                size = 0;
                retval = heap[0].first;
                myhandle = heap[0].firsthandle!;
                if (myhandle != null)
                {
                    myhandle.index = -1;
                }

                heap[0].first = default;
                heap[0].firsthandle = null;
            }
            else
            {
                retval = heap[0].last;
                myhandle = heap[0].lasthandle!;
                if (myhandle != null)
                {
                    myhandle.index = -1;
                }

                int lastcell = (size - 1) / 2;

                if (size % 2 == 0)
                {
                    UpdateLast(0, heap[lastcell].last, heap[lastcell].lasthandle);
                    heap[lastcell].last = default;
                    heap[lastcell].lasthandle = null;
                }
                else
                {
                    UpdateLast(0, heap[lastcell].first, heap[lastcell].firsthandle);
                    heap[lastcell].first = default;
                    heap[lastcell].firsthandle = null;
                }

                size--;
                HeapifyMax(0);
            }
            RaiseItemsRemoved(retval, 1);
            RaiseCollectionChanged();
            handle = myhandle!;
            return retval;
        }

        #endregion
    }
}
