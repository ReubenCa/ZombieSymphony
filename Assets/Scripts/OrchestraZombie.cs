using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrchestraZombie : MonoBehaviour
{
    public MusicManager manager;
    public float playLength; //In Milliseconds
    public BeatPositions beatPositions;
    public Sprite playSprite;
    private Sprite normalSprite;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        normalSprite=spriteRenderer.sprite;
    }
    public void enter(){
        GetComponent<Animator>().Play("OrchestraZombieWalk");
    }

    // Update is called once per frame
    void Update()
    {
        checkPlay();   
    }
    void checkPlay(){
        spriteRenderer.sprite=normalSprite;
        float TimeElapsedSinceAudioStarted=Time.time/1000f-manager.audioStarted;
        float actualBar=(TimeElapsedSinceAudioStarted%manager.TimeIn8Bars)*8*1000/manager.TimeIn8Bars;
        Debug.Log(actualBar);
        foreach (float BeatPosition in beatPositions.beatPositions)
        {
            if(BeatPosition<actualBar){
                if(actualBar-BeatPosition<playLength){
                    spriteRenderer.sprite=playSprite;
                }
            }
        }
    }
}
