using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private string bulletTag = "Bullet";// Tag für Drone Shots
    [SerializeField] private float damageAmount = 2f; // Schaden, den der Spieler erleidet
    private PlayerHealth playerHealth; // Referenz auf das PlayerHealth-Skript

    private void Start()
    {
        // Sucht nach dem PlayerHealth-Skript im selben GameObject
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen, ob das kollidierende Objekt den Tag "Bullet" hat
        if (other.CompareTag(bulletTag))
        {
            // Nachricht ausgeben, wenn der Spieler getroffen wird
            Debug.Log("User is shooted by drones");

            // Schaden dem Spieler zufügen
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            //Zerstören des Bullet-Objekts
            Destroy(other.gameObject, 0.1f);
        }
    }
}