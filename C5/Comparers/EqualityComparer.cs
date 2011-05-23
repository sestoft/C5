// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using System.Collections.Generic;

namespace C5.Comparers
{
    /// <summary>
    /// Defines methods to support the comparison of objects for equality.
    /// This class is intentionally declared internal - use the ComparerFactory to create an instance.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    internal class EqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        /// <summary>
        /// Constructs and equality comparer using two Func delegates.
        /// </summary>
        /// <param name="equals">The equals function.</param>
        /// <param name="getHashCode">The get hash code function.</param>
        public EqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}