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
    public float BaseForgiveness; //In Millisecond
    [HideInInspector]
    public float audioStarted;
    private int phase = 0;// Starts at Phase 0
    private int nextPhase=0; //passed to loadphase after invoke
    private float TimeInBar;
    [HideInInspector]
    public float TimeIn8Bars;
    public float offsetMillis;
    public  AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        TimeInBar=60000/(bpm/4); //In Millseconds
        TimeIn8Bars=TimeInBar*8;
        loadPhase(0);
    }

    public IEnumerator callLastPhases(List<float> TimePerPhase){
        for (int i = 2; i < 5; i++)
        {
            Debug.Log("Waiting for: "+TimePerPhase[i]);
            yield return new WaitForSeconds(TimePerPhase[i]);
            Debug.Log("LoadingWhenReady");
            loadPhaseWhenReady(i+1);
        }
    }


    public void loadPhaseWhenReady(int newPhase){
        audioSource = GetComponent<AudioSource>();
        TimeInBar = 60000 / (bpm / 4); //In Millseconds
        TimeIn8Bars = TimeInBar * 8;
        float TimeElapsedSinceAudioStarted=Time.time-audioStarted;
        float actualBar=(TimeElapsedSinceAudioStarted%(TimeIn8Bars/1000))*8*1000/TimeIn8Bars;
        float timeUntilNextBar = TimeIn8Bars/1000-actualBar;
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
        Debug.Log(newPhase);
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

    float totalmiss;
    int totalattempts;
    [SerializeField]
    float ForgivenessDecayPerSecond;
    [SerializeField]
    float ForgivenessAsymptopicMinimum;


    [SerializeField]
    float CurrentForgiveness;
    public bool checkIfBeat(){

        float TimeElapsedSinceAudioStarted=Time.time-audioStarted;
        float actualBar=(TimeElapsedSinceAudioStarted%(TimeIn8Bars/1000))*8*1000/TimeIn8Bars;
        float minDistanceToBeat=float.MaxValue;
        foreach (float BeatPosition in PhaseBeatPositions[phase].beatPositions)
        {
            minDistanceToBeat=Mathf.Min(minDistanceToBeat,Mathf.Abs(BeatPosition-actualBar+offsetMillis));
            //Debug.Log(-BeatPosition + actualBar);
        }
        //Debug.Log(minDistanceToBeat);
        //Debug.Log(minDistanceToBeat < forgiveness ? "HIT" : "MISS");

        //Debug.Log("");
        CurrentForgiveness = (BaseForgiveness * ForgivenessAsymptopicMinimum + BaseForgiveness *
            (1 - ForgivenessAsymptopicMinimum) * Mathf.Pow(1-ForgivenessDecayPerSecond, TimeElapsedSinceAudioStarted));
        return minDistanceToBeat < CurrentForgiveness;
    }
}
