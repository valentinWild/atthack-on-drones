using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeDuration = 1.0f; 

    [SerializeField]
    private bool fadeIn = false;

    public static FadeInOut Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }
        Instance = this;
    }

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

    public void setColor(Color color)
    {
        Image image = GetComponentInChildren<Image>();
        if (image != null)
        {
            image.color = color;
        }
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