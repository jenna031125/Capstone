using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePaper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _targetStackArea; // Drag StackTargetArea here
    [SerializeField] private float _dropDistanceThreshold = 200f; // Distance sensitivity

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _startPosition;
    private bool _isStacked = false;

    private DeskTidyingMinigame _manager;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _manager = FindFirstObjectByType<DeskTidyingMinigame>(FindObjectsInactive.Include);
    }

    void OnEnable()
    {
        _startPosition = _rectTransform.anchoredPosition;
        _isStacked = false;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isStacked) return;

        // Bring paper to the front layer while dragging so it passes over other papers
        _rectTransform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isStacked) return;
        _rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isStacked) return;
        _canvasGroup.blocksRaycasts = true;

        if (_targetStackArea != null)
        {
            float distance = Vector2.Distance(_rectTransform.position, _targetStackArea.position);

            if (distance <= _dropDistanceThreshold)
            {
                // Successful Drop! Align neatly to stack position
                SnapToStack();
                return;
            }
        }

        // Return to messy start spot if dropped outside target zone
        _rectTransform.anchoredPosition = _startPosition;
    }

    private void SnapToStack()
    {
        _isStacked = true;
        _canvasGroup.blocksRaycasts = false; // Disable dragging once stacked

        // Snap position directly to stack area with a tiny offset so papers look stacked neatly
        int stackIndex = _manager != null ? _manager.StackedCount : 0;
        Vector2 stackOffset = new Vector2(stackIndex * 2f, stackIndex * -2f); // Slight offset for stack effect

        _rectTransform.position = _targetStackArea.position + (Vector3)stackOffset;
        _rectTransform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to align straight

        if (_manager != null)
        {
            _manager.OnPaperStacked();
        }
    }

    public void ResetPaper()
    {
        _isStacked = false;
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }

        // Return paper to its initial unstacked position
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = _startPosition;
        }
    }
}