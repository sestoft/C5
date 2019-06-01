using System;

namespace C5
{
    /// <summary>
    /// An exception thrown by enumerators, range views etc. when accessed after 
    /// the underlying collection has been modified.
    /// </summary>
    [Serializable]
    public class CollectionModifiedException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public CollectionModifiedException() : base() { }

        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public CollectionModifiedException(string message) : base(message) { }
    }
}