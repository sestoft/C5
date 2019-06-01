using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FixedSizeCollectionException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public FixedSizeCollectionException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public FixedSizeCollectionException(string message) : base(message) { }
    }
}