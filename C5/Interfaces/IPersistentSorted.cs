using System;

namespace C5
{
    /// <summary>
    /// The type of a sorted collection with persistence
    /// </summary>
    public interface IPersistentSorted<T> : ISorted<T>, IDisposable
    {
        /// <summary>
        /// Make a (read-only) snap shot of this collection.
        /// </summary>
        /// <returns>The snap shot.</returns>
        ISorted<T> Snapshot();
    }
}