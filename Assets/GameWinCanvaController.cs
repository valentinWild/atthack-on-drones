using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinCanvaController : MonoBehaviour
{
    public AudioClip winSound; 
    private AudioSource audioSource;

    void Start()
    {
      
        gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            gameObject.SetActive(true);

            
            PlayWinSound();
        }
    }

    void PlayWinSound()
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            Debug.LogWarning("AudioSource oder WinSound nicht zugewiesen.");
        }
    }
}