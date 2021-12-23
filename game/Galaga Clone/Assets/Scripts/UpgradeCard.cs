using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
    public string type;
    public string level;
    public string title;
    [TextArea(3, 10)] public string discription;
    public Rarities rarity;

    private bool isDragging;
    private bool isOverSlot;
    private GameObject startParent;
    private GameObject slot;
    private Vector2 startPos;

    [HideInInspector]
    public SavesManager savesManager;
    [HideInInspector]
    public GameObject dragObject;
    [HideInInspector]
    public GameObject upgradeParent;
    [HideInInspector]
    public GameObject[] upgradeSlots;

    public enum Rarities
    {
        Common = 0,
        Rare = 1,
        Legendary = 2,
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.SetParent(dragObject.transform, true);
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public void OnDragStart()
    {
        if (isDragging)
        {
            return;
        }

        if (transform.parent.name == "Upgrades")
        {
            RemoveFromUpgrades();
        }

        startParent = transform.parent.gameObject;
        startPos = transform.position;
        isDragging = true;
    }

    public void OnDragEnd()
    {
        isDragging = false;
        if (isOverSlot && slot.name == "Upgrades")
        {
            transform.SetParent(slot.transform, false);
            AddToUpgrades();
        }
        else if (isOverSlot && slot.name != "Upgrades")
        {
            if (CanBeActivated())
            {
                if (slot.transform.childCount > 0)
                {
                    MoveCardToStart(slot.transform.GetChild(0));
                }
                transform.SetParent(slot.transform, false);
            }
            else
            {
                MoveCardToStart(gameObject.transform);
            }
        }
        else
        {
            MoveCardToStart(gameObject.transform);
        }
        savesManager.UpdateActiveUpgrades();
    }

    private void MoveCardToStart(Transform card)
    {
        card.SetParent(startParent.transform, true);
        card.position = startPos;

        if (startParent.name == "Upgrades")
        {
            AddToUpgrades();
        }
    }

    private bool CanBeActivated()
    {
        int num = 0;
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (upgradeSlots[i].transform.childCount > 0 && upgradeSlots[i] != slot)
            {
                if (upgradeSlots[i].transform.GetChild(0).gameObject.GetComponent<UpgradeCard>().type != type)
                {
                    num++;
                }
            }
            else
            {
                num++;
            }
        }

        if (num == upgradeSlots.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddToUpgrades()
    {
        RectTransform rect = upgradeParent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width + 150, rect.sizeDelta.y);
        rect.position = new Vector2(rect.position.x + 75, rect.position.y);
        savesManager.SortUpgradeCards();
    }

    public void RemoveFromUpgrades()
    {
        RectTransform rect = upgradeParent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width - 150, rect.sizeDelta.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverSlot = true;
        if (collision.gameObject.name == "UpgradeScroller")
        {
            slot = upgradeParent;
        }
        else
        {
            slot = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverSlot = false;
        slot = null;
    }
}
