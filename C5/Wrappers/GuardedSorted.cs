using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a sorted collection
    ///
    /// <i>This is mainly interesting as a base of other guard classes</i>
    /// </summary>
    [Serializable]
    public class GuardedSorted<T> : GuardedSequenced<T>, ISorted<T>
    {
        #region Fields

        private readonly ISorted<T> sorted;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a sorted collection in a read-only wrapper
        /// </summary>
        /// <param name="sorted"></param>
        public GuardedSorted(ISorted<T> sorted) : base(sorted) { this.sorted = sorted; }

        #endregion

        #region ISorted<T> Members

        /// <summary>
        /// Find the strict predecessor of item in the guarded sorted collection,
        /// that is, the greatest item in the collection smaller than the item.
        /// </summary>
        /// <param name="item">The item to find the predecessor for.</param>
        /// <param name="res">The predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a predecessor; otherwise false.</returns>
        public bool TryPredecessor(T item, out T res) { return sorted.TryPredecessor(item, out res); }


        /// <summary>
        /// Find the strict successor of item in the guarded sorted collection,
        /// that is, the least item in the collection greater than the supplied value.
        /// </summary>
        /// <param name="item">The item to find the successor for.</param>
        /// <param name="res">The successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a successor; otherwise false.</returns>
        public bool TrySuccessor(T item, out T res) { return sorted.TrySuccessor(item, out res); }


        /// <summary>
        /// Find the weak predecessor of item in the guarded sorted collection,
        /// that is, the greatest item in the collection smaller than or equal to the item.
        /// </summary>
        /// <param name="item">The item to find the weak predecessor for.</param>
        /// <param name="res">The weak predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak predecessor; otherwise false.</returns>
        public bool TryWeakPredecessor(T item, out T res) { return sorted.TryWeakPredecessor(item, out res); }


        /// <summary>
        /// Find the weak successor of item in the sorted collection,
        /// that is, the least item in the collection greater than or equal to the supplied value.
        /// </summary>
        /// <param name="item">The item to find the weak successor for.</param>
        /// <param name="res">The weak successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak successor; otherwise false.</returns>
        public bool TryWeakSuccessor(T item, out T res) { return sorted.TryWeakSuccessor(item, out res); }


        /// <summary>
        /// Find the predecessor of the item in the wrapped sorted collection
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists </exception>    
        /// <param name="item">The item</param>
        /// <returns>The predecessor</returns>
        public T Predecessor(T item) { return sorted.Predecessor(item); }


        /// <summary>
        /// Find the Successor of the item in the wrapped sorted collection
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists </exception>    
        /// <param name="item">The item</param>
        /// <returns>The Successor</returns>
        public T Successor(T item) { return sorted.Successor(item); }


        /// <summary>
        /// Find the weak predecessor of the item in the wrapped sorted collection
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists </exception>    
        /// <param name="item">The item</param>
        /// <returns>The weak predecessor</returns>
        public T WeakPredecessor(T item) { return sorted.WeakPredecessor(item); }


        /// <summary>
        /// Find the weak Successor of the item in the wrapped sorted collection
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists </exception>    
        /// <param name="item">The item</param>
        /// <returns>The weak Successor</returns>
        public T WeakSuccessor(T item) { return sorted.WeakSuccessor(item); }


        /// <summary>
        /// Run Cut on the wrapped sorted collection
        /// </summary>
        /// <param name="c"></param>
        /// <param name="low"></param>
        /// <param name="lval"></param>
        /// <param name="high"></param>
        /// <param name="hval"></param>
        /// <returns></returns>
        public bool Cut(IComparable<T> c, out T low, out bool lval, out T high, out bool hval)
        { return sorted.Cut(c, out low, out lval, out high, out hval); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public IDirectedEnumerable<T> RangeFrom(T bot) { return sorted.RangeFrom(bot); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IDirectedEnumerable<T> RangeFromTo(T bot, T top)
        { return sorted.RangeFromTo(bot, top); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public IDirectedEnumerable<T> RangeTo(T top) { return sorted.RangeTo(top); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <returns></returns>
        public IDirectedCollectionValue<T> RangeAll() { return sorted.RangeAll(); }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="items"></param>
        public void AddSorted(System.Collections.Generic.IEnumerable<T> items)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="low"></param>
        public void RemoveRangeFrom(T low)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="low"></param>
        /// <param name="hi"></param>
        public void RemoveRangeFromTo(T low, T hi)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="hi"></param>
        public void RemoveRangeTo(T hi)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        #endregion

        #region IPriorityQueue<T> Members

        /// <summary>
        /// Find the minimum of the wrapped collection
        /// </summary>
        /// <returns>The minimum</returns>
        public T FindMin() { return sorted.FindMin(); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public T DeleteMin()
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


        /// <summary>
        /// Find the maximum of the wrapped collection
        /// </summary>
        /// <returns>The maximum</returns>
        public T FindMax() { return sorted.FindMax(); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public T DeleteMax()
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        //TODO: we should guard the comparer!
        /// <summary>
        /// The comparer object supplied at creation time for the underlying collection
        /// </summary>
        /// <value>The comparer</value>
        public System.Collections.Generic.IComparer<T> Comparer => sorted.Comparer;
        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
        { return Backwards(); }

        #endregion
    }
}