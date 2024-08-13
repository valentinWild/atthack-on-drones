using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CorrectCode
{
    public bool[] code;
}


public class OrbManager : MonoBehaviour
{
    [SerializeField] private CorrectCode[] correctCodes; // Array to hold 4 correct code sequences visible in the Inspector
    private bool[] orbStates;
    public GameObject[] orbs;

    public GameObject crateObject;
    private Animation crateAnimation;
    public string openAnimationName = "Crate_Open";
    public string closeAnimationName = "Crate_Close";

    private bool isCrateOpen = false; // Tracks whether the crate is currently open

    private void Start()
    {
        correctCodes = new CorrectCode[4];
        orbStates = new bool[4];

        // Generate 4 random correct codes
        GenerateRandomCorrectCodes();

        // Get Animation component from the crate object
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
        for (int i = 0; i < correctCodes.Length; i++)
        {
            correctCodes[i] = new CorrectCode();
            correctCodes[i].code = GenerateRandomCode();
        }
    }

    private bool[] GenerateRandomCode()
    {
        bool[] code;
        do
        {
            code = new bool[4];
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = UnityEngine.Random.value > 0.5f; // Randomly assign true or false
            }
        } while (IsAllFalse(code)); // Ensure the code is not all false

        return code;
    }

    private bool IsAllFalse(bool[] code)
    {
        // Check if all elements in the code are false (i.e., 0000)
        for (int i = 0; i < code.Length; i++)
        {
            if (code[i])
            {
                return false; // If any value is true, return false
            }
        }
        return true; // All values were false
    }

    public void UpdateOrbState(int orbIndex, bool isOn)
    {
        if (orbIndex < 0 || orbIndex >= orbStates.Length)
        {
            Debug.LogWarning("Orb index out of bounds.");
            return;
        }

        orbStates[orbIndex] = isOn;
        CheckCode();
    }

    private void CheckCode()
    {
        foreach (var correctCode in correctCodes)
        {
            bool isCorrect = true;
            for (int i = 0; i < orbStates.Length; i++)
            {
                if (orbStates[i] != correctCode.code[i])
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                Debug.Log("Correct code entered! Triggering the action.");
                TriggerSuccessAction();
                return; // Exit after the first correct match
            }
        }

        // If none of the codes matched, close the crate (only if it's currently open)
        if (isCrateOpen)
        {
            Debug.Log("Incorrect code entered. Closing the crate.");
            CloseCrate();
        }
    }

    private void TriggerSuccessAction()
    {
        if (!isCrateOpen && crateAnimation != null)
        {
            crateAnimation.Play(openAnimationName); // Play open animation on the crate
            Debug.Log("Crate is opening.");
            isCrateOpen = true; // Mark the crate as open
        }
        else if (crateAnimation == null)
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }

    public void CloseCrate()
    {
        if (isCrateOpen && crateAnimation != null)
        {
            crateAnimation.Play(closeAnimationName); // Play close animation on the crate
            Debug.Log("Crate is closing.");
            isCrateOpen = false; // Mark the crate as closed
        }
        else if (crateAnimation == null)
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }

    public void ResetOrbs()
    {
        for (int i = 0; i < orbs.Length; i++)
        {
            var orb = orbs[i];
            if (orb != null)
            {
                var lightOrb = orb.GetComponent<Light>();
                var orbParticles = orb.GetComponent<ParticleSystem>();

                if (lightOrb != null)
                {
                    lightOrb.enabled = false;
                }

                if (orbParticles != null)
                {
                    orbParticles.Stop();
                }

                orbStates[i] = false; // Reset the orb state here as well
            }
        }
    }
}
