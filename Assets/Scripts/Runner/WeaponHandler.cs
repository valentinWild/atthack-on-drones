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
    public float defaultReloadTime = 1.5f;
    public float reloadTime = 1.5f;
    public int shotsPerReloadPeriod = 3;
    private int shotCounter = 0;
    private bool weaponLoaded = true;

    public AudioClip shootSound;
    private AudioSource audioSource;

    private void OnEnable()
    {
        shootAction.action.performed += OnShoot;
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        shootAction.action.performed -= OnShoot;
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    private void Start()
    {
        // AudioSource-Komponente abrufen
        audioSource = GetComponent<AudioSource>();
        reloadTime = defaultReloadTime;

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
            shotCounter++;
            if (shotCounter >= shotsPerReloadPeriod)
            {
                StartCoroutine(ReloadWeapon());
            }
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Attack Potion")
        {
            StartCoroutine(SetTemporarlyReloadTime(10f));
        }
    }

    private IEnumerator ReloadWeapon()
    {
        weaponLoaded = false;
        yield return new WaitForSeconds(reloadTime);
        weaponLoaded = true;
    }

    private IEnumerator SetTemporarlyReloadTime(float duration){
        reloadTime = 0f;
        yield return new WaitForSeconds(duration);
        reloadTime = defaultReloadTime;
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
