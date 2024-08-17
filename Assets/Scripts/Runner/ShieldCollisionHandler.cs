using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionHandler : MonoBehaviour
{
    public Material shieldMaterial; 
    public float collisionMaskPower = 10.0f; 
    public float normalMaskPower = -0.71f; 

    void Start()
    {
        
        if (shieldMaterial != null)
        {
            shieldMaterial.SetFloat("_MaskPower", normalMaskPower);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("Enemy") || other.CompareTag("Friend"))
        {
            
            if (shieldMaterial != null)
            {
                shieldMaterial.SetFloat("_MaskPower", collisionMaskPower);
            }

            
            if (other.CompareTag("Bullet"))
            {
                Destroy(other.gameObject);
            }

            
            if (other.CompareTag("Enemy"))
            {
                //DroneCounter.IncrementExplosionCounter();
                if (GameSyncManager.Instance)
                {
                    GameSyncManager.Instance.RpcIncreaseShotEnemyDrones(1);
                }
                Destroy(other.gameObject);
            }

            if (other.CompareTag("Friend"))
            {
                //DroneCounter.IncrementCollectedCounter();
                if (GameSyncManager.Instance)
                {
                    GameSyncManager.Instance.RpcIncreaseCollectedHintDrones();
                }
                Destroy(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("Enemy") || other.CompareTag("Friend"))
        {
            
            if (shieldMaterial != null)
            {
                shieldMaterial.SetFloat("_MaskPower", normalMaskPower);
            }
        }
    }
}