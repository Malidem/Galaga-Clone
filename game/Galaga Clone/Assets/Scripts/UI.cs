using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject eventSystem;
    private GameManager gameManager;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            gameManager = eventSystem.GetComponent<GameManager>();
        }
    }

    public void StartButton()
    {
        gameManager.gameStarted = true;
        gameManager.SpawnEnemies();
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void LoadScene(string scene)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public void SwitchUILayer(GameObject layer)
    {
        layer.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        print("Exiting Game");
        Application.Quit();
    }
}
