using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveableEntity
{
    public static Player Instance;
    public void Awake()
    {
        if(Instance != null) { throw new System.Exception("Multiple Players"); }
        Instance = this;
    }

    public bool isBiting;

    private Animator PlayerAnimator;

    void Start(){
        PlayerAnimator=GetComponent<Animator>();
        Init();
    }
    

    public new void Init()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        List<Entity> entities = TerrainManager.Instance.GetTerrainTile(x, y);
        foreach (Entity e in new List<Entity>(entities))
        {

            e.onContact();
        }
        if (Input.GetKey(KeyCode.Space)){
            if(!isBiting){setBiting(true);}
        }
        else{
            if(isBiting){setBiting(false);}
        }
        base.MoveableEntityUpdate();
        base.UpdateLeftRight();
    }

    void setBiting(bool newVal){
        PlayerAnimator.SetBool("biting",newVal);
        isBiting=newVal;
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
                base.faceLeft();
                MoveInputted(-1,0);
                break;
            case InputManager.Actions.right:
                base.faceRight();
                MoveInputted(1,0);
                break;
        }
    }
    
    public void MoveInputted(int deltaX, int deltaY)
    {
       if(! MusicManager.instance.checkIfBeat())
        {
            GameManager.instance.MoveNotOnBeat();
        }
        bool Success = TryScheduleMove(x+deltaX,y+deltaY);
        if (Success) {
            PlayerAnimator.Play("PlayerWalk");
            Zombie.PlayerX = x;
            Zombie.PlayerY = y;

        }

       
/*if (!Success)
        {
            Debug.Log("Move Failed");
        }*/
    }

    public override void ZombieSpawningUpdate()
    {
        //Terrible Software Design
        //*Shudders*
        throw new System.Exception("Shouldnt Call Zombie Spawning On Player");
    }

    public override void SleepUpdate()
    {
        //Dont do this at home kids
        throw new System.Exception("Shouldnt Call Sleep On Player");
    }
}
