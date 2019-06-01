// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;

namespace C5
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum EventTypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// 
        /// </summary>
        Changed = 0x00000001,
        /// <summary>
        /// 
        /// </summary>
        Cleared = 0x00000002,
        /// <summary>
        /// 
        /// </summary>
        Added = 0x00000004,
        /// <summary>
        /// 
        /// </summary>
        Removed = 0x00000008,
        /// <summary>
        /// 
        /// </summary>
        Basic = 0x0000000f,
        /// <summary>
        /// 
        /// </summary>
        Inserted = 0x00000010,
        /// <summary>
        /// 
        /// </summary>
        RemovedAt = 0x00000020,
        /// <summary>
        /// 
        /// </summary>
        All = 0x0000003f
    }

    /// <summary>
    /// Holds the real events for a collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal sealed class EventBlock<T>
    {
        internal EventTypeEnum events;

        private event CollectionChangedHandler<T> CollectionChangedInner;
        internal event CollectionChangedHandler<T> CollectionChanged
        {
            add
            {
                CollectionChangedInner += value;
                events |= EventTypeEnum.Changed;
            }
            remove
            {
                CollectionChangedInner -= value;
                if (CollectionChangedInner == null)
                {
                    events &= ~EventTypeEnum.Changed;
                }
            }
        }
        internal void RaiseCollectionChanged(object sender)
        {
            CollectionChangedInner?.Invoke(sender);
        }

        private event CollectionClearedHandler<T> CollectionClearedInner;
        internal event CollectionClearedHandler<T> CollectionCleared
        {
            add
            {
                CollectionClearedInner += value;
                events |= EventTypeEnum.Cleared;
            }
            remove
            {
                CollectionClearedInner -= value;
                if (CollectionClearedInner == null)
                {
                    events &= ~EventTypeEnum.Cleared;
                }
            }
        }

        internal void RaiseCollectionCleared(object sender, bool full, int count)
        {
            CollectionClearedInner?.Invoke(sender, new ClearedEventArgs(full, count));
        }

        internal void RaiseCollectionCleared(object sender, bool full, int count, int? start)
        {
            CollectionClearedInner?.Invoke(sender, new ClearedRangeEventArgs(full, count, start));
        }

        private event ItemsAddedHandler<T> ItemsAddedInner;
        internal event ItemsAddedHandler<T> ItemsAdded
        {
            add
            {
                ItemsAddedInner += value;
                events |= EventTypeEnum.Added;
            }
            remove
            {
                ItemsAddedInner -= value;
                if (ItemsAddedInner == null)
                {
                    events &= ~EventTypeEnum.Added;
                }
            }
        }
        internal void RaiseItemsAdded(object sender, T item, int count)
        {
            ItemsAddedInner?.Invoke(sender, new ItemCountEventArgs<T>(item, count));
        }

        private event ItemsRemovedHandler<T> ItemsRemovedInner;
        internal event ItemsRemovedHandler<T> ItemsRemoved
        {
            add
            {
                ItemsRemovedInner += value;
                events |= EventTypeEnum.Removed;
            }
            remove
            {
                ItemsRemovedInner -= value;
                if (ItemsRemovedInner == null)
                {
                    events &= ~EventTypeEnum.Removed;
                }
            }
        }

        internal void RaiseItemsRemoved(object sender, T item, int count)
        {
            ItemsRemovedInner?.Invoke(sender, new ItemCountEventArgs<T>(item, count));
        }

        private event ItemInsertedHandler<T> ItemInsertedInner;
        internal event ItemInsertedHandler<T> ItemInserted
        {
            add
            {
                ItemInsertedInner += value;
                events |= EventTypeEnum.Inserted;
            }
            remove
            {
                ItemInsertedInner -= value;
                if (ItemInsertedInner == null)
                {
                    events &= ~EventTypeEnum.Inserted;
                }
            }
        }

        internal void RaiseItemInserted(object sender, T item, int index)
        {
            ItemInsertedInner?.Invoke(sender, new ItemAtEventArgs<T>(item, index));
        }

        private event ItemRemovedAtHandler<T> ItemRemovedAtInner;
        internal event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add
            {
                ItemRemovedAtInner += value;
                events |= EventTypeEnum.RemovedAt;
            }
            remove
            {
                ItemRemovedAtInner -= value;
                if (ItemRemovedAtInner == null)
                {
                    events &= ~EventTypeEnum.RemovedAt;
                }
            }
        }

        internal void RaiseItemRemovedAt(object sender, T item, int index)
        {
            ItemRemovedAtInner?.Invoke(sender, new ItemAtEventArgs<T>(item, index));
        }
    }

    /// <summary>
    /// Tentative, to conserve memory in GuardedCollectionValueBase
    /// This should really be nested in Guarded collection value, only have a guardereal field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal sealed class ProxyEventBlock<T>
    {
        private readonly ICollectionValue<T> proxy, real;

        internal ProxyEventBlock(ICollectionValue<T> proxy, ICollectionValue<T> real)
        { this.proxy = proxy; this.real = real; }

        private event CollectionChangedHandler<T> CollectionChangedInner;

        private CollectionChangedHandler<T>? collectionChangedProxy = null;
        internal event CollectionChangedHandler<T> CollectionChanged
        {
            add
            {
                if (CollectionChangedInner == null)
                {
                    if (collectionChangedProxy == null)
                    {
                        collectionChangedProxy = delegate (object sender) { CollectionChangedInner(proxy); };
                    }

                    real.CollectionChanged += collectionChangedProxy;
                }
                CollectionChangedInner += value;
            }
            remove
            {
                CollectionChangedInner -= value;
                if (CollectionChangedInner == null)
                {
                    real.CollectionChanged -= collectionChangedProxy;
                }
            }
        }

        private event CollectionClearedHandler<T> CollectionClearedInner;

        private CollectionClearedHandler<T>? collectionClearedProxy = null;
        internal event CollectionClearedHandler<T> CollectionCleared
        {
            add
            {
                if (CollectionClearedInner == null)
                {
                    if (collectionClearedProxy == null)
                    {
                        collectionClearedProxy = delegate (object sender, ClearedEventArgs e) { CollectionClearedInner(proxy, e); };
                    }

                    real.CollectionCleared += collectionClearedProxy;
                }
                CollectionClearedInner += value;
            }
            remove
            {
                CollectionClearedInner -= value;
                if (CollectionClearedInner == null)
                {
                    real.CollectionCleared -= collectionClearedProxy;
                }
            }
        }

        private event ItemsAddedHandler<T> ItemsAddedInner;

        private ItemsAddedHandler<T>? itemsAddedProxy = null;
        internal event ItemsAddedHandler<T> ItemsAdded
        {
            add
            {
                if (ItemsAddedInner == null)
                {
                    if (itemsAddedProxy == null)
                    {
                        itemsAddedProxy = delegate (object sender, ItemCountEventArgs<T> e) { ItemsAddedInner(proxy, e); };
                    }

                    real.ItemsAdded += itemsAddedProxy;
                }
                ItemsAddedInner += value;
            }
            remove
            {
                ItemsAddedInner -= value;
                if (ItemsAddedInner == null)
                {
                    real.ItemsAdded -= itemsAddedProxy;
                }
            }
        }

        private event ItemInsertedHandler<T> ItemInsertedInner;

        private ItemInsertedHandler<T>? itemInsertedProxy = null;
        internal event ItemInsertedHandler<T> ItemInserted
        {
            add
            {
                if (ItemInsertedInner == null)
                {
                    if (itemInsertedProxy == null)
                    {
                        itemInsertedProxy = delegate (object sender, ItemAtEventArgs<T> e) { ItemInsertedInner(proxy, e); };
                    }

                    real.ItemInserted += itemInsertedProxy;
                }
                ItemInsertedInner += value;
            }
            remove
            {
                ItemInsertedInner -= value;
                if (ItemInsertedInner == null)
                {
                    real.ItemInserted -= itemInsertedProxy;
                }
            }
        }

        private event ItemsRemovedHandler<T>? ItemsRemovedInner = null;

        private ItemsRemovedHandler<T>? itemsRemovedProxy = null;
        internal event ItemsRemovedHandler<T> ItemsRemoved
        {
            add
            {
                if (ItemsRemovedInner == null)
                {
                    if (itemsRemovedProxy == null)
                    {
                        itemsRemovedProxy = delegate (object sender, ItemCountEventArgs<T> e) { ItemsRemovedInner?.Invoke(proxy, e); };
                    }

                    real.ItemsRemoved += itemsRemovedProxy;
                }
                ItemsRemovedInner += value;
            }
            remove
            {
                ItemsRemovedInner -= value;
                if (ItemsRemovedInner == null)
                {
                    real.ItemsRemoved -= itemsRemovedProxy;
                }
            }
        }

        private event ItemRemovedAtHandler<T> ItemRemovedAtInner;

        private ItemRemovedAtHandler<T>? itemRemovedAtProxy = null;
        internal event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add
            {
                if (ItemRemovedAtInner == null)
                {
                    if (itemRemovedAtProxy == null)
                    {
                        itemRemovedAtProxy = delegate (object sender, ItemAtEventArgs<T> e) { ItemRemovedAtInner(proxy, e); };
                    }

                    real.ItemRemovedAt += itemRemovedAtProxy;
                }
                ItemRemovedAtInner += value;
            }
            remove
            {
                ItemRemovedAtInner -= value;
                if (ItemRemovedAtInner == null)
                {
                    real.ItemRemovedAt -= itemRemovedAtProxy;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ItemAtEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public ItemAtEventArgs(T item, int index) { Item = item; Index = index; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(ItemAtEventArgs {0} '{1}')", Index, Item);
        }
    }

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

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ClearedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Full { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="full">True if the operation cleared all of the collection</param>
        /// <param name="count">The number of items removed by the clear.</param>
        public ClearedEventArgs(bool full, int count) { Full = full; Count = count; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(ClearedEventArgs {0} {1})", Count, Full);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
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

    /// <summary>
    /// The type of event raised after an operation on a collection has changed its contents.
    /// Normally, a multioperation like AddAll, 
    /// <see cref="M:C5.IExtensible`1.AddAll(System.Collections.Generic.IEnumerable{`0})"/> 
    /// will only fire one CollectionChanged event. Any operation that changes the collection
    /// must fire CollectionChanged as its last event.
    /// </summary>
    public delegate void CollectionChangedHandler<T>(object sender);

    /// <summary>
    /// The type of event raised after the Clear() operation on a collection.
    /// <para/>
    /// Note: The Clear() operation will not fire ItemsRemoved events. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void CollectionClearedHandler<T>(object sender, ClearedEventArgs eventArgs);

    /// <summary>
    /// The type of event raised after an item has been added to a collection.
    /// The event will be raised at a point of time, where the collection object is 
    /// in an internally consistent state and before the corresponding CollectionChanged 
    /// event is raised.
    /// <para/>
    /// Note: an Update operation will fire an ItemsRemoved and an ItemsAdded event.
    /// <para/>
    /// Note: When an item is inserted into a list (<see cref="T:C5.IList`1"/>), both
    /// ItemInserted and ItemsAdded events will be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs">An object with the item that was added</param>
    public delegate void ItemsAddedHandler<T>(object sender, ItemCountEventArgs<T> eventArgs);

    /// <summary>
    /// The type of event raised after an item has been removed from a collection.
    /// The event will be raised at a point of time, where the collection object is 
    /// in an internally consistent state and before the corresponding CollectionChanged 
    /// event is raised.
    /// <para/>
    /// Note: The Clear() operation will not fire ItemsRemoved events. 
    /// <para/>
    /// Note: an Update operation will fire an ItemsRemoved and an ItemsAdded event.
    /// <para/>
    /// Note: When an item is removed from a list by the RemoveAt operation, both an 
    /// ItemsRemoved and an ItemRemovedAt event will be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs">An object with the item that was removed</param>
    public delegate void ItemsRemovedHandler<T>(object sender, ItemCountEventArgs<T> eventArgs);

    /// <summary>
    /// The type of event raised after an item has been inserted into a list by an Insert, 
    /// InsertFirst or InsertLast operation.
    /// The event will be raised at a point of time, where the collection object is 
    /// in an internally consistent state and before the corresponding CollectionChanged 
    /// event is raised.
    /// <para/>
    /// Note: an ItemsAdded event will also be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void ItemInsertedHandler<T>(object sender, ItemAtEventArgs<T> eventArgs);

    /// <summary>
    /// The type of event raised after an item has been removed from a list by a RemoveAt(int i)
    /// operation (or RemoveFirst(), RemoveLast(), Remove() operation).
    /// The event will be raised at a point of time, where the collection object is 
    /// in an internally consistent state and before the corresponding CollectionChanged 
    /// event is raised.
    /// <para/>
    /// Note: an ItemRemoved event will also be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void ItemRemovedAtHandler<T>(object sender, ItemAtEventArgs<T> eventArgs);
}