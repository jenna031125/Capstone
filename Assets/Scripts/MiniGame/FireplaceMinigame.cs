using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Sprite _coldWorldSprite; // 불이 꺼진 차가운 상태의 월드 스프라이트 (미완료)
    public Sprite completedSprite;                    // 불이 켜진 상태의 월드 스프라이트 (완료)

    private Sprite _originalColdSprite;
    private int _woodPlacedCount = 0;
    private bool _isLit = false;

    // 배치된 장작들을 추적하기 위한 리스트
    private List<DraggableWood> _placedWoodList = new List<DraggableWood>();

    void Awake()
    {
        // 1. _coldWorldSprite가 직접 할당되어 있다면 명시적으로 사용
        if (_coldWorldSprite != null)
        {
            _originalColdSprite = _coldWorldSprite;
        }
        // 2. 할당이 없다면 시작 시점의 sprite를 기록
        else if (obj != null)
        {
            _originalColdSprite = obj.sprite;
        }

        // 게임 시작 시 미완료(차가운) 스프라이트로 강제 세팅
        if (obj != null && _originalColdSprite != null)
        {
            obj.sprite = _originalColdSprite;
        }
    }

    void OnEnable()
    {
        ResetMinigameState();
    }

    public void OnWoodPlaced(DraggableWood wood)
    {
        _woodPlacedCount++;

        if (!_placedWoodList.Contains(wood))
        {
            _placedWoodList.Add(wood);
        }

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

        // 1. 불이 켜진 UI 스프라이트로 변경
        if (_fireplaceImage != null && _litFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _litFireplaceSprite;
        }

        // 2. 불을 붙였으므로 드롭된 장작 Sprite들 모두 숨기기
        foreach (var wood in _placedWoodList)
        {
            if (wood != null)
            {
                wood.gameObject.SetActive(false);
            }
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
        FindFirstObjectByType<OfficeWorldManager>()?.UpdateAllObjectSprites();

        // 불이 다 켜지면 월드 스프라이트도 따뜻하게 타오르는 완료 스프라이트로 교체
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
        _placedWoodList.Clear();

        if (_fireplaceImage != null && _emptyFireplaceSprite != null)
        {
            _fireplaceImage.sprite = _emptyFireplaceSprite;
        }

        // 월드 오브젝트 원위치(차가운 벽난로)로 복원
        if (obj != null && _originalColdSprite != null)
        {
            obj.sprite = _originalColdSprite;
        }

        if (_lightFireButton != null)
        {
            _lightFireButton.gameObject.SetActive(false);
            _lightFireButton.interactable = true;
            _lightFireButton.onClick.RemoveAllListeners();
            _lightFireButton.onClick.AddListener(OnFireplaceClicked);
        }

        DraggableWood[] woods = GetComponentsInChildren<DraggableWood>(true);
        foreach (DraggableWood wood in woods)
        {
            wood.ResetWood();
        }
    }
}