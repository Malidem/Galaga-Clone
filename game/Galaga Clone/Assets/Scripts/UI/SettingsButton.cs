using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsButton : MonoBehaviour
{
    private GameObject blank;
    private GameObject image;

    void Start()
    {
        blank = GetComponentsInChildren<Transform>()[1].gameObject;
        image = GetComponentsInChildren<Transform>()[2].gameObject;
    }

    void Update()
    {
        if (IsMouseOver())
        {
            image.transform.Rotate(Vector3.forward, 75 * Time.deltaTime);
        }
        else
        {
            image.transform.rotation = Quaternion.Euler(0, 0, 0);
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
            if (raycastResultsList[i].gameObject != gameObject && raycastResultsList[i].gameObject != blank)
            {
                raycastResultsList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultsList.Count > 0;
    }
}
