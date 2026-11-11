using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 字典映射类
[Serializable]
public struct SerializableDictionary<TKey , TValue>
{
    public TKey key;
    public TValue value;

    public SerializableDictionary(TKey key , TValue value)
    {
        this.key = key;
        this.value = value;
    }

    public TValue GetValue(TKey key)
    {
        if (this.key.Equals(key))
        {
            return value;
        }
        else
        {
            throw new KeyNotFoundException($"Key {key} not found in SerializableDictionary.");
        }
    }

    public TKey GetKey(TValue value)
    {
        if (this.value.Equals(value))
        {
            return key;
        }
        else
        {
            throw new KeyNotFoundException($"Value {value} not found in SerializableDictionary.");
        }
    }
}
#endregion
