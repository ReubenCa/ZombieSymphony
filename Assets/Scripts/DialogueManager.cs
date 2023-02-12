using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]
public class DialogueLine{
    public AudioClip voiceLine;
    public float seconds;
    public string dialogue;

}
    
public class DialogueManager : MonoBehaviour
{
    public Animator graveDiggerAnimation;
    public TextMeshPro textComponent;
    public AudioSource audioSource;
    public List<DialogueLine> dialogue;
    public GameManager gameManager;
    public MusicManager musicManager;
    private Zombie firstZombie;
    [SerializeField]
    private Grave InitialSpawnGrave;
    // Start is called before the first frame update
    void Start()
    {
        if (InitialSpawnGrave == null)
        {
            throw new System.Exception("in the scene put a grave far a away from the player in the initial grave" +
                " option, cant do it now as I dont want to edit working scene");
        }
        gameManager.SpawnFlowers=false;
        gameManager.SpawnZombies=false;
        StartCoroutine(runDialogueFrom(PlayerPrefs.GetInt("tutorialphase",0))); //0 is Default
    }
    IEnumerator runFirstPhaseInSecond(){
        yield return new WaitForSeconds(0.1f);
        musicManager.loadPhase(1);
    }
    IEnumerator runDialogueFrom(int phase){
        int startDialogue=0;
        if(phase!=0){
            if(phase==1){startDialogue=7;
            musicManager.loadPhase(1);
            }
            if(phase==2){startDialogue=12;StartCoroutine(runFirstPhaseInSecond());}
            if(phase==3){startDialogue=19;StartCoroutine(runFirstPhaseInSecond());}
        }
        foreach (DialogueLine dialogueLine in dialogue.GetRange(startDialogue,dialogue.Count-startDialogue))
        {
            textComponent.text=dialogueLine.dialogue;
            audioSource.clip=dialogueLine.voiceLine;
            audioSource.Play();
            if(dialogueLine.dialogue.Contains("not good")){
                
                firstZombie = gameManager.SpawnZombie(InitialSpawnGrave,7,10,3f,3.1f,10000,1000);
            }
            if(dialogueLine.dialogue.Contains("one of them")){
                musicManager.loadPhaseWhenReady(1);
                PlayerPrefs.SetInt("tutorialphase",1);
            }
            if(dialogueLine.dialogue.Contains("WASD")){
                PlayerPrefs.SetInt("tutorialphase",2);
            }
            if(dialogueLine.dialogue.Contains("asleep")){
                //firstZombie.goSleepForever();
            }
            Debug.Log("Seconds "+dialogueLine.seconds);
            yield return new WaitForSeconds(dialogueLine.seconds);
            Debug.Log(dialogueLine.dialogue);
        }
        //ClearUp
        PlayerPrefs.SetInt("tutorialphase",3);
        gameManager.SpawnFlowers=true;
        gameManager.SpawnZombies=true;
        
        graveDiggerAnimation.Play("GraveDiggerLeave");
        musicManager.loadPhaseWhenReady(2);
        StartCoroutine(musicManager.callLastPhases(new List<float>{32f,32f,32f,32f}));
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
