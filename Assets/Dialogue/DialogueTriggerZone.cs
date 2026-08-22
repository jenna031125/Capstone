using UnityEngine;
using Yarn.Unity;

public class DialogueTriggerZone : MonoBehaviour
{
    [SerializeField] private string _nodeToStart = "Normal_Ending_Cutscene";
    [SerializeField] private bool _triggerOnce = true;

    // Optional: Only trigger if day matches (e.g. Day 4 for Normal Ending)
    [SerializeField] private bool _checkDayCount = true;
    [SerializeField] private float _requiredDayCount = 4f;

    private DialogueRunner _dialogueRunner;
    private VariableStorageBehaviour _variableStorage;
    private bool _hasTriggered = false;

    void Awake()
    {
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        _variableStorage = FindFirstObjectByType<VariableStorageBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasTriggered && _triggerOnce) return;

        if (collision.CompareTag("Player"))
        {
            // Check day count if required
            if (_checkDayCount && _variableStorage != null)
            {
                _variableStorage.TryGetValue("$day_count", out float currentDay);
                if (currentDay != _requiredDayCount) return; // Ignore if it's not Day 4!
            }

            if (_dialogueRunner != null && !_dialogueRunner.IsDialogueRunning)
            {
                _hasTriggered = true;
                _dialogueRunner.StartDialogue(_nodeToStart);
            }
        }
    }
}