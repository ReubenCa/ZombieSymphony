using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class MoveableEntity : Entity
{
    public int x { protected set; get; }
    public int y { protected set; get; }

    float CurrentMovementX;
    float CurrentMovementY;
    int MoveStartX;
    int MoveStartY;
    bool MoveIsAVerticalMove;
    int DestinationX;
    int DestinationY;
    public float TimeToMove;
    private MoveableEntityState State = MoveableEntityState.Idle;

    public override bool getPassable()
    {
        return true;
    }

    protected new void Init()
    {
       
        base.Init();
        TerrainManager.Instance.SetTerrainTile(x, y, this);

    }

    public enum MoveableEntityState
    {
        Moving,
        Idle
    }

    // Update is called once per frame
    public void MoveableEntityUpdate()
    {
     //   Debug.Log(State);
        switch (State)
        {
            case MoveableEntityState.Idle:
                IdleUpdate(); break;
            case MoveableEntityState.Moving:
                MovingUpdate(); break;
        }
    }

    abstract public void IdleUpdate();
    
    public void MovingUpdate()
    {
        float nextx =CurrentMovementX+ ((float)(DestinationX - MoveStartX)) * (Time.deltaTime/TimeToMove);
        float nexty =CurrentMovementY+ ((float)(DestinationY - MoveStartY)) * (Time.deltaTime/TimeToMove);

        if(!MoveIsAVerticalMove  && (nextx < DestinationX) == (nextx < MoveStartX))
        {
            nextx = DestinationX;
            State = MoveableEntityState.Idle;
        }

        if (MoveIsAVerticalMove && (nexty < DestinationY) == (nexty < MoveStartY))
        {
            nexty = DestinationY;
            State = MoveableEntityState.Idle;
        }


        Vector3 newpos = new Vector3(nextx,nexty,0);
        gameObject.transform.position = newpos;
        CurrentMovementX = nextx;
        CurrentMovementY = nexty;
    }


    public bool TryScheduleMove(int DestX, int DestY)
    {
        Debug.Assert(State != MoveableEntityState.Moving);
        if((DestX == x) == (DestY ==y))
        {
            throw new System.Exception("Either diagnol or zero move attempted to be schedled.");
        }
        CurrentMovementX = x;
        CurrentMovementY = y;

        if(!TerrainManager.Instance.PositionValid(DestX, DestY)) return false;
        MoveIsAVerticalMove = DestX == x;
        TerrainManager.Instance.EntityMoving(this, DestX, DestY);
        DestinationX= DestX;
        DestinationY = DestY;
        Debug.Log("Move Scheduled");
        State = MoveableEntityState.Moving;
        MoveStartX = x;
        MoveStartY = y;
        x = DestX;
        y = DestY;


        Zombie.PlayerX = x;
        Zombie.PlayerY = y;

        return true;
    }

    
    
}
