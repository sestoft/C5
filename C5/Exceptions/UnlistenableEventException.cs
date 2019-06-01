using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnlistenableEventException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public UnlistenableEventException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public UnlistenableEventException(string message) : base(message) { }
    }
}