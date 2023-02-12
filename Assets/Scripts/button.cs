using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class button : MonoBehaviour
{
    public SpriteRenderer colourChangeOnHover;
    private bool mouseOver;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnMouseEnter() {
        mouseOver=true;
        colourChangeOnHover.color=Color.black;
    }
    private void OnMouseExit() {
        mouseOver=false;
        colourChangeOnHover.color=Color.white;
    }
    private void OnMouseDown() {
        if(mouseOver){
            SceneManager.LoadScene("OrchestraScene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)){
            SceneManager.LoadScene("OrchestraScene");
        }
    }
}
