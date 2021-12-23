using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    public class ClearedRangeEventArgs : ClearedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int? Start { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="full"></param>
        /// <param name="count"></param>
        /// <param name="start"></param>
        public ClearedRangeEventArgs(bool full, int count, int? start) : base(full, count) { Start = start; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(ClearedRangeEventArgs {0} {1} {2})", Count, Full, Start);
        }
    }
}