using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public GameObject playerShieldPrefab; // Das Prefab des Shields
    public float shieldDistance = 2f; // Abstand des Shields vor dem Player
    public float shieldDuration = 10f; // Dauer, wie lange das Shield sichtbar bleibt

    private GameObject currentShield; // Referenz auf das aktuell aktive Shield

    void Update()
    {
        // �berpr�fe, ob die Leertaste gedr�ckt wurde
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateShield();
        }

        // Positioniere das Shield kontinuierlich vor dem Player
        if (currentShield != null)
        {
            Vector3 shieldPosition = transform.position + transform.forward * shieldDistance;
            currentShield.transform.position = shieldPosition;
        }
    }

    public void ActivateShield()
    {
        // Falls bereits ein Shield aktiv ist, zerst�re es
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        // Erstelle ein neues Shield 2 Meter vor dem Player
        Vector3 shieldPosition = transform.position + transform.forward * shieldDistance;
        currentShield = Instantiate(playerShieldPrefab, shieldPosition, Quaternion.identity);

        // Zerst�re das Shield nach der festgelegten Dauer
        Destroy(currentShield, shieldDuration);
    }
}
