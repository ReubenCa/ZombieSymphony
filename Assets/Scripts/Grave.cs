using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : ImmovableEntity
{
    public override bool getPassable()
    {
        return false;
    }

    private void Start()
    {
        base.Init();
    }
}
