using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBeat : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    // Update is called once per frame
    void Update()
    {
        if(MusicManager.instance.checkIfBeat())
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    }
}
