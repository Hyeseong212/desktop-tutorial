using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class MenuManager : MonoBehaviour
{
    [Header("장착아이템")]
    public GameObject Helmet;
    public GameObject Body;
    public GameObject Boots;
    public GameObject RhWeapon;
    public GameObject LhWeapon;

    public GameObject StatMenu;
    public GameObject EquipmentMenu;
    public GameObject[] Slots, UsingImage;
    public GameObject ExplainPanel;
    public GameObject CursorObject;

    public Image[] TabImage, ItemImage;
    public Sprite[] ItemSprite;
    public Sprite SlotIdleSprite;
    public Sprite TabIdleSprite, TabSelectSprite;
    IEnumerator PointerCoroutine;

    public InputField ItemNameInput, ItemNumberInput;
    [Header("테스트용")]
    public List<Item> UsingItem;

    public int slotNumber;
    bool isExplainPanelOn = true;

    public string curType = "Equipment";
    bool isExist;
    
    GameManager gm;
    Status status;

    bool iscursorOn = false;


    [Header("장비장착슬롯")]
    public Image HelmetImage;
    public Image BodyImage;
    public Image BootsImage;
    public Image RhWeaponImage;
    public Image LhWeaponImage;
    public void EquipmentMenuOn()
    {
        StatMenu.SetActive(false);
        EquipmentMenu.SetActive(true);
    }
    public void StatMenuOn()
    {
        StatMenu.SetActive(true);
        EquipmentMenu.SetActive(false);
    }
    public void TabClick(string tabName)
    {
        // 현재 아이템 리스트에 클릭한 타입만 추가
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        curType = tabName;
        gm.CurItemList = gm.MyItemList.FindAll(x => x.Type == tabName);
        for (int i = 0; i< Slots.Length; i++)
        {
            // 슬롯과 텍스트 보이기
            isExist = i < gm.CurItemList.Count;
            Slots[i].SetActive(isExist);
            Slots[i].GetComponentInChildren<Text>().text = isExist ? gm.CurItemList[i].Name : "";

            // 아이템 이미지와 사용중인지 보이기
            if (isExist)
            {
                ItemImage[i].sprite = ItemSprite[gm.AllItemList.FindIndex(x => x.Name == gm.CurItemList[i].Name)];
                ItemUsingCheck();
                UsingImage[i].SetActive(gm.CurItemList[i].isUsing);
            }
        }

        // 탭 이미지
        int tabNum = 0;
        switch (tabName)
        {
            case "Equipment": tabNum = 0; break;
            case "Consumable": tabNum = 1; break;
        }
        for (int i = 0; i < TabImage.Length; i++)
        {
            TabImage[i].sprite = i == tabNum ? TabSelectSprite : TabIdleSprite;
        }
    }
    public void GetItemClick()
    {
        Item curItem = gm.MyItemList.Find(x => x.Name == ItemNameInput.text);
        ItemNumberInput.text = ItemNumberInput.text == "" ? "1" : ItemNumberInput.text;
        if(curItem != null)
        {
            curItem.Number = (int.Parse(curItem.Number) + int.Parse(ItemNumberInput.text)).ToString();
        }
        else
        {
            // 전체에서 얻을 아이템을 찾아 내 아이템에 추가
            Item curAllItem = gm.AllItemList.Find(x => x.Name == ItemNameInput.text);
            if (curAllItem != null) 
            {
                curAllItem.Number = ItemNumberInput.text;
                gm.MyItemList.Add(curAllItem); 
            }
        }
        gm.MyItemList.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));
        gm.Save();

    }
    public void RemoveItemClick()
    {
        Item curItem = gm.MyItemList.Find(x => x.Name == ItemNameInput.text);
        if (curItem != null)
        {
            int curNumber = int.Parse(curItem.Number) - int.Parse(ItemNumberInput.text);

            if (curNumber <= 0) gm.MyItemList.Remove(curItem);
            else curItem.Number = curNumber.ToString();
        }
        gm.MyItemList.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));
        gm.Save();
    }
    public void ResetItemClick()
    {
        Item BasicItem = gm.AllItemList.Find(x => x.Name == "신의심판");
        gm.MyItemList = new List<Item>() { BasicItem };
        gm.Save();
    }
    public void SlotClick(int slotNum)
    {
        Item CurItem = gm.CurItemList[slotNum];
        Item UsingItem = gm.CurItemList.Find(x => x.isUsing == true);
        if (curType == "Equipment")
        {
            ItemUsingCheck();
        }
        print("Index : " + CurItem.Index + ", " +  "Type : " + CurItem.Type);
        gm.Save();
    }
    public void PointerEnter(int slotNum)
    {
        slotNumber = slotNum;
        iscursorOn = true;
        PointerCoroutine = PointerEnterDelay(slotNum);
        StartCoroutine(PointerCoroutine);
    }
    IEnumerator PointerEnterDelay(int slotNum)
    {
        yield return 0;
        if (iscursorOn)
        {
            ExplainPanel.SetActive(true);

            ExplainPanel.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Slots[slotNum].GetComponent<RectTransform>().anchoredPosition.x - 270,
                Slots[slotNum].GetComponent<RectTransform>().anchoredPosition.y + 201);

            ExplainPanel.GetComponentInChildren<Text>().text = gm.CurItemList[slotNum].Name;
            ExplainPanel.transform.GetChild(2).GetComponent<Image>().sprite = Slots[slotNum].transform.GetChild(1).GetComponent<Image>().sprite;
            if (gm.CurItemList[slotNum].Type == "Consumable")
            {
                ExplainPanel.transform.GetChild(3).GetComponent<Text>().text = gm.CurItemList[slotNum].Number + "개";
            }
            else
            {
                ExplainPanel.transform.GetChild(3).GetComponent<Text>().text = null;
            }
            ExplainPanel.transform.GetChild(4).GetComponent<Text>().text = gm.CurItemList[slotNum].Explain;
        }
    }
    int q;
    public void ItemUsingCheck()
    {
        for (int i = 0; i < gm.CurItemList.Count; i++)
        {
            gm.CurItemList[i].isUsing = false;
        }
        for (int i = 0; i < gm.CurEquipList.Count; i++)
        {
            for (int j = 0; j < gm.CurItemList.Count; j++)
            {
                if (gm.CurEquipList[i].Index == gm.CurItemList[j].Index)
                {
                    gm.CurItemList[j].isUsing = true;
                }
            }
        }
    }

        public void PointerExit(int slotNum)
    {
        iscursorOn = false;
        ExplainPanel.SetActive(false);
    }
    int weaponNum = 0;
    public void EquipmentSlot(Sprite sprite)
    {
        int a = 0;
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        status = GameObject.Find("Status").GetComponent<Status>();
        EquipItem equipItem = new EquipItem("","","","","","",true,"","","","");
        for (int i = 0; i < ItemSprite.Length; i++)
        {
            if (sprite == ItemSprite[i])
            {
                equipItem.Index = i.ToString();
                EquipItem curEquipItem = gm.EquipmentList.Find(x => x.Index == equipItem.Index);
                if (curEquipItem != null)
                {
                    Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Ray2D ray = new Ray2D(wp, Vector2.zero);
                    LayerMask drawerLayer = 1 << LayerMask.NameToLayer("Equipment");
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10000,drawerLayer);
                    if (hit) //드래그한 아이템이 장비 칸인지 확인
                    {
                        if (gm.CurEquipList != null) //현재 입고 있는 장비가 있을 시 장비 스탯및 이미지 변경
                        {
                            if (curEquipItem.Type == "Weapon")
                            {
                                if (hit.collider.tag == "LeftWeapon")
                                {
                                    if (sprite == LhWeaponImage.sprite || gm.CurEquipList.Find(x => x.IsLeft == true) != curEquipItem)
                                    {
                                        gm.CurEquipList.Remove(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true));//전에 입고있던 아이템 리스트에서 제거
                                    }
                                    LhWeaponImage.sprite = sprite;//이미지변경
                                    curEquipItem.IsLeft = true;//무기가 왼쪽인지 아닌지 판별
                                    for (int j = 0; j < gm.CurEquipList.Count; j++)
                                    {
                                        if (curEquipItem.Index == gm.CurEquipList[j].Index)//같은장비를 다시 장착시 리턴
                                        {
                                            return;
                                        }
                                        else if(a ==0)
                                        {
                                            gm.CurEquipList.Add(curEquipItem);//현재장비리스트에 장착시킨 아이템 추가
                                            gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //중복리스트 제거
                                            gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                                            gm.Save(); //현재 모든 리스트 세이브
                                            status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                                            a++;
                                        }
                                    }
                                }
                                else if (hit.collider.tag == "RightWeapon")
                                {
                                    if (sprite == RhWeaponImage.sprite || gm.CurEquipList.Find(x => x.IsLeft == true) != curEquipItem)
                                    {
                                        gm.CurEquipList.Remove(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false));//전에 입고있던 아이템 리스트에서 제거
                                    }
                                    RhWeaponImage.sprite = sprite;//이미지변경
                                    curEquipItem.IsLeft = false;//무기가 왼쪽인지 아닌지 판별
                                    for (int j = 0; j < gm.CurEquipList.Count; j++)
                                    {
                                        if (curEquipItem.Index == gm.CurEquipList[j].Index)//같은장비를 다시 장착시 리턴
                                        {
                                            return;
                                        }
                                        else if(a == 0)
                                        {
                                            gm.CurEquipList.Add(curEquipItem);//현재장비리스트에 장착시킨 아이템 추가
                                            gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //중복리스트 제거
                                            gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                                            gm.Save(); //현재 모든 리스트 세이브
                                            status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                                            a++;
                                        }
                                    }
                                }
                            }//무기
                            else if (curEquipItem.Type == "helmet" && hit.collider.tag == "Helmet")
                            {
                                HelmetImage.sprite = sprite;//이미지변경
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "helmet"));//전에 입고있던 아이템 리스트에서 제거
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//같은장비를 다시 장착시 리턴
                                    {
                                        return;
                                    }
                                    else if(a==0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//현재장비리스트에 장착시킨 아이템 추가
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //중복리스트 제거
                                        gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                                        gm.Save(); //현재 모든 리스트 세이브
                                        status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                                        a++;
                                    }
                                }
                            }//헬멧
                            else if (curEquipItem.Type == "body" && hit.collider.tag == "Body")
                            {
                                BodyImage.sprite = sprite; //이미지변경
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "body")); //전에 입고있던 아이템 리스트에서 제거
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//같은장비를 다시 장착시 리턴
                                    {
                                        return;
                                    }
                                    else if(a == 0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//현재장비리스트에 장착시킨 아이템 추가
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //중복리스트 제거
                                        gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                                        gm.Save(); //현재 모든 리스트 세이브
                                        status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                                        a++;
                                    }
                                }
                            }//갑옷
                            else if (curEquipItem.Type == "boots" && hit.collider.tag == "Boots")
                            {
                                BootsImage.sprite = sprite;//이미지변경
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "boots"));//전에 입고있던 아이템 리스트에서 제거\
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//같은장비를 다시 장착시 리턴
                                    {
                                        return;
                                    }
                                    else if(a == 0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//현재장비리스트에 장착시킨 아이템 추가
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //중복리스트 제거
                                        gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                                        gm.Save(); //현재 모든 리스트 세이브
                                        status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                                        a++;
                                    }
                                }
                            }//신발
                            
                        }
                        else //현재 입고 있는 장비가 없을 시 그대로 장비적용
                        {
                            if (curEquipItem.Type == "Weapon")
                            {
                                if (hit.collider.tag == "LeftWeapon")
                                {
                                    LhWeaponImage.sprite = sprite;
                                    curEquipItem.IsLeft = true;
                                }
                                else if (hit.collider.tag == "RightWeapon")
                                {
                                    RhWeaponImage.sprite = sprite;
                                    curEquipItem.IsLeft = false;
                                }
                            }
                            else if (curEquipItem.Type == "helmet" && hit.collider.tag == "Helmet")
                            {
                                HelmetImage.sprite = sprite;
                            }
                            else if (curEquipItem.Type == "body" && hit.collider.tag == "Body")
                            {
                                BodyImage.sprite = sprite;
                            }
                            else if (curEquipItem.Type == "boots" && hit.collider.tag == "Boots")
                            {
                                BootsImage.sprite = sprite;
                            }
                            gm.CurEquipList.Add(curEquipItem);
                            gm.CurEquipList = gm.CurEquipList.Distinct().ToList();
                            gm.testSave(curEquipItem);//데이터베이스에 현재 장착아이템 세이브
                            gm.Save();
                            status.EquipmentStatChange(curEquipItem);//현재입고있는 장비 스탯적용
                        }
                    }
                    else return;
                }
            }
        }
        CursorObject.SetActive(false);
    }

    public void EquipmentItemSave()
    {
        string jdata = JsonConvert.SerializeObject(gm.CurEquipList);
        File.WriteAllText(Application.dataPath + "/Data/PotpolioMyEquipDatabase.txt", jdata);
    }
    public void EquimentItemLoad()
    {
        if (gm.CurEquipList != null)
        {
            for (int i = 0; i < gm.CurEquipList.Count; i++)
            {
                if (gm.CurEquipList[i].Type == "helmet")
                    HelmetImage.sprite = ItemSprite[gm.AllItemList.FindIndex(a => a.Index == gm.CurEquipList[i].Index)];
                if (gm.CurEquipList[i].Type == "body")
                    BodyImage.sprite = ItemSprite[gm.AllItemList.FindIndex(a => a.Index == gm.CurEquipList[i].Index)];
                if (gm.CurEquipList[i].Type == "boots")
                    BootsImage.sprite = ItemSprite[gm.AllItemList.FindIndex(a => a.Index == gm.CurEquipList[i].Index)];
                if (gm.CurEquipList[i].Type == "Weapon" && gm.CurEquipList[i].IsLeft == true)
                    LhWeaponImage.sprite = ItemSprite[gm.AllItemList.FindIndex(a => a.Index == gm.CurEquipList[i].Index)];
                if (gm.CurEquipList[i].Type == "Weapon" && gm.CurEquipList[i].IsLeft == false)
                    RhWeaponImage.sprite = ItemSprite[gm.AllItemList.FindIndex(a => a.Index == gm.CurEquipList[i].Index)];
            }
        }
        else return;
    }
}
//{
//    // 전체에서 얻을 아이템을 찾아 내 아이템에 추가
//    Item curAllItem = gm.AllItemList.Find(x => x.Name == ItemNameInput.text);
//    if (curAllItem != null)
//    {
//        curAllItem.Number = ItemNumberInput.text;
//        gm.MyItemList.Add(curAllItem);
//    }
//}