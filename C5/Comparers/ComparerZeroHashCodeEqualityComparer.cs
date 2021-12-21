// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// An equalityComparer compatible with a given comparer. All hash codes are 0,
    /// meaning that anything based on hash codes will be quite inefficient.
    /// <para><b>Note: this will give a new EqualityComparer each time created!</b></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal class ComparerZeroHashCodeEqualityComparer<T> : SCG.IEqualityComparer<T>
    {
        private readonly SCG.IComparer<T> comparer;
        /// <summary>
        /// Create a trivial <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> compatible with the
        /// <see cref="T:System.Collections.Generic.IComparer`1"/> <code>comparer</code>
        /// </summary>
        /// <param name="comparer"></param>
        public ComparerZeroHashCodeEqualityComparer(SCG.IComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new NullReferenceException("Comparer cannot be null");
        }
        /// <summary>
        /// A trivial, inefficient hash function. Compatible with any equality relation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>0</returns>
        public int GetHashCode(T item) { return 0; }
        /// <summary>
        /// Equality of two items as defined by the comparer.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public bool Equals(T item1, T item2) { return comparer.Compare(item1, item2) == 0; }
    }
}
