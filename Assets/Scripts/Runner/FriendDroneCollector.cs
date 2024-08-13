using UnityEngine;

public class FriendDroneCollector : MonoBehaviour
{
    [SerializeField] private string friendTag = "Friend";// Tag für freundliche Drohnen
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