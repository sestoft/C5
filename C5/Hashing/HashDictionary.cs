// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A generic dictionary class based on a hash set class <see cref="T:C5.HashSet`1"/>.
/// </summary>
public class HashDictionary<K, V> : DictionaryBase<K, V>, IDictionary<K, V>
{
    /// <summary>
    /// Create a hash dictionary using a default equalityComparer for the keys.
    /// Initial capacity of internal table will be 16 entries and threshold for
    /// expansion is 66% fill.
    /// </summary>
    public HashDictionary() : this(EqualityComparer<K>.Default) { }

    /// <summary>
    /// Create a hash dictionary using a custom equalityComparer for the keys.
    /// Initial capacity of internal table will be 16 entries and threshold for
    /// expansion is 66% fill.
    /// </summary>
    /// <param name="keyEqualityComparer">The external key equalitySCG.Comparer</param>
    public HashDictionary(SCG.IEqualityComparer<K> keyEqualityComparer)
        : base(keyEqualityComparer)
    {
        pairs = new HashSet<SCG.KeyValuePair<K, V>>(new KeyValuePairEqualityComparer<K, V>(keyEqualityComparer));
    }

    /// <summary>
    /// Create a hash dictionary using a custom equalityComparer and prescribing the
    /// initial size of the dictionary and a non-default threshold for internal table expansion.
    /// </summary>
    /// <param name="capacity">The initial capacity. Will be rounded upwards to nearest
    /// power of 2, at least 16.</param>
    /// <param name="fill">The expansion threshold. Must be between 10% and 90%.</param>
    /// <param name="keyEqualityComparer">The external key equalitySCG.Comparer</param>
    public HashDictionary(int capacity, double fill, SCG.IEqualityComparer<K> keyEqualityComparer)
        : base(keyEqualityComparer)
    {
        pairs = new HashSet<SCG.KeyValuePair<K, V>>(capacity, fill, new KeyValuePairEqualityComparer<K, V>(keyEqualityComparer));
    }
}