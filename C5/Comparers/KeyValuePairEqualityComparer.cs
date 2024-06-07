using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// Default equalityComparer for dictionary entries.
/// Operations only look at keys and uses an externally defined equalityComparer for that.
/// </summary>
public sealed class KeyValuePairEqualityComparer<K, V> : SCG.IEqualityComparer<SCG.KeyValuePair<K, V>>
{
    private readonly SCG.IEqualityComparer<K> _keyEqualityComparer;

    /// <summary>
    /// Create an entry equalityComparer using the default equalityComparer for keys
    /// </summary>
    public KeyValuePairEqualityComparer() { _keyEqualityComparer = EqualityComparer<K>.Default; }

    /// <summary>
    /// Create an entry equalityComparer from a specified item equalityComparer for the keys
    /// </summary>
    /// <param name="keyEqualityComparer">The key equalitySCG.Comparer</param>
    public KeyValuePairEqualityComparer(SCG.IEqualityComparer<K> keyEqualityComparer)
    {
        _keyEqualityComparer = keyEqualityComparer ?? throw new NullReferenceException("Key equality comparer cannot be null");
    }

    /// <summary>
    /// Get the hash code of the entry
    /// </summary>
    /// <param name="entry">The entry</param>
    /// <returns>The hash code of the key</returns>
    public int GetHashCode(SCG.KeyValuePair<K, V> entry) { return _keyEqualityComparer.GetHashCode(entry.Key); }

    /// <summary>
    /// Test two entries for equality
    /// </summary>
    /// <param name="entry1">First entry</param>
    /// <param name="entry2">Second entry</param>
    /// <returns>True if keys are equal</returns>
    public bool Equals(SCG.KeyValuePair<K, V> entry1, SCG.KeyValuePair<K, V> entry2)
    {
        return _keyEqualityComparer.Equals(entry1.Key, entry2.Key);
    }
}