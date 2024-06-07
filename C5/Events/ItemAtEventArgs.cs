// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;

namespace C5;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ItemAtEventArgs<T> : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// 
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    public ItemAtEventArgs(T item, int index) { Item = item; Index = index; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("(ItemAtEventArgs {0} '{1}')", Index, Item);
    }
}