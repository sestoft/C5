using System;

namespace C5
{
    /// <summary>
    /// An exception thrown when trying to access a view (a list view on a <see cref="T:C5.IList`1"/> or 
    /// a snapshot on a <see cref="T:C5.IPersistentSorted`1"/>)
    /// that has been invalidated by some earlier operation.
    /// <para>
    /// The typical scenario is a view on a list that hash been invalidated by a call to 
    /// Sort, Reverse or Shuffle on some other, overlapping view or the whole list.
    /// </para>
    /// </summary>
    [Serializable]
    public class ViewDisposedException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public ViewDisposedException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public ViewDisposedException(string message) : base(message) { }
    }
}