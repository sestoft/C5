// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

internal class SortedArrayDictionary<K, V> : SortedDictionaryBase<K, V>
{
    #region Constructors

    public SortedArrayDictionary() : this(SCG.Comparer<K>.Default, EqualityComparer<K>.Default) { }

    /// <summary>
    /// Create a red-black tree dictionary using an external comparer for keys.
    /// </summary>
    /// <param name="comparer">The external comparer</param>
    public SortedArrayDictionary(SCG.IComparer<K> comparer) : this(comparer, new ComparerZeroHashCodeEqualityComparer<K>(comparer)) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="comparer"></param>
    /// <param name="equalityComparer"></param>
    public SortedArrayDictionary(SCG.IComparer<K> comparer, SCG.IEqualityComparer<K> equalityComparer)
        : base(comparer, equalityComparer)
    {
        pairs = sortedpairs = new SortedArray<SCG.KeyValuePair<K, V>>(new KeyValuePairComparer<K, V>(comparer));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="comparer"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="capacity"></param>
    public SortedArrayDictionary(int capacity, SCG.IComparer<K> comparer, SCG.IEqualityComparer<K> equalityComparer)
        : base(comparer, equalityComparer)
    {
        pairs = sortedpairs = new SortedArray<SCG.KeyValuePair<K, V>>(capacity, new KeyValuePairComparer<K, V>(comparer));
    }
    #endregion
}
