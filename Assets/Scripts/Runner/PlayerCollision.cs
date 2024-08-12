using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private string bulletTag = "Bullet";// Tag für Drone Shots

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen, ob das kollidierende Objekt den Tag "Bullet" hat
        if (other.CompareTag(bulletTag))
        {
            // Nachricht ausgeben, wenn der Spieler getroffen wird
            Debug.Log("User is shooted by drones");

            //Zerstören des Bullet-Objekts
            Destroy(other.gameObject, 0.1f);
        }
    }
}