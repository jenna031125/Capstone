using UnityEngine;
using TMPro; // Needed for TextMeshPro
using Yarn.Unity; // Needed to talk to Yarn Spinner

public class DayUIHandler : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private InMemoryVariableStorage _variableStorage;

    void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();

        // Find Yarn Spinner's variable brain in your scene
        _variableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
    }

    void Update()
    {
        if (_variableStorage != null && _textMeshPro != null)
        {
            // 1. Try to read the $day_count variable from Yarn Spinner
            // If it doesn't exist yet, it defaults to 1
            float currentDay = 1;

            if (_variableStorage.TryGetValue("$day_count", out float yarnDay))
            {
                currentDay = yarnDay;
            }

            // 2. Update the text on your screen
            _textMeshPro.text = "Day " + currentDay;
        }
    }
}