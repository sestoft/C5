using System;

namespace C5
{
    /// <summary>
    /// An exception thrown by an operation on a list (<see cref="T:C5.IList`1"/>)
    /// that only makes sense for a view, not for an underlying list.
    /// </summary>
    [Serializable]
    public class NotAViewException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public NotAViewException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public NotAViewException(string message) : base(message) { }
    }
}