using UnityEngine;
using TMPro;

public class CodeDisplay : MonoBehaviour
{
    private OrbManager orbManager;
    private TextMeshProUGUI codeDisplayText;

    private void Start()
    {
        codeDisplayText = GetComponent<TextMeshProUGUI>();
        orbManager = FindObjectOfType<OrbManager>();
        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (orbManager != null && codeDisplayText != null)
        {
            codeDisplayText.text = "Generated Codes:\n";
            bool[][] codes = orbManager.GetCorrectCodes();
            for (int i = 0; i < codes.Length; i++)
            {
                string binaryCode = orbManager.BoolArrayToBinaryString(codes[i]);
                codeDisplayText.text += $"Code {i + 1}: {binaryCode}\n";
            }
        }
    }
}
