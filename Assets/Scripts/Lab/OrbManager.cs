using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    public bool[] correctCode; // correct code sequence
    private bool[] orbStates;

    public GameObject crateObject;
    private Animation crateAnimation;
    public string openAnimationName = "Crate_Open";
    public string closeAnimationName = "Crate_Close";

    private void Start()
    {
        orbStates = new bool[correctCode.Length];

        // Get the Animation component from the crate object
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
    }

    public void UpdateOrbState(int orbIndex, bool isOn)
    {
        orbStates[orbIndex] = isOn;
        CheckCode();
    }

    private void CheckCode()
    {
        for (int i = 0; i < orbStates.Length; i++)
        {
            if (orbStates[i] != correctCode[i])
            {
                return; // Early return if the code doesn't match
            }
        }

        // If we get here, the code matches
        Debug.Log("Correct code entered! Triggering the action.");
        TriggerSuccessAction();
    }

    private void TriggerSuccessAction()
    {
        if (crateAnimation != null)
        {
            crateAnimation.Play(openAnimationName); // Play open animation on the crate
            Debug.Log("Crate is opening.");
        }
        else
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }

    public void CloseCrate()
    {
        if (crateAnimation != null)
        {
            crateAnimation.Play(closeAnimationName); // Play close animation on the crate
            Debug.Log("Crate is closing.");
        }
        else
        {
            Debug.LogWarning("Crate Animation component is missing.");
        }
    }
}