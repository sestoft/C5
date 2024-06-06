// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// A read-only wrapper for a sequenced collection
///
/// <i>This is mainly interesting as a base of other guard classes</i>
/// </summary>
public class GuardedSequenced<T> : GuardedCollection<T>, ISequenced<T>
{
    #region Fields

    private readonly ISequenced<T> sequenced;

    #endregion

    #region Constructor

    /// <summary>
    /// Wrap a sequenced collection in a read-only wrapper
    /// </summary>
    /// <param name="sorted"></param>
    public GuardedSequenced(ISequenced<T> sorted) : base(sorted) { sequenced = sorted; }

    #endregion

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the index of the first one.
    /// </summary>
    /// <param name="predicate">A delegate
    /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    public int FindIndex(Func<T, bool> predicate)
    {
        if (sequenced is IIndexed<T> indexed)
        {
            return indexed.FindIndex(predicate);
        }

        int index = 0;
        foreach (T item in this)
        {
            if (predicate(item))
            {
                return index;
            }

            index++;
        }
        return -1;
    }

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the index of the last one.
    /// </summary>
    /// <param name="predicate">A delegate
    /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    public int FindLastIndex(Func<T, bool> predicate)
    {
        if (sequenced is IIndexed<T> indexed)
        {
            return indexed.FindLastIndex(predicate);
        }

        int index = Count - 1;
        foreach (T item in Backwards())
        {
            if (predicate(item))
            {
                return index;
            }

            index--;
        }
        return -1;
    }



    #region ISequenced<T> Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public int GetSequencedHashCode()
    { return sequenced.GetSequencedHashCode(); }

    /// <summary>
    ///
    /// </summary>
    /// <param name="that"></param>
    /// <returns></returns>
    public bool SequencedEquals(ISequenced<T> that)
    { return sequenced.SequencedEquals(that); }

    #endregion

    #region IDirectedCollection<T> Members

    /// <summary>
    /// Get a collection that enumerates the wrapped collection in the opposite direction
    /// </summary>
    /// <returns>The mirrored collection</returns>
    public virtual IDirectedCollectionValue<T> Backwards()
    { return new GuardedDirectedCollectionValue<T>(sequenced.Backwards()); }

    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool FindLast(Func<T, bool> predicate, out T item) { return sequenced.FindLast(predicate, out item); }

    #endregion

    #region IDirectedEnumerable<T> Members

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    { return Backwards(); }



    /// <summary>
    /// <code>Forwards</code> if same, else <code>Backwards</code>
    /// </summary>
    /// <value>The enumeration direction relative to the original collection.</value>
    public Direction Direction => Direction.Forwards;

    #endregion
}