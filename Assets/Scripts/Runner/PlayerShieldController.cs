using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public GameObject playerShieldPrefab; // Das Prefab des Shields
    public float shieldDistance = 2f; // Abstand des Shields vor der Kamera
    public float shieldDuration = 10f; // Dauer, wie lange das Shield sichtbar bleibt

    private GameObject currentShield; // Referenz auf das aktuell aktive Shield
    private Camera mainCamera; // Referenz zur Hauptkamera

    void Start()
    {
        // Die Hauptkamera finden
        mainCamera = Camera.main;
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

        // Zerstöre das Shield nach der festgelegten Dauer
        Destroy(currentShield, shieldDuration);
    }
}