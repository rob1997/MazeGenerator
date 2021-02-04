using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Direction
{
    North = 0,
    South = 1,
    East = 2,
    West = 3
}

public static class Ext
{
    public static IntVector2 GetUnitVector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new IntVector2(0, 1);
            case Direction.South:
                return new IntVector2(0, -1);
            case Direction.East:
                return new IntVector2(1, 0);
            case Direction.West:
                return new IntVector2(- 1, 0);
            default:
                return default;
        }
    }
    
    public static Direction GetOpposite(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.South;
            case Direction.South:
                return Direction.North;
            case Direction.East:
                return Direction.West;
            case Direction.West:
                return Direction.East;
            default:
                return default;
        }
    }
    
    public static bool Contains(this IntVector2 container, IntVector2 limit, IntVector2 vector2)
    {
        bool containsX = container.x.Contains(limit.x, vector2.x);
        bool containsZ = container.z.Contains(limit.z, vector2.z);
        
        return containsX && containsZ;
    }
    
    public static bool Contains(this int container, int limit, int i)
    {
        bool contains;

        if (limit > container) contains = i <= limit && i > container;

        else if (limit < container) contains = i >= limit && i < container;

        else contains = false;
        
        return contains;
    }
}

[System.Serializable]
public struct IntVector2
{
    public int x;
    public int z;

    public static IntVector2 Zero = new IntVector2(0, 0);
    
    public IntVector2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    
    public static IntVector2 operator + (IntVector2 a, IntVector2 b) {
        a.x += b.x;
        a.z += b.z;
        return a;
    }
    
    public static IntVector2 operator - (IntVector2 a, IntVector2 b) {
        a.x -= b.x;
        a.z -= b.z;
        return a;
    }
}

public class MazeGenerator : MonoBehaviour
{
    public GameObject cellPrefab;

    public float cellUnitSize;

    public IntVector2 size;

    private Cell[,] _cells;
    
    private Cell _foreCell;

    public void Initialize()
    {
        _cells = new Cell[size.x,size.z];
        
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                CreateCell(new IntVector2(i, j));
            }
        }
        
        SetForeCell(_cells[Random.Range(0, size.x), Random.Range(0, size.z)]);
    }
    
    public void CreateCell(IntVector2 coordinate)
    {
        GameObject cellObj = Instantiate(cellPrefab, transform);

        cellObj.transform.localPosition = new Vector3(cellUnitSize * coordinate.x, 0, cellUnitSize * coordinate.z);
        
        Cell cell = cellObj.GetComponent<Cell>();

        _cells[coordinate.x, coordinate.z] = cell;
        
        cell.coordinate = coordinate;
    }
    
    public void Generate()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                UpdateVisitation(new IntVector2(i, j));
            }
        }
        
        List<Cell.Wall> walls = _foreCell.GetWalls(false).ToList();

        if (walls.Count == 0)
        {
            bool newForeCellFound = false;
            
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.z; j++)
                {
                    Cell cell = _cells[i, j];
                    
                    if (!cell.visited 
                        && 
                        cell.GetWalls(true)
                            .Count(w => size.Contains(IntVector2.Zero, cell.coordinate + w.direction.GetUnitVector())) != 0)
                    {
                        SetForeCell(cell);

                        newForeCellFound = true;
                        
                        break;
                    }
                }

                if (newForeCellFound)
                {
                    break;
                }
            }

            if (!newForeCellFound) return;

            _foreCell.visited = true;

            Cell.Wall[] visitedWalls = _foreCell.GetWalls(true).Where(w => 
                size.Contains(IntVector2.Zero, _foreCell.coordinate + w.direction.GetUnitVector())).ToArray();

            Cell.Wall visitedWall = visitedWalls[Random.Range(0, visitedWalls.Length)];
                
            IntVector2 visitedCellCoordinate = _foreCell.coordinate + visitedWall.direction.GetUnitVector();

            if (size.Contains(IntVector2.Zero, visitedCellCoordinate))
            {
                _foreCell.DisableWall(visitedWall.direction);
                
                _cells[visitedCellCoordinate.x, visitedCellCoordinate.z].DisableWall(visitedWall.direction.GetOpposite());
            }
                
            Generate();
            
            return;
        }
        
        Cell.Wall wall = walls[Random.Range(0, walls.Count)];

        Direction direction = wall.direction;
        
        IntVector2 coordinate = _foreCell.coordinate + direction.GetUnitVector();

        if (!size.Contains(IntVector2.Zero, coordinate))
        {
            wall.visited = true;
            
            Generate();
            
            return;
        }

        Cell adjacentCell = _cells[coordinate.x, coordinate.z];

        _foreCell.DisableWall(direction);
            
        SetForeCell(adjacentCell);

        _foreCell.DisableWall(direction.GetOpposite());
        
        Generate();
    }

    private void UpdateVisitation(IntVector2 coordinate)
    {
        if (!size.Contains(IntVector2.Zero, coordinate))
        {
            return;
        }

        Cell cell = _cells[coordinate.x, coordinate.z];
        
        cell.walls.ForEach(w =>
        {
            IntVector2 adjacentCellCoordinate = cell.coordinate + w.direction.GetUnitVector();

            w.visited = true;
            
            if (size.Contains(IntVector2.Zero, adjacentCellCoordinate))
            {
                Cell adjacentCell = _cells[adjacentCellCoordinate.x, adjacentCellCoordinate.z];
                
                w.visited = adjacentCell.visited;
            }
        });
    }
    
    private void SetForeCell(Cell cell)
    {
        UpdateVisitation(cell.coordinate);
        
        cell.walls.ForEach(w =>
        {
            UpdateVisitation(cell.coordinate + w.direction.GetUnitVector());
        });
        
        _foreCell = cell;

        _foreCell.visited = true;
    }
}