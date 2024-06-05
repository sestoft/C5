using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Default comparer for dictionary entries in a sorted dictionary.
    /// Entry comparisons only look at keys and uses an externally defined comparer for that.
    /// </summary>
    public class KeyValuePairComparer<K, V> : IComparer<System.Collections.Generic.KeyValuePair<K, V>>
    {
        private readonly System.Collections.Generic.IComparer<K> comparer;


        /// <summary>
        /// Create an entry comparer for a item comparer of the keys
        /// </summary>
        /// <param name="comparer">Comparer of keys</param>
        public KeyValuePairComparer(System.Collections.Generic.IComparer<K> comparer)
        {
            this.comparer = comparer ?? throw new NullReferenceException();
        }


        /// <summary>
        /// Compare two entries
        /// </summary>
        /// <param name="entry1">First entry</param>
        /// <param name="entry2">Second entry</param>
        /// <returns>The result of comparing the keys</returns>
        public int Compare(System.Collections.Generic.KeyValuePair<K, V> entry1, System.Collections.Generic.KeyValuePair<K, V> entry2)
        {
            return comparer.Compare(entry1.Key, entry2.Key);
        }
    }
}