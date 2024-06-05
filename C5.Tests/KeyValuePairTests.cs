// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using SCG = System.Collections.Generic;

namespace C5.Tests;

public class KeyValuePairTests
{
    [Test]
    public void Create()
    {
        var p1 = new SCG.KeyValuePair<int, string>(42, "The answer");
        var p2 = SCG.KeyValuePair.Create(42, "The answer");

        Assert.That(p2, Is.EqualTo(p1));
    }
}
