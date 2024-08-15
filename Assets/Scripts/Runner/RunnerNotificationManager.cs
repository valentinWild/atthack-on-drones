using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RunnerNotificationController : MonoBehaviour
{

    private TextMeshProUGUI messageField;

    private void OnEnable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        string message = potionType + " activated";
        ShowTextFor3Seconds(message);
    }

    // Start is called before the first frame update
    void Start()
    {
        messageField = GetComponent<TextMeshProUGUI>();
    }

    // Call this function to start the process
    public void ShowTextFor3Seconds(string message)
    {
        messageField.text = message;
        SetTextVisibility(true); // Make text visible
        StartCoroutine(HideTextAfterDelay(3f)); // Start coroutine to hide after 3 seconds
    }

    // Coroutine to hide the text after a delay
    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTextVisibility(false); // Hide the text
    }

    // Helper function to set text visibility
    private void SetTextVisibility(bool isVisible)
    {
        Color color = messageField.color;
        color.a = isVisible ? 1f : 0f; // Set alpha to 1 for visible, 0 for invisible
        messageField.color = color;
    }
}