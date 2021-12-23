using System;

namespace C5
{
    /// <summary>
    /// An exception thrown by an update operation on a Read-Only collection or dictionary.
    /// <para>This exception will be thrown unconditionally when an update operation 
    /// (method or set property) is called. No check is made to see if the update operation, 
    /// if allowed, would actually change the collection. </para>
    /// </summary>
    public class ReadOnlyCollectionException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public ReadOnlyCollectionException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public ReadOnlyCollectionException(string message) : base(message) { }
    }
}