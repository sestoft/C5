using System;

namespace C5
{
    /// <summary>
    /// Read-only wrapper for indexed sorted collections
    ///
    /// <i>Suitable for wrapping TreeSet, TreeBag and SortedArray</i>
    /// </summary>
    public class GuardedIndexedSorted<T> : GuardedSorted<T>, IIndexedSorted<T>
    {
        #region Fields

        private readonly IIndexedSorted<T> indexedsorted;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap an indexed sorted collection in a read-only wrapper
        /// </summary>
        /// <param name="list">the indexed sorted collection</param>
        public GuardedIndexedSorted(IIndexedSorted<T> list)
            : base(list)
        { indexedsorted = list; }

        #endregion

        #region IIndexedSorted<T> Members

        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public new IDirectedCollectionValue<T> RangeFrom(T bot)
        { return indexedsorted.RangeFrom(bot); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public new IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
        { return indexedsorted.RangeFromTo(bot, top); }


        /// <summary>
        /// Get the specified range from the wrapped collection. 
        /// (The current implementation erroneously does not wrap the result.)
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public new IDirectedCollectionValue<T> RangeTo(T top)
        { return indexedsorted.RangeTo(top); }


        /// <summary>
        /// Report the number of items in the specified range of the wrapped collection
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public int CountFrom(T bot) { return indexedsorted.CountFrom(bot); }


        /// <summary>
        /// Report the number of items in the specified range of the wrapped collection
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public int CountFromTo(T bot, T top) { return indexedsorted.CountFromTo(bot, top); }


        /// <summary>
        /// Report the number of items in the specified range of the wrapped collection
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public int CountTo(T top) { return indexedsorted.CountTo(top); }


        /// <summary>
        /// Run FindAll on the wrapped collection with the indicated filter.
        /// The result will <b>not</b> be read-only.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public IIndexedSorted<T> FindAll(Func<T, bool> f)
        { return indexedsorted.FindAll(f); }


        /// <summary>
        /// Run Map on the wrapped collection with the indicated mapper.
        /// The result will <b>not</b> be read-only.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="c">The comparer to use in the result</param>
        /// <returns></returns>
        public IIndexedSorted<V> Map<V>(Func<T, V> m, System.Collections.Generic.IComparer<V> c)
        { return indexedsorted.Map(m, c); }

        #endregion

        #region IIndexed<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value>The i'th item of the wrapped sorted collection</value>
        public T this[int i] => indexedsorted[i];

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed IndexingSpeed => indexedsorted.IndexingSpeed;

        /// <summary> </summary>
        /// <value>A directed collection of the items in the indicated interval of the wrapped collection</value>
        public IDirectedCollectionValue<T> this[int start, int end] => new GuardedDirectedCollectionValue<T>(indexedsorted[start, end]);


        /// <summary>
        /// Find the (first) index of an item in the wrapped collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) { return indexedsorted.IndexOf(item); }


        /// <summary>
        /// Find the last index of an item in the wrapped collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int LastIndexOf(T item) { return indexedsorted.LastIndexOf(item); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="i"></param>
        /// <returns></returns>
        public T RemoveAt(int i)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


        /// <summary>
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void RemoveInterval(int start, int count)
        { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
        { return Backwards(); }

        #endregion
    }
}