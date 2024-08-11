using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health; //current health of the player
    private float lerpTimer; // timer used for controlling the smooth transition of the health bar
    public float maxHealth = 100; //the maximum health the player can have
    public float chipSpeed = 2; //the speed at which the health bar transition occurs when health changes.
    public Image frontHealthBar; //UI image representing the current health, blue one
    public Image backHealthBar; //UI image, grey one, changing color depending on damage/restore

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //the health value stays within the range of 0 to maxHealth
        health = Mathf.Clamp(health, 0, maxHealth); 
        UpdateHealthUI();

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(Random.Range(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestoreHealth(Random.Range(5, 10));
        }

    }

    public void UpdateHealthUI()
    {
        //Store the current fill amounts of the front and back health bars
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount; 
        float fillB = backHealthBar.fillAmount;
        //calculates the fraction of health relative to the maximum health, this fraction is used to determine how full the health bars should be
        float hFraction = health / maxHealth;


        //wenn der hintere Balken größer ist als der vordere, zeigt das, dass der Spieler gerade Leben verloren hat
        //der hintere Balken beginnt dann langsam zu schrumpfen und gleicht sich dem vorderen Balken an
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplate = lerpTimer / chipSpeed;
            percentComplate = percentComplate * percentComplate;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplate);
        }

        //der hintere Balkon reagiert auf die Heilung und springt direkt zur neuen höheren Gesundheit
        //der vordere Balkon fängt an langsam zu wachsen
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplate = lerpTimer / chipSpeed;
            percentComplate = percentComplate * percentComplate;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplate);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    public void IncreaseHealth(int level)
    {
        maxHealth += (health * 0.01f)* ((100 - level) * 0.1f);
        health = maxHealth;
    }
}