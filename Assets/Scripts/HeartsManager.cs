using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeartsManager : MonoBehaviour
{
    [SerializeField]
    Heart[] Hearts;

    public static HeartsManager instance;
    private void Awake()
    {
        instance = this;
    }

    /* [SerializeField]
     float testlives;*/
    private void Update()
    {
        //UpdateHealth(testlives);
        UpdateHealth(GameManager.instance.Lives);
    }
    public void OnLifeLost(float TotalHealth)
    {
        try
        {
            Hearts[(int)Math.Ceiling(TotalHealth % 1.0f)].PlayParticles();
        }
        catch {//In God mode theere may not be a heart to represent lost life
               }
        
    }

    public void UpdateHealth(float Health)
    {
        if (Health < 0)
        {
            for (int i = 0; i < Hearts.Length; i++)
            {
                Hearts[i].UpdateHealth(0f);
            }
            return;
        }


        for(int i = 0; i < Hearts.Length; i++) {
            Hearts[i].UpdateHealth(Mathf.Min(Health, 1f));
            Health = Mathf.Max(0f, Health - 1f);
        }
    }
}
