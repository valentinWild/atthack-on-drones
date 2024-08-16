using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Material activeMaterial;
    public Material inActiveMaterial;
    public string arrowDirection;

    private MeshRenderer[] meshRenderers; 

    void Start()
    {
        // Get all MeshRenderer components in the children of this GameObject
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log("Arrow script started");
    }
    
    private void OnEnable()
    {
        PlayerLeanHandler.OnLeanValueChanged += OnLeanValueChanged;
    }

    private void OnDisable()
    {
        PlayerLeanHandler.OnLeanValueChanged -= OnLeanValueChanged;
    }

    public void OnLeanValueChanged(float leanDirection)
    {
        Debug.Log("OnLeanValue changed: " + leanDirection);
        if (leanDirection == 1.0f)
        {
            if (arrowDirection == "Right")
            {
                UpadateMaterial(activeMaterial);
                return;
            }
        } 
        else if (leanDirection == -1.0f)
        {
            if (arrowDirection == "Left")
            {
                UpadateMaterial(activeMaterial);
                return;
            }
        }
        UpadateMaterial(inActiveMaterial);
    }

    private void UpadateMaterial(Material newMaterial) {
        // Loop through each MeshRenderer and assign the new material
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = newMaterial;
        }
    }
}

