using UnityEngine;
using UnityEngine.InputSystem;

public class FriendDroneCollector : MonoBehaviour
{
    public string friendTag = "Friend"; // Tag, der den freundlichen Drohnen zugewiesen ist
    public InputAction collectAction; // Aktion, die zum Sammeln verwendet wird (z.B. linker Button auf dem Controller)

    private void OnEnable()
    {
        collectAction.Enable(); // Aktiviert die Sammel-Aktion
    }

    private void OnDisable()
    {
        collectAction.Disable(); // Deaktiviert die Sammel-Aktion
    }

    private void OnTriggerEnter(Collider other)
    {
        // �berpr�fe, ob das Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            // �berpr�fe, ob der Sammel-Button gedr�ckt wurde
            if (collectAction.triggered)
            {
                CollectFriendDrone(other.gameObject);
            }
        }
    }

    private void CollectFriendDrone(GameObject friendDrone)
    {
        Debug.Log("Friend Drone collected: " + friendDrone.name);

        // Entferne das Friend-Drone-Objekt
        Destroy(friendDrone);
    }
}