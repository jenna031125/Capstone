using UnityEngine;
using Yarn.Unity; // This allows the script to talk to Yarn Spinner

public class YarnInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string startNode;
    //bool dialogueFinished;

    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public bool CanInteract()
    {
        // Prevents interacting if a conversation is already happening
        return !dialogueRunner.IsDialogueRunning;
    }

    public void Interact()
    {
        /*if (dialogueFinished == true)
            return;*/

        // If dialogue isn't running, start the assigned node
        if (!dialogueRunner.IsDialogueRunning)
        {
            //dialogueRunner.onDialogueComplete.AddListener(DialogueComplete);
            dialogueRunner.StartDialogue(startNode);
        }
    }

   /* public void DialogueComplete()
    {
        dialogueFinished = true;
    }
   */
}