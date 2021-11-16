using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public string type;
    public string level;

    private bool isDragging;
    private bool isOverSlot;
    private GameObject startParent;
    private GameObject slot;
    private Vector2 startPos;

    [HideInInspector]
    public UpgradesManager upgradesManager;
    [HideInInspector]
    public GameObject dragObject;
    [HideInInspector]
    public GameObject upgradeParent;
    [HideInInspector]
    public Scrollbar scrollbar;

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
        else if (isOverSlot && slot.GetComponentsInChildren<Transform>().Length == 1) // 0 is the slot, 1 is the first child
        {
            transform.SetParent(slot.transform, false);
        }
        else
        {
            transform.SetParent(startParent.transform, true);
            transform.position = startPos;

            if (startParent.name == "Upgrades")
            {
                AddToUpgrades();
            }
        }
    }

    public void AddToUpgrades()
    {
        RectTransform rect = upgradeParent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width + 150, rect.sizeDelta.y);
        rect.position = new Vector2(rect.position.x + 75, rect.position.y);
        upgradesManager.SortUpgradeCards(gameObject);
        scrollbar.value = 0;
    }

    public void RemoveFromUpgrades()
    {
        RectTransform rect = upgradeParent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width - 150, rect.sizeDelta.y);
        rect.position = new Vector2(rect.position.x - 75, rect.position.y);
        scrollbar.value = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverSlot = true;
        slot = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverSlot = false;
        slot = null;
    }
}
