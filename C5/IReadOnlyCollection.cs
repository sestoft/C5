using System.Collections;
using System.Collections.Generic;

#if NET35 || NET40 || PROFILE328

namespace C5
{
    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// Enables System.Collections.Generic.IReadOnlyCollection to be used in .NET 4.5 projects
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if NET35
    public interface IReadOnlyCollection<T> : IEnumerable<T>, IEnumerable
#else
    public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
#endif
    {
#if NET35
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
#endif
    }
}

#endif
