// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A list collection class based on a doubly linked list data structure.
    /// </summary>
    [Serializable]
    public class HashedLinkedList<T> : SequencedBase<T>, IList<T>, SCG.IList<T>
    {
        #region Fields
        /// <summary>
        /// IExtensible.Add(T) always does AddLast(T), fIFO determines 
        /// if T Remove() does RemoveFirst() or RemoveLast()
        /// </summary>
        private bool fIFO = true;

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public override EventType ListenableEvents => underlying == null ? EventType.All : EventType.None;

        #endregion

        //Invariant:  startsentinel != null && endsentinel != null
        //If size==0: startsentinel.next == endsentinel && endsentinel.prev == startsentinel
        //Else:      startsentinel.next == First && endsentinel.prev == Last)
        /// <summary>
        /// Node to the left of first node 
        /// </summary>
        private Node? startsentinel;

        /// <summary>
        /// Node to the right of last node
        /// </summary>
        private Node? endsentinel;

        /// <summary>
        /// Offset of this view in underlying list
        /// </summary>
        private int? offset;

        /// <summary>
        /// underlying list of this view (or null for the underlying list)
        /// </summary>
        private HashedLinkedList<T>? underlying;

        //Note: all views will have the same views list since all view objects are created by MemberwiseClone()
        private WeakViewList<HashedLinkedList<T>>? views;
        private WeakViewList<HashedLinkedList<T>>.Node? myWeakReference;

        /// <summary>
        /// Has this list or view not been invalidated by some operation (by someone calling Dispose())
        /// </summary>
        private bool isValid = true;
        private readonly HashDictionary<T, Node> dict;

        /// <summary>
        /// Number of taggroups
        /// </summary>
        private int taggroups;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        private int Taggroups
        {
            get => underlying == null ? taggroups : underlying.taggroups;
            set
            {
                if (underlying == null)
                {
                    taggroups = value;
                }
                else
                {
                    underlying.taggroups = value;
                }
            }
        }

        #endregion

        #region Util

        // bool equals(T i1, T i2) { return itemequalityComparer.Equals(i1, i2); }

        #region Check utilities
        /// <summary>
        /// Check if it is valid to perform updates and increment stamp of 
        /// underlying if this is a view.
        /// <para>This method should be called in every public modifying 
        /// methods before any modifications are performed.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException"> if check fails.</exception>
        protected override void UpdateCheck()
        {
            Validitycheck();
            base.UpdateCheck();
            if (underlying != null)
            {
                underlying.stamp++;
            }
        }

        /// <summary>
        /// Check if we are a view that the underlying list has only been updated through us.
        /// <br/>
        /// This method should be called from enumerators etc to guard against 
        /// modification of the base collection.
        /// </summary>
        /// <exception cref="InvalidOperationException"> if check fails.</exception>
        private void Validitycheck()
        {
            if (!isValid)
            {
                throw new ViewDisposedException();
            }
        }

        /// <summary>
        /// Check that the list has not been updated since a particular time.
        /// </summary>
        /// <param name="stamp">The stamp indicating the time.</param>
        /// <exception cref="CollectionModifiedException"> if check fails.</exception>
        protected override void ModifyCheck(int stamp)
        {
            Validitycheck();
            if ((underlying != null ? underlying.stamp : this.stamp) != stamp)
            {
                throw new CollectionModifiedException();
            }
        }
        #endregion

        #region Searching
        private bool Contains(T item, out Node node)
        {
            if (dict.Find(ref item, out node))
            {
                return InsideView(node);
            }

            return false;
        }

        /// <summary>
        /// Search forwards from a node for a node with a particular item.
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <param name="node">On input, the node to start at. If item was found, the node found on output.</param>
        /// <param name="index">If node was found, the value will be the number of links followed higher than 
        /// the value on input. If item was not found, the value on output is undefined.</param>
        /// <returns>True if node was found.</returns>
        private bool Find(T item, ref Node node, ref int index)
        {
            while (node != endsentinel)
            {
                //if (item.Equals(node.item))
                if (itemequalityComparer.Equals(item, node!.item))
                {
                    return true;
                }

                index++;
                node = node.next!;
            }

            return false;
        }

        /*
        bool dnif(T item, ref Node node, ref int index)
        {
            while (node != startsentinel)
            {
                //if (item.Equals(node.item))
                if (itemequalityComparer.Equals(item, node.item))
                    return true;

                index--;
                node = node.prev!;
            }

            return false;
        }
        */

        private bool InsideView(Node node)
        {
            if (underlying == null)
            {
                return true;
            }

            return (startsentinel!.Precedes(node) && node.Precedes(endsentinel!));
        }

        #endregion

        #region Indexing
        /// <summary>
        /// Return the node at position pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Node Get(int pos)
        {
            if (pos < 0 || pos >= size)
            {
                throw new IndexOutOfRangeException();
            }
            else if (pos < size / 2)
            {              // Closer to front
                Node? node = startsentinel;

                for (int i = 0; i <= pos; i++)
                {
                    node = node!.next;
                }

                return node!;
            }
            else
            {                            // Closer to end
                Node? node = endsentinel;

                for (int i = size; i > pos; i--)
                {
                    node = node!.prev;
                }

                return node!;
            }
        }

        /// <summary>
        /// Find the distance from pos to the set given by positions. Return the
        /// signed distance as return value and as an out parameter, the
        /// array index of the nearest position. This is used for up to length 5 of
        /// positions, and we do not assume it is sorted. 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="positions"></param>
        /// <param name="nearest"></param>
        /// <returns></returns>
        private int Dist(int pos, out int nearest, int[] positions)
        {
            nearest = -1;
            int bestdist = int.MaxValue;
            int signeddist = bestdist;
            for (int i = 0; i < positions.Length; i++)
            {
                int thisdist = positions[i] - pos;
                if (thisdist >= 0 && thisdist < bestdist) { nearest = i; bestdist = thisdist; signeddist = thisdist; }
                if (thisdist < 0 && -thisdist < bestdist) { nearest = i; bestdist = -thisdist; signeddist = thisdist; }
            }
            return signeddist;
        }

        /// <summary>
        /// Find the node at position pos, given known positions of several nodes.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="positions"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Node Get(int pos, int[] positions, Node[] nodes)
        {
            int delta = Dist(pos, out int nearest, positions);
            Node node = nodes[nearest];
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    node = node.prev!;
                }
            }
            else
            {
                for (int i = 0; i > delta; i--)
                {
                    node = node.next!;
                }
            }

            return node!;
        }

        /// <summary>
        /// Get nodes at positions p1 and p2, given nodes at several positions.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="positions"></param>
        /// <param name="nodes"></param>
        private void GetPair(int p1, int p2, out Node n1, out Node n2, int[] positions, Node[] nodes)
        {
            int delta1 = Dist(p1, out int nearest1, positions), d1 = delta1 < 0 ? -delta1 : delta1;
            int delta2 = Dist(p2, out int nearest2, positions), d2 = delta2 < 0 ? -delta2 : delta2;

            if (d1 < d2)
            {
                n1 = Get(p1, positions, nodes);
                n2 = Get(p2, new int[] { positions[nearest2], p1 }, new Node[] { nodes[nearest2], n1 });
            }
            else
            {
                n2 = Get(p2, positions, nodes);
                n1 = Get(p1, new int[] { positions[nearest1], p2 }, new Node[] { nodes[nearest1], n2 });
            }
        }
        #endregion

        #region Insertion

        private void Insert(Node succ, T item)
        {
            Node newnode = new Node(item);
            if (dict.FindOrAdd(item, ref newnode))
            {
                throw new DuplicateNotAllowedException("Item already in indexed list");
            }

            InsertNode(true, succ, newnode);
        }

        /// <summary>
        /// Insert a Node before another one. Unchecked version. 
        /// </summary>
        /// <param name="succ">The successor to be</param>
        /// <param name="newnode">Node to insert</param>
        /// <param name="updateViews">update overlapping view in this call</param>
        private void InsertNode(bool updateViews, Node succ, Node newnode)
        {
            newnode.next = succ;
            Node pred = newnode.prev = succ.prev!;
            succ.prev!.next = newnode;
            succ.prev = newnode;
            size++;
            if (underlying != null)
            {
                underlying.size++;
            }

            SetTag(newnode);
            if (updateViews)
            {
                FixViewsAfterInsert(succ, pred!, 1);
            }
        }

        #endregion

        #region Removal
        private T Remove(Node node)
        {
            FixViewsBeforeSingleRemove(node); //, Offset + index);
            node.prev!.next = node.next;
            node.next!.prev = node.prev;
            size--;
            if (underlying != null)
            {
                underlying.size--;
            }

            RemoveFromTagGroup(node);

            return node.item;
        }

        private bool DictRemove(T item, out Node node)
        {
            if (underlying == null)
            {
                if (!dict.Remove(item, out node))
                {
                    return false;
                }
            }
            else
            {
                //We cannot avoid calling dict twice - have to intersperse the listorder test!
                if (!Contains(item, out node))
                {
                    return false;
                }

                dict.Remove(item);
            }
            return true;
        }
        #endregion

        #region fixView utilities
        /// <summary>
        /// 
        /// </summary>
        /// <param name="added">The actual number of inserted nodes</param>
        /// <param name="pred">The predecessor of the inserted nodes</param>
        /// <param name="succ">The successor of the added nodes</param>
        private void FixViewsAfterInsert(Node succ, Node pred, int added)
        {
            if (views != null)
            {
                foreach (HashedLinkedList<T> view in views)
                {
                    if (view != this)
                    {
                        if (pred.Precedes(view.startsentinel!) || (view.startsentinel == pred && view.size > 0))
                        {
                            view.offset += added;
                        }

                        if (view.startsentinel!.Precedes(pred) && succ.Precedes(view.endsentinel!))
                        {
                            view.size += added;
                        }

                        if (view.startsentinel == pred && view.size > 0)
                        {
                            view.startsentinel = succ.prev;
                        }

                        if (view.endsentinel == succ)
                        {
                            view.endsentinel = pred.next;
                        }
                    }
                }
            }
        }

        private void FixViewsBeforeSingleRemove(Node node)
        {
            if (views != null)
            {
                foreach (HashedLinkedList<T> view in views)
                {
                    if (view != this)
                    {
                        if (view.startsentinel!.Precedes(node) && node.Precedes(view.endsentinel!))
                        {
                            view.size--;
                        }

                        if (!view.startsentinel.Precedes(node))
                        {
                            view.offset--;
                        }

                        if (view.startsentinel == node)
                        {
                            view.startsentinel = node.prev;
                        }

                        if (view.endsentinel == node)
                        {
                            view.endsentinel = node.next;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherView"></param>
        /// <returns>The position of View(otherOffset, otherSize) wrt. this view</returns>
        private MutualViewPosition ViewPosition(HashedLinkedList<T> otherView)
        {
            Node otherstartsentinel = otherView.startsentinel!, otherendsentinel = otherView.endsentinel!,
              first = startsentinel!.next!, last = endsentinel!.prev!,
              otherfirst = otherstartsentinel.next!, otherlast = otherendsentinel.prev!;
            if (last.Precedes(otherfirst!) || otherlast.Precedes(first!))
            {
                return MutualViewPosition.NonOverlapping;
            }

            if (size == 0 || (otherstartsentinel.Precedes(first!) && last.Precedes(otherendsentinel)))
            {
                return MutualViewPosition.Contains;
            }

            if (otherView.size == 0 || (startsentinel.Precedes(otherfirst!) && otherlast.Precedes(endsentinel)))
            {
                return MutualViewPosition.ContainedIn;
            }

            return MutualViewPosition.Overlapping;

        }

        private void DisposeOverlappingViews(bool reverse)
        {
            if (views != null)
            {
                foreach (HashedLinkedList<T> view in views)
                {
                    if (view != this)
                    {
                        switch (ViewPosition(view))
                        {
                            case MutualViewPosition.ContainedIn:
                                if (reverse)
                                { }
                                else
                                {
                                    view.Dispose();
                                }

                                break;
                            case MutualViewPosition.Overlapping:
                                view.Dispose();
                                break;
                            case MutualViewPosition.Contains:
                            case MutualViewPosition.NonOverlapping:
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Create a linked list with en external item equalityComparer
        /// </summary>
        /// <param name="itemequalityComparer">The external equalitySCG.Comparer</param>
        public HashedLinkedList(SCG.IEqualityComparer<T> itemequalityComparer)
            : base(itemequalityComparer)
        {
            offset = 0;
            size = stamp = 0;
            startsentinel = new Node(default);
            endsentinel = new Node(default);
            startsentinel.next = endsentinel;
            endsentinel.prev = startsentinel;
            //It is important that the sentinels are different:
            startsentinel.taggroup = new TagGroup
            {
                tag = int.MinValue,
                count = 0
            };
            endsentinel.taggroup = new TagGroup
            {
                tag = int.MaxValue,
                count = 0
            };
            dict = new HashDictionary<T, Node>(itemequalityComparer);
        }

        /// <summary>
        /// Create a linked list with the natural item equalityComparer
        /// </summary>
        public HashedLinkedList() : this(EqualityComparer<T>.Default) { }

        #endregion

        #region Node nested class

        /// <summary>
        /// An individual cell in the linked list
        /// </summary>
        [Serializable]
        private class Node
        {
            public Node? prev;

            public Node? next;

            public T item;

            #region Tag support
            internal int tag;

            internal TagGroup taggroup;

            internal bool Precedes(Node that)
            {
                //Debug.Assert(taggroup != null, "taggroup field null");
                //Debug.Assert(that.taggroup != null, "that.taggroup field null");
                int t1 = taggroup.tag;
                int t2 = that.taggroup.tag;

                return t1 < t2 ? true : t1 > t2 ? false : tag < that.tag;
            }
            #endregion

            internal Node(T item) { this.item = item; }

            internal Node(T item, Node prev, Node? next)
            {
                this.item = item; this.prev = prev; this.next = next;
            }

            public override string ToString()
            {
                return string.Format("Node: (item={0}, tag={1})", item, tag);

            }
        }

        #endregion

        #region Taggroup nested class and tag maintenance utilities
        /// <summary>
        /// A group of nodes with the same high tag. Purpose is to be
        /// able to tell the sequence order of two nodes without having to scan through
        /// the list.
        /// </summary>
        [Serializable]
        private class TagGroup
        {
            internal int tag, count;

            internal Node? first, last;

            /// <summary>
            /// Pretty print a tag group
            /// </summary>
            /// <returns>Formatted tag group</returns>
            public override string ToString()
            { return string.Format("TagGroup(tag={0}, cnt={1}, fst={2}, lst={3})", tag, count, first, last); }
        }

        //Constants for tag maintenance
        private const int wordsize = 32;
        private const int lobits = 3;
        private const int hibits = lobits + 1;
        private const int losize = 1 << lobits;
        private const int hisize = 1 << hibits;

        // const int logwordsize = 5;

        private TagGroup GetTagGroup(Node pred, Node succ, out int lowbound, out int highbound)
        {
            TagGroup predgroup = pred.taggroup, succgroup = succ.taggroup;

            if (predgroup == succgroup)
            {
                lowbound = pred.tag + 1;
                highbound = succ.tag - 1;
                return predgroup;
            }
            else if (predgroup.first != null)
            {
                lowbound = pred.tag + 1;
                highbound = int.MaxValue;
                return predgroup;
            }
            else if (succgroup.first != null)
            {
                lowbound = int.MinValue;
                highbound = succ.tag - 1;
                return succgroup;
            }
            else
            {
                lowbound = int.MinValue;
                highbound = int.MaxValue;
                return new TagGroup();
            }
        }


        /// <summary>
        /// Put a tag on a node (already inserted in the list). Split taggroups and renumber as 
        /// necessary.
        /// </summary>
        /// <param name="node">The node to tag</param>
        private void SetTag(Node node)
        {
            Node pred = node.prev!, succ = node.next!;
            TagGroup predgroup = pred.taggroup, succgroup = succ.taggroup;

            if (predgroup == succgroup)
            {
                node.taggroup = predgroup;
                predgroup.count++;
                if (pred.tag + 1 == succ.tag)
                {
                    SplitTagGroup(predgroup);
                }
                else
                {
                    node.tag = (pred.tag + 1) / 2 + (succ.tag - 1) / 2;
                }
            }
            else if (predgroup.first != null)
            {
                node.taggroup = predgroup;
                predgroup.last = node;
                predgroup.count++;
                if (pred.tag == int.MaxValue)
                {
                    SplitTagGroup(predgroup);
                }
                else
                {
                    node.tag = pred.tag / 2 + int.MaxValue / 2 + 1;
                }
            }
            else if (succgroup.first != null)
            {
                node.taggroup = succgroup;
                succgroup.first = node;
                succgroup.count++;
                if (succ.tag == int.MinValue)
                {
                    SplitTagGroup(node.taggroup);
                }
                else
                {
                    node.tag = int.MinValue / 2 + (succ.tag - 1) / 2;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(Taggroups == 0);

                TagGroup newgroup = new TagGroup();

                Taggroups = 1;
                node.taggroup = newgroup;
                newgroup.first = newgroup.last = node;
                newgroup.count = 1;
                return;
            }
        }


        /// <summary>
        /// Remove a node from its taggroup.
        /// <br/> When this is called, node must already have been removed from the underlying list
        /// </summary>
        /// <param name="node">The node to remove</param>
        private void RemoveFromTagGroup(Node node)
        {

            TagGroup taggroup = node.taggroup;

            if (--taggroup.count == 0)
            {
                Taggroups--;
                return;
            }

            if (node == taggroup.first)
            {
                taggroup.first = node.next!;
            }

            if (node == taggroup.last)
            {
                taggroup.last = node.prev!;
            }

            //node.taggroup = null;
            if (taggroup.count != losize || Taggroups == 1)
            {
                return;
            }

            TagGroup otg;
            // bug20070911:
            Node neighbor;
            if ((neighbor = taggroup.first!.prev!) != startsentinel
                && (otg = neighbor.taggroup).count <= losize)
            {
                taggroup.first = otg.first;
            }
            else if ((neighbor = taggroup.last!.next!) != endsentinel
                     && (otg = neighbor.taggroup).count <= losize)
            {
                taggroup.last = otg.last;
            }
            else
            {
                return;
            }

            Node n = otg.first!;

            for (int i = 0, length = otg.count; i < length; i++)
            {
                n.taggroup = taggroup;
                n = n.next!;
            }

            taggroup.count += otg.count;
            Taggroups--;
            n = taggroup.first!;

            const int ofs = wordsize - hibits;

            for (int i = 0, count = taggroup.count; i < count; i++)
            {
                n.tag = (i - losize) << ofs; //(i-8)<<28 
                n = n.next!;
            }
        }


        /// <summary>
        /// Split a tag group to make rom for more tags.
        /// </summary>
        /// <param name="taggroup">The tag group</param>
        private void SplitTagGroup(TagGroup taggroup)
        {
            Node n = taggroup.first!;
            int ptgt = taggroup.first!.prev!.taggroup.tag;
            int ntgt = taggroup.last!.next!.taggroup.tag;

            System.Diagnostics.Debug.Assert(ptgt + 1 <= ntgt - 1);

            int ofs = wordsize - hibits;
            int newtgs = (taggroup.count - 1) / hisize;
            int tgtdelta = (int)((ntgt + 0.0 - ptgt) / (newtgs + 2)), tgtag = ptgt;

            tgtdelta = tgtdelta == 0 ? 1 : tgtdelta;
            for (int j = 0; j < newtgs; j++)
            {
                TagGroup newtaggroup = new TagGroup
                {
                    tag = (tgtag = tgtag >= ntgt - tgtdelta ? ntgt : tgtag + tgtdelta),
                    first = n,
                    count = hisize
                };
                for (int i = 0; i < hisize; i++)
                {
                    n.taggroup = newtaggroup;
                    n.tag = (i - losize) << ofs; //(i-8)<<28 
                    n = n.next!;
                }

                newtaggroup.last = n.prev!;
            }

            int rest = taggroup.count - hisize * newtgs;

            taggroup.first = n;
            taggroup.count = rest;
            taggroup.tag = (tgtag = tgtag >= ntgt - tgtdelta ? ntgt : tgtag + tgtdelta); ofs--;
            for (int i = 0; i < rest; i++)
            {
                n.tag = (i - hisize) << ofs; //(i-16)<<27 
                n = n.next!;
            }

            taggroup.last = n.prev!;
            Taggroups += newtgs;
            if (tgtag == ntgt)
            {
                RedistributeTagGroups(taggroup);
            }
        }


        private void RedistributeTagGroups(TagGroup taggroup)
        {
            TagGroup pred = taggroup, succ = taggroup, tmp;
            int bits = 1, count = 1;
            double limit = 1, bigt = Math.Pow(Taggroups, 1.0 / 30);//?????

            do
            {
                bits++;
                int lowmask = (1 << bits) - 1;
                int himask = ~lowmask;
                int target = taggroup.tag & himask;
                while ((tmp = pred.first!.prev!.taggroup).first != null && (tmp.tag & himask) == target)
                { count++; pred = tmp; }

                while ((tmp = succ.last!.next!.taggroup).last != null && (tmp.tag & himask) == target)
                { count++; succ = tmp; }

                limit *= bigt;
            } while (count > limit);

            //redistibute tags
            int lob = pred.first.prev.taggroup.tag, upb = succ.last.next.taggroup.tag;
            int delta = upb / (count + 1) - lob / (count + 1);

            System.Diagnostics.Debug.Assert(delta > 0);
            for (int i = 0; i < count; i++)
            {
                pred.tag = lob + (i + 1) * delta;
                pred = pred.last!.next!.taggroup;
            }
        }

        #endregion

        #region Position, PositionComparer and ViewHandler nested types
        [Serializable]
        private class PositionComparer : SCG.IComparer<Position>
        {
            private static PositionComparer _default;

            private PositionComparer() { }
            public static PositionComparer Default => _default ?? (_default = new PositionComparer());
            public int Compare(Position a, Position b)
            {
                return a.Endpoint == b.Endpoint ? 0 : a.Endpoint.Precedes(b.Endpoint) ? -1 : 1;

            }
        }

        /// <summary>
        /// During RemoveAll, we need to cache the original endpoint indices of views
        /// </summary>
        private struct Position
        {
            public HashedLinkedList<T>? View { get; }

            public bool Left { get; private set; }

            public Node Endpoint { get; }

            public Position(HashedLinkedList<T> view, bool left)
            {
                View = view;
                Left = left;
                Endpoint = (left ? view.startsentinel!.next : view.endsentinel!.prev)!;
            }

            public Position(Node node)
            {
                Endpoint = node;
                View = null;
                Left = false;
            }

        }

        //TODO: merge the two implementations using Position values as arguments
        /// <summary>
        /// Handle the update of (other) views during a multi-remove operation.
        /// </summary>
        private struct ViewHandler
        {
            private readonly ArrayList<Position>? leftEnds;
            private readonly ArrayList<Position>? rightEnds;
            private int leftEndIndex, rightEndIndex, leftEndIndex2, rightEndIndex2;
            internal readonly int viewCount;
            internal ViewHandler(HashedLinkedList<T> list)
            {
                leftEndIndex = rightEndIndex = leftEndIndex2 = rightEndIndex2 = viewCount = 0;
                leftEnds = rightEnds = null;
                if (list.views != null)
                {
                    foreach (HashedLinkedList<T> v in list.views)
                    {
                        if (v != list)
                        {
                            if (leftEnds == null || rightEnds == null)
                            {
                                leftEnds = new ArrayList<Position>();
                                rightEnds = new ArrayList<Position>();
                            }
                            leftEnds.Add(new Position(v, true));
                            rightEnds.Add(new Position(v, false));
                        }
                    }
                }

                if (leftEnds == null || rightEnds == null)
                {
                    return;
                }

                viewCount = leftEnds.Count;
                leftEnds.Sort(PositionComparer.Default);
                rightEnds.Sort(PositionComparer.Default);
            }
            internal void SkipEndpoints(int removed, Node n)
            {
                if (viewCount > 0)
                {
                    Position endpoint;
                    while (leftEndIndex < viewCount && ((endpoint = leftEnds![leftEndIndex]).Endpoint.prev!.Precedes(n)))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.offset -= removed;//TODO: extract offset.Value?
                        view.size += removed;
                        leftEndIndex++;
                    }
                    while (rightEndIndex < viewCount && (endpoint = rightEnds![rightEndIndex]).Endpoint.Precedes(n))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.size -= removed;
                        rightEndIndex++;
                    }
                }
                if (viewCount > 0)
                {
                    while (leftEndIndex2 < viewCount && (_ = leftEnds![leftEndIndex2]).Endpoint.prev!.Precedes(n))
                    {
                        leftEndIndex2++;
                    }

                    while (rightEndIndex2 < viewCount && (_ = rightEnds![rightEndIndex2]).Endpoint.next!.Precedes(n))
                    {
                        rightEndIndex2++;
                    }
                }
            }
            /// <summary>
            /// To be called with n pointing to the right of each node to be removed in a stretch. 
            /// And at the endsentinel. 
            /// 
            /// Update offset of a view whose left endpoint (has not already been handled and) is n or precedes n.
            /// I.e. startsentinel precedes n.
            /// Also update the size as a prelude to handling the right endpoint.
            /// 
            /// Update size of a view not already handled and whose right endpoint precedes n.
            /// </summary>
            /// <param name="removed">The number of nodes left of n to be removed</param>
            /// <param name="n"></param>
            internal void UpdateViewSizesAndCounts(int removed, Node n)
            {
                if (viewCount > 0)
                {
                    Position endpoint;
                    while (leftEndIndex < viewCount && ((endpoint = leftEnds![leftEndIndex]).Endpoint.prev!.Precedes(n)))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.offset -= removed; //TODO: fix use of offset
                        view.size += removed;
                        leftEndIndex++;
                    }
                    while (rightEndIndex < viewCount && (endpoint = rightEnds![rightEndIndex]).Endpoint.Precedes(n))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.size -= removed;
                        rightEndIndex++;
                    }
                }
            }
            /// <summary>
            /// To be called with n being the first not-to-be-removed node after a (stretch of) node(s) to be removed.
            /// 
            /// It will update the startsentinel of views (that have not been handled before and) 
            /// whose startsentinel precedes n, i.e. is to be deleted.
            /// 
            /// It will update the endsentinel of views (...) whose endsentinel precedes n, i.e. is to be deleted.
            /// 
            /// PROBLEM: DOESNT WORK AS ORIGINALLY ADVERTISED. WE MUST DO THIS BEFORE WE ACTUALLY REMOVE THE NODES. WHEN THE 
            /// NODES HAVE BEEN REMOVED, THE precedes METHOD WILL NOT WORK!
            /// </summary>
            /// <param name="n"></param>
            /// <param name="newstart"></param>
            /// <param name="newend"></param>
            internal void UpdateSentinels(Node n, Node newstart, Node newend)
            {
                if (viewCount > 0)
                {
                    Position endpoint;
                    while (leftEndIndex2 < viewCount && (endpoint = leftEnds![leftEndIndex2]).Endpoint.prev!.Precedes(n))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.startsentinel = newstart;
                        leftEndIndex2++;
                    }
                    while (rightEndIndex2 < viewCount && (endpoint = rightEnds![rightEndIndex2]).Endpoint.next!.Precedes(n))
                    {
                        HashedLinkedList<T> view = endpoint.View!;
                        view.endsentinel = newend;
                        rightEndIndex2++;
                    }
                }
            }

        }
        #endregion

        #region Range nested class

        [Serializable]
        private class Range : DirectedCollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            // int start;
            private readonly int count, rangestamp;
            private readonly Node? startnode, endnode;
            private readonly HashedLinkedList<T> list;
            private bool forwards;


            internal Range(HashedLinkedList<T> list, int start, int count, bool forwards)
            {
                this.list = list;
                rangestamp = list.underlying != null ? list.underlying.stamp : list.stamp;
                // this.start = start;
                this.count = count;
                this.forwards = forwards;
                if (count > 0)
                {
                    startnode = list.Get(start);
                    endnode = list.Get(start + count - 1);
                }
            }

            public override bool IsReadOnly => true;

            public override bool IsEmpty { get { list.ModifyCheck(rangestamp); return count == 0; } }

            public override int Count { get { list.ModifyCheck(rangestamp); return count; } }


            public override Speed CountSpeed { get { list.ModifyCheck(rangestamp); return Speed.Constant; } }


            public override T Choose()
            {
                list.ModifyCheck(rangestamp);
                if (count > 0)
                {
                    return startnode!.item;
                }

                throw new NoSuchItemException();
            }


            public override SCG.IEnumerator<T> GetEnumerator()
            {
                int togo = count;

                list.ModifyCheck(rangestamp);
                if (togo == 0)
                {
                    yield break;
                }

                Node cursor = (forwards ? startnode : endnode)!;

                yield return cursor.item;
                while (--togo > 0)
                {
                    cursor = (forwards ? cursor.next : cursor.prev)!;
                    list.ModifyCheck(rangestamp);
                    yield return cursor!.item;
                }
            }


            public override IDirectedCollectionValue<T> Backwards()
            {
                list.ModifyCheck(rangestamp);

                Range b = (Range)MemberwiseClone();

                b.forwards = !forwards;
                return b;
            }


            IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }


            public override EnumerationDirection Direction => forwards ? EnumerationDirection.Forwards : EnumerationDirection.Backwards;
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// Invalidate this list. If a view, just invalidate the view. 
        /// If not a view, invalidate the list and all views on it.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(false);
        }

        private void Dispose(bool disposingUnderlying)
        {
            if (isValid)
            {
                if (underlying != null)
                {
                    isValid = false;
                    if (!disposingUnderlying && views != null)
                    {
                        views.Remove(myWeakReference!);
                    }

                    endsentinel = null;
                    startsentinel = null;
                    underlying = null;
                    views = null;
                    myWeakReference = null;
                }
                else
                {
                    //isValid = false;
                    //endsentinel = null;
                    //startsentinel = null;
                    if (views != null)
                    {
                        foreach (HashedLinkedList<T> view in views)
                        {
                            view.Dispose(true);
                        }
                    }
                    //views = null;
                    Clear();
                }
            }
        }

        #endregion IDisposable stuff

        #region IList<T> Members

        /// <summary>
        /// </summary>
        /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
        /// <value>The first item in this list.</value>
        public virtual T First
        {
            get
            {
                Validitycheck();
                if (size == 0)
                {
                    throw new NoSuchItemException();
                }

                return startsentinel!.next!.item;
            }
        }


        /// <summary>
        /// </summary>
        /// <exception cref="NoSuchItemException"> if this list is empty.</exception>
        /// <value>The last item in this list.</value>
        public virtual T Last
        {
            get
            {
                Validitycheck();
                if (size == 0)
                {
                    throw new NoSuchItemException();
                }

                return endsentinel!.prev!.item;
            }
        }

        /// <summary>
        /// Since <code>Add(T item)</code> always add at the end of the list,
        /// this describes if list has FIFO or LIFO semantics.
        /// </summary>
        /// <value>True if the <code>Remove()</code> operation removes from the
        /// start of the list, false if it removes from the end. THe default for a new linked list is true.</value>
        public virtual bool FIFO
        {
            get { Validitycheck(); return fIFO; }
            set { UpdateCheck(); fIFO = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsFixedSize
        {
            get { Validitycheck(); return false; }
        }

        /// <summary>
        /// On this list, this indexer is read/write.
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <value>The i'th item of this list.</value>
        /// <param name="index">The index of the item to fetch or store.</param>
        public virtual T this[int index]
        {
            get { Validitycheck(); return Get(index).item; }
            set
            {
                UpdateCheck();
                Node n = Get(index);
                //
                T item = n.item;

                if (itemequalityComparer.Equals(value, item))
                {
                    n.item = value;
                    dict.Update(value, n);
                }
                else if (!dict.FindOrAdd(value, ref n))
                {
                    dict.Remove(item);
                    n.item = value;
                }
                else
                {
                    throw new ArgumentException("Item already in indexed list");
                } (underlying ?? this).RaiseForSetThis(index, value, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed IndexingSpeed => Speed.Linear;

        /// <summary>
        /// Insert an item at a specific index location in this list. 
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt; the size of the collection.</summary>
        /// <param name="i">The index at which to insert.</param>
        /// <param name="item">The item to insert.</param>
        public virtual void Insert(int i, T item)
        {
            UpdateCheck();
            Insert((i == size ? endsentinel : Get(i))!, item);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForInsert(i + Offset, item);
            }
        }

        /// <summary>
        /// Insert an item at the end of a compatible view, used as a pointer.
        /// <para>The <code>pointer</code> must be a view on the same list as
        /// <code>this</code> and the endpoint of <code>pointer</code> must be
        /// a valid insertion point of <code>this</code></para>
        /// </summary>
        /// <exception cref="IncompatibleViewException">If <code>pointer</code> 
        /// is not a view on the same list as <code>this</code></exception>
        /// <exception cref="IndexOutOfRangeException"><b>??????</b> if the endpoint of 
        ///  <code>pointer</code> is not inside <code>this</code></exception>
        /// <exception cref="DuplicateNotAllowedException"> if the list has
        /// <code>AllowsDuplicates==false</code> and the item is 
        /// already in the list.</exception>
        /// <param name="pointer"></param>
        /// <param name="item"></param>
        public void Insert(IList<T> pointer, T item)
        {
            UpdateCheck();
            if ((pointer == null) || ((pointer.Underlying ?? pointer) != (underlying ?? this)))
            {
                throw new IncompatibleViewException();
            }
#warning INEFFICIENT
            //TODO: make this efficient (the whole point of the method:
            //Do NOT use Insert, but insert the node at pointer.endsentinel, checking
            //via the ordering that this is a valid insertion point
            Insert(pointer.Offset + pointer.Count - Offset, item);
        }

        /// <summary>
        /// Insert into this list all items from an enumerable collection starting 
        /// at a particular index.
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt; the size of the collection.
        /// </summary>
        /// <param name="i">Index to start inserting at</param>
        /// <param name="items">Items to insert</param>
        public virtual void InsertAll(int i, SCG.IEnumerable<T> items)
        {
            InsertAll(i, items, true);
        }

        private void InsertAll(int i, SCG.IEnumerable<T> items, bool insertion)
        {
            UpdateCheck();
            Node succ, node, pred;
            int count = 0;
            succ = (i == size ? endsentinel : Get(i))!;
            pred = node = succ.prev!;
            TagGroup taggroup = GetTagGroup(node!, succ, out int thetag, out int taglimit);
            try
            {
                foreach (T item in items)
                {
                    Node tmp = new Node(item, node!, null);
                    if (!dict.FindOrAdd(item, ref tmp))
                    {
                        tmp.tag = thetag < taglimit ? ++thetag : thetag;
                        tmp.taggroup = taggroup;
                        node.next = tmp;
                        count++;
                        node = tmp;
                    }
                    else
                    {
                        throw new DuplicateNotAllowedException("Item already in indexed list");
                    }
                }
            }
            finally
            {
                if (count != 0)
                {
                    taggroup.count += count;
                    if (taggroup != pred.taggroup)
                    {
                        taggroup.first = pred.next!;
                    }

                    if (taggroup != succ.taggroup)
                    {
                        taggroup.last = node;
                    }

                    succ.prev = node;
                    node.next = succ;
                    if (node.tag == node.prev!.tag)
                    {
                        SplitTagGroup(taggroup);
                    }

                    size += count;
                    if (underlying != null)
                    {
                        underlying.size += count;
                    }

                    FixViewsAfterInsert(succ, pred, count);
                    RaiseForInsertAll(pred, i, count, insertion);
                }
            }

        }

        private void RaiseForInsertAll(Node node, int i, int added, bool insertion)
        {
            if (ActiveEvents != 0)
            {
                int index = Offset + i;
                if ((ActiveEvents & (EventType.Added | EventType.Inserted)) != 0)
                {
                    for (int j = index; j < index + added; j++)
                    {
#warning must we check stamps here?
                        node = node.next!;
                        T item = node!.item;
                        if (insertion)
                        {
                            RaiseItemInserted(item, j);
                        }

                        RaiseItemsAdded(item, 1);
                    }
                }

                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// Insert an item at the front of this list.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        public virtual void InsertFirst(T item)
        {
            UpdateCheck();
            Insert(startsentinel!.next!, item);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForInsert(0 + Offset, item);
            }
        }

        /// <summary>
        /// Insert an item at the back of this list.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        public virtual void InsertLast(T item)
        {
            UpdateCheck();
            Insert(endsentinel!, item);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForInsert(size - 1 + Offset, item);
            }
        }

        /// <summary>
        /// Create a new list consisting of the results of mapping all items of this
        /// list.
        /// </summary>
        /// <param name="mapper">The delegate defining the map.</param>
        /// <returns>The new list.</returns>
        public IList<V> Map<V>(Func<T, V> mapper)
        {
            Validitycheck();

            HashedLinkedList<V> retval = new HashedLinkedList<V>();
            return Map<V>(mapper, retval);
        }

        /// <summary>
        /// Create a new list consisting of the results of mapping all items of this
        /// list. The new list will use a specified equalityComparer for the item type.
        /// </summary>
        /// <typeparam name="V">The type of items of the new list</typeparam>
        /// <param name="mapper">The delegate defining the map.</param>
        /// <param name="equalityComparer">The equalityComparer to use for the new list</param>
        /// <returns>The new list.</returns>
        public IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer)
        {
            Validitycheck();

            HashedLinkedList<V> retval = new HashedLinkedList<V>(equalityComparer);
            return Map<V>(mapper, retval);
        }

        private IList<V> Map<V>(Func<T, V> mapper, HashedLinkedList<V> retval)
        {
            if (size == 0)
            {
                return retval;
            }

            int stamp = this.stamp;
            Node cursor = startsentinel!.next!;
            HashedLinkedList<V>.Node mcursor = retval.startsentinel!;

            double tagdelta = int.MaxValue / (size + 1.0);
            int count = 1;
            HashedLinkedList<V>.TagGroup taggroup = new HashedLinkedList<V>.TagGroup();
            retval.taggroups = 1;
            taggroup.count = size;
            while (cursor != endsentinel)
            {
                V v = mapper(cursor!.item);
                ModifyCheck(stamp);
                mcursor.next = new HashedLinkedList<V>.Node(v, mcursor, null);
                cursor = cursor.next!;
                mcursor = mcursor.next;
                retval.dict.Add(v, mcursor);
                mcursor.taggroup = taggroup;
                mcursor.tag = (int)(tagdelta * count++);
            }

            taggroup.first = retval.startsentinel!.next!;
            taggroup.last = mcursor;

            retval.endsentinel!.prev = mcursor;
            mcursor.next = retval.endsentinel;
            retval.size = size;
            return retval;
        }

        /// <summary>
        /// Remove one item from the list: from the front if <code>FIFO</code>
        /// is true, else from the back.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        public virtual T Remove()
        {
            UpdateCheck();
            if (size == 0)
            {
                throw new NoSuchItemException("List is empty");
            }

            T item = fIFO ? Remove(startsentinel!.next!) : Remove(endsentinel!.prev!);
            dict.Remove(item);
            (underlying ?? this).RaiseForRemove(item);
            return item;
        }

        /// <summary>
        /// Remove one item from the front of the list.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        public virtual T RemoveFirst()
        {
            UpdateCheck();
            if (size == 0)
            {
                throw new NoSuchItemException("List is empty");
            }

            T item = Remove(startsentinel!.next!);
            dict.Remove(item);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForRemoveAt(Offset, item);
            }

            return item;
        }

        /// <summary>
        /// Remove one item from the back of the list.
        /// <exception cref="NoSuchItemException"/> if this list is empty.
        /// </summary>
        /// <returns>The removed item.</returns>
        public virtual T RemoveLast()
        {
            UpdateCheck();
            if (size == 0)
            {
                throw new NoSuchItemException("List is empty");
            }

            T item = Remove(endsentinel!.prev!);
            dict.Remove(item);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForRemoveAt(size + Offset, item);
            }

            return item;
        }

        /// <summary>
        /// Create a list view on this list. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if the start or count is negative</exception>
        /// <exception cref="ArgumentException"> if the range does not fit within list.</exception>
        /// <param name="start">The index in this list of the start of the view.</param>
        /// <param name="count">The size of the view.</param>
        /// <returns>The new list view.</returns>
        public virtual IList<T>? View(int start, int count)
        {
            CheckRange(start, count);
            Validitycheck();
            if (views == null)
            {
                views = new WeakViewList<HashedLinkedList<T>>();
            }

            HashedLinkedList<T> retval = (HashedLinkedList<T>)MemberwiseClone();
            retval.underlying = underlying ?? (this);
            retval.offset = offset + start;
            retval.size = count;
            GetPair(start - 1, start + count, out retval.startsentinel, out retval.endsentinel,
                new int[] { -1, size }, new Node[] { startsentinel!, endsentinel! });
            //retval.startsentinel = start == 0 ? startsentinel : get(start - 1);
            //retval.endsentinel = start + count == size ? endsentinel : get(start + count);

            //TODO: for the purpose of Dispose, we need to retain a ref to the node
            retval.myWeakReference = views.Add(retval);
            return retval;
        }

        /// <summary>
        /// Create a list view on this list containing the (first) occurrence of a particular item. 
        /// </summary>
        /// <exception cref="ArgumentException"> if the item is not in this list.</exception>
        /// <param name="item">The item to find.</param>
        /// <returns>The new list view.</returns>
        public virtual IList<T>? ViewOf(T item)
        {
            Validitycheck();
            if (!Contains(item, out Node n))
            {
                return null;
            }

            HashedLinkedList<T> retval = (HashedLinkedList<T>)MemberwiseClone();
            retval.underlying = underlying ?? (this);
            retval.offset = null;
            retval.startsentinel = n.prev;
            retval.endsentinel = n.next;
            retval.size = 1;
            return retval;

        }

        /// <summary>
        /// Create a list view on this list containing the last occurrence of a particular item. 
        /// <exception cref="ArgumentException"/> if the item is not in this list.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The new list view.</returns>
        public virtual IList<T>? LastViewOf(T item)
        {
            return ViewOf(item);

        }

        /// <summary>
        /// Null if this list is not a view.
        /// </summary>
        /// <value>Underlying list for view.</value>
        public virtual IList<T>? Underlying { get { Validitycheck(); return underlying; } }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual bool IsValid => isValid;

        /// <summary>
        /// </summary>
        /// <value>Offset for this list view or 0 for a underlying list.</value>
        public virtual int Offset
        {
            get
            {
                Validitycheck();

                if (offset == null && underlying != null)
                {
                    //TODO: search from both ends simultaneously!
                    Node n = underlying.startsentinel!;
                    int i = 0;
                    while (n != startsentinel) { n = n.next!; i++; }
                    offset = i;
                }
                if (offset == null)
                {
                    return 0;
                }

                return offset.Value;
            }
        }

        /// <summary>
        /// Slide this list view along the underlying list.
        /// </summary>
        /// <exception cref="NotAViewException"> if this list is not a view.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> if the operation
        /// would bring either end of the view outside the underlying list.</exception>
        /// <param name="offset">The signed amount to slide: positive to slide
        /// towards the end.</param>
        public IList<T> Slide(int offset)
        {
            if (!TrySlide(offset, size))
            {
                throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        //TODO: more test cases
        /// <summary>
        /// Slide this list view along the underlying list, perhaps changing its size.
        /// </summary>
        /// <exception cref="NotAViewException"> if this list is not a view.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> if the operation
        /// would bring either end of the view outside the underlying list.</exception>
        /// <param name="offset">The signed amount to slide: positive to slide
        /// towards the end.</param>
        /// <param name="size">The new size of the view.</param>
        public IList<T> Slide(int offset, int size)
        {
            if (!TrySlide(offset, size))
            {
                throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual bool TrySlide(int offset) { return TrySlide(offset, size); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual bool TrySlide(int offset, int size)
        {
            UpdateCheck();
            if (underlying == null)
            {
                throw new NotAViewException("List not a view");
            }

            if (this.offset == null)
            {
                try
                {
                    GetPair(offset - 1, offset + size, out startsentinel, out endsentinel,
                        new int[] { -1, this.size }, new Node[] { startsentinel!, endsentinel! });
                    //TODO: maybe-update offset field
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            else
            {
                if (offset + this.offset < 0 || offset + this.offset + size > underlying.size)
                {
                    return false;
                }

                int oldoffset = (int)(this.offset);
                GetPair(offset - 1, offset + size, out startsentinel, out endsentinel,
                    new int[] { -oldoffset - 1, -1, this.size, underlying.size - oldoffset },
                    new Node[] { underlying.startsentinel!, startsentinel!, endsentinel!, underlying.endsentinel! });
            }
            this.size = size;
            this.offset += offset;
            return true;
        }


        //TODO: improve the complexity of the implementation
        /// <summary>
        /// 
        /// <para>Returns null if <code>otherView</code> is strictly to the left of this view</para>
        /// </summary>
        /// <param name="otherView"></param>
        /// <exception cref="IncompatibleViewException">If otherView does not have the same underlying list as this</exception>
        /// <returns></returns>
        public virtual IList<T>? Span(IList<T> otherView)
        {
            if ((otherView == null) || ((otherView.Underlying ?? otherView) != (underlying ?? this)))
            {
                throw new IncompatibleViewException();
            }

            if (otherView.Offset + otherView.Count - Offset < 0)
            {
                return null;
            }

            return (underlying ?? this).View(Offset, otherView.Offset + otherView.Count - Offset);
        }


        //Question: should we swap items or move nodes around?
        //The first seems much more efficient unless the items are value types 
        //with a large memory footprint.
        //(Swapping will do count*3/2 T assignments, linking around will do 
        // 4*count ref assignments; note that ref assignments are more expensive 
        //than copying non-ref bits)
        /// <summary>
        /// Reverse the list so the items are in the opposite sequence order.
        /// </summary>
        public virtual void Reverse()
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            Position[]? positions = null;
            int poslow = 0, poshigh = 0;
            if (views != null)
            {
                CircularQueue<Position>? _positions = null;
                foreach (HashedLinkedList<T> view in views)
                {
                    if (view != this)
                    {
                        switch (ViewPosition(view))
                        {
                            case MutualViewPosition.ContainedIn:
                                (_positions ?? (_positions = new CircularQueue<Position>())).Enqueue(new Position(view, true));
                                _positions.Enqueue(new Position(view, false));
                                break;
                            case MutualViewPosition.Overlapping:
                                view.Dispose();
                                break;
                            case MutualViewPosition.Contains:
                            case MutualViewPosition.NonOverlapping:
                                break;
                        }
                    }
                }
                if (_positions != null)
                {
                    positions = _positions.ToArray();
                    Sorting.IntroSort<Position>(positions, 0, positions.Length, PositionComparer.Default);
                    poshigh = positions.Length - 1;
                }
            }

            Node a = Get(0), b = Get(size - 1);
            for (int i = 0; i < size / 2; i++)
            {
                T swap;
                swap = a.item; a.item = b.item; b.item = swap;
                dict[a.item] = a; dict[b.item] = b;

                if (positions != null)
                {
                    MirrorViewSentinelsForReverse(positions, ref poslow, ref poshigh, a, b, i);
                }

                a = a.next!; b = b.prev!;
            }
            if (positions != null && size % 2 != 0)
            {
                MirrorViewSentinelsForReverse(positions, ref poslow, ref poshigh, a, b, size / 2);
            } (underlying ?? this).RaiseCollectionChanged();
        }

        private void MirrorViewSentinelsForReverse(Position[] positions, ref int poslow, ref int poshigh, Node a, Node b, int i)
        {
            int? aindex = offset + i, bindex = offset + size - 1 - i;

            Position pos;

            while (poslow <= poshigh && (pos = positions[poslow]).Endpoint == a)
            {
                //TODO: Note: in the case og hashed linked list, if this.offset == null, but pos.View.offset!=null
                //we may at this point compute this.offset and non-null values of aindex and bindex
                if (pos.Left)
                {
                    pos.View!.endsentinel = b.next;
                }
                else
                {
                    pos.View!.startsentinel = b.prev;
                    pos.View.offset = bindex;
                }
                poslow++;
            }
            while (poslow < poshigh && (pos = positions[poshigh]).Endpoint == b)
            {
                if (pos.Left)
                {
                    pos.View!.endsentinel = a.next;
                }
                else
                {
                    pos.View!.startsentinel = a.prev;
                    pos.View.offset = aindex;
                }
                poshigh--;
            }
        }

        /// <summary>
        /// Check if this list is sorted according to the default sorting order
        /// for the item type T, as defined by the <see cref="T:C5.Comparer`1"/> class 
        /// </summary>
        /// <exception cref="NotComparableException">if T is not comparable</exception>
        /// <returns>True if the list is sorted, else false.</returns>
        public bool IsSorted() { return IsSorted(SCG.Comparer<T>.Default); }

        /// <summary>
        /// Check if this list is sorted according to a specific sorting order.
        /// </summary>
        /// <param name="c">The comparer defining the sorting order.</param>
        /// <returns>True if the list is sorted, else false.</returns>
        public virtual bool IsSorted(SCG.IComparer<T> c)
        {
            Validitycheck();
            if (size <= 1)
            {
                return true;
            }

            Node node = startsentinel!.next!;
            T prevItem = node.item;

            node = node.next!;
            while (node != endsentinel)
            {
                if (c.Compare(prevItem, node!.item) > 0)
                {
                    return false;
                }
                else
                {
                    prevItem = node.item;
                    node = node.next!;
                }
            }

            return true;
        }

        /// <summary>
        /// Sort the items of the list according to the default sorting order
        /// for the item type T, as defined by the Comparer[T] class. 
        /// (<see cref="T:C5.Comparer`1"/>).
        /// The sorting is stable.
        /// </summary>
        /// <exception cref="InvalidOperationException">if T is not comparable</exception>
        public virtual void Sort() { Sort(SCG.Comparer<T>.Default); }

        // Sort the linked list using mergesort
        /// <summary>
        /// Sort the items of the list according to a specific sorting order.
        /// The sorting is stable.
        /// </summary>
        /// <param name="c">The comparer defining the sorting order.</param>
        public virtual void Sort(SCG.IComparer<T> c)
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            DisposeOverlappingViews(false);
            if (underlying != null)
            {
                Node cursor = startsentinel!.next!;
                while (cursor != endsentinel)
                {
                    cursor.taggroup.count--;
                    cursor = cursor.next!;
                }
            }
            // Build a linked list of non-empty runs.
            // The prev field in first node of a run points to next run's first node
            Node runTail = startsentinel!.next!;
            Node prevNode = startsentinel.next!;

            endsentinel!.prev!.next = null;
            while (prevNode != null)
            {
                Node node = prevNode.next!;

                while (node != null && c.Compare(prevNode.item, node.item) <= 0)
                {
                    prevNode = node;
                    node = prevNode.next!;
                }

                // Completed a run; prevNode is the last node of that run
                prevNode.next = null;	// Finish the run
                runTail.prev = node;	// Link it into the chain of runs
                runTail = node!;
                if (c.Compare(endsentinel.prev.item, prevNode.item) <= 0)
                {
                    endsentinel.prev = prevNode;   // Update last pointer to point to largest
                }

                prevNode = node!;		// Start a new run
            }

            // Repeatedly merge runs two and two, until only one run remains
            while (startsentinel.next!.prev != null)
            {
                Node run = startsentinel.next;
                Node? newRunTail = null;

                while (run != null && run.prev != null)
                { // At least two runs, merge
                    Node nextRun = run.prev.prev!;
                    Node newrun = MergeRuns(run, run.prev, c);

                    if (newRunTail != null)
                    {
                        newRunTail.prev = newrun;
                    }
                    else
                    {
                        startsentinel.next = newrun;
                    }

                    newRunTail = newrun;
                    run = nextRun!;
                }

                if (run != null) // Add the last run, if any
                {
                    newRunTail!.prev = run;
                }
            }

            endsentinel.prev.next = endsentinel;
            startsentinel.next.prev = startsentinel;

            //assert invariant();
            //assert isSorted();
            {
                Node cursor = startsentinel.next, end = endsentinel;
                TagGroup t = GetTagGroup(startsentinel, endsentinel, out int tag, out int taglimit);
                int tagdelta = taglimit / (size + 1) - tag / (size + 1);
                tagdelta = tagdelta == 0 ? 1 : tagdelta;
                if (underlying == null)
                {
                    taggroups = 1;
                }

                while (cursor != end)
                {
                    tag = tag + tagdelta > taglimit ? taglimit : tag + tagdelta;
                    cursor.tag = tag;
                    t.count++;
                    cursor.taggroup = t;
                    cursor = cursor.next!;
                }
                if (t != startsentinel.taggroup)
                {
                    t.first = startsentinel.next;
                }

                if (t != endsentinel.taggroup)
                {
                    t.last = endsentinel.prev;
                }

                if (tag == taglimit)
                {
                    SplitTagGroup(t);
                }
            }
            (underlying ?? this).RaiseCollectionChanged();
        }

        private static Node MergeRuns(Node run1, Node run2, SCG.IComparer<T> c)
        {
            //assert run1 != null && run2 != null;
            Node prev;
            bool prev1;	// is prev from run1?

            if (c.Compare(run1.item, run2.item) <= 0)
            {
                prev = run1;
                prev1 = true;
                run1 = run1.next!;
            }
            else
            {
                prev = run2;
                prev1 = false;
                run2 = run2.next!;
            }

            Node start = prev;

            //assert start != null;
            start.prev = null;
            while (run1 != null && run2 != null)
            {
                if (prev1)
                {
                    //assert prev.next == run1;
                    //Comparable run2item = (Comparable)run2.item;
                    while (run1 != null && c.Compare(run2.item, run1.item) >= 0)
                    {
                        prev = run1;
                        run1 = prev.next!;
                    }

                    if (run1 != null)
                    { // prev.item <= run2.item < run1.item; insert run2
                        prev.next = run2;
                        run2.prev = prev;
                        prev = run2;
                        run2 = prev.next!;
                        prev1 = false;
                    }
                }
                else
                {
                    //assert prev.next == run2;
                    //Comparable run1item = (Comparable)run1.item;
                    while (run2 != null && c.Compare(run1.item, run2.item) > 0)
                    {
                        prev = run2;
                        run2 = prev.next!;
                    }

                    if (run2 != null)
                    { // prev.item < run1.item <= run2.item; insert run1
                        prev.next = run1;
                        run1.prev = prev;
                        prev = run1;
                        run1 = prev.next!;
                        prev1 = true;
                    }
                }
            }

            //assert !(run1 != null && prev1) && !(run2 != null && !prev1);
            if (run1 != null)
            { // last run2 < all of run1; attach run1 at end
                prev.next = run1;
                run1.prev = prev;
            }
            else if (run2 != null)
            { // last run1 
                prev.next = run2;
                run2.prev = prev;
            }

            return start;
        }

        /// <summary>
        /// Randomly shuffle the items of this list. 
        /// <para>Will invalidate overlapping views???</para>
        /// </summary>
        public virtual void Shuffle() { Shuffle(new C5Random()); }


        /// <summary>
        /// Shuffle the items of this list according to a specific random source.
        /// <para>Will invalidate overlapping views???</para>
        /// </summary>
        /// <param name="rnd">The random source.</param>
        public virtual void Shuffle(Random rnd)
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            DisposeOverlappingViews(false);
            ArrayList<T> a = new ArrayList<T>();
            a.AddAll(this);
            a.Shuffle(rnd);
            Node cursor = startsentinel!.next!;
            int j = 0;
            while (cursor != endsentinel)
            {
                cursor.item = a[j++];
                dict[cursor.item] = cursor;
                cursor = cursor.next!;
            }
            (underlying ?? this).RaiseCollectionChanged();
        }

        #endregion

        #region IIndexed<T> Members

        /// <summary>
        /// <exception cref="IndexOutOfRangeException"/>.
        /// </summary>
        /// <value>The directed collection of items in a specific index interval.</value>
        /// <param name="start">The low index of the interval (inclusive).</param>
        /// <param name="count">The size of the range.</param>
        public IDirectedCollectionValue<T> this[int start, int count]
        {
            get
            {
                Validitycheck();
                CheckRange(start, count);
                return new Range(this, start, count, true);
            }
        }

        /// <summary>
        /// Searches for an item in the list going forwards from the start.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of item from start.</returns>
        public virtual int IndexOf(T item)
        {
            Validitycheck();
            if (!dict.Find(ref item, out Node node) || !InsideView(node))
            {
                return ~size;
            }

            node = startsentinel!.next!;
            int index = 0;
            if (Find(item, ref node, ref index))
            {
                return index;
            }
            else
            {
                return ~size;
            }
        }

        /// <summary>
        /// Searches for an item in the list going backwards from the end.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Index of of item from the end.</returns>
        public virtual int LastIndexOf(T item)
        {
            return IndexOf(item);

        }

        /// <summary>
        /// Remove the item at a specific position of the list.
        /// <exception cref="IndexOutOfRangeException"/> if i is negative or
        /// &gt;= the size of the collection.
        /// </summary>
        /// <param name="i">The index of the item to remove.</param>
        /// <returns>The removed item.</returns>
        public virtual T RemoveAt(int i)
        {
            UpdateCheck();
            T retval = Remove(Get(i));
            dict.Remove(retval);
            if (ActiveEvents != EventType.None)
            {
                (underlying ?? this).RaiseForRemoveAt(Offset + i, retval);
            }

            return retval;
        }

        /// <summary>
        /// Remove all items in an index interval.
        /// <exception cref="IndexOutOfRangeException"/>???. 
        /// </summary>
        /// <param name="start">The index of the first item to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        public virtual void RemoveInterval(int start, int count)
        {
            UpdateCheck();
            CheckRange(start, count);
            if (count == 0)
            {
                return;
            }

            View(start, count)!.Clear();

        }

        private void RaiseForRemoveInterval(int start, int count)
        {
            if (ActiveEvents != 0)
            {
                RaiseCollectionCleared(size == 0, count, start);
                RaiseCollectionChanged();
            }
        }
        #endregion

        #region ISequenced<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetSequencedHashCode() { Validitycheck(); return base.GetSequencedHashCode(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public override bool SequencedEquals(ISequenced<T> that) { Validitycheck(); return base.SequencedEquals(that); }

        #endregion

        #region IDirectedCollection<T> Members

        /// <summary>
        /// Create a collection containing the same items as this collection, but
        /// whose enumerator will enumerate the items backwards. The new collection
        /// will become invalid if the original is modified. Method typically used as in
        /// <code>foreach (T x in coll.Backwards()) {...}</code>
        /// </summary>
        /// <returns>The backwards collection.</returns>
        public override IDirectedCollectionValue<T> Backwards()
        { return this[0, size].Backwards(); }

        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

        #endregion

        #region IEditableCollection<T> Members

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>Speed.Linear</value>
        public virtual Speed ContainsSpeed => Speed.Constant;

        /// <summary>
        /// Performs a check for view validity before calling base.GetUnsequencedHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetUnsequencedHashCode()
        { Validitycheck(); return base.GetUnsequencedHashCode(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public override bool UnsequencedEquals(ICollection<T> that)
        { Validitycheck(); return base.UnsequencedEquals(that); }

        /// <summary>
        /// Check if this collection contains (an item equivalent to according to the
        /// itemequalityComparer) a particular value.
        /// </summary>
        /// <param name="item">The value to check for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public virtual bool Contains(T item)
        {
            Validitycheck();
            return Contains(item, out _);
        }

        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, return in the ref argument (a
        /// binary copy of) the actual value found.
        /// </summary>
        /// <param name="item">The value to look for.</param>
        /// <returns>True if the items is in this collection.</returns>
        public virtual bool Find(ref T item)
        {
            Validitycheck();
            if (Contains(item, out Node node)) { item = node.item; return true; }
            return false;
        }

        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// to with a binary copy of the supplied value. Will update a single item.
        /// </summary>
        /// <param name="item">Value to update.</param>
        /// <returns>True if the item was found and hence updated.</returns>
        public virtual bool Update(T item) { return Update(item, out _); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public virtual bool Update(T item, out T olditem)
        {
            UpdateCheck();

            if (Contains(item, out Node node))
            {
                olditem = node.item;
                node.item = item;
                //Avoid clinging onto a reference to olditem via dict!
                dict.Update(item, node);
                (underlying ?? this).RaiseForUpdate(item, olditem);
                return true;
            }

            olditem = default;
            return false;
        }

        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, return in the ref argument (a
        /// binary copy of) the actual value found. Else, add the item to the collection.
        /// </summary>
        /// <param name="item">The value to look for.</param>
        /// <returns>True if the item was found (hence not added).</returns>
        public virtual bool FindOrAdd(ref T item)
        {
            UpdateCheck();
            //This is an extended myinsert:
            Node node = new Node(item);
            if (!dict.FindOrAdd(item, ref node))
            {
                InsertNode(true, endsentinel!, node);
                (underlying ?? this).RaiseForAdd(item);
                return false;
            }
            if (!InsideView(node))
            {
                throw new ArgumentException("Item alredy in indexed list but outside view");
            }

            item = node.item;
            return true;

        }

        /// <summary>
        /// Check if this collection contains an item equivalent according to the
        /// itemequalityComparer to a particular value. If so, update the item in the collection 
        /// to with a binary copy of the supplied value; else add the value to the collection. 
        /// </summary>
        /// <param name="item">Value to add or update.</param>
        /// <returns>True if the item was found and updated (hence not added).</returns>
        public virtual bool UpdateOrAdd(T item) { return UpdateOrAdd(item, out _); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public virtual bool UpdateOrAdd(T item, out T olditem)
        {
            UpdateCheck();
            Node node = new Node(item);
            //NOTE: it is hard to do this without double access to the dictionary
            //in the update case
            if (dict.FindOrAdd(item, ref node))
            {
                if (!InsideView(node))
                {
                    throw new ArgumentException("Item in indexed list but outside view");
                }

                olditem = node.item;
                //Avoid clinging onto a reference to olditem via dict!
                dict.Update(item, node);
                node.item = item;
                (underlying ?? this).RaiseForUpdate(item, olditem);
                return true;
            }
            InsertNode(true, endsentinel!, node);
            (underlying ?? this).RaiseForAdd(item);

            olditem = default;
            return false;
        }

        /// <summary>
        /// Remove a particular item from this collection. Since the collection has bag
        /// semantics only one copy equivalent to the supplied item is removed. 
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public virtual bool Remove(T item)
        {
            UpdateCheck();
            if (!DictRemove(item, out Node node))
            {
                return false;
            }

            T removeditem = Remove(node);
            (underlying ?? this).RaiseForRemove(removeditem);
            return true;
        }

        /// <summary>
        /// Remove a particular item from this collection if found (only one copy). 
        /// If an item was removed, report a binary copy of the actual item removed in 
        /// the argument.
        /// </summary>
        /// <param name="item">The value to remove on input.</param>
        /// <param name="removeditem">The value removed.</param>
        /// <returns>True if the item was found (and removed).</returns>
        public virtual bool Remove(T item, out T removeditem)
        {
            UpdateCheck();

            if (!DictRemove(item, out Node node))
            {
                removeditem = default;
                return false;
            }
            removeditem = node.item;
            Remove(node);
            (underlying ?? this).RaiseForRemove(removeditem);
            return true;
        }

        /// <summary>
        /// Remove all items in another collection from this one, taking multiplicities into account.
        /// <para>Always removes from the front of the list.
        /// </para>
        /// <para>The asymptotic running time complexity of this method is <code>O(n+m+v*log(v))</code>, 
        /// where <code>n</code> is the size of this list, <code>m</code> is the size of the
        /// <code>items</code> collection and <code>v</code> is the number of views. 
        /// The method will temporarily allocate memory of size <code>O(m+v)</code>.
        /// </para>
        /// </summary>
        /// <param name="items">The items to remove.</param>
        public virtual void RemoveAll(SCG.IEnumerable<T> items)
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
            bool mustFire = raiseHandler.MustFire;
            foreach (T item in items)
            {
                if (DictRemove(item, out Node node))
                {
                    if (mustFire)
                    {
                        raiseHandler.Remove(node.item);
                    }

                    Remove(node);
                }
            }

            raiseHandler.Raise();
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        void RemoveAll(Func<T, bool> predicate)
        {
            updatecheck();
            if (size == 0)
                return;
            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
            bool mustFire = raiseHandler.MustFire;
            {
                Node n = startsentinel!.next!;

                while (n != endsentinel)
                {
                    bool removeIt = predicate(n!.item);
                    updatecheck();
                    if (removeIt)
                    {
                        dict.Remove(n.item);
                        remove(n);
                        if (mustFire)
                            raiseHandler.Remove(n.item);
                    }

                    n = n.next!;
                }
            }

            raiseHandler.Raise();
        }
        */

        /// <summary>
        /// Remove all items from this collection.
        /// </summary>
        public virtual void Clear()
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            int oldsize = size;
            if (underlying == null)
            {
                dict.Clear();
            }
            else
            {
                foreach (T item in this)
                {
                    dict.Remove(item);
                }
            }

            ClearInnner();
            (underlying ?? this).RaiseForRemoveInterval(Offset, oldsize);
        }

        private void ClearInnner()
        {
            if (size == 0)
            {
                return;
            }

            //TODO: mix with tag maintenance to only run through list once?
            ViewHandler viewHandler = new ViewHandler(this);
            if (viewHandler.viewCount > 0)
            {
                int removed = 0;
                Node n = startsentinel!.next!;
                viewHandler.SkipEndpoints(0, n);
                while (n != endsentinel)
                {
                    removed++;
                    n = n.next!;
                    viewHandler.UpdateViewSizesAndCounts(removed, n!);
                }
                viewHandler.UpdateSentinels(endsentinel!, startsentinel, endsentinel!);
                if (underlying != null)
                {
                    viewHandler.UpdateViewSizesAndCounts(removed, underlying.endsentinel!);
                }
            }

            if (underlying != null)
            {
                Node n = startsentinel!.next!;

                while (n != endsentinel)
                {
                    n.next!.prev = startsentinel;
                    startsentinel.next = n.next;
                    RemoveFromTagGroup(n);
                    n = n.next;
                }
            }
            else
            {
                taggroups = 0;
            }

            endsentinel!.prev = startsentinel;
            startsentinel!.next = endsentinel;
            if (underlying != null)
            {
                underlying.size -= size;
            }

            size = 0;
        }

        /// <summary>
        /// Remove all items not in some other collection from this one, taking multiplicities into account.
        /// <para>The asymptotic running time complexity of this method is <code>O(n+m+v*log(v))</code>, 
        /// where <code>n</code> is the size of this collection, <code>m</code> is the size of the
        /// <code>items</code> collection and <code>v</code> is the number of views. 
        /// The method will temporarily allocate memory of size <code>O(m+v)</code>. The stated complexity 
        /// holds under the assumption that the itemequalityComparer of this list is well-behaved.
        /// </para>
        /// </summary>
        /// <param name="items">The items to retain.</param>
        public virtual void RetainAll(SCG.IEnumerable<T> items)
        {
            UpdateCheck();
            if (size == 0)
            {
                return;
            }

            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
            bool mustFire = raiseHandler.MustFire;
            /*if (underlying == null)
            {
              HashDictionary<T, Node> newdict = new HashDictionary<T, Node>(itemequalityComparer);
              foreach (T item in items)
              {
                Node node;

                if (dict.Remove(item, out node))
                  newdict.Add(item, node);
              }
              foreach (System.Collections.Generic.KeyValuePair<T, Node> pair in dict)
              {
                Node n = pair.Value;
                fixViewsBeforeSingleRemove(n, 117);
                Node p = n.prev, s = n.next; s.prev = p; p.next = s;
                removefromtaggroup(n);
              }
              dict = newdict;
              size = dict.Count;
              //For a small number of items to retain it might be faster to 
              //iterate through the list and splice out the chunks not needed
            }
            else*/
            {
                HashSet<T> toremove = new HashSet<T>(itemequalityComparer);

                foreach (T item in this)
                {
                    toremove.Add(item);
                }

                foreach (T item in items)
                {
                    toremove.Remove(item);
                }

                Node n = startsentinel!.next!;

                while (n != endsentinel && toremove.Count > 0)
                {
                    if (toremove.Contains(n.item))
                    {
                        dict.Remove(n.item);
                        Remove(n);
                        if (mustFire)
                        {
                            raiseHandler.Remove(n.item);
                        }
                    }

                    n = n.next!;
                }
            }

            raiseHandler.Raise();
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        void RetainAll(Func<T, bool> predicate)
        {
            updatecheck();
            if (size == 0)
                return;
            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(underlying ?? this);
            bool mustFire = raiseHandler.MustFire;
            {
                Node n = startsentinel!.next!;

                while (n != endsentinel)
                {
                    bool removeIt = !predicate(n!.item);
                    updatecheck();
                    if (removeIt)
                    {
                        dict.Remove(n.item);
                        remove(n);
                        if (mustFire)
                            raiseHandler.Remove(n.item);
                    }

                    n = n.next!;
                }
            }

            raiseHandler.Raise();
        } */

        /// <summary>
        /// Check if this collection contains all the values in another collection
        /// with respect to multiplicities.
        /// </summary>
        /// <param name="items">The </param>
        /// <returns>True if all values in <code>items</code>is in this collection.</returns>
        public virtual bool ContainsAll(SCG.IEnumerable<T> items)
        {
            Validitycheck();
            foreach (T item in items)
            {
                if (!Contains(item, out Node node))
                {
                    return false;
                }
            }

            return true;

        }


        /// <summary>
        /// Create a new list consisting of the items of this list satisfying a 
        /// certain predicate.
        /// </summary>
        /// <param name="filter">The filter delegate defining the predicate.</param>
        /// <returns>The new list.</returns>
        public IList<T> FindAll(Func<T, bool> filter)
        {
            Validitycheck();
            int stamp = this.stamp;
            HashedLinkedList<T> retval = new HashedLinkedList<T>();
            Node cursor = startsentinel!.next!;
            Node mcursor = retval.startsentinel!;
            double tagdelta = int.MaxValue / (size + 1.0);
            int count = 1;
            TagGroup taggroup = new TagGroup();
            retval.taggroups = 1;
            while (cursor != endsentinel)
            {
                bool found = filter(cursor.item);
                ModifyCheck(stamp);
                if (found)
                {
                    mcursor.next = new Node(cursor.item, mcursor, null);
                    mcursor = mcursor.next;
                    retval.size++;
                    retval.dict.Add(cursor.item, mcursor);
                    mcursor.taggroup = taggroup;
                    mcursor.tag = (int)(tagdelta * count++);
                }
                cursor = cursor.next!;
            }
            if (retval.size > 0)
            {
                taggroup.count = retval.size;
                taggroup.first = retval.startsentinel!.next!;
                taggroup.last = mcursor!;
            }
            retval.endsentinel!.prev = mcursor;
            mcursor.next = retval.endsentinel;
            return retval;
        }


        /// <summary>
        /// Count the number of items of the collection equal to a particular value.
        /// Returns 0 if and only if the value is not in the collection.
        /// </summary>
        /// <param name="item">The value to count.</param>
        /// <returns>The number of copies found.</returns>
        public virtual int ContainsCount(T item)
        {
            return Contains(item) ? 1 : 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<T> UniqueItems()
        {
            return this;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities()
        {
            return new MultiplicityOne<T>(this);

        }

        /// <summary>
        /// Remove all items equivalent to a given value.
        /// <para>The asymptotic complexity of this method is <code>O(n+v*log(v))</code>, 
        /// where <code>n</code> is the size of the collection and <code>v</code> 
        /// is the number of views.
        /// </para>
        /// </summary>
        /// <param name="item">The value to remove.</param>
        public virtual void RemoveAllCopies(T item)
        {
            Remove(item);

        }

        #endregion

        #region ICollectionValue<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value>The number of items in this collection</value>
        public override int Count { get { Validitycheck(); return size; } }

        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override T Choose() { return First; }

        /// <summary>
        /// Create an enumerable, enumerating the items of this collection that satisfies 
        /// a certain condition.
        /// </summary>
        /// <param name="filter">The T->bool filter delegate defining the condition</param>
        /// <returns>The filtered enumerable</returns>
        public override SCG.IEnumerable<T> Filter(Func<T, bool> filter) { Validitycheck(); return base.Filter(filter); }

        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Create an enumerator for the collection
        /// </summary>
        /// <returns>The enumerator</returns>
        public override SCG.IEnumerator<T> GetEnumerator()
        {
            Validitycheck();
            Node cursor = startsentinel!.next!;
            int enumeratorstamp = underlying != null ? underlying.stamp : stamp;

            while (cursor != endsentinel)
            {
                ModifyCheck(enumeratorstamp);
                yield return cursor!.item;
                cursor = cursor.next!;
            }
        }

        #endregion

        #region IExtensible<T> Members
        /// <summary>
        /// Add an item to this collection if possible. 
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True.</returns>
        public virtual bool Add(T item)
        {
            UpdateCheck();
            Node node = new Node(item);
            if (!dict.FindOrAdd(item, ref node))
            {
                InsertNode(true, endsentinel!, node);
                (underlying ?? this).RaiseForAdd(item);
                return true;
            }
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>True since this collection has bag semantics.</value>
        public virtual bool AllowsDuplicates => false;

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting => true;

        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. 
        /// </summary>
        /// <param name="items">The items to add</param>
        public virtual void AddAll(SCG.IEnumerable<T> items)
        {

            UpdateCheck();
            int added = 0;
            Node pred = endsentinel!.prev!;
            foreach (var item in items)
            {
                Node node = new Node(item);
                if (!dict.FindOrAdd(item, ref node))
                {
                    InsertNode(false, endsentinel, node);
                    added++;
                }
            }
            if (added > 0)
            {
                FixViewsAfterInsert(endsentinel, pred!, added);
                RaiseForInsertAll(pred!, size - added, added, false);
            }

        }

        #endregion



        #region Diagnostic

        private bool CheckViews()
        {
            if (underlying != null)
            {
                throw new InternalException("checkViews() called on a view");
            }

            if (views == null)
            {
                return true;
            }

            bool retval = true;

            Node[] nodes = new Node[size + 2];
            int i = 0;
            Node n = startsentinel!;
            while (n != null)
            {
                nodes[i++] = n;
                n = n.next!;
            }
            //Logger.Log("###");
            foreach (HashedLinkedList<T> view in views)
            {
                if (!view.isValid)
                {
                    Logger.Log(string.Format("Invalid view(hash {0}, offset {1}, size {2})",
                      view.GetHashCode(), view.offset, view.size));
                    retval = false;
                    continue;
                }
                if (view.Offset > size || view.Offset < 0)
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), Offset > underlying.size ({2})",
                      view.GetHashCode(), view.offset, view.size, size));
                    retval = false;
                }
                else if (view.startsentinel != nodes[view.Offset])
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), startsentinel {3} should be {4}",
                      view.GetHashCode(), view.offset, view.size,
                      view.startsentinel + " " + view.startsentinel!.GetHashCode(),
                      nodes[view.Offset] + " " + nodes[view.Offset].GetHashCode()));
                    retval = false;
                }
                if (view.Offset + view.size > size || view.Offset + view.size < 0)
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), end index > underlying.size ({3})",
                      view.GetHashCode(), view.offset, view.size, size));
                    retval = false;
                }
                else if (view.endsentinel != nodes[view.Offset + view.size + 1])
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), endsentinel {3} should be {4}",
                      view.GetHashCode(), view.offset, view.size,
                      view.endsentinel + " " + view.endsentinel!.GetHashCode(),
                      nodes[view.Offset + view.size + 1] + " " + nodes[view.Offset + view.size + 1].GetHashCode()));
                    retval = false;
                }
                if (view.views != views)
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), wrong views list {3} <> {4}",
                      view.GetHashCode(), view.offset, view.size, view.views!.GetHashCode(), views.GetHashCode()));
                    retval = false;
                }
                if (view.underlying != this)
                {
                    Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), wrong underlying {3} <> this {4}",
                      view.GetHashCode(), view.offset, view.size, view.underlying!.GetHashCode(), GetHashCode()));
                    retval = false;
                }
                if (view.stamp != stamp)
                {
                    //Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), wrong stamp view:{2} underlying: {3}", view.GetHashCode(),view.offset, view.size, view.stamp, stamp));
                    //retval = false;
                }
            }
            return retval;
        }

        private string TheItem(Node node)
        {
            return node == null ? "(null node)" : node.item!.ToString();
        }

        /// <summary>
        /// Check the sanity of this list
        /// </summary>
        /// <returns>true if sane</returns>
        public virtual bool Check()
        {
            bool retval = true;

            /*if (underlying != null && underlying.stamp != stamp)
            {
              Logger.Log("underlying != null && underlying.stamp({0}) != stamp({1})", underlying.stamp, stamp);
              retval = false;
            }*/

            if (underlying != null)
            {
                //TODO: check that this view is included in viewsEndpoints tree
                return underlying.Check();
            }

            if (startsentinel == null)
            {
                Logger.Log("startsentinel == null");
                retval = false;
            }

            if (endsentinel == null)
            {
                Logger.Log("endsentinel == null");
                retval = false;
            }

            if (size == 0)
            {
                if (startsentinel != null && startsentinel.next != endsentinel)
                {
                    Logger.Log("size == 0 but startsentinel.next != endsentinel");
                    retval = false;
                }

                if (endsentinel != null && endsentinel.prev != startsentinel)
                {
                    Logger.Log("size == 0 but endsentinel.prev != startsentinel");
                    retval = false;
                }
            }

            if (startsentinel == null)
            {
                Logger.Log("NULL startsentinel");
                return retval;
            }

            int count = 0;
            Node node = startsentinel.next!, prev = startsentinel;
            int taggroupsize = 0, oldtaggroupsize = losize + 1, seentaggroups = 0;
            TagGroup? oldtg = null;

            if (underlying == null)
            {
                TagGroup tg = startsentinel.taggroup;

                if (tg.count != 0 || tg.first != null || tg.last != null || tg.tag != int.MinValue)
                {
                    Logger.Log(string.Format("Bad startsentinel tag group: {0}", tg));
                    retval = false;
                }

                tg = endsentinel!.taggroup;
                if (tg.count != 0 || tg.first != null || tg.last != null || tg.tag != int.MaxValue)
                {
                    Logger.Log(string.Format("Bad endsentinel tag group: {0}", tg));
                    retval = false;
                }
            }
            while (node != endsentinel)
            {
                count++;
                if (node.prev != prev)
                {
                    Logger.Log(string.Format("Bad backpointer at node {0}", count));
                    retval = false;
                }
                if (underlying == null)
                {
                    if (!node.prev!.Precedes(node))
                    {
                        Logger.Log(string.Format("node.prev.tag ({0}, {1}) >= node.tag ({2}, {3}) at index={4} item={5} ", node.prev.taggroup.tag, node.prev.tag, node.taggroup.tag, node.tag, count, node.item));
                        retval = false;
                    }

                    if (node.taggroup != oldtg)
                    {

                        if (node.taggroup.first != node)
                        {
                            string ntfi = TheItem(node.taggroup.first!);
                            Logger.Log(string.Format("Bad first pointer in taggroup: node.taggroup.first.item ({0}), node.item ({1}) at index={2} item={3}", ntfi, node.item, count, node.item));
                            retval = false;
                        }

                        if (oldtg != null)
                        {
                            if (oldtg.count != taggroupsize)
                            {
                                Logger.Log(string.Format("Bad taggroupsize: oldtg.count ({0}) != taggroupsize ({1}) at index={2} item={3}", oldtg.count, taggroupsize, count, node.item));
                                retval = false;
                            }

                            if (oldtaggroupsize <= losize && taggroupsize <= losize)
                            {
                                Logger.Log(string.Format("Two small taggroups in a row: oldtaggroupsize ({0}), taggroupsize ({1}) at index={2} item={3}", oldtaggroupsize, taggroupsize, count, node.item));
                                retval = false;
                            }

                            if (node.taggroup.tag <= oldtg.tag)
                            {
                                Logger.Log(string.Format("Taggroup tags not strictly increasing: oldtaggrouptag ({0}), taggrouptag ({1}) at index={2} item={3}", oldtg.tag, node.taggroup.tag, count, node.item));
                                retval = false;
                            }

                            if (oldtg.last != node.prev)
                            {
                                Logger.Log(string.Format("Bad last pointer in taggroup: oldtg.last.item ({0}), node.prev.item ({1}) at index={2} item={3}", oldtg.last!.item, node.prev.item, count, node.item));
                                retval = false;
                            }

                            oldtaggroupsize = taggroupsize;
                        }

                        seentaggroups++;
                        oldtg = node.taggroup;
                        taggroupsize = 1;
                    }
                    else
                    {
                        taggroupsize++;
                    }
                }

                prev = node;
                node = node.next!;
                if (node == null)
                {
                    Logger.Log(string.Format("Null next pointer at node {0}", count));
                    return false;
                }
            }

            if (underlying == null && size == 0 && taggroups != 0)
            {
                Logger.Log(string.Format("Bad taggroups for empty list: size={0}   taggroups={1}", size, taggroups));
                retval = false;
            }
            if (underlying == null && size > 0)
            {
                oldtg = node.prev!.taggroup;
                if (oldtg != null)
                {
                    if (oldtg.count != taggroupsize)
                    {
                        Logger.Log(string.Format("Bad taggroupsize: oldtg.count ({0}) != taggroupsize ({1}) at index={2} item={3}", oldtg.count, taggroupsize, count, node.item));
                        retval = false;
                    }

                    if (oldtaggroupsize <= losize && taggroupsize <= losize)
                    {
                        Logger.Log(string.Format("Two small taggroups in a row: oldtaggroupsize ({0}), taggroupsize ({1}) at index={2} item={3}", oldtaggroupsize, taggroupsize, count, node.item));
                        retval = false;
                    }

                    if (node.taggroup.tag <= oldtg.tag)
                    {
                        Logger.Log(string.Format("Taggroup tags not strictly increasing: oldtaggrouptag ({0}), taggrouptag ({1}) at index={2} item={3}", oldtg.tag, node.taggroup.tag, count, node.item));
                        retval = false;
                    }

                    if (oldtg.last != node.prev)
                    {
                        Logger.Log(string.Format("Bad last pointer in taggroup: oldtg.last.item ({0}), node.prev.item ({1}) at index={2} item={3}", TheItem(oldtg.last!), TheItem(node.prev), count, node.item));
                        retval = false;
                    }
                }

                if (seentaggroups != taggroups)
                {
                    Logger.Log(string.Format("seentaggroups ({0}) != taggroups ({1}) (at size {2})", seentaggroups, taggroups, size));
                    retval = false;
                }
            }
            if (count != size)
            {
                Logger.Log(string.Format("size={0} but enumeration gives {1} nodes ", size, count));
                retval = false;
            }

            retval = CheckViews() && retval;

            if (!retval)
            {
                return false;
            }

            if (underlying == null)
            {
                if (size != dict.Count)
                {
                    Logger.Log(string.Format("list.size ({0}) != dict.Count ({1})", size, dict.Count));
                    retval = false;
                }
                Node n = startsentinel.next!;
                while (n != endsentinel)
                {
                    if (!dict.Find(ref n!.item, out Node n2))
                    {
                        Logger.Log(string.Format("Item in list but not dict: {0}", n.item));
                        retval = false;
                    }
                    else if (n != n2)
                    {
                        Logger.Log(string.Format("Wrong node in dict for item: {0}", n.item));
                        retval = false;
                    }
                    n = n.next!;
                }
            }
            return retval;
        }
        #endregion

        #region System.Collections.Generic.IList<T> Members

        void System.Collections.Generic.IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        void System.Collections.Generic.ICollection<T>.Add(T item)
        {
            Add(item);
        }

        #endregion

        #region System.Collections.ICollection Members

        bool System.Collections.ICollection.IsSynchronized => false;

        [Obsolete]
        object System.Collections.ICollection.SyncRoot => underlying != null ? ((System.Collections.ICollection)underlying).SyncRoot! : startsentinel!;

        void System.Collections.ICollection.CopyTo(Array arr, int index)
        {
            if (index < 0 || index + Count > arr.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (T item in this)
            {
                arr.SetValue(item, index++);
            }
        }

        #endregion

        #region System.Collections.IList Members

        Object System.Collections.IList.this[int index]
        {
            get => this[index]!;
            set => this[index] = (T)value;
        }

        int System.Collections.IList.Add(Object o)
        {
            bool added = Add((T)o);
            // What position to report if item not added? SC.IList.Add doesn't say
            return added ? Count - 1 : -1;
        }

        bool System.Collections.IList.Contains(Object o)
        {
            return Contains((T)o);
        }

        int System.Collections.IList.IndexOf(Object o)
        {
            return Math.Max(-1, IndexOf((T)o));
        }

        void System.Collections.IList.Insert(int index, Object o)
        {
            Insert(index, (T)o);
        }

        void System.Collections.IList.Remove(Object o)
        {
            Remove((T)o);
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        #endregion
    }
}
