using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public float speed = 20f; // Geschwindigkeit des Projektils
/*     public GameObject bigExplosion; // Prefab f�r die Explosion */

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

/*     void OnTriggerEnter(Collider other)
    {
        // �berpr�fe, ob das Projektil die Drone trifft
        if (other.CompareTag("Enemy"))
        {
            // Explosion ausl�sen
            TriggerExplosion(other.gameObject);
        }
    } */

/*     void TriggerExplosion(GameObject droneObject)
    {
        // Instanziere die Explosion an der Position der Drone
        Instantiate(bigExplosion, droneObject.transform.position, droneObject.transform.rotation);

        // Zerst�re die Drone
        Destroy(droneObject);

        // Zerst�re das Projektil
        Destroy(gameObject);
    } */
}