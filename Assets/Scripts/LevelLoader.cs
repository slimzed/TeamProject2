using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    SceneManager sceneManager;

    private void Start()
    {
        ScoreManager.OnGameOver += LoadGameOverScene;

        // two win conditions, one from AudioManager and one from ScoreManager
        AudioManager.OnGameVictory += LoadNextScene;
        ScoreManager.OnGameVictory += LoadNextScene;
    }


    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes to load.");
        }   
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadTutorialScene()
    {
        Invoke("HandleTutorialSceneLoad", 0.2f); // slight delay to ensure the scene is loaded properly
    }
    private void HandleTutorialSceneLoad()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
