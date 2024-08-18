using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HealthPotionEffect : MonoBehaviour
{
    public float intensity = 0;
    public int activePriority = 1;  
    public int defaultPriority = 0;

    PostProcessVolume _volume;
    Vignette _vignette;

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
            _volume = GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings<Vignette>(out _vignette);

            if (!_vignette)
            {
            Debug.LogError("Vignette is not found in the PostProcessVolume profile.");
            }

            else
            {
                _vignette.enabled.Override(false);
                _volume.priority = defaultPriority;
        }
        }


    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Health Potion")
        {
            StartCoroutine(ActivateHealthPotionEffect());

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is missing!");
            }
            
        }
    }

    public IEnumerator ActivateHealthPotionEffect()
    {
        intensity = 0.8f;

        _volume.priority = activePriority;
        _vignette.enabled.Override(true);
        _vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(5.0f);

        while (intensity > 0)
        {
            intensity -= 0.02f;

            if (intensity < 0) intensity = 0;

            _vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }

        _vignette.enabled.Override(false);
        _volume.priority = defaultPriority;
        yield break;

    }
}
