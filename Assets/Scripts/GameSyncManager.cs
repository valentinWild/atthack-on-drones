using Fusion;
using UnityEngine;
using System;

public class GameSyncManager : NetworkBehaviour
{
    public static GameSyncManager Instance { get; set; }

    public static int requiredDronesPerHint = 5;

    public static event Action<float> OnRunnerHealthChanged;
    public static event Action<int> OnDecodedHintsChanged;
    public static event Action<int> OnUnlockedHintsChanged;
    public static event Action<string> OnActivePotionChanged;
    public static event Action<int> OnCurrentLevelChanged;
    public static event Action<int> OnCollectedDronesChanged;

    // Networked properties that will be synchronized across all clients
    [Networked] public float gameTimer { get; set; }
    [Networked] public int collectedHintDrones { get; set; }
    [Networked] public int unlockedHints { get; set; }
    [Networked] public float runnerHealth { get; set; }
    [Networked] public string activePotion { get; set; }
    [Networked] public int decodedHints { get; set; }
    [Networked] public int currentLevel { get; set; }

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
            currentLevel = 1;
            gameTimer = 0f;
            collectedHintDrones = 0;
            runnerHealth = 100;
        }
    }

    // RPC to change the Health of the Runner
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateRunnerHealth(float newHealth)
    {
        UpdateRunnerHealth(newHealth);
        OnRunnerHealthChanged?.Invoke(newHealth);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcIncreaseRunnerHealth(float amount)
    {
        float newHealth = runnerHealth + amount;
        UpdateRunnerHealth(newHealth);
        OnRunnerHealthChanged?.Invoke(newHealth);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcDecreaseRunnerHealth(float amount)
    {
        float newHealth = runnerHealth - amount;
        UpdateRunnerHealth(newHealth);
        OnRunnerHealthChanged?.Invoke(newHealth);
    }
    private void UpdateRunnerHealth(float newHealth)
    {
        if (HasStateAuthority)
        {
            runnerHealth = newHealth;
        }
    }

    // Networked RPC to change the amount of decoded Hints
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateDecodedHints(int currentAmount)
    {
        UpdateDecodedHints(currentAmount);
        OnDecodedHintsChanged?.Invoke(currentAmount);
    }
    private void UpdateDecodedHints(int currentAmount)
    {
        if (HasStateAuthority)
        {
            decodedHints = currentAmount;
        }
    }


    // Networked RPC to communicate active potions for the runner
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetRunnerPotion(string potionType)
    {
        UpdateRunnerPotion(potionType);
        OnActivePotionChanged?.Invoke(potionType);
    }
    private void UpdateRunnerPotion(string potionType)
    {
        if (HasStateAuthority)
        {
            activePotion = potionType;
        }
    }

    // Networked RPC to increase the level
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcIncreaseLevel()
    {
        int newLevel = currentLevel + 1;
        UpdateCurrentLevel(newLevel);
        OnCurrentLevelChanged?.Invoke(newLevel);
    }
    // Networked RPC to reset the level to 1
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcResetLevel()
    {
        int newLevel = 1;
        UpdateCurrentLevel(newLevel);
        OnCurrentLevelChanged?.Invoke(newLevel);
    }
    private void UpdateCurrentLevel(int newLevel)
    {
        if (HasStateAuthority)
        {
            currentLevel = newLevel;
        }
    }

    // Networked RPC to allow clients to request Collected Hint Drones Changes
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcIncreaseCollectedHintDrones()
    {
        int newAmount = collectedHintDrones + 1;
        UpdateCollectedHintDrones(newAmount);
        OnCollectedDronesChanged?.Invoke(newAmount);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcResetCollectedHintDrones()
    {
        int newAmount = 0;
        UpdateCollectedHintDrones(newAmount);
        OnCollectedDronesChanged?.Invoke(newAmount);
        CheckUnlockHints(requiredDronesPerHint);
    }
    private void UpdateCollectedHintDrones(int currentAmount) 
    {
        if (HasStateAuthority)
        {
            collectedHintDrones = currentAmount;
        }
    }

    private void CheckUnlockHints(int requiredDrones)
    {
        if (collectedHintDrones % 5 == 0 && unlockedHints < 4)
        {
            unlockedHints++;
            OnUnlockedHintsChanged?.Invoke(requiredDrones);
        }
    }

}