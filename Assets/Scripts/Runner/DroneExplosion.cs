using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : MonoBehaviour
{

    public GameObject droneExplosion;

    [SerializeField]
    private int defaultDroneHealth = 3;
    [SerializeField]
    private int decreasedDroneHealth = 1;
    [SerializeField]
    private float decreasedHealthTime = 10f;

    private float droneHealth = 3;

    private void OnEnable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
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
        // �berpr�fe, ob das Projektil die Drone trifft
        if (other.CompareTag("PlayerShot"))
        {
            // Explosion ausl�sen
            TriggerExplosion();
        }
    }
    
    void TriggerExplosion()
    {
        // Instanziere die Explosion an der Position der Drone
        GameObject explosion = Instantiate(droneExplosion, gameObject.transform.position, gameObject.transform.rotation);

        // Zerst�re die Drone
        Destroy(gameObject);

        // Zerstöre die Explosion
        Destroy(explosion, 2f);

        // Increment the counter
        DroneCounter.IncrementExplosionCounter();
    }

    private IEnumerator SetTemporarlyHealth(float duration)
    {
        droneHealth = decreasedDroneHealth;
        yield return new WaitForSeconds(duration);
        droneHealth = defaultDroneHealth;
    }
}
