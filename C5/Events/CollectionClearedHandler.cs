namespace C5
{
    /// <summary>
    /// The type of event raised after the Clear() operation on a collection.
    /// <para/>
    /// Note: The Clear() operation will not fire ItemsRemoved events. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void CollectionClearedHandler<T>(object sender, ClearedEventArgs eventArgs);
}