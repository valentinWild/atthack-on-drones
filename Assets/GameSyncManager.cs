using Fusion;
using UnityEngine;

public class GameSyncManager : NetworkBehaviour
{
    public static GameSyncManager Instance;

    // Reference to the NetworkRunner
    private NetworkRunner runner;

    [Networked] public float GameTimer { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find the NetworkRunner in the scene (assuming there's only one)
        runner = FindObjectOfType<NetworkRunner>();
    }

    private void Update()
    {
        // Ensure the runner is valid, we have state authority, and the object is spawned
        if (runner != null && HasStateAuthority)
        {
            GameTimer += Time.deltaTime;
        }
    }

}
