namespace C5
{
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