// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// A read-only wrapper for an <see cref="T:C5.ICollection`1"/>,
/// <para>
/// <i>Suitable for wrapping hash tables, <see cref="T:C5.HashSet`1"/>
/// and <see cref="T:C5.HashBag`1"/>  </i></para>
/// </summary>
public class GuardedCollection<T> : GuardedCollectionValue<T>, ICollection<T>
{
    #region Fields

    private readonly ICollection<T> collection;

    #endregion

    #region Constructor

    /// <summary>
    /// Wrap an ICollection&lt;T&gt; in a read-only wrapper
    /// </summary>
    /// <param name="collection">the collection to wrap</param>
    public GuardedCollection(ICollection<T> collection)
        : base(collection)
    {
        this.collection = collection;
    }

    #endregion

    #region ICollection<T> Members

    /// <summary>
    /// (This is a read-only wrapper)
    /// </summary>
    /// <value>True</value>
    public virtual bool IsReadOnly => true;


    /// <summary> </summary>
    /// <value>Speed of wrapped collection</value>
    public virtual Speed ContainsSpeed => collection.ContainsSpeed;

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual int GetUnsequencedHashCode()
    { return collection.GetUnsequencedHashCode(); }

    /// <summary>
    ///
    /// </summary>
    /// <param name="that"></param>
    /// <returns></returns>
    public virtual bool UnsequencedEquals(ICollection<T> that)
    { return collection.UnsequencedEquals(that); }


    /// <summary>
    /// Check if an item is in the wrapped collection
    /// </summary>
    /// <param name="item">The item</param>
    /// <returns>True if found</returns>
    public virtual bool Contains(T item) { return collection.Contains(item); }


    /// <summary>
    /// Count the number of times an item appears in the wrapped collection
    /// </summary>
    /// <param name="item">The item</param>
    /// <returns>The number of copies</returns>
    public virtual int ContainsCount(T item) { return collection.ContainsCount(item); }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual ICollectionValue<T> UniqueItems() { return new GuardedCollectionValue<T>(collection.UniqueItems()); }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities() { return new GuardedCollectionValue<System.Collections.Generic.KeyValuePair<T, int>>(collection.ItemMultiplicities()); }

    /// <summary>
    /// Check if all items in the argument is in the wrapped collection
    /// </summary>
    /// <param name="items">The items</param>
    /// <returns>True if so</returns>
    public virtual bool ContainsAll(System.Collections.Generic.IEnumerable<T> items) { return collection.ContainsAll(items); }

    /// <summary>
    /// Search for an item in the wrapped collection
    /// </summary>
    /// <param name="item">On entry the item to look for, on exit the equivalent item found (if any)</param>
    /// <returns></returns>
    public virtual bool Find(ref T item) { return collection.Find(ref item); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool FindOrAdd(ref T item)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Update(T item)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <param name="olditem"></param>
    /// <returns></returns>
    public virtual bool Update(T item, out T olditem)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool UpdateOrAdd(T item)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <param name="olditem"></param>
    /// <returns></returns>
    public virtual bool UpdateOrAdd(T item, out T olditem)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Remove(T item)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item">The value to remove.</param>
    /// <param name="removeditem">The removed value.</param>
    /// <returns></returns>
    public virtual bool Remove(T item, out T removeditem)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    public virtual void RemoveAllCopies(T item)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="items"></param>
    public virtual void RemoveAll(System.Collections.Generic.IEnumerable<T> items)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    public virtual void Clear()
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="items"></param>
    public virtual void RetainAll(System.Collections.Generic.IEnumerable<T> items)
    { throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object"); }

    /// <summary>
    /// Check  wrapped collection for internal consistency
    /// </summary>
    /// <returns>True if check passed</returns>
    public virtual bool Check() { return collection.Check(); }

    #endregion

    #region IExtensible<T> Members

    /// <summary> </summary>
    /// <value>False if wrapped collection has set semantics</value>
    public virtual bool AllowsDuplicates => collection.AllowsDuplicates;

    //TODO: the equalityComparer should be guarded
    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public virtual System.Collections.Generic.IEqualityComparer<T> EqualityComparer => collection.EqualityComparer;

    /// <summary>
    /// By convention this is true for any collection with set semantics.
    /// </summary>
    /// <value>True if only one representative of a group of equal items
    /// is kept in the collection together with the total count.</value>
    public virtual bool DuplicatesByCounting => collection.DuplicatesByCounting;


    /// <summary> </summary>
    /// <value>True if wrapped collection is empty</value>
    public override bool IsEmpty => collection.IsEmpty;


    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Add(T item)
    { throw new ReadOnlyCollectionException(); }

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="item"></param>
    void System.Collections.Generic.ICollection<T>.Add(T item)
    { throw new ReadOnlyCollectionException(); }

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="items"></param>
    public virtual void AddAll(System.Collections.Generic.IEnumerable<T> items)
    { throw new ReadOnlyCollectionException(); }

    #endregion
}