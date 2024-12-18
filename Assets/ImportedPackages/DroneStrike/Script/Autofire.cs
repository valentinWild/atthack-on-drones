using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autofire : MonoBehaviour
{
    public GameObject bullet1;
    public GameObject bullet2;
    public Transform[] spawnPoints; // Array to hold multiple spawn points
    public float fireSpeed1 = 50f;
    public float fireSpeed2 = 30f;
    public float fireInterval = 0.5f; // Interval between each burst
    public bool alternateFire = false; // Boolean to enable alternating fire mode
    public bool fireInBursts = false; // Boolean to enable firing bursts of 3 projectiles
    public float burstInterval = 0.1f; // Interval between shots in a burst
    public bool useSecondProjectile = false; // Boolean to toggle between projectiles

    private int nextSpawnPointIndex = 0;
    private bool isHit = false;

    private Coroutine fireCoroutine;
    private Transform playerTransform;

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    private void Start()
    {
        // Suche das PlayerObject in der Szene und speichere seinen Transform
        GameObject playerObject = GameObject.Find("RunnerManager");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("PlayerObject not found in the scene.");
        }

        //Shooting starts 5 seconds later
        Invoke("StartFiring", 8f);
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "End Potion")
        {
            
            isHit = true;

            
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }

            DestroyAllProjectiles();

            Debug.Log("End Potion activated. All drones have stopped shooting.");
        }
    }

    private void StartFiring()
    {
        StartCoroutine(FireBullets());
    }

    private IEnumerator FireBullets()
    {
        while (!isHit) // Continue shooting only if not hit
        {
            if (fireInBursts)
            {
                yield return StartCoroutine(FireBurst());
            }
            else
            {
                FireOnce();
                yield return new WaitForSeconds(fireInterval);
            }
        }
    }

    private IEnumerator FireBurst()
    {
        if (alternateFire)
        {
            yield return StartCoroutine(FireBurstAlternating());
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                FireBulletSimultaneous();
                yield return new WaitForSeconds(burstInterval);
            }
            yield return new WaitForSeconds(fireInterval - 3 * burstInterval);
        }
    }

    private IEnumerator FireBurstAlternating()
    {
        for (int i = 0; i < 3; i++)
        {
            if (isHit) yield break; // Stop firing if hit
            FireBulletFromCurrentSpawnPoint();
            yield return new WaitForSeconds(burstInterval);
        }
        // Move to the next spawn point after completing a burst
        nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Length;
        yield return new WaitForSeconds(fireInterval - 3 * burstInterval);
    }

    private void FireOnce()
    {
        if (isHit) return; // Stop firing if hit
        if (alternateFire)
        {
            FireBulletAlternating();
        }
        else
        {
            FireBulletSimultaneous();
        }
    }

    private void FireBulletSimultaneous()
    {
        if ((bullet1 == null && bullet2 == null) || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Bullets or spawn points are not assigned.");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                FireBulletFromSpawnPoint(spawnPoint);
            }
        }
    }

    private void FireBulletAlternating()
    {
        if ((bullet1 == null && bullet2 == null) || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Bullets or spawn points are not assigned.");
            return;
        }

        FireBulletFromCurrentSpawnPoint();

        // Move to the next spawn point after firing a single shot
        nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Length;
    }

    private void FireBulletFromCurrentSpawnPoint()
    {
        if (spawnPoints[nextSpawnPointIndex] != null)
        {
            FireBulletFromSpawnPoint(spawnPoints[nextSpawnPointIndex]);
        }
    }

    private void FireBulletFromSpawnPoint(Transform spawnPoint)
    {
        if (playerTransform != null)
        {
            // Berechne den Vektor von der Drohne zum PlayerObject
            Vector3 toPlayer = playerTransform.position - spawnPoint.position;

            // �berpr�fe die Entfernung zum PlayerObject
            float distanceToPlayer = toPlayer.magnitude;
            //Debug.Log("Distance to Player: " + distanceToPlayer);

            // Schie�en nur, wenn das PlayerObject innerhalb von 3 Metern ist
            if (distanceToPlayer > 30f)
            {
                //Debug.Log("Player is too far, not shooting.");
                return; // Wenn das PlayerObject weiter als 3 Meter entfernt ist, nicht schie�en
            }

            // �berpr�fe, ob die Drohne hinter dem PlayerObject ist
            float dotProduct = Vector3.Dot(playerTransform.forward, toPlayer.normalized);

            if (dotProduct >= 0)
            {
                // Die Drohne befindet sich hinter dem PlayerObject, also nicht schie�en
                return;
            }

            // Drehe den SpawnPoint, um auf das PlayerObject zu zeigen
            spawnPoint.rotation = Quaternion.LookRotation(toPlayer.normalized);

            GameObject selectedBullet = useSecondProjectile ? bullet2 : bullet1;
            float selectedSpeed = useSecondProjectile ? fireSpeed2 : fireSpeed1;

            if (selectedBullet != null)
            {
                // Instantiate the bullet at the spawn point's position and rotation
                GameObject spawnedBullet = Instantiate(selectedBullet, spawnPoint.position, spawnPoint.rotation);
                Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    // Setze die Geschwindigkeit des Geschosses auf die Richtung zum PlayerObject
                    rb.velocity = spawnPoint.forward * selectedSpeed;
                }
                else
                {
                    Debug.LogWarning("Spawned bullet does not have a Rigidbody component.");
                }

                // Destroy the bullet after 5 seconds to avoid clutter
                Destroy(spawnedBullet, 5f);
            }
        }
    }

    /*private void FireBulletFromSpawnPoint(Transform spawnPoint)
    {
        if (playerTransform != null)
        {
            // Berechne den Vektor von der Drohne zum PlayerObject
            Vector3 toPlayer = playerTransform.position - spawnPoint.position;

            // �berpr�fe, ob die Drohne hinter dem PlayerObject ist
            float dotProduct = Vector3.Dot(playerTransform.forward, toPlayer.normalized);

            if (dotProduct >= 0)
            {
                // Die Drohne befindet sich hinter dem PlayerObject, also nicht schie�en
                return;
            }

            // Drehe den SpawnPoint, um auf das PlayerObject zu zeigen
            spawnPoint.rotation = Quaternion.LookRotation(toPlayer.normalized);

            // Berechne das Kreuzprodukt, um die relative Position zu bestimmen
            Vector3 crossProduct = Vector3.Cross(playerTransform.forward, toPlayer);

            // Leichte Drehung je nachdem, ob die Drohne links oder rechts vom PlayerObject ist
            float angle = 10f; // �ndere diesen Wert, um die Drehung zu justieren
            if (crossProduct.y > 0)
            {
                // Die Drohne befindet sich rechts vom PlayerObject, drehe sie leicht nach rechts
                spawnPoint.rotation *= Quaternion.Euler(0f, angle, 0f); // z.B. 10 Grad nach rechts
            }
            else
            {
                // Die Drohne befindet sich links vom PlayerObject, drehe sie leicht nach links
                spawnPoint.rotation *= Quaternion.Euler(0f, -angle, 0f); // z.B. 10 Grad nach links
            }

            GameObject selectedBullet = useSecondProjectile ? bullet2 : bullet1;
            float selectedSpeed = useSecondProjectile ? fireSpeed2 : fireSpeed1;

            if (selectedBullet != null)
            {
                // Instantiate the bullet at the spawn point's position and rotation
                GameObject spawnedBullet = Instantiate(selectedBullet, spawnPoint.position, spawnPoint.rotation);
                Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    // Setze die Geschwindigkeit des Geschosses in die Richtung des gedrehten SpawnPoints
                    rb.velocity = spawnPoint.forward * selectedSpeed;
                }
                else
                {
                    Debug.LogWarning("Spawned bullet does not have a Rigidbody component.");
                }

                // Destroy the bullet after 5 seconds to avoid clutter
                Destroy(spawnedBullet, 5f);
            }
        }
    }*/

    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward * 2);
                }
            }
        }
    }

    // Method to be called when the drone is hit
    public void OnDroneHit()
    {
        isHit = true;

        // Stop the shooting coroutine
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }

        DestroyAllProjectiles();
    }

    public void DestroyAllProjectiles()
    {
        // Finde alle existierenden Projektile in der Szene
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        // Zerst�re jedes Projektil
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        Debug.Log("All projectiles destroyed."); // Debug-Message zur �berpr�fung
    }
}

