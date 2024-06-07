// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// Base class (abstract) for sequenced collection implementations.
/// </summary>
public abstract class SequencedBase<T> : DirectedCollectionBase<T>, IDirectedCollectionValue<T>
{
    #region Fields

    private int iSequencedHashCode, iSequencedHashCodeStamp = -1;

    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <param name="itemEqualityComparer"></param>
    protected SequencedBase(System.Collections.Generic.IEqualityComparer<T> itemEqualityComparer) : base(itemEqualityComparer) { }

    #region Util

    //TODO: make random for release
    private const int HashFactor = 31;

    /// <summary>
    /// Compute the unsequenced hash code of a collection
    /// </summary>
    /// <param name="items">The collection to compute hash code for</param>
    /// <param name="itemEqualityComparer">The item equalitySCG.Comparer</param>
    /// <returns>The hash code</returns>
    public static int ComputeHashCode(ISequenced<T> items, System.Collections.Generic.IEqualityComparer<T> itemEqualityComparer)
    {
        // NOTE: It must be possible to devise a much stronger combined hash code,
        // but unfortunately, it has to be universal. OR we could use a (strong)
        // family and initialize its parameter randomly at load time of this class!
        // (We would not want to have yet a flag to check for invalidation?!)
        // NB: the current hash code has the very bad property that items with hash code 0
        // is ignored.
        int iIndexedHashCode = 0;

        foreach (T item in items)
        {
            iIndexedHashCode = iIndexedHashCode * HashFactor + itemEqualityComparer.GetHashCode(item);
        }

        return iIndexedHashCode;
    }

    /// <summary>
    /// Examine if tit and tat are equal as sequenced collections
    /// using the specified item equalityComparer (assumed compatible with the two collections).
    /// </summary>
    /// <param name="collection1">The first collection</param>
    /// <param name="collection2">The second collection</param>
    /// <param name="itemEqualityComparer">The item equalityComparer to use for comparison</param>
    /// <returns>True if equal</returns>
    public static bool StaticEquals(ISequenced<T> collection1, ISequenced<T> collection2, System.Collections.Generic.IEqualityComparer<T> itemEqualityComparer)
    {
        if (ReferenceEquals(collection1, collection2))
        {
            return true;
        }

        if (collection1.Count != collection2.Count)
        {
            return false;
        }

        //This way we might run through both enumerations twice, but
        //probably not (if the hash codes are good)
        if (collection1.GetSequencedHashCode() != collection2.GetSequencedHashCode())
        {
            return false;
        }

        using System.Collections.Generic.IEnumerator<T> dat = collection2.GetEnumerator(), dit = collection1.GetEnumerator();

        while (dit.MoveNext())
        {
            dat.MoveNext();
            if (!itemEqualityComparer.Equals(dit.Current, dat.Current))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Get the sequenced collection hash code of this collection: from the cached
    /// value if present and up to date, else (re)compute.
    /// </summary>
    /// <returns>The hash code</returns>
    public virtual int GetSequencedHashCode()
    {
        if (iSequencedHashCodeStamp == stamp)
        {
            return iSequencedHashCode;
        }

        iSequencedHashCode = ComputeHashCode((ISequenced<T>)this, itemEqualityComparer);
        iSequencedHashCodeStamp = stamp;
        return iSequencedHashCode;
    }


    /// <summary>
    /// Check if the contents of that is equal to the contents of this
    /// in the sequenced sense. Using the item equalityComparer of this collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare to.</param>
    /// <returns>True if  equal</returns>
    public virtual bool SequencedEquals(ISequenced<T> otherCollection)
    {
        return StaticEquals((ISequenced<T>)this, otherCollection, itemEqualityComparer);
    }


    #endregion

    /// <summary>
    /// Create an enumerator for this collection.
    /// </summary>
    /// <returns>The enumerator</returns>
    public abstract override System.Collections.Generic.IEnumerator<T> GetEnumerator();

    /// <summary>
    /// <code>Forwards</code> if same, else <code>Backwards</code>
    /// </summary>
    /// <value>The enumeration direction relative to the original collection.</value>
    public override Direction Direction => Direction.Forwards;

    /// <summary>
    /// Check if there exists an item  that satisfies a
    /// specific predicate in this collection and return the index of the first one.
    /// </summary>
    /// <param name="predicate">A delegate defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    public int FindIndex(Func<T, bool> predicate)
    {
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
    /// <param name="predicate">A delegate defining the predicate</param>
    /// <returns>the index, if found, a negative value else</returns>
    public int FindLastIndex(Func<T, bool> predicate)
    {
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

}