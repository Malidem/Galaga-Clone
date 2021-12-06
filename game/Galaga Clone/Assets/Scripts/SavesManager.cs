using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavesManager : MonoBehaviour
{
    public Sprite completedLevel;
    public Sprite unlockedLevel;
    public List<GameObject> levels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getSaveData());
    }

    public IEnumerator getSaveData()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", DataBaseManager.email);
        form.AddField("currentSave", DataBaseManager.currentSave);
        WWW www = new WWW(DataBaseManager.URL + "fetch-saved-data.php", form);

        yield return www;

        if (www.text[0] == '0')
        {
            string[] data = www.text.Split('\t');
            DataBaseManager.money = int.Parse(data[1]);
            DataBaseManager.levelsUnlocked = int.Parse(data[2]);
            DataBaseManager.levelsCompleted = int.Parse(data[3]);
            DataBaseManager.upgradesUnlocked = data[4].Split(',');
            DataBaseManager.upgradesActive = data[5].Split(',');
            transform.GetChild(3).GetChild(2).GetComponent<Text>().text = "Money: " + DataBaseManager.money;
            LoadLevels();
            CreateUpgrades();
        }
        else
        {
            print("Failed to get saved data. Error code: " + www.text);
        }
    }

    private void LoadLevels()
    {
        for (int i = 0; i < DataBaseManager.levelsUnlocked; i++)
        {
            levels[i].GetComponent<Image>().sprite = unlockedLevel;
            levels[i].GetComponent<LevelHover>().isUnlocked = true;
        }

        for (int i = 0; i < DataBaseManager.levelsCompleted; i++)
        {
            levels[i].GetComponent<Image>().sprite = completedLevel;
        }
    }

    private void CreateUpgrades()
    {
        for (int i = 0; i < DataBaseManager.upgradesUnlocked.Length; i++)
        {
            // Instantiate the upgrade cards here
        }

        for (int i = 0; i < DataBaseManager.upgradesActive.Length; i++)
        {
            // Move the active upgrade cards to the card slots here
        }
    }
}
