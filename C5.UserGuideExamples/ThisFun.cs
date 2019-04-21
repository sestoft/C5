// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Experiment: implicit conversion of indexer to function
// sestoft@dina.kvl.dk * 2005-11-08

using System;
using C5;

class MyFunTest
{
    public static void Main(String[] args)
    {
        FooBar fb = new FooBar();
        IList<int> list = new LinkedList<int>();
        list.AddAll(new[] { 2, 3, 5, 7, 11 });
        list.Map<double>(fb).Apply(Console.WriteLine);
        list.Apply(fb);
    }
}

class FooBar
{
    public double this[int x]
    {
        get
        {
            Console.WriteLine(x);
            return x + 1.5;
        }
    }

    public Func<int, double> Func
    {
        get
        {
            return delegate(int x) { return this[x]; };
        }
    }

    public Action<int> Action
    {
        get
        {
            return delegate(int x) { double junk = this[x]; };
        }
    }

    public static implicit operator Func<int, double>(FooBar fb)
    {
        return delegate(int x) { return fb[x]; };
    }

    public static implicit operator Action<int>(FooBar fb)
    {
        return delegate(int x) { double junk = fb[x]; };
    }
}


