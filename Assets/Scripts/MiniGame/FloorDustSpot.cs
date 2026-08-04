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
        _manager = FindFirstObjectByType<FloorCleaningMinigame>(FindObjectsInactive.Include);
    }

    void OnEnable()
    {
        _clickCount = 0;
        _isClean = false;
        gameObject.SetActive(true);

        // Reset alpha opacity back to 100%
        if (_image != null)
        {
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClean) return;

        _clickCount++;

        // Play broom sweep animation
        if (_manager != null)
        {
            _manager.TriggerBroomSweepAnimation();
        }

        // Fade opacity per click
        if (_image != null)
        {
            float alpha = 1f - ((float)_clickCount / _clicksToClean);
            Color c = _image.color;
            c.a = Mathf.Clamp01(alpha);
            _image.color = c;
        }

        // Disable once fully cleaned
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