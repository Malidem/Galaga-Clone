using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static bool isStartingUp = true;
    public static GameObject dontDestroy;

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
        if (isStartingUp)
        {
            isStartingUp = false;
            dontDestroy = new GameObject
            {
                name = "DontDestroy"
            };

            DontDestroyOnLoad(dontDestroy);

            if (loggingEnabled)
            {
                dontDestroy.AddComponent<Logger>();
            }

            dontDestroy.AddComponent<AudioSource>();
            dontDestroy.AddComponent<ButtonAudio>();
        }

        Button[] allButtons = canvas.GetComponentsInChildren<Button>(true);
        foreach (Button button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(delegate { dontDestroy.GetComponent<ButtonAudio>().PlayClickSound(); });
        }

        if (PlayerPrefs.GetInt(DataBaseManager.Prefs.firstTimePlayed) == 0)
        {
            PlayerPrefs.SetInt(DataBaseManager.Prefs.firstTimePlayed, 1);
            PlayerPrefs.SetFloat(DataBaseManager.Prefs.soundVolume, 1);
        }

        float value = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
        dontDestroy.GetComponent<AudioSource>().volume = value;
        soundVolumeSlider.value = value;
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
    }
}
