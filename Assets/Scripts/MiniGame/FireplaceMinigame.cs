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

    private int _woodPlacedCount = 0;
    private bool _isLit = false;

    void OnEnable()
    {
        // Reset state
        _woodPlacedCount = 0;
        _isLit = false;

        if (_fireplaceImage != null && _emptyFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _emptyFireplaceSprite;
        }

        // Hide light fire button until logs are placed
        if (_lightFireButton != null)
        {
            _lightFireButton.gameObject.SetActive(false);
            _lightFireButton.onClick.RemoveAllListeners();
            _lightFireButton.onClick.AddListener(OnFireplaceClicked);
        }

        // Enable wood items
        foreach (var wood in _woodObjects)
        {
            if (wood != null) wood.SetActive(true);
        }
    }

    public void OnWoodPlaced()
    {
        _woodPlacedCount++;

        // All 3 woods placed! Enable click interaction on fireplace
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

        // Swap to fire sprite!
        if (_fireplaceImage != null && _litFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _litFireplaceSprite;
        }

        // Disable button click after lighting
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
    }

    public void CloseMinigame()
    {
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMAND ---
    [YarnCommand("start_fireplace_minigame")]
    public static void StartFireplaceCommand()
    {
        FireplaceMinigame minigame = FindFirstObjectByType<FireplaceMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }
}