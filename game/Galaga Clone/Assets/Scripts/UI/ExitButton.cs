using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExitButton : MonoBehaviour
{
    public Sprite normal;
    public Sprite hover;
    private GameObject image;

    void Start()
    {
        image = GetComponentsInChildren<Transform>()[1].gameObject;
    }

    void Update()
    {
        if (IsMouseOver())
        {
            image.GetComponent<Image>().sprite = hover;
        }
        else
        {
            image.GetComponent<Image>().sprite = normal;
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
            if (raycastResultsList[i].gameObject != gameObject && raycastResultsList[i].gameObject != image)
            {
                raycastResultsList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultsList.Count > 0;
    }
}
