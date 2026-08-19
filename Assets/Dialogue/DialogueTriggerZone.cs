using UnityEngine;
using Yarn.Unity;

public class DialogueTriggerZone : MonoBehaviour
{
    [SerializeField] private string _nodeToStart = "Mansion_Entrance_Arrival";
    [SerializeField] private bool _triggerOnce = true;

    private DialogueRunner _dialogueRunner;
    private bool _hasTriggered = false;

    void Awake()
    {
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasTriggered && _triggerOnce) return;

        if (collision.CompareTag("Player"))
        {
            if (_dialogueRunner != null && !_dialogueRunner.IsDialogueRunning)
            {
                _hasTriggered = true;
                _dialogueRunner.StartDialogue(_nodeToStart);
            }
        }
    }
}