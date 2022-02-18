using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelHover : MonoBehaviour
{
    public GameObject visable;
    public bool isUnlocked;

    void Update()
    {
        if (isUnlocked)
        {
            if (IsMouseOver())
            {
                visable.SetActive(true);
            }
            else
            {
                visable.SetActive(false);
            } 
        }
    }

    private bool IsMouseOver()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            if (raycastResultsList[i].gameObject != gameObject && raycastResultsList[i].gameObject != visable)
            {
                raycastResultsList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultsList.Count > 0;
    }
}
