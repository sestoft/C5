using System;

namespace C5
{
    internal class SortedArrayDictionary<K, V> : SortedDictionaryBase<K, V>
    {
        #region Constructors

        public SortedArrayDictionary() : this(System.Collections.Generic.Comparer<K>.Default, EqualityComparer<K>.Default) { }

        /// <summary>
        /// Create a red-black tree dictionary using an external comparer for keys.
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        public SortedArrayDictionary(System.Collections.Generic.IComparer<K> comparer) : this(comparer, new ComparerZeroHashCodeEqualityComparer<K>(comparer)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="equalityComparer"></param>
        public SortedArrayDictionary(System.Collections.Generic.IComparer<K> comparer, System.Collections.Generic.IEqualityComparer<K> equalityComparer)
            : base(comparer, equalityComparer)
        {
            pairs = sortedpairs = new SortedArray<System.Collections.Generic.KeyValuePair<K, V>>(new KeyValuePairComparer<K, V>(comparer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        /// <param name="equalityComparer"></param>
        public SortedArrayDictionary(int capacity, System.Collections.Generic.IComparer<K> comparer, System.Collections.Generic.IEqualityComparer<K> equalityComparer)
            : base(comparer, equalityComparer)
        {
            pairs = sortedpairs = new SortedArray<System.Collections.Generic.KeyValuePair<K, V>>(capacity, new KeyValuePairComparer<K, V>(comparer));
        }
        #endregion
    }
}