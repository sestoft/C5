// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

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