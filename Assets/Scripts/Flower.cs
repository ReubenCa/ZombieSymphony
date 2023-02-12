using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : ImmovableEntity
{   
    private SpriteRenderer spriteRenderer;
    public override bool getPassable(bool CanPassthroughZombies)
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
        float xOffset = Random.Range(-0.35f, 0.35f);
        float yOffset = Random.Range(-0.35f, 0.35f);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + xOffset
            , gameObject.transform.position.y + yOffset, 0);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
