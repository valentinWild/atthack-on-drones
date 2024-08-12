using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void Start()
    {
        spawner = FindObjectOfType<BasicSpawner>();
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
}

