using UnityEngine;

public class FriendDroneCollector : MonoBehaviour
{
    [SerializeField] private string friendTag = "Friend";
    private PlayerHealth playerHealth; 

    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

/*     private void OnEnable() {
        DroneCounter.RegisterEvents();
    } */

    private void Start()
    {
        // Findet das GameObject "Sphere" und das PlayerHealth-Skript darauf
        GameObject playerObject = GameObject.Find("RunnerManager");
        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth script not found on Sphere object.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "End Potion")
        {
            GameObject[] friendDrones = GameObject.FindGameObjectsWithTag(friendTag);
            foreach (GameObject drone in friendDrones)
            {
                Destroy(drone);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // überprüfen ob das andere Objekt den Tag "Friend" hat
        if (other.CompareTag(friendTag))
        {
            Debug.Log("Drone collected: " + other.gameObject.name);
            Destroy(other.gameObject); // Drone nach collection zerstören
            //DroneCounter.IncrementCollectedCounter(); // Zähler erhöhen für eingesammelte Drohnen

            if(GameSyncManager.Instance) 
            {
                GameSyncManager.Instance.RpcIncreaseCollectedHintDrones();
            }

            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
            else
            {
                if (collectSound == null)
                {
                    Debug.LogWarning("No AudioClip assigned for collection sound.");
                }
                else
                {
                    Debug.LogWarning("AudioSource is not assigned or missing.");
                }
            }

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