using UnityEngine;
using Yarn.Unity;

public class DialogueFastForward : MonoBehaviour
{
    // Drag the object with the 'Line Advancer' script into this slot in the Inspector
    public LineAdvancer lineAdvancer;
    private DialogueRunner runner;

    void Start()
    {
        runner = GetComponent<DialogueRunner>();
    }

    void Update()
    {
        // Safety check to ensure we have the references
        if (runner != null && lineAdvancer != null)
        {
            // If dialogue is running and you press or hold Left Control
            if (runner.IsDialogueRunning && Input.GetKey(KeyCode.LeftControl))
            {
                // This is the EXACT function found in your LineAdvancer script!
                lineAdvancer.RequestNextLine();
            }
        }
    }
}