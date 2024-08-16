using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int level;
    private int _completedChallenges;
    public int completedChallenges
    {
        get { return _completedChallenges; }
        set
        {
            _completedChallenges = Mathf.Clamp(value, 0, (int)requiredXp);
            currentXp = _completedChallenges; 
            UpdateXpUI(); 
        }
    }
    public float currentXp;
    public const float requiredXp = 4; // Immer 4 Herausforderungen für den Levelaufstieg

    private float lerpTimer;
    private float delayTimer;

    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    void Start()
    {
        frontXpBar.fillAmount = currentXp / requiredXp;
        backXpBar.fillAmount = currentXp / requiredXp;
        levelText.text = level.ToString();
    }

    void Update()
    {
        // Tastatureingaben für Testzwecke
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            completedChallenges++; // Erhöht die Anzahl der abgeschlossenen Herausforderungen
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            completedChallenges--; // Verringert die Anzahl der abgeschlossenen Herausforderungen
        }

        // Sicherstellen, dass completedChallenges innerhalb der Grenzen bleibt
        completedChallenges = Mathf.Clamp(completedChallenges, 0, (int)requiredXp);

        if (currentXp >= requiredXp)
        {
            LevelUp();
        }
    }

    /*void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = frontXpBar.fillAmount;

        // Schritt 1: BackXpBar sofort auf den neuen Wert setzen
        backXpBar.fillAmount = xpFraction;

        // Schritt 2: FrontXpBar nach einer kurzen Verzögerung animieren
        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > 0.5f)  // Kürzere Verzögerung als vorher, z.B. 0.5 Sekunden
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 0.3f; // Schneller animieren, indem der Wert verkleinert wird
                frontXpBar.fillAmount = Mathf.Lerp(FXP, xpFraction, percentComplete);

                if (frontXpBar.fillAmount >= xpFraction)
                {
                    lerpTimer = 0f; // Reset des Timers nach Abschluss der Animation
                    delayTimer = 0f; // Reset der Verzögerung
                }
            }
        }

        xpText.text = $"{currentXp}/{requiredXp} Challenges";
    }*/


    void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = frontXpBar.fillAmount;

        // Schritt 1: BackXpBar sofort auf den neuen Wert setzen
        if (backXpBar.fillAmount < xpFraction)
        {
            backXpBar.fillAmount = Mathf.MoveTowards(backXpBar.fillAmount, xpFraction, Time.deltaTime);
        }

        // Schritt 2: FrontXpBar nach dem vollständigen Laden der BackXpBar animieren
        if (backXpBar.fillAmount >= xpFraction)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > 0.5f)  // Kürzere Verzögerung als vorher, z.B. 0.5 Sekunden
            {
                lerpTimer += Time.deltaTime;
                frontXpBar.fillAmount = Mathf.MoveTowards(FXP, xpFraction, Time.deltaTime);

                // Sicherstellen, dass die FrontXPBar am Ende exakt den Zielwert erreicht
                if (Mathf.Abs(frontXpBar.fillAmount - xpFraction) < 0.01f)
                {
                    frontXpBar.fillAmount = xpFraction;
                    lerpTimer = 0f; // Reset des Timers nach Abschluss der Animation
                    delayTimer = 0f; // Reset der Verzögerung

                    // Wenn die XP-Bar vollständig geladen ist (100%)
                    if (frontXpBar.fillAmount >= 1.0f && currentXp >= requiredXp)
                    {
                        LevelUp(); // Nächstes Level starten und XP-Bars zurücksetzen
                    }
                }
            }
        }

        xpText.text = $"{completedChallenges}/{requiredXp}";
    }

    public void LevelUp()
    {
        level++;
        completedChallenges = 0; // Setzt completedChallenges zurück, was auch currentXp auf 0 setzt
        currentXp = 0f; // Reset der aktuellen XP
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        levelText.text = level.ToString();
    }

    /*public void LevelUp()
    {
        level++;
        completedChallenges = 0; // Setzt completedChallenges zurück, was auch currentXp auf 0 setzt
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        levelText.text = level.ToString();
    }*/
}