using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
    private bool isDragging;
    private bool isOverSlot;
    private GameObject startParent;
    private GameObject slot;
    private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
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

        if (isOverSlot)
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
        if (collision.gameObject.CompareTag("CardSlot"))
        {
            isOverSlot = true;
            slot = collision.gameObject;
        }
    }
}
