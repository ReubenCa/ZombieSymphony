using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.Assertions;

public abstract class MoveableEntity : Entity
{
    public int x { protected set; get; }
    public int y { protected set; get; }

    public enum Directions{
        left,
        right
    }
    private Directions DirectionFacing = Directions.right;

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
        x = (int)Math.Floor(gameObject.transform.position.x);
        y = (int)Math.Floor(gameObject.transform.position.y);
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
            return false;
           // throw new System.Exception("Either diagnol or zero move attempted to be schedled.");
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

    public void UpdateLeftRight(){
        float xScale=transform.localScale.x;
        if (xScale>0&&DirectionFacing==Directions.left){
            //Should be facing left 
            xScale*=-1;
        }
        else if(xScale<0&&DirectionFacing==Directions.right){
            //Should be facing right 
            xScale*=-1;
        }
        transform.localScale=new Vector3(xScale,transform.localScale.y,transform.localScale.z);
    }

    public void faceLeft(){
        DirectionFacing=Directions.left;
    }
    public void faceRight(){
        DirectionFacing=Directions.right;
    }
    
}
