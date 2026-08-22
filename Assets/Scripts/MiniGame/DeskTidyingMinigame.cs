using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class DeskTidyingMinigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _totalPapersCount = 8;
    [SerializeField] private float _delayBeforeClose = 1.5f;
    bool dialogueFinished;

    [Header("World Object")]
    public SpriteRenderer obj;
    public Sprite completedSprite;

    private Sprite _originalMessySprite; // Automatically remembers the messy sprite at start
    private int _stackedCount = 0;
    public int StackedCount => _stackedCount;

    void Awake()
    {
        // Remember whatever sprite the desk started with (the messy one)
        if (obj != null)
        {
            _originalMessySprite = obj.sprite;
        }
    }

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
        if (obj != null && completedSprite != null)
        {
            obj.sprite = completedSprite;
        }
    }

    public void CloseMinigame()
    {
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMANDS ---
    [YarnCommand("start_desk_minigame")]
    public static void StartDeskCommand()
    {
        DeskTidyingMinigame minigame = FindFirstObjectByType<DeskTidyingMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }

    [YarnCommand("reset_desk_minigame")]
    public static void ResetDeskCommand()
    {
        DeskTidyingMinigame minigame = FindFirstObjectByType<DeskTidyingMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.ResetMinigameState();
        }
    }

    public void ResetMinigameState()
    {
        _stackedCount = 0;

        // 1. Revert world desk sprite back to original messy sprite
        if (obj != null && _originalMessySprite != null)
        {
            obj.sprite = _originalMessySprite;
        }

        // 2. Reset every paper in the minigame back to its start position and state
        DraggablePaper[] papers = GetComponentsInChildren<DraggablePaper>(true);
        foreach (DraggablePaper paper in papers)
        {
            paper.gameObject.SetActive(true);
            paper.ResetPaper();
        }
    }

    public void DialogueComplete()
    {
        dialogueFinished = true;
    }
}