using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseUI : MonoBehaviour
{
    public void LoadScene(GameScenes scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene((int)scene, LoadSceneMode.Single);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public enum GameScenes
    {
        Menus = 0,
        Level = 1,
        SaveMenus = 2,
    }
}
