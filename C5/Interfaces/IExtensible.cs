using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A generic collection to which one may add items. This is just the intersection
    /// of the main stream generic collection interfaces and the priority queue interface,
    /// <see cref="T:C5.ICollection`1"/> and <see cref="T:C5.IPriorityQueue`1"/>.
    /// </summary>
    public interface IExtensible<T> : ICollectionValue<T>
    {
        /// <summary>
        /// If true any call of an updating operation will throw an
        /// <code>ReadOnlyCollectionException</code>
        /// </summary>
        /// <value>True if this collection is read-only.</value>
        new bool IsReadOnly { get; }

        //TODO: wonder where the right position of this is
        /// <summary>
        /// 
        /// </summary>
        /// <value>False if this collection has set semantics, true if bag semantics.</value>
        bool AllowsDuplicates { get; }

        //TODO: wonder where the right position of this is. And the semantics.
        /// <summary>
        /// (Here should be a discussion of the role of equalityComparers. Any ). 
        /// </summary>
        /// <value>The equalityComparer used by this collection to check equality of items. 
        /// Or null (????) if collection does not check equality at all or uses a comparer.</value>
        System.Collections.Generic.IEqualityComparer<T> EqualityComparer { get; }

        //ItemEqualityTypeEnum ItemEqualityType {get ;}

        //TODO: find a good name

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        bool DuplicatesByCounting { get; }

        /// <summary>
        /// Add an item to this collection if possible. If this collection has set
        /// semantics, the item will be added if not already in the collection. If
        /// bag semantics, the item will always be added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if item was added.</returns>
        new bool Add(T item);

        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. If this
        /// collection has set semantics, only items not already in the collection
        /// will be added.
        /// </summary>
        /// <param name="items">The items to add</param>
        void AddAll(IEnumerable<T> items);

        //void Clear(); // for priority queue
        //int Count why not?
        /// <summary>
        /// Check the integrity of the internal data structures of this collection.
        /// <i>This is only relevant for developers of the library</i>
        /// </summary>
        /// <returns>True if check was passed.</returns>
        bool Check();
    }
}