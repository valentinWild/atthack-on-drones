using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{

    public void setActivePotion(string potionType) 
    {
        if (potionType == "Health Potion")
        {
            /* GameSyncManager.Instance.runnerHealth += 10 */;
            Debug.Log("Health Potion activated, increased Player Health");
        }
        else if (potionType == "Death Potion")
        {
            /* GameSyncManager.Instance.runnerHealth -= 10; */
            Debug.Log("Death Potion activated, decreased Player Health");
        }
        else if (potionType == "Shield Potion")
        {
            // Shield aktivieren
            Debug.Log("Shield Potion activated");
        }
        else if (potionType == "Attack Potion")
        {
            // Double Lasers or something
            Debug.Log("Attack Potion activated");
        }
    }

}