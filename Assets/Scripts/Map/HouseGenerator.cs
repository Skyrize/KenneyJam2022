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
    public GameObject fencePrefab;
    public float fencePlacementDist = 1.5f;
    public GameObject[] housePrefabs;
    public GameObject[] decorationPrefabs;

    public Transform housesContainer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Clean()
    {
        housesContainer.DestroyChilds();
    }

    public void GenerateRandomInRange(Transform container, GameObject[] prefabs, Vector2 placementRange, Vector2 basePos)
    {
        int randomPrefabIndex = Random.Range(0, prefabs.Length);
        float randomPosX = Random.Range(-placementRange.x / 2f, placementRange.x / 2f);
        float randomPosZ = Random.Range(-placementRange.y / 2f, placementRange.y / 2f);
        Vector3 randomPos = new Vector3(basePos.x + randomPosX, 0, basePos.y + randomPosZ);
        Transform generated = (PrefabUtility.InstantiatePrefab(prefabs[randomPrefabIndex], container) as GameObject).transform;
        generated.localPosition = randomPos;
    }

    public override void Generate()
    {
        Transform houseContainer = new GameObject("House").transform;
        houseContainer.position = transform.position;
        houseContainer.rotation = transform.rotation;
        houseContainer.parent = housesContainer;

        //Place house
        GenerateRandomInRange(houseContainer, housePrefabs, housePlacementRange, housePlacementPos);

        //Place Fences
        Transform fenceLeft = (PrefabUtility.InstantiatePrefab(fencePrefab, houseContainer) as GameObject).transform;
        fenceLeft.localEulerAngles = new Vector3(0, -90, 0);
        fenceLeft.localPosition = Vector3.left * fencePlacementDist;

        Transform fenceRight = (PrefabUtility.InstantiatePrefab(fencePrefab, houseContainer) as GameObject).transform;
        fenceRight.localEulerAngles = new Vector3(0, 90, 0);
        fenceRight.localPosition = Vector3.right * fencePlacementDist;

        //Place Decors
        int randomDecorAmount = Random.Range(minDecorAmount, maxDecorAmount + 1);
        for (int i = 0; i != randomDecorAmount; i++)
        {
            GenerateRandomInRange(houseContainer, decorationPrefabs, decorPlacementRange, decorPlacementPos);
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
