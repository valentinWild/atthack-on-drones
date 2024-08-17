using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityToggleScript : MonoBehaviour
{

    private RoboHandsMovement robomove;
    private GameObject character = null;
    private bool Switch = false;
    

   

    // Start is called before the first frame update
    public void Start()
    {
        robomove = GetComponent<RoboHandsMovement>();
        character = GameObject.Find("XR Rig");

        if (character != null)
        {
            Debug.Log("Found Player");
           
        }

         Switch = false;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleGravity()
    {
        if (Switch == false)
        {
            if (RoboHandsMovement.LinearFall == true)
            {
                RoboHandsMovement.LinearFall = false;
                Switch = true;
                Debug.Log("Switch Enabled - LF false");
            }
            else
            {
                RoboHandsMovement.LinearFall = true;
                Switch = true;
                Debug.Log("Switch Enabled - LF true");
            }
        }
    }

    public void ResetSwitch()
    {
        Switch = false;
        Debug.Log("Switch Reset");
    }
}
