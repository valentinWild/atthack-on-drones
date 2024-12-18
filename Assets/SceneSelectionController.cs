using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
/* 
public class SceneSelectionController : MonoBehaviour
{
    // Function to load a Scene by its Name
    public void LoadScene(string sceneName)
    {
 if (!SceneExists(sceneName))
        {
            return;
        } 
        SceneManager.LoadScene(sceneName);
    }

    private bool SceneExists(string sceneName)
    {
        Debug.Log("Check Scene Name" + sceneName);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        Debug.Log(scene.IsValid());
        return scene.IsValid();
    }

using UnityEngine; */

public class SceneSelectionController : MonoBehaviour
{
    private BasicSpawner spawner;
    private bool isHost = false;
    private bool isClient = false;

    public Button labButton;
    public Button runnerButton;
    public Button startButtonLab; 
    public Button startButtonRunner;

    public TextMeshProUGUI startText;
    public TextMeshProUGUI waitText; 

    private void Start()
    {
        spawner = FindObjectOfType<BasicSpawner>();
        startButtonLab.gameObject.SetActive(false);
        startButtonRunner.gameObject.SetActive(false);

        if(startText.gameObject)
        {
            startText.gameObject.SetActive(true);
        }
        if(waitText.gameObject)
        {
            waitText.gameObject.SetActive(false);
        }

        if(labButton.gameObject)
        {
            labButton.gameObject.SetActive(true);
        }
        if(runnerButton.gameObject)
        {
            runnerButton.gameObject.SetActive(true);
        }
        /*
        if(startButtonLab.gameObject)
        {
            startButtonLab.gameObject.SetActive(true);
        }
        if(startButtonRunner.gameObject)
        {
            startButtonRunner.gameObject.SetActive(false);
        }*/
    }

    public void SetHost(bool isHost)
    {
        isHost = true;

        startText.gameObject.SetActive(false);
        waitText.gameObject.SetActive(true);

        labButton.gameObject.SetActive(false);
        runnerButton.gameObject.SetActive(false);
        startButtonLab.gameObject.SetActive(true);
    }

    public void SetClient(bool isClient)
    {
        isClient = true;

        startText.gameObject.SetActive(false);
        waitText.gameObject.SetActive(true);

        labButton.gameObject.SetActive(false);
        runnerButton.gameObject.SetActive(false);
        startButtonRunner.gameObject.SetActive(true);
    }

    public void LoadLabScene()
    {
        // Load the Lab Scene
        SceneManager.LoadScene("Lab");
    }

    public void LoadRunnerScene()
    {
        // Start the game as Client and load the RunnerScene
        SceneManager.LoadScene("Runner");
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbySceneVR");
    }
}

