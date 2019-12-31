// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// An implementation of Red-Black trees as an indexed, sorted collection with set semantics,
    /// cf. <a href="litterature.htm#CLRS">CLRS</a>. <see cref="T:C5.TreeBag`1"/> for a version 
    /// with bag semantics. <see cref="T:C5.TreeDictionary`2"/> for a sorted dictionary 
    /// based on this tree implementation.
    /// <i>
    /// The comparer (sorting order) may be either natural, because the item type is comparable 
    /// (generic: <see cref="T:C5.IComparable`1"/> or non-generic: System.IComparable) or it can
    /// be external and supplied by the user in the constructor.</i>
    ///
    /// <i>TODO: describe performance here</i>
    /// <i>TODO: discuss persistence and its useful usage modes. Warn about the space
    /// leak possible with other usage modes.</i>
    /// </summary>
    [Serializable]
    public class TreeSet<T> : SequencedBase<T>, IIndexedSorted<T>, IPersistentSorted<T>, SCG.ISet<T>
    {
        #region Fields

        private SCG.IComparer<T>? comparer;
        private Node? root;

        //TODO: wonder if we should remove that
        private int blackdepth = 0;

        //We double these stacks for the iterative add and remove on demand
        //TODO: refactor dirs[] into bool fields on Node (?)
        private int[]? dirs = new int[2];

        private Node?[]? path = new Node[2];

        //TODO: refactor into separate class
        private bool isSnapShot = false;
        private int generation;
        private bool isValid = true;
        private SnapRef? snapList;

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public override EventType ListenableEvents => EventType.Basic;

        #endregion
        #region Util

        /// <summary>
        /// Fetch the left child of n taking node-copying persistence into
        /// account if relevant. 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private Node Left(Node n)
        {
            if (isSnapShot)
            {

                if (n.lastgeneration >= generation && n.leftnode)
                {
                    return n.oldref;
                }
            }
            return n.left!;
        }


        private Node Right(Node n)
        {
            if (isSnapShot)
            {

                if (n.lastgeneration >= generation && !n.leftnode)
                {
                    return n.oldref;
                }
            }
            return n.right!;
        }


        //This method should be called by methods that use the internal 
        //traversal stack, unless certain that there is room enough
        private void StackCheck()
        {
            while (dirs!.Length < 2 * blackdepth)
            {
                dirs = new int[2 * dirs.Length];
                path = new Node[2 * dirs.Length];
            }
        }

        #endregion

        #region Node nested class


        /// <summary>
        /// The type of node in a Red-Black binary tree
        /// </summary>
        [Serializable]
        private class Node
        {
            public bool red = true;

            public T item;

            public Node? left;

            public Node? right;

            public int size = 1;

            //TODO: move everything into (separate) Extra
            public int generation;

            public int lastgeneration = -1;

            public Node? oldref;

            public bool leftnode;

            /// <summary>
            /// Update a child pointer
            /// </summary>
            /// <param name="cursor"></param>
            /// <param name="leftnode"></param>
            /// <param name="child"></param>
            /// <param name="maxsnapid"></param>
            /// <param name="generation"></param>
            /// <returns>True if node was *copied*</returns>
            internal static bool Update(ref Node cursor, bool leftnode, Node child, int maxsnapid, int generation)
            {
                Node oldref = (leftnode ? cursor.left : cursor.right)!;

                if (child == oldref)
                {
                    return false;
                }

                bool retval = false;

                if (cursor.generation <= maxsnapid)
                {
                    if (cursor.lastgeneration == -1)
                    {
                        cursor.leftnode = leftnode;
                        cursor.lastgeneration = maxsnapid;
                        cursor.oldref = oldref!;
                    }
                    else if (cursor.leftnode != leftnode || cursor.lastgeneration < maxsnapid)
                    {
                        CopyNode(ref cursor, maxsnapid, generation);
                        retval = true;
                    }
                }

                if (leftnode)
                {
                    cursor.left = child;
                }
                else
                {
                    cursor.right = child;
                }

                return retval;
            }


            //If cursor.extra.lastgeneration==maxsnapid, the extra pointer will 
            //always be used in the old copy of cursor. Therefore, after 
            //making the clone, we should update the old copy by restoring
            //the child pointer and setting extra to null.
            //OTOH then we cannot clean up unused Extra objects unless we link
            //them together in a doubly linked list.
            public static bool CopyNode(ref Node cursor, int maxsnapid, int generation)
            {
                if (cursor.generation <= maxsnapid)
                {
                    cursor = (Node)(cursor.MemberwiseClone());
                    cursor.generation = generation;
                    cursor.lastgeneration = -1;
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a red-black tree collection with natural comparer and item equalityComparer.
        /// We assume that if <code>T</code> is comparable, its default equalityComparer 
        /// will be compatible with the comparer.
        /// </summary>
        /// <exception cref="NotComparableException">If <code>T</code> is not comparable.
        /// </exception>
        public TreeSet() : this(SCG.Comparer<T>.Default, EqualityComparer<T>.Default) { }


        /// <summary>
        /// Create a red-black tree collection with an external comparer. 
        /// <para>The itemequalityComparer will be a compatible 
        /// <see cref="T:C5.ComparerZeroHashCodeEqualityComparer`1"/> since the 
        /// default equalityComparer for T (<see cref="P:C5.EqualityComparer`1.Default"/>)
        /// is unlikely to be compatible with the external comparer. This makes the
        /// tree inadequate for use as item in a collection of unsequenced or sequenced sets or bags
        /// (<see cref="T:C5.ICollection`1"/> and <see cref="T:C5.ISequenced`1"/>)
        /// </para>
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        public TreeSet(SCG.IComparer<T> comparer) : this(comparer, new ComparerZeroHashCodeEqualityComparer<T>(comparer)) { }

        /// <summary>
        /// Create a red-black tree collection with an external comparer and an external
        /// item equalityComparer, assumed consistent.
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        /// <param name="equalityComparer">The external item equalitySCG.Comparer</param>
        public TreeSet(SCG.IComparer<T> comparer, SCG.IEqualityComparer<T> equalityComparer)
            : base(equalityComparer)
        {
            this.comparer = comparer ?? throw new NullReferenceException("Item comparer cannot be null");
        }

        #endregion

        #region TreeSet.Enumerator nested class

        /// <summary>
        /// An enumerator for a red-black tree collection. Based on an explicit stack
        /// of subtrees waiting to be enumerated. Currently only used for the tree set 
        /// enumerators (tree bag enumerators use an iterator block based enumerator).
        /// </summary>
        [Serializable]
        internal class Enumerator : SCG.IEnumerator<T>
        {
            #region Private Fields
            private readonly TreeSet<T> tree;
            private bool valid = false;
            private readonly int stamp;
            private T current;
            private Node? cursor;
            private Node[]? path; // stack of nodes

            private int level = 0;
            #endregion
            /// <summary>
            /// Create a tree enumerator
            /// </summary>
            /// <param name="tree">The red-black tree to enumerate</param>
            public Enumerator(TreeSet<T> tree)
            {
                this.tree = tree;
                stamp = tree.stamp;
                path = new Node[2 * tree.blackdepth];
                cursor = new Node
                {
                    right = tree.root
                };
            }


            /// <summary>
            /// Undefined if enumerator is not valid (MoveNext hash been called returning true)
            /// </summary>
            /// <value>The current item of the enumerator.</value>
            public T Current
            {
                get
                {
                    if (valid)
                    {
                        return current;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }


            //Maintain a stack of nodes that are roots of
            //subtrees not completely exported yet. Invariant:
            //The stack nodes together with their right subtrees
            //consists of exactly the items we have not passed
            //yet (the top of the stack holds current item).
            /// <summary>
            /// Move enumerator to next item in tree, or the first item if
            /// this is the first call to MoveNext. 
            /// <exception cref="CollectionModifiedException"/> if underlying tree was modified.
            /// </summary>
            /// <returns>True if enumerator is valid now</returns>
            public bool MoveNext()
            {
                tree.ModifyCheck(stamp);
                if (cursor!.right != null)
                {
                    path![level] = cursor = cursor.right;
                    while (cursor.left != null)
                    {
                        path[++level] = cursor = cursor.left;
                    }
                }
                else if (level == 0)
                {
                    return valid = false;
                }
                else
                {
                    cursor = path![--level];
                }

                current = cursor.item;
                return valid = true;
            }


            #region IDisposable Members for Enumerator

            private bool disposed;


            /// <summary>
            /// Call Dispose(true) and then suppress finalization of this enumerator.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }


            /// <summary>
            /// Remove the internal data (notably the stack array).
            /// </summary>
            /// <param name="disposing">True if called from Dispose(),
            /// false if called from the finalizer</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                    }

                    current = default;
                    cursor = null;
                    path = null;
                    disposed = true;
                }
            }


            /// <summary>
            /// Finalizer for enumerator
            /// </summary>
            ~Enumerator()
            {
                Dispose(false);
            }
            #endregion


            #region IEnumerator Members

            object System.Collections.IEnumerator.Current => Current!;

            bool System.Collections.IEnumerator.MoveNext()
            {
                return MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// An enumerator for a snapshot of a node copy persistent red-black tree
        /// collection.
        /// </summary>
        [Serializable]
        internal class SnapEnumerator : SCG.IEnumerator<T>
        {
            #region Private Fields
            private TreeSet<T>? tree;
            private bool valid = false;
            private readonly int stamp;
            private T current;
            private Node? cursor;
            private Node[]? path; // stack of nodes

            private int level;
            #endregion

            /// <summary>
            /// Create an enumerator for a snapshot of a node copy persistent red-black tree
            /// collection
            /// </summary>
            /// <param name="tree">The snapshot</param>
            public SnapEnumerator(TreeSet<T> tree)
            {
                this.tree = tree;
                stamp = tree.stamp;
                path = new Node[2 * tree.blackdepth];
                cursor = new Node
                {
                    right = tree.root
                };
            }


            #region SCG.IEnumerator<T> Members

            /// <summary>
            /// Move enumerator to next item in tree, or the first item if
            /// this is the first call to MoveNext. 
            /// <exception cref="CollectionModifiedException"/> if underlying tree was modified.
            /// </summary>
            /// <returns>True if enumerator is valid now</returns>
            public bool MoveNext()
            {
                tree!.ModifyCheck(stamp);//???


                Node? next = tree.Right(cursor!);

                if (next != null)
                {
                    path![level] = cursor = next;
                    next = tree.Left(cursor);
                    while (next != null)
                    {
                        path[++level] = cursor = next;
                        next = tree.Left(cursor);
                    }
                }
                else if (level == 0)
                {
                    return valid = false;
                }
                else
                {
                    cursor = path![--level];
                }

                current = cursor.item;
                return valid = true;
            }


            /// <summary>
            /// Undefined if enumerator is not valid (MoveNext hash been called returning true)
            /// </summary>
            /// <value>The current value of the enumerator.</value>
            public T Current
            {
                get
                {
                    if (valid)
                    {
                        return current;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            #endregion

            #region IDisposable Members

            void System.IDisposable.Dispose()
            {
                tree = null;
                valid = false;
                current = default;
                cursor = null;
                path = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current => Current!;

            bool System.Collections.IEnumerator.MoveNext()
            {
                return MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region ISet<T> Members

        /// <summary>
        /// Modifies the current <see cref="TreeSet{T}"/> object to contain all elements that are present in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual void UnionWith(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            AddAll(other);
        }

        /// <summary>
        /// Modifies the current <see cref="TreeSet{T}"/> object so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual void IntersectWith(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // intersection of anything with empty set is empty set, so return if count is 0
            if (this.size == 0)
            {
                return;
            }

            // if other is empty, intersection is empty set; remove all elements and we're done
            // can only figure this out if implements ICollection<T>. (IEnumerable<T> has no count)
            if (other is SCG.ICollection<T> otherAsCollection)
            {
                if (otherAsCollection.Count == 0)
                {
                    Clear();
                    return;
                }

                // faster if other is a hashset using same equality comparer; so check 
                // that other is a hashset using the same equality comparer.
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    IntersectWithHashSetWithSameEC(otherAsCollection);
                    return;
                }
            }

            IntersectWithEnumerable(other);
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current <see cref="TreeSet{T}"/> object.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual void ExceptWith(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // this is already the enpty set; return
            if (this.size == 0)
                return;

            // special case if other is this; a set minus itself is the empty set
            if (other == this)
            {
                Clear();
                return;
            }

            // remove every element in other from this
            foreach (T element in other)
            {
                Remove(element);
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current 
        /// <see cref="TreeSet{T}"/> object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual void SymmetricExceptWith(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // if set is empty, then symmetric difference is other
            if (this.size == 0)
            {
                UnionWith(other);
                return;
            }

            // special case this; the symmetric difference of a set with itself is the empty set
            if (other == this)
            {
                Clear();
                return;
            }

            // If other is a HashSet, it has unique elements according to its equality comparer,
            // but if they're using different equality comparers, then assumption of uniqueness
            // will fail. So first check if other is a hashset using the same equality comparer;
            // symmetric except is a lot faster and avoids bit array allocations if we can assume
            // uniqueness
            if (other is SCG.ICollection<T> otherAsCollection && AreEqualityComparersEqual(this, otherAsCollection))
            {
                SymmetricExceptWithUniqueTreeSet(otherAsCollection);
            }
            else
            {
                var temp = new SCG.SortedSet<T>(other, comparer);
                temp.ExceptWith(this);
                ExceptWith(other);
                UnionWith(temp);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="TreeSet{T}"/> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="TreeSet{T}"/> object is a subset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool IsSubsetOf(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (this.size == 0)
            {
                return true;
            }

            // faster if other has unique elements according to this equality comparer; so check 
            // that other is a hashset using the same equality comparer.
            if (other is SCG.ICollection<T> otherAsCollection && AreEqualityComparersEqual(this, otherAsCollection))
            {
                // if this has more elements then it can't be a subset
                if (this.size > otherAsCollection.Count)
                {
                    return false;
                }

                // already checked that we're using same equality comparer. simply check that 
                // each element in this is contained in other.
                return IsSubsetOfTreeSetWithSameEC(otherAsCollection);
            }

            // we just need to return true if the other set
            // contains all of the elements of the this set,
            // but we need to use the comparison rules of the current set.
            this.CheckUniqueAndUnfoundElements(other, false, out int uniqueCount, out int _);
            return uniqueCount == this.size;
        }

        /// <summary>
        /// Determines whether a <see cref="TreeSet{T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="TreeSet{T}"/> object is a superset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool IsSupersetOf(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // try to fall out early based on counts
            if (other is SCG.ICollection<T> otherAsCollection)
            {
                // if other is the empty set then this is a superset
                if (otherAsCollection.Count == 0)
                    return true;

                // try to compare based on counts alone if other is a hashset with
                // same equality comparer
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (otherAsCollection.Count > this.size)
                    {
                        return false;
                    }
                }
            }

            return this.ContainsAll(other);
        }

        /// <summary>
        /// Determines whether a <see cref="TreeSet{T}"/> object is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="TreeSet{T}"/> object is a proper superset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool IsProperSupersetOf(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // the empty set isn't a proper superset of any set.
            if (this.size == 0)
            {
                return false;
            }

            if (other is SCG.ICollection<T> otherAsCollection)
            {
                // if other is the empty set then this is a superset
                if (otherAsCollection.Count == 0)
                    return true; // note that this has at least one element, based on above check

                // faster if other is a hashset with the same equality comparer
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (otherAsCollection.Count >= this.size)
                    {
                        return false;
                    }
                    // now perform element check
                    return ContainsAll(otherAsCollection);
                }
            }

            // couldn't fall out in the above cases; do it the long way
            this.CheckUniqueAndUnfoundElements(other, true, out int uniqueCount, out int unfoundCount);
            return uniqueCount < this.size && unfoundCount == 0;
        }

        /// <summary>
        /// Determines whether a <see cref="TreeSet{T}"/> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="TreeSet{T}"/> object is a proper subset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool IsProperSubsetOf(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));


            if (other is SCG.ICollection<T> otherAsCollection)
            {
                // the empty set is a proper subset of anything but the empty set
                if (this.size == 0)
                    return otherAsCollection.Count > 0;

                // faster if other is a hashset (and we're using same equality comparer)
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (this.size >= otherAsCollection.Count)
                    {
                        return false;
                    }
                    // this has strictly less than number of items in other, so the following
                    // check suffices for proper subset.
                    return IsSubsetOfTreeSetWithSameEC(otherAsCollection);
                }
            }

            this.CheckUniqueAndUnfoundElements(other, false, out int uniqueCount, out int unfoundCount);
            return uniqueCount == this.size && unfoundCount > 0;
        }

        /// <summary>
        /// Determines whether the current <see cref="TreeSet{T}"/> object and a specified collection share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="TreeSet{T}"/> object and other share at least one common element; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool Overlaps(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (this.size != 0)
            {
                foreach (var local in other)
                {
                    if (this.Contains(local))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the current <see cref="TreeSet{T}"/> and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="TreeSet{T}"/>.</param>
        /// <returns><c>true</c> if the current <see cref="TreeSet{T}"/> is equal to other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual bool SetEquals(SCG.IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            // faster if other is a treeset and we're using same equality comparer
            if (other is SCG.ICollection<T> otherAsCollection)
            {
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    // attempt to return early: since both contain unique elements, if they have 
                    // different counts, then they can't be equal
                    if (this.size != otherAsCollection.Count)
                        return false;

                    // already confirmed that the sets have the same number of distinct elements, so if
                    // one is a superset of the other then they must be equal
                    return ContainsAll(otherAsCollection);
                }
                else
                {
                    // if this count is 0 but other contains at least one element, they can't be equal
                    if (this.size == 0 && otherAsCollection.Count > 0)
                        return false;
                }
            }

            this.CheckUniqueAndUnfoundElements(other, true, out int uniqueCount, out int unfoundCount);
            return uniqueCount == this.size && unfoundCount == 0;
        }

        private void CheckUniqueAndUnfoundElements(SCG.IEnumerable<T> other, bool returnIfUnfound, out int uniqueCount, out int unfoundCount)
        {
            // need special case in case this has no elements.
            if (this.size == 0)
            {
                int numElementsInOther = 0;
                foreach (T item in other)
                {
                    numElementsInOther++;
                    // break right away, all we want to know is whether other has 0 or 1 elements
                    break;
                }
                uniqueCount = 0;
                unfoundCount = numElementsInOther;
                return;
            }

            int originalLastIndex = this.size;
            var bitArray = new System.Collections.BitArray(originalLastIndex, false);

            // count of unique items in other found in this
            uniqueCount = 0;
            // count of items in other not found in this
            unfoundCount = 0;

            foreach (var item in other)
            {
                var index = IndexOf(item);
                if (index >= 0)
                {
                    if (!bitArray.Get(index))
                    {
                        // item hasn't been seen yet
                        bitArray.Set(index, true);
                        uniqueCount++;
                    }
                }
                else
                {
                    unfoundCount++;
                    if (returnIfUnfound)
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using 
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        private static bool AreEqualityComparersEqual(TreeSet<T> set1, SCG.ICollection<T> set2)
        {
            if (set2 is TreeSet<T> treeSet)
                return set1.EqualityComparer.Equals(treeSet.EqualityComparer);
            else if (set2 is HashSet<T> hashSet)
                return set1.EqualityComparer.Equals(hashSet.EqualityComparer);
            else if (set2 is SCG.HashSet<T> scgHashSet)
                return set1.EqualityComparer.Equals(scgHashSet.Comparer);
            return false;
        }

        /// <summary>
        /// If other is a hashset that uses same equality comparer, intersect is much faster 
        /// because we can use other's Contains
        /// </summary>
        private void IntersectWithHashSetWithSameEC(SCG.ICollection<T> other)
        {
            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    Remove(item);
                }
            }
        }

        private void IntersectWithEnumerable(SCG.IEnumerable<T> other)
        {
            // keep track of current last index; don't want to move past the end of our bit array
            // (could happen if another thread is modifying the collection)
            int originalLastIndex = this.size;
            var bitArray = new System.Collections.BitArray(originalLastIndex, false);

            foreach (var item in other)
            {
                int index = IndexOf(item);
                if (index >= 0)
                    bitArray.Set(index, true);
            }

            // if anything unmarked, remove it.
            for (int i = originalLastIndex - 1; i >= 0; i--)
            {
                if (!bitArray.Get(i))
                    RemoveAt(i);
            }
        }

        /// <summary>
        /// if other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        /// 
        /// As with other methods, callers take care of ensuring that other is a hashset using the
        /// same equality comparer.
        /// </summary>
        private void SymmetricExceptWithUniqueTreeSet(SCG.ICollection<T> other)
        {
            foreach (T item in other)
            {
                if (!Remove(item))
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Implementation Notes:
        /// If other is a hashset and is using same equality comparer, then checking subset is 
        /// faster. Simply check that each element in this is in other.
        /// 
        /// Note: if other doesn't use same equality comparer, then Contains check is invalid,
        /// which is why callers must take are of this.
        /// 
        /// If callers are concerned about whether this is a proper subset, they take care of that.
        ///
        /// </summary>
        private bool IsSubsetOfTreeSetWithSameEC(SCG.ICollection<T> other)
        {

            foreach (T item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        private SCG.IEnumerator<T> GetEnumerator(Node? node, int origstamp)
        {
            if (node == null)
            {
                yield break;
            }

            if (node.left != null)
            {
                SCG.IEnumerator<T> child = GetEnumerator(node.left, origstamp);

                while (child.MoveNext())
                {
                    ModifyCheck(origstamp);
                    yield return child.Current;
                }
            }

            ModifyCheck(origstamp);
            yield return node.item;
            if (node.right != null)
            {
                SCG.IEnumerator<T> child = GetEnumerator(node.right, origstamp);

                while (child.MoveNext())
                {
                    ModifyCheck(origstamp);
                    yield return child.Current;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NoSuchItemException">If tree is empty</exception>
        /// <returns></returns>
        public override T Choose()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            return root!.item;
        }


        /// <summary>
        /// Create an enumerator for this tree
        /// </summary>
        /// <returns>The enumerator</returns>
        public override SCG.IEnumerator<T> GetEnumerator()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (isSnapShot)
            {
                return new SnapEnumerator(this);
            }

            return new Enumerator(this);

        }

        #endregion

        #region ISink<T> Members

        /// <summary>
        /// Add item to tree. If already there, return the found item in the second argument.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="founditem">item found</param>
        /// <param name="update">whether item in node should be updated</param>
        /// <param name="wasfound">true if found in bag, false if not found or tree is a set</param>
        /// <returns>True if item was added</returns>
        private bool AddIterative(T item, ref T founditem, bool update, out bool wasfound)
        {
            wasfound = false;
            if (root == null)
            {
                root = new Node
                {
                    red = false
                };
                blackdepth = 1;
                root.item = item;
                root.generation = generation;
                return true;
            }

            StackCheck();

            int level = 0;
            Node cursor = root;

            while (true)
            {
                int comp = comparer!.Compare(cursor.item, item);

                if (comp == 0)
                {
                    founditem = cursor.item;

                    bool nodeWasUpdated = update;
                    if (update)
                    {
                        Node.CopyNode(ref cursor, MaxSnapId, generation);
                        cursor.item = item;
                    }


                    while (level-- > 0)
                    {
                        if (nodeWasUpdated)
                        {
                            Node kid = cursor;

                            cursor = path![level]!;
                            Node.Update(ref cursor!, dirs![level] > 0, kid, MaxSnapId, generation);

                        }

                        path![level] = null;
                    }

                    if (update)
                    {
                        root = cursor;
                    }

                    return false;

                }

                //else
                Node? child = comp > 0 ? cursor.left : cursor.right;

                if (child == null)
                {
                    child = new Node
                    {
                        item = item,
                        generation = generation
                    };
                    Node.Update(ref cursor, comp > 0, child, MaxSnapId, generation);

                    cursor.size++;

                    dirs![level] = comp;
                    break;
                }
                else
                {
                    dirs![level] = comp;
                    path![level++] = cursor;
                    cursor = child;
                }
            }

            //We have just added the red node child to "cursor"
            while (cursor.red)
            {
                //take one step up:
                Node child = cursor;

                cursor = path![--level]!;
                path[level] = null;
                Node.Update(ref cursor!, dirs[level] > 0, child, MaxSnapId, generation);
                cursor.size++;
                int comp = dirs[level];
                Node? childsibling = comp > 0 ? cursor.right : cursor.left;

                if (childsibling != null && childsibling.red)
                {
                    //Promote
                    child.red = false;
                    Node.Update(ref cursor, comp < 0, childsibling, MaxSnapId, generation);
                    childsibling.red = false;

                    //color cursor red & take one step up the tree unless at root
                    if (level == 0)
                    {
                        root = cursor;
                        blackdepth++;
                        return true;
                    }
                    else
                    {
                        cursor.red = true;
                        child = cursor;
                        cursor = path[--level]!;
                        Node.Update(ref cursor!, dirs[level] > 0, child, MaxSnapId, generation);
                        path[level] = null;
                        cursor.size++;
                    }
                }
                else
                {
                    //ROTATE!!!
                    int childcomp = dirs[level + 1];

                    cursor.red = true;
                    if (comp > 0)
                    {
                        if (childcomp > 0)
                        {//zagzag
                            Node.Update(ref cursor, true, child.right!, MaxSnapId, generation);
                            Node.Update(ref child, false, cursor, MaxSnapId, generation);

                            cursor = child;
                        }
                        else
                        {//zagzig
                            Node badgrandchild = child.right!;
                            Node.Update(ref cursor, true, badgrandchild!.right!, MaxSnapId, generation);
                            Node.Update(ref child, false, badgrandchild.left!, MaxSnapId, generation);
                            Node.CopyNode(ref badgrandchild, MaxSnapId, generation);

                            badgrandchild.left = child;
                            badgrandchild.right = cursor;
                            cursor = badgrandchild;
                        }
                    }
                    else
                    {//comp < 0
                        if (childcomp < 0)
                        {//zigzig
                            Node.Update(ref cursor, false, child.left!, MaxSnapId, generation);
                            Node.Update(ref child, true, cursor, MaxSnapId, generation);

                            cursor = child;
                        }
                        else
                        {//zigzag
                            Node badgrandchild = child.left!;
                            Node.Update(ref cursor, false, badgrandchild.left!, MaxSnapId, generation);
                            Node.Update(ref child, true, badgrandchild.right!, MaxSnapId, generation);
                            Node.CopyNode(ref badgrandchild, MaxSnapId, generation);

                            badgrandchild.right = child;
                            badgrandchild.left = cursor;
                            cursor = badgrandchild;
                        }
                    }

                    cursor.red = false;

                    Node n;


                    n = cursor.right!;
                    cursor.size = n.size = (n.left == null ? 0 : n.left.size) + (n.right == null ? 0 : n.right.size) + 1;
                    n = cursor.left!;
                    n.size = (n.left == null ? 0 : n.left.size) + (n.right == null ? 0 : n.right.size) + 1;
                    cursor.size += n.size + 1;

                    if (level == 0)
                    {
                        root = cursor;
                        return true;
                    }
                    else
                    {
                        child = cursor;
                        cursor = path![--level]!;
                        path[level] = null;
                        Node.Update(ref cursor!, dirs[level] > 0, child, MaxSnapId, generation);

                        cursor.size++;
                        break;
                    }
                }
            }
            bool stillmore = true;
            while (level > 0)
            {
                Node child = cursor;

                cursor = path![--level]!;
                path[level] = null;
                if (stillmore)
                {
                    stillmore = Node.Update(ref cursor!, dirs[level] > 0, child, MaxSnapId, generation);
                }

                cursor.size++;
            }

            root = cursor;
            return true;
        }


        /// <summary>
        /// Add an item to this collection if possible. If this collection has set
        /// semantics, the item will be added if not already in the collection. If
        /// bag semantics, the item will always be added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if item was added.</returns>
        public bool Add(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            //Note: blackdepth of the tree is set inside addIterative
            T jtem = default;
            if (!Add(item, ref jtem))
            {
                return false;
            }

            if (ActiveEvents != 0)
            {
                RaiseForAdd(jtem);
            }

            return true;
        }

        /// <summary>
        /// Add an item to this collection if possible. If this collection has set
        /// semantics, the item will be added if not already in the collection. If
        /// bag semantics, the item will always be added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void SCG.ICollection<T>.Add(T item)
        {
            Add(item);
        }

        private bool Add(T item, ref T j)
        {

            if (AddIterative(item, ref j, false, out bool wasFound))
            {
                size++;
                if (!wasFound)
                {
                    j = item;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. If this
        /// collection has set semantics, only items not already in the collection
        /// will be added.
        /// </summary>
        /// <param name="items">The items to add</param>
        public void AddAll(SCG.IEnumerable<T> items)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            int c = 0;
            T j = default;

            bool raiseAdded = (ActiveEvents & EventType.Added) != 0;
            CircularQueue<T>? wasAdded = raiseAdded ? new CircularQueue<T>() : null;

            foreach (T i in items)
            {
                if (AddIterative(i, ref j, false, out bool tmp))
                {
                    c++;
                    if (raiseAdded)
                    {
                        wasAdded!.Enqueue(tmp ? j : i);
                    }
                }
            }

            if (c == 0)
            {
                return;
            }

            size += c;
            //TODO: implement a RaiseForAddAll() method
            if (raiseAdded)
            {
                foreach (T item in wasAdded!)
                {
                    RaiseItemsAdded(item, 1);
                }
            }

            if (((ActiveEvents & EventType.Changed) != 0))
            {
                RaiseCollectionChanged();
            }
        }


        /// <summary>
        /// Add all the items from another collection with an enumeration order that 
        /// is increasing in the items. <para>The idea is that the implementation may use
        /// a faster algorithm to merge the two collections.</para>
        /// <exception cref="ArgumentException"/> if the enumerated items turns out
        /// not to be in increasing order.
        /// </summary>
        /// <param name="items">The collection to add.</param>
        public void AddSorted(SCG.IEnumerable<T> items)
        {
            if (size > 0)
            {
                AddAll(items);
            }
            else
            {
                if (!isValid)
                {
                    throw new ViewDisposedException("Snapshot has been disposed");
                }

                UpdateCheck();
                AddSorted(items, true, true);
            }
        }

        #region add-sorted helpers

        //Create a RB tree from x+2^h-1  (x < 2^h, h>=1) nodes taken from a
        //singly linked list of red nodes using only the right child refs.
        //The x nodes at depth h+1 will be red, the rest black.
        //(h is the blackdepth of the resulting tree)
        private static Node MakeTreer(ref Node rest, int blackheight, int maxred, int red)
        {
            if (blackheight == 1)
            {
                Node top = rest;

                rest = rest.right!;
                if (red > 0)
                {
                    top.right = null;
                    rest.left = top;
                    top = rest;
                    top.size = 1 + red;
                    rest = rest.right!;
                    red--;
                }

                if (red > 0)
                {

                    top.right = rest;
                    rest = rest.right!;
                    top.right!.right = null;
                }
                else
                {
                    top.right = null;
                }

                top.red = false;
                return top;
            }
            else
            {
                maxred >>= 1;

                int lred = red > maxred ? maxred : red;
                Node left = MakeTreer(ref rest, blackheight - 1, maxred, lred);
                Node top = rest;

                rest = rest.right!;
                top.left = left;
                top.red = false;
                top.right = MakeTreer(ref rest!, blackheight - 1, maxred, red - lred);
                top.size = (maxred << 1) - 1 + red;
                return top;
            }
        }

        private void AddSorted(SCG.IEnumerable<T> items, bool safe, bool raise)
        {
            SCG.IEnumerator<T> e = items.GetEnumerator(); ;
            if (size > 0)
            {
                throw new InternalException("This can't happen");
            }

            if (!e.MoveNext())
            {
                return;
            }

            //To count theCollect 
            Node head = new Node(), tail = head;
            int z = 1;
            T lastitem = tail.item = e.Current;


            while (e.MoveNext())
            {

                z++;
                tail.right = new Node();
                tail = tail.right;
                tail.item = e.Current;
                if (safe)
                {
                    if (comparer!.Compare(lastitem, tail.item) >= 0)
                    {
                        throw new ArgumentException("Argument not sorted");
                    }

                    lastitem = tail.item;
                }
                tail.generation = generation;

            }

            int blackheight = 0, red = z, maxred = 1;

            while (maxred <= red)
            {
                red -= maxred;
                maxred <<= 1;
                blackheight++;
            }

            root = TreeSet<T>.MakeTreer(ref head, blackheight, maxred, red);
            blackdepth = blackheight;
            size = z;


            if (raise)
            {
                if ((ActiveEvents & EventType.Added) != 0)
                {
                    CircularQueue<T> wasAdded = new CircularQueue<T>();
                    foreach (T item in this)
                    {
                        wasAdded.Enqueue(item);
                    }

                    foreach (T item in wasAdded)
                    {
                        RaiseItemsAdded(item, 1);
                    }
                }
                if ((ActiveEvents & EventType.Changed) != 0)
                {
                    RaiseCollectionChanged();
                }
            }
            return;
        }

        #endregion


        /// <summary></summary>
        /// <value>False since this tree has set semantics.</value>
        public bool AllowsDuplicates => false;

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting => true;

        #endregion

        #region IEditableCollection<T> Members


        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>Speed.Log</value>
        public Speed ContainsSpeed => Speed.Log;

        /// <summary>
        /// Check if this collection contains (an item equivalent to according to the
        /// itemequalityComparer) a particular value.
        /// </summary>
        /// <param name="item">The value to check for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public bool Contains(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? next = root;
            while (next != null)
            {
                int comp = comparer!.Compare(next.item, item);
                if (comp == 0)
                {
                    return true;
                }

                next = comp < 0 ? Right(next) : Left(next);
            }

            return false;
        }


        //Variant for dictionary use
        //Will return the actual matching item in the ref argument.
        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, return in the ref argument (a
        /// binary copy of) the actual value found.
        /// </summary>
        /// <param name="item">The value to look for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public bool Find(ref T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? next = root;
            while (next != null)
            {
                int comp = comparer!.Compare(next.item, item);
                if (comp == 0)
                {
                    item = next.item;
                    return true;
                }

                next = comp < 0 ? Right(next) : Left(next);
            }

            return false;
        }


        /// <summary>
        /// Find or add the item to the tree. If the tree does not contain
        /// an item equivalent to this item add it, else return the existing
        /// one in the ref argument. 
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if item was found</returns>
        public bool FindOrAdd(ref T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            //Note: blackdepth of the tree is set inside addIterative
            if (AddIterative(item, ref item, false, out bool wasfound))
            {
                size++;
                if (ActiveEvents != 0 && !wasfound)
                {
                    RaiseForAdd(item);
                }

                return wasfound;
            }
            else
            {
                return true;
            }
        }


        //For dictionary use. 
        //If found, the matching entry will be updated with the new item.
        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// to with a binary copy of the supplied value. If the collection has bag semantics,
        /// this updates all equivalent copies in
        /// the collection.
        /// </summary>
        /// <param name="item">Value to update.</param>
        /// <returns>True if the item was found and hence updated.</returns>
        public bool Update(T item)
        {
            return Update(item, out _);
        }

        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// with a binary copy of the supplied value. If the collection has bag semantics,
        /// this updates all equivalent copies in
        /// the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool Update(T item, out T olditem)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            StackCheck();

            int level = 0;
            Node? cursor = root;
            while (cursor != null)
            {
                int comp = comparer!.Compare(cursor.item, item);
                if (comp == 0)
                {
                    Node.CopyNode(ref cursor!, MaxSnapId, generation);
                    olditem = cursor.item;

                    cursor.item = item;
                    while (level > 0)
                    {
                        Node child = cursor;

                        cursor = path![--level];
                        path[level] = null;
                        Node.Update(ref cursor!, dirs![level] > 0, child, MaxSnapId, generation);

                    }

                    root = cursor;

                    if (ActiveEvents != 0)
                    {
                        RaiseForUpdate(item, olditem);
                    }

                    return true;
                }
                dirs![level] = comp;
                path![level++] = cursor;
                cursor = comp < 0 ? cursor.right : cursor.left;
            }

            olditem = default;
            return false;
        }


        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// with a binary copy of the supplied value; else add the value to the collection. 
        ///
        /// <i>NOTE: the bag implementation is currently wrong! ?????</i>
        /// </summary>
        /// <param name="item">Value to add or update.</param>
        /// <returns>True if the item was found and updated (hence not added).</returns>
        public bool UpdateOrAdd(T item)
        {
            return UpdateOrAdd(item, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool UpdateOrAdd(T item, out T olditem)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            olditem = default;


            //Note: blackdepth of the tree is set inside addIterative
            if (AddIterative(item, ref olditem, true, out bool wasfound))
            {
                size++;
                if (ActiveEvents != 0)
                {
                    RaiseForAdd(wasfound ? olditem : item);
                }

                return wasfound;
            }
            else
            {
                if (ActiveEvents != 0)
                {
                    RaiseForUpdate(item, olditem, 1);
                }

                return true;
            }
        }


        /// <summary>
        /// Remove a particular item from this collection. If the collection has bag
        /// semantics only one copy equivalent to the supplied item is removed. 
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public bool Remove(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            if (root == null)
            {
                return false;
            }

            bool retval = RemoveIterative(ref item, out _);
            if (ActiveEvents != 0 && retval)
            {
                RaiseForRemove(item);
            }

            return retval;
        }

        /// <summary>
        /// Remove a particular item from this collection if found. If the collection
        /// has bag semantics only one copy equivalent to the supplied item is removed,
        /// which one is implementation dependent. 
        /// If an item was removed, report a binary copy of the actual item removed in 
        /// the argument.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <param name="removeditem">The removed value.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public bool Remove(T item, out T removeditem)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            removeditem = item;
            if (root == null)
            {
                return false;
            }

            bool retval = RemoveIterative(ref removeditem, out _);
            if (ActiveEvents != 0 && retval)
            {
                RaiseForRemove(item);
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">input: item to remove; output: item actually removed</param>
        /// <param name="wasRemoved"></param>
        /// <returns></returns>
        private bool RemoveIterative(ref T item, out int wasRemoved)
        {
            wasRemoved = 0;
            //Stage 1: find item
            StackCheck();

            int level = 0, comp;
            Node? cursor = root;

            while (true)
            {
                comp = comparer!.Compare(cursor!.item, item);
                if (comp == 0)
                {
                    item = cursor.item;

                    wasRemoved = 1;

                    break;
                }

                Node child = (comp > 0 ? cursor.left : cursor.right)!;

                if (child == null)
                {
                    return false;
                }

                dirs![level] = comp;
                path![level++] = cursor;
                cursor = child;
            }

            return RemoveIterativePhase2(cursor, level);
        }

        private bool RemoveIterativePhase2(Node cursor, int level)
        {
            if (size == 1)
            {
                ClearInner();
                return true;
            }

            //We are certain to remove one node:
            size--;

            //Stage 2: if item's node has no null child, find predecessor
            int level_of_item = level;

            if (cursor.left != null && cursor.right != null)
            {
                dirs![level] = 1;
                path![level++] = cursor;
                cursor = cursor.left;
                while (cursor.right != null)
                {
                    dirs[level] = -1;
                    path[level++] = cursor;
                    cursor = cursor.right;
                }
                Node.CopyNode(ref path[level_of_item]!, MaxSnapId, generation);
                path[level_of_item]!.item = cursor.item;

            }

            //Stage 3: splice out node to be removed
            Node? newchild = cursor.right ?? cursor.left;
            bool demote_or_rotate = newchild == null && !cursor.red;

            //assert newchild.red 
            if (newchild != null)
            {
                newchild.red = false;
            }

            if (level == 0)
            {
                root = newchild;
                return true;
            }

            level--;
            cursor = path![level]!;
            path[level] = null;

            int comp = dirs![level];
            Node? childsibling;
            Node.Update(ref cursor!, comp > 0, newchild!, MaxSnapId, generation);

            childsibling = comp > 0 ? cursor.right : cursor.left;
            cursor.size--;

            //Stage 4: demote till we must rotate
            Node? farnephew = null, nearnephew = null;

            while (demote_or_rotate)
            {
                if (childsibling!.red)
                {
                    break; //rotate 2+?
                }

                farnephew = comp > 0 ? childsibling.right : childsibling.left;
                if (farnephew != null && farnephew.red)
                {
                    break; //rotate 1b
                }

                nearnephew = comp > 0 ? childsibling.left : childsibling.right;
                if (nearnephew != null && nearnephew.red)
                {
                    break; //rotate 1c
                }

                //demote cursor
                childsibling.red = true;
                if (level == 0)
                {
                    cursor.red = false;
                    blackdepth--;
                    root = cursor;
                    return true;
                }
                else if (cursor.red)
                {
                    cursor.red = false;
                    demote_or_rotate = false;
                    break; //No rotation
                }
                else
                {
                    Node child = cursor;

                    cursor = path![--level]!;
                    path[level] = null;
                    comp = dirs[level];
                    childsibling = comp > 0 ? cursor.right : cursor.left;
                    Node.Update(ref cursor, comp > 0, child, MaxSnapId, generation);
                    cursor.size--;
                }
            }

            //Stage 5: rotate 
            if (demote_or_rotate)
            {
                //At start:
                //parent = cursor (temporary for swapping nodes)
                //childsibling is the sibling of the updated child (x)
                //cursor is always the top of the subtree
                Node parent = cursor;

                if (childsibling!.red)
                {//Case 2 and perhaps more. 
                    //The y.rank == px.rank >= x.rank+2 >=2 so both nephews are != null 
                    //(and black). The grandnephews are children of nearnephew
                    Node neargrandnephew, fargrandnephew;

                    if (comp > 0)
                    {
                        nearnephew = childsibling.left;
                        farnephew = childsibling.right;
                        neargrandnephew = nearnephew!.left!;
                        fargrandnephew = nearnephew.right!;
                    }
                    else
                    {
                        nearnephew = childsibling.right;
                        farnephew = childsibling.left;
                        neargrandnephew = nearnephew!.right!;
                        fargrandnephew = nearnephew.left!;
                    }

                    if (fargrandnephew != null && fargrandnephew.red)
                    {//Case 2+1b
                        Node.CopyNode(ref nearnephew!, MaxSnapId, generation);

                        //The end result of this will always be e copy of parent
                        Node.Update(ref parent, comp < 0, neargrandnephew!, MaxSnapId, generation);
                        Node.Update(ref childsibling!, comp > 0, nearnephew, MaxSnapId, generation);
                        if (comp > 0)
                        {
                            nearnephew.left = parent;
                            parent.right = neargrandnephew;
                        }
                        else
                        {
                            nearnephew.right = parent;
                            parent.left = neargrandnephew;
                        }

                        cursor = childsibling;
                        childsibling.red = false;
                        nearnephew.red = true;
                        fargrandnephew.red = false;
                        cursor.size = parent.size;
                        nearnephew.size = cursor.size - 1 - farnephew!.size;
                        parent.size = nearnephew.size - 1 - fargrandnephew.size;
                    }
                    else if (neargrandnephew != null && neargrandnephew.red)
                    {//Case 2+1c
                        Node.CopyNode(ref neargrandnephew, MaxSnapId, generation);
                        if (comp > 0)
                        {
                            Node.Update(ref childsibling!, true, neargrandnephew, MaxSnapId, generation);
                            Node.Update(ref nearnephew!, true, neargrandnephew.right!, MaxSnapId, generation);
                            Node.Update(ref parent, false, neargrandnephew.left!, MaxSnapId, generation);
                            neargrandnephew.left = parent;
                            neargrandnephew.right = nearnephew;
                        }
                        else
                        {
                            Node.Update(ref childsibling!, false, neargrandnephew, MaxSnapId, generation);
                            Node.Update(ref nearnephew!, false, neargrandnephew.left!, MaxSnapId, generation);
                            Node.Update(ref parent, true, neargrandnephew.right!, MaxSnapId, generation);
                            neargrandnephew.right = parent;
                            neargrandnephew.left = nearnephew;
                        }

                        cursor = childsibling;
                        childsibling.red = false;
                        cursor.size = parent.size;
                        parent.size = 1 + (parent.left == null ? 0 : parent.left.size) + (parent.right == null ? 0 : parent.right.size);
                        nearnephew.size = 1 + (nearnephew.left == null ? 0 : nearnephew.left.size) + (nearnephew.right == null ? 0 : nearnephew.right.size);
                        neargrandnephew.size = 1 + parent.size + nearnephew.size;
                    }
                    else
                    {//Case 2 only
                        Node.Update(ref parent, comp < 0, nearnephew, MaxSnapId, generation);
                        Node.Update(ref childsibling!, comp > 0, parent, MaxSnapId, generation);

                        cursor = childsibling;
                        childsibling.red = false;
                        nearnephew.red = true;
                        cursor.size = parent.size;
                        parent.size -= farnephew!.size + 1;
                    }
                }
                else if (farnephew != null && farnephew.red)
                {//Case 1b
                    nearnephew = comp > 0 ? childsibling.left : childsibling.right;
                    Node.Update(ref parent, comp < 0, nearnephew!, MaxSnapId, generation);
                    Node.CopyNode(ref childsibling!, MaxSnapId, generation);
                    if (comp > 0)
                    {
                        childsibling.left = parent;
                        childsibling.right = farnephew;
                    }
                    else
                    {
                        childsibling.right = parent;
                        childsibling.left = farnephew;
                    }

                    cursor = childsibling;
                    cursor.red = parent.red;
                    parent.red = false;
                    farnephew.red = false;

                    cursor.size = parent.size;
                    parent.size -= farnephew.size + 1;
                }
                else if (nearnephew != null && nearnephew.red)
                {//Case 1c
                    Node.CopyNode(ref nearnephew!, MaxSnapId, generation);
                    if (comp > 0)
                    {
                        Node.Update(ref childsibling!, true, nearnephew.right!, MaxSnapId, generation);
                        Node.Update(ref parent, false, nearnephew.left!, MaxSnapId, generation);

                        nearnephew.left = parent;
                        nearnephew.right = childsibling;
                    }
                    else
                    {
                        Node.Update(ref childsibling!, false, nearnephew.left!, MaxSnapId, generation);
                        Node.Update(ref parent, true, nearnephew.right!, MaxSnapId, generation);

                        nearnephew.right = parent;
                        nearnephew.left = childsibling;
                    }

                    cursor = nearnephew;
                    cursor.red = parent.red;
                    parent.red = false;
                    cursor.size = parent.size;
                    parent.size = 1 + (parent.left == null ? 0 : parent.left.size) + (parent.right == null ? 0 : parent.right.size);
                    childsibling.size = 1 + (childsibling.left == null ? 0 : childsibling.left.size) + (childsibling.right == null ? 0 : childsibling.right.size);
                }
                else
                {//Case 1a can't happen
                    throw new InternalException("Case 1a can't happen here");
                }

                //Resplice cursor:
                if (level == 0)
                {
                    root = cursor;
                }
                else
                {
                    Node swap = cursor;

                    cursor = path[--level]!;
                    path[level] = null;
                    Node.Update(ref cursor!, dirs[level] > 0, swap, MaxSnapId, generation);

                    cursor.size--;
                }
            }

            //Stage 6: fixup to the root
            while (level > 0)
            {
                Node child = cursor;

                cursor = path![--level]!;
                path[level] = null;
                if (child != (dirs[level] > 0 ? cursor.left : cursor.right))
                {
                    Node.Update(ref cursor, dirs[level] > 0, child, MaxSnapId, generation);
                }

                cursor.size--;
            }

            root = cursor;
            return true;
        }


        /// <summary>
        /// Remove all items from this collection.
        /// </summary>
        public void Clear()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            int oldsize = size;
            ClearInner();
            if ((ActiveEvents & EventType.Cleared) != 0)
            {
                RaiseCollectionCleared(true, oldsize);
            }

            if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }

        private void ClearInner()
        {
            size = 0;
            root = null;
            blackdepth = 0;
        }

        /// <summary>
        /// Remove all items in another collection from this one. If this collection
        /// has bag semantics, take multiplicities into account.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        public void RemoveAll(SCG.IEnumerable<T> items)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            T jtem;

            bool mustRaise = (ActiveEvents & (EventType.Removed | EventType.Changed)) != 0;
            RaiseForRemoveAllHandler? raiseHandler = mustRaise ? new RaiseForRemoveAllHandler(this) : null;

            foreach (T item in items)
            {
                if (root == null)
                {
                    break;
                }

                jtem = item;
                if (RemoveIterative(ref jtem, out int junk) && mustRaise)
                {
                    raiseHandler!.Remove(jtem);
                }
            }
            if (mustRaise)
            {
                raiseHandler!.Raise();
            }
        }

        /// <summary>
        /// Remove all items not in some other collection from this one. If this collection
        /// has bag semantics, take multiplicities into account.
        /// </summary>
        /// <param name="items">The items to retain.</param>
        public void RetainAll(SCG.IEnumerable<T> items)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            //A much more efficient version is possible if items is sorted like this.
            //Well, it is unclear how efficient it would be.
            //We could use a marking method!?
#warning how does this work together with persistence?
            TreeSet<T> t = (TreeSet<T>)MemberwiseClone();

            T jtem = default;
            t.ClearInner();
            foreach (T item in items)
            {
                if (ContainsCount(item) > t.ContainsCount(item))
                {
                    t.Add(item, ref jtem);
                }
            }

            if (size == t.size)
            {
                return;
            }

#warning improve (mainly for bag) by using a Node iterator instead of ItemMultiplicities()
            CircularQueue<System.Collections.Generic.KeyValuePair<T, int>>? wasRemoved = null;
            if ((ActiveEvents & EventType.Removed) != 0)
            {
                wasRemoved = new CircularQueue<System.Collections.Generic.KeyValuePair<T, int>>();
                SCG.IEnumerator<System.Collections.Generic.KeyValuePair<T, int>> ie = ItemMultiplicities().GetEnumerator();
                foreach (System.Collections.Generic.KeyValuePair<T, int> p in t.ItemMultiplicities())
                {
                    //We know p.Key is in this!
                    while (ie.MoveNext())
                    {
                        if (comparer!.Compare(ie.Current.Key, p.Key) == 0)
                        {

                            break;
                        }
                        else
                        {
                            wasRemoved.Enqueue(ie.Current);
                        }
                    }
                }
                while (ie.MoveNext())
                {
                    wasRemoved.Enqueue(ie.Current);
                }
            }

            root = t.root;
            size = t.size;
            blackdepth = t.blackdepth;
            if (wasRemoved != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<T, int> p in wasRemoved)
                {
                    RaiseItemsRemoved(p.Key, p.Value);
                }
            }

            if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// Check if this collection contains all the values in another collection.
        /// If this collection has bag semantics (<code>AllowsDuplicates==true</code>)
        /// the check is made with respect to multiplicities, else multiplicities
        /// are not taken into account.
        /// </summary>
        /// <param name="items">The </param>
        /// <returns>True if all values in <code>items</code>is in this collection.</returns>
        public bool ContainsAll(SCG.IEnumerable<T> items)
        {
            //TODO: fix bag implementation
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }
            //This is worst-case O(m*logn)
            foreach (T item in items)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }


        //Higher order:
        /// <summary>
        /// Create a new indexed sorted collection consisting of the items of this
        /// indexed sorted collection satisfying a certain predicate.
        /// </summary>
        /// <param name="filter">The filter delegate defining the predicate.</param>
        /// <returns>The new indexed sorted collection.</returns>
        public IIndexedSorted<T> FindAll(Func<T, bool> filter)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            TreeSet<T> res = new TreeSet<T>(comparer!);
            SCG.IEnumerator<T> e = GetEnumerator();
            Node? head = null, tail = null;
            int z = 0;

            while (e.MoveNext())
            {
                T thisitem = e.Current;

                if (filter(thisitem))
                {
                    if (head == null)
                    {
                        head = tail = new Node();
                    }
                    else
                    {

                        tail!.right = new Node();
                        tail = tail.right;
                    }

                    tail.item = thisitem;
                    z++;
                }
            }


            if (z == 0)
            {
                return res;
            }

            int blackheight = 0, red = z, maxred = 1;

            while (maxred <= red)
            {
                red -= maxred;
                maxred <<= 1;
                blackheight++;
            }

            res.root = TreeSet<T>.MakeTreer(ref head!, blackheight, maxred, red);
            res.blackdepth = blackheight;
            res.size = z;

            return res;
        }


        /// <summary>
        /// Create a new indexed sorted collection consisting of the results of
        /// mapping all items of this list.
        /// <exception cref="ArgumentException"/> if the map is not increasing over 
        /// the items of this collection (with respect to the two given comparison 
        /// relations).
        /// </summary>
        /// <param name="mapper">The delegate definging the map.</param>
        /// <param name="c">The comparion relation to use for the result.</param>
        /// <returns>The new sorted collection.</returns>
        public IIndexedSorted<V> Map<V>(Func<T, V> mapper, SCG.IComparer<V> c)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            TreeSet<V> res = new TreeSet<V>(c);

            if (size == 0)
            {
                return res;
            }

            SCG.IEnumerator<T> e = GetEnumerator();
            TreeSet<V>.Node? head = null, tail = null;
            V oldv = default;
            int z = 0;

            while (e.MoveNext())
            {
                T thisitem = e.Current;

                V newv = mapper(thisitem);

                if (head == null)
                {
                    head = tail = new TreeSet<V>.Node();
                    z++;
                }
                else
                {
                    int comp = c.Compare(oldv, newv);

                    if (comp >= 0)
                    {
                        throw new ArgumentException("mapper not monotonic");
                    }

                    tail!.right = new TreeSet<V>.Node();
                    tail = tail.right;
                    z++;
                }

                tail.item = oldv = newv;
            }


            int blackheight = 0, red = z, maxred = 1;

            while (maxred <= red)
            {
                red -= maxred;
                maxred <<= 1;
                blackheight++;
            }

            res.root = TreeSet<V>.MakeTreer(ref head!, blackheight, maxred, red);
            res.blackdepth = blackheight;
            res.size = size;
            return res;
        }


        //below is the bag utility stuff
        /// <summary>
        /// Count the number of items of the collection equal to a particular value.
        /// Returns 0 if and only if the value is not in the collection.
        /// </summary>
        /// <param name="item">The value to count.</param>
        /// <returns>The number of copies found.</returns>
        public int ContainsCount(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            //Since we are strictly not AllowsDuplicates we just do
            return Contains(item) ? 1 : 0;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<T> UniqueItems()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return this;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return new MultiplicityOne<T>(this);

        }

        /// <summary>
        /// Remove all items equivalent to a given value.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        public void RemoveAllCopies(T item)
        {

            Remove(item);

        }

        #endregion

        #region IIndexed<T> Members

        private Node FindNode(int i)
        {
            if (isSnapShot)
            {
                throw new NotSupportedException("Indexing not supported for snapshots");
            }

            Node? next = root;

            if (i >= 0 && i < size)
            {
                while (true)
                {
                    int j = next!.left == null ? 0 : next.left.size;

                    if (i > j)
                    {

                        i -= j + 1;

                        next = next.right;
                    }
                    else if (i == j)
                    {
                        return next;
                    }
                    else
                    {
                        next = next.left;
                    }
                }
            }

            throw new IndexOutOfRangeException();

        }


        /// <summary>
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <value>The i'th item of this list.</value>
        /// <param name="i">the index to lookup</param>
        public T this[int i] => FindNode(i).item;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed IndexingSpeed => Speed.Log;


        //TODO: return -upper instead of -1 in case of not found
        /// <summary>
        /// Searches for an item in this indexed collection going forwards from the start.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of first occurrence from start of the item
        /// if found, else the two-complement 
        /// (always negative) of the index at which the item would be put if it was added.</returns>
        public int IndexOf(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return IndexOf(item, out _);
        }


        private int IndexOf(T item, out int upper)
        {
            if (isSnapShot)
            {
                throw new NotSupportedException("Indexing not supported for snapshots");
            }

            int ind = 0; Node? next = root;

            while (next != null)
            {
                int comp = comparer!.Compare(item, next.item);

                if (comp < 0)
                {
                    next = next.left!;
                }
                else
                {
                    int leftcnt = next.left == null ? 0 : next.left.size;

                    if (comp == 0)
                    {

                        return upper = ind + leftcnt;

                    }
                    else
                    {

                        ind = ind + 1 + leftcnt;

                        next = next.right!;
                    }
                }
            }
            upper = ~ind;
            return ~ind;
        }


        /// <summary>
        /// Searches for an item in the tree going backwards from the end.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of last occurrence from the end of item if found, 
        /// else the two-complement (always negative) of the index at which 
        /// the item would be put if it was added.</returns>
        public int LastIndexOf(T item)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            //We have AllowsDuplicates==false for the set
            return IndexOf(item);

        }


        /// <summary>
        /// Remove the item at a specific position of the list.
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <param name="i">The index of the item to remove.</param>
        /// <returns>The removed item.</returns>
        public T RemoveAt(int i)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            T retval = RemoveAtInner(i);
            if (ActiveEvents != 0)
            {
                RaiseForRemove(retval);
            }

            return retval;
        }

        private T RemoveAtInner(int i)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();
            if (i < 0 || i >= size)
            {
                throw new IndexOutOfRangeException("Index out of range for sequenced collectionvalue");
            }

            //We must follow the pattern of removeIterative()
            while (dirs!.Length < 2 * blackdepth)
            {
                dirs = new int[2 * dirs.Length];
                path = new Node[2 * dirs.Length];
            }

            int level = 0;
            Node? cursor = root;

            while (true)
            {
                int j = cursor!.left == null ? 0 : cursor.left.size;

                if (i > j)
                {

                    i -= j + 1;

                    dirs[level] = -1;
                    path![level++] = cursor;
                    cursor = cursor.right;
                }
                else if (i == j)
                {
                    break;
                }
                else
                {
                    dirs[level] = 1;
                    path![level++] = cursor;
                    cursor = cursor.left;
                }
            }

            T retval = cursor.item;


            RemoveIterativePhase2(cursor, level);
            return retval;

        }


        /// <summary>
        /// Remove all items in an index interval.
        /// <exception cref="IndexOutOfRangeException"/>???. 
        /// </summary>
        /// <param name="start">The index of the first item to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        public void RemoveInterval(int start, int count)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (start < 0 || count < 0 || start + count > size)
            {
                throw new ArgumentOutOfRangeException();
            }

            UpdateCheck();

            if (count == 0)
            {
                return;
            }

            //This is terrible for large count. We should split the tree at 
            //the endpoints of the range and fuse the parts!
            //We really need good internal destructive split and catenate functions!
            //Alternative for large counts: rebuild tree using maketree()
            for (int i = 0; i < count; i++)
            {
                RemoveAtInner(start);
            }

            if ((ActiveEvents & EventType.Cleared) != 0)
            {
                RaiseCollectionCleared(false, count);
            }

            if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }


        /// <summary>
        /// <exception cref="IndexOutOfRangeException"/>.
        /// </summary>
        /// <value>The directed collection of items in a specific index interval.</value>
        /// <param name="start">The starting index of the interval (inclusive).</param>
        /// <param name="count">The length of the interval.</param>
        public IDirectedCollectionValue<T> this[int start, int count]
        {
            get
            {
                CheckRange(start, count);
                return new Interval(this, start, count, true);
            }
        }

        #region Interval nested class
        [Serializable]
        private class Interval : DirectedCollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            private readonly int start, length, stamp;
            private readonly bool forwards;
            private readonly TreeSet<T> tree;


            internal Interval(TreeSet<T> tree, int start, int count, bool forwards)
            {
                if (tree.isSnapShot)
                {
                    throw new NotSupportedException("Indexing not supported for snapshots");
                }

                this.start = start; length = count; this.forwards = forwards;
                this.tree = tree; stamp = tree.stamp;
            }

            public override bool IsReadOnly => true;

            public override bool IsEmpty => length == 0;

            public override int Count => length;


            public override Speed CountSpeed => Speed.Constant;


            public override T Choose()
            {
                if (length == 0)
                {
                    throw new NoSuchItemException();
                }

                return tree[start];
            }

            public override SCG.IEnumerator<T> GetEnumerator()
            {
                tree.ModifyCheck(stamp);

                Node? cursor = tree.root;
                Node[] path = new Node[2 * tree.blackdepth];
                int level = 0, totaltogo = length;

                if (totaltogo == 0)
                {
                    yield break;
                }

                if (forwards)
                {
                    int i = start;

                    while (true)
                    {
                        int j = cursor!.left == null ? 0 : cursor.left.size;

                        if (i > j)
                        {

                            i -= j + 1;

                            cursor = cursor.right;
                        }
                        else if (i == j)
                        {

                            break;
                        } // i < j, start point tree[start] is in left subtree
                        else
                        {
                            path[level++] = cursor;
                            cursor = cursor.left;
                        }
                    }

                    T current = cursor.item;

                    while (totaltogo-- > 0)
                    {
                        yield return current;
                        tree.ModifyCheck(stamp);

                        if (cursor.right != null)
                        {
                            path[level] = cursor = cursor.right;
                            while (cursor.left != null)
                            {
                                path[++level] = cursor = cursor.left;
                            }
                        }
                        else if (level == 0)
                        {
                            yield break;
                        }
                        else
                        {
                            cursor = path[--level];
                        }

                        current = cursor.item;

                    }
                }
                else // backwards
                {
                    int i = start + length - 1;

                    while (true)
                    {
                        int j = cursor!.left == null ? 0 : cursor.left.size;

                        if (i > j)
                        {

                            i -= j + 1;

                            path[level++] = cursor;
                            cursor = cursor.right;
                        }
                        else if (i == j)
                        {

                            break;
                        }
                        else // i <= j, end point tree[start+count-1] is in left subtree
                        {
                            cursor = cursor.left;
                        }
                    }

                    T current = cursor.item;

                    while (totaltogo-- > 0)
                    {
                        yield return current;
                        tree.ModifyCheck(stamp);

                        if (cursor.left != null)
                        {
                            path[level] = cursor = cursor.left;
                            while (cursor.right != null)
                            {
                                path[++level] = cursor = cursor.right;
                            }
                        }
                        else if (level == 0)
                        {
                            yield break;
                        }
                        else
                        {
                            cursor = path[--level];
                        }

                        current = cursor.item;

                    }
                }


            }


            public override IDirectedCollectionValue<T> Backwards()
            { return new Interval(tree, start, length, !forwards); }


            IDirectedEnumerable<T> C5.IDirectedEnumerable<T>.Backwards()
            { return Backwards(); }


            public override EnumerationDirection Direction => forwards ? EnumerationDirection.Forwards : EnumerationDirection.Backwards;
        }
        #endregion

        /// <summary>
        /// Create a collection containing the same items as this collection, but
        /// whose enumerator will enumerate the items backwards. The new collection
        /// will become invalid if the original is modified. Method typically used as in
        /// <code>foreach (T x in coll.Backwards()) {...}</code>
        /// </summary>
        /// <returns>The backwards collection.</returns>
        public override IDirectedCollectionValue<T> Backwards() { return RangeAll().Backwards(); }


        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

        #endregion

        #region PriorityQueue Members

        /// <summary>
        /// The comparer object supplied at creation time for this collection
        /// </summary>
        /// <value>The comparer</value>
        public SCG.IComparer<T> Comparer => comparer!;


        /// <summary>
        /// Find the current least item of this priority queue.
        /// </summary>
        /// <returns>The least item.</returns>
        public T FindMin()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            Node cursor = root!, next = Left(cursor!);

            while (next != null)
            {
                cursor = next;
                next = Left(cursor);
            }

            return cursor!.item;
        }


        /// <summary>
        /// Remove the least item from this  priority queue.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMin()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            //persistence guard?
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            //We must follow the pattern of removeIterative()
            StackCheck();

            T retval = DeleteMinInner();
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(retval, 1);
                RaiseCollectionChanged();
            }
            return retval;
        }

        private T DeleteMinInner()
        {
            int level = 0;
            Node? cursor = root;

            while (cursor!.left != null)
            {
                dirs![level] = 1;
                path![level++] = cursor;
                cursor = cursor.left;
            }

            T retval = cursor.item;

            RemoveIterativePhase2(cursor, level);
            return retval;
        }


        /// <summary>
        /// Find the current largest item of this priority queue.
        /// </summary>
        /// <returns>The largest item.</returns>
        public T FindMax()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            Node? cursor = root, next = Right(cursor!);

            while (next != null)
            {
                cursor = next;
                next = Right(cursor);
            }

            return cursor!.item;
        }


        /// <summary>
        /// Remove the largest item from this  priority queue.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T DeleteMax()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }
            //persistence guard?
            UpdateCheck();
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            //We must follow the pattern of removeIterative()
            StackCheck();

            T retval = DeleteMaxInner();
            if (ActiveEvents != 0)
            {
                RaiseItemsRemoved(retval, 1);
                RaiseCollectionChanged();
            }
            return retval;
        }

        private T DeleteMaxInner()
        {
            int level = 0;
            Node? cursor = root;

            while (cursor!.right != null)
            {
                dirs![level] = -1;
                path![level++] = cursor;
                cursor = cursor.right;
            }

            T retval = cursor.item;

            RemoveIterativePhase2(cursor, level);
            return retval;
        }
        #endregion

        #region ISorted<T> Members

        /// <summary>
        /// Find the strict predecessor of item in the sorted collection,
        /// that is, the greatest item in the collection smaller than the item.
        /// </summary>
        /// <param name="item">The item to find the predecessor for.</param>
        /// <param name="res">The predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a predecessor; otherwise false.</returns>
        public bool TryPredecessor(T item, out T res)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? cursor = root, bestsofar = null;

            while (cursor != null)
            {
                int comp = comparer!.Compare(cursor.item, item);

                if (comp < 0)
                {
                    bestsofar = cursor;
                    cursor = Right(cursor);
                }
                else if (comp == 0)
                {
                    cursor = Left(cursor);
                    while (cursor != null)
                    {
                        bestsofar = cursor;
                        cursor = Right(cursor);
                    }
                }
                else
                {
                    cursor = Left(cursor);
                }
            }
            if (bestsofar == null)
            {
                res = default;
                return false;
            }
            else
            {
                res = bestsofar.item;
                return true;
            }
        }


        /// <summary>
        /// Find the strict successor of item in the sorted collection,
        /// that is, the least item in the collection greater than the supplied value.
        /// </summary>
        /// <param name="item">The item to find the successor for.</param>
        /// <param name="res">The successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a successor; otherwise false.</returns>
        public bool TrySuccessor(T item, out T res)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? cursor = root, bestsofar = null;

            while (cursor != null)
            {
                int comp = comparer!.Compare(cursor.item, item);

                if (comp > 0)
                {
                    bestsofar = cursor;
                    cursor = Left(cursor);
                }
                else if (comp == 0)
                {
                    cursor = Right(cursor);
                    while (cursor != null)
                    {
                        bestsofar = cursor;
                        cursor = Left(cursor);
                    }
                }
                else
                {
                    cursor = Right(cursor);
                }
            }

            if (bestsofar == null)
            {
                res = default;
                return false;
            }
            else
            {
                res = bestsofar.item;
                return true;
            }
        }


        /// <summary>
        /// Find the weak predecessor of item in the sorted collection,
        /// that is, the greatest item in the collection smaller than or equal to the item.
        /// </summary>
        /// <param name="item">The item to find the weak predecessor for.</param>
        /// <param name="res">The weak predecessor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak predecessor; otherwise false.</returns>
        public bool TryWeakPredecessor(T item, out T res)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? cursor = root, bestsofar = null;

            while (cursor != null)
            {
                int comp = comparer!.Compare(cursor.item, item);

                if (comp < 0)
                {
                    bestsofar = cursor;
                    cursor = Right(cursor);
                }
                else if (comp == 0)
                {
                    res = cursor.item;
                    return true;
                }
                else
                {
                    cursor = Left(cursor);
                }
            }
            if (bestsofar == null)
            {
                res = default;
                return false;
            }
            else
            {
                res = bestsofar.item;
                return true;
            }
        }


        /// <summary>
        /// Find the weak successor of item in the sorted collection,
        /// that is, the least item in the collection greater than or equal to the supplied value.
        /// </summary>
        /// <param name="item">The item to find the weak successor for.</param>
        /// <param name="res">The weak successor, if any; otherwise the default value for T.</param>
        /// <returns>True if item has a weak successor; otherwise false.</returns>
        public bool TryWeakSuccessor(T item, out T res)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? cursor = root, bestsofar = null;

            while (cursor != null)
            {
                int comp = comparer!.Compare(cursor.item, item);

                if (comp == 0)
                {
                    res = cursor.item;
                    return true;
                }
                else if (comp > 0)
                {
                    bestsofar = cursor;
                    cursor = Left(cursor);
                }
                else
                {
                    cursor = Right(cursor);
                }
            }

            if (bestsofar == null)
            {
                res = default;
                return false;
            }
            else
            {
                res = bestsofar.item;
                return true;
            }
        }


        /// <summary>
        /// Find the strict predecessor in the sorted collection of a particular value,
        /// i.e. the largest item in the collection less than the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is less than or equal to the minimum of this collection.)</exception>
        /// <param name="item">The item to find the predecessor for.</param>
        /// <returns>The predecessor.</returns>
        public T Predecessor(T item)
        {
            if (TryPredecessor(item, out T res))
            {
                return res;
            }
            else
            {
                throw new NoSuchItemException();
            }
        }


        /// <summary>
        /// Find the weak predecessor in the sorted collection of a particular value,
        /// i.e. the largest item in the collection less than or equal to the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is less than the minimum of this collection.)</exception>
        /// <param name="item">The item to find the weak predecessor for.</param>
        /// <returns>The weak predecessor.</returns>
        public T WeakPredecessor(T item)
        {
            if (TryWeakPredecessor(item, out T res))
            {
                return res;
            }
            else
            {
                throw new NoSuchItemException();
            }
        }


        /// <summary>
        /// Find the strict successor in the sorted collection of a particular value,
        /// i.e. the least item in the collection greater than the supplied value.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is greater than or equal to the maximum of this collection.)</exception>
        /// <param name="item">The item to find the successor for.</param>
        /// <returns>The successor.</returns>
        public T Successor(T item)
        {
            if (TrySuccessor(item, out T res))
            {
                return res;
            }
            else
            {
                throw new NoSuchItemException();
            }
        }


        /// <summary>
        /// Find the weak successor in the sorted collection of a particular value,
        /// i.e. the least item in the collection greater than or equal to the supplied value.
        /// <exception cref="NoSuchItemException"> if no such element exists (the
        /// supplied  value is greater than the maximum of this collection.)</exception>
        /// </summary>
        /// <param name="item">The item to find the weak successor for.</param>
        /// <returns>The weak successor.</returns>
        public T WeakSuccessor(T item)
        {
            if (TryWeakSuccessor(item, out T res))
            {
                return res;
            }
            else
            {
                throw new NoSuchItemException();
            }
        }


        /// <summary>
        /// Query this sorted collection for items greater than or equal to a supplied value.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeFrom(T bot)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return new Range(this, true, bot, false, default, EnumerationDirection.Forwards);
        }


        /// <summary>
        /// Query this sorted collection for items between two supplied values.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive).</param>
        /// <param name="top">The upper bound (exclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return new Range(this, true, bot, true, top, EnumerationDirection.Forwards);
        }


        /// <summary>
        /// Query this sorted collection for items less than a supplied value.
        /// </summary>
        /// <param name="top">The upper bound (exclusive).</param>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeTo(T top)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return new Range(this, false, default, true, top, EnumerationDirection.Forwards);
        }


        /// <summary>
        /// Create a directed collection with the same items as this collection.
        /// </summary>
        /// <returns>The result directed collection.</returns>
        public IDirectedCollectionValue<T> RangeAll()
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return new Range(this, false, default, false, default, EnumerationDirection.Forwards);
        }


        IDirectedEnumerable<T> ISorted<T>.RangeFrom(T bot) { return RangeFrom(bot); }


        IDirectedEnumerable<T> ISorted<T>.RangeFromTo(T bot, T top) { return RangeFromTo(bot, top); }


        IDirectedEnumerable<T> ISorted<T>.RangeTo(T top) { return RangeTo(top); }

        //Utility for CountXxxx. Actually always called with strict = true.
        private int CountTo(T item, bool strict)
        {
            if (isSnapShot)
            {
                throw new NotSupportedException("Indexing not supported for snapshots");
            }

            int ind = 0; Node? next = root;

            while (next != null)
            {
                int comp = comparer!.Compare(item, next.item);
                if (comp < 0)
                {
                    next = next.left;
                }
                else
                {
                    int leftcnt = next.left == null ? 0 : next.left.size;

                    if (comp == 0)
                    {
                        return strict ? ind + leftcnt : ind + leftcnt + 1;
                    }
                    else
                    {
                        ind = ind + 1 + leftcnt;
                        next = next.right;
                    }

                }
            }

            //if we get here, we are at the same side of the whole collection:
            return ind;

        }


        /// <summary>
        /// Perform a search in the sorted collection for the ranges in which a
        /// non-increasing (i.e. weakly decreasing) function from the item type to 
        /// <code>int</code> is
        /// negative, zero respectively positive. If the supplied cut function is
        /// not non-increasing, the result of this call is undefined.
        /// </summary>
        /// <param name="c">The cut function <code>T</code> to <code>int</code>, given
        /// as an <code>IComparable&lt;T&gt;</code> object, where the cut function is
        /// the <code>c.CompareTo(T that)</code> method.</param>
        /// <param name="low">Returns the largest item in the collection, where the
        /// cut function is positive (if any).</param>
        /// <param name="lowIsValid">True if the cut function is positive somewhere
        /// on this collection.</param>
        /// <param name="high">Returns the least item in the collection, where the
        /// cut function is negative (if any).</param>
        /// <param name="highIsValid">True if the cut function is negative somewhere
        /// on this collection.</param>
        /// <returns></returns>
        public bool Cut(IComparable<T> c, out T low, out bool lowIsValid, out T high, out bool highIsValid)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            Node? cursor = root, lbest = null, rbest = null;
            bool res = false;

            while (cursor != null)
            {
                int comp = c.CompareTo(cursor.item);

                if (comp > 0)
                {
                    lbest = cursor;
                    cursor = Right(cursor);
                }
                else if (comp < 0)
                {
                    rbest = cursor;
                    cursor = Left(cursor);
                }
                else
                {
                    res = true;

                    Node tmp = Left(cursor);

                    while (tmp != null && c.CompareTo(tmp.item) == 0)
                    {
                        tmp = Left(tmp);
                    }

                    if (tmp != null)
                    {
                        lbest = tmp;
                        tmp = Right(tmp);
                        while (tmp != null)
                        {
                            if (c.CompareTo(tmp.item) > 0)
                            {
                                lbest = tmp;
                                tmp = Right(tmp);
                            }
                            else
                            {
                                tmp = Left(tmp);
                            }
                        }
                    }

                    tmp = Right(cursor);
                    while (tmp != null && c.CompareTo(tmp.item) == 0)
                    {
                        tmp = Right(tmp);
                    }

                    if (tmp != null)
                    {
                        rbest = tmp;
                        tmp = Left(tmp);
                        while (tmp != null)
                        {
                            if (c.CompareTo(tmp.item) < 0)
                            {
                                rbest = tmp;
                                tmp = Left(tmp);
                            }
                            else
                            {
                                tmp = Right(tmp);
                            }
                        }
                    }

                    break;
                }
            }

            if (highIsValid = (rbest != null))
            {
                high = rbest!.item;
            }
            else
            {
                high = default;
            }

            if (lowIsValid = (lbest != null))
            {
                low = lbest!.item;
            }
            else
            {
                low = default;
            }

            return res;
        }


        /// <summary>
        /// Determine the number of items at or above a supplied threshold.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountFrom(T bot)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return size - CountTo(bot, true);
        }


        /// <summary>
        /// Determine the number of items between two supplied thresholds.
        /// </summary>
        /// <param name="bot">The lower bound (inclusive)</param>
        /// <param name="top">The upper bound (exclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountFromTo(T bot, T top)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            if (comparer!.Compare(bot, top) >= 0)
            {
                return 0;
            }

            return CountTo(top, true) - CountTo(bot, true);
        }


        /// <summary>
        /// Determine the number of items below a supplied threshold.
        /// </summary>
        /// <param name="top">The upper bound (exclusive)</param>
        /// <returns>The number of matching items.</returns>
        public int CountTo(T top)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            return CountTo(top, true);
        }


        /// <summary>
        /// Remove all items of this collection above or at a supplied threshold.
        /// </summary>
        /// <param name="low">The lower threshold (inclusive).</param>
        public void RemoveRangeFrom(T low)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            int count = CountFrom(low);

            if (count == 0)
            {
                return;
            }

            StackCheck();
            CircularQueue<T>? wasRemoved = (ActiveEvents & EventType.Removed) != 0 ? new CircularQueue<T>() : null;

            for (int i = 0; i < count; i++)
            {
                T item = DeleteMaxInner();
                if (wasRemoved != null)
                {
                    wasRemoved.Enqueue(item);
                }
            }
            if (wasRemoved != null)
            {
                RaiseForRemoveAll(wasRemoved);
            }
            else if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }


        /// <summary>
        /// Remove all items of this collection between two supplied thresholds.
        /// </summary>
        /// <param name="low">The lower threshold (inclusive).</param>
        /// <param name="hi">The upper threshold (exclusive).</param>
        public void RemoveRangeFromTo(T low, T hi)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            int count = CountFromTo(low, hi);

            if (count == 0)
            {
                return;
            }

            CircularQueue<T>? wasRemoved = (ActiveEvents & EventType.Removed) != 0 ? new CircularQueue<T>() : null;
            for (int i = 0; i < count; i++)
            {
                T item = Predecessor(hi);
                RemoveIterative(ref item, out _);
                if (wasRemoved != null)
                {
                    wasRemoved.Enqueue(item);
                }
            }
            if (wasRemoved != null)
            {
                RaiseForRemoveAll(wasRemoved);
            }
            else if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }


        /// <summary>
        /// Remove all items of this collection below a supplied threshold.
        /// </summary>
        /// <param name="hi">The upper threshold (exclusive).</param>
        public void RemoveRangeTo(T hi)
        {
            if (!isValid)
            {
                throw new ViewDisposedException("Snapshot has been disposed");
            }

            UpdateCheck();

            int count = CountTo(hi);

            if (count == 0)
            {
                return;
            }

            StackCheck();
            CircularQueue<T>? wasRemoved = (ActiveEvents & EventType.Removed) != 0 ? new CircularQueue<T>() : null;

            for (int i = 0; i < count; i++)
            {
                T item = DeleteMinInner();
                if (wasRemoved != null)
                {
                    wasRemoved.Enqueue(item);
                }
            }
            if (wasRemoved != null)
            {
                RaiseForRemoveAll(wasRemoved);
            }
            else if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }

        #endregion

        #region IPersistent<T> Members
        private int MaxSnapId => snapList == null ? -1 : FindLastLiveSnapShot();

        private int FindLastLiveSnapShot()
        {
            if (snapList == null)
            {
                return -1;
            }

            SnapRef? lastLiveSnapRef = snapList.Prev;
            object? _snapshot = null;
            while (lastLiveSnapRef != null && (_snapshot = lastLiveSnapRef.Tree.Target) == null)
            {
                lastLiveSnapRef = lastLiveSnapRef.Prev;
            }

            if (lastLiveSnapRef == null)
            {
                snapList = null;
                return -1;
            }
            if (snapList.Prev != lastLiveSnapRef)
            {
                snapList.Prev = lastLiveSnapRef;
                lastLiveSnapRef.Next = snapList;
            }
            return ((TreeSet<T>)_snapshot!).generation;
        }

        [Serializable]
        private class SnapRef
        {
            public SnapRef? Prev, Next;
            public WeakReference Tree;
            public SnapRef(TreeSet<T> tree) { Tree = new WeakReference(tree); }
            public void Dispose()
            {
                Next!.Prev = Prev;
                if (Prev != null)
                {
                    Prev.Next = Next;
                }

                Next = Prev = null;
            }
        }

        /// <summary>
        /// If this tree is a snapshot, remove registration in base tree
        /// </summary>
        public void Dispose()
        {
            if (!isValid)
            {
                return;
            }

            if (isSnapShot)
            {
                snapList!.Dispose();
                SnapDispose();
            }
            else
            {
                if (snapList != null)
                {
                    SnapRef someSnapRef = snapList.Prev!;
                    while (someSnapRef != null)
                    {
                        if (someSnapRef.Tree.Target is TreeSet<T> lastsnap)
                        {
                            lastsnap.SnapDispose();
                        }

                        someSnapRef = someSnapRef.Prev!;
                    }
                }
                snapList = null;
                Clear();
            }

        }

        private void SnapDispose()
        {
            root = null;
            dirs = null;
            path = null;
            comparer = null;
            isValid = false;
            snapList = null;
        }

        /// <summary>
        /// Make a (read-only) snapshot of this collection.
        /// </summary>
        /// <returns>The snapshot.</returns>
        public ISorted<T> Snapshot()
        {
            if (isSnapShot)
            {
                throw new InvalidOperationException("Cannot snapshot a snapshot");
            }

            TreeSet<T> res = (TreeSet<T>)MemberwiseClone();
            SnapRef newSnapRef = new SnapRef(res);
            res.isReadOnlyBase = true;
            res.isSnapShot = true;
            res.snapList = newSnapRef;

            FindLastLiveSnapShot();
            if (snapList == null)
            {
                snapList = new SnapRef(this);
            }

            SnapRef lastLiveSnapRef = snapList.Prev!;

            newSnapRef.Prev = lastLiveSnapRef;
            if (lastLiveSnapRef != null)
            {
                lastLiveSnapRef.Next = newSnapRef;
            }

            newSnapRef.Next = snapList;
            snapList.Prev = newSnapRef;

            generation++;

            return res;
        }

        #endregion

        #region TreeSet.Range nested class

        [Serializable]
        internal class Range : DirectedCollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            private int stamp;
            private readonly int size;
            private readonly TreeSet<T> basis;

            private readonly T lowend, highend;

            private readonly bool haslowend, hashighend;
            private EnumerationDirection direction;


            public Range(TreeSet<T> basis, bool haslowend, T lowend, bool hashighend, T highend, EnumerationDirection direction)
            {
                this.basis = basis;
                stamp = basis.stamp;

                //lowind will be const; should we cache highind?
                this.lowend = lowend; //Inclusive
                this.highend = highend;//Exclusive
                this.haslowend = haslowend;
                this.hashighend = hashighend;
                this.direction = direction;
                if (!basis.isSnapShot)
                {
                    size = haslowend ?
                        (hashighend ? basis.CountFromTo(lowend, highend) : basis.CountFrom(lowend)) :
                        (hashighend ? basis.CountTo(highend) : basis.Count);
                }
            }

            #region IEnumerable<T> Members


            #region TreeSet.Range.Enumerator nested class

            [Serializable]
            internal class Enumerator : SCG.IEnumerator<T>
            {
                #region Private Fields
                private bool valid = false, ready = true;

                private SCG.IComparer<T>? comparer;

                private T current;


                private Node? cursor;

                private Node[]? path; // stack of nodes

                private int level = 0;

                private Range? range;

                private readonly bool forwards;

                #endregion
                public Enumerator(Range range)
                {
                    comparer = range.basis.comparer;
                    path = new Node[2 * range.basis.blackdepth];
                    this.range = range;
                    forwards = range.direction == EnumerationDirection.Forwards;
                    cursor = new Node();
                    if (forwards)
                    {
                        cursor.right = range.basis.root;
                    }
                    else
                    {
                        cursor.left = range.basis.root;
                    }

                    range.basis.ModifyCheck(range.stamp);
                }

                private int Compare(T i1, T i2)
                {
                    return comparer!.Compare(i1, i2);
                }

                /// <summary>
                /// Undefined if enumerator is not valid (MoveNext hash been called returning true)
                /// </summary>
                /// <value>The current value of the enumerator.</value>
                public T Current
                {
                    get
                    {
                        if (valid)
                        {
                            return current;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }


                //Maintain a stack of nodes that are roots of
                //subtrees not completely exported yet. Invariant:
                //The stack nodes together with their right subtrees
                //consists of exactly the items we have not passed
                //yet (the top of the stack holds current item).
                /// <summary>
                /// Move enumerator to next item in tree, or the first item if
                /// this is the first call to MoveNext. 
                /// <exception cref="CollectionModifiedException"/> if underlying tree was modified.
                /// </summary>
                /// <returns>True if enumerator is valid now</returns>
                public bool MoveNext()
                {
                    range!.basis.ModifyCheck(range.stamp);
                    if (!ready)
                    {
                        return false;
                    }

                    if (forwards)
                    {
                        if (!valid && range.haslowend)
                        {
                            cursor = cursor!.right;
                            while (cursor != null)
                            {
                                int comp = Compare(cursor.item, range.lowend);

                                if (comp > 0)
                                {
                                    path![level++] = cursor;
                                    cursor = range.basis.Left(cursor);

                                }
                                else if (comp < 0)
                                {
                                    cursor = range.basis.Right(cursor);

                                }
                                else
                                {
                                    path![level] = cursor;
                                    break;
                                }
                            }

                            if (cursor == null)
                            {
                                if (level == 0)
                                {
                                    return valid = ready = false;
                                }
                                else
                                {
                                    cursor = path![--level];
                                }
                            }
                        }
                        else if (range.basis.Right(cursor!) != null)
                        {
                            path![level] = cursor = range.basis.Right(cursor!);

                            Node next = range.basis.Left(cursor);

                            while (next != null)
                            {
                                path[++level] = cursor = next;
                                next = range.basis.Left(cursor);
                            }
                        }

                        else if (level == 0)
                        {
                            return valid = ready = false;
                        }
                        else
                        {
                            cursor = path![--level];
                        }

                        current = cursor.item;
                        if (range.hashighend && Compare(current, range.highend) >= 0)
                        {
                            return valid = ready = false;
                        }

                        return valid = true;
                    }
                    else
                    {
                        if (!valid && range.hashighend)
                        {
                            cursor = cursor!.left;
                            while (cursor != null)
                            {
                                int comp = Compare(cursor.item, range.highend);

                                if (comp < 0)
                                {
                                    path![level++] = cursor;
                                    cursor = range.basis.Right(cursor);

                                }
                                else
                                {
                                    cursor = range.basis.Left(cursor);

                                }
                            }

                            if (cursor == null)
                            {
                                if (level == 0)
                                {
                                    return valid = ready = false;
                                }
                                else
                                {
                                    cursor = path![--level];
                                }
                            }
                        }
                        else if (range.basis.Left(cursor!) != null)
                        {
                            path![level] = cursor = range.basis.Left(cursor!);

                            Node next = range.basis.Right(cursor);

                            while (next != null)
                            {
                                path[++level] = cursor = next;
                                next = range.basis.Right(cursor);
                            }
                        }

                        else if (level == 0)
                        {
                            return valid = ready = false;
                        }
                        else
                        {
                            cursor = path![--level];
                        }

                        current = cursor.item;
                        if (range.haslowend && Compare(current, range.lowend) < 0)
                        {
                            return valid = ready = false;
                        }

                        return valid = true;
                    }
                }


                public void Dispose()
                {
                    comparer = null;
                    current = default;
                    cursor = null;
                    path = null;
                    range = null;
                }

                #region IEnumerator Members

                object System.Collections.IEnumerator.Current => Current!;

                bool System.Collections.IEnumerator.MoveNext()
                {
                    return MoveNext();
                }

                void System.Collections.IEnumerator.Reset()
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            #endregion


            public override T Choose()
            {
                if (size == 0)
                {
                    throw new NoSuchItemException();
                }

                return lowend;
            }

            public override SCG.IEnumerator<T> GetEnumerator() { return new Enumerator(this); }


            public override EnumerationDirection Direction => direction;


            #endregion

            #region Utility

            /* bool inside(T item)
            {
                return (!haslowend || basis.comparer!.Compare(item, lowend) >= 0) && (!hashighend || basis.comparer!.Compare(item, highend) < 0);
            } */


            /* void checkstamp()
            {
                if (stamp < basis.stamp)
                    throw new CollectionModifiedException();
            } */


            //  void syncstamp() { stamp = basis.stamp; }

            #endregion

            public override IDirectedCollectionValue<T> Backwards()
            {
                Range b = (Range)MemberwiseClone();

                b.direction = direction == EnumerationDirection.Forwards ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
                return b;
            }


            IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

            public override bool IsReadOnly => true;

            public override bool IsEmpty => size == 0;

            public override int Count => size;

            //TODO: check that this is correct
            public override Speed CountSpeed => Speed.Constant;

        }

        #endregion

        #region Diagnostics
        /// <summary>
        /// Display this node on the console, and recursively its subnodes.
        /// </summary>
        /// <param name="n">Node to display</param>
        /// <param name="space">Indentation</param>
        private void MiniDump(Node? n, string space)
        {
            if (n == null)
            {
                //	System.Logger.Log(space + "null");
            }
            else
            {
                MiniDump(n.right, space + "  ");
                Logger.Log(string.Format("{0} {4} (size={1}, items={8}, h={2}, gen={3}, id={6}){7}", space + n.item,
 n.size,

 0,
 n.generation,
 n.red ? "RED" : "BLACK",
         0,
         0,

 n.lastgeneration == -1 ? "" : string.Format(" [extra: lg={0}, c={1}, i={2}]", n.lastgeneration, n.leftnode ? "L" : "R", n.oldref == null ? "()" : "" + n.oldref.item),


 1

));
                MiniDump(n.left, space + "  ");
            }
        }

        /// <summary>
        /// Print the tree structure to the console stdout.
        /// </summary>
        public void Dump() { Dump(""); }

        /// <summary>
        /// Print the tree structure to the console stdout.
        /// </summary>
        public void Dump(string msg)
        {
            Logger.Log(string.Format(">>>>>>>>>>>>>>>>>>> dump {0} (count={1}, blackdepth={2}, depth={3}, gen={4})", msg, size, blackdepth,
            0
            ,
 generation
));
            MiniDump(root, "");
            CheckInner();
            Logger.Log("<<<<<<<<<<<<<<<<<<<");
        }


        /// <summary>
        /// Display this tree on the console.
        /// </summary>
        /// <param name="msg">Identifying string of this call to dump</param>
        /// <param name="err">Extra (error)message to include</param>
        private void Dump(string msg, string err)
        {
            Logger.Log(string.Format(">>>>>>>>>>>>>>>>>>> dump {0} (count={1}, blackdepth={2}, depth={3}, gen={4})", msg, size, blackdepth,
            0
            ,
 generation
));
            MiniDump(root, ""); Logger.Log(err);
            Logger.Log("<<<<<<<<<<<<<<<<<<<");
        }


        /// <summary>
        /// Print warning m on logger if b is false.
        /// </summary>
        /// <param name="b">Condition that should hold</param>
        /// <param name="n">Place (used for id display)</param>
        /// <param name="m">Message</param>
        /// <returns>b</returns>
        private bool Massert(bool b, Node n, string m)
        {
            if (!b)
            {
                Logger.Log(string.Format("*** Node (item={0}, id={1}): {2}", n.item,
              0
              , m));
            }

            return b;
        }

        private bool RbMiniCheck(Node n, bool redp, out T min, out T max, out int blackheight)
        {//Red-Black invariant
            bool res = true;

            res = Massert(!(n.red && redp), n, "RED parent of RED node") && res;
            res = Massert(n.left == null || n.right != null || n.left.red, n, "Left child black, but right child empty") && res;
            res = Massert(n.right == null || n.left != null || n.right.red, n, "Right child black, but left child empty") && res;
            bool sb = n.size == (n.left == null ? 0 : n.left.size) + (n.right == null ? 0 : n.right.size) + 1;

            res = Massert(sb, n, "Bad size") && res;
            min = max = n.item;

            T otherext;
            int lbh = 0, rbh = 0;

            if (n.left != null)
            {
                res = RbMiniCheck(n.left, n.red, out min, out otherext, out lbh) && res;
                res = Massert(comparer!.Compare(n.item, otherext) > 0, n, "Value not > all left children") && res;
            }

            if (n.right != null)
            {
                res = RbMiniCheck(n.right, n.red, out otherext, out max, out rbh) && res;
                res = Massert(comparer!.Compare(n.item, otherext) < 0, n, "Value not < all right children") && res;
            }

            res = Massert(rbh == lbh, n, "Different blackheights of children") && res;
            blackheight = n.red ? rbh : rbh + 1;
            return res;
        }

        private bool RbMiniSnapCheck(Node n, out int size, out T min, out T max)
        {
            bool res = true;

            min = max = n.item;

            int lsz = 0, rsz = 0;
            T otherext;

            Node? child = (n.lastgeneration >= generation && n.leftnode) ? n.oldref : n.left;
            if (child != null)
            {
                res = RbMiniSnapCheck(child, out lsz, out min, out otherext) && res;
                res = Massert(comparer!.Compare(n.item, otherext) > 0, n, "Value not > all left children") && res;
            }


            child = (n.lastgeneration >= generation && !n.leftnode) ? n.oldref : n.right;
            if (child != null)
            {
                res = RbMiniSnapCheck(child, out rsz, out otherext, out max) && res;
                res = Massert(comparer!.Compare(n.item, otherext) < 0, n, "Value not < all right children") && res;
            }

            size = 1 + lsz + rsz;

            return res;
        }

        /// <summary>
        /// Checks red-black invariant. Dumps tree to console if bad
        /// </summary>
        /// <param name="name">Title of dump</param>
        /// <returns>false if invariant violation</returns>
        public bool Check(string name)
        {
            System.Text.StringBuilder e = new System.Text.StringBuilder();

            if (!CheckInner())
            {
                return true;
            }
            else
            {
                Dump(name, e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Checks red-black invariant. Dumps tree to console if bad
        /// </summary>
        /// <returns>false if invariant violation</returns>
        public bool Check()
        {
            //return check("");
            //Logger.Log("bamse");
            if (!isValid)
            {
                return true;
            }

            return Check("-");
        }

        private bool CheckInner()
        {
            if (root != null)
            {
                if (isSnapShot)
                {
                    //Logger.Log("Im'a snapshot");
                    bool rv = RbMiniSnapCheck(root, out int thesize, out _, out _);

                    rv = Massert(size == thesize, root, "bad snapshot size") && rv;
                    return !rv;
                }
                bool res = RbMiniCheck(root, false, out _, out _, out int blackheight);
                res = Massert(blackheight == blackdepth, root, "bad blackh/d") && res;
                res = Massert(!root.red, root, "root is red") && res;
                res = Massert(root.size == size, root, "count!=root.size") && res;
                return !res;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}

