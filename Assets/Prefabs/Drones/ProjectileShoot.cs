using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public float speed = 20f; // Geschwindigkeit des Projektils
    public GameObject bigExplosion; // Prefab für die Explosion

    private Vector3 direction; // Richtung, in die das Projektil fliegt

    // Richtung des Projektils setzen
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        // Bewege das Projektil in die angegebene Richtung
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob das Projektil die Drone trifft
        if (other.CompareTag("Enemy"))
        {
            // Explosion auslösen
            TriggerExplosion(other.gameObject);
        }
    }

    void TriggerExplosion(GameObject droneObject)
    {
        // Instanziere die Explosion an der Position der Drone
        Instantiate(bigExplosion, droneObject.transform.position, droneObject.transform.rotation);

        // Zerstöre die Drone
        Destroy(droneObject);

        // Zerstöre das Projektil
        Destroy(gameObject);
    }
}