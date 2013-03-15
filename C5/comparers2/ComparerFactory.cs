// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using System.Collections.Generic;

namespace C5.Comparers
{
    /// <summary>
    /// Factory class to create comparers and equality comparers using Func delegates
    /// </summary>
    /// <typeparam name="T">The type to compare</typeparam>
    public class ComparerFactory<T>
    {
        /// <summary>
        /// Create a new comparer.
        /// </summary>
        /// <param name="comparer">The compare function.</param>
        /// <returns>The comparer</returns>
        public static IComparer<T> CreateComparer(Func<T, T, int> comparer)
        {
            return new Comparer<T>(comparer);
        }

        /// <summary>
        /// Creates a new equality comparer.
        /// </summary>
        /// <param name="equals">The equals function.</param>
        /// <param name="getHashCode">The getHashCode function.</param>
        /// <returns>The equality comparer.</returns>
        public static IEqualityComparer<T> CreateEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            return new EqualityComparer<T>(equals, getHashCode);
        }
    }
}