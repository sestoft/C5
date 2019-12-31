using System;

namespace C5
{
    /// <summary>
    /// Base class for collection classes of dynamic array type implementations.
    /// </summary>
    [Serializable]
    public abstract class ArrayBase<T> : SequencedBase<T>
    {
        #region Fields
        /// <summary>
        /// The actual internal array container. Will be extended on demand.
        /// </summary>
        protected T[] array;

        /// <summary>
        /// The offset into the internal array container of the first item. The offset is 0 for a 
        /// base dynamic array and may be positive for an updatable view into a base dynamic array.
        /// </summary>
        protected int offsetField;
        #endregion

        #region Util
        /// <summary>
        /// Double the size of the internal array.
        /// </summary>
        protected virtual void Expand()
        {
            Expand(checked(2 * array.Length), size);
        }


        /// <summary>
        /// Expand the internal array container.
        /// </summary>
        /// <param name="newcapacity">The new size of the internal array - 
        /// will be rounded upwards to a power of 2.</param>
        /// <param name="newsize">The (new) size of the (base) collection.</param>
        protected virtual void Expand(int newcapacity, int newsize)
        {
            System.Diagnostics.Debug.Assert(newcapacity >= newsize);

            int newlength = array.Length;

            while (newlength < newcapacity)
            {
                newlength = checked(newlength * 2);
            }

            T[] newarray = new T[newlength];

            Array.Copy(array, newarray, newsize);
            array = newarray;
        }


        /// <summary>
        /// Insert an item at a specific index, moving items to the right
        /// upwards and expanding the array if necessary.
        /// </summary>
        /// <param name="i">The index at which to insert.</param>
        /// <param name="item">The item to insert.</param>
        protected virtual void InsertProtected(int i, T item)
        {
            if (size == array.Length)
            {
                Expand();
            }

            if (i < size)
            {
                Array.Copy(array, i, array, i + 1, size - i);
            }

            array[i] = item;
            size++;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create an empty ArrayBase object.
        /// </summary>
        /// <param name="capacity">The initial capacity of the internal array container.
        /// Will be rounded upwards to the nearest power of 2 greater than or equal to 8.</param>
        /// <param name="itemequalityComparer">The item equalityComparer to use, primarily for item equality</param>
        protected ArrayBase(int capacity, System.Collections.Generic.IEqualityComparer<T> itemequalityComparer)
            : base(itemequalityComparer)
        {
            int newlength = 8;
            while (newlength < capacity)
            {
                newlength *= 2;
            }

            array = new T[newlength];
        }

        #endregion

        #region IIndexed members

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the arguments does not describe a 
        /// valid range in the indexed collection, cf. <see cref="M:C5.CollectionBase`1.checkRange(System.Int32,System.Int32)"/>.</exception>
        /// <value>The directed collection of items in a specific index interval.</value>
        /// <param name="start">The low index of the interval (inclusive).</param>
        /// <param name="count">The size of the range.</param>
        public virtual IDirectedCollectionValue<T> this[int start, int count]
        {
            get
            {
                CheckRange(start, count);
                return new Range(this, start, count, true);
            }
        }

        #endregion

        #region IEditableCollection members
        /// <summary>
        /// Remove all items and reset size of internal array container.
        /// </summary>
        public override void Clear()
        {
            UpdateCheck();
            array = new T[8];
            size = 0;
        }


        /// <summary>
        /// Create an array containing (copies) of the items of this collection in enumeration order.
        /// </summary>
        /// <returns>The new array</returns>
        public override T[] ToArray()
        {
            T[] res = new T[size];

            Array.Copy(array, offsetField, res, 0, size);
            return res;
        }


        /// <summary>
        /// Perform an internal consistency (invariant) test on the array base.
        /// </summary>
        /// <returns>True if test succeeds.</returns>
        public virtual bool Check()
        {
            bool retval = true;

            if (size > array.Length)
            {
                Logger.Log(string.Format("Bad size ({0}) > array.Length ({1})", size, array.Length));
                return false;
            }

            for (int i = 0; i < size; i++)
            {
                if (array[i] == null)
                {
                    Logger.Log(string.Format("Bad element: null at index {0}", i));
                    return false;
                }
            }

            return retval;
        }

        #endregion

        #region IDirectedCollection<T> Members

        /// <summary>
        /// Create a directed collection with the same contents as this one, but 
        /// opposite enumeration sequence.
        /// </summary>
        /// <returns>The mirrored collection.</returns>
        public override IDirectedCollectionValue<T> Backwards() { return this[0, size].Backwards(); }

        #endregion

        /// <summary>
        /// Choose some item of this collection. The result is the last item in the internal array,
        /// making it efficient to remove.
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override T Choose() { if (size > 0) { return array[size - 1]; } throw new NoSuchItemException(); }

        #region IEnumerable<T> Members
        /// <summary>
        /// Create an enumerator for this array based collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public override System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            int thestamp = stamp, theend = size + offsetField, thestart = offsetField;

            for (int i = thestart; i < theend; i++)
            {
                ModifyCheck(thestamp);
                yield return array[i];
            }
        }
        #endregion

        #region Range nested class
        /// <summary>
        /// A helper class for defining results of interval queries on array based collections.
        /// </summary>
        [Serializable]
        protected class Range : DirectedCollectionValueBase<T>, IDirectedCollectionValue<T>
        {
            private int start;
            private readonly int count;
            private int delta;
            private readonly int stamp;
            private readonly ArrayBase<T> thebase;


            internal Range(ArrayBase<T> thebase, int start, int count, bool forwards)
            {
                this.thebase = thebase; stamp = thebase.stamp;
                delta = forwards ? 1 : -1;
                this.start = start + thebase.offsetField; this.count = count;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>True if this collection is empty.</value>
            public override bool IsEmpty { get { thebase.ModifyCheck(stamp); return count == 0; } }

            /// <summary>
            /// Gets a value indicating whether the <see cref="Range"/> is read-only. Always returns <c>true</c>.
            /// </summary>
            public override bool IsReadOnly => true;

            /// <summary>
            /// 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>The number of items in the range</value>
            public override int Count { get { thebase.ModifyCheck(stamp); return count; } }

            /// <summary>
            /// The value is symbolic indicating the type of asymptotic complexity
            /// in terms of the size of this collection (worst-case or amortized as
            /// relevant).
            /// </summary>
            /// <value>A characterization of the speed of the 
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <code>Count</code> property in this collection.</value>
            public override Speed CountSpeed { get { thebase.ModifyCheck(stamp); return Speed.Constant; } }

            /// <summary>
            /// Choose some item of this collection. 
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <exception cref="NoSuchItemException">if range is empty.</exception>
            /// <returns></returns>
            public override T Choose()
            {
                thebase.ModifyCheck(stamp);
                if (count == 0)
                {
                    throw new NoSuchItemException();
                }

                return thebase.array[start];
            }


            /// <summary>
            /// Create an enumerator for this range of an array based collection.
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <returns>The enumerator</returns>
            public override System.Collections.Generic.IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < count; i++)
                {
                    thebase.ModifyCheck(stamp);
                    yield return thebase.array[start + delta * i];
                }
            }


            /// <summary>
            /// Create an array collection range with the same contents as this one, but 
            /// opposite enumeration sequence.
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <returns>The mirrored collection.</returns>
            public override IDirectedCollectionValue<T> Backwards()
            {
                thebase.ModifyCheck(stamp);

                Range res = (Range)MemberwiseClone();

                res.delta = -delta;
                res.start = start + (count - 1) * delta;
                return res;
            }


            IDirectedEnumerable<T> C5.IDirectedEnumerable<T>.Backwards()
            {
                return Backwards();
            }


            /// <summary>
            /// <code>Forwards</code> if same, else <code>Backwards</code>
            /// </summary>
            /// <exception cref="CollectionModifiedException">if underlying collection has been modified.</exception>
            /// <value>The enumeration direction relative to the original collection.</value>
            public override EnumerationDirection Direction
            {
                get
                {
                    thebase.ModifyCheck(stamp);
                    return delta > 0 ? EnumerationDirection.Forwards : EnumerationDirection.Backwards;
                }
            }
        }
        #endregion
    }
}