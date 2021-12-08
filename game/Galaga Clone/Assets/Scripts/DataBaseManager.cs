using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager
{
    public static readonly string URL = "http://localhost/galaga_clone/";
    public static string email;
    public static int currentSave;
    public static int money;
    public static int levelsUnlocked;
    public static int levelsCompleted;
    public static string[] upgradesUnlocked = {};
    public static string[] upgradesActive = {};

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
}
