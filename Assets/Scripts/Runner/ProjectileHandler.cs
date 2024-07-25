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

}
