using UnityEngine;
using Yarn.Unity; // This allows the script to talk to Yarn Spinner

public class YarnInteractable : MonoBehaviour, IInteractable
{
    // Type the name of the node from your .yarn file here in the Inspector
    [SerializeField] private string startNode;

    private DialogueRunner dialogueRunner;

    void Start()
    {
        // Automatically finds the DialogueRunner in your scene
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public bool CanInteract()
    {
        // Prevents interacting if a conversation is already happening
        return !dialogueRunner.IsDialogueRunning;
    }

    public void Interact()
    {
        // If dialogue isn't running, start the assigned node
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(startNode);
        }
    }
}