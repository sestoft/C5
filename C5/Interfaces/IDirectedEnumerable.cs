// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System.Collections.Generic;

namespace C5;

/// <summary>
/// A generic collection, that can be enumerated backwards.
/// </summary>
public interface IDirectedEnumerable<T> : IEnumerable<T> // TODO: Type parameter should be 'out T' when Silverlight supports is (version 5 and onwards)
{
    /// <summary>
    /// Create a collection containing the same items as this collection, but
    /// whose enumerator will enumerate the items backwards. The new collection
    /// will become invalid if the original is modified. Method typically used as in
    /// <code>foreach (T x in coll.Backwards()) {...}</code>
    /// </summary>
    /// <returns>The backwards collection.</returns>
    IDirectedEnumerable<T> Backwards();

    /// <summary>
    /// <code>Forwards</code> if same, else <code>Backwards</code>
    /// </summary>
    /// <value>The enumeration direction relative to the original collection.</value>
    Direction Direction { get; }
}


//TODO: decide if this should extend ICollection


/*************************************************************************/


/*******************************************************************/
/*/// <summary>
/// The type of an item comparer
/// <i>Implementations of this interface must asure that the method is self-consistent
/// and defines a sorting order on items, or state precise conditions under which this is true.</i>
/// <i>Implementations <b>must</b> assure that repeated calls of
/// the method to the same (in reference or binary identity sense) arguments
/// will return values with the same sign (-1, 0 or +1), or state precise conditions
/// under which the user
/// can be assured repeated calls will return the same sign.</i>
/// <i>Implementations of this interface must always return values from the method
/// and never throw exceptions.</i>
/// <i>This interface is identical to System.Collections.Generic.IComparer&lt;T&gt;</i>
/// </summary>
public interface ISystem.Collections.Generic.Comparer<T>
{
  /// <summary>
  /// Compare two items with respect to this item comparer
  /// </summary>
  /// <param name="item1">First item</param>
  /// <param name="item2">Second item</param>
  /// <returns>Positive if item1 is greater than item2, 0 if they are equal, negative if item1 is less than item2</returns>
  int Compare(T item1, T item2);
}

/// <summary>
/// The type of an item equalityComparer.
/// <i>Implementations of this interface <b>must</b> assure that the methods are
/// consistent, that is, that whenever two items i1 and i2 satisfies that Equals(i1,i2)
/// returns true, then GetHashCode returns the same value for i1 and i2.</i>
/// <i>Implementations of this interface <b>must</b> assure that repeated calls of
/// the methods to the same (in reference or binary identity sense) arguments
/// will return the same values, or state precise conditions under which the user
/// can be assured repeated calls will return the same values.</i>
/// <i>Implementations of this interface must always return values from the methods
/// and never throw exceptions.</i>
/// <i>This interface is similar in function to System.IKeyComparer&lt;T&gt;</i>
/// </summary>
public interface System.Collections.Generic.IEqualityComparer<T>
{
  /// <summary>
  /// Get the hash code with respect to this item equalityComparer
  /// </summary>
  /// <param name="item">The item</param>
  /// <returns>The hash code</returns>
  int GetHashCode(T item);


  /// <summary>
  /// Check if two items are equal with respect to this item equalityComparer
  /// </summary>
  /// <param name="item1">first item</param>
  /// <param name="item2">second item</param>
  /// <returns>True if equal</returns>
  bool Equals(T item1, T item2);
}*/
