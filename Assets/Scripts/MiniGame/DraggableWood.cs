using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _dropTarget; // Assign the DropTargetArea here
    [SerializeField] private float _dropDistanceThreshold = 150f; // Max pixel distance to register a drop

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
        // Remember starting position to reset if missed
        _startPosition = _rectTransform.anchoredPosition;
        _isPlaced = false;
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _canvasGroup.blocksRaycasts = false; // Lets mouse see target underneath
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _rectTransform.position = Input.mousePosition; // Move with mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isPlaced) return;
        _canvasGroup.blocksRaycasts = true;

        // Check distance between wood drop location and target area
        if (_dropTarget != null)
        {
            float distance = Vector2.Distance(_rectTransform.position, _dropTarget.position);
            if (distance <= _dropDistanceThreshold)
            {
                // Successful Drop!
                _isPlaced = true;
                gameObject.SetActive(false); // Hide log from bottom right

                if (_manager != null)
                {
                    _manager.OnWoodPlaced();
                }
                return;
            }
        }

        // Return to start position if missed
        _rectTransform.anchoredPosition = _startPosition;
    }
}