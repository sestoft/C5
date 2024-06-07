// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// An editable collection maintaining a definite sequence order of the items.
///
/// <i>Implementations of this interface must compute the hash code and
/// equality exactly as prescribed in the method definitions in order to
/// be consistent with other collection classes implementing this interface.</i>
/// <i>This interface is usually implemented by explicit interface implementation,
/// not as ordinary virtual methods.</i>
/// </summary>
public interface ISequenced<T> : ICollection<T>, IDirectedCollectionValue<T>
{
    /// <summary>
    /// The hashcode is defined as <code>h(...h(h(h(x1),x2),x3),...,xn)</code> for
    /// <code>h(a,b)=CONSTANT*a+b</code> and the x's the hash codes of the items of
    /// this collection.
    /// </summary>
    /// <returns>The sequence order hashcode of this collection.</returns>
    int GetSequencedHashCode();


    /// <summary>
    /// Compare this sequenced collection to another one in sequence order.
    /// </summary>
    /// <param name="otherCollection">The sequenced collection to compare to.</param>
    /// <returns>True if this collection and that contains equal (according to
    /// this collection's itemequalityComparer) in the same sequence order.</returns>
    bool SequencedEquals(ISequenced<T> otherCollection);
}