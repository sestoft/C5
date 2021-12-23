// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;
namespace C5
{
    /// <summary>
    /// A base class for implementing an IEnumerable&lt;T&gt;
    /// </summary>
    public abstract class EnumerableBase<T> : SCG.IEnumerable<T>
    {
        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public abstract SCG.IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Count the number of items in an enumerable by enumeration
        /// </summary>
        /// <param name="items">The enumerable to count</param>
        /// <returns>The size of the enumerable</returns>
        protected static int CountItems(SCG.IEnumerable<T> items)
        {
            // ICollectionValue<T> jtems = items as ICollectionValue<T>;

            if (items is ICollectionValue<T> jtems)
            {
                return jtems.Count;
            }

            int count = 0;

            using (SCG.IEnumerator<T> e = items.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    count++;
                }
            }

            return count;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
