using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;


public class Zombie : MoveableEntity
{
    private Animator PlayerAnimator;
    private void Update()
    {
        base.MoveableEntityUpdate();
        base.UpdateLeftRight();
    }
    private void Start()
    {
        List<(int, int)> t = Navigator.AStar(-4, -4, 4, 4);
        foreach((int,int) p in t)
        {
            Debug.Log(p);
        }
        PlayerAnimator=GetComponent<Animator>();
    }

    [NonSerialized]
    public static int PlayerX;
    [NonSerialized]
    public static int PlayerY;

    [SerializeField]//Measured in how many moves it takes to update
    private int TargetUpdateFrequency;

    private Queue<(int, int)> MovementQueue = new Queue<(int, int)>();
    private int MovesUntilUpdate = 4;
    private int TargetAccuracy = 2;
    public override void IdleUpdate()
    {
        if (MovesUntilUpdate <= 0 || MovementQueue.Count ==0) {
            (int, int) NewTarget = PickTarget();
            MovementQueue = new Queue<(int, int)>(Navigator.AStar(x, y, NewTarget.Item1, NewTarget.Item2));
            MovesUntilUpdate = TargetUpdateFrequency;
        }
        (int,int) NextMove = MovementQueue.Dequeue();
        bool moveSuccessful = TryScheduleMove(NextMove.Item1, NextMove.Item2);
        //Update Facing Position - David
        //TODO
        Debug.Log("moved");
        if(moveSuccessful){
            PlayerAnimator.Play("PlayerWalk");
            if(NextMove.Item1>x){
                base.faceRight();
                Debug.Log("right");
            }
            else if(NextMove.Item1<x){
                base.faceLeft();
                Debug.Log("left");
            }
        }



        MovesUntilUpdate--;
    }
    static System.Random rand = new System.Random();
    private (int,int) PickTarget()
    {
        int targetx = -100;
        int targety = -100;
        while(!TerrainManager.Instance.PositionValid(targetx, targety))
        {
            targetx = rand.Next(PlayerX - TargetAccuracy, PlayerX + TargetAccuracy);
            targety = rand.Next(PlayerY - TargetAccuracy, PlayerY + TargetAccuracy);
        }
        return (targetx,targety);
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

  /*  public static void Init()
    {
        (int, int, int, int) dims = TerrainManager.Instance.GetDimensions();
        TileAllowed = new bool[dims.Item1, dims.Item2];
        for (int xiter = 0; xiter < dims.Item1; xiter++)
        {
            for (int yiter = 0; yiter < dims.Item2; yiter++)
            {
                (int, int) GCoords = ConvertToGameCoords(xiter, yiter);
                TileAllowed[xiter, yiter] = TerrainManager.Instance.TilePassable(GCoords.Item1, GCoords.Item2);
            }
        }

    }*/
    private static int AStarHeuristic(int startX, int startY, int DestX, int DestY)
    {
        return Math.Abs(startX - DestX) + Math.Abs(startY - DestY);
    }

    public static List<(int, int)> AStar(int startX, int startY, int DestX, int DestY)
    {
        if(startX == DestX&& startY == DestY)
        {
            Debug.Log("Tried to navigate to tile its already on");
            return new List<(int, int)> { (startX, startY) };
        }
        PriorityQueue<AStarPointData, int> PQ = new PriorityQueue<AStarPointData, int>();
        HashSet<(int,int)> ClosedPoints = new   HashSet<(int, int)>();
        AStarPointData start = new AStarPointData(startX, startY, null,0);
        PQ.Enqueue(start, AStarHeuristic(startX, startY, DestX, DestY));
        AStarPointData? finaldat = null;
        while(PQ.Count > 0)
        {
            AStarPointData CurrentPoint = PQ.Dequeue();
            ClosedPoints.Add((CurrentPoint.x, CurrentPoint.y));
            List<(int, int)> Neighbors = GenerateNeighbors(CurrentPoint.x, CurrentPoint.y);
            foreach((int,int) neighbor in Neighbors)
            {
                if(ClosedPoints.Contains(neighbor)) continue;
                
                AStarPointData newdat = new AStarPointData(neighbor.Item1, neighbor.Item2, 
                    CurrentPoint, CurrentPoint.DistanceFromStart + 1);
                if(neighbor.Item1 == DestX && neighbor.Item2 == DestY)
                {
                    finaldat = new AStarPointData(neighbor.Item1, neighbor.Item2,
                        CurrentPoint, CurrentPoint.DistanceFromStart + 1);
                    break;
                        }
                PQ.Enqueue(newdat, newdat.DistanceFromStart + AStarHeuristic(newdat.x, newdat.y, DestX, DestY));
            }
            if(finaldat != null)
            {
                break;
            }
        }
        if (finaldat == null)
        {
            throw new System.Exception("No path found From (" +  startX + "," + startY + ") -> (" + DestX + "," + DestY + ")");
        }
        List<(int, int)> r = new List<(int, int)>();
        AStarPointData c = finaldat;
        while (c != null)
        {
            r.Add((c.x, c.y));
            c = c.previous;
        }
        return r;
    }

    public static List<(int,int)> GenerateNeighbors(int x, int y)
    {
        List<(int,int)> r = new List<(int, int)>();
        List<(int, int)> PointsToConsider = new List<(int, int)> { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) };
        foreach(var point in PointsToConsider)
        {
            if (TerrainManager.Instance.PositionValid(point.Item1, point.Item2))
                {
                r.Add(point);
            }
        }
        return r;
    }

    public  class AStarPointData
        {
       public readonly int x;
      public readonly int y;
       public readonly AStarPointData previous;
        public readonly int DistanceFromStart;

        public AStarPointData(int x, int y, AStarPointData previous, int DistanceFromStart)
        {
            this.x = x;
            this.y = y;
            this.previous = previous;
            this.DistanceFromStart = DistanceFromStart;
        }
    }


}