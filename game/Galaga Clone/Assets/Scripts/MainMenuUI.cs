using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject canvas;
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject accountMenu;
    public GameObject errorMenu;
    public GameObject savesMenu;
    public List<GameObject> createSaveTexts = new List<GameObject>();
    public List<GameObject> gameStatsText = new List<GameObject>();
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button SignUpButton;
    private Text errorButtonText;
    private Text errorTitle;
    private Text errorDescription;

    void Start()
    {
        errorTitle = errorMenu.GetComponentsInChildren<Transform>()[1].gameObject.GetComponent<Text>();
        errorDescription = errorMenu.GetComponentsInChildren<Transform>()[2].gameObject.GetComponent<Text>();
        errorButtonText = errorMenu.GetComponentsInChildren<Transform>()[3].transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    public void PlayGameButton()
    {
        int lastUsedSave = PlayerPrefs.GetInt("lastUsedSave");
        if (lastUsedSave == 1 || lastUsedSave == 2 || lastUsedSave == 3 || lastUsedSave == 4)
        {
            LoadSaveButton(lastUsedSave);
        }
        else
        {
            mainMenu.SetActive(false);
            savesMenu.SetActive(true);
        }
    }

    public void SettingsBackButton()
    {
        mainMenu.SetActive(true);
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }

    public void CallSignUp()
    {
        IsValidEmail(SighUp());
    }

    private IEnumerator SighUp()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailInputField.text.ToLower());
        form.AddField("password", passwordInputField.text);
        WWW www = new WWW(DataBaseManager.URL + "sign-up.php", form);

        yield return www;

        if (www.text == "0")
        {
            print("Successfully created a new account");
            CallLogin();
        }
        else if (www.text == "3")
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Email exists";
            errorDescription.text = "The email you have entered all ready exists";
            errorButtonText.text = "Ok";
        }
        else
        {
            print("Failed to create a new account. Error code: " + www.text);
        }
    }

    public void CallLogin()
    {
        IsValidEmail(Login(emailInputField.text, passwordInputField.text));
    }

    public IEnumerator Login(string email, string password)
    {
        email = email.ToLower();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        WWW www = new WWW(DataBaseManager.URL + "login.php", form);

        yield return www;

        if (www.text[0] == '0')
        {
            print("User logged in as: " + email);
            DataBaseManager.email = email;
            //MainMenuManager.money = int.Parse(www.text.Split('\t')[1]);
            accountMenu.SetActive(false);
            mainMenu.SetActive(true);
            mainMenu.GetComponentsInChildren<Text>()[5].text = "Email: " + DataBaseManager.email;
            PlayerPrefs.SetString("email", DataBaseManager.email);
            PlayerPrefs.SetString("password", password);
            UpdateSavesMenu(www.text);
        }
        else if (www.text[0] == '5')
        {
            errorMenu.SetActive(true);
            errorTitle.text = "No account";
            errorDescription.text = "The email you entered is not an account";
            errorButtonText.text = "Ok";
        }
        else if (www.text[0] == '6')
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Incorrect password";
            errorDescription.text = "You entered an incorrect password for this email";
            errorButtonText.text = "Ok";
        }
        else
        {
            print("User login failed. Error code: " + www.text[0]);
            accountMenu.SetActive(true);
        }
    }

    private void UpdateSavesMenu(string text)
    {
        string[] saveData = text.Split('\t');

        if (saveData[5] != "1")
        {
            createSaveTexts[0].SetActive(false);
            gameStatsText[0].SetActive(true);
            gameStatsText[0].GetComponent<Text>().text = "Mission: " + saveData[5] + "\nMoney: " + saveData[1];
        }

        if (saveData[6] != "1")
        {
            createSaveTexts[1].SetActive(false);
            gameStatsText[1].SetActive(true);
            gameStatsText[0].GetComponent<Text>().text = "Mission: " + saveData[6] + "\nMoney: " + saveData[2];
        }

        if (saveData[7] != "1")
        {
            createSaveTexts[2].SetActive(false);
            gameStatsText[2].SetActive(true);
            gameStatsText[0].GetComponent<Text>().text = "Mission: " + saveData[7] + "\nMoney: " + saveData[3];
        }

        if (saveData[8] != "1")
        {
            createSaveTexts[3].SetActive(false);
            gameStatsText[4].SetActive(true);
            gameStatsText[0].GetComponent<Text>().text = "Mission: " + saveData[8] + "\nMoney: " + saveData[4];
        }
    }

    public void IsValidEmail(IEnumerator result)
    {
        string str = emailInputField.text.ToLower();
        if (!emailInputField.text.Contains("@"))
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "Email address needs an '@'";
            errorButtonText.text = "Ok";
        }
        else if (str.Substring(0, str.IndexOf("@")).Length < 1)
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "The '@' must have a prefix";
            errorButtonText.text = "Ok";
        }
        else if (str.Substring(str.IndexOf("@")).Length < 2)
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "Email address needs a domain";
            errorButtonText.text = "Ok";
        }
        else
        {
            StartCoroutine(result);
        }
    }

    public void VerifyFields()
    {
        bool validStrings = emailInputField.text.Length >= 8 && passwordInputField.text.Length >= 8;
        SignUpButton.interactable = validStrings;
        loginButton.interactable = validStrings;
    }

    public void LogOut()
    {
        PlayerPrefs.SetString("email", "");
        PlayerPrefs.SetString("password", "");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync(scene);
    }

    public void SaveData()
    {
        StartCoroutine(SaveDataToDatabase());
    }

    private IEnumerator SaveDataToDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", DataBaseManager.email.ToLower());
        form.AddField("save", "save" + DataBaseManager.currentSave);
        form.AddField("money", DataBaseManager.money);
        form.AddField("levels_completed", DataBaseManager.levelsCompleted);
        form.AddField("levels_unlocked", DataBaseManager.levelsUnlocked);
        form.AddField("upgrades_unlocked", string.Join(",", DataBaseManager.upgradesUnlocked));
        form.AddField("upgrades_active", string.Join(",", DataBaseManager.upgradesActive));
        WWW www = new WWW(DataBaseManager.URL + "savedata.php", form);

        yield return www;

        if (www.text == "0")
        {
            print("Game saved");
        }
        else
        {
            print("Failed to save game. Error code: " + www.text);
        }
    }

    public void LoadSaveButton(int save)
    {
        mainMenu.SetActive(false);
        Instantiate(gameMenu, canvas.transform);
        PlayerPrefs.SetInt("lastUsedSave", save);
        DataBaseManager.currentSave = save;
    }

    public void ErrorButton()
    {
        errorMenu.SetActive(false);
        errorTitle.text = "";
        errorDescription.text = "";
    }

    public void DeleteSave(string save)
    {
        StartCoroutine(DeleteSaveFromDB(save));
    }

    public IEnumerator DeleteSaveFromDB(string save)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", DataBaseManager.email.ToLower());
        form.AddField("save", save);
        form.AddField("money", 0);
        form.AddField("levels_completed", 0);
        form.AddField("levels_unlocked", 1);
        form.AddField("upgrades_unlocked", "none");
        form.AddField("upgrades_active", "none");
        WWW www = new WWW(DataBaseManager.URL + "savedata.php", form);

        yield return www;

        if (www.text == "0")
        {
            PlayerPrefs.SetInt("lastUsedSave", 0);
            print("Successfully deleted save");
        }
        else
        {
            print("Failed to delete save. Error code: " + www.text);
        }
    }
}
