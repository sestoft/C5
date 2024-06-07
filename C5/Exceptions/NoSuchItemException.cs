// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// An exception thrown by a lookup or lookup with update operation that does not
/// find the lookup item and has no other means to communicate failure.
/// <para>The typical scenario is a lookup by key in a dictionary with an indexer,
/// see e.g. <see cref="P:C5.IDictionary`2.Item(`0)"/></para>
/// </summary>
public class NoSuchItemException : Exception
{
    /// <summary>
    /// Create a simple exception with no further explanation.
    /// </summary>
    public NoSuchItemException() : base() { }
    /// <summary>
    /// Create the exception with an explanation of the reason.
    /// </summary>
    /// <param name="message"></param>
    public NoSuchItemException(string message) : base(message) { }
}