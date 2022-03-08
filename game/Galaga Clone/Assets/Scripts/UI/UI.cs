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
        if (SceneManager.GetActiveScene().name == "Level")
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
        yield return StartCoroutine(Constants.SaveDataToDatabase());
        LoadScene("Menus");
    }

    public void turnOnLayer(GameObject layer)
    {
        layer.SetActive(true);
    }

    public void LoadLevel(string level)
    {
        StartCoroutine(StartloadLevel(level));
    }

    private IEnumerator StartloadLevel(string level)
    {
        yield return StartCoroutine(Constants.SaveDataToDatabase());
        LoadScene(level);
    }

    public void StarChartButton()
    {
        LoadScene("LoadedSave");
    }
    
    public void CancelButton()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
