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

public class FriendDroneCollector : MonoBehaviour
{
    public string friendTag = "Friend"; // Tag für freundliche Drohnen
    //public Transform leftControllerTransform; // Referenz zur Position des linken Controllers
    //private SphereCollider controllerCollider; // Referenz zum Collider des Controllers

    /*private void Awake()
    {
        // Fügt einen SphereCollider zum Controller hinzu, falls nicht bereits vorhanden
        controllerCollider = leftControllerTransform.gameObject.GetComponent<SphereCollider>();
        if (controllerCollider == null)
        {
            controllerCollider = leftControllerTransform.gameObject.AddComponent<SphereCollider>();
            controllerCollider.isTrigger = true; // Setzt den Collider auf Trigger, damit er Kollisionen auslöst
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob das andere Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            Debug.Log("Drone collected: " + other.gameObject.name);
            Destroy(other.gameObject); // Zerstöre die Drohne, wenn sie eingesammelt wird
            DroneCounter.IncrementCollectedCounter(); // Erhöhe den Zähler für eingesammelte Drohnen
        }
    }
}