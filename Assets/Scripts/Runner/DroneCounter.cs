using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DroneCounter
{
    private static int droneExplosionCount = 0;

    public static void IncrementCounter()
    {
        droneExplosionCount++;
    }

    public static int GetCounter()
    {
        return droneExplosionCount;
    }
}