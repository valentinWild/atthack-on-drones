using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerNetworkController : MonoBehaviour
{
    private void Update()
    {
        // Access the synchronized timer
        float currentTime = GameSyncManager.Instance.GameTimer;
        Debug.Log($"Runner Scene Timer: {currentTime}");

    }
}
