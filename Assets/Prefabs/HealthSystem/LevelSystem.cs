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
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnDecodedHintsChanged -= OnDecodedHintsChanged;
        }
    }

    private void OnDecodedHintsChanged(int amount)
    {
        completedChallenges = amount;
    }

    void Start()
    {
        frontXpBar.fillAmount = currentXp / requiredXp;
        backXpBar.fillAmount = currentXp / requiredXp;
        levelText.text = level.ToString();
    }

    void Update()
    {
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
    {
        level++;
        completedChallenges = 0;
        currentXp = 0f; 
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        levelText.text = level.ToString();
    }
}