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
    public Material speedPotion;
    public Material endPotion;
    public Material water; // Default material after reset
    public Material whiteLiquid;

    // Mixing Vial
    public GameObject fluid; // Reference to the fluid inside the vial
    public GameObject mixingVial;

    // Audio Sources
    public AudioSource creation;
    public AudioSource fail;
    public AudioSource fluidAdded;
    public AudioSource backgroundMusic;
    public AudioSource winningMusic;

    // Glow Effect Parameters
    public Material glowMaterial; // The material with a glow effect
    public float glowDuration = 0.5f; // Duration of the glow effect

    // winning effects 
    public ParticleSystem sparkle;
    public ParticleSystem confetti;
    public ParticleSystem shine;

    // UI Elements for Speech Bubble
    public GameObject winDisplay; // The speech bubble GameObject
    public GameObject uIDisplay;
    public TextMeshProUGUI potionText; // The TextMeshPro component for the speech bubble text
    public TextMeshProUGUI tutorialText;
    public float potionMessageDuration = 3.0f; // Duration the speech bubble stays visible

    // Tutorial Text
    //public string tutorialMessage = "Combine two vials by pouring or smashing them to create a new potion!"; // Default tutorial text

    // Potion Reset Parameters
    public float potionResetDelay = 3.0f; // Time to reset after potion creation

    // List to track poured liquids
    private List<string> pouredLiquids = new List<string>();

    // HashSet to avoid duplicate triggers from the same liquid
    private HashSet<string> detectedLiquids = new HashSet<string>();

    // List of valid liquid tags
    private HashSet<string> validLiquidTags = new HashSet<string> { "YellowParticle", "GreenParticle", "BlueParticle", "RedParticle", "BlackParticle", "GoldParticle" };

    private Renderer vialRenderer;
    private Material originalVialMaterial;
    private bool showingPotionMessage = false; // Track whether a potion message is being shown
    private bool potionCreated = false; 

    private void Start()
    {
        // Cache the renderer and original material of the vial
        vialRenderer = mixingVial.GetComponent<Renderer>();
        originalVialMaterial = vialRenderer.material;

        // set correct UI display at start
        winDisplay.gameObject.SetActive(false);
        uIDisplay.gameObject.SetActive(true);

        // set correct text at start
        tutorialText.gameObject.SetActive(true); // Ensure the tutorial text is visible at the start
        potionText.gameObject.SetActive(false);

        // disable particle effects at start
        sparkle.gameObject.SetActive(false);
        sparkle.Stop();
        shine.gameObject.SetActive(false);
        shine.Stop();
        confetti.Stop();  
        confetti.gameObject.SetActive(false);   

        // Set the default tutorial message
        //ShowTutorialMessage();

        // Set the fluid to its default material (e.g., water)
        fluid.GetComponent<Renderer>().material = whiteLiquid;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Fluid poured into vial: " + other.gameObject.name);
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
            } else if (pouredLiquids.Contains("GoldParticle")) { MixLiquids();} // mix instantly if there is gold liquid
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
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("YellowParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = water; // Another combination
            potionName = "Water";
            Debug.Log("Created a new material by mixing Yellow and Green");
            creation.Play();
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion; // Another combination
            potionName = "Health Potion";
            Debug.Log("Created a new material by mixing Blue and Red");
            creation.Play();
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("BlueParticle") && pouredLiquids.Contains("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = shieldPotion; // Another combination
            potionName = "Shield Potion";
            Debug.Log("Created a new material by mixing Blue and Green");
            creation.Play();
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("BlackParticle") && pouredLiquids.Contains("RedParticle"))
        {
            fluid.GetComponent<Renderer>().material = deathPotion; // Death potion overrides others
            potionName = "Death Potion";
            Debug.Log("Created a new material by mixing Black and Red");
            creation.Play();
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("BlackParticle") && pouredLiquids.Contains("BlueParticle"))
        {
            fluid.GetComponent<Renderer>().material = speedPotion;
            potionName = "Speed Potion";
            Debug.Log("Created a new material by mixing Black and Blue");
            creation.Play();
            potionCreated = true;
        }
        else if (pouredLiquids.Contains("GoldParticle"))
        {
            potionCreated = true;
            fluid.GetComponent<Renderer>().material = endPotion;
            potionName = "End Potion";
            Debug.Log("Created a new material by adding Gold");
            // set sound effects and music
            backgroundMusic.Stop();
            creation.Play();
            winningMusic.Play();
            // set winning display
            uIDisplay.gameObject.SetActive(false);
            winDisplay.gameObject.SetActive(true);
            // activate particle systems
            sparkle.gameObject.SetActive(true);
            sparkle.Play();
            shine.gameObject.SetActive(true);
            shine.Play();   
            confetti.gameObject.SetActive(true);
            confetti.Play();

            if (GameSyncManager.Instance)
            {
                GameSyncManager.Instance.RpcIncreaseLevel();
            }
        }
        else
        {
            // Default case for invalid combinations
            Debug.Log("Failed to mix the liquids");
            fail.Play(); // Play failure sound
            potionCreated = false;
            ShowPotionMessage("Unvalid combination");
            StartCoroutine(ResetPotion());
        }

        if (potionCreated)
        {
            if (GameSyncManager.Instance)
            {
                GameSyncManager.Instance.RpcSetRunnerPotion(potionName);
            }
            // Show the speech bubble with the created potion name
            ShowPotionMessage("Potion created: " + potionName);

            // Reset potion creation after a delay
            StartCoroutine(ResetPotion());
        }
    }

    private void ShowPotionMessage(string message)
    {
        potionText.text = message; // Update the text
        tutorialText.gameObject.SetActive(false); // Hide the tutorial text
        potionText.gameObject.SetActive(true);
        //showingPotionMessage = true; // Indicate that we're showing a potion message
        //speechBubble.SetActive(true); // Show the speech bubble
        StartCoroutine(HidePotionMessageAfterDelay()); // Hide after delay
    }

    private IEnumerator HidePotionMessageAfterDelay()
    {
        yield return new WaitForSeconds(potionMessageDuration);
        //speechBubble.SetActive(false); // Hide the speech bubble
        //showingPotionMessage = false; // Reset the flag
        potionText.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(true);

        // Revert to the tutorial message
        // ShowTutorialMessage();
    }

    /*
    private void ShowTutorialMessage()
    {
        if (!showingPotionMessage) // Only show the tutorial if we're not showing a potion message
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialMessage; // Set the tutorial text
            //speechBubble.SetActive(true); // Ensure the speech bubble is visible
        }
    } */

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