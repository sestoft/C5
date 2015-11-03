using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C5
{
    /// <summary>
    /// A base class for implementing an IEnumerable&lt;T&gt;
    /// </summary>

    public abstract class EnumerableBase<T> : IEnumerable<T>
    {
        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public abstract IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Count the number of items in an enumerable by enumeration
        /// </summary>
        /// <param name="items">The enumerable to count</param>
        /// <returns>The size of the enumerable</returns>
        protected static int countItems(IEnumerable<T> items)
        {
            ICollectionValue<T> jtems = items as ICollectionValue<T>;

            if (jtems != null)
                return jtems.Count;

            int count = 0;

            using (IEnumerator<T> e = items.GetEnumerator())
                while (e.MoveNext()) count++;

            return count;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
