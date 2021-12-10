using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static bool loadStarChart;

    public GameObject canvas;
    public GameObject accountMenu;
    public Texture2D cursor;

    private MainMenuUI mainMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI = canvas.GetComponent<MainMenuUI>();
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
        if (PlayerPrefs.GetString("email") != "")
        {
            StartCoroutine(mainMenuUI.Login(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
        }
        else
        {
            accountMenu.SetActive(true);
        }

        if (loadStarChart)
        {
            mainMenuUI.LoadSaveButton(DataBaseManager.currentSave);
            loadStarChart = false;
        }
    }
}
