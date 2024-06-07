// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// An exception thrown by operations on a list that expects an argument
/// that is a view on the same underlying list.
/// </summary>
public class IncompatibleViewException : Exception
{
    /// <summary>
    /// Create a simple exception with no further explanation.
    /// </summary>
    public IncompatibleViewException() : base() { }
    /// <summary>
    /// Create the exception with an explanation of the reason.
    /// </summary>
    /// <param name="message"></param>
    public IncompatibleViewException(string message) : base(message) { }
}