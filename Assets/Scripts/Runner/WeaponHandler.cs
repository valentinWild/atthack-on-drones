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

    private void OnEnable()
    {
        shootAction.action.performed += OnShoot;
    }

    private void OnDisable()
    {
        shootAction.action.performed -= OnShoot;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("On Shoot triggered");
        FireBullet();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
