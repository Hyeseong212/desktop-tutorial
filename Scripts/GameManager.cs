using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

[System.Serializable]
public class Item
{
    public Item
    (string _Type, string _Index, string _Name, string _Explain, string _Number, bool _isUsing)
    { Type = _Type; Index = _Index; Name = _Name; Explain = _Explain; Number = _Number; isUsing = _isUsing; }
    public string Type, Index, Name, Explain, Number;
    public bool isUsing;
}
[System.Serializable]
public class EquipItem
{
    public EquipItem
    (string _Index, string _Type, string _Name, string _Attackpt, string _Cirticalpt,  string _AttackSpeed, bool _IsLeft, string _Defpt, string _Speed, string _AtkPlusValue, string _AtkLate)
    { 
        Index = _Index; 
        Type = _Type; 
        Name = _Name; 
        Attackpt = _Attackpt; 
        Cirticalpt = _Cirticalpt; 
        AttackSpeed = _AttackSpeed; 
        IsLeft = _IsLeft;
        Defpt = _Defpt;
        Speed = _Speed;
        AtkPlusValue = _AtkPlusValue;
        AtkLate = _AtkLate;
    }
    public string Index, Type, Name, Attackpt, Cirticalpt, AttackSpeed, Defpt, Speed, AtkPlusValue, AtkLate; 
    public bool IsLeft;
}
public class GameManager : MonoBehaviour
{
    [Header("���ӽ��ھ����")]
    [SerializeField] GameObject stageClearPanel;
    [SerializeField] Text stageNum;
    [SerializeField] Text scoreText;
    public float score = 0;
    public float targetScore = 1;
    public float stageNumber;


    [Header("������")]
    public GameObject equipmentPanel;
    public bool isEquipmentPanelOn;

    Status status;
    MonsterSpawn monsterSpawn;
    [Header("�����۰���")]
    public TextAsset ItemDatabase;
    public TextAsset EquipmentDatabase;
    public List<Item> AllItemList, MyItemList, CurItemList;
    public List<EquipItem> EquipmentList, CurEquipList;

    public GameObject pauseMenu;
    public bool isPauseMenuOn;

    MenuManager mg;
    StatusMenu sm;

    TestDataHandler tdh;
    private void Awake()
    {
        //�����ۺκ�
        string[] line = ItemDatabase.text.Substring(0, ItemDatabase.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');

            AllItemList.Add(new Item(row[0], row[1], row[2], row[3], row[4], row[5] == "TRUE"));
        }
        //���κ�
        string[] Equipment = EquipmentDatabase.text.Substring(0, EquipmentDatabase.text.Length - 1).Split('\n');
        for (int i = 0; i < Equipment.Length; i++)
        {
            string[] row = Equipment[i].Split('\t');

            EquipmentList.Add(new EquipItem(row[0], row[1], row[2], row[3], row[4], row[5], row[6] == "TRUE", row[7], row[8], row[9], row[10]));
        }
        if (CurEquipList == null)
        {
            CurEquipList = new List<EquipItem>();
        }
        Load();
        testLoad();
        mg.ItemUsingCheck();
        mg.ItemUsingCheck();

    }
    private void Start()
    {
        stageNumber = 1;
        targetScore = 1;
        status = GameObject.Find("Status").GetComponent<Status>();
        sm = GameObject.Find("Status").GetComponent<StatusMenu>();
        monsterSpawn = GameObject.Find("SpawnPoint").GetComponent<MonsterSpawn>();
        stageNum.text = "STAGE " + stageNumber.ToString();


        equipmentPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isEquipmentPanelOn = !isEquipmentPanelOn;
            equipmentPanel.SetActive(isEquipmentPanelOn);
            if (isEquipmentPanelOn)
            {
                mg.ItemUsingCheck();
                Load();
                Time.timeScale = 0;
            }
            else
                Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseMenuOn = !isPauseMenuOn;
            pauseMenu.SetActive(isPauseMenuOn);
            if (isPauseMenuOn)
            {
                Time.timeScale = 0;
            }
            else
                Time.timeScale = 1;
        }

    }
    public void Save()
    {
        string jdata = JsonConvert.SerializeObject(MyItemList);
        File.WriteAllText(Application.dataPath + "/Data/PotpolioMyDatabase.txt", jdata);
        mg.TabClick(mg.curType);
    }
    public void Load()
    {
        mg = GameObject.Find("Equipment,Stat").GetComponent<MenuManager>();
        string jdata = File.ReadAllText(Application.dataPath + "/Data/PotpolioMyDatabase.txt");
        MyItemList = JsonConvert.DeserializeObject<List<Item>>(jdata);
        mg.TabClick(mg.curType);
    }
    public void testSave(EquipItem _ItemData)
    {
        tdh = GameObject.Find("LoginDataHandler").GetComponent<TestDataHandler>();
        string[] itemData = new string[5];
        for (int i = 0; i < tdh.UserItemDatabaseList[0].Item_Data.Length; i++)
        {
            itemData[i] = tdh.UserItemDatabaseList[0].Item_Data[i];//������ �����͸� itemData[0~4]�迭�� ����
            char sp = '-';
            string[] ItemDataDetail = itemData[i].Split(sp);//������ �����۵����͸� -���� �����Ͽ� �ڸ� [0] = �ε���, [1] = ������Ÿ��, [2] = ���������ƴ���
            if(_ItemData.Index != ItemDataDetail[0])//����������Ų �ε����� �������� ������ �������� �ε����� �ٸ����
            {
                if (ItemDataDetail[1] == "helmet" && _ItemData.Type == "helmet") 
                {
                    tdh.UserItemDatabaseList[0].Item_Data[i] = _ItemData.Index + "-" + _ItemData.Type + "-" + IsLeftDatabaseBoolConverter(_ItemData.IsLeft);
                    TestUserItemData newData = new TestUserItemData
                    {
                        Guid = tdh.UserItemDatabaseList[0].Guid,
                        Item_Data = tdh.UserItemDatabaseList[0].Item_Data
                    };
                    tdh.context.SaveAsync(newData, (result) =>
                    {
                        //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
                        if (result.Exception == null)
                        {
                            Debug.Log(newData.Guid);
                            Debug.Log("Success!");
                        }
                        else
                            Debug.Log(result.Exception);
                    });
                }//��� �ϰ��
                else if (ItemDataDetail[1] == "body" && _ItemData.Type == "body")
                {
                    tdh.UserItemDatabaseList[0].Item_Data[i] = _ItemData.Index + "-" + _ItemData.Type + "-" + IsLeftDatabaseBoolConverter(_ItemData.IsLeft);
                    TestUserItemData newData = new TestUserItemData
                    {
                        Guid = tdh.UserItemDatabaseList[0].Guid,
                        Item_Data = tdh.UserItemDatabaseList[0].Item_Data
                    };
                    tdh.context.SaveAsync(newData, (result) =>
                    {
                        //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
                        if (result.Exception == null)
                        {
                            Debug.Log(newData.Guid);
                            Debug.Log("Success!");
                        }
                        else
                            Debug.Log(result.Exception);
                    });
                }//���� �ϰ��
                else if(ItemDataDetail[1] == "boots" && _ItemData.Type == "boots")
                {
                    tdh.UserItemDatabaseList[0].Item_Data[i] = _ItemData.Index + "-" + _ItemData.Type + "-" + IsLeftDatabaseBoolConverter(_ItemData.IsLeft);
                    TestUserItemData newData = new TestUserItemData
                    {
                        Guid = tdh.UserItemDatabaseList[0].Guid,
                        Item_Data = tdh.UserItemDatabaseList[0].Item_Data
                    };
                    tdh.context.SaveAsync(newData, (result) =>
                    {
                        //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
                        if (result.Exception == null)
                        {
                            Debug.Log(newData.Guid);
                            Debug.Log("Success!");
                        }
                        else
                            Debug.Log(result.Exception);
                    });
                }//�����ϰ��
                else if (ItemDataDetail[1] == "Weapon" && _ItemData.Type == "Weapon")
                {
                    if (ItemDataDetail[2] == "1" && _ItemData.IsLeft)
                    {
                        tdh.UserItemDatabaseList[0].Item_Data[i] = _ItemData.Index + "-" + _ItemData.Type + "-" + IsLeftDatabaseBoolConverter(_ItemData.IsLeft);
                        TestUserItemData newData = new TestUserItemData
                        {
                            Guid = tdh.UserItemDatabaseList[0].Guid,
                            Item_Data = tdh.UserItemDatabaseList[0].Item_Data
                        };
                        tdh.context.SaveAsync(newData, (result) =>
                        {
                            //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
                            if (result.Exception == null)
                            {
                                Debug.Log(newData.Guid);
                                Debug.Log("Success!");
                            }
                            else
                                Debug.Log(result.Exception);
                        });
                    }
                    else if (ItemDataDetail[2] == "0" && !_ItemData.IsLeft)
                    {
                        tdh.UserItemDatabaseList[0].Item_Data[i] = _ItemData.Index + "-" + _ItemData.Type + "-" + IsLeftDatabaseBoolConverter(_ItemData.IsLeft);
                        TestUserItemData newData = new TestUserItemData
                        {
                            Guid = tdh.UserItemDatabaseList[0].Guid,
                            Item_Data = tdh.UserItemDatabaseList[0].Item_Data
                        };
                        tdh.context.SaveAsync(newData, (result) =>
                        {
                            //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
                            if (result.Exception == null)
                            {
                                Debug.Log(newData.Guid);
                                Debug.Log("Success!");
                            }
                            else
                                Debug.Log(result.Exception);
                        });
                    }
                }
            }
        }
    }
    public int IsLeftDatabaseBoolConverter(bool IsLeft)
    {
        if (IsLeft)
        {
            return 1;
        }
        else
        {
            return 0;
        }
        
    }
    public void testLoad()
    {
        tdh = GameObject.Find("LoginDataHandler").GetComponent<TestDataHandler>();
        string[] itemData = new string[5];
        for (int i = 0; i < tdh.UserItemDatabaseList[0].Item_Data.Length; i++) 
        {
            itemData[i] =tdh.UserItemDatabaseList[0].Item_Data[i];//������ �����͸� itemData[0~4]�迭�� ����
            char sp = '-';
            string[] ItemDataDetail = itemData[i].Split(sp);//������ �����۵����͸� -���� �����Ͽ� �ڸ�
            CurEquipList.Add(EquipmentList.Find(x => x.Index == (MyItemList.Find(x => x.Index == ItemDataDetail[0]).Index)));
            if (CurEquipList[i].Type == "Weapon")
            {
                if (ItemDataDetail[2] == "1")
                {
                    CurEquipList[i].IsLeft = true;
                }
                else if (ItemDataDetail[2] == "0")
                {
                    CurEquipList[i].IsLeft = false;
                }
            }
        }
        {
            //for (int i = 0; i < tdh.UserItemDatabaseList[0].Item_Index.Length; i++) // for������ �������� ������ �ε����迭�� length��ŭ ����
            //{
            //    CurEquipList.Add(EquipmentList.Find(x => x.Index == (MyItemList.Find(x => x.Index == (tdh.UserItemDatabaseList[0].Item_Index[i])).Index)));
            //    if(CurEquipList[i].Type == "Weapon")
            //    {
            //        if (tdh.UserItemDatabaseList[0].Is_Left[i] == 1)
            //        {
            //            CurEquipList[i].IsLeft = true;
            //        }
            //        else if (tdh.UserItemDatabaseList[0].Is_Left[i] == 0)
            //        {
            //            CurEquipList[i].IsLeft = false;
            //        }
            //    }
            //}
        }

        mg.EquimentItemLoad();
    }
    public void ScoreAdd()
    {
        score++;
        status.maxEXPtext.text = ((int)status.EXP).ToString() + " / " + ((int)status.maxEXP).ToString();
        status.expBar.fillAmount = status.EXP / status.maxEXP;
        if (status.EXP >= status.maxEXP)
        {
            status.LevelUp();
        }
        scoreText.text = "SCORE : " + score.ToString();
        if (score == stageNumber * 200)// �������� 
        {
            scoreText.text = "SCORE : " + score.ToString();
            stageNumber = stageNumber + 1;
            StageClear();
        }
    }
    public void StageClear()
    {
        stageNum.text = "STAGE " + stageNumber.ToString();
        if (monsterSpawn.createTime > 0.1f)
        {
            monsterSpawn.createTime -= 0.075f;
        }
        else
        {
            monsterSpawn.createTime = 0;
        }
    } 
    public void NextStage()
    {
        stageClearPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
