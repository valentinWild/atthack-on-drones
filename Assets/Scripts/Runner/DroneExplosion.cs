using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : MonoBehaviour
{

    public GameObject droneExplosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        // �berpr�fe, ob das Projektil die Drone trifft
        if (other.CompareTag("PlayerShot"))
        {
            // Explosion ausl�sen
            TriggerExplosion();
        }
    }
    
    void TriggerExplosion()
    {
        // Instanziere die Explosion an der Position der Drone
        GameObject explosion = Instantiate(droneExplosion, gameObject.transform.position, gameObject.transform.rotation);

        // Zerst�re die Drone
        Destroy(gameObject);

        // Zerstöre die Explosion
        Destroy(explosion, 2f);

        // Increment the counter
        DroneCounter.IncrementCounter();
    }
}
