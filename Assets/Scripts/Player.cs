using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveableEntity
{
    private PlayerStates PlayerState = PlayerStates.Idle;
    private Animator PlayerAnimator;
    public enum PlayerStates
        {
        Moving,
        Idle
    }
    void Start(){
        PlayerAnimator=GetComponent<Animator>();
    }
    public enum Directions{
        left,
        right
    }
    private Directions DirectionFacing = Directions.right;

    public new void Init()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        base.MoveableEntityUpdate();
        UpdateLeftRight();
    }

    void UpdateLeftRight(){
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


    public override void IdleUpdate()
    {
        InputManager.Actions Action = InputManager.Instance.getInput();
        switch (Action)
        {
            case InputManager.Actions.up:
                MoveInputted(0, 1);
                break;
            case InputManager.Actions.down:
                MoveInputted(0, -1);
                break;
            case InputManager.Actions.left:
                DirectionFacing=Directions.left;
                MoveInputted(-1,0);
                break;
            case InputManager.Actions.right:
                DirectionFacing=Directions.right;
                MoveInputted(1,0);
                break;
        }
    }
    
    public void MoveInputted(int deltaX, int deltaY)
    {
        bool Success = TryScheduleMove(x+deltaX,y+deltaY);
        if(Success){
            PlayerAnimator.Play("PlayerWalk");
            }


        if(!Success)
        {
            Debug.Log("Move Failed");
        }
    }
    
}
