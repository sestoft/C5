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
