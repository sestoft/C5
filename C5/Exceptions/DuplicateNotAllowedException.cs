using System;

namespace C5
{
    /// <summary>
    /// An exception thrown when an operation attempts to create a duplicate in a collection with set semantics 
    /// (<see cref="P:C5.IExtensible`1.AllowsDuplicates"/> is false) or attempts to create a duplicate key in a dictionary.
    /// <para>With collections this can only happen with Insert operations on lists, since the Add operations will
    /// not try to create duplictes and either ignore the failure or report it in a bool return value.
    /// </para>
    /// <para>With dictionaries this can happen with the <see cref="M:C5.IDictionary`2.Add(`0,`1)"/> metod.</para>
    /// </summary>
    public class DuplicateNotAllowedException : Exception
    {
        /// <summary>
        /// Create a simple exception with no further explanation.
        /// </summary>
        public DuplicateNotAllowedException() : base() { }
        /// <summary>
        /// Create the exception with an explanation of the reason.
        /// </summary>
        /// <param name="message"></param>
        public DuplicateNotAllowedException(string message) : base(message) { }
    }
}