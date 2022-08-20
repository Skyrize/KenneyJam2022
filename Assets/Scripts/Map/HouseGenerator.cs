using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HouseGenerator : Generator
{
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

    public override void Generate()
    {
        Transform houseContainer = new GameObject("House").transform;
        houseContainer.position = transform.position;
        houseContainer.rotation = transform.rotation;
        houseContainer.parent = housesContainer;

        //Place house
        GenerateRandomInRange(houseContainer, prefabLibrary.housePrefabs, housePlacementRange, housePlacementPos);

        //Place Fences
        Transform fenceLeft = (PrefabUtility.InstantiatePrefab(prefabLibrary.fencePrefab, houseContainer) as GameObject).transform;
        fenceLeft.localEulerAngles = new Vector3(0, -90, 0);
        fenceLeft.localPosition = Vector3.left * fencePlacementDist;

        Transform fenceRight = (PrefabUtility.InstantiatePrefab(prefabLibrary.fencePrefab, houseContainer) as GameObject).transform;
        fenceRight.localEulerAngles = new Vector3(0, 90, 0);
        fenceRight.localPosition = Vector3.right * fencePlacementDist;

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
}
