/*
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using System.Collections;

public class PlayerLeanHandler : MonoBehaviour
{

    public Transform headTransform;
    public float leanThreshold = 0.1f;

    public static event Action<float> OnLeanValueChanged;

    private Vector3 initialHeadPosition;

    private float currentLeanValue = 0f;

    [SerializeField]
    private UnityEvent<float> leanEvent;

    private void Start()
    {
        // Store the initial head position as the center point
        Debug.Log("Player Leaning Handler started");
        initialHeadPosition = headTransform.localPosition;
        //StartCoroutine(testScript());
    }

    private IEnumerator testScript() 
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            OnLeanValueChanged.Invoke(-1f);
            //ChangeArrowColors(-1f);
            Debug.Log("Lean left simulated");
            yield return new WaitForSeconds(2f);
            OnLeanValueChanged.Invoke(1f);
            //ChangeArrowColors(1f);
            Debug.Log("Lean right simulated");
        }
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

        if(currentLeanValue != leanValue)
        {
            currentLeanValue = leanValue;
            //ChangeArrowColors(currentLeanValue);
            OnLeanValueChanged.Invoke(currentLeanValue);
        }

        if (leanValue == 1 || leanValue == -1)
        {
            leanEvent.Invoke(leanValue);
        }

    }

    private void ChangeArrowColors(float direction) {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow");
        // Iterate through each GameObject and call the method on the ArrowController script
        foreach (GameObject arrow in arrows)
        {
            ArrowController controller = arrow.GetComponent<ArrowController>();
            if (controller != null)
            {
                controller.OnLeanValueChanged(direction);  // Replace DoSomething with your desired method
            }
        }
    }
}
*/
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using System.Collections;

public class PlayerLeanHandler : MonoBehaviour
{
    public Transform headTransform;
    public float leanThreshold = 30f; // Angle in degrees for detecting leaning

    public static event Action<float> OnLeanValueChanged;

    private float currentLeanValue = 0f;

    [SerializeField]
    private UnityEvent<float> leanEvent;

    private void Start()
    {
        Debug.Log("Player Leaning Handler started");
    }

    void Update()
    {
        // Calculate the tilt angle relative to the vertical axis (world's up direction)
        Vector3 headRight = headTransform.right; // Local right direction of the head
        Vector3 worldUp = Vector3.up;

        // Calculate the angle between the head's right vector and the world up vector
        float tiltAngle = Vector3.SignedAngle(worldUp, headRight, headTransform.forward);

        // Determine Lean Value based on tilt angle
        float leanValue = 0f;
        if (tiltAngle > leanThreshold)
        {
            leanValue = 1f; // Leaning to the right
        }
        else if (tiltAngle < -leanThreshold)
        {
            leanValue = -1f; // Leaning to the left
        }

        // Trigger the event only if lean value changes
        if (currentLeanValue != leanValue)
        {
            currentLeanValue = leanValue;
            OnLeanValueChanged?.Invoke(currentLeanValue);
        }

        if (leanValue == 1 || leanValue == -1)
        {
            leanEvent.Invoke(leanValue);
        }
    }
}