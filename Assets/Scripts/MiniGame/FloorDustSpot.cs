using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FloorDustSpot : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private int _clicksToClean = 6;

    private Image _image;
    private int _clickCount = 0;
    private bool _isClean = false;
    public bool IsClean => _isClean;

    private FloorCleaningMinigame _manager;

    void Awake()
    {
        _image = GetComponent<Image>();
        // Fetches the specific active manager holding this dust spot
        _manager = GetComponentInParent<FloorCleaningMinigame>();
    }

    void OnEnable()
    {
        _clickCount = 0;
        _isClean = false;
        gameObject.SetActive(true);

        if (_image != null)
        {
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;
        }

        // Re-check manager reference when canvas enables
        if (_manager == null)
        {
            _manager = GetComponentInParent<FloorCleaningMinigame>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClean) return;

        _clickCount++;

        if (_manager != null)
        {
            _manager.TriggerBroomSweepAnimation();
        }

        if (_image != null)
        {
            float alpha = 1f - ((float)_clickCount / _clicksToClean);
            Color c = _image.color;
            c.a = Mathf.Clamp01(alpha);
            _image.color = c;
        }

        if (_clickCount >= _clicksToClean)
        {
            _isClean = true;
            gameObject.SetActive(false);

            if (_manager != null)
            {
                _manager.CheckMinigameCompletion();
            }
        }
    }
}