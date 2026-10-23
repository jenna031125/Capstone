using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FloorCleaningMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _broomCursorImage;
    [SerializeField] private FloorDustSpot[] _dustPiles;

    [Header("Sprites")]
    [SerializeField] private Sprite _broomIdleSprite;
    [SerializeField] private Sprite _broomClickedSprite;

    [Header("Settings")]
    [SerializeField] private float _sweepAnimDuration = 0.15f;
    [SerializeField] private float _delayBeforeClose = 1.5f;

    [Header("World Object")]
    public SpriteRenderer obj;
    [SerializeField] private Sprite _dirtyWorldSprite; // 먼지/쓰레기가 쌓인 상태의 월드 스프라이트 (미완료)
    public Sprite completedSprite;                     // 깨끗이 쓸어낸 상태의 월드 스프라이트 (완료)

    private Sprite _originalDirtySprite;
    private bool _isComplete = false;

    void Awake()
    {
        // 1. _dirtyWorldSprite가 직접 지정되어 있으면 사용
        if (_dirtyWorldSprite != null)
        {
            _originalDirtySprite = _dirtyWorldSprite;
        }
        // 2. 할당이 없다면 시작 시점의 sprite를 기록
        else if (obj != null)
        {
            _originalDirtySprite = obj.sprite;
        }

        // 게임 시작 시 미완료(더러운) 스프라이트로 강제 세팅
        if (obj != null && _originalDirtySprite != null)
        {
            obj.sprite = _originalDirtySprite;
        }
    }

    void OnEnable()
    {
        _isComplete = false;

        // Set up broom cursor
        if (_broomCursorImage != null && _broomIdleSprite != null)
        {
            _broomCursorImage.sprite = _broomIdleSprite;
            _broomCursorImage.gameObject.SetActive(true);
            Cursor.visible = false; // Hide system cursor
        }
    }

    void OnDisable()
    {
        Cursor.visible = true; // Restore system cursor
    }

    void Update()
    {
        // Broom follows mouse cursor
        if (_broomCursorImage != null && !_isComplete)
        {
            _broomCursorImage.transform.position = Input.mousePosition;
        }
    }

    public void TriggerBroomSweepAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(BroomSweepRoutine());
    }

    private IEnumerator BroomSweepRoutine()
    {
        if (_broomCursorImage != null && _broomClickedSprite != null)
        {
            _broomCursorImage.sprite = _broomClickedSprite;
            yield return new WaitForSeconds(_sweepAnimDuration);
            _broomCursorImage.sprite = _broomIdleSprite;
        }
    }

    public void CheckMinigameCompletion()
    {
        // Verify if all dust piles in the scene are completely cleaned
        foreach (var dust in _dustPiles)
        {
            if (dust != null && !dust.IsClean)
            {
                return; // Still some dust left
            }
        }

        CompleteMinigame();
        FindFirstObjectByType<OfficeWorldManager>()?.UpdateAllObjectSprites();
    }

    private void CompleteMinigame()
    {
        _isComplete = true;

        // 청소가 완료되면 월드 스프라이트를 깨끗한 스프라이트로 변경
        if (obj != null && completedSprite != null)
        {
            obj.sprite = completedSprite;
        }

        Cursor.visible = true;
        StartCoroutine(WaitAndCloseRoutine());
    }

    private IEnumerator WaitAndCloseRoutine()
    {
        yield return new WaitForSeconds(_delayBeforeClose);
        CloseMinigame();
    }

    public void CloseMinigame()
    {
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMANDS ---
    [YarnCommand("start_floor_minigame_left")]
    public static void StartFloorLeftCommand()
    {
        // Finds the specific canvas assigned to the left spot
        FloorCleaningMinigame[] games = FindObjectsByType<FloorCleaningMinigame>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var game in games)
        {
            if (game.gameObject.name.Contains("Left"))
            {
                game.gameObject.SetActive(true);
                break;
            }
        }
    }

    [YarnCommand("start_floor_minigame_right")]
    public static void StartFloorRightCommand()
    {
        // Finds the specific canvas assigned to the right spot
        FloorCleaningMinigame[] games = FindObjectsByType<FloorCleaningMinigame>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var game in games)
        {
            if (game.gameObject.name.Contains("Right"))
            {
                game.gameObject.SetActive(true);
                break;
            }
        }
    }
}