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
    public Material water; // Default material after reset
    public Material whiteLiquid;

    // Mixing Vial
    public GameObject fluid; // Reference to the fluid inside the vial
    public GameObject mixingVial;

    // Audio Sources
    public AudioSource creation;
    public AudioSource fail;
    public AudioSource fluidAdded;

    // Glow Effect Parameters
    public Material glowMaterial; // The material with a glow effect
    public float glowDuration = 0.5f; // Duration of the glow effect

    // UI Elements for Speech Bubble
    public GameObject speechBubble; // The speech bubble GameObject
    public TextMeshProUGUI speechText; // The TextMeshPro component for the speech bubble text
    public TextMeshProUGUI tutorialText;
    public float speechBubbleDuration = 2.0f; // Duration the speech bubble stays visible

    // Tutorial Text
    public string tutorialMessage = "Combine two vials by pouring or smashing them to create a new potion!"; // Default tutorial text

    // Potion Reset Parameters
    public float potionResetDelay = 3.0f; // Time to reset after potion creation

    // List to track poured liquids
    private List<string> pouredLiquids = new List<string>();

    // HashSet to avoid duplicate triggers from the same liquid
    private HashSet<string> detectedLiquids = new HashSet<string>();

    // List of valid liquid tags
    private HashSet<string> validLiquidTags = new HashSet<string> { "YellowParticle", "GreenParticle", "BlueParticle", "RedParticle", "BlackParticle" };

    private Renderer vialRenderer;
    private Material originalVialMaterial;
    private bool showingPotionMessage = false; // Track whether a potion message is being shown

    private void Start()
    {
        // Cache the renderer and original material of the vial
        vialRenderer = mixingVial.GetComponent<Renderer>();
        originalVialMaterial = vialRenderer.material;

        // Initially hide the speech bubble
        speechBubble.SetActive(true);
        tutorialText.gameObject.SetActive(true); // Ensure the tutorial text is visible at the start


        // Set the default tutorial message
        ShowTutorialMessage();

        // Set the fluid to its default material (e.g., water)
        fluid.GetComponent<Renderer>().material = whiteLiquid;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add the liquid to the list
        if (validLiquidTags.Contains(other.tag) && !pouredLiquids.Contains(other.tag))
        {
            detectedLiquids.Add(other.tag); // Add the tag to the set to avoid reprocessing
            pouredLiquids.Add(other.tag);
            Debug.Log($"{other.tag} detected");
            fluidAdded.Play();
            // Trigger the glow effect on the vial
            StartCoroutine(GlowVial());
            

            // If two liquids have been added, mix them
            if (pouredLiquids.Count == 2)
            {
                MixLiquids();
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
        bool potionCreated = true;

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
            creation.Play();
        }
        else
        {
            // Default case for invalid combinations
            Debug.Log("Failed to mix the liquids");
            fail.Play(); // Play failure sound
            potionCreated = false;
        }

        if (potionCreated)
        {
            GameSyncManager.Instance.RpcSetRunnerPotion(potionName);
            // Show the speech bubble with the created potion name
            ShowPotionMessage("Potion created: " + potionName);

            // Reset potion creation after a delay
            StartCoroutine(ResetPotion());
        }
    }

    private void ShowPotionMessage(string message)
    {
        showingPotionMessage = true; // Indicate that we're showing a potion message
        tutorialText.gameObject.SetActive(false); // Hide the tutorial text
        speechText.text = message; // Update the text
        speechBubble.SetActive(true); // Show the speech bubble
        StartCoroutine(HidePotionMessageAfterDelay()); // Hide after delay
    }

    private IEnumerator HidePotionMessageAfterDelay()
    {
        yield return new WaitForSeconds(speechBubbleDuration);
        speechBubble.SetActive(false); // Hide the speech bubble
        showingPotionMessage = false; // Reset the flag

        // Revert to the tutorial message
        ShowTutorialMessage();
    }

    private void ShowTutorialMessage()
    {
        if (!showingPotionMessage) // Only show the tutorial if we're not showing a potion message
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialMessage; // Set the tutorial text
            //speechBubble.SetActive(true); // Ensure the speech bubble is visible
        }
    }

    private IEnumerator ResetPotion()
    {
        // Wait for the potion reset delay
        yield return new WaitForSeconds(potionResetDelay);

        // Reset the fluid material to the default (e.g., water)
        fluid.GetComponent<Renderer>().material = whiteLiquid;

        // Clear the lists so the next potion requires two new liquids
        pouredLiquids.Clear();
        detectedLiquids.Clear();

        Debug.Log("Potion has been reset, ready for a new combination.");
    }
}