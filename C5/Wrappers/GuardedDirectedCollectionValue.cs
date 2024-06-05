using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a directed collection
    ///
    /// <i>This is mainly interesting as a base of other guard classes</i>
    /// </summary>
    public class GuardedDirectedCollectionValue<T> : GuardedCollectionValue<T>, IDirectedCollectionValue<T>
    {
        #region Fields

        private readonly IDirectedCollectionValue<T> directedcollection;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a directed collection in a read-only wrapper
        /// </summary>
        /// <param name="directedcollection">the collection to wrap</param>
        public GuardedDirectedCollectionValue(IDirectedCollectionValue<T> directedcollection)
            :
            base(directedcollection)
        { this.directedcollection = directedcollection; }

        #endregion

        #region IDirectedCollection<T> Members

        /// <summary>
        /// Get a collection that enumerates the wrapped collection in the opposite direction
        /// </summary>
        /// <returns>The mirrored collection</returns>
        public virtual IDirectedCollectionValue<T> Backwards()
        { return new GuardedDirectedCollectionValue<T>(directedcollection.Backwards()); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool FindLast(Func<T, bool> predicate, out T item) { return directedcollection.FindLast(predicate, out item); }

        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
        { return Backwards(); }


        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public Direction Direction => directedcollection.Direction;

        #endregion
    }
}