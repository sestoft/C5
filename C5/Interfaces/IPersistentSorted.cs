// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

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