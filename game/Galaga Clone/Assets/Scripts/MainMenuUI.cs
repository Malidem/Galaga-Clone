using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject canvas;
    public GameObject mainMenu;
    public GameObject gameMenu;

    public void PlayGameButton()
    {
        mainMenu.SetActive(false);
        Instantiate(gameMenu, canvas.transform);
    }

    public void SettingsBackButton()
    {
        mainMenu.SetActive(true);
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }
}
