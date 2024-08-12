/*using System.Collections;
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

    
    private void Start()
    {
        //Shooting starts 5 seconds later
        Invoke("StartFiring", 5f);

    }

    private void StartFiring()
        {
            StartCoroutine(FireBullets());
        }
     

    private IEnumerator FireBullets()
    {
        while (true)
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
            FireBulletFromCurrentSpawnPoint();
            yield return new WaitForSeconds(burstInterval);
        }
        // Move to the next spawn point after completing a burst
        nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Length;
        yield return new WaitForSeconds(fireInterval - 3 * burstInterval);
    }

    private void FireOnce()
    {
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
        GameObject selectedBullet = useSecondProjectile ? bullet2 : bullet1;
        float selectedSpeed = useSecondProjectile ? fireSpeed2 : fireSpeed1;

        if (selectedBullet != null)
        {
            // Instantiate the bullet at the spawn point's position and rotation
            GameObject spawnedBullet = Instantiate(selectedBullet, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply velocity in the local forward direction of the spawn point
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
}
*/

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
    public string playerTag = "Player"; // Tag used to identify the player

    private int nextSpawnPointIndex = 0;
    private bool isHit = false;

    private Coroutine fireCoroutine;
    private Transform playerTransform;

    private void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found in the scene.");
        }

        // Shooting starts after 8 seconds
        Invoke("StartFiring", 8f);
    }
    private void StartFiring()
    {
        if (playerTransform != null)
        {
            fireCoroutine = StartCoroutine(FireBullets());
        }
        else
        {
            Debug.LogWarning("Cannot start firing because playerTransform is null.");
        }
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
                AlignWithPlayer(spawnPoint);
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
            AlignWithPlayer(spawnPoints[nextSpawnPointIndex]);
            FireBulletFromSpawnPoint(spawnPoints[nextSpawnPointIndex]);
        }
    }

    private void FireBulletFromSpawnPoint(Transform spawnPoint)
    {
        GameObject selectedBullet = useSecondProjectile ? bullet2 : bullet1;
        float selectedSpeed = useSecondProjectile ? fireSpeed2 : fireSpeed1;

        if (selectedBullet != null)
        {
            // Instantiate the bullet at the spawn point's position and rotation
            GameObject spawnedBullet = Instantiate(selectedBullet, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply velocity in the local forward direction of the spawn point
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

    private void AlignWithPlayer(Transform spawnPoint)
    {
        if (playerTransform != null)
        {
            // Calculate the direction from the spawn point to the player
            Vector3 directionToPlayer = (playerTransform.position - spawnPoint.position).normalized;

            // Add a random spread angle to the direction
            float spreadAngle = 5f; // Adjust this value to control the spread
            directionToPlayer = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0) * directionToPlayer;

            // Align the spawn point's forward direction with the direction to the player
            spawnPoint.forward = directionToPlayer;
        }
    }

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

        // Zerstöre jedes Projektil
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        Debug.Log("All projectiles destroyed."); // Debug-Message zur Überprüfung
    }
}
