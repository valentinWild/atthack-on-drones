/*using UnityEngine;
using UnityEngine.InputSystem;

public class FriendDroneCollector : MonoBehaviour
{
    public string friendTag = "Friend"; // Tag, der den freundlichen Drohnen zugewiesen ist
    public InputActionReference collectAction; // Aktion, die zum Sammeln verwendet wird (z.B. linker Button auf dem Controller)
    public float collectDistance = 5.0f; // Die maximale Entfernung, um eine Drohne zu sammeln

    private GameObject currentFriendDrone;


   private void OnEnable()
    {
         collectAction.action.performed += CollectFriendDrone; // Aktiviert die Sammel-Aktion
    }

    private void OnDisable()
    {
        collectAction.action.performed -= CollectFriendDrone; // Deaktiviert die Sammel-Aktion
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        // �berpr�fe, ob das Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            currentFriendDrone = other.gameObject;

            Debug.Log("Friend drone Collision");
           //collectAction.action.performed += CollectFriendDrone;
        }

    }

    /* private void OnTriggerExit(Collider other)
     {

         // �berpr�fe, ob das Objekt den Tag "Friend" hat
         if (other.CompareTag(friendTag))
         {
             Debug.Log("Friend drone Collision exit");
             collectAction.action.performed -= CollectFriendDrone;
             currentFriendDrone = null;
         }

     }*

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentFriendDrone)
        {
            currentFriendDrone = null;
        }
    }



    /*private void CollectFriendDrone(InputAction.CallbackContext context)
    {
        Debug.Log("Collect Action perfomed");
        if (currentFriendDrone != null){
            Destroy(currentFriendDrone);
            Debug.Log("Friend Drone collected");
            collectAction.action.performed -= CollectFriendDrone;
        }

        // Entferne das Friend-Drone-Objekt
    Destroy(friendDrone); 
    }
}*

 private void CollectFriendDrone(InputAction.CallbackContext context)
    {
        if (currentFriendDrone != null)
        {
            float distance = Vector3.Distance(transform.position, currentFriendDrone.transform.position);
            if (distance <= collectDistance)
            {
                Debug.Log("Drone collected: " + currentFriendDrone.name);
                Destroy(currentFriendDrone); // Zerstöre die Drohne, wenn sie eingesammelt wird
                currentFriendDrone = null; // Setze das aktuelle Drohnenobjekt auf null, da es zerstört wurde
            }
            else
            {
                Debug.Log("Drone is too far away to collect");
            }
        }
    }
}*/


using UnityEngine;
using UnityEngine.InputSystem;

public class FriendDroneCollector : MonoBehaviour
{
    public string friendTag = "Friend"; // Tag für freundliche Drohnen
    public InputActionReference collectAction; // Aktion zum Sammeln (z.B. linker Button auf dem Controller)
    public float collectDistance = 0.02f; // Die maximale Entfernung, um eine Drohne zu sammeln
    public Transform leftControllerTransform; // Referenz zur Position des linken Controllers

    private void OnEnable()
    {
        collectAction.action.performed += CollectFriendDrone; // Aktiviert die Sammel-Aktion
    }

    private void OnDisable()
    {
        collectAction.action.performed -= CollectFriendDrone; // Deaktiviert die Sammel-Aktion
    }

    private void CollectFriendDrone(InputAction.CallbackContext context)
    {
        // Finde alle Objekte mit dem Tag "Friend"
        GameObject[] friendDrones = GameObject.FindGameObjectsWithTag(friendTag);

        foreach (GameObject drone in friendDrones)
        {
            // Berechne die Entfernung zwischen dem linken Controller und der Drohne
            float distance = Vector3.Distance(leftControllerTransform.position, drone.transform.position);
            if (distance <= collectDistance)
            {
                Debug.Log("Drone collected: " + drone.name);
                Destroy(drone); // Zerstöre die Drohne, wenn sie eingesammelt wird
                DroneCounter.IncrementCollectedCounter(); // Erhöhe den Zähler für eingesammelte Drohnen
            }
        }
    }
}