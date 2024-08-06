using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExpolision : MonoBehaviour
{
    public GameObject droneObject;  // Das Zielobjekt (Sphere)
    public float speed = 5f;  // Die Geschwindigkeit, mit der die Sphere fliegt
    private bool shouldMove = false;  // Um zu verfolgen, ob die Sphere sich bewegen soll
    private bool droneHit = false;  // Um zu verfolgen, ob die Drone getroffen wurde
    public float escapeSpeed = 10f;  // Die Geschwindigkeit, mit der die Drone aus der Szene fliegt
    public float destroyDelay = 3f;  // Die Zeit, nach der die Drone zerstört wird

    private Vector3 dronePosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (droneObject != null)
        {
            // Speichere die aktuelle Position der Drone
            dronePosition = droneObject.transform.position;
        }

        // Überprüfe, ob die Leertaste gedrückt wird
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Starte die Bewegung der Sphere
            shouldMove = true;
        }

        // Wenn die Sphere sich bewegen soll
        if (shouldMove)
        {
            // Bewege die Sphere zur Drone
            MoveTowardsTarget();
        }

        // Wenn die Drone getroffen wurde, bewege sie aus der Szene hinaus
        if (droneHit)
        {
            MoveDroneOutOfScene();
        }
    }

    void MoveTowardsTarget()
    {
        if (droneObject == null) return;

        // Berechne die Richtung zur aktuellen Position der Drone
        Vector3 direction = (dronePosition - transform.position).normalized;

        // Bewege die Sphere in Richtung der Drone
        transform.position += direction * speed * Time.deltaTime;

        // Überprüfe, ob die Sphere die Drone erreicht hat
        if (Vector3.Distance(transform.position, dronePosition) < 0.1f)
        {
            // Stoppe die Bewegung der Sphere
            shouldMove = false;

            // Setze droneHit auf true, um die Fluchtbewegung zu starten
            droneHit = true;
        }
    }

    void MoveDroneOutOfScene()
    {

        if (droneObject == null) return;

        // Bewege die Drone in eine zufällige Richtung aus der Szene hinaus
        Vector3 escapeDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Bewege die Drone in die Escape-Richtung
        droneObject.transform.position += escapeDirection * escapeSpeed * Time.deltaTime;

        // Zerstöre die Drone nach einer Verzögerung
        Destroy(droneObject, destroyDelay);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == droneObject)
        {
            // Startet die Bewegung aus der Szene
            droneHit = true;

            // Zugriff auf das Autofire-Skript und Schießen stoppen
            Autofire autofire = droneObject.GetComponent<Autofire>();
            if (autofire != null)
            {
                autofire.OnDroneHit();
                autofire.DestroyAllProjectiles(); // Sicherstellen, dass alle Projektile zerstört werden
            }
        }
    }
}
