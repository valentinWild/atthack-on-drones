using UnityEngine;
using TMPro;

public class HintManager : MonoBehaviour
{
    public GameObject hint1; // Reference to Hint1
    public GameObject hint2; // Reference to Hint2
    public GameObject hint3; // Reference to Hint3
    public GameObject hint4; // Reference to Hint4

    private HintCounter hintCounterScript; // Reference to the HintCounter script

    private void Start()
    {
        // Find the HintCounter script in the scene
        hintCounterScript = FindObjectOfType<HintCounter>();

        if (hintCounterScript != null)
        {
            hintCounterScript.OnHintCounterChanged += UpdateHintVisibility;
        }

        // Initialize the visibility of hints
        UpdateHintVisibility(0); // Initially set to 0
    }

    private void UpdateHintVisibility(int hintCounter)
    {
        // Clamp the hintCounter value to ensure it doesn't exceed 4
        hintCounter = Mathf.Clamp(hintCounter, 0, 4);

        // Set the visibility of the hints based on the hintCounter value
        hint1.SetActive(hintCounter >= 1);
        hint2.SetActive(hintCounter >= 2);
        hint3.SetActive(hintCounter >= 3);
        hint4.SetActive(hintCounter == 4);
    }
}