using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static bool loadStarChart;

    public GameObject canvas;
    public GameObject accountMenu;
    public Slider soundVolumeSlider;
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

        if (PlayerPrefs.GetInt(DataBaseManager.Prefs.firstTimePlayed) == 0)
        {
            PlayerPrefs.SetInt(DataBaseManager.Prefs.firstTimePlayed, 1);
            PlayerPrefs.SetFloat(DataBaseManager.Prefs.soundVolume, 1);
        }

        float value = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
        canvas.GetComponent<AudioSource>().volume = value;
        soundVolumeSlider.value = value;

        if (loadStarChart)
        {
            mainMenuUI.LoadSaveButton(DataBaseManager.currentSave);
            loadStarChart = false;
        }
    }
}
