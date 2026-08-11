using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class DustingMinigame : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image _dustImage;
    [SerializeField] private Image _dusterCursorImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _dusterIdleSprite;
    [SerializeField] private Sprite _dusterClickedSprite;

    [Header("Settings")]
    [SerializeField] private int _clicksRequired = 10;
    [SerializeField] private float _clickAnimDuration = 0.15f;
    [SerializeField] private float _delayBeforeClose = 1.5f;

    [Header("World Object")]
    public SpriteRenderer obj;
    public Sprite completedSprite;

    private int _currentClicks = 0;
    private bool _isComplete = false;

    void OnEnable()
    {
        _currentClicks = 0;
        _isComplete = false;

        // Reset dust opacity to 100%
        if (_dustImage != null)
        {
            Color c = _dustImage.color;
            c.a = 1f;
            _dustImage.color = c;
            _dustImage.gameObject.SetActive(true);
        }

        // Setup custom duster cursor
        if (_dusterCursorImage != null && _dusterIdleSprite != null)
        {
            _dusterCursorImage.sprite = _dusterIdleSprite;
            _dusterCursorImage.gameObject.SetActive(true);
            Cursor.visible = false;
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        if (_dusterCursorImage != null && !_isComplete)
        {
            _dusterCursorImage.transform.position = Input.mousePosition;
        }
    }

    // Listens for mouse clicks anywhere on this Canvas/Image
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isComplete) return;

        _currentClicks++;

        // Lower dust opacity per click
        if (_dustImage != null)
        {
            float alpha = 1f - ((float)_currentClicks / _clicksRequired);
            Color c = _dustImage.color;
            c.a = Mathf.Clamp01(alpha);
            _dustImage.color = c;
        }

        // Play 1-frame click animation
        StopAllCoroutines();
        StartCoroutine(DusterClickAnimationRoutine());

        // Check completion
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
        obj.sprite = completedSprite;
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