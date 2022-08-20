using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnManager
{
    [SerializeField] private List<GameObject> m_zombiePrefabs;
    [SerializeField] private GameObject m_fxPoufPrefab;

    public GameObject SpawnZombie(Vector3 _position, Quaternion _rotation)
    {
        int prefabIndex = UnityEngine.Random.Range(0, m_zombiePrefabs.Count);
        return GameObject.Instantiate(m_zombiePrefabs[prefabIndex], _position, _rotation);
    }

    public GameObject SpawnPouf(Vector3 _position)
    {
        int prefabIndex = UnityEngine.Random.Range(0, m_zombiePrefabs.Count);
        return GameObject.Instantiate(m_fxPoufPrefab, _position, Quaternion.identity);
    }
}
