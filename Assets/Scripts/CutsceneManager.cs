using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class CutsceneManager : MonoBehaviour
{
    // Usage in Yarn: <<play_cutscene Intro_Cutscene_Timeline>>
    // Usage in Yarn: <<play_cutscene Ending_Good_Timeline>>
    [YarnCommand("play_cutscene")]
    public static void PlayCutscene(string timelineObjectName)
    {
        GameObject timelineObj = GameObject.Find(timelineObjectName);

        if (timelineObj != null)
        {
            PlayableDirector director = timelineObj.GetComponent<PlayableDirector>();
            if (director != null)
            {
                director.Play();
            }
            else
            {
                Debug.LogWarning($"[CutsceneManager] '{timelineObjectName}' missing PlayableDirector!");
            }
        }
        else
        {
            Debug.LogWarning($"[CutsceneManager] Could not find Timeline GameObject named '{timelineObjectName}'");
        }
    }
}