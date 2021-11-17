using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public GameObject upgradesParent;
    public GameObject upgradeSlot1;
    public GameObject upgradeSlot2;
    public GameObject upgradeSlot3;
    public GameObject dragObject;
    public GameObject scroller;
    public List<GameObject> UpgradeCards = new List<GameObject>();

    void Start()
    {
        Rect rect = scroller.GetComponent<RectTransform>().rect;
        scroller.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        scroller.GetComponent<BoxCollider2D>().size = new Vector2(rect.width / scroller.transform.lossyScale.x, rect.height / scroller.transform.lossyScale.y);
        LoadActiveUpgrades();
    }

    private void LoadActiveUpgrades()
    {
        PlayerPrefs.SetString("activeUpgrades", "Gun,1|Speed,2|Overheat,3");
        PlayerPrefs.SetString("unlockedUpgrades", "Gun,1|Speed,2|Overheat,3|None,4|None,5|None,6|None,7|None,8");

        string[] activeUpgrades = PlayerPrefs.GetString("activeUpgrades").Split('|');
        string[] unlockedUpgrades = PlayerPrefs.GetString("unlockedUpgrades").Split('|');

        for (int i = 0; i < unlockedUpgrades.Length; i++)
        {
            string upgradeType = unlockedUpgrades[i].Split(',')[0];
            string upgradeLevel = unlockedUpgrades[i].Split(',')[1];
            for (int i2 = 0; i2 < UpgradeCards.Count; i2++)
            {
                if (UpgradeCards[i2].GetComponent<UpgradeCard>().type == upgradeType && UpgradeCards[i2].GetComponent<UpgradeCard>().level == upgradeLevel)
                {
                    UpgradeCard card = Instantiate(UpgradeCards[i], upgradesParent.transform).GetComponent<UpgradeCard>();
                    card.upgradesManager = gameObject.GetComponent<UpgradesManager>();
                    card.dragObject = dragObject;
                    card.upgradeParent = upgradesParent;
                    card.AddToUpgrades();
                }
            }
        }

        for (int i = 0; i < activeUpgrades.Length; i++)
        {
            for (int i2 = 0; i2 < upgradesParent.transform.childCount; i2++)
            {
                Transform card = upgradesParent.transform.GetChild(i2);
                UpgradeCard cardProps = card.GetComponent<UpgradeCard>();

                if (cardProps.type == activeUpgrades[i].Split(',')[0] && cardProps.level == activeUpgrades[i].Split(',')[1])
                {
                    if (upgradeSlot1.transform.childCount < 1)
                    {
                        card.SetParent(upgradeSlot1.transform);
                        cardProps.RemoveFromUpgrades();
                    }
                    else if (upgradeSlot2.transform.childCount < 1)
                    {
                        card.SetParent(upgradeSlot2.transform);
                        cardProps.RemoveFromUpgrades();
                    }
                    else if (upgradeSlot3.transform.childCount < 1)
                    {
                        card.SetParent(upgradeSlot3.transform);
                        cardProps.RemoveFromUpgrades();
                    }
                }
            }
        }
    }

    public void SortUpgradeCards(GameObject upgrade)
    {
        Transform[] children = upgradesParent.transform.GetComponentsInChildren<Transform>();
        List<Transform> list = children.ToList();
        list.Remove(upgradesParent.transform);
        children = list.ToArray();
        Transform[] ordered = children.OrderBy(o => o.gameObject.GetComponent<UpgradeCard>().level).ToArray();

        for (int i = 0; i < ordered.Length; i++)
        {
            ordered[i].SetSiblingIndex(i);
        }
    }
}
