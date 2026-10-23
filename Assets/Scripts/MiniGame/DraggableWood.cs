using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _dropTarget;
    [SerializeField] private float _dropDistanceThreshold = 150f;

    private Vector3 _startPosition;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private bool _isPlaced = false;

    private FireplaceMinigame _manager;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _manager = FindFirstObjectByType<FireplaceMinigame>(FindObjectsInactive.Include);
    }

    void OnEnable()
    {
        // 초기 시작 위치 기억 (Reset용)
        if (_startPosition == Vector3.zero)
        {
            _startPosition = _rectTransform.anchoredPosition;
        }
        ResetWood();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;

        if (_dropTarget != null)
        {
            float distance = Vector2.Distance(_rectTransform.position, _dropTarget.position);
            if (distance <= _dropDistanceThreshold)
            {
                // 성공적 드롭: 위치를 타겟 위치(벽난로 안)로 이동하고 고정
                _isPlaced = true;
                _rectTransform.position = _dropTarget.position;

                // 더 이상 드래그하거나 클릭되지 않도록 조치
                _canvasGroup.blocksRaycasts = false;

                if (_manager != null)
                {
                    _manager.OnWoodPlaced(this);
                }
                return;
            }
        }

        // 실패 시 제자리 복귀
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.anchoredPosition = _startPosition;
    }

    // --- RESET METHOD ---
    public void ResetWood()
    {
        _isPlaced = false;
        gameObject.SetActive(true);
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }
        if (_rectTransform != null && _startPosition != Vector3.zero)
        {
            _rectTransform.anchoredPosition = _startPosition;
        }
    }
}