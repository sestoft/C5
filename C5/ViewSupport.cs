// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Characterize the mutual position of some view B (other) relative to view A (this)
    /// </summary>
    enum MutualViewPosition
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

    #region View List Nested class
    /// <summary>
    /// This class is shared between the linked list and array list implementations.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    class WeakViewList<V> where V : class
    {
        Node start;

        [Serializable]
        internal class Node
        {
            internal WeakReference weakview; internal Node prev, next;
            internal Node(V view) { weakview = new WeakReference(view); }
        }
        internal Node Add(V view)
        {
            Node newNode = new Node(view);
            if (start != null) { start.prev = newNode; newNode.next = start; }
            start = newNode;
            return newNode;
        }
        internal void Remove(Node n)
        {
            if (n == start) { start = start.next; if (start != null) start.prev = null; }
            else { n.prev.next = n.next; if (n.next != null) n.next.prev = n.prev; }
        }
        /// <summary>
        /// Note that it is safe to call views.Remove(view.myWeakReference) if view
        /// is the currently yielded object
        /// </summary>
        /// <returns></returns>
        public SCG.IEnumerator<V> GetEnumerator()
        {
            Node n = start;
            while (n != null)
            {
                //V view = n.weakview.Target as V; //This provokes a bug in the beta1 verifyer
                object o = n.weakview.Target;
                V view = o is V ? (V)o : null;
                if (view == null)
                    Remove(n);
                else
                    yield return view;
                n = n.next;
            }
        }
    }

    #endregion
}