// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// A collection where items are maintained in sorted order together
/// with their indexes in that order.
/// </summary>
public interface IIndexedSorted<T> : ISorted<T>, IIndexed<T>
{
    /// <summary>
    /// Determine the number of items at or above a supplied threshold.
    /// </summary>
    /// <param name="bot">The lower bound (inclusive)</param>
    /// <returns>The number of matching items.</returns>
    int CountFrom(T bot);


    /// <summary>
    /// Determine the number of items between two supplied thresholds.
    /// </summary>
    /// <param name="bot">The lower bound (inclusive)</param>
    /// <param name="top">The upper bound (exclusive)</param>
    /// <returns>The number of matching items.</returns>
    int CountFromTo(T bot, T top);


    /// <summary>
    /// Determine the number of items below a supplied threshold.
    /// </summary>
    /// <param name="top">The upper bound (exclusive)</param>
    /// <returns>The number of matching items.</returns>
    int CountTo(T top);


    /// <summary>
    /// Query this sorted collection for items greater than or equal to a supplied value.
    /// </summary>
    /// <param name="bot">The lower bound (inclusive).</param>
    /// <returns>The result directed collection.</returns>
    new IDirectedCollectionValue<T> RangeFrom(T bot);


    /// <summary>
    /// Query this sorted collection for items between two supplied values.
    /// </summary>
    /// <param name="bot">The lower bound (inclusive).</param>
    /// <param name="top">The upper bound (exclusive).</param>
    /// <returns>The result directed collection.</returns>
    new IDirectedCollectionValue<T> RangeFromTo(T bot, T top);


    /// <summary>
    /// Query this sorted collection for items less than a supplied value.
    /// </summary>
    /// <param name="top">The upper bound (exclusive).</param>
    /// <returns>The result directed collection.</returns>
    new IDirectedCollectionValue<T> RangeTo(T top);


    /// <summary>
    /// Create a new indexed sorted collection consisting of the items of this
    /// indexed sorted collection satisfying a certain predicate.
    /// </summary>
    /// <param name="predicate">The filter delegate defining the predicate.</param>
    /// <returns>The new indexed sorted collection.</returns>
    IIndexedSorted<T> FindAll(Func<T, bool> predicate);


    /// <summary>
    /// Create a new indexed sorted collection consisting of the results of
    /// mapping all items of this list.
    /// <exception cref="ArgumentException"/> if the map is not increasing over
    /// the items of this collection (with respect to the two given comparison
    /// relations).
    /// </summary>
    /// <param name="mapper">The delegate definging the map.</param>
    /// <param name="comparer">The comparion relation to use for the result.</param>
    /// <returns>The new sorted collection.</returns>
    IIndexedSorted<V> Map<V>(Func<T, V> mapper, System.Collections.Generic.IComparer<V> comparer);
}