using UnityEngine;

public class GameWinManager : MonoBehaviour
{
    public GameObject winCanvas; // Ziehe hier das Canvas GameObject hinein
    public AudioClip winSound;
    private AudioSource audioSource;

    void Start()
    {
        winCanvas.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            winCanvas.SetActive(true);

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