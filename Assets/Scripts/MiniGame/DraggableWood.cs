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
        _startPosition = _rectTransform.anchoredPosition;
        _isPlaced = false;
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;
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
        _canvasGroup.blocksRaycasts = true;

        if (_dropTarget != null)
        {
            float distance = Vector2.Distance(_rectTransform.position, _dropTarget.position);
            if (distance <= _dropDistanceThreshold)
            {
                _isPlaced = true;
                gameObject.SetActive(false);

                if (_manager != null)
                {
                    _manager.OnWoodPlaced();
                }
                return;
            }
        }

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
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = _startPosition;
        }
    }
}