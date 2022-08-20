using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnManager
{
    [SerializeField] private List<GameObject> m_zombiePrefabs;
    [SerializeField] private List<GameObject> m_survivorPrefabs;
    [SerializeField] private GameObject m_fxPoufPrefab;
    [SerializeField] private GameObject m_fxHitPrefab;

    public GameObject SpawnZombie(Vector3 _position, Quaternion _rotation)
    {
        int prefabIndex = UnityEngine.Random.Range(0, m_zombiePrefabs.Count);
        return GameObject.Instantiate(m_zombiePrefabs[prefabIndex], _position, _rotation);
    }

    public GameObject SpawnSurvivor(Vector3 _position, Quaternion _rotation)
    {
        int prefabIndex = UnityEngine.Random.Range(0, m_survivorPrefabs.Count);
        return GameObject.Instantiate(m_survivorPrefabs[prefabIndex], _position, _rotation);
    }

    public GameObject SpawnPouf(Vector3 _position)
    {
        return GameObject.Instantiate(m_fxPoufPrefab, _position, Quaternion.identity);
    }

    public GameObject SpawnHit(Vector3 _position)
    {
        return GameObject.Instantiate(m_fxHitPrefab, _position, Quaternion.identity);
    }
}
