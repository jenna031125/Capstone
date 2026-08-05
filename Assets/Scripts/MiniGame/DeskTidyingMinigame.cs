using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class DeskTidyingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _totalPapersCount = 8;
    [SerializeField] private float _delayBeforeClose = 1.5f;
    bool dialogueFinished;

    private int _stackedCount = 0;
    public int StackedCount => _stackedCount;

    void OnEnable()
    {
        _stackedCount = 0;
    }

    public void OnPaperStacked()
    {
        _stackedCount++;

        // Check if all papers are neatly stacked
        if (_stackedCount >= _totalPapersCount)
        {
            StartCoroutine(CompleteMinigameRoutine());
        }
    }

    private IEnumerator CompleteMinigameRoutine()
    {
        Debug.Log("Desk is completely tidied!");
        yield return new WaitForSeconds(_delayBeforeClose);
        CloseMinigame();
    }

    public void CloseMinigame()
    {
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMAND ---
    [YarnCommand("start_desk_minigame")]
    public static void StartDeskCommand()
    {
        DeskTidyingMinigame minigame = FindFirstObjectByType<DeskTidyingMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }
    public void DialogueComplete()
    {
        dialogueFinished = true;
    }
}