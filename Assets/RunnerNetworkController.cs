using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerNetworkController : MonoBehaviour
{

    private GameSyncManager gameSyncManager;


    private void Start()
    {
        gameSyncManager = FindObjectOfType<GameSyncManager>();

        if (gameSyncManager != null)
        {
            Debug.Log("Current Game Timer: " + gameSyncManager.GameTimer);
            gameSyncManager.AddScore(10);
        }
    }

    private void Update()
    {
        if (gameSyncManager != null)
        {
            gameSyncManager.AddScore(1);
        }
    }
}
