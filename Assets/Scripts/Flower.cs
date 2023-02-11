using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : ImmovableEntity
{
    public override bool getPassable()
    {
        return true;
    }


    public override void onContact()
    {
        GameManager.instance.onFlowerCollected();
        TerrainManager.Instance.ClearFromTile(BottomLeftX, BottomLeftY, this);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        //TODO randdomise Position Slightly
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
