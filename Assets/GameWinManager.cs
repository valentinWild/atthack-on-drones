using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameWinManager : MonoBehaviour
{
    public GameObject winCanvas;
    public PostProcessVolume postProcessingWin;
    public GameObject canva;

    private AudioSource winSound;
    private Vignette vignetteEffect;

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    void Start()
    {
        winCanvas.SetActive(false);
        winSound = GetComponent<AudioSource>();

        if (postProcessingWin != null && postProcessingWin.gameObject.activeInHierarchy)
        {
            if (postProcessingWin.profile.TryGetSettings(out vignetteEffect))
            {
                Debug.Log("Vignette effect found and initialized.");
                vignetteEffect.active = false;
            }
            else
            {
                Debug.LogWarning("Vignette effect not found in PostProcessVolume.");
            }
        }
        else
        {
            Debug.LogWarning("PostProcessingWin is not active or not assigned.");
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "End Potion")
        {
            ActivateVignetteEffect();
            ActivateWinCanvas();
            PlayWinSound();
            HideCanva();
        }
    }

    /*void Update()
    {
            PlayWinSound();
            ActivateVignetteEffect();
            HideCanva();
            ActivateWinCanvas();
    }*/

    void PlayWinSound()
    {
        if (winSound != null)
        {
            winSound.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or WinSound is not assigned.");
        }
    }

    void ActivateVignetteEffect()
    {
        if (vignetteEffect != null)
        {
            vignetteEffect.active = true;
        }
    }

    void HideCanva()
    {
        if (canva != null)
        {
            canva.SetActive(false); // Hide "Canva"
        }
        else
        {
            Debug.LogWarning("Canva object is not assigned.");
        }
    }

    void ActivateWinCanvas()
    {
        if (winCanvas != null)
        {
            winCanvas.SetActive(true); // Aktiviert das WinCanvas
        }
        else
        {
            Debug.LogWarning("winCanvas ist nicht zugewiesen.");
        }
    }
}