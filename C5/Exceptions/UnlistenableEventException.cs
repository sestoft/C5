// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
///
/// </summary>
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