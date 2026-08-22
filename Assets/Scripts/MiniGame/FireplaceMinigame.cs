using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FireplaceMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _fireplaceImage;
    [SerializeField] private Sprite _emptyFireplaceSprite;
    [SerializeField] private Sprite _litFireplaceSprite;
    [SerializeField] private Button _lightFireButton;
    [SerializeField] private GameObject[] _woodObjects;

    [Header("Settings")]
    [SerializeField] private float _delayBeforeClose = 2.0f;

    [Header("World Object")]
    public SpriteRenderer obj;
    public Sprite completedSprite;

    private Sprite _originalColdSprite; // Remembers cold fireplace sprite
    private int _woodPlacedCount = 0;
    private bool _isLit = false;

    void Awake()
    {
        if (obj != null)
        {
            _originalColdSprite = obj.sprite;
        }
    }

    void OnEnable()
    {
        _woodPlacedCount = 0;
        _isLit = false;

        if (_fireplaceImage != null && _emptyFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _emptyFireplaceSprite;
        }

        if (_lightFireButton != null)
        {
            _lightFireButton.gameObject.SetActive(false);
            _lightFireButton.interactable = true;
            _lightFireButton.onClick.RemoveAllListeners();
            _lightFireButton.onClick.AddListener(OnFireplaceClicked);
        }

        foreach (var wood in _woodObjects)
        {
            if (wood != null) wood.SetActive(true);
        }
    }

    public void OnWoodPlaced()
    {
        _woodPlacedCount++;

        if (_woodPlacedCount >= 3)
        {
            if (_lightFireButton != null)
            {
                _lightFireButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnFireplaceClicked()
    {
        if (_isLit || _woodPlacedCount < 3) return;

        _isLit = true;

        if (_fireplaceImage != null && _litFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _litFireplaceSprite;
        }

        if (_lightFireButton != null)
        {
            _lightFireButton.interactable = false;
        }

        StartCoroutine(WaitAndCloseRoutine());
    }

    private IEnumerator WaitAndCloseRoutine()
    {
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
    [YarnCommand("start_fireplace_minigame")]
    public static void StartFireplaceCommand()
    {
        FireplaceMinigame minigame = FindFirstObjectByType<FireplaceMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }

    [YarnCommand("reset_fireplace_minigame")]
    public static void ResetFireplaceCommand()
    {
        FireplaceMinigame minigame = FindFirstObjectByType<FireplaceMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.ResetMinigameState();
        }
    }

    public void ResetMinigameState()
    {
        _woodPlacedCount = 0;
        _isLit = false;

        // 1. Revert world object back to cold/unlit sprite
        if (obj != null && _originalColdSprite != null)
        {
            obj.sprite = _originalColdSprite;
        }

        // 2. Reset UI button
        if (_lightFireButton != null)
        {
            _lightFireButton.gameObject.SetActive(false);
            _lightFireButton.interactable = true;
        }

        // 3. Reset draggable wood items
        DraggableWood[] woods = GetComponentsInChildren<DraggableWood>(true);
        foreach (DraggableWood wood in woods)
        {
            wood.ResetWood();
        }
    }
}