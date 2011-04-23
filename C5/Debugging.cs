namespace C5
{
    /// <summary>
    /// Class containing debugging symbols - to eliminate preprocessor directives
    /// </summary>
    internal class Debugging
    {
        /// <summary>
        /// Use deterministic hashing in hash sets/bags.
        /// </summary>
        internal static bool UseDeterministicHashing { get; set; }
    }
}
