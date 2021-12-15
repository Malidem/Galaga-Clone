using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public int price;
    public SavesManager savesManager;

    [HideInInspector]
    public bool hasCard;

    public void ShopSlotButton()
    {
        if (DataBaseManager.money > price)
        {
            GameObject card = transform.GetChild(2).gameObject;
            UpgradeCard cardProps = card.GetComponent<UpgradeCard>();
            List<string> unlockedCards = new List<string>(DataBaseManager.upgradesUnlocked);

            transform.GetChild(3).GetComponent<Button>().interactable = false;
            transform.GetChild(1).gameObject.SetActive(false);
            card.transform.SetParent(savesManager.upgradesParent.transform, false);
            cardProps.AddToUpgrades();
            DataBaseManager.money -= price;
            unlockedCards.Add(cardProps.type + "|" + cardProps.level);
            DataBaseManager.upgradesUnlocked = unlockedCards.ToArray();
            hasCard = false;
        }
    }
}
