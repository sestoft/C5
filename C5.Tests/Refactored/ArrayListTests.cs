// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests;

public class ArrayListTests
{
    [Test]
    public void ArrayList_Implements_IReadOnlyList()
    {
        var list = new ArrayList<int> { 0, 1, 2, 3, 4 } as SCG.IReadOnlyList<int>;

        Assert.That(list, Has.Count.EqualTo(5));
        Assert.That(list[2], Is.EqualTo(2));
    }

    [Test]
    public void IList_index_set_given_null_when_T_is_value_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentNullException>(() => list[1] = null);
    }

    [Test]
    public void IList_index_set_given_wrong_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentException>(() => list[1] = "four");
    }

    [Test]
    public void IList_Add_given_null_when_T_is_value_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentNullException>(() => list.Add(null));
    }

    [Test]
    public void IList_Add_given_wrong_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentException>(() => list.Add("four"));
    }

    [Test]
    public void IList_Contains_given_null_when_T_is_value_type_returns_false()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.That(list.Contains(null), Is.False);
    }

    [Test]
    public void IList_Contains_given_wrong_type_returns_false()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.That(list.Contains("four"), Is.False);
    }

    [Test]
    public void IList_IndexOf_given_null_when_T_is_value_type_returns_minus_1()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.That(list.IndexOf(null), Is.EqualTo(-1));
    }

    [Test]
    public void IList_IndexOf_given_wrong_type_returns()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.DoesNotThrow(() => list.IndexOf("four"));
    }

    [Test]
    public void IList_Insert_given_null_when_T_is_value_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentNullException>(() => list.Insert(1, null));
    }


    [Test]
    public void IList_Insert_given_wrong_type_throws()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.Throws<ArgumentException>(() => list.Insert(1, "two"));
    }

    [Test]
    public void IList_Remove_given_null_when_T_is_value_type_returns()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.DoesNotThrow(() => list.Remove(null));
    }

    [Test]
    public void IList_Remove_given_wrong_type_returns()
    {
        System.Collections.IList list = new ArrayList<int> { 1, 2, 3 };

        Assert.DoesNotThrow(() => list.Remove("four"));
    }
}
