using System.Collections; // Needed for Coroutines
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FlowerPotMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _flowerPotImage;
    [SerializeField] private Image _wateringCanImage;
    [SerializeField] private Sprite _dryPotSprite;
    [SerializeField] private Sprite _healthyPotSprite;

    [Header("Settings")]
    [SerializeField] private float _waterTimeRequired = 3.0f; // Seconds needed to water
    [SerializeField] private float _delayBeforeClose = 2.0f;  // Seconds to display healthy flower before closing

    private float _currentWaterTimer = 0f;
    private bool _isComplete = false;
    private bool _isHolding = false;

    void OnEnable()
    {
        // Reset state every time minigame opens
        _currentWaterTimer = 0f;
        _isComplete = false;
        _isHolding = false;
        if (_flowerPotImage != null && _dryPotSprite != null)
        {
            _flowerPotImage.sprite = _dryPotSprite;
        }
    }

    void Update()
    {
        if (_isComplete) return;

        // Check if player is holding click over the screen
        if (Input.GetMouseButtonDown(0))
        {
            _isHolding = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isHolding = false;
        }

        if (_isHolding)
        {
            _currentWaterTimer += Time.deltaTime;

            // Move watering can to follow mouse while holding
            if (_wateringCanImage != null)
            {
                _wateringCanImage.transform.position = Input.mousePosition;
            }

            // Check completion condition
            if (_currentWaterTimer >= _waterTimeRequired)
            {
                CompleteMinigame();
            }
        }
    }

    private void CompleteMinigame()
    {
        _isComplete = true;
        _isHolding = false;

        // 1. Swap to healthy flower sprite immediately
        if (_flowerPotImage != null && _healthyPotSprite != null)
        {
            _flowerPotImage.sprite = _healthyPotSprite;
        }

        // 2. Start waiting before closing
        StartCoroutine(WaitAndCloseRoutine());
    }

    private IEnumerator WaitAndCloseRoutine()
    {
        // Pause execution for the set duration so the player can see the healthy flower
        yield return new WaitForSeconds(_delayBeforeClose);

        CloseMinigame();
    }

    public void CloseMinigame()
    {
        gameObject.SetActive(false);
    }

    // --- YARN SPINNER COMMAND ---
    [YarnCommand("start_watering_minigame")]
    public static void StartMinigameCommand()
    {
        FlowerPotMinigame minigame = FindFirstObjectByType<FlowerPotMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }
}