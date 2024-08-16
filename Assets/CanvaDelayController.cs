using System.Collections;
using UnityEngine;

public class CanvaDelayController : MonoBehaviour
{
    public Canvas guideCanvas; 
    private Canvas thisCanvas; 

    void Start()
    {
       
        thisCanvas = GetComponent<Canvas>();

        if (thisCanvas == null)
        {
            Debug.LogError("No Canvas component found on this GameObject!");
            return;
        }

        
        thisCanvas.gameObject.SetActive(false);

        if (guideCanvas == null)
        {
            Debug.LogError("GuideCanva was not correctly referenced!");
            return;
        }

       
        StartCoroutine(WaitForGuideCanvasToDisappear());
    }

    IEnumerator WaitForGuideCanvasToDisappear()
    {
        Debug.Log("Waiting for GuideCanva to deactivate...");

        
        while (guideCanvas.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        Debug.Log("GuideCanva is deactivated, activating this Canvas.");

       
        thisCanvas.gameObject.SetActive(true);
    }
}