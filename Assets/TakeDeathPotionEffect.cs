using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

public class TakeDeathPotionEffect : MonoBehaviour
{
    public float intensity = 0;

    PostProcessVolume _volume;
    Vignette _vignette;

    public AudioClip potionActivateSound;
    private AudioSource audioSource;


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
            print("error, vignette is empty");
        }

        else
        {
            _vignette.enabled.Override(false);
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Death Potion")
        {
            StartCoroutine(TakeDamageEffect());
        }
    }

    public IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        if (potionActivateSound != null)
        {
            audioSource.PlayOneShot(potionActivateSound);
        }

        _vignette.enabled.Override(true);
        _vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(5.0f);

        while (intensity > 0)
        {
            intensity -= 0.01f;

            if (intensity < 0) intensity = 0;

            _vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }

        _vignette.enabled.Override(false);
        yield break;

    }
}