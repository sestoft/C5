// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A base class for implementing a sorted dictionary based on a sorted set collection implementation.
/// <i>See the source code for <see cref="T:C5.TreeDictionary`2"/> for an example</i>
/// </summary>
public abstract class SortedDictionaryBase<K, V> : DictionaryBase<K, V>, ISortedDictionary<K, V>
{
    #region Fields

    /// <summary>
    ///
    /// </summary>
    protected ISorted<SCG.KeyValuePair<K, V>> sortedPairs;
    private readonly SCG.IComparer<K> keyComparer;

    /// <summary>
    ///
    /// </summary>
    /// <param name="keyComparer"></param>
    /// <param name="keyequalityComparer"></param>
    protected SortedDictionaryBase(SCG.IComparer<K> keyComparer, SCG.IEqualityComparer<K> keyequalityComparer) : base(keyequalityComparer) { this.keyComparer = keyComparer; }

    #endregion

    #region ISortedDictionary<K,V> Members

    /// <summary>
    /// The key comparer used by this dictionary.
    /// </summary>
    /// <value></value>
    public SCG.IComparer<K> Comparer => keyComparer;

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public new ISorted<K> Keys => new SortedKeysCollection(this, sortedPairs, keyComparer, EqualityComparer);

    /// <summary>
    /// Find the entry in the dictionary whose key is the
    /// predecessor of the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="res">The predecessor, if any</param>
    /// <returns>True if key has a predecessor</returns>
    public bool TryPredecessor(K key, out SCG.KeyValuePair<K, V> res)
    {
        return sortedPairs.TryPredecessor(new SCG.KeyValuePair<K, V>(key, default!), out res);
    }

    /// <summary>
    /// Find the entry in the dictionary whose key is the
    /// successor of the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="res">The successor, if any</param>
    /// <returns>True if the key has a successor</returns>
    public bool TrySuccessor(K key, out SCG.KeyValuePair<K, V> res)
    {
        return sortedPairs.TrySuccessor(new SCG.KeyValuePair<K, V>(key, default!), out res);
    }

    /// <summary>
    /// Find the entry in the dictionary whose key is the
    /// weak predecessor of the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="res">The predecessor, if any</param>
    /// <returns>True if key has a weak predecessor</returns>
    public bool TryWeakPredecessor(K key, out SCG.KeyValuePair<K, V> res)
    {
        return sortedPairs.TryWeakPredecessor(new SCG.KeyValuePair<K, V>(key, default!), out res);
    }

    /// <summary>
    /// Find the entry in the dictionary whose key is the
    /// weak successor of the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="res">The weak successor, if any</param>
    /// <returns>True if the key has a weak successor</returns>
    public bool TryWeakSuccessor(K key, out SCG.KeyValuePair<K, V> res)
    {
        return sortedPairs.TryWeakSuccessor(new SCG.KeyValuePair<K, V>(key, default!), out res);
    }

    /// <summary>
    /// Get the entry in the dictionary whose key is the
    /// predecessor of the specified key.
    /// </summary>
    /// <exception cref="NoSuchItemException"></exception>
    /// <param name="key">The key</param>
    /// <returns>The entry</returns>
    public SCG.KeyValuePair<K, V> Predecessor(K key)
    {
        return sortedPairs.Predecessor(new SCG.KeyValuePair<K, V>(key, default!));
    }

    /// <summary>
    /// Get the entry in the dictionary whose key is the
    /// successor of the specified key.
    /// </summary>
    /// <exception cref="NoSuchItemException"></exception>
    /// <param name="key">The key</param>
    /// <returns>The entry</returns>
    public SCG.KeyValuePair<K, V> Successor(K key)
    {
        return sortedPairs.Successor(new SCG.KeyValuePair<K, V>(key, default!));
    }

    /// <summary>
    /// Get the entry in the dictionary whose key is the
    /// weak predecessor of the specified key.
    /// </summary>
    /// <exception cref="NoSuchItemException"></exception>
    /// <param name="key">The key</param>
    /// <returns>The entry</returns>
    public SCG.KeyValuePair<K, V> WeakPredecessor(K key)
    {
        return sortedPairs.WeakPredecessor(new SCG.KeyValuePair<K, V>(key, default!));
    }

    /// <summary>
    /// Get the entry in the dictionary whose key is the
    /// weak successor of the specified key.
    /// </summary>
    /// <exception cref="NoSuchItemException"></exception>
    /// <param name="key">The key</param>
    /// <returns>The entry</returns>
    public SCG.KeyValuePair<K, V> WeakSuccessor(K key)
    {
        return sortedPairs.WeakSuccessor(new SCG.KeyValuePair<K, V>(key, default!));
    }

    #endregion

    #region ISortedDictionary<K,V> Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public SCG.KeyValuePair<K, V> FindMin()
    {
        return sortedPairs.FindMin();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public SCG.KeyValuePair<K, V> DeleteMin()
    {
        return sortedPairs.DeleteMin();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public SCG.KeyValuePair<K, V> FindMax()
    {
        return sortedPairs.FindMax();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public SCG.KeyValuePair<K, V> DeleteMax()
    {
        return sortedPairs.DeleteMax();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cutter"></param>
    /// <param name="lowEntry"></param>
    /// <param name="lowIsValid"></param>
    /// <param name="highEntry"></param>
    /// <param name="highIsValid"></param>
    /// <returns></returns>
    public bool Cut(IComparable<K> cutter, out SCG.KeyValuePair<K, V> lowEntry, out bool lowIsValid, out SCG.KeyValuePair<K, V> highEntry, out bool highIsValid)
    {
        return sortedPairs.Cut(new KeyValuePairComparable(cutter), out lowEntry, out lowIsValid, out highEntry, out highIsValid);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public IDirectedEnumerable<SCG.KeyValuePair<K, V>> RangeFrom(K bot)
    {
        return sortedPairs.RangeFrom(new SCG.KeyValuePair<K, V>(bot, default));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    public IDirectedEnumerable<SCG.KeyValuePair<K, V>> RangeFromTo(K bot, K top)
    {
        return sortedPairs.RangeFromTo(new SCG.KeyValuePair<K, V>(bot, default), new SCG.KeyValuePair<K, V>(top, default));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="top"></param>
    /// <returns></returns>
    public IDirectedEnumerable<SCG.KeyValuePair<K, V>> RangeTo(K top)
    {
        return sortedPairs.RangeTo(new SCG.KeyValuePair<K, V>(top, default!));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public IDirectedCollectionValue<SCG.KeyValuePair<K, V>> RangeAll()
    {
        return sortedPairs.RangeAll();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="items"></param>
    public void AddSorted(SCG.IEnumerable<SCG.KeyValuePair<K, V>> items)
    {
        sortedPairs.AddSorted(items);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="lowKey"></param>
    public void RemoveRangeFrom(K lowKey)
    {
        sortedPairs.RemoveRangeFrom(new SCG.KeyValuePair<K, V>(lowKey, default!));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="lowKey"></param>
    /// <param name="highKey"></param>
    public void RemoveRangeFromTo(K lowKey, K highKey)
    {
        sortedPairs.RemoveRangeFromTo(new SCG.KeyValuePair<K, V>(lowKey, default!), new SCG.KeyValuePair<K, V>(highKey, default!));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="highKey"></param>
    public void RemoveRangeTo(K highKey)
    {
        sortedPairs.RemoveRangeTo(new SCG.KeyValuePair<K, V>(highKey, default!));
    }

    #endregion
    private class KeyValuePairComparable : IComparable<SCG.KeyValuePair<K, V>>
    {
        private readonly IComparable<K> cutter;

        internal KeyValuePairComparable(IComparable<K> cutter) { this.cutter = cutter; }

        public int CompareTo(SCG.KeyValuePair<K, V> other) { return cutter.CompareTo(other.Key); }

        public bool Equals(SCG.KeyValuePair<K, V> other) { return cutter.Equals(other.Key); }
    }

    private class ProjectedDirectedEnumerable(IDirectedEnumerable<SCG.KeyValuePair<K, V>> directedpairs) : MappedDirectedEnumerable<SCG.KeyValuePair<K, V>, K>(directedpairs)
    {
        public override K Map(SCG.KeyValuePair<K, V> pair) => pair.Key;
    }

    private class ProjectedDirectedCollectionValue(IDirectedCollectionValue<SCG.KeyValuePair<K, V>> directedpairs) : MappedDirectedCollectionValue<SCG.KeyValuePair<K, V>, K>(directedpairs)
    {
        public override K Map(SCG.KeyValuePair<K, V> pair) => pair.Key;
    }

    private class SortedKeysCollection : SequencedBase<K>, ISorted<K>
    {
        private readonly ISortedDictionary<K, V> sortedDict;

        //TODO: eliminate this. Only problem is the Find method because we lack method on dictionary that also
        //      returns the actual key.
        private readonly ISorted<SCG.KeyValuePair<K, V>> sortedPairs;
        private readonly SCG.IComparer<K> comparer;

        internal SortedKeysCollection(ISortedDictionary<K, V> sortedDict, ISorted<SCG.KeyValuePair<K, V>> sortedPairs, SCG.IComparer<K> comparer, SCG.IEqualityComparer<K> itemequalityComparer)
            : base(itemequalityComparer)
        {
            this.sortedDict = sortedDict;
            this.sortedPairs = sortedPairs;
            this.comparer = comparer;
        }

        public override K Choose() { return sortedDict.Choose().Key; }

        public override SCG.IEnumerator<K> GetEnumerator()
        {
            foreach (SCG.KeyValuePair<K, V> p in sortedDict)
            {
                yield return p.Key;
            }
        }

        public override bool IsEmpty => sortedDict.IsEmpty;

        public override int Count => sortedDict.Count;

        public override Speed CountSpeed => sortedDict.CountSpeed;

        #region ISorted<K> Members

        public K FindMin() { return sortedDict.FindMin().Key; }

        public K DeleteMin() { throw new ReadOnlyCollectionException(); }

        public K FindMax() { return sortedDict.FindMax().Key; }

        public K DeleteMax() { throw new ReadOnlyCollectionException(); }

        public SCG.IComparer<K> Comparer => comparer;

        public bool TryPredecessor(K item, out K res)
        {
            bool success = sortedDict.TryPredecessor(item, out SCG.KeyValuePair<K, V> pRes);
            res = pRes.Key;
            return success;
        }

        public bool TrySuccessor(K item, out K res)
        {
            bool success = sortedDict.TrySuccessor(item, out SCG.KeyValuePair<K, V> pRes);
            res = pRes.Key;
            return success;
        }

        public bool TryWeakPredecessor(K item, out K res)
        {
            bool success = sortedDict.TryWeakPredecessor(item, out SCG.KeyValuePair<K, V> pRes);
            res = pRes.Key;
            return success;
        }

        public bool TryWeakSuccessor(K item, out K res)
        {
            bool success = sortedDict.TryWeakSuccessor(item, out SCG.KeyValuePair<K, V> pRes);
            res = pRes.Key;
            return success;
        }

        public K Predecessor(K item) { return sortedDict.Predecessor(item).Key; }

        public K Successor(K item) { return sortedDict.Successor(item).Key; }

        public K WeakPredecessor(K item) { return sortedDict.WeakPredecessor(item).Key; }

        public K WeakSuccessor(K item) { return sortedDict.WeakSuccessor(item).Key; }

        public bool Cut(IComparable<K> c, out K low, out bool lowIsValid, out K high, out bool highIsValid)
        {
            bool retval = sortedDict.Cut(c, out SCG.KeyValuePair<K, V> lowpair, out lowIsValid, out SCG.KeyValuePair<K, V> highpair, out highIsValid);
            low = lowpair.Key;
            high = highpair.Key;
            return retval;
        }

        public IDirectedEnumerable<K> RangeFrom(K bot)
        {
            return new ProjectedDirectedEnumerable(sortedDict.RangeFrom(bot));
        }

        public IDirectedEnumerable<K> RangeFromTo(K bot, K top)
        {
            return new ProjectedDirectedEnumerable(sortedDict.RangeFromTo(bot, top));
        }

        public IDirectedEnumerable<K> RangeTo(K top)
        {
            return new ProjectedDirectedEnumerable(sortedDict.RangeTo(top));
        }

        public IDirectedCollectionValue<K> RangeAll()
        {
            return new ProjectedDirectedCollectionValue(sortedDict.RangeAll());
        }

        public void AddSorted(SCG.IEnumerable<K> items) { throw new ReadOnlyCollectionException(); }

        public void RemoveRangeFrom(K low) { throw new ReadOnlyCollectionException(); }

        public void RemoveRangeFromTo(K low, K hi) { throw new ReadOnlyCollectionException(); }

        public void RemoveRangeTo(K hi) { throw new ReadOnlyCollectionException(); }
        #endregion

        #region ICollection<K> Members
        public Speed ContainsSpeed => sortedDict.ContainsSpeed;

        public bool Contains(K key) { return sortedDict.Contains(key); }

        public int ContainsCount(K item) { return sortedDict.Contains(item) ? 1 : 0; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<K> UniqueItems()
        {
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<SCG.KeyValuePair<K, int>> ItemMultiplicities()
        {
            return new MultiplicityOne<K>(this);
        }


        public bool ContainsAll(SCG.IEnumerable<K> items)
        {
            //TODO: optimize?
            foreach (K item in items)
            {
                if (!sortedDict.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Find(ref K item)
        {
            SCG.KeyValuePair<K, V> p = new(item, default);
            bool retval = sortedPairs.Find(ref p);
            item = p.Key;
            return retval;
        }

        public bool FindOrAdd(ref K item) { throw new ReadOnlyCollectionException(); }

        public bool Update(K item) { throw new ReadOnlyCollectionException(); }

        public bool Update(K item, out K olditem) { throw new ReadOnlyCollectionException(); }

        public bool UpdateOrAdd(K item) { throw new ReadOnlyCollectionException(); }

        public bool UpdateOrAdd(K item, out K olditem) { throw new ReadOnlyCollectionException(); }

        public bool Remove(K item) { throw new ReadOnlyCollectionException(); }

        public bool Remove(K item, out K removeditem) { throw new ReadOnlyCollectionException(); }

        public void RemoveAllCopies(K item) { throw new ReadOnlyCollectionException(); }

        public void RemoveAll(SCG.IEnumerable<K> items) { throw new ReadOnlyCollectionException(); }

        public void Clear() { throw new ReadOnlyCollectionException(); }

        public void RetainAll(SCG.IEnumerable<K> items) { throw new ReadOnlyCollectionException(); }

        #endregion

        #region IExtensible<K> Members
        public override bool IsReadOnly => true;

        public bool AllowsDuplicates => false;

        public bool DuplicatesByCounting => true;

        public bool Add(K item) { throw new ReadOnlyCollectionException(); }

        void SCG.ICollection<K>.Add(K item) { throw new ReadOnlyCollectionException(); }

        public void AddAll(SCG.IEnumerable<K> items) { throw new ReadOnlyCollectionException(); }

        public bool Check() { return sortedDict.Check(); }

        #endregion

        #region IDirectedCollectionValue<K> Members

        public override IDirectedCollectionValue<K> Backwards()
        {
            return RangeAll().Backwards();
        }

        #endregion

        #region IDirectedEnumerable<K> Members

        IDirectedEnumerable<K> IDirectedEnumerable<K>.Backwards() { return Backwards(); }
        #endregion
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="stringbuilder"></param>
    /// <param name="rest"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public override bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
    {
        return Showing.ShowDictionary(this, stringbuilder, ref rest, formatProvider);
    }
}