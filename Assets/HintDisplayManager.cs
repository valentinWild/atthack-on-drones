using UnityEngine;
using TMPro;

public class HintDisplayManager : MonoBehaviour
{
    public GameObject hint1; // Reference to Hint1
    public GameObject hint2; // Reference to Hint2
    public GameObject hint3; // Reference to Hint3
    public GameObject hint4; // Reference to Hint4

    private OrbManager orbManager; // Reference to the OrbManager
    private TextMeshProUGUI hint1Text;
    private TextMeshProUGUI hint2Text;
    private TextMeshProUGUI hint3Text;
    private TextMeshProUGUI hint4Text;

    private void Start()
    {
        // Find the OrbManager script in the scene
        orbManager = FindObjectOfType<OrbManager>();

        if (orbManager == null)
        {
            Debug.LogError("OrbManager could not be found in the scene.");
            return;
        }

        // Get the TextMeshProUGUI components of the hint GameObjects
        hint1Text = hint1.GetComponent<TextMeshProUGUI>();
        hint2Text = hint2.GetComponent<TextMeshProUGUI>();
        hint3Text = hint3.GetComponent<TextMeshProUGUI>();
        hint4Text = hint4.GetComponent<TextMeshProUGUI>();

        // Initialize the visibility and text of hints
        UpdateHintVisibility(0); // Initially set to 0

        // Subscribe to the hint counter change event
        /*
        HintCounter hintCounterScript = FindObjectOfType<HintCounter>();
        if (hintCounterScript != null)
        {
            hintCounterScript.OnHintCounterChanged += UpdateHintVisibility;
        }
        else
        {
            Debug.LogError("HintCounter could not be found in the scene.");
        }
        */
    }

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnCollectedDronesChanged += UpdateHintVisibility;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnCollectedDronesChanged -= UpdateHintVisibility;
        }
    }

    public void UpdateHintVisibility(int hintCounter)
    {
        Debug.Log($"UpdateHintVisibility called with hintCounter: {hintCounter}");

        // Clamp the hintCounter value to ensure it doesn't exceed 4
        hintCounter = Mathf.Clamp(hintCounter, 0, 4);

        // Set the visibility of the hints based on the hintCounter value
        if (hintCounter >= 1)
        {
            hint1.SetActive(true);
            hint1Text.text = $"Hint 1\n{orbManager.correctCodesDecimal[0]}";
            Debug.Log($"Hint 1 set to: {hint1Text.text}");
        }
        else
        {
            hint1.SetActive(false);
        }

        if (hintCounter >= 2)
        {
            hint2.SetActive(true);
            hint2Text.text = $"Hint 2\n{orbManager.correctCodesDecimal[1]}";
            Debug.Log($"Hint 2 set to: {hint2Text.text}");
        }
        else
        {
            hint2.SetActive(false);
        }

        if (hintCounter >= 3)
        {
            hint3.SetActive(true);
            hint3Text.text = $"Hint 3\n{orbManager.correctCodesDecimal[2]}";
            Debug.Log($"Hint 3 set to: {hint3Text.text}");
        }
        else
        {
            hint3.SetActive(false);
        }

        if (hintCounter == 4)
        {
            hint4.SetActive(true);
            hint4Text.text = $"Hint 4\n{orbManager.correctCodesDecimal[3]}";
            Debug.Log($"Hint 4 set to: {hint4Text.text}");
        }
        else
        {
            hint4.SetActive(false);
        }
    }

    public void ChangeHintColor(int index, Color color)
    {
        switch (index)
        {
            case 0:
                hint1Text.color = color;
                break;
            case 1:
                hint2Text.color = color;
                break;
            case 2:
                hint3Text.color = color;
                break;
            case 3:
                hint4Text.color = color;
                break;
            default:
                Debug.LogWarning("Invalid hint index for color change.");
                break;
        }
    }

}