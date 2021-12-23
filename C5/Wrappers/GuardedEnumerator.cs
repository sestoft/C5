// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A read-only wrapper class for a generic enumerator
    /// </summary>
    public class GuardedEnumerator<T> : IEnumerator<T>
    {
        #region Fields

        private readonly IEnumerator<T> enumerator;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a wrapper around a generic enumerator
        /// </summary>
        /// <param name="enumerator">The enumerator to wrap</param>
        public GuardedEnumerator(IEnumerator<T> enumerator)
        { this.enumerator = enumerator; }

        #endregion

        #region IEnumerator<T> Members

        /// <summary>
        /// Move wrapped enumerator to next item, or the first item if
        /// this is the first call to MoveNext.
        /// </summary>
        /// <returns>True if enumerator is valid now</returns>
        public bool MoveNext() { return enumerator.MoveNext(); }


        /// <summary>
        /// Undefined if enumerator is not valid (MoveNext hash been called returning true)
        /// </summary>
        /// <value>The current item of the wrapped enumerator.</value>
        public T Current => enumerator.Current;

        #endregion

        #region IDisposable Members

        //TODO: consider possible danger of calling through to Dispose.
        /// <summary>
        /// Dispose wrapped enumerator.
        /// </summary>
        public void Dispose() { enumerator.Dispose(); }

        #endregion


        #region IEnumerator Members

        object System.Collections.IEnumerator.Current => enumerator.Current!;

        void System.Collections.IEnumerator.Reset()
        {
            enumerator.Reset();
        }

        #endregion
    }
}