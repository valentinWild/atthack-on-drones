using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class PlayerLeanHandler : MonoBehaviour
{

    public Transform headTransform;
    public float leanThreshold = 0.1f;

    private Vector3 initialHeadPosition;

    [SerializeField]
    private UnityEvent<float> leanEvent;

    void Start()
    {
        // Store the initial head position as the center point
        initialHeadPosition = headTransform.localPosition;
    }

    void Update()
    {
        // Calculate the lateral offset from the initial head position
        float lateralOffset = headTransform.localPosition.x - initialHeadPosition.x;

        // Check if the player is leaning left or right & determine Lean Value
        float leanValue = 0f;
        if (lateralOffset > leanThreshold)
        {
            //Debug.Log("lean right");
            leanValue = 1f;
        }
        else if (lateralOffset < -leanThreshold)
        {
            //Debug.Log("lean left");
            leanValue = -1f;
        }

        if (leanValue == 1 || leanValue == -1)
        {
            leanEvent.Invoke(leanValue);
        }

    }
}

