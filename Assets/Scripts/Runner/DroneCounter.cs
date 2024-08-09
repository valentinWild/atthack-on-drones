using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DroneCounter
{
    private static int droneExplosionCount = 0;
    private static int collectedDroneCount = 0;

    public static void IncrementExplosionCounter()
    {
        droneExplosionCount++;
    }

    public static int GetExplosionCounter()
    {
        return droneExplosionCount;
    }

    public static void IncrementCollectedCounter()
    {
        collectedDroneCount++;
    }

    public static int GetCollectedCounter()
    {
        return collectedDroneCount;
    }


}