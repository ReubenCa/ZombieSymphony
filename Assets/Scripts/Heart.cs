using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Heart : MonoBehaviour
{
    [SerializeField]
    float MaskEmptyHeight;

    [SerializeField]
    float MaskFullHeight;

    [SerializeField]
    float Fill;

    [SerializeField]
    private GameObject mask;

    public void UpdateHealth(float health)
    {
       // Debug.Log(mask.gameObject.transform.position);
        float Height = MaskEmptyHeight + health *(MaskFullHeight- MaskEmptyHeight );
        mask.gameObject.transform.localPosition = new Vector3(mask.gameObject.transform.localPosition.x
            , Height, mask.gameObject.transform.localPosition.z);
      //  Debug.Log(mask.gameObject.transform.position);
    }

}
