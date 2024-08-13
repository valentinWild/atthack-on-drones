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
    public GameObject fluid; // Reference to the fluid inside the vial
    public GameObject mixingVial; 
    public AudioSource creation;
    public AudioSource fail;

    // Variables to track the last two particle collisions
    private string firstParticleTag = "";
    private string secondParticleTag = "";

    // Dictionary to define material combinations based on particle tags
    private Dictionary<string, Material> materialCombinations = new Dictionary<string, Material>();

    // Glow Effect Parameters
    public Material glowMaterial; // The material with a glow effect
    public float glowDuration = 0.5f; // Duration of the glow effect

    void Start()
    {
        // Define combinations of particles that result in different materials
        materialCombinations.Add("RedParticle+GreenParticle", healthPotion);
        materialCombinations.Add("YellowParticle+BlueParticle", shieldPotion);
        materialCombinations.Add("YellowParticle+GreenParticle", attackPotion); // 'Any' signifies it doesn't matter what the second particle is
        materialCombinations.Add("BlackParticle+Any", deathPotion);
       // materialCombinations.Add("OrangeParticle+Any", deathPotionMaterial);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("YellowParticle"))
        {
            fluid.GetComponent<Renderer>().material = healthPotion;
            Debug.Log("Yellow Liquid detected");
        }
        if (other.CompareTag("GreenParticle"))
        {
            fluid.GetComponent<Renderer>().material = deathPotion;
            Debug.Log("Green Liquid detected");
        }
        if (other.CompareTag("BlueParticle"))
        {
            fluid.GetComponent<Renderer>().material = shieldPotion;
            Debug.Log("Blue Liquid detected");
        }
    }

    private void CheckMixingResult()
    {
        // Check for exact combination or with 'Any' wildcard
        string combinationKey = firstParticleTag + "+" + secondParticleTag;
        string reverseCombinationKey = secondParticleTag + "+" + firstParticleTag;
        string wildcardKey1 = firstParticleTag + "+Any";
        string wildcardKey2 = "Any+" + secondParticleTag;

        if (materialCombinations.ContainsKey(combinationKey))
        {
            fluid.GetComponent<Renderer>().material = materialCombinations[combinationKey];
            Debug.Log(combinationKey + " = Material Applied");
            creation.Play();
        }
        else if (materialCombinations.ContainsKey(reverseCombinationKey))
        {
            fluid.GetComponent<Renderer>().material = materialCombinations[reverseCombinationKey];
            Debug.Log(reverseCombinationKey + " = Material Applied");
            creation.Play();
        }
        else if (materialCombinations.ContainsKey(wildcardKey1))
        {
            fluid.GetComponent<Renderer>().material = materialCombinations[wildcardKey1];
            Debug.Log(wildcardKey1 + " = Material Applied");
            creation.Play();
        }
        else if (materialCombinations.ContainsKey(wildcardKey2))
        {
            fluid.GetComponent<Renderer>().material = materialCombinations[wildcardKey2];
            Debug.Log(wildcardKey2 + " = Material Applied");
            creation.Play();
        }
        else
        {
            Debug.Log("No valid combination found");
            //fail.Play();
        }
    }

    private void ResetParticles()
    {
        firstParticleTag = "";
        secondParticleTag = "";
    }

    // Coroutine to handle the glow effect
    private IEnumerator GlowEffect()
    {
        Renderer renderer = mixingVial.GetComponent<Renderer>();
        Material originalMaterial = renderer.material;

        // Apply the glow material
        renderer.material = glowMaterial;

        // Wait for the duration of the glow effect
        yield return new WaitForSeconds(glowDuration);

        // Revert to the original material
        renderer.material = originalMaterial;
    }
}
