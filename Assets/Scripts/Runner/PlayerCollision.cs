using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        // Überprüfen, ob das kollidierende Objekt den Tag "Bullet" hat
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Nachricht ausgeben, wenn der Spieler getroffen wird
            Debug.Log("User shooted by drones");

        
        }
    }
}