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
        StartCoroutine(runDialogue());
    }
    IEnumerator runDialogue(){
        foreach (DialogueLine dialogueLine in dialogue)
        {
            textComponent.text=dialogueLine.dialogue;
            audioSource.clip=dialogueLine.voiceLine;
            audioSource.Play();
            if(dialogueLine.dialogue.Contains("not good")){
                
                firstZombie = gameManager.SpawnZombie(InitialSpawnGrave,7,10,3f,3.1f,10000,1000);
            }
            if(dialogueLine.dialogue.Contains("asleep")){
                //firstZombie.goSleepForever();
            }
            yield return new WaitForSeconds(dialogueLine.seconds);
        }
        //ClearUp
        gameManager.SpawnFlowers=true;
        gameManager.SpawnZombies=true;
        gameObject.SetActive(false);
        graveDiggerAnimation.Play("GraveDiggerLeave");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
