using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            throw new System.Exception("Multiple Game Managers");
        }
        instance = this;
    }

    [SerializeField]
    private GameObject FlowerPrefab;
    [SerializeField]
    private int FlowersCollected;

    int FlowersCurrentlyInScene = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Navigator.Init();
        //SpawnFlower();
    }

    public void onFlowerCollected()
    {
        FlowersCollected++;
        FlowersCurrentlyInScene--;
    }

    public bool SpawnFlowers = true;
    float flowerspawncritera = 10f;
    float flowerspawntimer = 0f;
    void Update()
    {
        if (!SpawnFlowers)
            return;
       // Debug.Log(flowerspawncritera);
        flowerspawntimer += Time.deltaTime/ ((float)FlowersCurrentlyInScene+1);
        if(flowerspawncritera<flowerspawntimer)
        { 
            SpawnFlower();
            flowerspawntimer = 0f;
            flowerspawncritera =0.5f + (float)Random.Range(0, 128) / 16f;
        }
    }
    GameObject ZombiePrefab;
    public void SpawnZombie(int x, int y)
    {
       Instantiate(ZombiePrefab, new Vector3((float)x,(float)y,0), Quaternion.identity);
        
    }
    
    public void SpawnFlower()
    {
        int spawnx = -100;
        int spawny = -100;
        int breakout = 0;
        while(!TerrainManager.Instance.PositionValid(spawnx,spawny))
        {
            (int, int, int, int) dims = TerrainManager.Instance.GetDimensions();
            spawnx = Random.Range(dims.Item3, dims.Item3 + dims.Item1);
            spawny = Random.Range(dims.Item4, dims.Item4 + dims.Item2);
            breakout++;
            if (breakout > 1000)
            {
                throw new System.Exception("Infinite Loop");
            }
        }
       GameObject newFlower = Instantiate(FlowerPrefab, new Vector3(spawnx,spawny,0), Quaternion.identity);
        TerrainManager.Instance.SetTerrainTile(spawnx,spawny, newFlower.GetComponent<Flower>());

        FlowersCurrentlyInScene++;
    }
}
