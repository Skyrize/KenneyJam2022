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
    public HouseGenerator houseGenerator;
    public Transform roadContainer;
    public Transform floorContainer;
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
            Transform generated = GenerateRandomInRange(roadContainer, prefabLibrary.survivorPrefabs, new Vector2(cellSize, cellSize), new Vector2(x * cellSize, y * cellSize));
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

    void AddBarrier(int x, int y, int angle)
    {

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

        if (x == 0)
        {
            for (int i = 0; i != blockSize - 1; i++)
            {
                AddBarrier(x + i, y + blockSize - 1, 0);
            }
        }
        if (y == 0)
        {
            
        }
        if (x == citySize.x - blockSize)
        {

        }
        if (y == citySize.y - blockSize)
        {
            
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
        for (int x = 0; x != citySize.x; x++)
        {
            for (int y = 0; y != citySize.y; y++)
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

    public override void Clean()
    {
        roadContainer.DestroyChilds();
        floorContainer.DestroyChilds();
        houseGenerator.Clean();
        houseGenerator.transform.position = transform.position;
    }


    public override void Generate()
    {
        if (!houseGenerator)
            throw new System.Exception("Missing house generator");
        Clean();
        citySize = new Vector2Int(citySize.x + blockSize * 2, citySize.y + blockSize * 2);
        Cell[,] grid = new Cell[citySize.x, citySize.y];
        FillBasic(grid);
    }

#endif
}
