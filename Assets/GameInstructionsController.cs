using System.Collections;
using UnityEngine;

public class GameInstructionsController : MonoBehaviour
{
    private Canvas guideCanvas; 
    public GameObject canvaObject; 
    void Start()
    {
        
        guideCanvas = GetComponent<Canvas>();

        
        if (guideCanvas != null)
        {
            StartCoroutine(HideCanvasAfterDelay(10f));
        }
        else
        {
            Debug.LogError("No Canvas component found on this GameObject.");
        }

      
        if (canvaObject != null)
        {
            canvaObject.SetActive(false);
        }
        else
        {
            Debug.LogError("No Canva GameObject assigned.");
        }
    }

    IEnumerator HideCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 

        guideCanvas.gameObject.SetActive(false);

        
        if (canvaObject != null)
        {
            canvaObject.SetActive(true);
        }
    }
}