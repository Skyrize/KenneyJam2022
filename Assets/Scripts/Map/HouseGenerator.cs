using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HouseGenerator : Generator
{
#if UNITY_EDITOR

    public Vector2 size = new Vector2(3, 2);
    public Vector2 housePlacementPos = new Vector2(-0.5f, 0.1f);
    public Vector2 housePlacementRange = new Vector2(3, 0.5f);
    public Vector2 decorPlacementPos = new Vector2(-0.5f, 0.1f);
    public Vector2 decorPlacementRange = new Vector2(3, 0.5f);
    public int minDecorAmount = 1;
    public int maxDecorAmount = 3;
    public float fencePlacementDist = 1.5f;
    public int minSurvivorAmount = 1;
    public int maxSurvivorAmount = 3;

    public Transform housesContainer;

    public override void Clean()
    {
        housesContainer.DestroyChilds();
    }

    Transform GenerateRandomHouseInRange(Transform container, Vector2 placementRange, Vector2 basePos)
    {
        int randomPrefabIndex = Random.Range(0, prefabLibrary.housePrefabs.Length);
        Material randomRoofMat = prefabLibrary.roofMaterials[Random.Range(0, prefabLibrary.roofMaterials.Length)];
        Material randomWallMat = prefabLibrary.wallMaterials[Random.Range(0, prefabLibrary.wallMaterials.Length)];
        Transform house = GenerateInRange(container, prefabLibrary.housePrefabs[randomPrefabIndex], placementRange, basePos);
        MeshRenderer meshRenderer = house.GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = house.GetComponentInChildren<MeshRenderer>();
        Material[] materials = meshRenderer.sharedMaterials;
        for (int i = 0; i != materials.Length; i++)
        {
            if (materials[i].name == "roof")
            {
                materials[i] = randomRoofMat;
            }
            if (materials[i].name == "_defaultMat")
            {
                materials[i] = randomWallMat;
            }
        }
        meshRenderer.sharedMaterials = materials;
        return house;
    }

    void GenerateFence(Transform container, Vector3 rotation, Vector3 direction, Material woodMat)
    {
        Transform fenceLeft = (PrefabUtility.InstantiatePrefab(prefabLibrary.fencePrefab, container) as GameObject).transform;
        fenceLeft.localEulerAngles = rotation;
        fenceLeft.localPosition = direction * fencePlacementDist;
        MeshRenderer meshRenderer = fenceLeft.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.sharedMaterials;
        for (int i = 0; i != materials.Length; i++)
        {
            if (materials[i].name == "wood")
            {
                materials[i] = woodMat;
            }
        }
        meshRenderer.sharedMaterials = materials;
    }

    void GenerateFences(Transform container)
    {
        Material randomWoodMat = prefabLibrary.woodMaterials[Random.Range(0, prefabLibrary.woodMaterials.Length)];
        GenerateFence(container, new Vector3(0, -90, 0), Vector3.left, randomWoodMat);
        GenerateFence(container, new Vector3(0, 90, 0), Vector3.right, randomWoodMat);
    }

    public override void Generate()
    {
        Transform houseContainer = new GameObject("House").transform;
        houseContainer.position = transform.position;
        houseContainer.rotation = transform.rotation;
        houseContainer.parent = housesContainer;

        //Place house
        GenerateRandomHouseInRange(houseContainer, housePlacementRange, housePlacementPos);

        //Place Fences
        GenerateFences(houseContainer);

        //Place Decors and Survivors
        int randomDecorAmount = Random.Range(minDecorAmount, maxDecorAmount + 1);
        for (int i = 0; i != randomDecorAmount; i++)
        {
            GenerateRandomInRange(houseContainer, prefabLibrary.decorationPrefabs, decorPlacementRange, decorPlacementPos);
        }
        int randomSurvivorAmount = Random.Range(minSurvivorAmount, maxSurvivorAmount + 1);
        for (int i = 0; i != randomSurvivorAmount; i++)
        {
            GenerateRandomInRange(houseContainer, prefabLibrary.survivorPrefabs, decorPlacementRange, decorPlacementPos);
        }
    }

    private void OnDrawGizmos() {
        //Draw full
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix; 
        Vector3 debugSize = new Vector3(size.x, 20, size.y);
        Gizmos.DrawWireCube(Vector3.zero, debugSize);

        //Draw house placement
        Gizmos.color = Color.red;
        debugSize = new Vector3(housePlacementRange.x, 20, housePlacementRange.y);
        Gizmos.DrawWireCube(new Vector3(housePlacementPos.x, 0, housePlacementPos.y), debugSize);

        //Draw decor placement
        Gizmos.color = Color.yellow;
        debugSize = new Vector3(decorPlacementRange.x, 20, decorPlacementRange.y);
        Gizmos.DrawWireCube(new Vector3(decorPlacementPos.x, 0, decorPlacementPos.y), debugSize);

        //Draw fence placement
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Vector3.left * fencePlacementDist, new Vector3(2f, 20, size.x));
        Gizmos.DrawWireCube(-Vector3.left * fencePlacementDist, new Vector3(2f, 20, size.x));
    }

#endif
}
