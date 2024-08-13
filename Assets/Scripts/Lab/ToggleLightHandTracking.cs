using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightHandTracking : MonoBehaviour
{
    public Light lightOrb; // Reference to the Light component on the orb
    public ParticleSystem orbParticles; // Reference to the Particle System on the orb
    public OrbManager orbManager; // Reference to the OrbManager

    public int orbIndex; // Index of this orb in the OrbManager

    private bool isLightOn = false;
    private bool isDebouncing = false; // To track if we're in the debounce period
    public float debounceTime = 0.5f; // Time in seconds to wait before allowing another toggle



    private void Start()
    {
        // turn off light and particle system initially
        lightOrb = gameObject.GetComponent<Light>();
        lightOrb.enabled = false;
        orbParticles = gameObject.GetComponent<ParticleSystem>();
        orbParticles.Stop();
    }

    // Detect when the hand enters the collider
    private void OnTriggerEnter(Collider other)
    {

        if (!isDebouncing) // Only toggle if we're not in the debounce period
        {
            ToggleLightAndParticles();
            StartCoroutine(Debounce()); // Start the debounce timer
        }

    }

    private void ToggleLightAndParticles()
    {
        // Toggle light and particles
        isLightOn = !isLightOn;
        lightOrb.enabled = isLightOn;

        if (isLightOn)
        {
            orbParticles.Play();
        }
        else
        {
            orbParticles.Stop();
        }

        // Update the orb state in the OrbManager
        if (orbManager != null)
        {
            orbManager.UpdateOrbState(orbIndex, isLightOn);
        }
    }

    public void ResetLightAndParticles()
    {
        isLightOn = false;
        lightOrb.enabled = false;
        orbParticles.Stop();
    }

    private IEnumerator Debounce()
    {
        isDebouncing = true; // Set debouncing to true to prevent further toggling
        yield return new WaitForSeconds(debounceTime); // Wait for the debounce time
        isDebouncing = false; // Reset debouncing to allow toggling again
    }

}