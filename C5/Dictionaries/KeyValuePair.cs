using System;

namespace C5
{
    /// <summary>
    /// An entry in a dictionary from K to V.
    /// </summary>
    [Serializable]
    public struct KeyValuePair<K, V> : IEquatable<KeyValuePair<K, V>>, IShowable
    {
        /// <summary>
        /// The key field of the entry
        /// </summary>
        public K Key;

        /// <summary>
        /// The value field of the entry
        /// </summary>
        public V Value;

        /// <summary>
        /// Create an entry with specified key and value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public KeyValuePair(K key, V value) { Key = key; Value = value; }


        /// <summary>
        /// Create an entry with a specified key. The value will be the default value of type <code>V</code>.
        /// </summary>
        /// <param name="key">The key</param>
        public KeyValuePair(K key) { Key = key; Value = default; }


        /// <summary>
        /// Pretty print an entry
        /// </summary>
        /// <returns>(key, value)</returns>
        public override string ToString() { return "(" + Key + ", " + Value + ")"; }


        /// <summary>
        /// Check equality of entries. 
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>True if obj is an entry of the same type and has the same key and value</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is KeyValuePair<K, V>))
            {
                return false;
            }

            KeyValuePair<K, V> other = (KeyValuePair<K, V>)obj;
            return Equals(other);
        }

        /// <summary>
        /// Get the hash code of the pair.
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() { return EqualityComparer<K>.Default.GetHashCode(Key) + 13984681 * EqualityComparer<V>.Default.GetHashCode(Value); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(KeyValuePair<K, V> other)
        {
            return EqualityComparer<K>.Default.Equals(Key, other.Key) && EqualityComparer<V>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair1"></param>
        /// <param name="pair2"></param>
        /// <returns></returns>
        public static bool operator ==(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2) { return pair1.Equals(pair2); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair1"></param>
        /// <param name="pair2"></param>
        /// <returns></returns>
        public static bool operator !=(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2) { return !pair1.Equals(pair2); }

        #region IShowable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="formatProvider"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            if (rest < 0)
            {
                return false;
            }

            if (!Showing.Show(Key, stringbuilder, ref rest, formatProvider))
            {
                return false;
            }

            stringbuilder.Append(" => ");
            rest -= 4;
            if (!Showing.Show(Value, stringbuilder, ref rest, formatProvider))
            {
                return false;
            }

            return rest >= 0;
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Showing.ShowString(this, format, formatProvider);
        }

        #endregion
    }

    /// <summary>
    /// Static class to allow creation of KeyValuePair using type inference
    /// </summary>
    public static class KeyValuePair
    {
        /// <summary>
        /// Create an instance of the KeyValuePair using type inference.
        /// </summary>
        public static KeyValuePair<K, V> Create<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}