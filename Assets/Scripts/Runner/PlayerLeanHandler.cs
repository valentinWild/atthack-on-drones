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
    public float leanThreshold = 20f; // Angle threshold in degrees

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
        // Calculate the tilt angle around the local z-axis (which determines left/right tilt)
        float headTiltAngle = Vector3.SignedAngle(headTransform.up, Vector3.up, headTransform.forward);

        // Determine the lean value based on the tilt angle
        float leanValue = 0f;
        if (headTiltAngle > leanThreshold)
        {
            leanValue = 1f; // Leaning to the right
        }
        else if (headTiltAngle < -leanThreshold)
        {
            leanValue = -1f; // Leaning to the left
        }

        // Invoke the lean value change event if the lean value has changed
        if (currentLeanValue != leanValue)
        {
            currentLeanValue = leanValue;
            OnLeanValueChanged?.Invoke(currentLeanValue);
        }

        // Invoke the UnityEvent if the player is leaning
        if (leanValue == 1 || leanValue == -1)
        {
            leanEvent.Invoke(leanValue);
        }
    }
}

