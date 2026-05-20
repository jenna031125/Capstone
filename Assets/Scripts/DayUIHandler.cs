using UnityEngine;
using TMPro;
using Yarn.Unity;

public class DayUIHandler : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private InMemoryVariableStorage _variableStorage;

    void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _variableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
    }

    void Update()
    {
        if (_variableStorage != null && _textMeshPro != null)
        {
            // Default fallback if Yarn hasn't processed the variable yet
            float currentDay = 1;

            // Safely ask Yarn's dictionary for the value of $day_count
            if (_variableStorage.TryGetValue("$day_count", out float yarnDay))
            {
                currentDay = yarnDay;
            }

            // Push the result directly to your TextMeshPro component
            _textMeshPro.text = "Day " + currentDay;
        }
    }
}