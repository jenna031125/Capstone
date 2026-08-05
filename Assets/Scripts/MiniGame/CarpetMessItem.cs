using UnityEngine;
using UnityEngine.EventSystems;

public class CarpetMessItem : MonoBehaviour, IPointerClickHandler
{
    private CarpetMinigame _manager;

    void Awake()
    {
        _manager = GetComponentInParent<CarpetMinigame>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Hide this mess item
        gameObject.SetActive(false);

        // Inform manager to check remaining mess
        if (_manager != null)
        {
            _manager.CheckMessCleared();
        }
    }
}