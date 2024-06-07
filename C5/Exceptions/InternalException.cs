// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// An exception to throw from library code when an internal inconsistency is encountered.
/// </summary>
public class InternalException : Exception
{
    internal InternalException(string message) : base(message) { }
}