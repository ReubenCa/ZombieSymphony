using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveableEntity
{
    private PlayerStates PlayerState = PlayerStates.Idle;
    public enum PlayerStates
        {
        Moving,
        Idle
    }

    public new void Init()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        base.MoveableEntityUpdate();

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
                MoveInputted(-1,0);
                break;
            case InputManager.Actions.right:
                MoveInputted(1,0);
                break;
        }
    }
    
    public void MoveInputted(int deltaX, int deltaY)
    {
        bool Success = TryScheduleMove(x+deltaX,y+deltaY);
        if(!Success)
        {
            Debug.Log("Move Failed");
        }
    }
    
}
