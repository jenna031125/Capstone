using UnityEngine;
using Yarn.Unity;

public class InteractableGlow : MonoBehaviour
{
    [Header("Target Day")]
    [Tooltip("Which day is this object active on? (e.g., Day 1, Day 2)")]
    [SerializeField] private int _activeDay = 1;

    [Header("Yarn Variable Name")]
    [Tooltip("The exact Yarn variable tracking if this object is cleaned (e.g., $is_pot_watered)")]
    [SerializeField] private string _yarnCleanVariable = "$is_pot_watered";

    [Header("Glow Effect Reference")]
    [SerializeField] private GameObject _glowOutlineObject;

    private InMemoryVariableStorage _variableStorage;

    void Awake()
    {
        _variableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
    }

    void Update()
    {
        if (_variableStorage == null || _glowOutlineObject == null) return;

        // 1. Get current day from Yarn
        float currentDay = 0;
        if (_variableStorage.TryGetValue("$day_count", out float day))
        {
            currentDay = day;
        }

        // 2. Check if this object is already cleaned
        bool isClean = false;
        if (_variableStorage.TryGetValue(_yarnCleanVariable, out bool cleaned))
        {
            isClean = cleaned;
        }

        // 3. Show glow ONLY if it's the correct day AND not cleaned yet
        bool shouldGlow = (Mathf.RoundToInt(currentDay) == _activeDay) && !isClean;

        if (_glowOutlineObject.activeSelf != shouldGlow)
        {
            _glowOutlineObject.SetActive(shouldGlow);
        }
    }
}