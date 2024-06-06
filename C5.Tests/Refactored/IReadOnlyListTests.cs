// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System.Collections.Generic;

namespace C5.Tests;

public class IReadOnlyListTests
{
    [Test]
    public void ArrayList_Implements_IReadOnlyList()
    {
        var list = new ArrayList<int> { 0, 1, 2, 3, 4 } as IReadOnlyList<int>;

        Assert.That(list.Count, Is.EqualTo(5));
        Assert.That(list[2], Is.EqualTo(2));
    }
}