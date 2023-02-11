using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using System.Net.NetworkInformation;

public class Zombie : MoveableEntity
{
    [SerializeField]
    private GameObject Mask;
    [SerializeField]
    float TimeBetweenMoves;

    private float TimesinceLastMove = 0;
    private Animator PlayerAnimator;
    private void Update()
    {
        base.MoveableEntityUpdate();
        base.UpdateLeftRight();
    }
    private void Start()
    {
        base.Init();
        PlayerAnimator=GetComponent<Animator>();
        Invoke("rise",0.01f);
    }
    private void rise(){
        State = MoveableEntityState.ZombieSpawning;
        OriginalY = gameObject.transform.position.y - DistancetoRise;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,
            OriginalY - DistancetoRise, gameObject.transform.position.z);
        Mask.gameObject.SetActive(true);
        //Find Grave
        foreach (Entity possibleGrave in TerrainManager.Instance.GetTerrainTile(x,y))
        {
            Debug.Log(possibleGrave);
            if(possibleGrave.gameObject.tag=="grave"){
                possibleGrave.gameObject.GetComponentInChildren<Animator>().Play("GraveExplosion");
                Debug.Log("PLayed");
            }
        }
    }

  

    public static int PlayerX;
    public static int PlayerY;

    [SerializeField]//Measured in how many moves it takes to update
    private int TargetUpdateFrequency;

    private Queue<(int, int)> MovementQueue = new Queue<(int, int)>();
    private int MovesUntilUpdate = 4;
    [SerializeField]
    private int TargetAccuracy = 2;
    public override void IdleUpdate()
    {
        if(TimesinceLastMove < TimeBetweenMoves)
        {
            TimesinceLastMove += Time.deltaTime;
            return;
        }
        TimesinceLastMove = 0;
        if (MovesUntilUpdate <= 0 || MovementQueue.Count ==0) {
            (int, int) NewTarget = PickTarget();
            Debug.Log("New Tartget: " + NewTarget);
            if (NewTarget.Item1 == x && NewTarget.Item2 == y)
            {
                return;
            }
            MovementQueue = new Queue<(int, int)>(Navigator.AStar(x, y, NewTarget.Item1, NewTarget.Item2));
            MovementQueue.Dequeue();
            MovesUntilUpdate = TargetUpdateFrequency;
        }
        (int,int) NextMove = MovementQueue.Dequeue();

        int dx = x;
        int dy = y;
        bool success = TryScheduleMove(NextMove.Item1, NextMove.Item2);
        // Debug.Log("Schedule Move from (" + dx + "," + dy + ") -> (" + NextMove.Item1 + "," + NextMove.Item2 + ")"
        //  + " Success:" + success);
        if (success)
        {
            PlayerAnimator.Play("PlayerWalk");
            if (NextMove.Item1 > dx)
            {
                base.faceRight();
            }
            else if (NextMove.Item1 < dx)
            {
                base.faceLeft();
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
    [SerializeField]
    float RiseFromGraveSpeed;
    [SerializeField]
    float DistancetoRise;

    private float distancerisen;
    private float OriginalY;
    public override void ZombieSpawningUpdate()
    {
        float deltay= RiseFromGraveSpeed * Time.deltaTime;
        float newy = gameObject.transform.position.y + deltay;
        Mask.gameObject.transform.position = new Vector3(Mask.gameObject.transform.position.x,
            Mask.gameObject.transform.position.y - deltay, Mask.gameObject.transform.position.z);
        if (newy > DistancetoRise + OriginalY)
        {
            newy = DistancetoRise + OriginalY;
            State = MoveableEntityState.Idle;
            Mask.gameObject.SetActive(false) ;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, newy, gameObject.transform.position.z);
        distancerisen += newy;
        
    }
}

public static class Navigator
{
    static bool[,] TileAllowed;
    static int BottomLeftX;
    static int BottomLeftY;

 
    private static int AStarHeuristic(int startX, int startY, int DestX, int DestY)
    {
        return Math.Abs(startX - DestX) + Math.Abs(startY - DestY);
    }

    static public bool TilesareNeighbors((int,int) t1, (int, int) t2)
    {
        return Math.Abs(t1.Item1 - t2.Item1) + Math.Abs(t1.Item2- t2.Item2) <=1;
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
        for (int i = 0; i< r.Count-1; i++)
        {
            Debug.Assert(TilesareNeighbors(r[i], r[i + 1]));
        }
        r.Reverse();
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