// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using System.Collections.Generic;

namespace C5;

/// <summary>
/// A sequenced collection, where indices of items in the order are maintained
/// </summary>
public interface IIndexed<T> : ISequenced<T>, IReadOnlyList<T>
{
    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    Speed IndexingSpeed { get; }

    /// <summary>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <value>The directed collection of items in a specific index interval.</value>
    /// <param name="start">The low index of the interval (inclusive).</param>
    /// <param name="count">The size of the range.</param>
    IDirectedCollectionValue<T> this[int start, int count] { get; }


    /// <summary>
    /// Searches for an item in the list going forwards from the start.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>Index of item from start. A negative number if item not found,
    /// namely the one's complement of the index at which the Add operation would put the item.</returns>
    int IndexOf(T item);


    /// <summary>
    /// Searches for an item in the list going backwards from the end.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>Index of of item from the end. A negative number if item not found,
    /// namely the two-complement of the index at which the Add operation would put the item.</returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the index of the first one.
    /// </summary>
    /// <param name="predicate">A delegate
    /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    int FindIndex(Func<T, bool> predicate);

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the index of the last one.
    /// </summary>
    /// <param name="predicate">A delegate
    /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    int FindLastIndex(Func<T, bool> predicate);


    /// <summary>
    /// Remove the item at a specific position of the list.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"> if <code>index</code> is negative or
    /// &gt;= the size of the collection.</exception>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>The removed item.</returns>
    T RemoveAt(int index);


    /// <summary>
    /// Remove all items in an index interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> if start or count
    /// is negative or start+count &gt; the size of the collection.</exception>
    /// <param name="start">The index of the first item to remove.</param>
    /// <param name="count">The number of items to remove.</param>
    void RemoveInterval(int start, int count);
}