using System;

namespace C5
{
    /// <summary>
    /// Tentative, to conserve memory in GuardedCollectionValueBase
    /// This should really be nested in Guarded collection value, only have a guardereal field
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
                    collectionChangedProxy ??= delegate (object sender) { CollectionChangedInner(proxy); };

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
                    collectionClearedProxy ??= delegate (object sender, ClearedEventArgs e) { CollectionClearedInner(proxy, e); };

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
                    itemsAddedProxy ??= delegate (object sender, ItemCountEventArgs<T> e) { ItemsAddedInner(proxy, e); };

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
                    itemInsertedProxy ??= delegate (object sender, ItemAtEventArgs<T> e) { ItemInsertedInner(proxy, e); };

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
                    itemsRemovedProxy ??= delegate (object sender, ItemCountEventArgs<T> e) { ItemsRemovedInner?.Invoke(proxy, e); };

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
                    itemRemovedAtProxy ??= delegate (object sender, ItemAtEventArgs<T> e) { ItemRemovedAtInner(proxy, e); };

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
}