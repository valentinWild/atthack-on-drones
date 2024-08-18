using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    
    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
    }

    
    public void PlayExplosionSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}