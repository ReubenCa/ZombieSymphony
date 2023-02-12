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
    // Start is called before the first frame update
    void Start()
    {
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
                firstZombie = gameManager.SpawnZombie();
            }
            if(dialogueLine.dialogue.Contains("asleep")){
                firstZombie.goSleepForever();
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
