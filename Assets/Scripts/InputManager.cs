using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { private set; get; }
    private void Awake()
    {
        if(Instance != null)
        {
            throw new System.Exception();
        }
        Instance = this; 
    }
    public enum Actions
        {
        up,
        down, left, right,
        none
    }
    // Update is called once per frame
    public Actions getInput()
    {
     //   Debug.Log("Updating");
        if(Input.GetKeyDown(KeyCode.W))
        {
            return Actions.up;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            return Actions.right;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            return Actions.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            return Actions.left;
        }
        return Actions.none;
    }
}
