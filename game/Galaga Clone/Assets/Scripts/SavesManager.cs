﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SavesManager : MonoBehaviour
{
    public GameObject upgradesParent;
    public GameObject dragObject;
    public GameObject scroller;
    public Sprite completedLevel;
    public Sprite unlockedLevel;
    public Text moneyText;
    public GameObject[] upgradeSlots;
    public GameObject[] shopSlots;
    public List<GameObject> levels = new List<GameObject>();
    public List<GameObject> upgrades = new List<GameObject>();
    public List<GameObject> buyableUpgrades = new List<GameObject>();
    public List<GameObject> commonUpgrades = new List<GameObject>();
    public List<GameObject> rareUpgrades = new List<GameObject>();
    public List<GameObject> legendaryUpgrades = new List<GameObject>();

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        buyableUpgrades = upgrades.ToList();
        StartCoroutine(getSaveData());
        Rect rect = scroller.GetComponent<RectTransform>().rect;
        scroller.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        scroller.GetComponent<BoxCollider2D>().size = new Vector2(rect.width / scroller.transform.lossyScale.x, rect.height / scroller.transform.lossyScale.y);
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
            DataBaseManager.levelsUnlocked = int.Parse(data[2]);
            DataBaseManager.levelsCompleted = int.Parse(data[3]);
            DataBaseManager.upgradesUnlocked = data[4].Split(',');
            DataBaseManager.upgradesActive = data[5].Split(',');
            transform.GetChild(3).GetChild(2).GetComponent<Text>().text = "Money: " + DataBaseManager.money;
            LoadLevels();
            CreateUpgrades();
            commonUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().rarity == UpgradeCard.Rarities.Common).ToList();
            rareUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().rarity == UpgradeCard.Rarities.Rare).ToList();
            legendaryUpgrades = buyableUpgrades.Where(x => x.GetComponent<UpgradeCard>().rarity == UpgradeCard.Rarities.Legendary).ToList();
            RefreshShop();
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
            levels[i].GetComponent<LevelHover>().isUnlocked = false;
        }
    }

    private void CreateUpgrades()
    {
        if (DataBaseManager.upgradesUnlocked[0].Split('|')[0] != "none")
        {
            for (int i = 0; i < DataBaseManager.upgradesUnlocked.Length; i++)
            {
                string[] upgrade = DataBaseManager.upgradesUnlocked[i].Split('|');
                for (int i2 = 0; i2 < upgrades.Count; i2++)
                {
                    string upgradeType = upgrades[i2].GetComponent<UpgradeCard>().type;
                    string upgradeLevel = upgrades[i2].GetComponent<UpgradeCard>().level;
                    if (upgrade[0] == upgradeType && upgrade[1] == upgradeLevel)
                    {
                        UpgradeCard card = Instantiate(upgrades[i], upgradesParent.transform).GetComponent<UpgradeCard>();
                        card.savesManager = GetComponent<SavesManager>();
                        card.upgradeParent = upgradesParent;
                        card.dragObject = dragObject;
                        card.upgradeSlots = upgradeSlots;
                        card.AddToUpgrades();
                        buyableUpgrades.Remove(upgrades[i]);
                    }
                }
            }

            if (!isEmpty(DataBaseManager.upgradesActive[0]) || !isEmpty(DataBaseManager.upgradesActive[1]) || !isEmpty(DataBaseManager.upgradesActive[2]))
            {
                for (int i = 0; i < DataBaseManager.upgradesActive.Length; i++)
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

    // CLEAN UP THIS METHOD!!! + shorten
    public void RefreshShop()
    {
        int iteration = 0;
        int i = 0;
        int i2 = 0;
        while (i < shopSlots.Length && iteration < 5)
        {
            iteration++;
            ShopButton shopButton = shopSlots[i].GetComponent<ShopButton>();
            Text cardPrice = shopSlots[i].transform.GetChild(1).GetComponent<Text>();
            GameObject card;
            int price;
            start:
            int num = random.Next(0, 100);
            if (num >= 80)
            {
                try
                {
                    card = legendaryUpgrades[random.Next(0, legendaryUpgrades.Count)];
                    legendaryUpgrades.Remove(card);
                    price = 10000;
                    i2++;
                }
                catch
                {
                    if (ShopIfs())
                    {
                        goto start;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if (num < 80 && num >= 50)
            {
                try
                {
                    card = rareUpgrades[random.Next(0, rareUpgrades.Count)];
                    rareUpgrades.Remove(card);
                    price = 5000;
                    i2++;
                }
                catch
                {
                    if (ShopIfs())
                    {
                        goto start;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                try
                {
                    card = commonUpgrades[random.Next(0, commonUpgrades.Count)];
                    commonUpgrades.Remove(card);
                    price = 2000;
                    i2++;
                }
                catch
                {
                    if (ShopIfs())
                    {
                        goto start;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            UpgradeCard instanceProps = Instantiate(card, shopSlots[i].transform).GetComponent<UpgradeCard>();
            instanceProps.savesManager = GetComponent<SavesManager>();
            instanceProps.upgradeParent = upgradesParent;
            instanceProps.dragObject = dragObject;
            instanceProps.upgradeSlots = upgradeSlots;
            instanceProps.transform.SetSiblingIndex(2);
            shopButton.hasCard = true;
            shopButton.price = price;
            cardPrice.text = string.Format("{0:n0}", price);
            i = i2;
        }
        if (iteration == 5)
        {
            for (int i3 = 0; i3 < shopSlots.Length; i3++)
            {
                if (shopSlots[i3].GetComponent<ShopButton>().hasCard == false)
                {
                    shopSlots[i3].transform.GetChild(2).GetComponent<Button>().interactable = false;
                    shopSlots[i3].transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    private bool ShopIfs()
    {
        return commonUpgrades.Count > 0 || rareUpgrades.Count > 0 || legendaryUpgrades.Count > 0;
    }
}
