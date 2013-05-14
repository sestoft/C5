/*
 Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
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
using System.Linq;
using System.Reflection;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Utility class for building default generic equality comparers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EqualityComparer<T>
    {
        private static SCG.IEqualityComparer<T> _default;

        readonly static Type SequencedCollectionEqualityComparer = typeof(SequencedCollectionEqualityComparer<,>);

        readonly static Type UnsequencedCollectionEqualityComparer = typeof(UnsequencedCollectionEqualityComparer<,>);

        /// <summary>
        /// A default generic equality comparer for type T. The procedure is as follows:
        /// <list>
        /// <item>If the actual generic argument T implements the generic interface
        /// <see cref="T:C5.ISequenced`1"/> for some value W of its generic parameter T,
        /// the equalityComparer will be <see cref="T:C5.SequencedCollectionEqualityComparer`2"/></item>
        /// <item>If the actual generic argument T implements 
        /// <see cref="T:C5.ICollection`1"/> for some value W of its generic parameter T,
        /// the equalityComparer will be <see cref="T:C5.UnsequencedCollectionEqualityComparer`2"/></item>
        /// <item>Otherwise the SCG.EqualityComparer&lt;T&gt;.Default is returned</item>
        /// </list>   
        /// </summary>
        /// <value>The comparer</value>
        public static SCG.IEqualityComparer<T> Default
        {
            get
            {
                if (_default != null)
                {
                    return _default;
                }

                var type = typeof(T);
                var interfaces = type.GetInterfaces();

                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(ISequenced<>)))
                {
                    return CreateAndCache(SequencedCollectionEqualityComparer.MakeGenericType(new[] { type, type.GetGenericArguments()[0] }));
                }

                var isequenced = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ISequenced<>)));
                if (isequenced != null)
                {
                    return CreateAndCache(SequencedCollectionEqualityComparer.MakeGenericType(new[] { type, isequenced.GetGenericArguments()[0] }));
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(ICollection<>)))
                {
                    return CreateAndCache(UnsequencedCollectionEqualityComparer.MakeGenericType(new[] { type, type.GetGenericArguments()[0] }));
                }

                var icollection = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ICollection<>)));
                if (icollection != null)
                {
                    return CreateAndCache(UnsequencedCollectionEqualityComparer.MakeGenericType(new[] { type, icollection.GetGenericArguments()[0] }));
                }

                return _default = SCG.EqualityComparer<T>.Default;
            }
        }

        private static SCG.IEqualityComparer<T> CreateAndCache(Type equalityComparertype)
        {
            return _default = (SCG.IEqualityComparer<T>)(equalityComparertype.GetProperty("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null, null));
        }
    }

    /// <summary>
    /// An equalityComparer compatible with a given comparer. All hash codes are 0, 
    /// meaning that anything based on hash codes will be quite inefficient.
    /// <para><b>Note: this will give a new EqualityComparer each time created!</b></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
            if (comparer == null)
                throw new NullReferenceException("Comparer cannot be null");
            this.comparer = comparer;
        }
        /// <summary>
        /// A trivial, inefficient hash fuction. Compatible with any equality relation.
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
