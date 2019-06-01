using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InvalidPriorityQueueHandleException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public InvalidPriorityQueueHandleException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public InvalidPriorityQueueHandleException(string message) : base(message) { }
    }
}