using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    private float health; //current health of the player
    private float lerpTimer; // timer used for controlling the smooth transition of the health bar
    public float maxHealth = 100; //the maximum health the player can have
    public float chipSpeed = 1; //the speed at which the health bar transition occurs when health changes.
    public Image frontHealthBar; //UI image representing the current health, blue one
    public Image backHealthBar; //UI image, grey one, changing color depending on damage/restore
    public TextMeshProUGUI healthText;

    [SerializeField]
    private UnityEvent<float> healthUpdateEvent;

    public TextMeshProUGUI friendCounterText;
    public TextMeshProUGUI enemyCounterText;

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

        UpdateFriendCounter(DroneCounter.GetCollectedCounter());
        UpdateEnemyCounter(DroneCounter.GetExplosionCounter());

    }

    public void UpdateHealthUI()
    {
        //Store the current fill amounts of the front and back health bars
        //Debug.Log(health);
        float fillF = frontHealthBar.fillAmount; 
        float fillB = backHealthBar.fillAmount;
        //calculates the fraction of health relative to the maximum health, this fraction is used to determine how full the health bars should be
        float hFraction = health / maxHealth;


        //wenn der hintere Balken gr��er ist als der vordere, zeigt das, dass der Spieler gerade Leben verloren hat
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

        //der hintere Balkon reagiert auf die Heilung und springt direkt zur neuen h�heren Gesundheit
        //der vordere Balkon f�ngt an langsam zu wachsen
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplate = lerpTimer / chipSpeed;
            percentComplate = percentComplate * percentComplate;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplate);
        }

        healthText.text = Mathf.Round(health).ToString();
    }

    public void TakeDamage(float damage)
    {
        var newHealth = health - damage;
        UpdateHealth(newHealth);
        lerpTimer = 0f;
    }

    public void RestoreHealth(float healAmount)
    {
        var newHealth = health + healAmount;
        UpdateHealth(newHealth);
        lerpTimer = 0f;
    }

    private void UpdateHealth(float newHealth)
    {
        health = newHealth;
        //healthUpdateEvent.Invoke(health);
        if (GameSyncManager.Instance) {
            GameSyncManager.Instance.RpcUpdateRunnerHealth(health);
        }
        //Debug.Log("Update Health, new Value: " + health);
    }

    public void IncreaseHealth(int level)
    {
        maxHealth += (health * 0.01f)* ((100 - level) * 0.1f);
        UpdateHealth(maxHealth);
    }

    public void UpdateFriendCounter(int friendCount)
    {
        //Debug.Log("Friend Counter Updated: " + friendCount);
        friendCounterText.text = "Hints: " + friendCount.ToString();
    }

    public void UpdateEnemyCounter(int enemyCount)
    {
        //Debug.Log("Enemy Counter Updated: " + enemyCount);
        enemyCounterText.text = "Enemies: " + enemyCount.ToString();
    }
}