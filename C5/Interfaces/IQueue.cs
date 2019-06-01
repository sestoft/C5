namespace C5
{
    /// <summary>
    /// The interface describing the operations of a FIFO queue data structure.
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    public interface IQueue<T> : IDirectedCollectionValue<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        bool AllowsDuplicates { get; }
        /// <summary>
        /// Get the <code>index</code>'th element of the queue.  The front of the queue has index 0.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
        /// <summary>
        /// Enqueue an item at the back of the queue. 
        /// </summary>
        /// <param name="item">The item</param>
        void Enqueue(T item);
        /// <summary>
        /// Dequeue an item from the front of the queue.
        /// </summary>
        /// <returns>The item</returns>
        T Dequeue();
    }
}