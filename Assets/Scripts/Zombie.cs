using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;

public class Zombie : MoveableEntity
{
    private void Start()
    {
        
    }
    public enum 
    public override void IdleUpdate()
    {
        
    }

    

}

public static class Navigator
{
    static bool[,] TileAllowed;
    static int BottomLeftX;
    static int BottomLeftY;

    static (int, int) ConvertToGameCoords(int x, int y)
    {
        return (x + BottomLeftX, y + BottomLeftY);
    }

    static (int, int) ConvertToArrayCoords(int x, int y)
    {
        return (x - BottomLeftX, y - BottomLeftY);
    }

    public static void Init()
    {
        (int, int,int,int) dims = TerrainManager.Instance.GetDimensions();
        TileAllowed = new bool[dims.Item1, dims.Item2];
        for (int xiter = 0; xiter < dims.Item1; xiter++)
        {
            for(int yiter = 0; yiter< dims.Item2; yiter++)
            {
                (int, int) GCoords = ConvertToGameCoords(xiter, yiter);
                TileAllowed[xiter, yiter] = TerrainManager.Instance.TilePassable(GCoords.Item1, GCoords.Item2);
            }
        }

    }
    private static int AStarHeuristic(int startX, int startY, int DestX, int DesY)
    {
        return Math.Abs(startX - DestX) + Math.Abs(startY - DestY);
    }

    public static List<(int,int)> AStar(int startX, int startY, int DestX, int DesY)
    {
        PriorityQueue<(int, int), int> PQ = new PriorityQueue<(int, int), int>();
    }


}