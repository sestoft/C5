namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index. 
    /// Enables System.Collections.Generic.IReadOnlyList to be used in .NET 4.5 projects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
    }
}
