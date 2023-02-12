using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            throw new System.Exception("Multiple Game Managers");
        }
        instance = this;
        AllGraves = new List<Grave>();
    }

    
    public List<Grave> AllGraves { private set; get;}

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
    [SerializeField]
   public float Lives = 3f;
    public void MoveNotOnBeat()
    {
        Lives -= 1;
        if(Lives < 0 && !godMode)
        {
            PlayerOutOfLives();
        }
    }
    bool ranout = false;
    private void PlayerOutOfLives()
    {
        if (ranout)
            return;
        Lives = -10;
        BetweenScenesData.WokeZombies = true;
        ranout = true;
        foreach(Grave grave in AllGraves) { 
        SpawnZombie(grave,1,1,1000,10000,1,1,0.1f);
        }
    }

    [SerializeField]
    bool godMode = false;
    public void PlayerBitten()
    {
        if(!godMode)
        {
            BetweenScenesData.score = FlowersCollected;
            SceneManager.LoadScene("DeathScene");
        }
    }
    public void onFlowerCollected()
    {
        FlowersCollected++;
        FlowersCurrentlyInScene--;
    }

    public bool SpawnFlowers = true;
    float flowerspawncritera = 10f;
    float flowerspawntimer = 0f;
    [SerializeField]
    float LifeRegenRate;

    [SerializeField]
    float BaseMinZombieSpawn;

    public void ZombieDied()
    {
        ZombiesAlive--;
    }

    [SerializeField]
    float BaseMaxZombieSpawn;

    public bool SpawnZombies = true;
    float timesincelastzombiespawn = 0f;
    float NextSpawnCriteria = 0f;
    int ZombiesAlive = 0;
    void Update()
    {
        FlowerText.text = FlowersCollected.ToString();
        timesincelastzombiespawn += Time.deltaTime / (ZombiesAlive + 1);
        //new List<float> { 1f,2f}
        if((ZombiesAlive == 0 || timesincelastzombiespawn > NextSpawnCriteria) && SpawnZombies)
        {
            SpawnZombie();
            timesincelastzombiespawn = 0;
            NextSpawnCriteria = Random.Range(BaseMinZombieSpawn, BaseMaxZombieSpawn);
        }

        Lives = Mathf.Min(Lives + LifeRegenRate * Time.deltaTime, 3f);

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
    [SerializeField]
    GameObject ZombiePrefab;


    public Zombie SpawnZombie(Grave spawngrave = null, int TargetAccuracy = 2, int TargetUpdateFreq = 4, 
        float MinTimetoSleep = 3, float MaxTimeToSleep = 30, float MaxTimeSleeping = 12, float minTimeSleeping = 7 ,
       float TimeToMove = -10)
    {
        if (spawngrave == null)
        {
            spawngrave = AllGraves[Random.Range(0, AllGraves.Count)];
        }


        ZombiesAlive++;
        GameObject zombie = Instantiate(ZombiePrefab, new Vector3((float)spawngrave.BottomLeftX, (float)spawngrave.BottomLeftY, 0), Quaternion.identity);
        Zombie zombieComp =  zombie.GetComponent<Zombie>();
        zombieComp.SetStats(MinTimetoSleep, MaxTimeToSleep,MaxTimeSleeping,minTimeSleeping,TargetAccuracy, TargetUpdateFreq, TimeToMove);
        return zombieComp;

        
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

    [SerializeField]
    private TextMeshProUGUI FlowerText;

}
