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
    [SerializeField] private GameObject m_fxDestructionPrefab;

    private List<Collider> colliderBuffer = new List<Collider>(5);

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

    public void SpawnDestruction(GameObject _destroyedObject, ref List<ParticleSystem> _particlesOutput)
    {
        const float PADDING = 1.0f;

        colliderBuffer.Clear();
        _destroyedObject.GetComponents<Collider>(colliderBuffer);
        foreach (Collider collider in colliderBuffer)
        {
            ParticleSystem particles = GameObject.Instantiate(m_fxDestructionPrefab).GetComponent<ParticleSystem>();
            
            Vector3 spawnPosition = collider.bounds.center;
            spawnPosition.y = -1;
            particles.transform.position = spawnPosition;
            
            ParticleSystem.ShapeModule shapeModule = particles.shape;
            shapeModule.scale = new Vector3(collider.bounds.extents.x * 2 + PADDING, collider.bounds.extents.z * 2 + PADDING, 0.0f);
            
            _particlesOutput.Add(particles);
        }

        for (int i = 0; i < _destroyedObject.transform.childCount; ++i)
            SpawnDestruction(_destroyedObject.transform.GetChild(i).gameObject, ref _particlesOutput);
    }
}
