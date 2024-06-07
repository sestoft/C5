// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System.Linq;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A base class for implementing an IEnumerable&lt;T&gt;
/// </summary>
public abstract class EnumerableBase<T> : SCG.IEnumerable<T>
{
    /// <summary>
    /// Create an enumerator for this collection.
    /// </summary>
    /// <returns>The enumerator</returns>
    public abstract SCG.IEnumerator<T> GetEnumerator();

    /// <summary>
    /// Count the number of items in an enumerable by enumeration
    /// </summary>
    /// <param name="items">The enumerable to count</param>
    /// <returns>The size of the enumerable</returns>
    protected static int CountItems(SCG.IEnumerable<T> items) => items is ICollectionValue<T> collectionValue ? collectionValue.Count : items.Count();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
