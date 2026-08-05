using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class CarpetMinigame : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image _carpetImage;
    [SerializeField] private GameObject _messContainer; // Parent object holding all 5 mess items
    [SerializeField] private List<GameObject> _messItems;

    [Header("Carpet Sprites")]
    [SerializeField] private Sprite _carpetUnflippedSprite;
    [SerializeField] private Sprite _carpetFlippedSprite;

    [Header("Settings")]
    [SerializeField] private float _delayBeforeClose = 1.0f;

    private bool _isFlipped = false;
    private bool _allMessCleared = false;
    private bool _isComplete = false;

    void OnEnable()
    {
        _isFlipped = false;
        _allMessCleared = false;
        _isComplete = false;

        // Reset Carpet to Unflipped
        if (_carpetImage != null && _carpetUnflippedSprite != null)
        {
            _carpetImage.sprite = _carpetUnflippedSprite;
        }

        // Hide mess items initially
        if (_messContainer != null)
        {
            _messContainer.SetActive(false);
        }

        // Re-enable all mess items for a fresh run
        foreach (var item in _messItems)
        {
            if (item != null) item.SetActive(true);
        }
    }

    // Handles clicking the carpet itself
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isComplete) return;

        // Phase 1: Flip carpet open to reveal mess
        if (!_isFlipped)
        {
            _isFlipped = true;
            if (_carpetImage != null && _carpetFlippedSprite != null)
            {
                _carpetImage.sprite = _carpetFlippedSprite;
            }

            // Disable raycasts on carpet so clicks pass through to the mess below
            CanvasGroup group = _carpetImage.GetComponent<CanvasGroup>();
            if (group != null) group.blocksRaycasts = false;

            if (_messContainer != null)
            {
                _messContainer.SetActive(true);
            }
            return;
        }
    

        // Phase 3: Unflip carpet after mess is completely cleaned
        if (_isFlipped && _allMessCleared)
        {
            _isFlipped = false;
            if (_carpetImage != null && _carpetUnflippedSprite != null)
            {
                _carpetImage.sprite = _carpetUnflippedSprite;
            }

            CompleteMinigame();
        }
    }

    public void CheckMessCleared()
    {
        foreach (var item in _messItems)
        {
            // If any item is still active, mess is not fully cleared
            if (item != null && item.activeSelf)
            {
                return;
            }
        }

        // All items are gone!
        _allMessCleared = true;
    }

    private void CompleteMinigame()
    {
        _isComplete = true;
        StartCoroutine(WaitAndCloseRoutine());
    }

    private IEnumerator WaitAndCloseRoutine()
    {
        yield return new WaitForSeconds(_delayBeforeClose);
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMAND ---
    [YarnCommand("start_carpet_minigame")]
    public static void StartCarpetCommand(string location = "")
    {
        CarpetMinigame[] games = FindObjectsByType<CarpetMinigame>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var game in games)
        {
            if (string.IsNullOrEmpty(location) || game.gameObject.name.ToLower().Contains(location.ToLower()))
            {
                game.gameObject.SetActive(true);
                return;
            }
        }
    }
}