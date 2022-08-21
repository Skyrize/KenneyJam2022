using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public enum CellType
{
    ROAD_HORIZONTAL,
    ROAD_VERTICAL,
    ROAD_INTERSECT,
    HOUSE
}

struct Cell
{
    public bool isOccupied;
}
#endif

public class CityGenerator : Generator
{
#if UNITY_EDITOR

    public float cellSize = 20;
    public Vector2Int citySize = new Vector2Int(20, 20);
    public Vector2Int finalCitySize;
    public HouseGenerator houseGenerator;
    Transform roadContainer;
    Transform floorContainer;
    Transform survivorContainer;
    Transform barrierContainer;
    public int minSurvivorAmount = 0;
    public int maxSurvivorAmount = 4;

    void SeparateAndFill(Cell[,] grid)
    {

    }

    void AddHouse(Cell[,] grid, int x, int y)
    {
        float houseSize = cellSize * 2;
        houseGenerator.transform.position = transform.position + new Vector3(x * cellSize + houseSize / 2f, 0, y * cellSize + houseSize / 2f);
        int randomOrientation = Random.Range(0, 3);
        houseGenerator.transform.rotation = Quaternion.Euler(0, randomOrientation * 90, 0);
        grid[x, y].isOccupied = true;
        
        grid[x+1, y].isOccupied = true;
        
        grid[x, y+1].isOccupied = true;
        
        grid[x+1, y+1].isOccupied = true;

        houseGenerator.Generate();
    }

    void AddIntersection(Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(prefabLibrary.intersectRoadPrefab, roadContainer)as GameObject).transform;
        float intersectionSize = cellSize;
        road.position = transform.position + new Vector3(x * cellSize + intersectionSize / 2f, 0, y * cellSize + intersectionSize / 2f);
        grid[x, y].isOccupied = true;
        RandomAddSurvivor(x, y);
    }

    void RandomAddSurvivor(int x, int y)
    {
        int randomSurvivorAmount = Random.Range(minSurvivorAmount, maxSurvivorAmount + 1);
        for (int i = 0; i != randomSurvivorAmount; i++)
        {
            Transform generated = GenerateRandomInRange(survivorContainer, prefabLibrary.survivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x * cellSize, y * cellSize));
            generated.eulerAngles = new Vector3(0, Random.Range(0, 359), 0);
        }
    }

    void AddRoad(int rotation, Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(prefabLibrary.roadPrefab, roadContainer)as GameObject).transform;
        float roadSize = cellSize;
        road.position = transform.position + new Vector3(x * cellSize + roadSize / 2f, 0, y * cellSize + roadSize / 2f);
        road.eulerAngles = new Vector3(0, rotation, 0);
        grid[x, y].isOccupied = true;
        RandomAddSurvivor(x, y);
    }

    void Fill(Cell[,] grid, int x, int y, CellType cellType)
    {
        switch (cellType)
        {
            case CellType.ROAD_HORIZONTAL:
                AddRoad(0, grid, x, y);
            break;
            case CellType.ROAD_VERTICAL:
                AddRoad(90, grid, x, y);
            break;
            case CellType.ROAD_INTERSECT:
                AddIntersection(grid, x, y);
            break;
            case CellType.HOUSE:
                AddHouse(grid, x, y);
            break;
            default:
            break;
        }
    }

    void AddBarrier(int x, int y, int rotation)
    {
        Transform barrier = (PrefabUtility.InstantiatePrefab(prefabLibrary.barrierPrefab, barrierContainer)as GameObject).transform;
        float roadSize = cellSize;
        barrier.position = transform.position + new Vector3(x * cellSize + roadSize / 2f, 0, y * cellSize + roadSize / 2f);
        if (Random.Range(0, 2) == 0)
        {
            rotation += 180;
        }
        if (Random.Range(0, 2) == 0)
        {
            barrier.localScale = new Vector3(barrier.localScale.x, barrier.localScale.y, -1);
        }
        if (Random.Range(0, 2) == 0)
        {
            barrier.localScale = new Vector3(-1, barrier.localScale.y, barrier.localScale.z);
        }
        barrier.eulerAngles = new Vector3(0, rotation, 0);
    }

    int blockSize = 5;
    void FillBasicBlock(Cell[,] grid, int x, int y)
    {
        Fill(grid, x, y, CellType.HOUSE);
        Fill(grid, x+2, y, CellType.HOUSE);
        Fill(grid, x, y+2, CellType.HOUSE);
        Fill(grid, x+2, y+2, CellType.HOUSE);
        
        for (int i = 0; i != blockSize - 1; i++)
        {
            Fill(grid, x + blockSize - 1, y + i, CellType.ROAD_VERTICAL);
        }

        for (int i = 0; i != blockSize - 1; i++)
        {
            Fill(grid, x + i, y + blockSize - 1, CellType.ROAD_HORIZONTAL);
        }

        Fill(grid, x + blockSize - 1, y + blockSize - 1, CellType.ROAD_INTERSECT);

        if (x == 0) //Barriers left side of map
        {
            for (int i = 0; i != blockSize; i++)
            {
                AddBarrier(x + blockSize - 1, y + i, 0);
            }
        }
        if (y == 0) //Barriers down side of map
        {
            for (int i = 0; i != blockSize; i++)
            {
                AddBarrier(x + i, y + blockSize - 1, 90);
            }
        }
        if (x == finalCitySize.x - blockSize) //Barriers right side of map
        {
            for (int i = 0; i != blockSize; i++)
            {
                AddBarrier(x - 1, y + i, 0);
            }
        }
        if (y == finalCitySize.y - blockSize) //Barriers up side of map
        {
            for (int i = 0; i != blockSize; i++)
            {
                AddBarrier(x + i, y - 1, 90);
            }
            
        }
    }

    void GenerateFloor(int x, int y)
    {
        Transform floor = (PrefabUtility.InstantiatePrefab(prefabLibrary.floorPrefab, floorContainer)as GameObject).transform;
        floor.position = transform.position + new Vector3(x*cellSize+cellSize*2.5f, 0, y*cellSize+cellSize*2.5f);
        MeshRenderer meshRenderer = floor.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.sharedMaterials;
        Material randomFloorMat = prefabLibrary.grassMaterials[Random.Range(0, prefabLibrary.grassMaterials.Length)];
        materials[0] = randomFloorMat;
        meshRenderer.sharedMaterials = materials;
    }

    void FillBasic(Cell[,] grid)
    {
        for (int x = 0; x != finalCitySize.x; x++)
        {
            for (int y = 0; y != finalCitySize.y; y++)
            {
                if (!grid[x, y].isOccupied)
                {
                    FillBasicBlock(grid, x, y);
                }
                if (x % 5 == 0 && y % 5 == 0)
                {
                    GenerateFloor(x, y);
                }
            }
        }
    }

    public void CleanTarget(string targetName)
    {
        Transform target = transform.Find(targetName);
        if (target)
        {
            DestroyImmediate(target.gameObject);
        }
    }

    public override void Clean()
    {
        houseGenerator.transform.position = transform.position;
        CleanTarget("floorContainer");
        CleanTarget("survivorContainer");
        CleanTarget("barrierContainer");
        CleanTarget("housesContainer");
        CleanTarget("roadContainer");
    }


    public override void Generate()
    {
        if (!houseGenerator)
            throw new System.Exception("Missing house generator");
        Clean();
        roadContainer = new GameObject("roadContainer").transform;
        roadContainer.transform.parent = transform;
        roadContainer.transform.localPosition = Vector3.zero;
        floorContainer = new GameObject("floorContainer").transform;
        floorContainer.transform.parent = transform;
        floorContainer.transform.localPosition = Vector3.zero;
        survivorContainer = new GameObject("survivorContainer").transform;
        survivorContainer.transform.parent = transform;
        survivorContainer.transform.localPosition = Vector3.zero;
        barrierContainer = new GameObject("barrierContainer").transform;
        barrierContainer.transform.parent = transform;
        barrierContainer.transform.localPosition = Vector3.zero;
        houseGenerator.housesContainer = new GameObject("housesContainer").transform;
        houseGenerator.housesContainer.transform.parent = transform;
        houseGenerator.housesContainer.transform.localPosition = Vector3.zero;
        
        finalCitySize = new Vector2Int(citySize.x + blockSize * 2, citySize.y + blockSize * 2);
        Cell[,] grid = new Cell[finalCitySize.x, finalCitySize.y];
        FillBasic(grid);
    }

#endif
}
