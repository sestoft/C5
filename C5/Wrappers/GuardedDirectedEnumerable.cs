using System;

namespace C5
{
    /// <summary>
    /// A read-only wrapper for a generic directed enumerable
    ///
    /// <i>This is mainly interesting as a base of other guard classes</i>
    /// </summary>
    public class GuardedDirectedEnumerable<T> : GuardedEnumerable<T>, IDirectedEnumerable<T>
    {
        #region Fields

        private readonly IDirectedEnumerable<T> directedenumerable;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap a directed enumerable in a read-only wrapper
        /// </summary>
        /// <param name="directedenumerable">the collection to wrap</param>
        public GuardedDirectedEnumerable(IDirectedEnumerable<T> directedenumerable)
            : base(directedenumerable)
        { this.directedenumerable = directedenumerable; }

        #endregion

        #region IDirectedEnumerable<T> Members

        /// <summary>
        /// Get a enumerable that enumerates the wrapped collection in the opposite direction
        /// </summary>
        /// <returns>The mirrored enumerable</returns>
        public IDirectedEnumerable<T> Backwards()
        { return new GuardedDirectedEnumerable<T>(directedenumerable.Backwards()); }


        /// <summary>
        /// <code>Forwards</code> if same, else <code>Backwards</code>
        /// </summary>
        /// <value>The enumeration direction relative to the original collection.</value>
        public Direction Direction => directedenumerable.Direction;

        #endregion
    }
}