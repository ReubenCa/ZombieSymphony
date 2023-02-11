using System;
using System.Collections.Generic;
using Utils;
using UnityEngine;

public static class Navigator
{
    //static bool[,] TileAllowed;
    //static int BottomLeftX;
    //static int BottomLeftY;


    public static int AStarHeuristic(int startX, int startY, int DestX, int DestY)
    {
        return Math.Abs(startX - DestX) + Math.Abs(startY - DestY);
    }

    static public bool TilesareNeighbors((int, int) t1, (int, int) t2)
    {
        return Math.Abs(t1.Item1 - t2.Item1) + Math.Abs(t1.Item2 - t2.Item2) <= 1;
    }

    public static List<(int, int)> AStar(int startX, int startY, int DestX, int DestY)
    {
        if (startX == DestX && startY == DestY)
        {
            Debug.Log("Tried to navigate to tile its already on");
            return new List<(int, int)> { (startX, startY) };
        }
        PriorityQueue<AStarPointData, int> PQ = new PriorityQueue<AStarPointData, int>();
        HashSet<(int, int)> ClosedPoints = new HashSet<(int, int)>();
        AStarPointData start = new AStarPointData(startX, startY, null, 0);
        PQ.Enqueue(start, AStarHeuristic(startX, startY, DestX, DestY));
        AStarPointData? finaldat = null;
        while (PQ.Count > 0)
        {
            AStarPointData CurrentPoint = PQ.Dequeue();
            ClosedPoints.Add((CurrentPoint.x, CurrentPoint.y));
            List<(int, int)> Neighbors = GenerateNeighbors(CurrentPoint.x, CurrentPoint.y);
            foreach ((int, int) neighbor in Neighbors)
            {
                if (ClosedPoints.Contains(neighbor)) continue;

                AStarPointData newdat = new AStarPointData(neighbor.Item1, neighbor.Item2,
                    CurrentPoint, CurrentPoint.DistanceFromStart + 1);
                if (neighbor.Item1 == DestX && neighbor.Item2 == DestY)
                {
                    finaldat = new AStarPointData(neighbor.Item1, neighbor.Item2,
                        CurrentPoint, CurrentPoint.DistanceFromStart + 1);
                    break;
                }
                PQ.Enqueue(newdat, newdat.DistanceFromStart + AStarHeuristic(newdat.x, newdat.y, DestX, DestY));
            }
            if (finaldat != null)
            {
                break;
            }
        }
        if (finaldat == null)
        {
            // throw new System.Exception("No path found From (" + startX + "," + startY + ") -> (" + DestX + "," + DestY + ")");
            return null;
        }
        List<(int, int)> r = new List<(int, int)>();
        AStarPointData c = finaldat;
        while (c != null)
        {
            r.Add((c.x, c.y));
            c = c.previous;
        }
        for (int i = 0; i < r.Count - 1; i++)
        {
            Debug.Assert(TilesareNeighbors(r[i], r[i + 1]));
        }
        r.Reverse();
        return r;
    }

    public static List<(int, int)> GenerateNeighbors(int x, int y)
    {
        List<(int, int)> r = new List<(int, int)>();
        List<(int, int)> PointsToConsider = new List<(int, int)> { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) };
        foreach (var point in PointsToConsider)
        {
            if (TerrainManager.Instance.PositionValid(point.Item1, point.Item2,false))
            {
                r.Add(point);
            }
        }
        return r;
    }

    public class AStarPointData
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