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
    public TextMeshPro textComponent;
    public AudioSource audioSource;
    public List<DialogueLine> dialogue;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    IEnumerator runDialogue(){
        foreach (DialogueLine dialogueLine in dialogue)
        {
            textComponent.text=dialogueLine.dialogue;
            audioSource.clip=dialogueLine.voiceLine;
            audioSource.Play();
            yield return new WaitForSeconds(dialogueLine.seconds);
        }
        //ClearUp
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
