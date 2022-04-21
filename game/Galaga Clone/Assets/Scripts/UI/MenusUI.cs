﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MenusUI : BaseUI
{
    public GameObject canvas;
    public GameObject mainMenu;
    public GameObject accountMenu;
    public GameObject errorMenu;
    public GameObject savesMenu;
    public GameObject feedbackErrorText;
    public GameObject submitFeedbackButton;
    public GameObject shortPasswordText;
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField feedbackInputField;
    public Button loginButton;
    public Button SignUpButton;
    public Transform confirmDeletionMenu;
    public Text charCountText;
    public MenusManager menusManager;
    public List<GameObject> createSaveTexts = new List<GameObject>();
    public List<GameObject> gameStatTexts = new List<GameObject>();
    public List<GameObject> deleteSaveButtons = new List<GameObject>();
    private Text errorButtonText;
    private Text errorTitle;
    private Text errorDescription;
    private bool isPasswordShown;

    void Start()
    {
        errorTitle = errorMenu.transform.GetChild(1).gameObject.GetComponent<Text>();
        errorDescription = errorMenu.transform.GetChild(2).gameObject.GetComponent<Text>();
        errorButtonText = errorMenu.transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }

    public void PlayGameButton()
    {
        int lastUsedSave = PlayerPrefs.GetInt(Constants.Prefs.lastUsedSave);
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

    public void ExitGame()
    {
        print("Exiting Game");
        Application.Quit();
    }

    public void SoundVolumeSlider(float value)
    {
        PlayerPrefs.SetFloat(Constants.Prefs.soundVolume, value);
        Constants.dontDestroy.GetComponent<AudioSource>().volume = value;
    }

    public void CallSignUp()
    {
        if (IsValidEmail() && IsValidPassword())
        {
            StartCoroutine(SignUp());
        }
    }

    private IEnumerator SignUp()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailInputField.text.ToLower());
        form.AddField("password", passwordInputField.text);
        WWW www = new WWW(Constants.URL + "sign-up.php", form);

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
        if (IsValidEmail() && IsValidPassword())
        {
            StartCoroutine(Login(emailInputField.text, passwordInputField.text));
        }
    }

    public IEnumerator Login(string email, string password)
    {
        email = email.ToLower();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        WWW www = new WWW(Constants.URL + "login.php", form);

        yield return www;

        if (www.text[0] == '0')
        {
            print("User logged in as: " + email);
            Constants.email = email;
            Constants.password = password;
            accountMenu.SetActive(false);
            mainMenu.SetActive(true);
            menusManager.SetEmail(email);
            PlayerPrefs.SetString("email", Constants.email);
            PlayerPrefs.SetString("password", password);
            Constants.isLoggedIn = true;
            isPasswordShown = false;
            UpdateSavesMenu(www.text);
        }
        else if (www.text[0] == '5')
        {
            accountMenu.SetActive(true);
            errorMenu.SetActive(true);
            errorTitle.text = "No account";
            errorDescription.text = "The email you entered is not an account";
            errorButtonText.text = "Ok";
        }
        else if (www.text[0] == '6')
        {
            accountMenu.SetActive(true);
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
        string[] missions = { saveData[5], saveData[6], saveData[7], saveData[8] };
        string[] money = { saveData[1], saveData[2], saveData[3], saveData[4] };

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i] != "1")
            {
                createSaveTexts[i].SetActive(false);
                gameStatTexts[i].SetActive(true);
                gameStatTexts[i].GetComponent<Text>().text = "Mission: " + missions[i] + "\nMoney: " + string.Format("{0:n0}", int.Parse(money[i]));
                deleteSaveButtons[i].SetActive(true);
            }
            else
            {
                createSaveTexts[i].SetActive(true);
                gameStatTexts[i].SetActive(false);
                deleteSaveButtons[i].SetActive(false);
            }
        }
    }

    public bool IsValidEmail()
    {
        string str = emailInputField.text.ToLower();
        if (!str.Contains("@"))
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "Email address needs an '@'";
            errorButtonText.text = "Ok";
            return false;
        }
        else if (str.Substring(0, str.IndexOf("@")).Length < 1)
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "The '@' must have a prefix";
            errorButtonText.text = "Ok";
            return false;
        }
        else if (str.Substring(str.IndexOf("@")).Length < 2)
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid email";
            errorDescription.text = "Email address needs a domain";
            errorButtonText.text = "Ok";
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsValidPassword()
    {
        string str = passwordInputField.text;
        if (str.Any(char.IsWhiteSpace))
        {
            errorMenu.SetActive(true);
            errorTitle.text = "Invalid password";
            errorDescription.text = "Password cannot have any spaces";
            errorButtonText.text = "Ok";
            return false;
        }
        else
        {
            return true;
        }
    }

    public void VerifyFields()
    {
        bool validStrings = emailInputField.text.Length > 0 && passwordInputField.text.Length >= 8;
        SignUpButton.interactable = validStrings;
        loginButton.interactable = validStrings;
    }

    public void OnPasswordEdit()
    {
        if (passwordInputField.text.Length < 8)
        {
            shortPasswordText.SetActive(true);
        }
        else
        {
            shortPasswordText.SetActive(false);
        }
    }

    public void LogOut()
    {
        PlayerPrefs.SetString("email", "");
        PlayerPrefs.SetString("password", "");
        LoadScene(GameScenes.Menus);
    }

    public void LoadSaveButton(int save)
    {
        LoadScene(GameScenes.SaveMenus);
        PlayerPrefs.SetInt(Constants.Prefs.lastUsedSave, save);
        Constants.currentSave = save;
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
        yield return StartCoroutine(Constants.DeleteSaveFromDatabase(save));
        yield return StartCoroutine(GetSavesStatusData());
        confirmDeletionMenu.gameObject.SetActive(false);
    }

    public IEnumerator GetSavesStatusData()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", Constants.email);
        form.AddField("password", Constants.password);
        WWW www = new WWW(Constants.URL + "login.php", form);

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

    public void OnFeedbackInputChanged()
    {
        if (string.IsNullOrWhiteSpace(feedbackInputField.text) || string.IsNullOrEmpty(feedbackInputField.text))
        {
            feedbackErrorText.SetActive(true);
        }
        else
        {
            feedbackErrorText.SetActive(false);
        }
        charCountText.text = feedbackInputField.text.Length + "/" + feedbackInputField.characterLimit;
    }

    private void SendFeedbackButton()
    {
        if (string.IsNullOrWhiteSpace(feedbackInputField.text) || string.IsNullOrEmpty(feedbackInputField.text))
        {
            feedbackErrorText.SetActive(true);
            return;
        }
        else
        {
            feedbackErrorText.SetActive(false);
        }

        submitFeedbackButton.GetComponent<Button>().interactable = false;

        MailMessage email = new MailMessage();
        SmtpClient client = new SmtpClient("smtp.gmail.com")
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Port = 587,
            Credentials = new NetworkCredential("teampolemos@gmail.com", "wczzdgdccmbscemb")
        };

        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) { return true; };

        email.From = new MailAddress("teampolemos@gmail.com", "Polemos");
        email.To.Add("teampolemos@gmail.com");
        email.Subject = "Polemos Feedback" + " | " + Constants.email;
        email.Body = feedbackInputField.text;

        client.SendCompleted += OnAsyncComplete;

        client.SendAsync(email, null);
    }

    private void OnAsyncComplete(object sender, AsyncCompletedEventArgs completedEventArgs)
    {
        submitFeedbackButton.transform.parent.gameObject.SetActive(false);

        if (completedEventArgs.Error != null)
        {
            Debug.LogError(completedEventArgs.Error.Message);
            errorTitle.text = "Failed to send feedback";
            errorDescription.text = "Please check your internet and try again. If the problem continues, please contact the developers";
            errorButtonText.text = "Retry";
            errorButtonText.transform.parent.GetComponent<Button>().onClick.AddListener(delegate { SendFeedbackButton(); });
            errorMenu.SetActive(true);
            return;
        }

        if (completedEventArgs.Cancelled)
        {
            Debug.LogWarning("Feedback cancelled");
            return;
        }

        print("Successfully sent feedback");

        SmtpClient sndr = (SmtpClient)sender;
        sndr.SendCompleted -= OnAsyncComplete;

        ResetFeedbackMenu();

        errorTitle.text = "";
        errorDescription.text = "Thanks for your feedback!";
        errorButtonText.text = "Close";
        errorMenu.SetActive(true);
    }

    public void ResetFeedbackMenu()
    {
        feedbackInputField.text = "";
        charCountText.text = "0/" + feedbackInputField.characterLimit;
        feedbackErrorText.SetActive(false);
        submitFeedbackButton.GetComponent<Button>().interactable = true;
    }

    public void HideShowButton()
    {
        if (isPasswordShown)
        {
            isPasswordShown = false;
            passwordInputField.contentType = InputField.ContentType.Password;
        }
        else
        {
            isPasswordShown = true;
            passwordInputField.contentType = InputField.ContentType.Custom;
            passwordInputField.inputType = InputField.InputType.Standard;
        }
        passwordInputField.ForceLabelUpdate();
    }
}
