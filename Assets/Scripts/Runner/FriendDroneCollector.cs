using UnityEngine;

public class FriendDroneCollector : MonoBehaviour
{
    [SerializeField] private string friendTag = "Friend";// Tag für freundliche Drohnen
    private PlayerHealth playerHealth; // Referenz auf das PlayerHealth-Skript

    private void Start()
    {
        // Findet das GameObject "Sphere" und das PlayerHealth-Skript darauf
        GameObject sphere = GameObject.Find("Sphere");
        if (sphere != null)
        {
            playerHealth = sphere.GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth script not found on Sphere object.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // überprüfen ob das andere Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            Debug.Log("Drone collected: " + other.gameObject.name);
            Destroy(other.gameObject); // Drone nach collection zerstören
            DroneCounter.IncrementCollectedCounter(); // Zähler erhöhen für eingesammelte Drohnen

            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(5);
            }
            else
            {
                Debug.LogWarning("PlayerHealth reference is not set!");
            }
        }
    }
}