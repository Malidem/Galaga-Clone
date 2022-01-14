using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SavesManager : MonoBehaviour
{
    public GameObject upgradesParent;
    public GameObject dragObject;
    public GameObject scroller;
    public GameObject levelsFolder;
    public Sprite completedLevel;
    public Sprite unlockedLevel;
    public Text moneyText;
    public GameObject[] upgradeSlots;
    public GameObject[] shopSlots;
    public List<GameObject> upgrades = new List<GameObject>();
    public List<GameObject> buyableUpgrades = new List<GameObject>();
    public List<GameObject> commonUpgrades = new List<GameObject>();
    public List<GameObject> rareUpgrades = new List<GameObject>();
    public List<GameObject> legendaryUpgrades = new List<GameObject>();

    private System.Random random = new System.Random();
    private Transform[] levels;

    // Start is called before the first frame update
    void Start()
    {
        levels = levelsFolder.transform.GetComponentsInChildren<Transform>();
        buyableUpgrades = upgrades.ToList();
        StartCoroutine(getSaveData());
        Rect rect = scroller.GetComponent<RectTransform>().rect;
        scroller.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        scroller.GetComponent<BoxCollider2D>().size = new Vector2(rect.width / scroller.transform.lossyScale.x, rect.height / scroller.transform.lossyScale.y);
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(DataBaseManager.Prefs.soundVolume);
    }

    void Update()
    {
        moneyText.text = "Money: " + string.Format("{0:n0}", DataBaseManager.money);
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
            DataBaseManager.levelsUnlocked = 6;
            DataBaseManager.levelsCompleted = int.Parse(data[3]);
            DataBaseManager.upgradesUnlocked = data[4].Split(',');
            DataBaseManager.upgradesActive = data[5].Split(',');
            DataBaseManager.shopItems = data[6].Split(',');
            LoadLevels();
            CreateUpgrades();
            LoadShop();
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
            levels[i + 1].GetComponent<Image>().sprite = unlockedLevel;
            levels[i + 1].GetComponent<LevelHover>().isUnlocked = true;
        }

        for (int i = 0; i < DataBaseManager.levelsCompleted; i++)
        {
            levels[i + 1].GetComponent<Image>().sprite = completedLevel;
            levels[i + 1].GetComponent<LevelHover>().isUnlocked = false;
        }
    }

    private void CreateUpgrades()
    {
        for (int i = 0; i < DataBaseManager.upgradesUnlocked.Length; i++)
        {
            if (DataBaseManager.upgradesUnlocked[i] != "none")
            {
                string[] upgrade = DataBaseManager.upgradesUnlocked[i].Split('|');
                for (int i2 = 0; i2 < upgrades.Count; i2++)
                {
                    string upgradeType = upgrades[i2].GetComponent<UpgradeCard>().type;
                    string upgradeLevel = upgrades[i2].GetComponent<UpgradeCard>().level;
                    if (upgrade[0] == upgradeType && upgrade[1] == upgradeLevel)
                    {
                        UpgradeCard card = Instantiate(upgrades[i2], upgradesParent.transform).GetComponent<UpgradeCard>();
                        card.savesManager = GetComponent<SavesManager>();
                        card.upgradeParent = upgradesParent;
                        card.dragObject = dragObject;
                        card.upgradeSlots = upgradeSlots;
                        card.AddToUpgrades();
                        buyableUpgrades.Remove(upgrades[i2]);
                    }
                }
            }
        }

        if (!isEmpty(DataBaseManager.upgradesActive[0]) || !isEmpty(DataBaseManager.upgradesActive[1]) || !isEmpty(DataBaseManager.upgradesActive[2]))
        {
            for (int i = 0; i < DataBaseManager.upgradesActive.Length; i++)
            {
                if (DataBaseManager.upgradesActive[i] != "none")
                {
                    string[] upgrade = DataBaseManager.upgradesActive[i].Split('|');
                    for (int i2 = 0; i2 < upgradesParent.transform.childCount; i2++)
                    {
                        string upgradeType = upgradesParent.transform.GetChild(i2).gameObject.GetComponent<UpgradeCard>().type;
                        string upgradeLevel = upgradesParent.transform.GetChild(i2).gameObject.GetComponent<UpgradeCard>().level;
                        if (upgrade[0] == upgradeType && upgrade[1] == upgradeLevel)
                        {
                            Transform card = upgradesParent.transform.GetChild(i2);
                            card.SetParent(upgradeSlots[i].transform);
                            card.gameObject.GetComponent<UpgradeCard>().RemoveFromUpgrades();
                        }
                    }
                }
            }
        }
    }

    private bool isEmpty(string str)
    {
        return str == "" || str == "none";
    }

    public void UpdateActiveUpgrades()
    {
        List<string> activeUpgrades = new List<string>();
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (upgradeSlots[i].transform.childCount == 1)
            {
                UpgradeCard cardProps = upgradeSlots[i].transform.GetChild(0).gameObject.GetComponent<UpgradeCard>();
                activeUpgrades.Add(cardProps.type + "|" + cardProps.level);
            }
            else
            {
                activeUpgrades.Add("none");
            }
        }
        DataBaseManager.upgradesActive = activeUpgrades.ToArray();
    }

    public void SortUpgradeCards()
    {
        Transform[] children = upgradesParent.transform.GetComponentsInChildren<Transform>();
        List<Transform> list = children.ToList();
        list.Remove(upgradesParent.transform);
        children = list.ToArray();
        children = children.OrderBy(o => o.gameObject.GetComponent<UpgradeCard>().level).ToArray();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    private void LoadShop()
    {
        commonUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().level == "1").ToList();
        rareUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().level == "2").ToList();
        legendaryUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().level == "3").ToList();
        if (isEmpty(DataBaseManager.shopItems[0]) && isEmpty(DataBaseManager.shopItems[1]) && isEmpty(DataBaseManager.shopItems[2]))
        {
            RefreshShop();
        }
        else
        {
            LoadShopState();
        }
    }

    private void RefreshShop()
    {
        int iteration = 0;
        int i = 0;
        int i2 = 0;
        while (i < shopSlots.Length && iteration < 5)
        {
            iteration++;
            GameObject card;
            int price;
            start:
            List<GameObject> upgradeList;
            int num = random.Next(0, 100);
            if (num >= 80)
            {
                upgradeList = legendaryUpgrades;
                price = 10000;
            }
            else if (num < 80 && num >= 50)
            {
                upgradeList = rareUpgrades;
                price = 5000;
            }
            else
            {
                upgradeList = commonUpgrades;
                price = 2000;
            }

            try
            {
                card = upgradeList[random.Next(0, upgradeList.Count)];
                upgradeList.Remove(card);
                i2++;
            }
            catch
            {
                if (commonUpgrades.Count > 0 || rareUpgrades.Count > 0 || legendaryUpgrades.Count > 0)
                {
                    goto start;
                }
                else
                {
                    continue;
                }
            }
            CreateCardInSlot(card, price, i);
            i = i2;
        }
        AfterShopLoaded();
    }

    private void LoadShopState()
    {
        for (int i = 0; i < DataBaseManager.shopItems.Length; i++)
        {
            string[] upgrade = DataBaseManager.shopItems[i].Split('|');
            for (int i2 = 0; i2 < buyableUpgrades.Count; i2++)
            {
                if (DataBaseManager.shopItems[i] != "none")
                {
                    string upgradeType = buyableUpgrades[i2].GetComponent<UpgradeCard>().type;
                    string upgradeLevel = buyableUpgrades[i2].GetComponent<UpgradeCard>().level;
                    if (upgrade[0] == upgradeType && upgrade[1] == upgradeLevel)
                    {
                        if (upgradeLevel == "3")
                        {
                            CreateCardInSlot(buyableUpgrades[i2], 10000, i);
                        }
                        else if (upgradeLevel == "2")
                        {
                            CreateCardInSlot(buyableUpgrades[i2], 5000, i);
                        }
                        else
                        {
                            CreateCardInSlot(buyableUpgrades[i2], 2000, i);
                        }
                    }
                }
            }
        }
        AfterShopLoaded();
    }

    private void CreateCardInSlot(GameObject card, int price, int slotIndex)
    {
        ShopButton shopButton = shopSlots[slotIndex].GetComponent<ShopButton>();
        Text cardPrice = shopSlots[slotIndex].transform.GetChild(1).GetComponent<Text>();
        UpgradeCard instanceProps = Instantiate(card, shopSlots[slotIndex].transform).GetComponent<UpgradeCard>();

        instanceProps.savesManager = GetComponent<SavesManager>();
        instanceProps.upgradeParent = upgradesParent;
        instanceProps.dragObject = dragObject;
        instanceProps.upgradeSlots = upgradeSlots;
        instanceProps.transform.SetSiblingIndex(2);
        shopButton.hasCard = true;
        shopButton.price = price;
        cardPrice.text = string.Format("{0:n0}", price);
        buyableUpgrades.Remove(card);
    }

    private void AfterShopLoaded()
    {
        List<string> shopItems = new List<string>();
        for (int i3 = 0; i3 < shopSlots.Length; i3++)
        {
            Transform child2 = shopSlots[i3].transform.GetChild(2);
            if (shopSlots[i3].GetComponent<ShopButton>().hasCard == false)
            {
                child2.GetComponent<Button>().interactable = false;
                shopSlots[i3].transform.GetChild(1).gameObject.SetActive(false);
                shopItems.Add("none");
            }
            else
            {
                UpgradeCard cardProps = child2.GetComponent<UpgradeCard>();
                shopItems.Add(cardProps.type + "|" + cardProps.level);
            }
        }
        DataBaseManager.shopItems = shopItems.ToArray();
    }
}
