using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Stats
{
    public Stats
    (string _Damage, string _Speed, string _Stamina, string _HP, string _Critical, string _Level, string _Defpt,string _ID)
    { Damage = _Damage; Speed = _Speed; Stamina = _Stamina; HP = _HP; Critical = _Critical; Level = _Level; DefPt = _Defpt; ID = _ID; }
    public string Damage, Speed, Stamina, HP, Critical, Level, DefPt, ID;
}
public class Status : MonoBehaviour
{
    public List<Stats> MyStatList;

    public Image HPbar;
    public Image Staminabar;
    public Image expBar;

    public Text HPfont;
    public Text Staminafont;
    public Text maxEXPtext;
    public Text levelText;

    [Header("����")]
    public float hp;
    public float Maxhp;
    public float stamina;
    public float Maxstamina;
    public float damagePlus;
    public float defpoint;
    public float speed;
    public float defaultSpeed = 5;
    public float criticalChance;
    public float EXP;
    public float maxEXP = 100;
    public int level = 1;

    public int leftPointint;

    public float staminaConsumption;

    public float atklatept; // ���Ӱ��ҷ�

    [SerializeField] AudioClip clip;
    [SerializeField] GameObject gameOver;
    GameManager gm;
    public TextAsset StatDatabase;
    StatusMenu statusMenu;
    WeaponManager wm;
    PlayerController pc;
    private void Awake()
    {
        statusMenu = GameObject.Find("Stat").GetComponent<StatusMenu>();
    }
    void Start()
    {
        level = 1;
        maxEXP = 100;
        stamina = Maxstamina;
        criticalChance = 0;//����� ���ȿ��� ������
        hp = Maxhp;
        HPfont = GameObject.Find("HPfont").GetComponent<Text>();
        Staminafont = GameObject.Find("Staminafont").GetComponent<Text>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        wm = GameObject.Find("WeaponManager").GetComponent<WeaponManager>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        //���Ⱥκ�
        string[] Stat = StatDatabase.text.Substring(0, StatDatabase.text.Length - 1).Split('\n');
        for (int i = 0; i < Stat.Length; i++)
        {
            string[] row = Stat[i].Split('\t');

            //MyStatList.Add(new Stats(row[0], row[1], row[2], row[3], row[4], row[5], row[6]));
        }
        if (MyStatList == null)
        {
            MyStatList = new List<Stats>();
        }
        StatInit();
    }
    public void StatInit()
    {
        //����
        atklatept = 1 - ((float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").AttackSpeed) + float.Parse(gm.CurEquipList.Find(x => x.Type == "body").AttackSpeed)) / 100);//���Ӱ��ҷ� ������ ���ҷ� ����
        wm.atkCooltime = atklatept * float.Parse(gm.CurEquipList.Find(x => x.IsLeft == true).AtkLate); // ���ʰ��� ���ӽ����Ҷ� ����
        wm.rightatkCooltime = atklatept * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).AtkLate); //�����ʰ��� ���ӽ����Ҷ� ����
        statusMenu.atkLate.text ="-"+ (((float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").AttackSpeed) + float.Parse(gm.CurEquipList.Find(x => x.Type == "body").AttackSpeed)) / 100)*100).ToString()+ " %";
        //ġ��ŸȮ��
        criticalChance = (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Cirticalpt) + float.Parse(gm.CurEquipList.Find(x => x.Type == "boots").Cirticalpt));
        statusMenu.curCriticalChance.text = criticalChance.ToString() + "%";
        //����
        defpoint = 1 - (1 - (float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").Defpt) / 100)) * (1 - (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Defpt) / 100)) * (1 - (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Defpt) / 100));
        //�̵��ӵ�
        speed = defaultSpeed + float.Parse(gm.CurEquipList.Find(x => x.Type == "boots").Speed);
        pc.fixedspeed = speed;
        pc.speed = pc.fixedspeed;

    }
    public void EquipmentStatChange(EquipItem _item)
    {
        atklatept = 1 - ((float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").AttackSpeed) + float.Parse(gm.CurEquipList.Find(x => x.Type == "body").AttackSpeed)) / 100);
        if (_item.Type == "helmet"|| _item.Type == "body" || _item.Type == "boots")// ���,����,�������� Ȯ��
        {
            //���ӷ�
            wm.atkCooltime = atklatept * float.Parse(gm.CurEquipList.Find(x => x.IsLeft == true).AtkLate); //���� ���� ���� ���� ����
            wm.rightatkCooltime = atklatept * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).AtkLate); //������ ���� ���� ���� ����
            //ġ��ŸȮ��
            criticalChance = statusMenu.criticalpt + (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Cirticalpt) 
                + float.Parse(gm.CurEquipList.Find(x => x.Type == "boots").Cirticalpt));
            //���� (������ 1 - (1-����)*(1-����)*(1-����))
            defpoint = 1 - (1-(float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").Defpt)/100)) *
                (1 - (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Defpt) / 100)) * (1 - (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Defpt) / 100));
            //�̵��ӵ�
            speed = defaultSpeed + float.Parse(gm.CurEquipList.Find(x => x.Type == "boots").Speed);
            pc.fixedspeed = speed;
        }
        else if (_item.Type == "Weapon") //�����Ͻ� �԰��ִ���� ���� ����
        {
            //���ӷ�
            if (_item.IsLeft == true)
            {
                wm.atkCooltime = atklatept * float.Parse(gm.CurEquipList.Find(x => x.IsLeft == true).AtkLate);
            }
            else if (_item.IsLeft == false)
            {
                wm.rightatkCooltime = atklatept * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).AtkLate);
            }
            //ġ��ŸȮ��
        };
        statusMenu.atkLate.text = "-" + (((float.Parse(gm.CurEquipList.Find(x => x.Type == "helmet").AttackSpeed) + float.Parse(gm.CurEquipList.Find(x => x.Type == "body").AttackSpeed)) / 100) * 100).ToString() + " %";
        statusMenu.curCriticalChance.text = criticalChance.ToString() + "%";
    }
    public void PlayerStamina()
    {
        stamina -= staminaConsumption / 100;
        Staminabar.fillAmount = stamina / Maxstamina;
        Staminafont.text = (int)stamina + " / " + (int)Maxstamina;
    }
    public void PlayerStaminaRestore()
    {
        if (stamina <= Maxstamina)
        {
            stamina += 0.1f;
            Staminabar.fillAmount = stamina / Maxstamina;
            Staminafont.text = (int)stamina + " / " + (int)Maxstamina;
        }
    }
    public void PlayerHP()
    {
        if (hp >= 1)
        {
            HPfont.text = (int)hp + " / " + (int)Maxhp;
            HPbar.fillAmount = hp / Maxhp;
        }
        else
        {
            gameOver.SetActive(true);
        }
    }
    public void PlayerHit()
    {
        SoundManager.instance.SFXPlay("Hit", clip, 1);
    }
    public int CriticalHit()
    {
        int randomInt = UnityEngine.Random.Range(0, 100);
        if(randomInt <= criticalChance)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
    public void LevelUp()
    {
        level++;
        levelText.text ="Lv. "+ level.ToString();
        gm.isEquipmentPanelOn = !gm.isEquipmentPanelOn;
        gm.equipmentPanel.SetActive(gm.isEquipmentPanelOn);
        MenuManager menuManager = GameObject.Find("Equipment,Stat").GetComponent<MenuManager>();
        menuManager.StatMenuOn();
        if (gm.isEquipmentPanelOn)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        leftPointint += 5;
        StatusMenu statusMenu = GameObject.Find("Stat").GetComponent<StatusMenu>();
        statusMenu.leftPoint.text = leftPointint.ToString();
        EXP = 0;
        maxEXP = (int)(maxEXP * 1.25f);
        expBar.fillAmount = EXP / maxEXP;
        maxEXPtext.text = EXP.ToString() + " / "+ maxEXP.ToString();
    }
    public void StatSave()
    {
        string jdata = JsonConvert.SerializeObject(MyStatList);
        File.WriteAllText(Application.dataPath + "/Data/PotpolioMyStatDatabase.txt", jdata);

        Debug.Log(jdata);
    }
    //public void Save()
    //{
    //    string jdata = JsonConvert.SerializeObject(MyItemList);
    //    File.WriteAllText(Application.dataPath + "/Data/PotpolioMyDatabase.txt", jdata);

    //    mg.TabClick(mg.curType);
    //}
    //public void Load()
    //{
    //    mg = GameObject.Find("Equipment,Stat").GetComponent<MenuManager>();
    //    string jdata = File.ReadAllText(Application.dataPath + "/Data/PotpolioMyDatabase.txt");
    //    MyItemList = JsonConvert.DeserializeObject<List<Item>>(jdata);

    //    mg.TabClick(mg.curType);
    //}
}
