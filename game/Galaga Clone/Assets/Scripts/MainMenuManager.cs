using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject accountMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("email") != "")
        {
            StartCoroutine(canvas.GetComponent<MainMenuUI>().Login(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
        }
        else
        {
            accountMenu.SetActive(true);
        }
    }
}
