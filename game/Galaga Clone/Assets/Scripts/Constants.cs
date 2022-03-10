using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // Game Functionality
    public static readonly string URL = "http://polemos.atspace.cc/php/";
    public static bool isStartingUp = true;
    public static bool isLoggedIn;
    public static GameObject dontDestroy;

    // User Account
    public static string email;
    public static string password;
    public static int currentSave;
    public static int money;
    public static int levelsUnlocked;
    public static int levelsCompleted;
    public static string[] upgradesUnlocked = {};
    public static string[] upgradesActive = {};
    public static string[] shopItems = {};

    public static IEnumerator SaveDataToDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email.ToLower());
        form.AddField("save", "save" + currentSave);
        form.AddField("money", money);
        form.AddField("levels_completed", levelsCompleted);
        form.AddField("levels_unlocked", levelsUnlocked);
        form.AddField("upgrades_unlocked", string.Join(",", upgradesUnlocked));
        form.AddField("upgrades_active", string.Join(",", upgradesActive));
        form.AddField("shop_items", string.Join(",", shopItems));
        WWW www = new WWW(URL + "savedata.php", form);

        yield return www;

        if (www.text == "0")
        {
            Debug.Log("Game saved");
        }
        else
        {
            Debug.Log("Failed to save game. Error code: " + www.text);
        }
    }

    public static IEnumerator DeleteSaveFromDatabase(string save)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email.ToLower());
        form.AddField("save", save);
        form.AddField("money", 0);
        form.AddField("levels_completed", 0);
        form.AddField("levels_unlocked", 1);
        form.AddField("upgrades_unlocked", "none");
        form.AddField("upgrades_active", "none,none,none");
        form.AddField("shop_items", "none,none,none");
        WWW www = new WWW(URL + "savedata.php", form);

        yield return www;

        if (www.text == "0")
        {
            if (save == "save" + currentSave)
            {
                PlayerPrefs.SetInt(Prefs.lastUsedSave, 0);
            }
            Debug.Log("Successfully deleted " + save);
        }
        else
        {
            Debug.Log("Failed to delete save. Error code: " + www.text);
        }
    }

    public static class Prefs
    {
        public static readonly string soundVolume = Set("soundVolume");
        public static readonly string lastUsedSave = Set("lastUsedSave");
        public static readonly string firstTimePlayed = Set("firstTimePlayed");

        private static string Set(string name)
        {
            return email + name;
        }
    }
}
