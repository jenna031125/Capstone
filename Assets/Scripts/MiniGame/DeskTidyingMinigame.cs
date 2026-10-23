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
    [SerializeField] private Sprite _messyWorldSprite; // 어지러운 상태의 월드 스프라이트 (미완료)
    public Sprite completedSprite;                      // 정돈된 상태의 월드 스프라이트 (완료)

    private Sprite _originalMessySprite;
    private int _stackedCount = 0;
    public int StackedCount => _stackedCount;

    void Awake()
    {
        // 1. _messyWorldSprite가 직접 할당되어 있다면 이를 명시적으로 사용
        if (_messyWorldSprite != null)
        {
            _originalMessySprite = _messyWorldSprite;
        }
        // 2. 할당이 없다면 기존처럼 시작 시점의 sprite를 기재
        else if (obj != null)
        {
            _originalMessySprite = obj.sprite;
        }

        // 게임 시작 시 미완료(어지러운) 스프라이트로 강제 세팅
        if (obj != null && _originalMessySprite != null)
        {
            obj.sprite = _originalMessySprite;
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

        // 1. 미니게임 완료 시 정돈된 스프라이트로 교체
        if (obj != null && completedSprite != null)
        {
            obj.sprite = completedSprite;
        }

        // 2. OfficeWorldManager에 청소 상태 즉시 반영
        FindFirstObjectByType<OfficeWorldManager>()?.UpdateAllObjectSprites();
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