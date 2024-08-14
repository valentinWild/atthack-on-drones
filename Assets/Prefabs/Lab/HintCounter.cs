using UnityEngine;
using TMPro;
using System;

public class HintCounter : MonoBehaviour
{
    [SerializeField] private int hintCounter = 0;  // Keeps track of the number of hints
    private TextMeshProUGUI hintCounterText;  // Reference to the UI Text element displaying the hint counter

    // Event to notify when the hint counter changes
    public event Action<int> OnHintCounterChanged;

    private void Start()
    {
        hintCounterText = GetComponent<TextMeshProUGUI>();  // Get the attached TextMeshProUGUI component

        UpdateHintCounterDisplay();  // Initial update of the display
    }

    // Method to increase the hint counter with button in UI (for testing)
    public void IncreaseCounter()
    {
        hintCounter++;
        UpdateHintCounterDisplay();
        OnHintCounterChanged?.Invoke(hintCounter);  // Notify subscribers (like OrbManager)
    }

    // Method to reset the hint counter to zero
    public void ResetCounter()
    {
        hintCounter = 0;
        UpdateHintCounterDisplay();
        OnHintCounterChanged?.Invoke(hintCounter);
    }

    // Method to update the UI text display
    private void UpdateHintCounterDisplay()
    {
        if (hintCounterText != null)
        {
            hintCounterText.text = hintCounter.ToString();
        }
        else
        {
            Debug.LogWarning("Hint counter text component is not assigned.");
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateHintCounterDisplay();
        }
    }

}
