using Fusion;
using UnityEngine;

public class GameSyncManager : NetworkBehaviour
{
    public static GameSyncManager Instance { get; set; }

    // Networked properties that will be synchronized across all clients
    [Networked] public float GameTimer { get; set; }
    [Networked] public int Score { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
    }

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            // Initialize values if this is the authoritative instance
            GameTimer = 0f;
            Score = 0;
        }
    }

    private void Update()
    {
        if (HasStateAuthority)
        {
            // Update the timer every second
            GameTimer += Time.deltaTime;
        }

        // Other logic to handle synced values...
    }

    public void AddScore(int points)
    {
        if (HasStateAuthority)
        {
            Score += points;
        }
    }
}
