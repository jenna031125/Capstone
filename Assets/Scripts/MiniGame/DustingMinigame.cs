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
    [SerializeField] private Sprite _dustyWorldSprite; // 먼지가 쌓인 월드 스프라이트 (미완료)
    public Sprite completedSprite;                      // 깨끗하게 닦인 월드 스프라이트 (완료)

    private Sprite _originalDustySprite;
    private int _currentClicks = 0;
    private bool _isComplete = false;

    void Awake()
    {
        // 1. _dustyWorldSprite가 할당되어 있으면 직접 지정
        if (_dustyWorldSprite != null)
        {
            _originalDustySprite = _dustyWorldSprite;
        }
        // 2. 할당이 없으면 시작 시점의 sprite 기억
        else if (obj != null)
        {
            _originalDustySprite = obj.sprite;
        }

        // 게임 시작 시 미완료(먼지 쌓인) 스프라이트로 강제 세팅
        if (obj != null && _originalDustySprite != null)
        {
            obj.sprite = _originalDustySprite;
        }
    }

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
            FindFirstObjectByType<OfficeWorldManager>()?.UpdateAllObjectSprites();
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

        // 먼지를 다 털었으므로 깨끗해진 액자 월드 스프라이트로 교체
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