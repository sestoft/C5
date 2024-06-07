// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// The interface describing the operations of a LIFO stack data structure.
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public interface IStack<T> : IDirectedCollectionValue<T>
{
    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    bool AllowsDuplicates { get; }
    /// <summary>
    /// Get the <code>index</code>'th element of the stack.  The bottom of the stack has index 0.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; }
    /// <summary>
    /// Push an item to the top of the stack.
    /// </summary>
    /// <param name="item">The item</param>
    void Push(T item);
    /// <summary>
    /// Pop the item at the top of the stack from the stack.
    /// </summary>
    /// <returns>The popped item.</returns>
    T Pop();
}