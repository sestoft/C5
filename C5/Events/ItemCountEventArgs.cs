using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ItemCountEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="item"></param>
        public ItemCountEventArgs(T item, int count) { Item = item; Count = count; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(ItemCountEventArgs {0} '{1}')", Count, Item);
        }
    }
}