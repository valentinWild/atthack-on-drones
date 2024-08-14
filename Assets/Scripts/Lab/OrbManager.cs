using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class CorrectCode
{
    public bool[] code; // 4-bit binary code represented as a boolean array
}

public class OrbManager : MonoBehaviour
{
    [SerializeField] private CorrectCode[] correctCodes; // Array to hold 4 correct code sequences
    [SerializeField] private bool[] orbStates; // Tracks the player's input for the current level

    public GameObject crateObject;
    private Animation crateAnimation;
    public string openAnimationName = "Crate_Open";
    public string closeAnimationName = "Crate_Close";

    private bool isCrateOpen = false; // Tracks whether the crate is currently open

    // UI references to display the codes and hints
    [SerializeField] public TextMeshProUGUI codeDisplayText;
    [SerializeField] public TextMeshProUGUI hintDisplayText;

    [SerializeField] private HintCounter hintCounter; // Reference to the HintCounter

    private void Start()
    {
        correctCodes = new CorrectCode[4];
        orbStates = new bool[4];

        hintCounter = FindObjectOfType<HintCounter>();

        if (hintCounter != null)
        {
            hintCounter.OnHintCounterChanged += UpdateHints;  // Subscribe to the hint counter change event
        }

        GenerateRandomCorrectCodes();

        if (crateObject != null)
        {
            crateAnimation = crateObject.GetComponentInChildren<Animation>();
            if (crateAnimation == null)
            {
                Debug.LogWarning("Animation component not found on the crate object or its children.");
            }
        }
        else
        {
            Debug.LogWarning("Crate object is not assigned.");
        }

        ResetOrbs();
    }

    private void GenerateRandomCorrectCodes()
    {
        List<bool[]> generatedCodes = new();

        for (int i = 0; i < correctCodes.Length; i++)
        {
            correctCodes[i] = new CorrectCode();
            bool[] newCode;

            do
            {
                newCode = GenerateRandomCode();
            } while (IsAllFalse(newCode) || ContainsDuplicateCode(generatedCodes, newCode));

            generatedCodes.Add(newCode);
            correctCodes[i].code = newCode;

            Debug.Log($"Generated Code {i + 1}: {BoolArrayToBinaryString(newCode)} (Decimal: {ConvertCodeToDecimal(newCode)})");
        }

        UpdateCodeDisplay();
    }

    private bool[] GenerateRandomCode()
    {
        bool[] code = new bool[4];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = UnityEngine.Random.value > 0.5f; // Randomly assign true or false
        }
        return code;
    }

    private bool IsAllFalse(bool[] code)
    {
        foreach (bool bit in code)
        {
            if (bit) return false;
        }
        return true;
    }

    private bool ContainsDuplicateCode(List<bool[]> generatedCodes, bool[] newCode)
    {
        foreach (bool[] existingCode in generatedCodes)
        {
            if (AreCodesEqual(existingCode, newCode)) return true;
        }
        return false;
    }

    private bool AreCodesEqual(bool[] code1, bool[] code2)
    {
        for (int i = 0; i < code1.Length; i++)
        {
            if (code1[i] != code2[i]) return false;
        }
        return true;
    }

    public string ConvertCodeToDecimal(bool[] code)
    {
        string binaryString = BoolArrayToBinaryString(code);
        int decimalValue = Convert.ToInt32(binaryString, 2);
        return decimalValue.ToString();
    }

    public string BoolArrayToBinaryString(bool[] code)
    {
        return string.Join("", Array.ConvertAll(code, bit => bit ? "1" : "0"));
    }

    public bool[][] GetCorrectCodes()
    {
        bool[][] codes = new bool[correctCodes.Length][];
        for (int i = 0; i < correctCodes.Length; i++)
        {
            codes[i] = correctCodes[i].code;
        }
        return codes;
    }

    public void UpdateOrbState(int orbIndex, bool isOn)
    {
        if (orbIndex < 0 || orbIndex >= orbStates.Length)
        {
            Debug.LogWarning("Orb index out of bounds.");
            return;
        }

        orbStates[orbIndex] = isOn;

        bool codeIsCorrect = false;
        foreach (var correctCode in correctCodes)
        {
            if (AreCodesEqual(orbStates, correctCode.code))
            {
                codeIsCorrect = true;
                break;
            }
        }

        if (codeIsCorrect && !isCrateOpen)
        {
            OpenCrate();
        }
        else if (!codeIsCorrect && isCrateOpen)
        {
            CloseCrate();
        }
    }

    private void OpenCrate()
    {
        if (crateAnimation != null && !isCrateOpen)
        {
            crateAnimation.Play(openAnimationName);
            isCrateOpen = true;
        }
        else if (crateAnimation == null)
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }

    private void CloseCrate()
    {
        if (crateAnimation != null && isCrateOpen)
        {
            crateAnimation.Play(closeAnimationName);
            isCrateOpen = false;
        }
        else if (crateAnimation == null)
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }

    public void ResetOrbs()
    {
        for (int i = 0; i < orbStates.Length; i++)
        {
            orbStates[i] = false;
        }
    }

    // This method updates the hints displayed based on the current hint counter
    private void UpdateHints(int hintCount)
    {
        if (hintDisplayText != null)
        {
            hintDisplayText.text = "Hints (Decimal):\n";
            for (int i = 0; i < hintCount && i < correctCodes.Length; i++)
            {
                hintDisplayText.text += $"Hint {i + 1}: {ConvertCodeToDecimal(correctCodes[i].code)}\n";
            }
        }
    }

    // This method updates the code display for debugging purposes
    private void UpdateCodeDisplay()
    {
        if (codeDisplayText != null)
        {
            codeDisplayText.text = "Generated Codes:\n";
            for (int i = 0; i < correctCodes.Length; i++)
            {
                codeDisplayText.text += $"Code {i + 1}: {BoolArrayToBinaryString(correctCodes[i].code)}\n";
            }
        }
    }
}
