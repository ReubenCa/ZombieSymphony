using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract bool getPassable();

    public virtual void onContact()
    {
        return;
    }

    protected void Init()
    {
        //if (TerrainManager.Instance.GetTerrainTile(x,y) != null)
        //{
         //   throw new System.Exception("Muliple entities are initiated with x,y coords (" + x + "," + y + ")");
        //}
        

    }

}
