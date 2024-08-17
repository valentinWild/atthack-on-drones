using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
