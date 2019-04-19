// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;
namespace C5
{
    /// <summary>
    /// A collection class implementing a sorted dynamic array data structure.
    /// </summary>
    [Serializable]
    public class SortedArray<T> : ArrayBase<T>, IIndexedSorted<T>
    {
        #region Events

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public override EventTypeEnum ListenableEvents { get { return EventTypeEnum.Basic; } }

        #endregion

        #region Fields

        readonly SCG.IComparer<T> _comparer;

        #endregion

        #region Util
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <param name="middle">The least index, middle, for which array[middle] >= item</param>
        /// <returns>True if item found</returns>
        private bool BinarySearch(T item, out int middle)
        {
            int bottom = 0, top = size;

            middle = top / 2;

            while (top > bottom)
            {
                int comparer;

                if ((comparer = _comparer.Compare(array[middle], item)) == 0)
                {
                    return true;
                }

                if (comparer > 0)
                { 
                    top = middle; 
                }
                else
                {
                    bottom = middle + 1;
                }

                middle = bottom + ((top - bottom) / 2); 
            }

            return false;
        }

        private int indexOf(T item)
        {

            if (BinarySearch(item, out int ind))
                return ind;

            return ~ind;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a dynamic sorted array with a natural comparer
        /// (and item equalityComparer,  assumed compatible)
        /// </summary>
        /// <exception cref="NotComparableException">If <code>T</code> is not comparable.
        /// </exception>
        public SortedArray() : this(8) { }


        /// <summary>
        /// Create a dynamic sorted array with a natural comparer 
        /// (and item equalityComparer,  assumed compatible)
        /// and prescribed initial capacity.
        /// </summary>
        /// <exception cref="NotComparableException">If <code>T</code> is not comparable.
        /// </exception>
        /// <param name="capacity">The capacity</param>
        public SortedArray(int capacity)
            : this(capacity, SCG.Comparer<T>.Default, EqualityComparer<T>.Default) { }


        /// <summary>
        /// Create a dynamic sorted array with an external comparer.
        /// <para>The itemequalityComparer will be compatible 
        /// <see cref="T:C5.ComparerZeroHashCodeEqualityComparer`1"/> since the 
        /// default equalityComparer for T (<see cref="P:C5.EqualityComparer`1.Default"/>)
        /// is unlikely to be compatible with the external comparer. This makes the
        /// array inadequate for use as item in a collection of unsequenced or sequenced sets or bags
        /// (<see cref="T:C5.ICollection`1"/> and <see cref="T:C5.ISequenced`1"/>)
        /// </para>
        /// </summary>
        /// <param name="comparer">The comparer</param>
        public SortedArray(SCG.IComparer<T> comparer)
            : this(8, comparer) { }

        /// <summary>
        /// Create a dynamic sorted array with an external comparer
        /// and prescribed initial capacity.
        /// <para>The itemequalityComparer will be a compatible 
        /// <see cref="T:C5.ComparerZeroHashCodeEqualityComparer`1"/> since the 
        /// default equalityComparer for T (<see cref="P:C5.EqualityComparer`1.Default"/>)
        /// is unlikely to be compatible with the external comparer. This makes the
        /// sorted array inadequate for use as item in a collection of unsequenced or sequenced sets or bags
        /// (<see cref="T:C5.ICollection`1"/> and <see cref="T:C5.ISequenced`1"/>)
        /// </para>
        /// </summary>
        /// <param name="capacity">The capacity</param>
        /// <param name="comparer">The comparer</param>
        public SortedArray(int capacity, SCG.IComparer<T> comparer)
            : this(capacity, comparer, new ComparerZeroHashCodeEqualityComparer<T>(comparer)) { }

        /// <summary>
        /// Create a dynamic sorted array with an external comparer, an external item equalityComparer
        /// and prescribed initial capacity. This is the constructor to use if the collection 
        /// will be used as item in a hash table based collection.
        /// </summary>
        /// <param name="capacity">The capacity</param>
        /// <param name="comparer">The item comparer</param>
        /// <param name="equalityComparer">The item equalityComparer (assumed compatible)</param>
        public SortedArray(int capacity, SCG.IComparer<T> comparer, SCG.IEqualityComparer<T> equalityComparer)
            : base(capacity, equalityComparer)
        {
            this._comparer = comparer ?? throw new NullReferenceException("Comparer cannot be null");
        }

        #endregion

        #region IIndexedSorted<T> Members

        /// <summary>
        /// Determine the number of items at or above a supplied threshold.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountFrom(T bot)
        {

            BinarySearch(bot, out int lo);
            return size - lo;
        }


        /// <summary>
        /// Determine the number of items between two supplied thresholds.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive)</param>
        /// <param name="top">The upper bound (exclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountFromTo(T bot, T top)
        {

            BinarySearch(bot, out int lo);
            BinarySearch(top, out int hi);
            return hi > lo ? hi - lo : 0;
        }


        /// <summary>
        /// Determine the number of items below a supplied threshold.
        /// </summary>
        /// <param name="top">The upper bound (exclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountTo(T top)
        {

            BinarySearch(top, out int hi);
            return hi;
        }


        /// <summary>
        /// Query this sorted collection for items greater than or equal to a supplied value.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeFrom(T bot)
        {

            BinarySearch(bot, out int lo);
            return new Range(this, lo, size - lo, true);
        }


        /// <summary>
        /// Query this sorted collection for items between two supplied values.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive).</param>
        /// <param name="top">The upper bound (exclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
        {

            BinarySearch(bot, out int lo);
            BinarySearch(top, out int hi);

            int sz = hi - lo;

            return new Range(this, lo, sz, true);
        }


        /// <summary>
        /// Query this sorted collection for items less than a supplied value.
        /// </summary>
        /// <param name="top">The upper bound (exclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeTo(T top)
        {

            BinarySearch(top, out int hi);
            return new Range(this, 0, hi, true);
        }


        /// <summary>
        /// Create a new indexed sorted collection consisting of the items of this
        /// indexed sorted collection satisfying a certain predicate.
        /// </summary>
        /// <param name="f">The filter delegate defining the predicate.</param>
        /// <returns>The new indexed sorted collection.</returns>
        public IIndexedSorted<T> FindAll(Func<T, bool> f)
        {
            SortedArray<T> res = new SortedArray<T>(_comparer);
            int j = 0, rescap = res.array.Length;

            for (int i = 0; i < size; i++)
            {
                T a = array[i];

                if (f(a))
                {
                    if (j == rescap) res.expand(rescap = 2 * rescap, j);

                    res.array[j++] = a;
                }
            }

            res.size = j;
            return res;
        }


        /// <summary>
        /// Create a new indexed sorted collection consisting of the results of
        /// mapping all items of this list.
        /// <exception cref="ArgumentException"/> if the map is not increasing over 
        /// the items of this collection (with respect to the two given comparison 
        /// relations).
        /// </summary>
        /// <param name="m">The delegate definging the map.</param>
        /// <param name="c">The comparion relation to use for the result.</param>
        /// <returns>The new sorted collection.</returns>
        public IIndexedSorted<V> Map<V>(Func<T, V> m, SCG.IComparer<V> c)
        {
            SortedArray<V> res = new SortedArray<V>(size, c);

            if (size > 0)
            {
                V oldv = res.array[0] = m(array[0]), newv;

                for (int i = 1; i < size; i++)
                {
                    if (c.Compare(oldv, newv = res.array[i] = m(array[i])) >= 0)
                        throw new ArgumentException("mapper not monotonic");

                    oldv = newv;
                }
            }

            res.size = size;
            return res;
        }

        #endregion

        #region ISorted<T> Members

        /// <summary>
        /// Find the strict predecessor of item in the sorted array,
        /// that is, the greatest item in the collection smaller than the item.
        /// </summary>
        /// <param name="item">The item to find the predecessor for.</param>
        /// <param name="res">The predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a predecessor; otherwise false.</returns>
        public bool TryPredecessor(T item, out T res)
        {
            BinarySearch(item, out int lo);
            if (lo == 0)
            {
                res = default;
                return false;
            }
            else
            {
                res = array[lo - 1];
                return true;
            }
        }


        /// <summary>
        /// Find the strict successor of item in the sorted array,
        /// that is, the least item in the collection greater than the supplied value.
        /// </summary>
        /// <param name="item">The item to find the successor for.</param>
        /// <param name="res">The successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a successor; otherwise false.</returns>
        public bool TrySuccessor(T item, out T res)
        {
            if (BinarySearch(item, out int hi))
                hi++;
            if (hi >= size)
            {
                res = default;
                return false;
            }
            else
            {
                res = array[hi];
                return true;
            }
        }


        /// <summary>
        /// Find the weak predecessor of item in the sorted array,
        /// that is, the greatest item in the collection smaller than or equal to the item.
        /// </summary>
        /// <param name="item">The item to find the weak predecessor for.</param>
        /// <param name="res">The weak predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak predecessor; otherwise false.</returns>
        public bool TryWeakPredecessor(T item, out T res)
        {

            if (!BinarySearch(item, out int lo))
                lo--;

            if (lo < 0)
            {
                res = default;
                return false;
            }
            else
            {
                res = array[lo];
                return true;
            }
        }


        /// <summary>
        /// Find the weak successor of item in the sorted array,
        /// that is, the least item in the collection greater than or equal to the supplied value.
        /// </summary>
        /// <param name="item">The item to find the weak successor for.</param>
        /// <param name="res">The weak successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak successor; otherwise false.</returns>
        public bool TryWeakSuccessor(T item, out T res)
        {

            BinarySearch(item, out int hi);
            if (hi >= size)
            {
                res = default;
                return false;
            }
            else
            {
                res = array[hi];
                return true;
            }
        }


        /// <summary>
        /// Find the strict predecessor in the sorted collection of a particular value,
        /// i.e. the largest item in the collection less than the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is less than or equal to the minimum of this collection.)</exception>
        /// <param name="item">The item to find the predecessor for.</param>
        /// <returns>The predecessor.</returns>
        public T Predecessor(T item)
        {

            BinarySearch(item, out int lo);
            if (lo == 0)
                throw new NoSuchItemException();

            return array[lo - 1];
        }


        /// <summary>
        /// Find the strict successor in the sorted collection of a particular value,
        /// i.e. the least item in the collection greater than the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is greater than or equal to the maximum of this collection.)</exception>
        /// <param name="item">The item to find the successor for.</param>
        /// <returns>The successor.</returns>
        public T Successor(T item)
        {

            if (BinarySearch(item, out int hi)) hi++;

            if (hi >= size)
                throw new NoSuchItemException();

            return array[hi];
        }


        /// <summary>
        /// Find the weak predecessor in the sorted collection of a particular value,
        /// i.e. the largest item in the collection less than or equal to the supplied value.
        /// <exception cref="NoSuchItemException"/> if no such element exists (the
        /// supplied  value is less than the minimum of this collection.)
        /// </summary>
        /// <param name="item">The item to find the weak predecessor for.</param>
        /// <returns>The weak predecessor.</returns>
        public T WeakPredecessor(T item)
        {

            if (!BinarySearch(item, out int lo)) lo--;

            if (lo < 0)
                throw new NoSuchItemException();

            return array[lo];
        }


        /// <summary>
        /// Find the weak successor in the sorted collection of a particular value,
        /// i.e. the least item in the collection greater than or equal to the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is greater than the maximum of this collection.)</exception>
        /// <param name="item">The item to find the weak successor for.</param>
        /// <returns>The weak successor.</returns>
        public T WeakSuccessor(T item)
        {

            BinarySearch(item, out int hi);
            if (hi >= size)
                throw new NoSuchItemException();

            return array[hi];
        }


        /// <summary>
        /// Perform a search in the sorted collection for the ranges in which a
        /// non-increasing (i.e. weakly decreasing) function from the item type to 
        /// <code>int</code> is
        /// negative, zero respectively positive. If the supplied cut function is
        /// not non-increasing, the result of this call is undefined.
        /// </summary>
        /// <param name="c">The cut function <code>T</code> to <code>int</code>, given
        /// as an <code>IComparable&lt;T&gt;</code> object, where the cut function is
        /// the <code>c.CompareTo(T that)</code> method.</param>
        /// <param name="low">Returns the largest item in the collection, where the
        /// cut function is positive (if any).</param>
        /// <param name="lowIsValid">True if the cut function is positive somewhere
        /// on this collection.</param>
        /// <param name="high">Returns the least item in the collection, where the
        /// cut function is negative (if any).</param>
        /// <param name="highIsValid">True if the cut function is negative somewhere
        /// on this collection.</param>
        /// <returns></returns>
        public bool Cut(IComparable<T> c, out T low, out bool lowIsValid, out T high, out bool highIsValid)
        {
            int lbest = -1, rbest = size;

            low = default;
            lowIsValid = false;
            high = default;
            highIsValid = false;

            int bot = 0, top = size, mid, comp = -1, sol;

            mid = top / 2;
            while (top > bot)
            {
                if ((comp = c.CompareTo(array[mid])) == 0)
                    break;

                if (comp < 0)
                { rbest = top = mid; }
                else
                { lbest = mid; bot = mid + 1; }

                mid = (bot + top) / 2;
            }

            if (comp != 0)
            {
                if (lbest >= 0) { lowIsValid = true; low = array[lbest]; }

                if (rbest < size) { highIsValid = true; high = array[rbest]; }

                return false;
            }

            sol = mid;
            bot = sol - 1;

            //Invariant: c.Compare(array[x]) < 0  when rbest <= x < size 
            //           c.Compare(array[x]) >= 0 when x < bot)
            //(Assuming c.Compare monotonic)
            while (rbest > bot)
            {
                mid = (bot + rbest) / 2;
                if (c.CompareTo(array[mid]) < 0)
                { rbest = mid; }
                else
                { bot = mid + 1; }
            }

            if (rbest < size) { highIsValid = true; high = array[rbest]; }

            top = sol + 1;

            //Invariant: c.Compare(array[x]) > 0  when 0 <= x <= lbest
            //           c.Compare(array[x]) <= 0 when x>top)
            //(Assuming c.Compare monotonic)
            while (top > lbest)
            {
                mid = (lbest + top + 1) / 2;
                if (c.CompareTo(array[mid]) > 0)
                { lbest = mid; }
                else
                { top = mid - 1; }
            }

            if (lbest >= 0) { lowIsValid = true; low = array[lbest]; }

            return true;
        }


        IDirectedEnumerable<T> ISorted<T>.RangeFrom(T bot)
        { return RangeFrom(bot); }


        IDirectedEnumerable<T> ISorted<T>.RangeFromTo(T bot, T top)
        { return RangeFromTo(bot, top); }


        IDirectedEnumerable<T> ISorted<T>.RangeTo(T top)
        { return RangeTo(top); }


        /// <summary>
        /// Create a directed collection with the same items as this collection.
        /// </summary>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeAll()
        { return new Range(this, 0, size, true); }


        /// <summary>
        /// Add all the items from another collection with an enumeration order that 
        /// is increasing in the items.
        /// <exception cref="ArgumentException"/> if the enumerated items turns out
        /// not to be in increasing order.
        /// </summary>
        /// <param name="items">The collection to add.</param>
        public void AddSorted(SCG.IEnumerable<T> items)
        {
            //Unless items have <=1 elements we would expect it to be
            //too expensive to do repeated inserts, thus:
            updatecheck();

            int j = 0, i = 0, c, itemcount = countItems(items), numAdded = 0;
            SortedArray<T> res = new SortedArray<T>(size + itemcount, _comparer);
            T lastitem = default;
            T[] addedItems = new T[itemcount];

            foreach (T item in items)
            {
                while (i < size && (c = _comparer.Compare(array[i], item)) <= 0)
                {
                    lastitem = res.array[j++] = array[i++];
                    if (c == 0)
                        goto next;
                }

                if (j > 0 && _comparer.Compare(lastitem, item) >= 0)
                    throw new ArgumentException("Argument not sorted");

                addedItems[numAdded++] = lastitem = res.array[j++] = item;
            next:
                c = -1;
            }

            while (i < size) res.array[j++] = array[i++];

            size = j;
            array = res.array;
            raiseForAddAll(addedItems, numAdded);
        }


        /// <summary>
        /// Remove all items of this collection above or at a supplied threshold.
        /// </summary>
        /// <param name="low">The lower threshold (inclusive).</param>
        public void RemoveRangeFrom(T low)
        {

            BinarySearch(low, out int lowind);
            if (lowind == size)
                return;

            T[] removed = new T[size - lowind];
            Array.Copy(array, lowind, removed, 0, removed.Length);
            Array.Reverse(removed);

            Array.Clear(array, lowind, size - lowind);
            size = lowind;

            raiseForRemoveRange(removed);
        }


        /// <summary>
        /// Remove all items of this collection between two supplied thresholds.
        /// </summary>
        /// <param name="low">The lower threshold (inclusive).</param>
        /// <param name="hi">The upper threshold (exclusive).</param>
        public void RemoveRangeFromTo(T low, T hi)
        {

            BinarySearch(low, out int lowind);
            BinarySearch(hi, out int highind);
            if (highind <= lowind)
                return;

            T[] removed = new T[highind - lowind];
            Array.Copy(array, lowind, removed, 0, removed.Length);
            Array.Reverse(removed);

            Array.Copy(array, highind, array, lowind, size - highind);
            Array.Clear(array, size - highind + lowind, highind - lowind);
            size -= highind - lowind;

            raiseForRemoveRange(removed);
        }


        /// <summary>
        /// Remove all items of this collection below a supplied threshold.
        /// </summary>
        /// <param name="hi">The upper threshold (exclusive).</param>
        public void RemoveRangeTo(T hi)
        {

            BinarySearch(hi, out int highind);
            if (highind == 0)
                return;

            T[] removed = new T[highind];
            Array.Copy(array, 0, removed, 0, removed.Length);

            Array.Copy(array, highind, array, 0, size - highind);
            Array.Clear(array, size - highind, highind);
            size -= highind;

            raiseForRemoveRange(removed);
        }

        private void raiseForRemoveRange(T[] removed)
        {
            foreach (T item in removed)
                raiseItemsRemoved(item, 1);

            if (removed.Length > 0)
                raiseCollectionChanged();
        }

        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case).
        /// </summary>
        /// <value>Speed.Log</value>
        public Speed ContainsSpeed { get { return Speed.Log; } }

        /// <summary>
        /// Remove all items from this collection, resetting internal array size.
        /// </summary>
        public override void Clear()
        {
            int oldCount = size;
            base.Clear();
            if (oldCount > 0)
            {
                raiseCollectionCleared(true, oldCount);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        /// Check if this collection contains (an item equivalent according to the
        /// itemequalityComparer) to a particular value.
        /// </summary>
        /// <param name="item">The value to check for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public bool Contains(T item)
        {
            return BinarySearch(item, out _);
        }


        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, return in the ref argument (a
        /// binary copy of) the actual value found.
        /// </summary>
        /// <param name="item">The value to look for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public bool Find(ref T item)
        {

            if (BinarySearch(item, out int ind))
            {
                item = array[ind];
                return true;
            }

            return false;
        }


        //This should probably just be bool Add(ref T item); !!!
        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, return in the ref argument (a
        /// binary copy of) the actual value found. Else, add the item to the collection.
        /// </summary>
        /// <param name="item">The value to look for.</param>
        /// <returns>True if the item was added (hence not found).</returns>
        public bool FindOrAdd(ref T item)
        {
            updatecheck();


            if (BinarySearch(item, out int ind))
            {
                item = array[ind];
                return true;
            }

            if (size == array.Length) expand();

            Array.Copy(array, ind, array, ind + 1, size - ind);
            array[ind] = item;
            size++;
            raiseForAdd(item);
            return false;
        }


        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// to with a binary copy of the supplied value. If the collection has bag semantics,
        /// it is implementation dependent if this updates all equivalent copies in
        /// the collection or just one.
        /// </summary>
        /// <param name="item">Value to update.</param>
        /// <returns>True if the item was found and hence updated.</returns>
        public bool Update(T item)
        {
            return Update(item, out _); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool Update(T item, out T olditem)
        {
            updatecheck();


            if (BinarySearch(item, out int ind))
            {
                olditem = array[ind];
                array[ind] = item;
                raiseForUpdate(item, olditem);
                return true;
            }

            olditem = default;
            return false;
        }


        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// to with a binary copy of the supplied value; else add the value to the collection. 
        /// </summary>
        /// <param name="item">Value to add or update.</param>
        /// <returns>True if the item was found and updated (hence not added).</returns>
        public bool UpdateOrAdd(T item)
        {
            return UpdateOrAdd(item, out _); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool UpdateOrAdd(T item, out T olditem)
        {
            updatecheck();


            if (BinarySearch(item, out int ind))
            {
                olditem = array[ind];
                array[ind] = item;
                raiseForUpdate(item, olditem);
                return true;
            }

            if (size == array.Length) expand();

            Array.Copy(array, ind, array, ind + 1, size - ind);
            array[ind] = item;
            size++;
            olditem = default;
            raiseForAdd(item);
            return false;
        }


        /// <summary>
        /// Remove a particular item from this collection. If the collection has bag
        /// semantics only one copy equivalent to the supplied item is removed. 
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public bool Remove(T item)
        {

            updatecheck();
            if (BinarySearch(item, out int ind))
            {
                T removeditem = array[ind];
                Array.Copy(array, ind + 1, array, ind, size - ind - 1);
                array[--size] = default;
                raiseForRemove(removeditem);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Remove a particular item from this collection if found. If the collection
        /// has bag semantics only one copy equivalent to the supplied item is removed,
        /// which one is implementation dependent. 
        /// If an item was removed, report a binary copy of the actual item removed in 
        /// the argument.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <param name="removeditem">The removed value.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public bool Remove(T item, out T removeditem)
        {

            updatecheck();
            if (BinarySearch(item, out int ind))
            {
                removeditem = array[ind];
                Array.Copy(array, ind + 1, array, ind, size - ind - 1);
                array[--size] = default;
                raiseForRemove(removeditem);
                return true;
            }

            removeditem = default;
            return false;
        }


        /// <summary>
        /// Remove all items in another collection from this one. 
        /// </summary>
        /// <param name="items">The items to remove.</param>
        public void RemoveAll(SCG.IEnumerable<T> items)
        {
            //This is O(m*logn) with n bits extra storage
            //(Not better to collect the m items and sort them)
            updatecheck();

            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(this);
            bool mustFire = raiseHandler.MustFire;

            int[] toremove = new int[(size >> 5) + 1];
            int j = 0;

            foreach (T item in items)
                if (BinarySearch(item, out int ind))
                    toremove[ind >> 5] |= 1 << (ind & 31);

            for (int i = 0; i < size; i++)
                if ((toremove[i >> 5] & (1 << (i & 31))) == 0)
                    array[j++] = array[i];
                else if (mustFire)
                    raiseHandler.Remove(array[i]);

            Array.Clear(array, j, size - j);
            size = j;
            if (mustFire)
                raiseHandler.Raise();
        }

        /// <summary>
        /// Remove all items not in some other collection from this one. 
        /// </summary>
        /// <param name="items">The items to retain.</param>
        public void RetainAll(SCG.IEnumerable<T> items)
        {
            //This is O(m*logn) with n bits extra storage
            //(Not better to collect the m items and sort them)
            updatecheck();

            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(this);
            bool mustFire = raiseHandler.MustFire;

            int[] toretain = new int[(size >> 5) + 1];
            int j = 0;

            foreach (T item in items)
                if (BinarySearch(item, out int ind))
                    toretain[ind >> 5] |= 1 << (ind & 31);

            for (int i = 0; i < size; i++)
                if ((toretain[i >> 5] & (1 << (i & 31))) != 0)
                    array[j++] = array[i];
                else if (mustFire)
                    raiseHandler.Remove(array[i]);

            Array.Clear(array, j, size - j);
            size = j;
            if (mustFire)
                raiseHandler.Raise();
        }

        /// <summary>
        /// Check if this collection contains all the values in another collection.
        /// Multiplicities are not taken into account.
        /// </summary>
        /// <param name="items">The </param>
        /// <returns>True if all values in <code>items</code>is in this collection.</returns>
        public bool ContainsAll(SCG.IEnumerable<T> items)
        {

            foreach (T item in items)
                if (!BinarySearch(item, out int tmp))
                    return false;

            return true;
        }


        /// <summary>
        /// Count the number of items of the collection equal to a particular value.
        /// Returns 0 if and only if the value is not in the collection.
        /// </summary>
        /// <param name="item">The value to count.</param>
        /// <returns>The number of copies found (0 or 1).</returns>
        public int ContainsCount(T item)
        {
            return BinarySearch(item, out _) ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<T> UniqueItems() { return this; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            return new MultiplicityOne<T>(this);
        }

        /// <summary>
        /// Remove all (0 or 1) items equivalent to a given value.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        public void RemoveAllCopies(T item) { Remove(item); }


        /// <summary>
        /// Check the integrity of the internal data structures of this collection.
        /// Only available in DEBUG builds???
        /// </summary>
        /// <returns>True if check does not fail.</returns>
        public override bool Check()
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

                if (i > 0 && _comparer.Compare(array[i], array[i - 1]) <= 0)
                {
                    Logger.Log(string.Format("Inversion at index {0}", i));
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
        /// <value>False since this collection has set semantics</value>
        public bool AllowsDuplicates { get { return false; } }

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting { get { return true; } }

        /// <summary>
        /// Add an item to this collection if possible. If this collection has set
        /// semantics, the item will be added if not already in the collection. If
        /// bag semantics, the item will always be added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if item was added.</returns>
        public bool Add(T item)
        {
            updatecheck();


            if (BinarySearch(item, out int ind)) return false;

            InsertProtected(ind, item);
            raiseForAdd(item);
            return true;
        }

        /// <summary>
        /// Add an item to this collection if possible. 
        /// </summary>
        /// <param name="item">The item to add.</param>
        void SCG.ICollection<T>.Add(T item)
        {
            Add(item);
        }


        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. Since this
        /// collection has set semantics, only items not already in the collection
        /// will be added.
        /// </summary>
        /// <param name="items">The items to add</param>
        public void AddAll(SCG.IEnumerable<T> items)
        {
            int toadd = countItems(items), newsize = array.Length;

            while (newsize < size + toadd) { newsize *= 2; }

            T[] newarr = new T[newsize];

            toadd = 0;
            foreach (T item in items) newarr[size + toadd++] = item;

            Sorting.IntroSort<T>(newarr, size, toadd, _comparer);

            int j = 0, i = 0, numAdded = 0;
            T lastitem = default;
            T[] addedItems = new T[toadd];

            //The following eliminates duplicates (including duplicates in input)
            //while merging the old and new collection
            for (int k = size, klimit = size + toadd; k < klimit; k++)
            {
                while (i < size && _comparer.Compare(array[i], newarr[k]) <= 0)
                    lastitem = newarr[j++] = array[i++];

                if (j == 0 || _comparer.Compare(lastitem, newarr[k]) < 0)
                    addedItems[numAdded++] = lastitem = newarr[j++] = newarr[k];
            }

            while (i < size) newarr[j++] = array[i++];

            Array.Clear(newarr, j, size + toadd - j);
            size = j;
            array = newarr;

            raiseForAddAll(addedItems, numAdded);
        }

        private void raiseForAddAll(T[] addedItems, int numAdded)
        {
            if ((ActiveEvents & EventTypeEnum.Added) != 0)
                for (int i = 0; i < numAdded; i += 1)
                    raiseItemsAdded(addedItems[i], 1);
            if (numAdded > 0)
                raiseCollectionChanged();
        }

        #endregion

        #region IPriorityQueue<T> Members

        /// <summary>
        /// Find the current least item of this priority queue.
        /// </summary>
        /// <returns>The least item.</returns>
        public T FindMin()
        {
            if (size == 0)
                throw new NoSuchItemException();

            return array[0];
        }

        /// <summary>
        /// Remove the least item from this  priority queue.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMin()
        {
            updatecheck();
            if (size == 0)
                throw new NoSuchItemException();

            T retval = array[0];

            size--;
            Array.Copy(array, 1, array, 0, size);
            array[size] = default;
            raiseForRemove(retval);
            return retval;
        }


        /// <summary>
        /// Find the current largest item of this priority queue.
        /// </summary>
        /// <returns>The largest item.</returns>
        public T FindMax()
        {
            if (size == 0)
                throw new NoSuchItemException();

            return array[size - 1];
        }


        /// <summary>
        /// Remove the largest item from this  priority queue.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMax()
        {
            updatecheck();
            if (size == 0)
                throw new NoSuchItemException();

            T retval = array[size - 1];

            size--;
            array[size] = default;
            raiseForRemove(retval);
            return retval;
        }

        /// <summary>
        /// The comparer object supplied at creation time for this collection
        /// </summary>
        /// <value>The comparer</value>
        public SCG.IComparer<T> Comparer { get { return _comparer; } }

        #endregion

        #region IIndexed<T> Members

        /// <summary>
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <value>The i'th item of this list.</value>
        /// <param name="i">the index to lookup</param>
        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= size)
                    throw new IndexOutOfRangeException();

                return array[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed IndexingSpeed { get { return Speed.Constant; } }

        /// <summary>
        /// Searches for an item in the list going forwards from the start.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of item from start.</returns>
        public int IndexOf(T item) { return indexOf(item); }


        /// <summary>
        /// Searches for an item in the list going backwards from the end.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of of item from the end.</returns>
        public int LastIndexOf(T item) { return indexOf(item); }


        /// <summary>
        /// Remove the item at a specific position of the list.
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <param name="i">The index of the item to remove.</param>
        /// <returns>The removed item.</returns>
        public T RemoveAt(int i)
        {
            if (i < 0 || i >= size)
                throw new IndexOutOfRangeException("Index out of range for sequenced collectionvalue");

            updatecheck();

            T retval = array[i];

            size--;
            Array.Copy(array, i + 1, array, i, size - i);
            array[size] = default;
            raiseForRemoveAt(i, retval);
            return retval;
        }

        /// <summary>
        /// Remove all items in an index interval.
        /// <exception cref="IndexOutOfRangeException"/>???. 
        /// </summary>
        /// <param name="start">The index of the first item to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        public void RemoveInterval(int start, int count)
        {
            updatecheck();
            checkRange(start, count);
            Array.Copy(array, start + count, array, start, size - start - count);
            size -= count;
            Array.Clear(array, size, count);
            raiseForRemoveInterval(count);
        }

        private void raiseForRemoveInterval(int count)
        {
            if (ActiveEvents != 0 && count > 0)
            {
                raiseCollectionCleared(size == 0, count);
                raiseCollectionChanged();
            }
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
        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
        { return Backwards(); }

        #endregion
    }
}