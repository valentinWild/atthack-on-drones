using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{

    public GameObject explosionPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject spawnedExpolsion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
        Destroy(spawnedExpolsion, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get the layer ID for the "Ground" layer
        int groundLayer = LayerMask.NameToLayer("Ground");

        // Check if the other collider's game object is on the "Ground" layer
        if (other.gameObject.layer == groundLayer)
        {
            GameObject spawnedExpolsion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
            Destroy(spawnedExpolsion, 2);
            
        }
    }
}
