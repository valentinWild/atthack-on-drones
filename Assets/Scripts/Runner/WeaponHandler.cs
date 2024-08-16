using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponHandler : MonoBehaviour
{

    public InputActionReference shootAction;
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 50f;
    public float reloadTime = 3f;
    private bool weaponLoaded = true;

    public AudioClip shootSound;
    private AudioSource audioSource;

    private void OnEnable()
    {
        shootAction.action.performed += OnShoot;
    }

    private void OnDisable()
    {
        shootAction.action.performed -= OnShoot;
    }

    private void Start()
    {
        // AudioSource-Komponente abrufen
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource component found on this GameObject.");
        }
    }


    private void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("On Shoot triggered");
        if(weaponLoaded){
            FireBullet();
            StartCoroutine(ReloadWeapon());
        }
    }

    private IEnumerator ReloadWeapon()
    {
        weaponLoaded = false;
        yield return new WaitForSeconds(reloadTime);
        weaponLoaded = true;
    }

    public void FireBullet() 
    {
        GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        spawnedBullet.transform.position = spawnPoint.position;
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.forward * fireSpeed;
        }
        Destroy(spawnedBullet, 5);

        // Schuss-Sound abspielen
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound); // Spielt den zugewiesenen AudioClip ab
        }
        else
        {
            if (shootSound == null)
            {
                Debug.LogWarning("No AudioClip assigned for shooting sound.");
            }
            else
            {
                Debug.LogWarning("AudioSource is not assigned or missing.");
            }
        }
    }
}
