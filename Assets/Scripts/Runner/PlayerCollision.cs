using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private string bulletTag = "Bullet";// Tag f�r Drone Shots

    private void OnTriggerEnter(Collider other)
    {
        // �berpr�fen, ob das kollidierende Objekt den Tag "Bullet" hat
        if (other.CompareTag(bulletTag))
        {
            // Nachricht ausgeben, wenn der Spieler getroffen wird
            Debug.Log("User is shooted by drones");

            //Zerst�ren des Bullet-Objekts
            Destroy(other.gameObject, 0.1f);
        }
    }
}