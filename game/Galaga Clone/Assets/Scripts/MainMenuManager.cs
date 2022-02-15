using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static bool isStartingUp = true;
    public static bool loadStarChart;
    public static GameObject logger;

    public bool loggingEnabled;
    public GameObject canvas;
    public GameObject accountMenu;
    public GameObject backgroundPrefab;
    public GameObject backgroundsFolder;
    public Slider soundVolumeSlider;
    public Texture2D cursor;

    private MainMenuUI mainMenuUI;

    void OnEnable()
    {
        if (isStartingUp && loggingEnabled)
        {
            isStartingUp = false;
            logger = new GameObject
            {
                name = "Logger"
            };
            logger.AddComponent<Logger>();
            logger.AddComponent<ButtonAudio>();
            logger.AddComponent<AudioSource>();
            logger.AddComponent<AudioSource>();
        }

        GameObject[] allButtons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(delegate { logger.GetComponent<ButtonAudio>().PlayClickSound(); });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI = canvas.GetComponent<MainMenuUI>();
        Cursor.SetCursor(cursor, Vector3.zero, CursorMode.ForceSoftware);
        Instantiate(backgroundPrefab, backgroundsFolder.transform.position, transform.rotation, backgroundsFolder.transform);

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
