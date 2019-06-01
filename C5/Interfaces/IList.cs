using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// This is an indexed collection, where the item order is chosen by 
    /// the user at insertion time.
    ///
    /// NBNBNB: we need a description of the view functionality here!
    /// </summary>
    public interface IList<T> : IIndexed<T>, IDisposable, System.Collections.Generic.IList<T>, System.Collections.IList
    {
        /// <summary>
        /// </summary>
        /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
        /// <value>The first item in this list.</value>
        T First { get; }

        /// <summary>
        /// </summary>
        /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
        /// <value>The last item in this list.</value>
        T Last { get; }

        /// <summary>
        /// Since <code>Add(T item)</code> always add at the end of the list,
        /// this describes if list has FIFO or LIFO semantics.
        /// </summary>
        /// <value>True if the <code>Remove()</code> operation removes from the
        /// start of the list, false if it removes from the end.</value>
        bool FIFO { get; set; }

        /// <summary>
        /// On this list, this indexer is read/write.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"> if index is negative or
        /// &gt;= the size of the collection.</exception>
        /// <value>The index'th item of this list.</value>
        /// <param name="index">The index of the item to fetch or store.</param>
        new T this[int index] { get; set; }

        #region Ambiguous calls when extending System.Collections.Generic.IList<T>

        #region System.Collections.Generic.ICollection<T>
        /// <summary>
        /// 
        /// </summary>
        new int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        new bool IsReadOnly { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        new bool Add(T item);

        /// <summary>
        /// 
        /// </summary>
        new void Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        new bool Contains(T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        new void CopyTo(T[] array, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        new bool Remove(T item);

        #endregion

        #region System.Collections.Generic.IList<T> proper

        /// <summary>
        /// Searches for an item in the list going forwards from the start. 
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of item from start. A negative number if item not found, 
        /// namely the one's complement of the index at which the Add operation would put the item.</returns>
        new int IndexOf(T item);

        /// <summary>
        /// Remove the item at a specific position of the list.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"> if <code>index</code> is negative or
        /// &gt;= the size of the collection.</exception>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The removed item.</returns>
        new T RemoveAt(int index);

        #endregion

        #endregion

        /*/// <summary>
    /// Insert an item at a specific index location in this list. 
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"> if <code>index</code> is negative or
    /// &gt; the size of the collection.</exception>
    /// <exception cref="DuplicateNotAllowedException"> if the list has
    /// <code>AllowsDuplicates==false</code> and the item is 
    /// already in the list.</exception>
    /// <param name="index">The index at which to insert.</param>
    /// <param name="item">The item to insert.</param>
    void Insert(int index, T item);*/

        /// <summary>
        /// Insert an item at the end of a compatible view, used as a pointer.
        /// <para>The <code>pointer</code> must be a view on the same list as
        /// <code>this</code> and the endpoint of <code>pointer</code> must be
        /// a valid insertion point of <code>this</code></para>
        /// </summary>
        /// <exception cref="IncompatibleViewException">If <code>pointer</code> 
        /// is not a view on the same list as <code>this</code></exception>
        /// <exception cref="IndexOutOfRangeException"><b>??????</b> if the endpoint of 
        ///  <code>pointer</code> is not inside <code>this</code></exception>
        /// <exception cref="DuplicateNotAllowedException"> if the list has
        /// <code>AllowsDuplicates==false</code> and the item is 
        /// already in the list.</exception>
        /// <param name="pointer"></param>
        /// <param name="item"></param>
        void Insert(IList<T> pointer, T item);

        /// <summary>
        /// Insert an item at the front of this list.
        /// <exception cref="DuplicateNotAllowedException"/> if the list has
        /// <code>AllowsDuplicates==false</code> and the item is 
        /// already in the list.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        void InsertFirst(T item);

        /// <summary>
        /// Insert an item at the back of this list.
        /// <exception cref="DuplicateNotAllowedException"/> if the list has
        /// <code>AllowsDuplicates==false</code> and the item is 
        /// already in the list.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        void InsertLast(T item);

        /// <summary>
        /// Insert into this list all items from an enumerable collection starting 
        /// at a particular index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"> if <code>index</code> is negative or
        /// &gt; the size of the collection.</exception>
        /// <exception cref="DuplicateNotAllowedException"> if the list has 
        /// <code>AllowsDuplicates==false</code> and one of the items to insert is
        /// already in the list.</exception>
        /// <param name="index">Index to start inserting at</param>
        /// <param name="items">Items to insert</param>
        void InsertAll(int index, IEnumerable<T> items);

        /// <summary>
        /// Create a new list consisting of the items of this list satisfying a 
        /// certain predicate.
        /// </summary>
        /// <param name="filter">The filter delegate defining the predicate.</param>
        /// <returns>The new list.</returns>
        IList<T> FindAll(Func<T, bool> filter);

        /// <summary>
        /// Create a new list consisting of the results of mapping all items of this
        /// list. The new list will use the default equalityComparer for the item type V.
        /// </summary>
        /// <typeparam name="V">The type of items of the new list</typeparam>
        /// <param name="mapper">The delegate defining the map.</param>
        /// <returns>The new list.</returns>
        IList<V> Map<V>(Func<T, V> mapper);

        /// <summary>
        /// Create a new list consisting of the results of mapping all items of this
        /// list. The new list will use a specified equalityComparer for the item type.
        /// </summary>
        /// <typeparam name="V">The type of items of the new list</typeparam>
        /// <param name="mapper">The delegate defining the map.</param>
        /// <param name="equalityComparer">The equalityComparer to use for the new list</param>
        /// <returns>The new list.</returns>
        IList<V> Map<V>(Func<T, V> mapper, System.Collections.Generic.IEqualityComparer<V> equalityComparer);

        /// <summary>
        /// Remove one item from the list: from the front if <code>FIFO</code>
        /// is true, else from the back.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        T Remove();

        /// <summary>
        /// Remove one item from the front of the list.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        T RemoveFirst();

        /// <summary>
        /// Remove one item from the back of the list.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        T RemoveLast();

        /// <summary>
        /// Create a list view on this list. 
        /// <exception cref="ArgumentOutOfRangeException"/> if the view would not fit into
        /// this list.
        /// </summary>
        /// <param name="start">The index in this list of the start of the view.</param>
        /// <param name="count">The size of the view.</param>
        /// <returns>The new list view.</returns>
        IList<T>? View(int start, int count);

        /// <summary>
        /// Create a list view on this list containing the (first) occurrence of a particular item. 
        /// <exception cref="NoSuchItemException"/> if the item is not in this list.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The new list view.</returns>
        IList<T>? ViewOf(T item);

        /// <summary>
        /// Create a list view on this list containing the last occurrence of a particular item. 
        /// <exception cref="NoSuchItemException"/> if the item is not in this list.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The new list view.</returns>
        IList<T>? LastViewOf(T item);

        /// <summary>
        /// Null if this list is not a view.
        /// </summary>
        /// <value>Underlying list for view.</value>
        IList<T>? Underlying { get; }

        /// <summary>
        /// </summary>
        /// <value>Offset for this list view or 0 for an underlying list.</value>
        int Offset { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        bool IsValid { get; }

        /// <summary>
        /// Slide this list view along the underlying list.
        /// </summary>
        /// <exception cref="NotAViewException"> if this list is not a view.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> if the operation
        /// would bring either end of the view outside the underlying list.</exception>
        /// <param name="offset">The signed amount to slide: positive to slide
        /// towards the end.</param>
        IList<T> Slide(int offset);

        /// <summary>
        /// Slide this list view along the underlying list, changing its size.
        /// 
        /// </summary>
        /// <exception cref="NotAViewException"> if this list is not a view.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> if the operation
        /// would bring either end of the view outside the underlying list.</exception>
        /// <param name="offset">The signed amount to slide: positive to slide
        /// towards the end.</param>
        /// <param name="size">The new size of the view.</param>
        IList<T> Slide(int offset, int size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        bool TrySlide(int offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        bool TrySlide(int offset, int size);

        /// <summary>
        /// 
        /// <para>Returns null if <code>otherView</code> is strictly to the left of this view</para>
        /// </summary>
        /// <param name="otherView"></param>
        /// <exception cref="IncompatibleViewException">If otherView does not have the same underlying list as this</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <code>otherView</code> is strictly to the left of this view</exception>
        /// <returns></returns>
        IList<T>? Span(IList<T> otherView);

        /// <summary>
        /// Reverse the list so the items are in the opposite sequence order.
        /// </summary>
        void Reverse();

        /// <summary>
        /// Check if this list is sorted according to the default sorting order
        /// for the item type T, as defined by the <see cref="T:C5.Comparer`1"/> class 
        /// </summary>
        /// <exception cref="NotComparableException">if T is not comparable</exception>
        /// <returns>True if the list is sorted, else false.</returns>
        bool IsSorted();

        /// <summary>
        /// Check if this list is sorted according to a specific sorting order.
        /// </summary>
        /// <param name="comparer">The comparer defining the sorting order.</param>
        /// <returns>True if the list is sorted, else false.</returns>
        bool IsSorted(System.Collections.Generic.IComparer<T> comparer);

        /// <summary>
        /// Sort the items of the list according to the default sorting order
        /// for the item type T, as defined by the <see cref="T:C5.Comparer`1"/> class 
        /// </summary>
        /// <exception cref="NotComparableException">if T is not comparable</exception>
        void Sort();

        /// <summary>
        /// Sort the items of the list according to a specified sorting order.
        /// <para>The sorting does not perform duplicate elimination or identify items
        /// according to the comparer or itemequalityComparer. I.e. the list as an 
        /// unsequenced collection with binary equality, will not change.
        /// </para>
        /// </summary>
        /// <param name="comparer">The comparer defining the sorting order.</param>
        void Sort(System.Collections.Generic.IComparer<T> comparer);


        /// <summary>
        /// Randomly shuffle the items of this list. 
        /// </summary>
        void Shuffle();


        /// <summary>
        /// Shuffle the items of this list according to a specific random source.
        /// </summary>
        /// <param name="rnd">The random source.</param>
        void Shuffle(Random rnd);
    }
}