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
    public GameObject volumeButton;
    public GameObject volumeMenu;
    public GameObject videoButton;
    public GameObject videoMenu;
    public GameObject controlesButton;
    public GameObject controlesMenu;
    public GameObject canvas;
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
        gameManager.CallUpdateDialogue();
        gameObject.transform.parent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public void SwitchsettingsMenu(GameObject layer)
    {
        gameObject.transform.parent.parent.gameObject.SetActive(false);
        layer.SetActive(true);
    }

    public void ExitGamemenu()
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in objects)
        {
            if (go.name.Equals("MainMenu"))
            {
                go.SetActive(true);
            }
            if (go.name.Equals("SavesMenu"))
            {
                go.SetActive(false);
            }
        }
        Destroy(gameObject.transform.parent.parent.gameObject);
    }

    public void turnOnLayer(GameObject layer)
    {
        layer.SetActive(true);
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

    public void NavigateSettingButton()
    {
        gameObject.GetComponent<Button>().interactable = false;

        if (gameObject == volumeButton)
        {
            volumeMenu.SetActive(true);
            videoButton.GetComponent<Button>().interactable = true;
            videoMenu.SetActive(false);
            controlesButton.GetComponent<Button>().interactable = true;
            controlesMenu.SetActive(false);
        }
        else if (gameObject == videoButton)
        {
            videoMenu.SetActive(true);
            volumeButton.GetComponent<Button>().interactable = true;
            volumeMenu.SetActive(false);
            controlesButton.GetComponent<Button>().interactable = true;
            controlesMenu.SetActive(false);
        }
        else if(gameObject == controlesButton)
        {
            controlesMenu.SetActive(true);
            volumeButton.GetComponent<Button>().interactable = true;
            volumeMenu.SetActive(false);
            videoButton.GetComponent<Button>().interactable = true;
            videoMenu.SetActive(false);
        }
    }

    public void StarChartButton()
    {
        LoadScene("MainMenu");
        MainMenuManager.loadStarChart = true;
    }

    public void SoundVolumeSlider()
    {
        float value = GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("soundVolume", value);
        canvas.GetComponent<AudioSource>().volume = value;
    }
}
