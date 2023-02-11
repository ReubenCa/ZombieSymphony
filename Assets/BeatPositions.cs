using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class BeatPositions : ScriptableObject
{
    public List<float> beatPositions;
}