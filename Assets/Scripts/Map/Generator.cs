using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Generator : MonoBehaviour
{
#if UNITY_EDITOR

    public abstract void Generate();
    public abstract void Clean();
    public PrefabLibrary prefabLibrary;

    public void PlaceInRange(Transform target, Vector2 placementRange, Vector2 basePos)
    {
        float randomPosX = Random.Range(-placementRange.x / 2f, placementRange.x / 2f);
        float randomPosZ = Random.Range(-placementRange.y / 2f, placementRange.y / 2f);
        Vector3 randomPos = new Vector3(basePos.x + randomPosX, 0, basePos.y + randomPosZ);
        target.localPosition = randomPos;
    }

    public Transform GenerateInRange(Transform container, GameObject prefabs, Vector2 placementRange, Vector2 basePos)
    {
        Transform generated = (PrefabUtility.InstantiatePrefab(prefabs, container) as GameObject).transform;
        PlaceInRange(generated, placementRange, basePos);
        return generated;
    }

    public Transform GenerateRandomInRange(Transform container, GameObject[] prefabs, Vector2 placementRange, Vector2 basePos)
    {
        int randomPrefabIndex = Random.Range(0, prefabs.Length);
        return GenerateInRange(container, prefabs[randomPrefabIndex], placementRange, basePos);
    }
#endif
}
