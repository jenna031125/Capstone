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

    [Header("Watering Can Sprites")]
    [SerializeField] private Sprite _defaultWateringCanSprite; // 클릭하지 않을 때 (기본)
    [SerializeField] private Sprite _wateringCanSprite;       // 클릭하여 물을 줄 때 (물 나오는 상태)

    [Header("Settings")]
    [SerializeField] private float _waterTimeRequired = 3.0f; // Seconds needed to water
    [SerializeField] private float _delayBeforeClose = 2.0f;  // Seconds to display healthy flower before closing

    private float _currentWaterTimer = 0f;
    private bool _isComplete = false;
    private bool _isHolding = false;

    [Header("World Object")]
    public SpriteRenderer obj;
    [SerializeField] private Sprite _dryWorldSprite; // 물주기 전 월드 스프라이트 (미완료 상태)
    public Sprite completedSprite;                   // 물준 후 월드 스프라이트 (완료 상태)

    void Awake()
    {
        // 씬 시작 시 월드 오브젝트 스프라이트를 미완료(Dirty/Dry) 상태로 강제 설정
        if (obj != null && _dryWorldSprite != null)
        {
            obj.sprite = _dryWorldSprite;
        }
    }

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

        // 물뿌리개 초기 상태 설정 (기본 스프라이트)
        if (_wateringCanImage != null)
        {
            _wateringCanImage.gameObject.SetActive(true);
            if (_defaultWateringCanSprite != null)
            {
                _wateringCanImage.sprite = _defaultWateringCanSprite;
            }
        }
    }

    void Update()
    {
        if (_isComplete) return;

        // Check if player is holding click over the screen
        if (Input.GetMouseButtonDown(0))
        {
            _isHolding = true;

            // 클릭을 시작했을 때 물이 나오는 스프라이트로 변경
            if (_wateringCanImage != null && _wateringCanSprite != null)
            {
                _wateringCanImage.sprite = _wateringCanSprite;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isHolding = false;

            // 클릭을 뗐을 때 기본 물뿌리개 스프라이트로 복원
            if (_wateringCanImage != null && _defaultWateringCanSprite != null)
            {
                _wateringCanImage.sprite = _defaultWateringCanSprite;
            }
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
                FindFirstObjectByType<OfficeWorldManager>()?.UpdateAllObjectSprites();
            }
        }
    }

    private void CompleteMinigame()
    {
        _isComplete = true;

        // 미니게임 완료 시 월드 스프라이트를 완료 상태로 교체
        if (obj != null && completedSprite != null)
        {
            obj.sprite = completedSprite;
        }
        _isHolding = false;

        // 1. Swap to healthy flower sprite immediately
        if (_flowerPotImage != null && _healthyPotSprite != null)
        {
            _flowerPotImage.sprite = _healthyPotSprite;
        }

        // 2. 물뿌리개를 기본 상태로 되돌리기
        if (_wateringCanImage != null)
        {
            if (_defaultWateringCanSprite != null)
            {
                _wateringCanImage.sprite = _defaultWateringCanSprite;
            }
        }

        // 3. Start waiting before closing
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

    // --- YARN SPINNER COMMANDS ---
    [YarnCommand("start_watering_minigame")]
    public static void StartMinigameCommand()
    {
        FlowerPotMinigame minigame = FindFirstObjectByType<FlowerPotMinigame>(FindObjectsInactive.Include);
        if (minigame != null)
        {
            minigame.gameObject.SetActive(true);
        }
    }

    // 리셋 시 원래의 시든/미완료 스프라이트로 되돌리기
    public void ResetMinigameState()
    {
        _isComplete = false;
        if (obj != null && _dryWorldSprite != null)
        {
            obj.sprite = _dryWorldSprite;
        }
    }
}