using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZBuffer : MonoBehaviour
{
     float Ratio = 0.01f;
    [SerializeField]
    float Offset = 0f;
    private void Update()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
            gameObject.transform.position.y, gameObject.transform.position.y * Ratio +Offset);
    }
}
