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
    Sprite AwakeHeadSprite;

    

    [SerializeField]
    private GameObject Mask;
    [SerializeField]
    float TimeBetweenMoves;

    [SerializeField]
    float MinTimeToSleep;
    [SerializeField]
    float MaxTimeToSleep;
    [SerializeField]
    float MinTimeSleeping;
    [SerializeField]
    float MaxTimeSleeping;
    [SerializeField]
    int NoWakeUpDistance;
    public void SetStats( float MinTimeToSleep, float MaxTimeToSleep, float MaxTimeSleeping, float MinTimeSleeping
        , int TargetAccuracy, int TargetUpdateFreq, float TimeToMove = -10)
    {
        if(TimeToMove >0)
        {
            this.TimeToMove = TimeToMove;
        }
        this.MinTimeToSleep= MinTimeToSleep;
        this.MaxTimeToSleep= MaxTimeToSleep;
        this.MinTimeSleeping = MinTimeSleeping;
        this.MaxTimeSleeping = MaxTimeSleeping;
        this.TargetAccuracy = TargetAccuracy;
        this.TargetUpdateFrequency = TargetUpdateFreq;
    }

    float TimeSinceLastSleep = 0f;
    float TimeAsleep = 0f;

    private float TimesinceLastMove = 0;
    private Animator PlayerAnimator;
    [SerializeField]
    Sprite SleepyHead;
    private void Update()
    {
        if (MoveableEntityState.Sleeping != State && State != MoveableEntityState.Dying)
            HeadspriteRenderer.sprite = AwakeHeadSprite;
        else if (MoveableEntityState.Sleeping == State)
            HeadspriteRenderer.sprite = SleepyHead;

            if (State != MoveableEntityState.ZombieSpawning && State != MoveableEntityState.Sleeping)
            TimeSinceLastSleep += Time.deltaTime;
        if (TimeSinceLastSleep > NextSleepTime && State == MoveableEntityState.Idle)
        {
            State = MoveableEntityState.Sleeping;
            //Debug.Log("Calling Sleep Animation");
            PlayerAnimator.Play("ZombieSleeping");
            NextWakeUpTime = UnityEngine.Random.Range(MinTimeSleeping, MaxTimeSleeping);
        }

        base.MoveableEntityUpdate();
        base.UpdateLeftRight();
    }
    public void goSleepForever()
    {
        State = MoveableEntityState.Sleeping;
        //Debug.Log("Calling Sleep Animation");
        PlayerAnimator.Play("ZombieSleeping");
        NextWakeUpTime = float.MaxValue;
    }
    float NextWakeUpTime;
    public override void SleepUpdate()
    {
        TimeAsleep += Time.deltaTime;
        if (TimeAsleep > NextWakeUpTime && Navigator.AStarHeuristic(x, y, PlayerX, PlayerY) > NoWakeUpDistance)
        {
            PlayerAnimator.Play("PlayerIdle");
            State = MoveableEntityState.Idle;
            NextSleepTime = UnityEngine.Random.Range(MinTimeToSleep, MaxTimeToSleep);
            TimeSinceLastSleep = 0f;
            TimeAsleep = 0f;
        }

    }
    float NextSleepTime;
    [SerializeField]
   SpriteRenderer HeadspriteRenderer;
    private void Start()
    {
        base.Init();
        
        PlayerAnimator = GetComponent<Animator>();
        Invoke("rise", 0.01f);
        NextSleepTime = UnityEngine.Random.Range(MinTimeToSleep, MaxTimeToSleep);
    }
    private void rise()
    {
        State = MoveableEntityState.ZombieSpawning;
        OriginalY = gameObject.transform.position.y - DistancetoRise;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,
            OriginalY - DistancetoRise, gameObject.transform.position.z);
        Mask.gameObject.SetActive(true);
        foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
        {
            sp.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        int orderInLayer = GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
        Mask.GetComponent<SpriteMask>().frontSortingOrder = orderInLayer + 4;
        Mask.GetComponent<SpriteMask>().backSortingOrder = orderInLayer - 4;
        //Find Grave
        foreach (Entity possibleGrave in TerrainManager.Instance.GetTerrainTile(x, y))
        {
            Debug.Log(possibleGrave);
            try
            {
                if (possibleGrave.gameObject.tag == "grave")
                {
                    possibleGrave.gameObject.GetComponentInChildren<Animator>().Play("GraveExplosion");
                    Debug.Log("PLayed");
                }

            }
            catch
            {
                //Sometimes this loop would access dead zombies and crash 
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
        if (TimesinceLastMove < TimeBetweenMoves)
        {
            TimesinceLastMove += Time.deltaTime;
            return;
        }
        TimesinceLastMove = 0;
        if (MovesUntilUpdate <= 0 || MovementQueue == null || MovementQueue.Count == 0)
        {
            (int, int) NewTarget = PickTarget();
            // Debug.Log("New Tartget: " + NewTarget);
            if (NewTarget.Item1 == x && NewTarget.Item2 == y)
            {
                return;
            }
            List<(int, int)> AStarResult = Navigator.AStar(x, y, NewTarget.Item1, NewTarget.Item2);
            if (AStarResult == null)
                return;
            MovementQueue = new Queue<(int, int)>(AStarResult);

            MovementQueue.Dequeue();
            MovesUntilUpdate = TargetUpdateFrequency;
        }
        (int, int) NextMove = MovementQueue.Dequeue();

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

    private (int, int) PickTarget()
    {
        int targetx = -100;
        int targety = -100;
        while (!TerrainManager.Instance.PositionValid(targetx, targety))
        {
            targetx = rand.Next(PlayerX - TargetAccuracy, PlayerX + TargetAccuracy);
            targety = rand.Next(PlayerY - TargetAccuracy, PlayerY + TargetAccuracy);
        }
        return (targetx, targety);
    }
    [SerializeField]
    float RiseFromGraveSpeed;
    [SerializeField]
    float DistancetoRise;

    private float distancerisen;
    private float OriginalY;
    public override void ZombieSpawningUpdate()
    {
        float deltay = RiseFromGraveSpeed * Time.deltaTime;
        float newy = gameObject.transform.position.y + deltay;
        Mask.gameObject.transform.position = new Vector3(Mask.gameObject.transform.position.x,
            Mask.gameObject.transform.position.y - deltay, Mask.gameObject.transform.position.z);
        if (newy > DistancetoRise + OriginalY)
        {
            newy = DistancetoRise + OriginalY;
            State = MoveableEntityState.Idle;
            foreach (SpriteRenderer sp in GetComponentsInChildren<SpriteRenderer>())
            {
                sp.maskInteraction = SpriteMaskInteraction.None;
            }
            Mask.gameObject.SetActive(false);
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, newy, gameObject.transform.position.z);
        distancerisen += newy;

    }


    public override void onContact()
    {
        if (MoveableEntityState.Dying == State)
            return;
        if (State != MoveableEntityState.Sleeping && State!= MoveableEntityState.Dying)
        {
            GameManager.instance.PlayerBitten();
            return;
        }
        

        if (!Player.Instance.isBiting)
        {
            // Debug.Log("player not biting");
            return;
        }
        PlayerAnimator.Play("ZombieBitten");
        GameManager.instance.ZombieDied();
        //TerrainManager.Instance.ClearFromTile(x, y, this);
        State = MoveableEntityState.Dying;
    }

    public override bool getPassable(bool CanPassthroughZombies)
    {
        return CanPassthroughZombies;
    }
    float TimeSpendDying = 0f;
    public override void DyingUpdate()
    {
        TimeSpendDying+= Time.deltaTime;
        if(TimeSpendDying< 1f)
        {
            return;
        }
        TerrainManager.Instance.ClearFromTile(x, y, this);
        Destroy(gameObject);
    }
}

