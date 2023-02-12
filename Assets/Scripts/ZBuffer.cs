using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZBuffer : MonoBehaviour
{
     float Ratio = 2f;
     public int offset;
    private void Update()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingOrder=(int)Mathf.Floor(-gameObject.transform.position.y * Ratio)+offset;
        }
    }
}
