using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

public class TakeDamageScript : MonoBehaviour
{
    public float intensity = 0;

    PostProcessVolume _volume;
    Vignette _vignette;

    public AudioSource audioSource;

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

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(TakeDamageEffect());
    }*/

    public IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        if (audioSource != null)
        {
            audioSource.Play();
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

