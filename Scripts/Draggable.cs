using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public static Vector2 DefaultPos;
    public Sprite IdleSprite;
    public GameObject cursorObject;
    public RaycastHit2D hit;
    public int slotNumber;
    MenuManager mm;
    GameManager gm;
    private void Start()
    {
        mm = GameObject.Find("Equipment,Stat").GetComponent<MenuManager>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!gm.CurItemList[mm.slotNumber].isUsing)
        {
            cursorObject.SetActive(true);
            DefaultPos = cursorObject.transform.position;
            cursorObject.GetComponent<Image>().sprite = this.transform.GetChild(1).GetComponent<Image>().sprite;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObject.transform.position = mousePos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        cursorObject.GetComponent<Image>().sprite = IdleSprite;
        cursorObject.transform.position = DefaultPos;
    }
}