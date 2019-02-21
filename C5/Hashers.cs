/*
 Copyright (c) 2003-2019 Niels Kokholm, Peter Sestoft, and Rasmus Lystrøm
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

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
        SCG.IComparer<T> comparer;
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

    /// <summary>
    /// Prototype for a sequenced equalityComparer for something (T) that implements ISequenced[W].
    /// This will use ISequenced[W] specific implementations of the equality comparer operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    [Serializable]
    public class SequencedCollectionEqualityComparer<T, W> : SCG.IEqualityComparer<T>
        where T : ISequenced<W>
    {
        static SequencedCollectionEqualityComparer<T, W> cached;
        SequencedCollectionEqualityComparer() { }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static SequencedCollectionEqualityComparer<T, W> Default
        {
            get { return cached ?? (cached = new SequencedCollectionEqualityComparer<T, W>()); }
        }
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

    /// <summary>
    /// Prototype for an unsequenced equalityComparer for something (T) that implements ICollection[W]
    /// This will use ICollection[W] specific implementations of the equalityComparer operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    [Serializable]
    public class UnsequencedCollectionEqualityComparer<T, W> : SCG.IEqualityComparer<T>
        where T : ICollection<W>
    {
        static UnsequencedCollectionEqualityComparer<T, W> cached;
        UnsequencedCollectionEqualityComparer() { }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static UnsequencedCollectionEqualityComparer<T, W> Default { get { return cached ?? (cached = new UnsequencedCollectionEqualityComparer<T, W>()); } }
        /// <summary>
        /// Get the hash code with respect to this unsequenced equalityComparer
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <returns>The hash code</returns>
        public int GetHashCode(T collection) { return collection.GetUnsequencedHashCode(); }


        /// <summary>
        /// Check if two collections are equal with respect to this unsequenced equalityComparer
        /// </summary>
        /// <param name="collection1">first collection</param>
        /// <param name="collection2">second collection</param>
        /// <returns>True if equal</returns>
        public bool Equals(T collection1, T collection2) { return collection1 == null ? collection2 == null : collection1.UnsequencedEquals(collection2); }
    }
}
