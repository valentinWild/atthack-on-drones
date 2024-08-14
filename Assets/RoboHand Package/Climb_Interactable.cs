using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Climb_Interactable : XRBaseInteractable
{

    private RoboHandsMovement robomove;

    private void Start()
    {
        robomove = GetComponent<RoboHandsMovement>();
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {

        base.OnSelectEntered(interactor);

        Debug.Log("SELECT ENTER");

        if (interactor is XRDirectInteractor)
        {
            RoboHandsMovement.ClimbingHand = interactor.GetComponent<XRController>();
        }
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {

        base.OnSelectExited(interactor);

        Debug.Log("SELECT EXIT");

        if (interactor is XRDirectInteractor)
        {
            if (RoboHandsMovement.ClimbingHand && RoboHandsMovement.ClimbingHand.name == interactor.name)
            {
                RoboHandsMovement.ClimbingHand = null;
                
            }
        }

    }
   
}
