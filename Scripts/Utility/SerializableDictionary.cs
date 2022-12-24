using System;
using System.Collections.Generic;
using UnityEngine;

//https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> m_Keys = new List<TKey>();
    [SerializeField] private List<TValue> m_Values = new List<TValue>();

    // Save the dictionary to lists
    public void OnBeforeSerialize()
    {
        m_Keys.Clear();
        m_Values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            m_Keys.Add(pair.Key);
            m_Values.Add(pair.Value);
        }
    }

    // Load dictionary from lists
    public void OnAfterDeserialize()
    {
        Clear();

        if (m_Keys.Count != m_Values.Count)
            throw new Exception($"There are {m_Keys.Count} keys and {m_Values.Count} values after deserialization. Make sure that both key and value types are serializable.");

        for (int i = 0; i < m_Keys.Count; i++)
            Add(m_Keys[i], m_Values[i]);
    }
}
