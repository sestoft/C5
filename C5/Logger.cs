// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5
{
    /// <summary>
    /// Logging module
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <example>The following is an example of assigning a observer to the logging module:
        ///   <code>
        ///     Logger.Log = x => Console.WriteLine(x);
        ///   </code>
        /// </example>
        /// <remarks>
        /// If Log is not set it will return a dummy action
        /// <c>_ => {})</c>
        /// eliminating the need for null-reference checks.
        /// </remarks>
        /// <value>
        /// The log.
        /// </value>
        public static Action<string> Log { get; set; } = _ => {};
    }
}