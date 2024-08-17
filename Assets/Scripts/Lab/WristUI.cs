using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WristUI : MonoBehaviour
{

    public TextMeshProUGUI runnerHP;
    public TextMeshProUGUI activePotion;
    public TextMeshProUGUI enemyDrones;
    public TextMeshProUGUI friendDrones;

    private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnRunnerHealthChanged += ChangeRunnerHP;
            GameSyncManager.OnActivePotionChanged += ChangeActivePotion;
            GameSyncManager.OnShotEnemyDronesChanged += ChangeEnemyDrones;
            GameSyncManager.OnCollectedDronesChanged += ChangeFriendDrones;
        }
    }



    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnRunnerHealthChanged -= ChangeRunnerHP;
            GameSyncManager.OnActivePotionChanged -= ChangeActivePotion;
            GameSyncManager.OnShotEnemyDronesChanged -= ChangeEnemyDrones;
            GameSyncManager.OnCollectedDronesChanged -= ChangeFriendDrones;
        }
    }

    private void ChangeRunnerHP(float currentHP)
    {
        runnerHP.text = currentHP.ToString();
    }

    private void ChangeActivePotion(string potionName)
    {
        activePotion.text = potionName;
    }

    private void ChangeFriendDrones(int dronesCaught)
    {
        friendDrones.text = dronesCaught.ToString();
    }

    private void ChangeEnemyDrones(int dronesShot)
    {
        enemyDrones.text = dronesShot.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
