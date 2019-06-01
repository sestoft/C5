namespace C5
{
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
        /// B is containd in A(this), but not vice versa
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
}