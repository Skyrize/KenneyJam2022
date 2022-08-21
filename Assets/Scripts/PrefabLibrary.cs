using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PrefabLibrary", order = 1)]
public class PrefabLibrary : ScriptableObject
{
    public GameObject fencePrefab;
    public GameObject[] housePrefabs;
    public GameObject[] decorationPrefabs;
    public GameObject[] survivorPrefabs;
    public GameObject[] armedSurvivorPrefabs;
    public GameObject roadPrefab;
    public GameObject intersectRoadPrefab;
    public GameObject floorPrefab;
    public Material[] roofMaterials;
    public Material[] wallMaterials;
    public Material[] grassMaterials;
    public Material[] woodMaterials;
    public GameObject barrierPrefab;
    public GameObject[] cityPrefab;
    public GameObject cityFloorPrefab;
    public GameObject colliderPrefab;
    public GameObject landminePrefab;
}