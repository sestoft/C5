namespace C5
{
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
}