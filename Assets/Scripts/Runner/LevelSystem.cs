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
    public const float requiredXp = 4; 
    private float lerpTimer;
    private float delayTimer;

    [Header("Audio")]
    public AudioClip levelUpSound;

    private AudioSource audioSource;

    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnDecodedHintsChanged += OnDecodedHintsChanged;
            //GameSyncManager.OnCurrentLevelChanged += OnCurrentLevelChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnDecodedHintsChanged -= OnDecodedHintsChanged;
            //GameSyncManager.OnCurrentLevelChanged -= OnCurrentLevelChanged;
        }
    }

    private void OnDecodedHintsChanged(int amount)
    {
        completedChallenges = amount;
        //UpdateXpUI();
    }

    /*private void OnCurrentLevelChanged(int newLevel)
    {
        // This will trigger when the current level changes
        level = newLevel;
        levelText.text = level.ToString();  // Update the level text
    }*/

    void Start()
    {

        /*  if (GameSyncManager.Instance)
        {
            level = GameSyncManager.Instance.currentLevel;
            completedChallenges = GameSyncManager.Instance.decodedHints;

            UpdateXpUI(); // Initial UI update
            levelText.text = level.ToString();

         audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        } 
        }*/

        frontXpBar.fillAmount = currentXp / requiredXp;
        backXpBar.fillAmount = currentXp / requiredXp;
        if (GameSyncManager.Instance)
        {
            level = GameSyncManager.Instance.currentLevel;
        }
        levelText.text = level.ToString();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    { 
        /*sollte leer sein
         */
        if (currentXp >= requiredXp)
        {
            LevelUp();
        }
    }

    void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = frontXpBar.fillAmount;

        if (backXpBar.fillAmount < xpFraction)
        {
            backXpBar.fillAmount = Mathf.MoveTowards(backXpBar.fillAmount, xpFraction, Time.deltaTime);
        }

        if (backXpBar.fillAmount >= xpFraction)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > 0.5f)
            {
                lerpTimer += Time.deltaTime;
                frontXpBar.fillAmount = Mathf.MoveTowards(FXP, xpFraction, Time.deltaTime);

                if (Mathf.Abs(frontXpBar.fillAmount - xpFraction) < 0.01f)
                {
                    frontXpBar.fillAmount = xpFraction;
                    lerpTimer = 0f;
                    delayTimer = 0f;

                    if (frontXpBar.fillAmount >= 1.0f && currentXp >= requiredXp)
                    {
                        LevelUp();
                    }
                }
            }
        }

        xpText.text = $"{completedChallenges}/{requiredXp}";
    }

    public void LevelUp()

    /*// Let the GameSyncManager handle the level-up logic via RPC
        if (GameSyncManager.Instance)
        {
            GameSyncManager.Instance.RpcIncreaseLevel();
        }

        // Play the level-up sound
        if (levelUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }
    }*/ 
    {
        level++;
        completedChallenges = 0;
        currentXp = 0f; 
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        levelText.text = level.ToString();

        if (levelUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    private int _completedChallenges;
    public int completedChallenges
    {
        get { return _completedChallenges; }
        private set
        {
            _completedChallenges = Mathf.Clamp(value, 0, (int)requiredXp);
            currentXp = _completedChallenges; 
            UpdateXpUI(); 
        }
    }

    public float currentXp;
    public const float requiredXp = 4; // 4 represents 100% progress
    private float lerpTimer;
    private float delayTimer;

    [Header("Audio")]
    public AudioClip levelUpSound;

    private AudioSource audioSource;

    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnDecodedHintsChanged += OnDecodedHintsChanged;
            GameSyncManager.OnCurrentLevelChanged += OnCurrentLevelChanged;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnDecodedHintsChanged -= OnDecodedHintsChanged;
            GameSyncManager.OnCurrentLevelChanged -= OnCurrentLevelChanged;
        }
    }

    private void Start()
    {
        if (GameSyncManager.Instance)
        {
            level = GameSyncManager.Instance.currentLevel;
            completedChallenges = GameSyncManager.Instance.decodedHints;

            UpdateXpUI(); // Initial UI update

            levelText.text = GameSyncManager.Instance.currentLevel.ToString();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Additional logic if needed
    }

    private void OnDecodedHintsChanged(int amount)
    {
        completedChallenges = amount;  // Update the progress bar when decoded hints change
    }

    private void OnCurrentLevelChanged(int newLevel)
    {
        level = newLevel;
        levelText.text = level.ToString();

        ResetProgressBar(); // Reset progress bar when level changes
    }

    private void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;  // xpFraction ist der Prozentsatz des Fortschritts hier z.B. 1/4 25%....
        float targetFill = xpFraction; // For frontXpBar //Ziel-Füllstand der frontXpBar, der schrittweise erreicht werden soll

        // Gradually move the backXpBar to the new xpFraction
        backXpBar.fillAmount = Mathf.MoveTowards(backXpBar.fillAmount, xpFraction, Time.deltaTime); //Diese Methode bewegt den aktuellen Wert (backXpBar.fillAmount) allmählich in Richtung des Zielwerts (xpFraction) mit einer Geschwindigkeit, die von Time.deltaTime abhängt
//backXpBar.fillAmount: Dies ist der aktuelle Füllstand der backXpBar, der schrittweise erhöht wird, bis er den Wert xpFraction erreich

        if (backXpBar.fillAmount >= xpFraction) //Überprüfung, ob die backXpBar den Zielwert erreicht hat
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > 0.5f)
            {
                lerpTimer += Time.deltaTime;
                frontXpBar.fillAmount = Mathf.MoveTowards(frontXpBar.fillAmount, xpFraction, Time.deltaTime);

                if (Mathf.Abs(frontXpBar.fillAmount - xpFraction) < 0.01f) //Überprüfung, ob die frontXpBar den Zielwert erreicht hat
//Diese Methode berechnet den absoluten Unterschied zwischen dem aktuellen Füllstand der frontXpBar und dem Zielwert xpFraction.
//Überprüfung: Wenn dieser Unterschied kleiner als 0.01 ist, wird davon ausgegangen, dass die frontXpBar den Zielwert erreicht hat
                {
                    frontXpBar.fillAmount = xpFraction;
                    lerpTimer = 0f;
                    delayTimer = 0f;
                }
            }
        }

        xpText.text = $"{completedChallenges}/{requiredXp}";
    }

    private void ResetProgressBar()
    {
        // Setze nur die visuellen Elemente zurück
    currentXp = 0f;
    frontXpBar.fillAmount = 0f;
    backXpBar.fillAmount = 0f;
        xpText.text = $"{completedChallenges}/{requiredXp}";
    }

    public void LevelUp()
    {
        if (levelUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }
    }
}*/