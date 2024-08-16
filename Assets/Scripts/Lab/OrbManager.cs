using System;
using System.Collections;
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
    private int dronesCollected; //Future hintCounter when networking
    private int numberOfDecodedHints;

    private Light[] orbLights; // Array to store references to the orb lights

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
        orbLights = GetComponentsInChildren<Light>();


        numberOfDecodedHints = 0;

        hintDisplayManager = FindObjectOfType<HintDisplayManager>();


        GenerateRandomCorrectCodes();
        ResetOrbs();
        ResetDecodedHints();
    }

    private void OnCollectedHintDronesChanged(int newAmount)
    {
        dronesCollected = newAmount;
        Debug.Log("New Amount of Collect Drones: " + newAmount + "and dronesCollected set to " + dronesCollected);
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
        Debug.Log("incremented number of decoded hints. Hints decoded: " + numberOfDecodedHints);
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
    /* Green Light
    public void UpdateOrbState(int orbIndex, bool isOn)
    {

        Debug.Log("UpdateOrbState function called in OrbManager");
        if (orbIndex < 0 || orbIndex >= orbStates.Length)
        {
            Debug.LogWarning("Orb index out of bounds.");
            return;
        }

        orbStates[orbIndex] = isOn;
        Debug.Log($"Orb {orbIndex} state updated: {isOn}");

        for (int i = 0; i < correctCodes.Length; i++)
        {
            Debug.Log($"Checking if entered code matches {BoolArrayToBinaryString(correctCodes[i].code)}");
            if (AreCodesEqual(orbStates, correctCodes[i].code))
            {
                if (dronesCollected >= i + 1)  // Ensure the hintCounter is high enough
                {
                    Debug.Log($"Entered code is correct and hint counter is sufficient ({dronesCollected} >= {i + 1})!");

                    if (!hintWasDecoded[dronesCollected - 1]) // Ensure Hint hasn't been decoded
                    {
                        hintWasDecoded[dronesCollected - 1] = true;
                        IncrementNumberOfDecodedHints();
                        // Change the color of all orbs to green
                        ChangeOrbLightColors(Color.green);
                        hintDisplayManager.ChangeHintColor(i, Color.black); // Change hint text color to black
                        // Wait a few seconds before resetting the lights
                        StartCoroutine(ResetLightsAfterDelay(3.0f));
                    }
                    else
                    {
                        Debug.Log("Hint Number " + i + 1 + " has already been decoded");
                    }

                    // Check if all hints are decoded
                    if (CheckAllHintsDecoded())
                    {
                        TriggerAllHintsDecodedAction();
                    }
                    //wait a few seconds before turning off all lights and reset the color
                }
                else
                {
                    Debug.Log($"Hint counter is too low to decode hint {i + 1}. Current counter: {dronesCollected}");
                }
                break;
            }
        }
    }
    */

    public void UpdateOrbState(int orbIndex, bool isOn)
    {

        Debug.Log("UpdateOrbState function called in OrbManager");
        if (orbIndex < 0 || orbIndex >= orbStates.Length)
        {
            Debug.LogWarning("Orb index out of bounds.");
            return;
        }

        orbStates[orbIndex] = isOn;
        Debug.Log($"Orb {orbIndex} state updated: {isOn}");

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
                    Debug.Log("Hint Number " + i + "was set to:" + hintWasDecoded[i]);
                    Debug.Log("New state of decoded hints: " + string.Join(", ", hintWasDecoded));

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
    private IEnumerator ResetLightsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the color and state of all orb lights
        foreach (var light in orbLights)
        {
            ToggleLight toggleLight = light.GetComponent<ToggleLight>();
            if (toggleLight != null)
            {
                toggleLight.ResetLightAndParticles();
            }
        }

        // Optionally change the orb light colors back to their original color
        ResetOrbLightColors();
    }

    // Helper method to change the color of all orb lights
    private void ChangeOrbLightColors(Color newColor)
    {
        // Assuming that orbLights contains the references to the parent orb GameObjects (not just the Light components)
        foreach (var orbLight in orbLights)
        {
            // Get the ToggleLight script attached to the orb GameObject
            ToggleLight toggleLight = orbLight.GetComponentInParent<ToggleLight>();
            if (toggleLight != null)
            {
                toggleLight.ChangeLightColor(newColor);  // Use the method inside ToggleLight to change the light color
            }
        }
    }

    private void ResetOrbLightColors()
    {
        // Assuming that orbLights contains the references to the parent orb GameObjects (not just the Light components)
        foreach (var orbLight in orbLights)
        {
            // Get the ToggleLight script attached to the orb GameObject
            ToggleLight toggleLight = orbLight.GetComponentInParent<ToggleLight>();
            if (toggleLight != null)
            {
                toggleLight.ResetToDefaultColor();  // Use the method inside ToggleLight to change the light color
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
        //IncrementLevel();
        // todo: show button
    }

    public void ResetOrbs()
    {
        for (int i = 0; i < orbStates.Length; i++)
        {
            orbStates[i] = false;
        }

        Debug.Log("Reset orb state");
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