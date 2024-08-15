using Fusion;
using UnityEngine;

public class GameSyncManager : NetworkBehaviour
{
    public static GameSyncManager Instance { get; set; }

    // Networked properties that will be synchronized across all clients
    [Networked] public float gameTimer { get; set; }
    [Networked] public int collectedHintDrones { get; set; }
    [Networked] public float runnerHealth { get; set; }
    [Networked] public string activePotion { get; set; }

    private ChangeDetector _changeDetector;

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
            gameTimer = 0f;
            collectedHintDrones = 0;
            runnerHealth = 100;
        }
    }

    private void Update()
    {
        if (HasStateAuthority)
        {
            // Update the timer every second
            //GameTimer += Time.deltaTime;
        }
    }

    // Networked RPC to allow clients to request Collected Hint Drones Changes
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcUpdateCollectedHintDrones(int currentAmount)
    {
        if (HasStateAuthority)
        {
            collectedHintDrones = currentAmount;
        }

        GameObject orbManager = GameObject.Find("OrbManager");
        var orbManagerScript = orbManager.GetComponent<OrbManager>();

        if (orbManagerScript != null)
        {
            orbManagerScript.SetHintCounter(collectedHintDrones);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcIncrementCollectedHintDrones()
    {
        if (HasStateAuthority)
        {
            collectedHintDrones++;
        }
    }

    // Networked RPC to allow clients to request Health changes
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcUpdateRunnerHealth(float newHealth)
    {
        if (HasStateAuthority)
        {
            // Only the authoritative instance should modify the Score
            runnerHealth = newHealth;

        }
    }

    // Networked RPC to communicate active potions for the runner
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetRunnerPotion(string potionType)
    {

        if (HasStateAuthority)
        {
            activePotion = potionType;
          

        }

        if (potionType == "Health Potion")
        {
            if (HasStateAuthority)
            {
                runnerHealth += 10;
            }
            //GameSyncManager.Instance.runnerHealth += 10;
            Debug.Log("Health Potion activated, increased Player Health");
        }
        else if (potionType == "Death Potion")
        {
            //GameSyncManager.Instance.runnerHealth -= 10;
            if (HasStateAuthority)
            {
                runnerHealth -= 10;
            }
            Debug.Log("Death Potion activated, decreased Player Health");
        }
        else if (potionType == "Shield Potion")
        {
            // Shield aktivieren
            Debug.Log("Shield Potion activated");
        }
        else if (potionType == "Attack Potion")
        {
            // Double Lasers or something
            Debug.Log("Attack Potion activated");
        }

        GameObject gameManager = GameObject.Find("gameManager");
        if (gameManager == null)
        {
            return;
        }
        var potionManager = gameManager.GetComponent<PotionManager>();
        if (potionManager != null)
        {
            potionManager.setActivePotion(potionType);
        }

    }

}
