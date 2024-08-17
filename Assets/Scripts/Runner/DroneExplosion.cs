using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : MonoBehaviour
{

    public GameObject droneExplosion;
    public AudioClip explosionSound;

    [SerializeField]
    private int defaultDroneHealth = 1;
    [SerializeField]
    private int decreasedDroneHealth = 1;
    [SerializeField]
    private float decreasedHealthTime = 10f;

    private float droneHealth = 1;
    private AudioSource audioSource;

    private void OnEnable()
    {
/*         if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        } */
    }

    private void OnDisable()
    {
/*         if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        } */
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Attack Potion")
        {
            StartCoroutine(SetTemporarlyHealth(decreasedHealthTime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob das Projektil die Drone trifft
        if (other.CompareTag("PlayerShot"))
        {
            droneHealth--;
        }

        if (droneHealth <= 0)
        {
            TriggerExplosion();
        }
    }
    
    void TriggerExplosion()
    {

        GameObject explosion = Instantiate(droneExplosion, gameObject.transform.position, gameObject.transform.rotation);
        
/*         if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        } */

        Destroy(gameObject);
        Destroy(explosion, 2f);

        if(GameSyncManager.Instance) 
        {
            GameSyncManager.Instance.RpcIncreaseShotEnemyDrones(1);
        }

        //DroneCounter.IncrementExplosionCounter();
    }

    private IEnumerator SetTemporarlyHealth(float duration)
    {
        droneHealth = decreasedDroneHealth;
        yield return new WaitForSeconds(duration);
        droneHealth = defaultDroneHealth;
    }
}
