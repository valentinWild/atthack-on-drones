using UnityEngine;
using TMPro;
using System;

public class RandomCodesDisplay : MonoBehaviour
{
    private OrbManager orbManager;  // Reference to the OrbManager
    private TextMeshProUGUI codeDisplayText;  // Reference to the UI Text element for displaying codes

    private void Start()
    {
        orbManager = FindObjectOfType<OrbManager>();
        codeDisplayText = GetComponent<TextMeshProUGUI>(); // Get the attached TextMeshProUGUI component

        if (orbManager == null)
        {
            Debug.LogWarning("OrbManager reference is missing in CodeDisplay.");
        }

        UpdateDisplay(); // Initial display
    }

    private void Update()
    {
        UpdateDisplay(); // Continuously update the display during runtime
    }

    private void UpdateDisplay()
    {
        if (orbManager != null && codeDisplayText != null)
        {
            codeDisplayText.text = "Generated Codes:\n";
            bool[][] codes = orbManager.GetCorrectCodes();
            for (int i = 0; i < codes.Length; i++)
            {
                string binaryCode = BoolArrayToBinaryString(codes[i]);
                codeDisplayText.text += $"Code {i + 1}: {binaryCode}\n";
            }
        }
    }

    private string BoolArrayToBinaryString(bool[] code)
    {
        // Convert bool array to binary string representation (e.g., [false, true, false, true] -> "0101")
        return string.Join("", Array.ConvertAll(code, bit => bit ? "1" : "0"));
    }
}
