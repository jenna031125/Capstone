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
    public Sprite completedSprite;

    private bool _isComplete = false;

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
    }

    private void CompleteMinigame()
    {
        _isComplete = true;
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