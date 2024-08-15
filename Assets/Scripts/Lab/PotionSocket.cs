using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSocket : MonoBehaviour
{
    public Transform socket; // The socket where the potion will spawn and reset
    public string floorTag = "Floor"; // Tag assigned to the floor
    public Rigidbody potionRigidbody; // Reference to the potion's Rigidbody component
    public float resetDelay = 0f; // Time to wait before resetting the potion after it hits the floor

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        // Store the initial position and rotation of the potion
        initialPosition = socket.position;
        initialRotation = socket.rotation;

        // Place the potion in the socket at the start
        ResetPotionToSocket();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the potion has collided with the floor
        if (collision.gameObject.CompareTag(floorTag))
        {
            // Start the reset coroutine after a delay
            StartCoroutine(ResetPotionAfterDelay());
        }
    }

    private IEnumerator ResetPotionAfterDelay()
    {
        // Wait for the specified delay before resetting the potion
        yield return new WaitForSeconds(resetDelay);

        // Reset the potion to its socket position
        ResetPotionToSocket();
    }

    private void ResetPotionToSocket()
    {
        // Disable physics temporarily to avoid interference during reset
        potionRigidbody.isKinematic = true;

        // Reset position and rotation to the socket
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Re-enable physics
        potionRigidbody.isKinematic = false;
    }
}