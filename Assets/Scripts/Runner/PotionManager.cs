using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{

    private void Start() {
        //StartCoroutine(testPotion(10f));
    }

    private IEnumerator testPotion(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameSyncManager.Instance.RpcSetRunnerPotion("Health Potion");
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
            Debug.Log("Health Potion activated, increased Player Health");
        }
        else if (potionType == "Death Potion")
        {
            Debug.Log("Death Potion activated, decreased Player Health");
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

    private void setPotionMessage (string message)
    {

    }

}