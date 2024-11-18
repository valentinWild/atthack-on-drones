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
    public AudioSource lose;
    public AudioSource backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        loseScreenDisplay.gameObject.SetActive(false);
    }

   private void OnEnable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnRunnerDied += ActivateLoseScreen;
        }
    }

    private void OnDisable()
    {
        if (GameSyncManager.Instance)
        {
            GameSyncManager.OnRunnerDied -= ActivateLoseScreen;
        }
    }

    private void ActivateLoseScreen()
    {
        Debug.Log("Lose Screen Activated");
        backgroundMusic.Stop();
        lose.Play();
        loseScreenDisplay.gameObject.SetActive(true);
        if (GameSyncManager.Instance)
        {
            string decodedHints = GameSyncManager.Instance.decodedHints.ToString();
            string shotEnenemyDrones = GameSyncManager.Instance.shotEnemyDrones.ToString();
            //string level = GameSyncManager.Instance.currentLevel.ToString();
            // TODO: drone shot ?
            playerStats.text = "Decoded hints: " + decodedHints + "@\nShot Drones: " + shotEnenemyDrones;
        }   
    }
}
