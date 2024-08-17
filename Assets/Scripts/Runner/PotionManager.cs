using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{

    private void Start() {
        StartCoroutine(testPotion(10f));
        StartCoroutine(testChallenges(10f));
    }

    private IEnumerator testChallenges(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcUpdateDecodedHints(1);
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcUpdateDecodedHints(2);
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcUpdateDecodedHints(3);
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcUpdateDecodedHints(4);
    }


    private IEnumerator testPotion(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcSetRunnerPotion("Death Potion");
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcSetRunnerPotion("Health Potion");
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcSetRunnerPotion("Shield Potion");
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcSetRunnerPotion("Attack Potion");
    }


    private void OnEnable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged += OnActivePotionChanged;
        }
    }

    private void OnDisable()
    {
        if(GameSyncManager.Instance) {
            GameSyncManager.OnActivePotionChanged -= OnActivePotionChanged;
        }
    }

    private void OnActivePotionChanged(string potionType)
    {
        if (potionType == "Health Potion")
        {
            if (GameSyncManager.Instance)
            {
                GameSyncManager.Instance.RpcIncreaseRunnerHealth(20f);        
                Debug.Log("Health Potion activated, increased Player Health");     
            }
        }
        else if (potionType == "Death Potion")
        {
            if (GameSyncManager.Instance)
            {
                GameSyncManager.Instance.RpcDecreaseRunnerHealth(20f);        
                Debug.Log("Death Potion activated, decreased Player Health");
            }
        }
        else if (potionType == "Shield Potion")
        {
            Debug.Log("Shield Potion activated");
        }
        else if (potionType == "Attack Potion")
        {
            // Double Lasers or something
            Debug.Log("Attack Potion activated");
        }

    }

}