// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5;

/// <summary>
/// A read-only wrapper for a dictionary.
///
/// <i>Suitable for wrapping a HashDictionary. <see cref="T:C5.HashDictionary`2"/></i>
/// </summary>
public class GuardedDictionary<K, V> : GuardedCollectionValue<SCG.KeyValuePair<K, V>>, IDictionary<K, V>
{
    #region Fields

    private readonly IDictionary<K, V> dict;

    #endregion

    #region Constructor

    /// <summary>
    /// Wrap a dictionary in a read-only wrapper
    /// </summary>
    /// <param name="dict">the dictionary</param>
    public GuardedDictionary(IDictionary<K, V> dict) : base(dict) { this.dict = dict; }

    #endregion

    #region IDictionary<K,V> Members

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public SCG.IEqualityComparer<K> EqualityComparer => dict.EqualityComparer;

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a
    /// read-only wrapper if used as a setter</exception>
    /// <value>Get the value corresponding to a key in the wrapped dictionary</value>
    public V this[K key]
    {
        get => dict[key];
        set => throw new ReadOnlyCollectionException();
    }

    /// <summary>
    /// (This is a read-only wrapper)
    /// </summary>
    /// <value>True</value>
    public bool IsReadOnly => true;

    /// <summary> </summary>
    /// <value>The collection of keys of the wrapped dictionary</value>
    public ICollectionValue<K> Keys => new GuardedCollectionValue<K>(dict.Keys);


    /// <summary> </summary>
    /// <value>The collection of values of the wrapped dictionary</value>
    public ICollectionValue<V> Values => new GuardedCollectionValue<V>(dict.Values);

    /// <summary>
    ///
    /// </summary>
    public virtual Func<K, V> Func => k => this[k];

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    public void Add(K key, V val) => throw new ReadOnlyCollectionException();

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="items"></param>
    public void AddAll<L, W>(SCG.IEnumerable<SCG.KeyValuePair<L, W>> items)
        where L : K
        where W : V => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(K key) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public bool Remove(K key, out V val) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    public void Clear() => throw new ReadOnlyCollectionException();

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    public Speed ContainsSpeed => dict.ContainsSpeed;

    /// <summary>
    /// Check if the wrapped dictionary contains a specific key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>True if it does</returns>
    public bool Contains(K key) => dict.Contains(key);

    /// <summary>
    ///
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public bool ContainsAll<H>(SCG.IEnumerable<H> keys) where H : K  => dict.ContainsAll(keys);

    /// <summary>
    /// Search for a key in the wrapped dictionary, reporting the value if found
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="val">On exit: the value if found</param>
    /// <returns>True if found</returns>
    public bool Find(ref K key, out V val) => dict.Find(ref key, out val);

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public bool Update(K key, V val) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="oldval"></param>
    /// <returns></returns>
    public bool Update(K key, V val, out V oldval) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public bool FindOrAdd(K key, ref V val) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public bool UpdateOrAdd(K key, V val) => throw new ReadOnlyCollectionException();

    /// <summary>
    /// </summary>
    /// <exception cref="ReadOnlyCollectionException"> since this is a read-only wrapper</exception>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="oldval"></param>
    /// <returns></returns>
    public bool UpdateOrAdd(K key, V val, out V oldval) => throw new ReadOnlyCollectionException();


    /// <summary>
    /// Check the internal consistency of the wrapped dictionary
    /// </summary>
    /// <returns>True if check passed</returns>
    public bool Check() { return dict.Check(); }

    #endregion
}