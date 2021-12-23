// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5
{
    /// <summary>
    /// Class containing debugging symbols - to eliminate preprocessor directives
    /// </summary>
    internal class Debug
    {
        /// <summary>
        /// Flag used to test hashing. Set to true when unit testing hash functions.
        /// </summary>
        internal static bool UseDeterministicHashing { get; set; }
    }
}
