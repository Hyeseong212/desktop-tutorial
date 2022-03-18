using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class MenuManager : MonoBehaviour
{
    [Header("����������")]
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
    [Header("�׽�Ʈ��")]
    public List<Item> UsingItem;

    public int slotNumber;
    bool isExplainPanelOn = true;

    public string curType = "Equipment";
    bool isExist;
    
    GameManager gm;
    Status status;

    bool iscursorOn = false;


    [Header("�����������")]
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
        // ���� ������ ����Ʈ�� Ŭ���� Ÿ�Ը� �߰�
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        curType = tabName;
        gm.CurItemList = gm.MyItemList.FindAll(x => x.Type == tabName);
        for (int i = 0; i< Slots.Length; i++)
        {
            // ���԰� �ؽ�Ʈ ���̱�
            isExist = i < gm.CurItemList.Count;
            Slots[i].SetActive(isExist);
            Slots[i].GetComponentInChildren<Text>().text = isExist ? gm.CurItemList[i].Name : "";

            // ������ �̹����� ��������� ���̱�
            if (isExist)
            {
                ItemImage[i].sprite = ItemSprite[gm.AllItemList.FindIndex(x => x.Name == gm.CurItemList[i].Name)];
                ItemUsingCheck();
                UsingImage[i].SetActive(gm.CurItemList[i].isUsing);
            }
        }

        // �� �̹���
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
            // ��ü���� ���� �������� ã�� �� �����ۿ� �߰�
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
        Item BasicItem = gm.AllItemList.Find(x => x.Name == "���ǽ���");
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
                ExplainPanel.transform.GetChild(3).GetComponent<Text>().text = gm.CurItemList[slotNum].Number + "��";
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
                    if (hit) //�巡���� �������� ��� ĭ���� Ȯ��
                    {
                        if (gm.CurEquipList != null) //���� �԰� �ִ� ��� ���� �� ��� ���ȹ� �̹��� ����
                        {
                            if (curEquipItem.Type == "Weapon")
                            {
                                if (hit.collider.tag == "LeftWeapon")
                                {
                                    if (sprite == LhWeaponImage.sprite || gm.CurEquipList.Find(x => x.IsLeft == true) != curEquipItem)
                                    {
                                        gm.CurEquipList.Remove(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true));//���� �԰��ִ� ������ ����Ʈ���� ����
                                    }
                                    LhWeaponImage.sprite = sprite;//�̹�������
                                    curEquipItem.IsLeft = true;//���Ⱑ �������� �ƴ��� �Ǻ�
                                    for (int j = 0; j < gm.CurEquipList.Count; j++)
                                    {
                                        if (curEquipItem.Index == gm.CurEquipList[j].Index)//������� �ٽ� ������ ����
                                        {
                                            return;
                                        }
                                        else if(a ==0)
                                        {
                                            gm.CurEquipList.Add(curEquipItem);//������񸮽�Ʈ�� ������Ų ������ �߰�
                                            gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //�ߺ�����Ʈ ����
                                            gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                                            gm.Save(); //���� ��� ����Ʈ ���̺�
                                            status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
                                            a++;
                                        }
                                    }
                                }
                                else if (hit.collider.tag == "RightWeapon")
                                {
                                    if (sprite == RhWeaponImage.sprite || gm.CurEquipList.Find(x => x.IsLeft == true) != curEquipItem)
                                    {
                                        gm.CurEquipList.Remove(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false));//���� �԰��ִ� ������ ����Ʈ���� ����
                                    }
                                    RhWeaponImage.sprite = sprite;//�̹�������
                                    curEquipItem.IsLeft = false;//���Ⱑ �������� �ƴ��� �Ǻ�
                                    for (int j = 0; j < gm.CurEquipList.Count; j++)
                                    {
                                        if (curEquipItem.Index == gm.CurEquipList[j].Index)//������� �ٽ� ������ ����
                                        {
                                            return;
                                        }
                                        else if(a == 0)
                                        {
                                            gm.CurEquipList.Add(curEquipItem);//������񸮽�Ʈ�� ������Ų ������ �߰�
                                            gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //�ߺ�����Ʈ ����
                                            gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                                            gm.Save(); //���� ��� ����Ʈ ���̺�
                                            status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
                                            a++;
                                        }
                                    }
                                }
                            }//����
                            else if (curEquipItem.Type == "helmet" && hit.collider.tag == "Helmet")
                            {
                                HelmetImage.sprite = sprite;//�̹�������
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "helmet"));//���� �԰��ִ� ������ ����Ʈ���� ����
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//������� �ٽ� ������ ����
                                    {
                                        return;
                                    }
                                    else if(a==0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//������񸮽�Ʈ�� ������Ų ������ �߰�
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //�ߺ�����Ʈ ����
                                        gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                                        gm.Save(); //���� ��� ����Ʈ ���̺�
                                        status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
                                        a++;
                                    }
                                }
                            }//���
                            else if (curEquipItem.Type == "body" && hit.collider.tag == "Body")
                            {
                                BodyImage.sprite = sprite; //�̹�������
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "body")); //���� �԰��ִ� ������ ����Ʈ���� ����
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//������� �ٽ� ������ ����
                                    {
                                        return;
                                    }
                                    else if(a == 0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//������񸮽�Ʈ�� ������Ų ������ �߰�
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //�ߺ�����Ʈ ����
                                        gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                                        gm.Save(); //���� ��� ����Ʈ ���̺�
                                        status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
                                        a++;
                                    }
                                }
                            }//����
                            else if (curEquipItem.Type == "boots" && hit.collider.tag == "Boots")
                            {
                                BootsImage.sprite = sprite;//�̹�������
                                gm.CurEquipList.Remove(gm.CurEquipList.Find(x => x.Type == "boots"));//���� �԰��ִ� ������ ����Ʈ���� ����\
                                for (int j = 0; j < gm.CurEquipList.Count; j++)
                                {
                                    if (curEquipItem.Index == gm.CurEquipList[j].Index)//������� �ٽ� ������ ����
                                    {
                                        return;
                                    }
                                    else if(a == 0)
                                    {
                                        gm.CurEquipList.Add(curEquipItem);//������񸮽�Ʈ�� ������Ų ������ �߰�
                                        gm.CurEquipList = gm.CurEquipList.Distinct().ToList(); //�ߺ�����Ʈ ����
                                        gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                                        gm.Save(); //���� ��� ����Ʈ ���̺�
                                        status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
                                        a++;
                                    }
                                }
                            }//�Ź�
                            
                        }
                        else //���� �԰� �ִ� ��� ���� �� �״�� �������
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
                            gm.testSave(curEquipItem);//�����ͺ��̽��� ���� ���������� ���̺�
                            gm.Save();
                            status.EquipmentStatChange(curEquipItem);//�����԰��ִ� ��� ��������
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
//    // ��ü���� ���� �������� ã�� �� �����ۿ� �߰�
//    Item curAllItem = gm.AllItemList.Find(x => x.Name == ItemNameInput.text);
//    if (curAllItem != null)
//    {
//        curAllItem.Number = ItemNumberInput.text;
//        gm.MyItemList.Add(curAllItem);
//    }
//}