using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Function to quit the game
    public void Quit()
    {
        // Quit the application
        Application.Quit();

        // For testing in the editor (this won't be included in the actual build)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
