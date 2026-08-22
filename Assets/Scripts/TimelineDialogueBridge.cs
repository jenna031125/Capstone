using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class TimelineDialogueBridge : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private DialogueRunner dialogueRunner;

    // Call this from Timeline Signal
    public void StartDialogueFromTimeline(string nodeName)
    {
        if (director != null)
        {
            director.Pause(); // Pause timeline movement while speaking
        }

        if (dialogueRunner != null)
        {
            dialogueRunner.StartDialogue(nodeName);
        }
    }

    // Call this from Yarn when dialogue node completes
    [YarnCommand("resume_timeline")]
    public void ResumeTimeline()
    {
        if (director != null)
        {
            director.Resume(); // Resume character movement after speaking
        }
    }
}