// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// The simplest interface of a main stream generic collection
/// with lookup, insertion and removal operations.
/// </summary>
public interface ICollection<T> : IExtensible<T>, SCG.ICollection<T>
{
    //This is somewhat similar to the RandomAccess marker itf in java
    /// <summary>
    /// The value is symbolic indicating the type of asymptotic complexity
    /// in terms of the size of this collection (worst-case or amortized as
    /// relevant).
    /// <para>See <see cref="T:C5.Speed"/> for the set of symbols.</para>
    /// </summary>
    /// <value>A characterization of the speed of lookup operations
    /// (<code>Contains()</code> etc.) of the implementation of this collection.</value>
    Speed ContainsSpeed { get; }

    /// <summary>
    /// </summary>
    /// <value>The number of items in this collection</value>
    new int Count { get; }

    /// <summary>
    /// If true any call of an updating operation will throw an
    /// <code>ReadOnlyCollectionException</code>
    /// </summary>
    /// <value>True if this collection is read-only.</value>
    new bool IsReadOnly { get; }

    /// <summary>
    /// Add an item to this collection if possible. If this collection has set
    /// semantics, the item will be added if not already in the collection. If
    /// bag semantics, the item will always be added.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True if item was added.</returns>
    new bool Add(T item);

    /// <summary>
    /// Copy the items of this collection to a contiguous part of an array.
    /// </summary>
    /// <param name="array">The array to copy to</param>
    /// <param name="index">The index at which to copy the first item</param>
    new void CopyTo(T[] array, int index);

    /// <summary>
    /// The unordered collection hashcode is defined as the sum of
    /// <code>h(hashcode(item))</code> over the items
    /// of the collection, where the function <code>h</code> is a function from
    /// int to int of the form <code> t -> (a0*t+b0)^(a1*t+b1)^(a2*t+b2)</code>, where
    /// the ax and bx are the same for all collection classes.
    /// <para>The current implementation uses fixed values for the ax and bx,
    /// specified as constants in the code.</para>
    /// </summary>
    /// <returns>The unordered hashcode of this collection.</returns>
    int GetUnsequencedHashCode();


    /// <summary>
    /// Compare the contents of this collection to another one without regards to
    /// the sequence order. The comparison will use this collection's itemequalityComparer
    /// to compare individual items.
    /// </summary>
    /// <param name="otherCollection">The collection to compare to.</param>
    /// <returns>True if this collection and that contains the same items.</returns>
    bool UnsequencedEquals(ICollection<T> otherCollection);


    /// <summary>
    /// Check if this collection contains (an item equivalent to according to the
    /// itemequalityComparer) a particular value.
    /// </summary>
    /// <param name="item">The value to check for.</param>
    /// <returns>True if the items is in this collection.</returns>
    new bool Contains(T item);


    /// <summary>
    /// Count the number of items of the collection equal to a particular value.
    /// Returns 0 if and only if the value is not in the collection.
    /// </summary>
    /// <param name="item">The value to count.</param>
    /// <returns>The number of copies found.</returns>
    int ContainsCount(T item);


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    ICollectionValue<T> UniqueItems();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities();

    /// <summary>
    /// Check whether this collection contains all the values in another collection.
    /// If this collection has bag semantics (<code>AllowsDuplicates==true</code>)
    /// the check is made with respect to multiplicities, else multiplicities
    /// are not taken into account.
    /// </summary>
    /// <param name="items">The </param>
    /// <returns>True if all values in <code>items</code>is in this collection.</returns>
    bool ContainsAll(SCG.IEnumerable<T> items);


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, return in the ref argument (a
    /// binary copy of) the actual value found.
    /// </summary>
    /// <param name="item">The value to look for.</param>
    /// <returns>True if the items is in this collection.</returns>
    bool Find(ref T item);


    //This should probably just be bool Add(ref T item); !!!
    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, return in the ref argument (a
    /// binary copy of) the actual value found. Else, add the item to the collection.
    /// </summary>
    /// <param name="item">The value to look for.</param>
    /// <returns>True if the item was found (hence not added).</returns>
    bool FindOrAdd(ref T item);


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// with a (binary copy of) the supplied value. If the collection has bag semantics,
    /// it depends on the value of DuplicatesByCounting if this updates all equivalent copies in
    /// the collection or just one.
    /// </summary>
    /// <param name="item">Value to update.</param>
    /// <returns>True if the item was found and hence updated.</returns>
    bool Update(T item);

    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// with a (binary copy of) the supplied value. If the collection has bag semantics,
    /// it depends on the value of DuplicatesByCounting if this updates all equivalent copies in
    /// the collection or just one.
    /// </summary>
    /// <param name="item">Value to update.</param>
    /// <param name="olditem">On output the olditem, if found.</param>
    /// <returns>True if the item was found and hence updated.</returns>
    bool Update(T item, out T olditem);


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// to with a binary copy of the supplied value; else add the value to the collection.
    /// </summary>
    /// <param name="item">Value to add or update.</param>
    /// <returns>True if the item was found and updated (hence not added).</returns>
    bool UpdateOrAdd(T item);


    /// <summary>
    /// Check if this collection contains an item equivalent according to the
    /// itemequalityComparer to a particular value. If so, update the item in the collection
    /// to with a binary copy of the supplied value; else add the value to the collection.
    /// </summary>
    /// <param name="item">Value to add or update.</param>
    /// <param name="olditem">On output the olditem, if found.</param>
    /// <returns>True if the item was found and updated (hence not added).</returns>
    bool UpdateOrAdd(T item, out T olditem);

    /// <summary>
    /// Remove a particular item from this collection. If the collection has bag
    /// semantics only one copy equivalent to the supplied item is removed.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <returns>True if the item was found (and removed).</returns>
    new bool Remove(T item);


    /// <summary>
    /// Remove a particular item from this collection if found. If the collection
    /// has bag semantics only one copy equivalent to the supplied item is removed,
    /// which one is implementation dependent.
    /// If an item was removed, report a binary copy of the actual item removed in
    /// the argument.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <param name="removeditem">The value removed if any.</param>
    /// <returns>True if the item was found (and removed).</returns>
    bool Remove(T item, out T removeditem);


    /// <summary>
    /// Remove all items equivalent to a given value.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    void RemoveAllCopies(T item);


    /// <summary>
    /// Remove all items in another collection from this one. If this collection
    /// has bag semantics, take multiplicities into account.
    /// </summary>
    /// <param name="items">The items to remove.</param>
    void RemoveAll(SCG.IEnumerable<T> items);

    //void RemoveAll(Func<T, bool> predicate);

    /// <summary>
    /// Remove all items from this collection.
    /// </summary>
    new void Clear();


    /// <summary>
    /// Remove all items not in some other collection from this one. If this collection
    /// has bag semantics, take multiplicities into account.
    /// </summary>
    /// <param name="items">The items to retain.</param>
    void RetainAll(SCG.IEnumerable<T> items);

    //void RetainAll(Func<T, bool> predicate);
    //IDictionary<T> UniqueItems()
}