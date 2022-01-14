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
    public Transform confirmDeletionMenu;
    private Text errorButtonText;
    private Text errorTitle;
    private Text errorDescription;

    void Start()
    {
        errorTitle = errorMenu.transform.GetChild(1).gameObject.GetComponent<Text>();
        errorDescription = errorMenu.transform.GetChild(2).gameObject.GetComponent<Text>();
        errorButtonText = errorMenu.transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }

    public void PlayGameButton()
    {
        int lastUsedSave = PlayerPrefs.GetInt(DataBaseManager.Prefs.lastUsedSave);
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

    public void CallSignUp()
    {
        IsValidEmail(SignUp());
    }

    private IEnumerator SignUp()
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
            DataBaseManager.password = password;
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
            gameStatsText[0].GetComponent<Text>().text = "Mission: " + saveData[5] + "\nMoney: " + string.Format("{0:n0}", int.Parse(saveData[1]));
        }
        else
        {
            createSaveTexts[0].SetActive(true);
            gameStatsText[0].SetActive(false);
        }

        if (saveData[6] != "1")
        {
            createSaveTexts[1].SetActive(false);
            gameStatsText[1].SetActive(true);
            gameStatsText[1].GetComponent<Text>().text = "Mission: " + saveData[6] + "\nMoney: " + string.Format("{0:n0}", int.Parse(saveData[2]));
        }
        else
        {
            createSaveTexts[1].SetActive(true);
            gameStatsText[1].SetActive(false);
        }

        if (saveData[7] != "1")
        {
            createSaveTexts[2].SetActive(false);
            gameStatsText[2].SetActive(true);
            gameStatsText[2].GetComponent<Text>().text = "Mission: " + saveData[7] + "\nMoney: " + string.Format("{0:n0}", int.Parse(saveData[3]));
        }
        else
        {
            createSaveTexts[2].SetActive(true);
            gameStatsText[2].SetActive(false);
        }

        if (saveData[8] != "1")
        {
            createSaveTexts[3].SetActive(false);
            gameStatsText[3].SetActive(true);
            gameStatsText[3].GetComponent<Text>().text = "Mission: " + saveData[8] + "\nMoney: " + string.Format("{0:n0}", int.Parse(saveData[4]));
        }
        else
        {
            createSaveTexts[3].SetActive(true);
            gameStatsText[3].SetActive(false);
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
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadSaveButton(int save)
    {
        mainMenu.SetActive(false);
        Instantiate(gameMenu, canvas.transform);
        PlayerPrefs.SetInt(DataBaseManager.Prefs.lastUsedSave, save);
        DataBaseManager.currentSave = save;
    }

    public void ErrorButton()
    {
        errorMenu.SetActive(false);
        errorTitle.text = "";
        errorDescription.text = "";
    }

    public void DeleteSaveButton(string save)
    {
        Button button = confirmDeletionMenu.GetChild(2).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { ConfirmDeletionButton(save); });
        confirmDeletionMenu.gameObject.SetActive(true);
    }

    public void ConfirmDeletionButton(string save)
    {
        StartCoroutine(ConfirmDeletion(save));
    }

    private IEnumerator ConfirmDeletion(string save)
    {
        yield return StartCoroutine(DataBaseManager.DeleteSaveFromDatabase(save));
        yield return StartCoroutine(GetSavesStatusData());
        confirmDeletionMenu.gameObject.SetActive(false);
    }

    public IEnumerator GetSavesStatusData()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", DataBaseManager.email);
        form.AddField("password", DataBaseManager.password);
        WWW www = new WWW(DataBaseManager.URL + "login.php", form);

        yield return www;

        if (www.text[0] == '0')
        {
            print("Successfully received save status data");
            UpdateSavesMenu(www.text);
        }
        else
        {
            print("Failed to get save status data. Error code: " + www.text[0]);
        }
    }
}
