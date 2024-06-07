// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;

namespace C5.Tests;

public class UnsequencedCollectionEqualityComparerTests
{
    [Test]
    public void Equals_given_null_and_null_returns_true()
    {
        var comparer = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;

        Assert.That(comparer.Equals(null, null), Is.True);
    }

    [Test]
    public void Equals_given_null_and_something_returns_false()
    {
        var comparer = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;

        TreeBag<char> something = ['a', 'b', 'c'];

        Assert.That(comparer.Equals(null, something), Is.False);
    }

    [Test]
    public void Equals_given_something_and_null_returns_false()
    {
        var comparer = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;

        TreeBag<char> something = ['a', 'b', 'c'];

        Assert.That(comparer.Equals(something, null), Is.False);
    }
}