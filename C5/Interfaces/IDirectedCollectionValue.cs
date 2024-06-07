// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// A sized generic collection, that can be enumerated backwards.
/// </summary>
public interface IDirectedCollectionValue<T> : ICollectionValue<T>, IDirectedEnumerable<T>
{
    /// <summary>
    /// Create a collection containing the same items as this collection, but
    /// whose enumerator will enumerate the items backwards. The new collection
    /// will become invalid if the original is modified. Method typically used as in
    /// <code>foreach (T x in coll.Backwards()) {...}</code>
    /// </summary>
    /// <returns>The backwards collection.</returns>
    new IDirectedCollectionValue<T> Backwards();

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the first one in enumeration order.
    /// </summary>
    /// <param name="predicate">A delegate
    /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
    /// <param name="item"></param>
    /// <returns>True is such an item exists</returns>
    bool FindLast(Func<T, bool> predicate, out T item);
}