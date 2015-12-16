namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// Enables System.Collections.Generic.IReadOnlyCollection to be used in .NET 4.5 projects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollection<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
    }
}
