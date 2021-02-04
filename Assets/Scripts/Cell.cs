using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [System.Serializable]
    public class Wall
    {
        public Direction direction;
        
        public Transform transform;

        [HideInInspector] public bool visited;
    }

    [HideInInspector] public IntVector2 coordinate;

    [HideInInspector] public bool visited;
    
    public List<Wall> walls; 
    
    public void DisableWall(Direction direction)
    {
        Wall wall = walls.Find(w => w.direction == direction);

        wall.transform.gameObject.SetActive(false);

        wall.visited = true;
    }
    
    public void DisableAllWalls()
    {
        walls.ForEach(w =>
        {
            DisableWall(w.direction);
        });
    }

    public Wall[] GetWalls(bool seen)
    {
        return walls.Where(w => w.visited == seen).ToArray();
    }
    
    public Wall GetWall(Direction direction)
    {
        return walls.Find(w => w.direction == direction);
    }
}