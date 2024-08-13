using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabNetworkController : MonoBehaviour
{
    private void Start()
    {
        // Start the timer when LabScene starts
        if (GameSyncManager.Instance.Runner != null && GameSyncManager.Instance.Runner.IsServer)
        {
            GameSyncManager.Instance.GameTimer = 0f;
        }
    }

    private void Update()
    {

    }

}
