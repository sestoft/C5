// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A sorted generic dictionary based on a red-black tree set.
/// </summary>
public class TreeDictionary<K, V> : SortedDictionaryBase<K, V>, IDictionary<K, V>, ISortedDictionary<K, V>
{

    #region Constructors

    /// <summary>
    /// Create a red-black tree dictionary using the natural comparer for keys.
    /// <exception cref="ArgumentException"/> if the key type K is not comparable.
    /// </summary>
    public TreeDictionary() : this(SCG.Comparer<K>.Default, EqualityComparer<K>.Default) { }

    /// <summary>
    /// Create a red-black tree dictionary using an external comparer for keys.
    /// </summary>
    /// <param name="comparer">The external comparer</param>
    public TreeDictionary(SCG.IComparer<K> comparer) : this(comparer, new ComparerZeroHashCodeEqualityComparer<K>(comparer)) { }

    private TreeDictionary(SCG.IComparer<K> comparer, SCG.IEqualityComparer<K> equalityComparer)
        : base(comparer, equalityComparer)
    {
        pairs = sortedPairs = new TreeSet<System.Collections.Generic.KeyValuePair<K, V>>(new KeyValuePairComparer<K, V>(comparer));
    }

    #endregion

    //TODO: put in interface
    /// <summary>
    /// Make a snapshot of the current state of this dictionary
    /// </summary>
    /// <returns>The snapshot</returns>
    public SCG.IEnumerable<System.Collections.Generic.KeyValuePair<K, V>> Snapshot()
    {
        TreeDictionary<K, V> res = (TreeDictionary<K, V>)MemberwiseClone();

        res.pairs = (TreeSet<System.Collections.Generic.KeyValuePair<K, V>>)((TreeSet<System.Collections.Generic.KeyValuePair<K, V>>)sortedPairs).Snapshot();
        return res;
    }
}