using UnityEngine;
using TMPro;

public class HintDisplay : MonoBehaviour
{
    [SerializeField] private OrbManager orbManager;
    private TextMeshProUGUI hintDisplayText;

    private void Start()
    {
        hintDisplayText = GetComponent<TextMeshProUGUI>();

        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (orbManager != null && hintDisplayText != null)
        {
            hintDisplayText.text = "Hints (Decimal):\n";
            bool[][] codes = orbManager.GetCorrectCodes();
            for (int i = 0; i < codes.Length; i++)
            {
                string decimalHint = orbManager.ConvertCodeToDecimal(codes[i]);
                hintDisplayText.text += $"Hint {i + 1}: {decimalHint}\n";
            }
        }
    }
}