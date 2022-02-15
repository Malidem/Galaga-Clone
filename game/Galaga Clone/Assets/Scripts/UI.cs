using System;
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
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    public void SwitchUILayer(GameObject layer)
    {
        layer.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void StartExitGameMenu()
    {
        StartCoroutine(ExitGameMenu());
    }

    private IEnumerator ExitGameMenu()
    {
        yield return StartCoroutine(DataBaseManager.SaveDataToDatabase());
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
        yield return StartCoroutine(Resources.FindObjectsOfTypeAll<Canvas>()[0].GetComponent<MainMenuUI>().GetSavesStatusData());
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

    public void LoadLevel(string level)
    {
        StartCoroutine(StartloadLevel(level));
    }

    private IEnumerator StartloadLevel(string level)
    {
        yield return StartCoroutine(DataBaseManager.SaveDataToDatabase());
        LoadScene(level);
    }

    public void StarChartButton()
    {
        LoadScene("MainMenu");
        MainMenuManager.loadStarChart = true;
    }

    public void SoundVolumeSlider()
    {
        float value = GetComponent<Slider>().value;
        PlayerPrefs.SetFloat(DataBaseManager.Prefs.soundVolume, value);
        MainMenuManager.dontDestroy.GetComponent<AudioSource>().volume = value;
    }
    
    public void CancelButton()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
