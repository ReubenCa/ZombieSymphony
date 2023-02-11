using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { private set; get; }
    private void Awake()
    {
        if (instance != null) { throw new System.Exception("TWO MUSIC MANAGERS"); }
        instance = this;
    }
    //5 Phases of Music are rythym loops of 8 bars each
    //
    public List<BeatPositions> PhaseBeatPositions;
    public List<AudioClip> PhaseAudioClip;
    public List<OrchestraZombie> ZombiesToTrigger;
    public int bpm;
    public float forgiveness; //In Millisecond
    [HideInInspector]
    public float audioStarted;
    private int phase = 0;// Starts at Phase 0
    private int nextPhase=0; //passed to loadphase after invoke
    private float TimeInBar;
    [HideInInspector]
    public float TimeIn8Bars;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        TimeInBar=60000/(bpm/4); //In Millseconds
        TimeIn8Bars=TimeInBar*8;
        loadPhase(0);
        StartCoroutine(callPhases(new List<float>{0f,16f,16f,16f,16f,16f}));
    }

    IEnumerator callPhases(List<float> TimePerPhase){
        for (int i = 0; i < 6; i++)
        {
            Debug.Log("Waiting for: "+TimePerPhase[i]);
            yield return new WaitForSeconds(TimePerPhase[i]);
            Debug.Log("LoadingWhenReady");
            loadPhaseWhenReady(i+1);
        }
    }


    public void loadPhaseWhenReady(int newPhase){
        float TimeElapsedSinceAudioStarted=Time.time-audioStarted;
        float actualBar=TimeElapsedSinceAudioStarted%TimeIn8Bars;
        float timeUntilNextBar = TimeIn8Bars-actualBar;
        nextPhase=newPhase;
        Debug.Log(timeUntilNextBar);
        Invoke("loadPhase",timeUntilNextBar/1000f);
    }
    public void loadPhase(){
        loadPhase(-1);
    }

    public void loadPhase(int newPhase){
        if(newPhase==-1){
            newPhase=nextPhase;
        }
        audioSource.clip=PhaseAudioClip[newPhase];
        audioSource.Play();
        audioStarted=Time.time;
        phase=newPhase;
        if(newPhase!=0){
             ZombiesToTrigger[newPhase].enter();
             if(newPhase==1){
                ZombiesToTrigger[0].enter();
             }
        }
       
    }

    // Update is called once per frame


    public bool checkIfBeat(){

        float TimeElapsedSinceAudioStarted=Time.time-audioStarted;
        float actualBar=TimeElapsedSinceAudioStarted%TimeIn8Bars;
        float minDistanceToBeat=float.MaxValue;
        foreach (float BeatPosition in PhaseBeatPositions[phase].beatPositions)
        {
            minDistanceToBeat=Mathf.Min(minDistanceToBeat,Mathf.Abs(BeatPosition-actualBar));
        }
        Debug.Log(minDistanceToBeat < forgiveness ? "HIT" : "MISS");
        return minDistanceToBeat<forgiveness;
    }
}
