using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    public bool loggingEnabled;
    public GameObject canvas;
    public GameObject accountMenu;
    public GameObject backgroundPrefab;
    public GameObject backgroundsFolder;
    public Slider soundVolumeSlider;
    public Texture2D cursor;

    private MenusUI mainMenuUI;

    void OnEnable()
    {
        if (Constants.isStartingUp)
        {
            Constants.isStartingUp = false;
            Constants.dontDestroy = new GameObject
            {
                name = "DontDestroy"
            };

            DontDestroyOnLoad(Constants.dontDestroy);

            if (loggingEnabled)
            {
                Constants.dontDestroy.AddComponent<Logger>();
            }

            Constants.dontDestroy.AddComponent<AudioSource>();
            Constants.dontDestroy.AddComponent<ButtonAudio>();
        }

        Button[] allButtons = canvas.GetComponentsInChildren<Button>(true);
        foreach (Button button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(delegate { Constants.dontDestroy.GetComponent<ButtonAudio>().PlayClickSound(); });
        }

        if (PlayerPrefs.GetInt(Constants.Prefs.firstTimePlayed) == 0)
        {
            PlayerPrefs.SetInt(Constants.Prefs.firstTimePlayed, 1);
            PlayerPrefs.SetFloat(Constants.Prefs.soundVolume, 1);
        }

        float value = PlayerPrefs.GetFloat(Constants.Prefs.soundVolume);
        Constants.dontDestroy.GetComponent<AudioSource>().volume = value;
        soundVolumeSlider.value = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI = canvas.GetComponent<MenusUI>();
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
