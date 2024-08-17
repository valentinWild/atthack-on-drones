using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public GameObject playerShieldPrefab;
    public float shieldDistance = 2f; 
    public float shieldDuration = 10f;
    public AudioClip shieldActivateSound;

    private GameObject currentShield; 
    private Camera mainCamera;
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
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Shield Potion")
        {
            ActivateShield();
        }
    }

    void Update()
    {
            /*if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ActivateShield();
            }*/


            // Shield erscheint kontinuierlich vor der Kamera
            if (currentShield != null)
        {
            Vector3 shieldPosition = mainCamera.transform.position + mainCamera.transform.forward * shieldDistance;
            currentShield.transform.position = shieldPosition;
            currentShield.transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void ActivateShield()
    {
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        // Shield 2 Meter vor der Kamera
        Vector3 shieldPosition = mainCamera.transform.position + mainCamera.transform.forward * shieldDistance;
        currentShield = Instantiate(playerShieldPrefab, shieldPosition, Quaternion.identity);

        // Ausrichtung des Shields an der Kamera
        currentShield.transform.SetParent(mainCamera.transform);

        if (shieldActivateSound != null)
        {
            audioSource.clip = shieldActivateSound;
            audioSource.Play();
            StartCoroutine(FadeOutVolume());
        }

        // Zerstöre das Shield nach der festgelegten Dauer
        Destroy(currentShield, shieldDuration);
    }

    private IEnumerator FadeOutVolume()
    {
        float startVolume = audioSource.volume;
        float fadeStartTime = 7f;
        float fadeDuration = 3f;

        
        yield return new WaitForSeconds(fadeStartTime);

        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / fadeDuration);
            yield return null;
        }

        
        audioSource.volume = 0f;

        
        audioSource.Stop();
    }
}