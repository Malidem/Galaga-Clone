using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public void AddToUpgrades(GameObject upgrade)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        List<Transform> list = children.ToList();
        list.Remove(gameObject.transform);
        children = list.ToArray();
        Transform[] ordered = children.OrderBy(o => o.gameObject.GetComponent<UpgradeCard>().level).ToArray();

        for (int i = 0; i < ordered.Length; i++)
        {
            ordered[i].SetSiblingIndex(i);
        }
    }
}
