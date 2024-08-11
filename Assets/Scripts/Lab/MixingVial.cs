using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingVial : MonoBehaviour
{
    private Dictionary<string, bool> particlesInVial = new Dictionary<string, bool>();

    // Set of Colors to be mixed
    public Material healthPotionMaterial;
    public Material deathPotionMaterial;
    public GameObject mixingVial;

    // Start is called before the first frame update
    void Start()
    {
        particlesInVial.Add("RedParticle", false);
        particlesInVial.Add("OrangeParticle", false);
        particlesInVial.Add("YellowParticle", false);
        particlesInVial.Add("GreenParticle", false);
        particlesInVial.Add("BlueParticle", false);
        particlesInVial.Add("PurpleParticle", false);
        particlesInVial.Add("BlackParticle", false);
        particlesInVial.Add("WhiteParticle", false);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            string particleTag = other.tag;

            if (particlesInVial.ContainsKey(particleTag))
            {
                particlesInVial[particleTag] = true;
                CheckMixingResult();
            }
        }
    }

    private void CheckMixingResult()
    {
        if (particlesInVial["RedParticle"] && particlesInVial["WhiteParticle"])
        {
            mixingVial.GetComponent<Renderer>().material = healthPotionMaterial;
        }
        if (particlesInVial["BlackParticle"] && particlesInVial["WhiteParticle"])
        {
            mixingVial.GetComponent<Renderer>().material = deathPotionMaterial;
        }
    }

}
