using System;

namespace C5
{
    /// <summary>
    /// An exception thrown by an operation that need to construct a natural
    /// comparer for a type.
    /// </summary>
    [Serializable]
    public class NotComparableException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public NotComparableException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public NotComparableException(string message) : base(message) { }
    }
}