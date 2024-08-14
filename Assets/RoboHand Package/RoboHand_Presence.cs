using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RoboHand_Presence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;
    
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    //Animator Variables (Hand Model Tracking)
    public int HandModelNum;
    public List<GameObject> HandModels;
    public int WatchDog;
    public int OldWatchDog;
    public int LengthOfModelList;
   

    /// <summary>
    /// The Hand Models List should work as follows:
    /// 
    /// 0. RoboHands
    /// 1. Red Gloves
    /// 
    /// </summary>

    //Animator Variables (Finger Tracking)
    public float triggerFloatValue;
    public float gripFloatValue;
    public bool gripped;
    public float triggerTouchFloat;
    public bool triggerTouched;
    public bool joyTouch;
    public bool customGrab;
    public int CurrentPose;

    // THIS IS THE EDITED MULTI-POSE SCRIPT.
	
    // Start in this script is used to initialize the hand models as well as set customAnim to false, an unused variable =REDACTED=
    void Start()
    {
        HandModelNum = PlayerPrefs.GetInt("HandModel#");
        handModelPrefab = HandModels[HandModelNum];
        LengthOfModelList = (HandModels.Count - 1);
        TryInitialize();
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                //Debug.Log("Did not find corresponding controller model");
            }

            Destroy(spawnedHandModel);
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    } // VALEM code

    void UpdateTriggerValues()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            triggerFloatValue = triggerValue;
            handAnimator.SetFloat("Trigger", triggerValue);
            
        }
        else
        {
            triggerFloatValue = 0f;
            handAnimator.SetFloat("Trigger", 0);
            
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
            
            gripFloatValue = gripValue;
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
            
            gripFloatValue = 0f;
        }

        if (gripValue < 0.5f)
        {
            gripped = false;
        }
        if (gripValue > 0.5f)
        {
            gripped = true;
        }
    }// This void sets public variables for Trigger and Grip Trigger Values.
	
	void UpdatePointer() // This void updates the triggerTouched variable tracking Index Finger Position (on/off Trigger)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.indexTouch, out float triggerTouchFloat))
        {
          //  Debug.Log(triggerTouchFloat);

            if (triggerTouchFloat > 0)
            {
                handAnimator.SetBool("BTrigger", true);
                triggerTouched = true;
            }
            else
            {
                handAnimator.SetBool("BTrigger", false);
                triggerTouched = false;
            }
        }
        else
        {
            handAnimator.SetBool("BTrigger", false);
            triggerTouched = false;
        }

       // Debug.Log(triggerTouched);

	}

    void UpdateThumb() // This void updates public variables tracking the Thumb's position.
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool thumbtouch))
        {
            if (thumbtouch == true)
            {
                joyTouch = true;
            }
            else
            {
                joyTouch = false;
            }
        }
    }

    void UpdateGrabType() // This void is used to identify any items the player is grabbing to assign custom grab poses (such as for guns or props).
    {
        customGrab = false;
    }

    void UpdatePose() // This is the main Animation void. It uses all previously calculated variables to make a Pose selection.
    {

       // Debug.Log("gripped =" + gripped);

        if (customGrab == true)
        {
            // Not currently used until UpdateGrabType() is implemented.
        }
        else 
        {

            if (triggerTouched == false && gripped == true && joyTouch == true)
            {
                handAnimator.SetInteger("PoseInt", 1); // pointer finger pose
            }

            else if (triggerTouched == false && joyTouch == false)
            {
                handAnimator.SetInteger("PoseInt", 2); // Finger Gun Pose
            }

            else if (triggerTouched == true && triggerFloatValue > 0.75f && gripped == true && joyTouch == false)
            {
                handAnimator.SetInteger("PoseInt", 4); // Thumbs Up Pose
            }

            else if (triggerTouched == true && joyTouch == false)
            {
                handAnimator.SetInteger("PoseInt", 5); // IDLE THUMBS Pose
            }

            else
            {
                handAnimator.SetInteger("PoseInt", 0); // Main blend pose
            }
               
            
        }

        CurrentPose = handAnimator.GetInteger("PoseInt");

    }

    // Update is called once per frame
    void Update()
    {
        
        HandModelNum = PlayerPrefs.GetInt("HandModel#");
        ModelWatchdog();

        if (!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            if (showController)
            {
                if(spawnedHandModel)
                    spawnedHandModel.SetActive(false);
                if(spawnedController)
                    spawnedController.SetActive(true);
            }
            else
            {
                if (spawnedHandModel)
                    spawnedHandModel.SetActive(true);
                if (spawnedController)
                    spawnedController.SetActive(false);
                UpdateGrabType();
                UpdateTriggerValues();
				UpdatePointer();
                UpdateThumb();
                UpdatePose();
            }
        }
    }

   

    public void ModelWatchdog()
    {
        
        OldWatchDog = WatchDog;
        WatchDog = HandModelNum;
        if (WatchDog != OldWatchDog)
        {

            ChangeHandModel();

        }
        
    }

    public void ChangeHandModel()

    {

        
        handModelPrefab = HandModels[HandModelNum];
        TryInitialize();

    }

    // Voids below are currently unused

    public void customAnimStart()
    { 
    
    }

    public void customAnimStop()
    { 
    
    }

    void CustomPose(int Pose)
    { 
    
    }

    public void vibrateRightController()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, devices);

        foreach (var device in devices)
        {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.5f;
                    float duration = 0.2f;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }

    public void vibrateLeftController()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, devices);

        foreach (var device in devices)
        {
            UnityEngine.XR.HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.5f;
                    float duration = 0.2f;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
    }
}






