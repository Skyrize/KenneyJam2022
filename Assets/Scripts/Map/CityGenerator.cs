using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class CityGenerator : Generator
{
    public Vector2Int citySize = new Vector2Int(20, 20);
    public HouseGenerator houseGenerator;
    public GameObject roadPrefab;
    public GameObject intersectRoadPrefab;
    public GameObject floorPrefab;
    public Transform roadContainer;
    public Transform floorContainer;

    void SeparateAndFill(Cell[,] grid)
    {

    }

    void AddHouse(Cell[,] grid, int x, int y)
    {
        houseGenerator.transform.position = transform.position + new Vector3(x+1, 0, y+1);
        grid[x, y].isOccupied = true;
        
        grid[x+1, y].isOccupied = true;
        
        grid[x, y+1].isOccupied = true;
        
        grid[x+1, y+1].isOccupied = true;

        houseGenerator.Generate();
    }

    void AddIntersection(Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(intersectRoadPrefab, roadContainer)as GameObject).transform;
        road.position = transform.position + new Vector3(x+0.5f, 0, y+0.5f);
        grid[x, y].isOccupied = true;
    }

    void AddRoad(int rotation, Cell[,] grid, int x, int y)
    {
        Transform road = (PrefabUtility.InstantiatePrefab(roadPrefab, roadContainer)as GameObject).transform;
        road.position = transform.position + new Vector3(x+0.5f, 0, y+0.5f);
        road.eulerAngles = new Vector3(0, rotation, 0);
        grid[x, y].isOccupied = true;
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

    void FillBasicBlock(Cell[,] grid, int x, int y)
    {
        Fill(grid, x, y, CellType.HOUSE);
        Fill(grid, x+2, y, CellType.HOUSE);
        Fill(grid, x, y+2, CellType.HOUSE);
        Fill(grid, x+2, y+2, CellType.HOUSE);
        
        Fill(grid, x+4, y, CellType.ROAD_VERTICAL);
        Fill(grid, x+4, y+1, CellType.ROAD_VERTICAL);
        Fill(grid, x+4, y+2, CellType.ROAD_VERTICAL);
        Fill(grid, x+4, y+3, CellType.ROAD_VERTICAL);

        Fill(grid, x, y+4, CellType.ROAD_HORIZONTAL);
        Fill(grid, x+1, y+4, CellType.ROAD_HORIZONTAL);
        Fill(grid, x+2, y+4, CellType.ROAD_HORIZONTAL);
        Fill(grid, x+3, y+4, CellType.ROAD_HORIZONTAL);

        Fill(grid, x+4, y+4, CellType.ROAD_INTERSECT);
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
                if (x % 10 == 0 && y % 10 == 0)
                {
                    Transform floor = (PrefabUtility.InstantiatePrefab(floorPrefab, floorContainer)as GameObject).transform;
                    floor.position = transform.position + new Vector3(x+5, 0, y+5);
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
        
        Cell[,] grid = new Cell[citySize.x, citySize.y];
        FillBasic(grid);
    }
}
