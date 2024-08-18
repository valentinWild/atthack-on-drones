using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    private int _completedChallenges;
    public int level;
    public int completedChallenges
    {
        get { return _completedChallenges; }
        private set
        {
            _completedChallenges = Mathf.Clamp(value, 0, (int)requiredXp);
            currentXp = _completedChallenges;
            StartCoroutine(UpdateXpUICoroutine());
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
        level = 0;

        if (GameSyncManager.Instance)
        {
            level = GameSyncManager.Instance.currentLevel;
            completedChallenges = GameSyncManager.Instance.decodedHints;

            StartCoroutine(UpdateXpUICoroutine());
            ResetProgressBar();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            levelText.text = GameSyncManager.Instance.currentLevel.ToString();
        }
    }

    /*private void Update()
    {
        SimulateProgress();
    }*/

    private void OnDecodedHintsChanged(int amount)
    {
        completedChallenges = amount;

        if (levelUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }

        StartCoroutine(UpdateXpUICoroutine());
       
    }

    private void OnCurrentLevelChanged(int newLevel)
    {
        level = newLevel;
        levelText.text = level.ToString();

        ResetProgressBar();
    }

   private IEnumerator UpdateXpUICoroutine()
    {
        float xpFraction = currentXp / requiredXp;
        float fillSpeed = 0.5f;

        while (Mathf.Abs(backXpBar.fillAmount - xpFraction) > 0.01f)
        {
            backXpBar.fillAmount = Mathf.MoveTowards(backXpBar.fillAmount, xpFraction, fillSpeed * Time.deltaTime);
            yield return null;
        }

        backXpBar.fillAmount = xpFraction;

        delayTimer = 0f;
        while (Mathf.Abs(frontXpBar.fillAmount - xpFraction) > 0.01f)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > 0.5f)
            {
                frontXpBar.fillAmount = Mathf.MoveTowards(frontXpBar.fillAmount, xpFraction, fillSpeed * Time.deltaTime);
            }
            yield return null;
        }

        frontXpBar.fillAmount = xpFraction;

        xpText.text = $"{completedChallenges}/{requiredXp}";
    }

    private void ResetProgressBar()
    {
        
        currentXp = 0f;
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        xpText.text = $"0/{requiredXp}";
    }

    /*public void LevelUp()
    {
        if (levelUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }
    }*/

    /*
    // Function to simulate progress when the "U" key is pressed
    private void SimulateProgress()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Add 25% progress
            completedChallenges += 1;

            // If completedChallenges reaches requiredXp, simulate a level up
            if (completedChallenges >= requiredXp)
            {
                LevelUp();
                level++;
                completedChallenges = 0;
                ResetProgressBar();
                levelText.text = level.ToString();
            }

            StartCoroutine(UpdateXpUICoroutine());
        }
    }*/ 
}