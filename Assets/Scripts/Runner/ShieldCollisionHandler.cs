using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Bullet"))
        {
            
            Destroy(other.gameObject);
        }

        
        if (other.CompareTag("Enemy"))
        {
            DroneCounter.IncrementExplosionCounter();

            Destroy(other.gameObject);
        }

        
        if (other.CompareTag("Friend"))
        {
            DroneCounter.IncrementCollectedCounter();

            Destroy(other.gameObject);
        }
    }
}