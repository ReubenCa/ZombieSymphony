using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ImmovableEntity : Entity
{

    [SerializeField]
    int Height;
    [SerializeField]
    int Width;

    public override bool getPassable()
    {
        return false;
    }


    public new void Init()
    {
        int BottomLeftX = (int)Math.Floor(gameObject.transform.position.x);
        base.Init();
        int BottomLeftY = (int)Math.Floor(gameObject.transform.position.y);
        for (int xiter = BottomLeftX; xiter< BottomLeftX + Width; xiter++)
        {
            for(int yiter= BottomLeftY; yiter<BottomLeftY+ Height; yiter++)
            {
                TerrainManager.Instance.SetTerrainTile(xiter, yiter, this);
            }
        }
    }
}
