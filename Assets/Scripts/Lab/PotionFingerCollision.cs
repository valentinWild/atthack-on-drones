using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding is a finger from the SenseGlove
        if (other.CompareTag("SenseGloveFinger")) // Ensure fingers have the appropriate tag
        {
            Debug.Log("Finger touched the potion!");
            // Trigger any additional effects or interactions
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Detect when the finger leaves the potion
        if (other.CompareTag("SenseGloveFinger"))
        {
            Debug.Log("Finger left the potion.");
        }
    }
}
