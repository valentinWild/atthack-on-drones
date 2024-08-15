using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceController : MonoBehaviour
{
    public Transform controller; // Reference to the controller (left or right)
    public float scaleMultiplier = 2.0f; // Scaling factor (2x movement)

    private Vector3 initialControllerPosition;
    private Vector3 initialHandPosition;

    private void Start()
    {
        // Save the initial positions of the controller and the hand
        initialControllerPosition = controller.localPosition;
        initialHandPosition = transform.localPosition;
    }

    private void Update()
    {
        // Calculate the offset of the controller from its initial position
        Vector3 controllerOffset = controller.localPosition - initialControllerPosition;

        // Scale the offset by the desired multiplier
        Vector3 scaledOffset = controllerOffset * scaleMultiplier;

        // Apply the scaled offset to the hand's initial position
        transform.localPosition = initialHandPosition + scaledOffset;
    }
}
