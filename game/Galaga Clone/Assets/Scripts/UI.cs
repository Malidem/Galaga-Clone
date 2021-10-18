using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    public GameObject eventSystem;
    public GameObject shopButton;
    public GameObject shopMenu;
    public GameObject shipButton;
    public GameObject shipMenu;
    public GameObject galaxyButton;
    public GameObject galaxyMenu;
    private GameManager gameManager;
    
    public void MainSettings(GameObject pausemenu)
    {
        pausemenu.SetActive(true);
        gameObject.transform.parent.GetComponentsInChildren<Transform>()[1].gameObject.SetActive(false);
    }

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
        gameManager.StartWaves();
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void LoadScene(string scene)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene);
        SceneManager.UnloadSceneAsync(currentScene);
        Time.timeScale = 1;
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
            shipButton.GetComponent<Button>().interactable = true;
            shipMenu.SetActive(false);
            galaxyButton.GetComponent<Button>().interactable = true;
            galaxyMenu.SetActive(false);

        }
        else if (gameObject == shipButton)
        {
            shipMenu.SetActive(true);
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
            shipButton.GetComponent<Button>().interactable = true;
            shipMenu.SetActive(false);
        }
    }
}
