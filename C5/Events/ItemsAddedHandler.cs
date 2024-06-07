// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

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