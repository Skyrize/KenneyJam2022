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
    HOUSE,
    CITY
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
    public int worldSize = 20;
    public int suburbanSize = 20;
    public int suburbanDangerousSize = 20;
    public int citySize = 20;
    public Vector2Int finalCitySize;
    public HouseGenerator houseGenerator;
    public Transform worldContainer;
    Transform cityContainer;
    Transform roadContainer;
    Transform floorContainer;
    Transform survivorContainer;
    Transform barrierContainer;
    public int minSurvivorAmount = 0;
    public int maxSurvivorAmount = 4;
    public int level1Prob = 6;
    public int level2Prob = 3;

    void GenerateHouse(Cell[,] grid, int x, int y)
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
    
    void GenerateCity(Cell[,] grid, int x, int y)
    {
        //TODO : city placement
        float citySize = cellSize * 2;
        int randomPrefabIndex = Random.Range(0, prefabLibrary.cityPrefab.Length);
        Transform city = (PrefabUtility.InstantiatePrefab(prefabLibrary.cityPrefab[randomPrefabIndex], cityContainer)as GameObject).transform;
        city.transform.position = transform.position + new Vector3(x * cellSize + citySize / 2f, 0, y * cellSize + citySize / 2f);
        int randomOrientation = Random.Range(0, 3);
        city.transform.rotation = Quaternion.Euler(0, randomOrientation * 90, 0);
        grid[x, y].isOccupied = true;
        
        grid[x+1, y].isOccupied = true;
        
        grid[x, y+1].isOccupied = true;
        
        grid[x+1, y+1].isOccupied = true;
    }

    void GenerateIntersection(Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(prefabLibrary.intersectRoadPrefab, roadContainer)as GameObject).transform;
        float intersectionSize = cellSize;
        road.position = transform.position + new Vector3(x * cellSize + intersectionSize / 2f, 0, y * cellSize + intersectionSize / 2f);
        grid[x, y].isOccupied = true;
        RandomGenerateSurvivor(road.position.x, road.position.z);
    }

    int currentLevel = 0;

    void RandomGenerateSurvivor(float x, float y)
    {
        int randomSurvivorAmount = Random.Range(minSurvivorAmount, maxSurvivorAmount + 1);
        for (int i = 0; i != randomSurvivorAmount; i++)
        {
            Transform generated;
            if (currentLevel == 0)
            {
                generated = GenerateRandomInRange(survivorContainer, prefabLibrary.survivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x, y));
            }
            else if (currentLevel == 1)
            {
                if (Random.Range(0, level1Prob + 1) == 0)
                {
                    generated = GenerateRandomInRange(survivorContainer, prefabLibrary.armedSurvivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x, y));
                }
                else
                {
                    generated = GenerateRandomInRange(survivorContainer, prefabLibrary.survivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x, y));
                }
            }
            else
            {
                if (Random.Range(0, level2Prob + 1) == 0)
                {
                    generated = GenerateRandomInRange(survivorContainer, prefabLibrary.armedSurvivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x, y));
                }
                else
                {
                    generated = GenerateRandomInRange(survivorContainer, prefabLibrary.survivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x, y));
                }
            }
            generated.eulerAngles = new Vector3(0, Random.Range(0, 359), 0);
        }
    }

    void GenerateRoad(int rotation, Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(prefabLibrary.roadPrefab, roadContainer)as GameObject).transform;
        float roadSize = cellSize;
        road.position = transform.position + new Vector3(x * cellSize + roadSize / 2f, 0, y * cellSize + roadSize / 2f);
        road.eulerAngles = new Vector3(0, rotation, 0);
        grid[x, y].isOccupied = true;
        RandomGenerateSurvivor(road.position.x, road.position.z);
    }

    void Generate(Cell[,] grid, int x, int y, CellType cellType)
    {
        switch (cellType)
        {
            case CellType.ROAD_HORIZONTAL:
                GenerateRoad(0, grid, x, y);
            break;
            case CellType.ROAD_VERTICAL:
                GenerateRoad(90, grid, x, y);
            break;
            case CellType.ROAD_INTERSECT:
                GenerateIntersection(grid, x, y);
            break;
            case CellType.HOUSE:
                GenerateHouse(grid, x, y);
            break;
            case CellType.CITY:
                GenerateCity(grid, x, y);
            break;
            default:
            break;
        }
    }

    void GenerateBarrier(int x, int y, int rotation)
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
    void GenerateSuburbanBlock(Cell[,] grid, int x, int y)
    {
        Generate(grid, x, y, CellType.HOUSE);
        Generate(grid, x+2, y, CellType.HOUSE);
        Generate(grid, x, y+2, CellType.HOUSE);
        Generate(grid, x+2, y+2, CellType.HOUSE);
        
        for (int i = 0; i != blockSize - 1; i++)
        {
            Generate(grid, x + blockSize - 1, y + i, CellType.ROAD_VERTICAL);
        }

        for (int i = 0; i != blockSize - 1; i++)
        {
            Generate(grid, x + i, y + blockSize - 1, CellType.ROAD_HORIZONTAL);
        }

        Generate(grid, x + blockSize - 1, y + blockSize - 1, CellType.ROAD_INTERSECT);
    }
    
    void GenerateCityBlock(Cell[,] grid, int x, int y)
    {
        Generate(grid, x, y, CellType.CITY);
        Generate(grid, x+2, y, CellType.CITY);
        Generate(grid, x, y+2, CellType.CITY);
        Generate(grid, x+2, y+2, CellType.CITY);
        
        for (int i = 0; i != blockSize - 1; i++)
        {
            Generate(grid, x + blockSize - 1, y + i, CellType.ROAD_VERTICAL);
        }

        for (int i = 0; i != blockSize - 1; i++)
        {
            Generate(grid, x + i, y + blockSize - 1, CellType.ROAD_HORIZONTAL);
        }

        Generate(grid, x + blockSize - 1, y + blockSize - 1, CellType.ROAD_INTERSECT);
    }

    void GenerateBarriers()
    {
        for (int i = 0; i != finalCitySize.y; i++)//Barriers left and right side of map
        {
            GenerateBarrier(blockSize - 1, i, 0);
            GenerateBarrier(finalCitySize.x - blockSize - 1, i, 0);
        }
        for (int i = 0; i != finalCitySize.x; i++) //Barriers up & down side of map
        {
            GenerateBarrier(i, blockSize - 1, 90);
            GenerateBarrier(i, finalCitySize.y - blockSize - 1, 90);
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
        if (worldContainer)
            worldContainer.DestroyChilds();
    }

    void GenerateMapCollider(int x, int y, float rotation)
    {
        Transform collider = (PrefabUtility.InstantiatePrefab(prefabLibrary.colliderPrefab, barrierContainer)as GameObject).transform;
        float roadSize = cellSize;
        collider.position = transform.position + new Vector3(x * cellSize + roadSize / 2f, 0, y * cellSize + roadSize / 2f);
        collider.eulerAngles = new Vector3(0, rotation, 0);
        BoxCollider box = collider.GetComponent<BoxCollider>();
        box.size = new Vector3(finalCitySize.x * cellSize * 2, box.size.y, box.size.z);
    }

    void GenerateMapColliders()
    {
        GenerateMapCollider(finalCitySize.x / 2, blockSize, 0); // bottom 
        GenerateMapCollider(finalCitySize.x / 2, finalCitySize.y - blockSize, 0); // top
        
        GenerateMapCollider(blockSize, finalCitySize.y / 2, 90); // left 
        GenerateMapCollider(finalCitySize.y - blockSize - 1, finalCitySize.y / 2, 90); // right
    }

    public override void Generate()
    {
        if (!houseGenerator)
            throw new System.Exception("Missing house generator");
        Clean();
        if (!worldContainer)
        {
            worldContainer = new GameObject("worldContainer").transform;
            worldContainer.transform.position = transform.position;
        }
        roadContainer = new GameObject("roadContainer").transform;
        roadContainer.transform.parent = worldContainer;
        roadContainer.transform.localPosition = Vector3.zero;
        cityContainer = new GameObject("cityContainer").transform;
        cityContainer.transform.parent = worldContainer;
        cityContainer.transform.localPosition = Vector3.zero;
        floorContainer = new GameObject("floorContainer").transform;
        floorContainer.transform.parent = worldContainer;
        floorContainer.transform.localPosition = Vector3.zero;
        survivorContainer = new GameObject("survivorContainer").transform;
        survivorContainer.transform.parent = worldContainer;
        survivorContainer.transform.localPosition = Vector3.zero;
        barrierContainer = new GameObject("barrierContainer").transform;
        barrierContainer.transform.parent = worldContainer;
        barrierContainer.transform.localPosition = Vector3.zero;
        houseGenerator.housesContainer = new GameObject("housesContainer").transform;
        houseGenerator.housesContainer.transform.parent = worldContainer;
        houseGenerator.housesContainer.transform.localPosition = Vector3.zero;
        
        finalCitySize = new Vector2Int(worldSize + blockSize * 2, citySize + suburbanSize + suburbanDangerousSize + blockSize * 2);
        Cell[,] grid = new Cell[finalCitySize.x, finalCitySize.y];

        GenerateMapColliders();
        GenerateBarriers();
        for (int x = 0; x != finalCitySize.x; x++)
        {
            for (int y = 0; y != finalCitySize.y; y++)
            {
                if (!grid[x, y].isOccupied)
                {
                    if (y < suburbanSize)
                    {
                        currentLevel = 0;
                        GenerateSuburbanBlock(grid, x, y);
                    }
                    else if (y <= suburbanSize + suburbanDangerousSize)
                    {
                        currentLevel = 1;
                        GenerateSuburbanBlock(grid, x, y);
                    }
                    else
                    {
                        currentLevel = 2;
                        GenerateCityBlock(grid, x, y);
                    }
                }
                if (x % 5 == 0 && y % 5 == 0)
                {
                    GenerateFloor(x, y);
                }
            }
        }
    }

#endif
}
