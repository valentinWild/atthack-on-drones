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

        
        {
            postProcessingWin.profile.TryGetSettings(out vignetteEffect);
            if (vignetteEffect != null)
            {
                vignetteEffect.active = false;
            }
            else
            {
                Debug.LogWarning("Vignette-Effekt nicht im PostProcessVolume gefunden.");
            }
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "End Potion")
        {
            ActivateVignetteEffect();
        }
    }

    void Update()
    {
        PlayWinSound();

        ActivateVignetteEffect();  // This line is correct

        HideCanva();
    }


    void PlayWinSound()
    {
        if (winSound != null)
        {
            winSound.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource oder WinSound nicht zugewiesen.");
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
            canva.SetActive(false); // "Canva" ausblenden
        }
        else
        {
            Debug.LogWarning("Canva-Objekt nicht zugewiesen.");
        }
    }
}