// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// The type of event raised after an operation on a collection has changed its contents.
/// Normally, a multioperation like AddAll, 
/// <see cref="M:C5.IExtensible`1.AddAll(System.Collections.Generic.IEnumerable{`0})"/> 
/// will only fire one CollectionChanged event. Any operation that changes the collection
/// must fire CollectionChanged as its last event.
/// </summary>
public delegate void CollectionChangedHandler<T>(object sender);