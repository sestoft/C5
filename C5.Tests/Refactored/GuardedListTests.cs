// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;

namespace C5.Tests;

public class GuardedListTests
{
    [Test]
    public void IList_Contains_given_null_when_T_is_value_type_returns_false()
    {
        System.Collections.IList list = new GuardedList<int>(new ArrayList<int> { 1, 2, 3 });

        Assert.That(list.Contains(null), Is.False);
    }

    [Test]
    public void IList_Contains_given_wrong_type_returns_false()
    {
        System.Collections.IList list = new GuardedList<int>(new ArrayList<int> { 1, 2, 3 });

        Assert.That(list.Contains("four"), Is.False);
    }

    [Test]
    public void IList_IndexOf_given_null_when_T_is_value_type_returns_minus_1()
    {
        System.Collections.IList list = new GuardedList<int>(new ArrayList<int> { 1, 2, 3 });

        Assert.That(list.IndexOf(null), Is.EqualTo(-1));
    }

    [Test]
    public void IList_IndexOf_given_wrong_type_returns()
    {
        System.Collections.IList list = new GuardedList<int>(new ArrayList<int> { 1, 2, 3 });

        Assert.DoesNotThrow(() => list.IndexOf("four"));
    }
}
