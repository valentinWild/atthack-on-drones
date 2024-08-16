using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonFollowVisual : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 lokalAxis;
    public float resetSpeed;

    private bool freeze = false; 

    private Vector3 initialLocalPos;

    private Vector3 offset;
    private Transform pokeAttachTransform; 

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    // Start is called before the first frame update
    void Start()
    {
        initialLocalPos = visualTarget.localPosition;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(ResetButton);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;
            
            isFollowing = true;
            freeze = false;

            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;
        }
    }

    public void ResetButton(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false; 
        }
        
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
        {
            return; 
        }
        if (isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constraintLocalTargetPosition = Vector3.Project(localTargetPosition, lokalAxis);

            visualTarget.position = visualTarget.TransformPoint(constraintLocalTargetPosition);
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPos, Time.deltaTime * resetSpeed);
        }
    }
}
