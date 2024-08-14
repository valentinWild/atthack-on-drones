using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingVial : MonoBehaviour
{
    // Set of Colors to be mixed
    public Material attackPotion;
    public Material healthPotion;
    public Material shieldPotion;
    public Material deathPotion;
    public Material water;
    public GameObject fluid; // Reference to the fluid inside the vial
    public GameObject mixingVial;
    public AudioSource creation;
    public AudioSource fail;

    // Glow Effect Parameters
    public Material glowMaterial; // The material with a glow effect
    public float glowDuration = 0.5f; // Duration of the glow effect

    // List to track poured liquids
    private List<string> pouredLiquids = new List<string>();

    // HashSet to avoid duplicate triggers from the same liquid
    private HashSet<string> detectedLiquids = new HashSet<string>();

    // List of valid liquid tags
    private HashSet<string> validLiquidTags = new HashSet<string> { "YellowParticle", "GreenParticle", "BlueParticle", "RedParticle", "BlackParticle" };


    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("YellowParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion;
            Debug.Log("Yellow Liquid detected");
        }
        if (other.CompareTag("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = attackPotion;
            Debug.Log("Green Liquid detected");
        }
        if (other.CompareTag("BlueParticle"))
        {
            fluid.GetComponent<Renderer>().material = shieldPotion;
            Debug.Log("Blue Liquid detected");
        }
        if (other.CompareTag("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion;
            Debug.Log("Red Liquid detected");
        }
        if (other.CompareTag("BlackParticle"))
        {
            fluid.GetComponent<Renderer>().material = deathPotion;
            Debug.Log("Black Liquid detected");
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        // Add the liquid to the list
        if (validLiquidTags.Contains(other.tag) && !pouredLiquids.Contains(other.tag))
        {
            detectedLiquids.Add(other.tag); // Add the tag to the set to avoid reprocessing
            pouredLiquids.Add(other.tag);
            Debug.Log($"{other.tag} detected");

            // If two liquids have been added, mix them
            if (pouredLiquids.Count == 2)
            {
                MixLiquids();
                // Clear the list for the next mixing
                pouredLiquids.Clear();
                detectedLiquids.Clear(); // Reset detection for the next round of mixing
            }
        }
    }

    private void MixLiquids()
    {
        // Example: Mixing logic based on two liquids
        if (pouredLiquids.Contains("YellowParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = attackPotion; // New combination material
            Debug.Log("Created a new material by mixing Yellow and Red");
            creation.Play();
        }
        else if (pouredLiquids.Contains("YellowParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = water; // Another combination
            Debug.Log("Created a new material by mixing Yellow and Green");
            creation.Play();
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion; // Another combination
            Debug.Log("Created a new material by mixing Blue and Red");
            creation.Play();
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = shieldPotion; // Another combination
            Debug.Log("Created a new material by mixing Blue and Green");
            creation.Play();
        }
        else if (pouredLiquids.Contains("BlackParticle"))
        {
            fluid.GetComponent<Renderer>().material = deathPotion; // Death potion overrides others
            Debug.Log("Black liquid mixed, created death potion");
        }
        else
        {
            // Default case for invalid combinations
            Debug.Log("Failed to mix the liquids");
            fail.Play(); // Play failure sound
        }
    }

}
