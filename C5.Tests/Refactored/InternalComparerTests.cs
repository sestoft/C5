// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;

namespace C5.Tests;

public class InternalComparerTests
{
    [Test]
    public void Compare_given_null_and_null_returns_0()
    {
        var comparer = new InternalComparer<string>(StringComparer.InvariantCultureIgnoreCase.Compare);

        Assert.That(comparer.Compare(null, null), Is.EqualTo(0));
    }

    [Test]
    public void Compare_given_null_and_something_returns_minus_1()
    {
        var comparer = new InternalComparer<string>(StringComparer.InvariantCultureIgnoreCase.Compare);

        Assert.That(comparer.Compare(null, "something"), Is.EqualTo(-1));
    }

    [Test]
    public void Compare_given_something_and_null_returns_1()
    {
        var comparer = new InternalComparer<string>(StringComparer.InvariantCultureIgnoreCase.Compare);

        Assert.That(comparer.Compare("something", null), Is.EqualTo(1));
    }
}
