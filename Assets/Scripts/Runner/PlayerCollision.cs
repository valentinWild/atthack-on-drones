using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempleRun.Player {

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private string bulletTag = "Bullet";// Tag f�r Drone Shots
    [SerializeField] private float damageAmount = 2f; // Schaden, den der Spieler erleidet
    private PlayerHealth playerHealth; // Referenz auf das PlayerHealth-Skript
    private PlayerController playerController;

    private void Start()
    {
        // Sucht nach dem PlayerHealth-Skript im selben GameObject
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Player Collision with: " + other.gameObject.tag);

        // �berpr�fen, ob das kollidierende Objekt den Tag "Bullet" hat
        if (other.CompareTag(bulletTag))
        {
            // Nachricht ausgeben, wenn der Spieler getroffen wird
            Debug.Log("User is shooted by drones");

            // Schaden dem Spieler zuf�gen
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            //Zerst�ren des Bullet-Objekts
            Destroy(other.gameObject, 0.1f);
        }

        else if (other.CompareTag("TurnLeftFailed"))
        {
            playerController.ForcePlayerTurn(-1);
        }

        else if (other.CompareTag("TurnRightFailed"))
        {
            playerController.ForcePlayerTurn(1);
        }

        else if (other.CompareTag("TurnSidewaysFailed"))
        {
            playerController.ForcePlayerTurn(-1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Player Collision with: " + collision);

    }
}
}