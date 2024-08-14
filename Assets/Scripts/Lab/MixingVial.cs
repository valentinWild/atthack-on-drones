using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro

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

    // UI Elements for Speech Bubble
    public GameObject speechBubble; // The speech bubble GameObject
    public TextMeshProUGUI speechText; // The TextMeshPro component for the speech bubble text
    public float speechBubbleDuration = 2.0f; // Duration the speech bubble stays visible

    // List to track poured liquids
    private List<string> pouredLiquids = new List<string>();

    // HashSet to avoid duplicate triggers from the same liquid
    private HashSet<string> detectedLiquids = new HashSet<string>();

    // List of valid liquid tags
    private HashSet<string> validLiquidTags = new HashSet<string> { "YellowParticle", "GreenParticle", "BlueParticle", "RedParticle", "BlackParticle" };

    private Renderer vialRenderer;
    private Material originalVialMaterial;

    private void Start()
    {
        // Cache the renderer and original material of the vial
        vialRenderer = mixingVial.GetComponent<Renderer>();
        originalVialMaterial = vialRenderer.material;

        // Initially hide the speech bubble
        speechBubble.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add the liquid to the list
        if (validLiquidTags.Contains(other.tag) && !pouredLiquids.Contains(other.tag))
        {
            detectedLiquids.Add(other.tag); // Add the tag to the set to avoid reprocessing
            pouredLiquids.Add(other.tag);
            Debug.Log($"{other.tag} detected");

            // Trigger the glow effect on the vial
            StartCoroutine(GlowVial());

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

    private IEnumerator GlowVial()
    {
        // Set the material to the glow material
        vialRenderer.material = glowMaterial;

        // Wait for the duration of the glow
        yield return new WaitForSeconds(glowDuration);

        // Revert the material back to the original one
        vialRenderer.material = originalVialMaterial;
    }

    private void MixLiquids()
    {
        string potionName = "";
        // Example: Mixing logic based on two liquids
        if (pouredLiquids.Contains("YellowParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = attackPotion; // New combination material
            potionName = "Attack Potion";
            Debug.Log("Created a new material by mixing Yellow and Red");
            creation.Play();
        }
        else if (pouredLiquids.Contains("YellowParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = water; // Another combination
            potionName = "Water";
            Debug.Log("Created a new material by mixing Yellow and Green");
            creation.Play();
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion; // Another combination
            potionName = "Health Potion";
            Debug.Log("Created a new material by mixing Blue and Red");
            creation.Play();
            
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = shieldPotion; // Another combination
            potionName = "Shield Potion";
            Debug.Log("Created a new material by mixing Blue and Green");
            creation.Play();
        }
        else if (pouredLiquids.Contains("BlackParticle"))
        {
            fluid.GetComponent<Renderer>().material = deathPotion; // Death potion overrides others
            potionName = "Death Potion";
            Debug.Log("Black liquid mixed, created death potion");
        }
        else
        {
            // Default case for invalid combinations
            Debug.Log("Failed to mix the liquids");
            fail.Play(); // Play failure sound
            return; // Exit early, no potion was created
        }
        // Show the speech bubble with the created potion name
        ShowSpeechBubble("Potion created: " + potionName);

        // Send potion to runner
        if (GameSyncManager.Instance != null)
        {
            GameSyncManager.Instance.RpcSetRunnerPotion(potionName);
            Debug.Log("Sent potion to runner");
        }   
    }

    private void ShowSpeechBubble(string message)
    {
        speechText.text = message; // Update the text
        speechBubble.SetActive(true); // Show the speech bubble
        StartCoroutine(HideSpeechBubbleAfterDelay()); // Hide after delay
    }

    private IEnumerator HideSpeechBubbleAfterDelay()
    {
        yield return new WaitForSeconds(speechBubbleDuration);
        speechBubble.SetActive(false); // Hide the speech bubble
    }

}
