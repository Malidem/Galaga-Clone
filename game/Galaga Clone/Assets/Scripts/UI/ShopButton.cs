using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public SavesManager savesManager;

    [HideInInspector]
    public int price;
    [HideInInspector]
    public bool hasCard;

    private GameObject card;
    private UpgradeCard cardProps;

    private Transform confirmMenu;
    private Button confirmButton;
    private Text confirmButtonText;

    private void OnEnable()
    {
        card = transform.GetChild(2).gameObject;
        cardProps = card.GetComponent<UpgradeCard>();
        confirmMenu = transform.parent.GetChild(7);
        confirmButton = confirmMenu.GetChild(5).GetComponent<Button>();
        confirmButtonText = confirmButton.gameObject.transform.GetChild(0).GetComponent<Text>();
    }

    public void ShopSlotButton()
    {
        confirmButtonText.text = string.Format("{0:n0}", price);
        confirmMenu.GetChild(2).GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
        confirmMenu.GetChild(3).GetComponent<Text>().text = cardProps.title;
        confirmMenu.GetChild(4).GetComponent<Text>().text = cardProps.discription;
        confirmButton.onClick.RemoveAllListeners();
        if (Constants.money >= price)
        {
            confirmButtonText.color = new Color(0.1960784F, 0.1960784F, 0.1960784F);
            confirmButton.onClick.AddListener(delegate { ConfirmPurchaseButton(); });
        }
        else
        {
            confirmButtonText.color = new Color(1, 0, 0);
        }
        confirmMenu.gameObject.SetActive(true);
    }

    public void ConfirmPurchaseButton()
    {
        List<string> unlockedCards = new List<string>(Constants.upgradesUnlocked);
        List<string> shopItems = new List<string>(Constants.shopItems);
        string asString = cardProps.type + "|" + cardProps.level;
        int index = shopItems.IndexOf(asString);

        transform.GetChild(3).GetComponent<Button>().interactable = false;
        transform.GetChild(1).gameObject.SetActive(false);
        card.transform.SetParent(savesManager.upgradesParent.transform, false);
        cardProps.AddToUpgrades();
        Constants.money -= price;
        savesManager.UpdateMoney();
        if (string.Join(",", Constants.upgradesUnlocked) == "none")
        {
            unlockedCards.Clear();
        }
        unlockedCards.Add(asString);
        Constants.upgradesUnlocked = unlockedCards.ToArray();
        shopItems[index] = "none";
        Constants.shopItems = shopItems.ToArray();
        hasCard = false;
        confirmMenu.gameObject.SetActive(false);
    }
}
