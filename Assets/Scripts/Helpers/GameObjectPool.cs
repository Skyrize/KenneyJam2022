using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectPoolConfig
{
    public GameObject prefab;
    public uint size;
}

[Serializable]
public class GameObjectPool
{
    [SerializeField] private Transform m_destination;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private uint m_size = 1;

    private List<GameObject> m_objects;

    public void SetDestination(Transform _destination)
    {
        Debug.Assert(m_objects is null);

        m_destination = _destination;
    }

    public void SetConfig(GameObjectPoolConfig _config)
    {
        Debug.Assert(m_objects is null);

        m_prefab = _config.prefab;
        m_size = _config.size;
    }

    public void Initialize()
    {
        m_objects = new List<GameObject>((int)m_size);
        for (int i = 0; i < m_size; ++i)
        {
            m_objects.Add(MakeNewObject(false));
        }
    }

    public void Terminate()
    {
        foreach (GameObject go in m_objects)
        {
            GameObject.Destroy(go);
        }
        m_objects.Clear();
        m_objects = null;
    }

    public GameObject GetObject()
    {
        for (int i = 0; i < m_objects.Count; ++i)
        {
            GameObject go = m_objects[i];
            if (go == null)
            {
                go = MakeNewObject(true);
                m_objects[i] = go;
                return go;
            }
            else if (!go.activeSelf)
            {
                go.SetActive(true);
                return go;
            }
        }
        {
            Debug.Log("Pool(" + m_prefab.name + ") capacity has been extended, consider increasing the pool size.");
            GameObject go = MakeNewObject(true);
            m_objects.Add(go);
            return go;
        }
    }

    private GameObject MakeNewObject(bool _activated)
    {
        GameObject go = GameObject.Instantiate(m_prefab, m_destination);
        go.SetActive(_activated);
        return go;
    }
}
