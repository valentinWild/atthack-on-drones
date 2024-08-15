using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public GameObject playerShieldPrefab;
    public float shieldDistance = 2f; // Abstand des Shields vor der Kamera
    public float shieldDuration = 10f; // Dauer, wie lange das Shield sichtbar bleibt
    public AudioClip shieldActivateSound;

    private GameObject currentShield; 
    private Camera mainCamera;
    private AudioSource audioSource;

    void Start()
    {
        // Die Hauptkamera finden
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Überprüfe, ob die Leertaste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateShield();
        }

        // Positioniere das Shield kontinuierlich vor der Kamera
        if (currentShield != null)
        {
            Vector3 shieldPosition = mainCamera.transform.position + mainCamera.transform.forward * shieldDistance;
            currentShield.transform.position = shieldPosition;
            currentShield.transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void ActivateShield()
    {
        // Falls bereits ein Shield aktiv ist, zerstöre es
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        // Erstelle ein neues Shield 2 Meter vor der Kamera
        Vector3 shieldPosition = mainCamera.transform.position + mainCamera.transform.forward * shieldDistance;
        currentShield = Instantiate(playerShieldPrefab, shieldPosition, Quaternion.identity);

        // Richte das Shield an der Kamera aus
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