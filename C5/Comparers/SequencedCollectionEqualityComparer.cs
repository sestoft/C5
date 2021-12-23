using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Prototype for a sequenced equalityComparer for something (T) that implements ISequenced[W].
    /// This will use ISequenced[W] specific implementations of the equality comparer operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    public class SequencedCollectionEqualityComparer<T, W> : IEqualityComparer<T>
        where T : ISequenced<W>
    {
        private static SequencedCollectionEqualityComparer<T, W> cached;

        private SequencedCollectionEqualityComparer() { }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static SequencedCollectionEqualityComparer<T, W> Default => cached ??= new SequencedCollectionEqualityComparer<T, W>();
        /// <summary>
        /// Get the hash code with respect to this sequenced equalityComparer
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <returns>The hash code</returns>
        public int GetHashCode(T collection) { return collection.GetSequencedHashCode(); }

        /// <summary>
        /// Check if two items are equal with respect to this sequenced equalityComparer
        /// </summary>
        /// <param name="collection1">first collection</param>
        /// <param name="collection2">second collection</param>
        /// <returns>True if equal</returns>
        public bool Equals(T collection1, T collection2) { return collection1 == null ? collection2 == null : collection1.SequencedEquals(collection2); }
    }
}