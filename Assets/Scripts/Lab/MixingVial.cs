using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingVial : MonoBehaviour
{
    private Dictionary<string, bool> particlesInVial = new Dictionary<string, bool>();

    // Set of Colors to be mixed
    public Material red;
    public Material yellow;
    public Material green;
    public Material blue;
    public Material black;
    public GameObject fluid; // Reference to the fluid inside the vial

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

    private void OnTriggerEnter(Collider other)
    {
        string particleTag = other.tag;

        if (particlesInVial.ContainsKey(particleTag))
        {
            particlesInVial[particleTag] = true;
            CheckMixingResult();
        }
    }

    private void CheckMixingResult()
    {
        if (particlesInVial["RedParticle"])
        {
            fluid.GetComponent<Renderer>().material = red;
        }
        if (particlesInVial["BlackParticle"])
        {
            fluid.GetComponent<Renderer>().material = black;
        }
        if (particlesInVial["YellowParticle"])
        {
            fluid.GetComponent<Renderer>().material = yellow;
        }
        if (particlesInVial["GreenParticle"])
        {
            fluid.GetComponent<Renderer>().material = green;
        }
        if (particlesInVial["BlueParticle"])
        {
            fluid.GetComponent<Renderer>().material = blue;
        }
    }
}
