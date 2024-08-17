using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : MonoBehaviour
{

    public GameObject droneExplosion;
    public AudioSource audioSource;

    [SerializeField]
    private int defaultDroneHealth = 3;
    [SerializeField]
    private int decreasedDroneHealth = 3;
    [SerializeField]
    private float decreasedHealthTime = 10f;

    private float droneHealth = 3;


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

    private void Start()
    {
        // Cache the AudioSource component at the start
        //audioSource = GetComponent<AudioSource>();
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Attack Potion")
        {
            StartCoroutine(SetTemporarlyHealth(decreasedHealthTime));
        }

        if (potionType == "End Potion")
        {
            Destroy(gameObject);
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

        if (audioSource != null)
        {
            audioSource.Play();
        }


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
