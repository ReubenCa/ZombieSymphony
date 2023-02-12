using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BetweenScenesData 
{
    //Yeah yeah yeah theres better ways of doing this

    public static bool WokeZombies = false;

    public static int score = 0;

    public static void Reset()
    {
        WokeZombies = false;
        score = 0;
    }
}
