using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSystem : MonoBehaviour , IDropHandler
{
    MenuManager Mm;
    Sprite CurImage;
    bool isEquip;
    RaycastHit2D hit;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData != null)
        {
            Mm = GameObject.Find("Equipment,Stat").GetComponent<MenuManager>();
            CurImage = gameObject.GetComponent<Image>().sprite;
            Mm.EquipmentSlot(CurImage);
        }
        else return;
    }

}
