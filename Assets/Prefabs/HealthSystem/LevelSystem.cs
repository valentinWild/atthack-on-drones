using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int level;
    public int completedChallenges;
    private const int totalChallenges = 4;

    private float lerpTimer;
    private float delayTimer;
    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;

    public TextMeshProUGUI levelText;
    //public TextMeshProUGUI xpText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateXpUI();
        levelText.text = level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();

        // Tastatureingaben abfragen
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetChallengeProgress(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetChallengeProgress(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetChallengeProgress(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetChallengeProgress(4);
        }

        if (completedChallenges >= totalChallenges)
        {
            LevelUp();
        }
    }

    public void UpdateXpUI()
    {
        float xpFraction = (float)completedChallenges / totalChallenges;
        float FXP = frontXpBar.fillAmount;

        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXpBar.fillAmount = xpFraction;
            if (delayTimer > 3)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 1;
                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
            }
        }

        //xpText.text = $"{completedChallenges}/{totalChallenges} Challenges";
    }

    public void SetChallengeProgress(int progress)
    {
        if (progress >= 1 && progress <= totalChallenges)
        {
            completedChallenges = progress;
            lerpTimer = 0f;
            delayTimer = 0f;
        }
    }

    public void LevelUp()
    {
        level++;
        completedChallenges = 0;
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        levelText.text = level.ToString();  // Nur die Zahl anzeigen
    }
}