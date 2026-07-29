using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DustingMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _dustImage;
    [SerializeField] private Image _dusterCursorImage;
    [SerializeField] private Button _frameClickButton;

    [Header("Sprites")]
    [SerializeField] private Sprite _dusterIdleSprite;
    [SerializeField] private Sprite _dusterClickedSprite;

    [Header("Settings")]
    [SerializeField] private int _clicksRequired = 10;
    [SerializeField] private float _clickAnimDuration = 0.15f; // How long the clicked sprite stays
    [SerializeField] private float _delayBeforeClose = 1.5f;

    private int _currentClicks = 0;
    private bool _isComplete = false;

    void OnEnable()
    {
        _currentClicks = 0;
        _isComplete = false;

        // Reset Dust Transparency to 100% full opacity
        if (_dustImage != null)
        {
            Color c = _dustImage.color;
            c.a = 1f;
            _dustImage.color = c;
            _dustImage.gameObject.SetActive(true);
        }

        // Set up cursor sprite
        if (_dusterCursorImage != null && _dusterIdleSprite != null)
        {
            _dusterCursorImage.sprite = _dusterIdleSprite;
            _dusterCursorImage.gameObject.SetActive(true);
            Cursor.visible = false; // Hide system hardware cursor while dusting
        }

        // Hook up button click listener
        if (_frameClickButton != null)
        {
            _frameClickButton.interactable = true;
            _frameClickButton.onClick.RemoveAllListeners();
            _frameClickButton.onClick.AddListener(OnFrameClicked);
        }
    }

    void OnDisable()
    {
        // Restore standard hardware cursor when minigame closes
        Cursor.visible = true;
    }

    void Update()
    {
        // Smoothly stick the custom duster sprite to mouse cursor position
        if (_dusterCursorImage != null && !_isComplete)
        {
            _dusterCursorImage.transform.position = Input.mousePosition;
        }
    }

    private void OnFrameClicked()
    {
        if (_isComplete) return;

        _currentClicks++;

        // 1. Calculate and lower dust opacity linearly
        if (_dustImage != null)
        {
            float alpha = 1f - ((float)_currentClicks / _clicksRequired);
            Color c = _dustImage.color;
            c.a = Mathf.Clamp01(alpha);
            _dustImage.color = c;
        }

        // 2. Play 1-frame click animation on the duster
        StopAllCoroutines();
        StartCoroutine(DusterClickAnimationRoutine());

        // 3. Check for completion
        if (_currentClicks >= _clicksRequired)
        {
            CompleteMinigame();
        }
    }

    private IEnumerator DusterClickAnimationRoutine()
    {
        if (_dusterCursorImage != null && _dusterClickedSprite != null)
        {
            _dusterCursorImage.sprite = _dusterClickedSprite;
            yield return new WaitForSeconds(_clickAnimDuration);
            _dusterCursorImage.sprite = _dusterIdleSprite;
        }
    }

    private void CompleteMinigame()
    {
        _isComplete = true;

        if (_frameClickButton != null)
        {
            _frameClickButton.interactable = false;
        }

        // Keep cursor visible again once completed
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

    // --- YARN SPINNER COMMAND ---
    [YarnCommand("start_dusting_minigame")]
    public static void StartDustingCommand()
    {
        DustingMinigame minigame = FindFirstObjectByType<DustingMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }
}