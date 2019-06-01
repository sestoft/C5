using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a sorted dictionary.
    ///
    /// <i>Suitable for wrapping a Dictionary. <see cref="T:C5.Dictionary`2"/></i>
    /// </summary>
    [Serializable]
    public class GuardedSortedDictionary<K, V> : GuardedDictionary<K, V>, ISortedDictionary<K, V>
    {
        #region Fields

        private readonly ISortedDictionary<K, V> sorteddict;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a sorted dictionary in a read-only wrapper
        /// </summary>
        /// <param name="sorteddict">the dictionary</param>
        public GuardedSortedDictionary(ISortedDictionary<K, V> sorteddict)
            : base(sorteddict)
        { this.sorteddict = sorteddict; }

        #endregion

        #region ISortedDictionary<K,V> Members

        /// <summary>
        /// The key comparer used by this dictionary.
        /// </summary>
        /// <value></value>
        public System.Collections.Generic.IComparer<K> Comparer => sorteddict.Comparer;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public new ISorted<K>? Keys => null;

        /// <summary>
        /// Find the entry in the dictionary whose key is the
        /// predecessor of the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="res">The predecessor, if any</param>
        /// <returns>True if key has a predecessor</returns>
        public bool TryPredecessor(K key, out KeyValuePair<K, V> res)
        {
            return sorteddict.TryPredecessor(key, out res);
        }

        /// <summary>
        /// Find the entry in the dictionary whose key is the
        /// successor of the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="res">The successor, if any</param>
        /// <returns>True if the key has a successor</returns>
        public bool TrySuccessor(K key, out KeyValuePair<K, V> res)
        {
            return sorteddict.TrySuccessor(key, out res);
        }

        /// <summary>
        /// Find the entry in the dictionary whose key is the
        /// weak predecessor of the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="res">The predecessor, if any</param>
        /// <returns>True if key has a weak predecessor</returns>
        public bool TryWeakPredecessor(K key, out KeyValuePair<K, V> res)
        {
            return sorteddict.TryWeakPredecessor(key, out res);
        }

        /// <summary>
        /// Find the entry in the dictionary whose key is the
        /// weak successor of the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="res">The weak successor, if any</param>
        /// <returns>True if the key has a weak successor</returns>
        public bool TryWeakSuccessor(K key, out KeyValuePair<K, V> res)
        {
            return sorteddict.TryWeakSuccessor(key, out res);
        }

        /// <summary>
        /// Get the entry in the wrapped dictionary whose key is the
        /// predecessor of a specified key.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such entry exists </exception>    
        /// <param name="key">The key</param>
        /// <returns>The entry</returns>
        public KeyValuePair<K, V> Predecessor(K key)
        { return sorteddict.Predecessor(key); }

        /// <summary>
        /// Get the entry in the wrapped dictionary whose key is the
        /// successor of a specified key.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such entry exists </exception>    
        /// <param name="key">The key</param>
        /// <returns>The entry</returns>
        public KeyValuePair<K, V> Successor(K key)
        { return sorteddict.Successor(key); }


        /// <summary>
        /// Get the entry in the wrapped dictionary whose key is the
        /// weak predecessor of a specified key.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such entry exists </exception>    
        /// <param name="key">The key</param>
        /// <returns>The entry</returns>
        public KeyValuePair<K, V> WeakPredecessor(K key)
        { return sorteddict.WeakPredecessor(key); }


        /// <summary>
        /// Get the entry in the wrapped dictionary whose key is the
        /// weak successor of a specified key.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such entry exists </exception>    
        /// <param name="key">The key</param>
        /// <returns>The entry</returns>
        public KeyValuePair<K, V> WeakSuccessor(K key)
        { return sorteddict.WeakSuccessor(key); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<K, V> FindMin()
        {
            return sorteddict.FindMin();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public KeyValuePair<K, V> DeleteMin()
        { throw new ReadOnlyCollectionException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<K, V> FindMax()
        {
            return sorteddict.FindMax();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns></returns>
        public KeyValuePair<K, V> DeleteMax()
        { throw new ReadOnlyCollectionException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="lowEntry"></param>
        /// <param name="lowIsValid"></param>
        /// <param name="highEntry"></param>
        /// <param name="highIsValid"></param>
        /// <returns></returns>
        public bool Cut(IComparable<K> c, out KeyValuePair<K, V> lowEntry, out bool lowIsValid, out KeyValuePair<K, V> highEntry, out bool highIsValid)
        {
            return sorteddict.Cut(c, out lowEntry, out lowIsValid, out highEntry, out highIsValid); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public IDirectedEnumerable<KeyValuePair<K, V>> RangeFrom(K bot)
        {
            return new GuardedDirectedEnumerable<KeyValuePair<K, V>>(sorteddict.RangeFrom(bot));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IDirectedEnumerable<KeyValuePair<K, V>> RangeFromTo(K bot, K top)
        {
            return new GuardedDirectedEnumerable<KeyValuePair<K, V>>(sorteddict.RangeFromTo(bot, top));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public IDirectedEnumerable<KeyValuePair<K, V>> RangeTo(K top)
        {
            return new GuardedDirectedEnumerable<KeyValuePair<K, V>>(sorteddict.RangeTo(top));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDirectedCollectionValue<KeyValuePair<K, V>> RangeAll()
        {
            return new GuardedDirectedCollectionValue<KeyValuePair<K, V>>(sorteddict.RangeAll());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="items"></param>
        public void AddSorted(System.Collections.Generic.IEnumerable<KeyValuePair<K, V>> items)
        { throw new ReadOnlyCollectionException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="low"></param>
        public void RemoveRangeFrom(K low)
        { throw new ReadOnlyCollectionException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="low"></param>
        /// <param name="hi"></param>
        public void RemoveRangeFromTo(K low, K hi)
        { throw new ReadOnlyCollectionException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <param name="hi"></param>
        public void RemoveRangeTo(K hi)
        { throw new ReadOnlyCollectionException(); }

        #endregion
    }
}