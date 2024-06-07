// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// Characterize the mutual position of some view B (other) relative to view A (this)
/// </summary>
internal enum MutualViewPosition
{
    /// <summary>
    /// B contains A(this)
    /// </summary>
    Contains,
    /// <summary>
    /// B is contained in A(this), but not vice versa
    /// </summary>
    ContainedIn,
    /// <summary>
    /// A and B does not overlap
    /// </summary>
    NonOverlapping,
    /// <summary>
    /// A and B overlap, but neither is contained in the other
    /// </summary>
    Overlapping
}