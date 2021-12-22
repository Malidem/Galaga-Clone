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

    public void ShopSlotButton()
    {
        Transform confirmMenu = transform.parent.GetChild(6);
        Button confirmButton = confirmMenu.GetChild(1).GetComponent<Button>();
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate { ConfirmPurchaseButton(); });
        confirmMenu.gameObject.SetActive(true);
    }

    public void ConfirmPurchaseButton()
    {
        if (DataBaseManager.money > price)
        {
            GameObject card = transform.GetChild(2).gameObject;
            UpgradeCard cardProps = card.GetComponent<UpgradeCard>();
            List<string> unlockedCards = new List<string>(DataBaseManager.upgradesUnlocked);
            List<string> shopItems = new List<string>(DataBaseManager.shopItems);
            string asString = cardProps.type + "|" + cardProps.level;
            int index = shopItems.IndexOf(asString);

            transform.GetChild(3).GetComponent<Button>().interactable = false;
            transform.GetChild(1).gameObject.SetActive(false);
            card.transform.SetParent(savesManager.upgradesParent.transform, false);
            cardProps.AddToUpgrades();
            DataBaseManager.money -= price;
            if (string.Join(",", DataBaseManager.upgradesUnlocked) == "none")
            {
                unlockedCards.Clear();
            }
            unlockedCards.Add(asString);
            DataBaseManager.upgradesUnlocked = unlockedCards.ToArray();
            shopItems[index] = "none";
            DataBaseManager.shopItems = shopItems.ToArray();
            hasCard = false;
            transform.parent.GetChild(6).gameObject.SetActive(false);
        }
    }
}
