using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DirectedCollectionValueBase<T> : CollectionValueBase<T>, IDirectedCollectionValue<T>
    {
        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public virtual EnumerationDirection Direction => EnumerationDirection.Forwards;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDirectedCollectionValue<T> Backwards();

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the first one in enumeration order.
        /// </summary>
        /// <param name="predicate">A delegate 
        /// (<see cref="T:Func`2"/> with <code>R == bool</code>) defining the predicate</param>
        /// <param name="item"></param>
        /// <returns>True is such an item exists</returns>
        public virtual bool FindLast(Func<T, bool> predicate, out T item)
        {
            foreach (T jtem in Backwards())
            {
                if (predicate(jtem))
                {
                    item = jtem;
                    return true;
                }
            }

            item = default;
            return false;
        }
    }
}