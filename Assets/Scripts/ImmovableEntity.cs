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


    public int BottomLeftX { private set; get; }
    public int BottomLeftY { private set; get; }

    public new void Init()
    {
         BottomLeftX = (int)Math.Floor(gameObject.transform.position.x);
        base.Init();
         BottomLeftY = (int)Math.Floor(gameObject.transform.position.y);
        for (int xiter = BottomLeftX; xiter< BottomLeftX + Width; xiter++)
        {
            for(int yiter= BottomLeftY; yiter<BottomLeftY+ Height; yiter++)
            {
                TerrainManager.Instance.SetTerrainTile(xiter, yiter, this);
            }
        }
    }
}
