using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

public class ToggleLight : MonoBehaviour
{

    Light lightOrb;


    private void Start()
    {
        lightOrb = gameObject.GetComponent<Light>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger within orb");

        if (lightOrb.enabled == true)
        {
            lightOrb.enabled = false;
            Debug.Log("LightOrb disabled");
        } else if (lightOrb.enabled == false)
        {
            lightOrb.enabled = true;
            Debug.Log("LightOrb enabled");
        }

    }

}
