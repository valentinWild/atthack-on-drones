using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DroneCounter
{
    private static int droneExplosionCount = 0;
    private static int collectedDroneCount = 0;

    public static void RegisterEvents() {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnCollectedDronesChanged += OnCollectedDronesChanged;
        }
    }

    public static void IncrementExplosionCounter()
    {
        droneExplosionCount++;
    }

    private static void OnCollectedDronesChanged(int amount) {
        collectedDroneCount = amount;
    }

    public static int GetExplosionCounter()
    {
        return droneExplosionCount;
    }

    public static void IncrementCollectedCounter()
    {
        if(GameSyncManager.Instance) 
        {
            //collectedDroneCount = GameSyncManager.Instance.collectedHintDrones + 1;
            GameSyncManager.Instance.RpcIncreaseCollectedHintDrones();
        }
    }

    public static int GetCollectedCounter()
    {
        return collectedDroneCount;
    }


}