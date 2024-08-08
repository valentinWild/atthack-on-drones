using UnityEngine;
using UnityEngine.InputSystem;

public class FriendDroneCollector : MonoBehaviour
{
    public string friendTag = "Friend"; // Tag, der den freundlichen Drohnen zugewiesen ist
    public InputActionReference collectAction; // Aktion, die zum Sammeln verwendet wird (z.B. linker Button auf dem Controller)

    private GameObject currentFriendDrone;


/*     private void OnEnable()
    {
         collectAction.action.performed += CollectFriendDrone; // Aktiviert die Sammel-Aktion
    }

    private void OnDisable()
    {
        collectAction.action.performed -= CollectFriendDrone; // Deaktiviert die Sammel-Aktion
    } */

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        // �berpr�fe, ob das Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            currentFriendDrone = other.gameObject;

            Debug.Log("Friend drone Collision");
           collectAction.action.performed += CollectFriendDrone;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        // �berpr�fe, ob das Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            Debug.Log("Friend drone Collision exit");
            collectAction.action.performed -= CollectFriendDrone;
            currentFriendDrone = null;
        }

    }



    private void CollectFriendDrone(InputAction.CallbackContext context)
    {
        Debug.Log("Collect Action perfomed");
        if (currentFriendDrone != null){
            Destroy(currentFriendDrone);
            Debug.Log("Friend Drone collected");
            collectAction.action.performed -= CollectFriendDrone;
        }

        // Entferne das Friend-Drone-Objekt
/*         Destroy(friendDrone); */
    }
}