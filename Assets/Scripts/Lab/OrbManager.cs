using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    // This array tracks the current state of each orb (true = on, false = off)
    [SerializeField] private bool[] orbStates = new bool[4];

    // Define the correct code sequence
    [SerializeField] private bool[] correctCode = new bool[] { true, false, true, false };

    // Method called by each orb to update its state
    public void UpdateOrbState(int orbIndex, bool isOn)
    {
        orbStates[orbIndex] = isOn;
        CheckCode();
    }

    // Check if the current state matches the correct code
    private void CheckCode()
    {
        for (int i = 0; i < orbStates.Length; i++)
        {
            if (orbStates[i] != correctCode[i])
            {
                return; // Early return if the code doesn't match
            }
        }

        // Code matches
        Debug.Log("Correct code entered!");
        TriggerSuccessAction();
    }

    // Trigger the action when the correct code is entered
    private void TriggerSuccessAction()
    {
        Debug.Log("Puzzle Piece revealed!");
        //TODO: insert opening of box for puzzle piece
    }
}
