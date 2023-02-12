using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreenUpdater : MonoBehaviour
{
    [SerializeField]
    TMP_Text ScoreText;

  
    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text =(BetweenScenesData.WokeZombies ? "You Woke the Zombies!": "You Were Bitten!" )
            + "\nScore:\t" + BetweenScenesData.score.ToString();
        BetweenScenesData.Reset();
    }

    
}
