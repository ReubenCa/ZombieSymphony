using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsManager : MonoBehaviour
{
    [SerializeField]
    Heart[] Hearts;

   /* [SerializeField]
    float testlives;*/
    private void Update()
    {
        //UpdateHealth(testlives);
        UpdateHealth(GameManager.instance.Lives);
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
