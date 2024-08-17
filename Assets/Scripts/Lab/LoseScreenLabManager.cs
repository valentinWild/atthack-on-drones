using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoseScreenLabManager : MonoBehaviour
{
    public GameObject loseScreenDisplay;
    public TextMeshProUGUI playerStats;
    public Button tryAgainButton;
    public Button quitGameButton;

    // Start is called before the first frame update
    void Start()
    {
        //loseScreenDisplay.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameSyncManager.Instance)
        {
            if (GameSyncManager.Instance.runnerHealth == 0)
            {
                loseScreenDisplay.gameObject.SetActive(true);
                GetPlayerStats();
            }
        }
        
    }

    private void GetPlayerStats()
    {
        string decodedHints = GameSyncManager.Instance.decodedHints.ToString();
        string level = GameSyncManager.Instance.currentLevel.ToString();
        // TODO: drone shot ?
        playerStats.text = "Decoded hints: " + decodedHints + "@\nCurrent Level: " + level;
    }
}
