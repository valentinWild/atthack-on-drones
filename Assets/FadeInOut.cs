using System.Collections;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeDuration = 2.0f; 

    [SerializeField]
    private bool fadeIn = false;

    private void Start()
    {
        if (fadeIn)
        {
            StartCoroutine(FadeInAndOut());
        }
        else
        {
            FadeOut();
        }
    }

    /*private void Update()
    {
        // Check if the Return key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Toggle fadeIn and trigger the appropriate fade effect
            if (fadeIn)
            {
                FadeOut();
            }
            else
            {
                StartCoroutine(FadeInAndOut());
            }

            // Toggle the state for the next key press
            fadeIn = !fadeIn;
        }
    }*/

    public IEnumerator FadeInAndOut()
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration)); 
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration)); 
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}