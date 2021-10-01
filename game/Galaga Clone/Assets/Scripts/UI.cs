using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject eventSystem;
    private GameManager gameManager;
    public GameObject shopButton;
    public GameObject shopMenu;
    public GameObject upgradesButton;
    public GameObject upgradesMenu;
    public GameObject galaxyButton;
    public GameObject galaxyMenu;

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

    public void NavigationButton()
    {
        gameObject.GetComponent<Button>().interactable = false;

        if (gameObject == shopButton)
        {
            shopMenu.SetActive(true);
            upgradesButton.GetComponent<Button>().interactable = true;
            upgradesMenu.SetActive(false);
            galaxyButton.GetComponent<Button>().interactable = true;
            galaxyMenu.SetActive(false);

        }
        else if (gameObject == upgradesButton)
        {
            upgradesMenu.SetActive(true);
            shopButton.GetComponent<Button>().interactable = true;
            shopMenu.SetActive(false);
            galaxyButton.GetComponent<Button>().interactable = true;
            galaxyMenu.SetActive(false);
        }
        else if (gameObject == galaxyButton)
        {
            galaxyMenu.SetActive(true);
            shopButton.GetComponent<Button>().interactable = true;
            shopMenu.SetActive(false);
            upgradesButton.GetComponent<Button>().interactable = true;
            upgradesMenu.SetActive(false);
        }
    }
}
