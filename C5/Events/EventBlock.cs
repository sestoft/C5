// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5
{
    /// <summary>
    /// Holds the real events for a collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal sealed class EventBlock<T>
    {
        internal EventType events;

        private event CollectionChangedHandler<T> CollectionChangedInner;
        internal event CollectionChangedHandler<T> CollectionChanged
        {
            add
            {
                CollectionChangedInner += value;
                events |= EventType.Changed;
            }
            remove
            {
                CollectionChangedInner -= value;
                if (CollectionChangedInner == null)
                {
                    events &= ~EventType.Changed;
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
                events |= EventType.Cleared;
            }
            remove
            {
                CollectionClearedInner -= value;
                if (CollectionClearedInner == null)
                {
                    events &= ~EventType.Cleared;
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
                events |= EventType.Added;
            }
            remove
            {
                ItemsAddedInner -= value;
                if (ItemsAddedInner == null)
                {
                    events &= ~EventType.Added;
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
                events |= EventType.Removed;
            }
            remove
            {
                ItemsRemovedInner -= value;
                if (ItemsRemovedInner == null)
                {
                    events &= ~EventType.Removed;
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
                events |= EventType.Inserted;
            }
            remove
            {
                ItemInsertedInner -= value;
                if (ItemInsertedInner == null)
                {
                    events &= ~EventType.Inserted;
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
                events |= EventType.RemovedAt;
            }
            remove
            {
                ItemRemovedAtInner -= value;
                if (ItemRemovedAtInner == null)
                {
                    events &= ~EventType.RemovedAt;
                }
            }
        }

        internal void RaiseItemRemovedAt(object sender, T item, int index)
        {
            ItemRemovedAtInner?.Invoke(sender, new ItemAtEventArgs<T>(item, index));
        }
    }
}