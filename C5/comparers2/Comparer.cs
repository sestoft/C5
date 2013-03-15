// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using System.Collections.Generic;

namespace C5.Comparers
{
    /// <summary>
    /// Defines a method that a type implements to compare two objects.
    /// This class is intentionally declared internal - use the ComparerFactory to create an instance.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    internal class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _compare;

        /// <summary>
        /// Constructs a comparer using one Func delegate.
        /// </summary>
        /// <param name="compare">The compare function.</param>
        public Comparer(Func<T, T, int> compare)
        {
            _compare = compare;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of x and y, as shown in the following table. Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.</returns>
        public int Compare(T x, T y)
        {
            return _compare(x, y);
        }
    }
}