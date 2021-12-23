using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a generic indexable queue (allows indexing).
    /// 
    /// <para>Suitable for wrapping a <see cref="T:C5.CircularQueue`1"/></para>
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class GuardedQueue<T> : GuardedDirectedCollectionValue<T>, IQueue<T>
    {
        #region Fields

        private readonly IQueue<T> queue;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a queue in a read-only wrapper
        /// </summary>
        /// <param name="queue">The queue</param>
        public GuardedQueue(IQueue<T> queue) : base(queue) { this.queue = queue; }

        #endregion

        #region IQueue<T> Members
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public bool AllowsDuplicates => queue.AllowsDuplicates;

        /// <summary>
        /// Index into the wrapped queue
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i] => queue[i];

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public void Enqueue(T item)
        { throw new ReadOnlyCollectionException("Queue cannot be modified through this guard object"); }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
        /// <returns>-</returns>
        public T Dequeue()
        { throw new ReadOnlyCollectionException("Queue cannot be modified through this guard object"); }

        #endregion
    }
}