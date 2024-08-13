using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class RunnerNetworkController : NetworkBehaviour
{

    private GameSyncManager gameSyncManager;

    private void Start()
    {
        gameSyncManager = FindObjectOfType<GameSyncManager>();

        if (gameSyncManager != null)
        {
            Debug.Log("Current Runner Health: " + gameSyncManager.RunnerHealth);
        }
    }

    private void Update()
    {
        if (gameSyncManager != null)
        {
            Debug.Log(gameSyncManager.RunnerHealth);
        }

    }
    public void UpdateRunnerHealth(float newHealth) {
        if (gameSyncManager != null)
        {
            Debug.Log("Update Runner health from Runner Controller");
            gameSyncManager.RpcUpdateRunnerHealth(newHealth);
        }
    }
}
