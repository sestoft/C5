// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
public class CircularQueue<T> : SequencedBase<T>, IQueue<T>, IStack<T>
{
    #region Fields
    /*
    Invariant: the itemes in the queue ar the elements from front upwards,
    possibly wrapping around at the end of array, to back.

    if front<=back then size = back - front + 1;
    else size = array.Length + back - front + 1;

    */
    private int front, back;

    /// <summary>
    /// The internal container array is doubled when necessary, but never shrinked.
    /// </summary>
    private T[] array;
    private bool forwards = true;
    private bool original = true;
    #endregion

    #region Events

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public override EventType ListenableEvents => EventType.Basic;

    #endregion

    #region Util
    private void Expand()
    {
        int newlength = 2 * array.Length;
        T[] newarray = new T[newlength];

        if (front <= back)
        {
            Array.Copy(array, front, newarray, 0, size);
        }
        else
        {
            int half = array.Length - front;
            Array.Copy(array, front, newarray, 0, half);
            Array.Copy(array, 0, newarray, half, size - half);
        }

        front = 0;
        back = size;
        array = newarray;
    }

    #endregion

    #region Constructors

    /// <summary>
    ///
    /// </summary>
    public CircularQueue() : this(8) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="capacity"></param>
    public CircularQueue(int capacity)
        : base(EqualityComparer<T>.Default)
    {
        int newlength = 8;
        while (newlength < capacity)
        {
            newlength *= 2;
        }

        array = new T[newlength];
    }

    #endregion

    #region IQueue<T> Members
    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public virtual bool AllowsDuplicates => true;

    /// <summary>
    /// Get the i'th item in the queue. The front of the queue is at index 0.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public virtual T this[int i]
    {
        get
        {
            if (i < 0 || i >= size)
            {
                throw new IndexOutOfRangeException();
            }

            i += front;
            //Bug fix by Steve Wallace 2006/02/10
            return array[i >= array.Length ? i - array.Length : i];
        }
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    public virtual void Enqueue(T item)
    {
        if (!original)
        {
            throw new ReadOnlyCollectionException();
        }

        stamp++;
        if (size == array.Length - 1)
        {
            Expand();
        }

        size++;
        int oldback = back++;
        if (back == array.Length)
        {
            back = 0;
        }

        array[oldback] = item;
        if (ActiveEvents != 0)
        {
            RaiseForAdd(item);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual T Dequeue()
    {
        if (!original)
        {
            throw new ReadOnlyCollectionException("Object is a non-updatable clone");
        }

        stamp++;
        if (size == 0)
        {
            throw new NoSuchItemException();
        }

        size--;
        int oldfront = front++;
        if (front == array.Length)
        {
            front = 0;
        }

        T retval = array[oldfront];
        // array[oldfront] = default;
        // We can't use default as it would be null -> we just store garbage
        if (ActiveEvents != 0)
        {
            RaiseForRemove(retval);
        }

        return retval;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    public void Push(T item) //== Enqueue
    {
        if (!original)
        {
            throw new ReadOnlyCollectionException();
        }

        stamp++;
        if (size == array.Length - 1)
        {
            Expand();
        }

        size++;
        int oldback = back++;
        if (back == array.Length)
        {
            back = 0;
        }

        array[oldback] = item;
        if (ActiveEvents != 0)
        {
            RaiseForAdd(item);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (!original)
        {
            throw new ReadOnlyCollectionException("Object is a non-updatable clone");
        }

        stamp++;
        if (size == 0)
        {
            throw new NoSuchItemException();
        }

        size--;
        back--;
        if (back == -1)
        {
            back = array.Length - 1;
        }

        T retval = array[back];
        //array[back] = default;
        //We can't use default -> just store garbage
        if (ActiveEvents != 0)
        {
            RaiseForRemove(retval);
        }

        return retval;
    }
    #endregion

    #region ICollectionValue<T> Members

    //TODO: implement these with Array.Copy instead of relying on XxxBase:
    /*
        public void CopyTo(T[] a, int i)
        {
        }

        public T[] ToArray()
        {
        }*/

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override T Choose()
    {
        if (size == 0)
        {
            throw new NoSuchItemException();
        }

        return array[front];
    }

    #endregion

    #region IEnumerable<T> Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override SCG.IEnumerator<T> GetEnumerator()
    {
        int stamp = this.stamp;
        if (forwards)
        {
            int position = front;
            int end = front <= back ? back : array.Length;
            while (position < end)
            {
                if (stamp != this.stamp)
                {
                    throw new CollectionModifiedException();
                }

                yield return array[position++];
            }
            if (front > back)
            {
                position = 0;
                while (position < back)
                {
                    if (stamp != this.stamp)
                    {
                        throw new CollectionModifiedException();
                    }

                    yield return array[position++];
                }
            }
        }
        else
        {
            int position = back - 1;
            int end = front <= back ? front : 0;
            while (position >= end)
            {
                if (stamp != this.stamp)
                {
                    throw new CollectionModifiedException();
                }

                yield return array[position--];
            }
            if (front > back)
            {
                position = array.Length - 1;
                while (position >= front)
                {
                    if (stamp != this.stamp)
                    {
                        throw new CollectionModifiedException();
                    }

                    yield return array[position--];
                }
            }
        }
    }

    #endregion

    #region IDirectedCollectionValue<T> Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override IDirectedCollectionValue<T> Backwards()
    {
        CircularQueue<T> retval = (CircularQueue<T>)MemberwiseClone();
        retval.original = false;
        retval.forwards = !forwards;
        return retval;
    }

    #endregion

    #region IDirectedEnumerable<T> Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
        return Backwards();
    }

    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual bool Check()
    {
        if (front < 0 || front >= array.Length || back < 0 || back >= array.Length ||
            (front <= back && size != back - front) || (front > back && size != array.Length + back - front))
        {
            Logger.Log(string.Format("Bad combination of (front,back,size,array.Length): ({0},{1},{2},{3})",
                front, back, size, array.Length));
            return false;
        }
        return true;
    }
}