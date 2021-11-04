using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
    public string type;
    public string level;

    private bool isDragging;
    private bool isOverSlot;
    private GameObject startParent;
    private GameObject slot;
    private GameObject dragObject;
    private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        dragObject = GameObject.Find("DragObject");
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
        }
        else if (isOverSlot && slot.GetComponentsInChildren<Transform>().Length == 1) // 0 is the slot, 1 is the first child
        {
            transform.SetParent(slot.transform, false);
        }
        else
        {
            transform.SetParent(startParent.transform, true);
            transform.position = startPos;
        }
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
