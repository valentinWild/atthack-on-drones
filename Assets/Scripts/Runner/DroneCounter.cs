using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCounter
{
    public int droneExplosionCount = 0;
    public int collectedDroneCount = 0;

/*     public void RegisterEvents() {

    } */

    private void OnEnable() {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnCollectedDronesChanged += OnCollectedDronesChanged;
            GameSyncManager.OnShotEnemyDronesChanged += OnShotEnemyDronesChanged;
        }
    }

    private void OnDisable() {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnCollectedDronesChanged -= OnCollectedDronesChanged;
            GameSyncManager.OnShotEnemyDronesChanged -= OnShotEnemyDronesChanged;
        }
    }

/*     public static void IncrementExplosionCounter()
    {
        droneExplosionCount++;
    } */

    private void OnCollectedDronesChanged(int amount) {
        collectedDroneCount = amount;
    }

    private void OnShotEnemyDronesChanged(int amount) {
        droneExplosionCount = amount;
    }

    public int GetExplosionCounter()
    {
        return droneExplosionCount;
    }

/*     public static void IncrementCollectedCounter()
    {

    } */

    public int GetCollectedCounter()
    {
        return collectedDroneCount;
    }

}