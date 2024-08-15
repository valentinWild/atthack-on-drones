using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CorrectCode
{
    public bool[] code; // 4-bit binary code represented as a boolean array
}


public class OrbManager : MonoBehaviour
{
    [SerializeField] private CorrectCode[] correctCodes; // Array to hold 4 correct code sequences
    [SerializeField] public String[] correctCodesDecimal; // Array to hold 4 hints
    [SerializeField] public bool[] hintWasDecoded; // Array to hold state of decoding
    [SerializeField] private bool[] orbStates;

    // UI references to display the codes and hints for testing
    [SerializeField] public TextMeshProUGUI codeDisplayText;
    [SerializeField] public TextMeshProUGUI hintDisplayText;

    private HintDisplayManager hintDisplayManager;
    //private HintCounter hintCounter; // Reference to the HintCounter
    private int numberOfDecodedHints;
    private int dronesCollected; //Future hintCounter when networking

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnCollectedDronesChanged += OnCollectedHintDronesChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnCollectedDronesChanged -= OnCollectedHintDronesChanged;
        }
    }

    private void Start()
    {
        correctCodes = new CorrectCode[4];
        correctCodesDecimal = new String[4];
        hintWasDecoded = new bool[4];
        orbStates = new bool[4];

        numberOfDecodedHints = 0;

        hintDisplayManager = FindObjectOfType<HintDisplayManager>();

        /*
        hintCounter = FindObjectOfType<HintCounter>();

        if (hintCounter != null)
        {
            hintCounter.OnHintCounterChanged += UpdateHints;  // Subscribe to the hint counter change event
        }*/

        GenerateRandomCorrectCodes();
        ResetOrbs();
        ResetDecodedHints();
    }

    private void OnCollectedHintDronesChanged(int newAmount)
    {
        dronesCollected = newAmount;
        Debug.Log("New Amount of Collect Drones: " + newAmount);
    }

    private void ResetDecodedHints()
    {
        for (int i = 0; i < hintWasDecoded.Length; i++)
        {
            hintWasDecoded[i] = false;
        }

    }

    private void IncrementNumberOfDecodedHints()
    {
        numberOfDecodedHints++;
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
            correctCodesDecimal[i] = ConvertCodeToDecimal(newCode);

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
        Debug.Log($"Orb {orbIndex} state updated: {isOn}");

        // Get the current hint counter value
        //int currentHintCounter = hintCounter != null ? hintCounter.HintCounterValue : 0;

        for (int i = 0; i < correctCodes.Length; i++)
        {
            Debug.Log($"Checking if entered code matches {BoolArrayToBinaryString(correctCodes[i].code)}");
            if (AreCodesEqual(orbStates, correctCodes[i].code))
            {
                if (dronesCollected >= i + 1)  // Ensure the hintCounter is high enough
                {
                    Debug.Log($"Entered code is correct and hint counter is sufficient ({dronesCollected} >= {i + 1})!");
                    hintDisplayManager.ChangeHintColor(i, Color.black); // Change hint text color to black
                    hintWasDecoded[i] = true;

                    // Check if all hints are decoded
                    if (CheckAllHintsDecoded())
                    {
                        TriggerAllHintsDecodedAction();
                    }
                }
                else
                {
                    Debug.Log($"Hint counter is too low to decode hint {i + 1}. Current counter: {dronesCollected}");
                }
                break;
            }
        }
    }

    private bool CheckAllHintsDecoded()
    {
        foreach (bool decoded in hintWasDecoded)
        {
            if (!decoded)
            {
                return false;
            }
        }
        return true;
    }

    private void TriggerAllHintsDecodedAction()
    {
        Debug.Log("All hints decoded!");
        IncrementLevel();
    }

    private void IncrementLevel()
    {
        Debug.Log("Incremented level. Current Level: ");
        //todo: call increment level function in gameSyncManager
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

    public void SetHintCounter(int newValue)
    {
        dronesCollected = newValue;
    }
}