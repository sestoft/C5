// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

/// <summary>
/// The base type of a priority queue handle
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPriorityQueueHandle<T>
{
    //TODO: make abstract and prepare for double dispatch:
    //public virtual bool Delete(IPriorityQueue<T> q) { throw new InvalidFooException();}
    //bool Replace(T item);
}