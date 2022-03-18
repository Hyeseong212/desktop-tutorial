using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusMenu : MonoBehaviour
{
    [Header("스탯텍스트")]
    [SerializeField] Text damage;
    [SerializeField] Text criticalChance;
    [SerializeField] Text stamina;
    [SerializeField] Text hp;
    [Header("현재스탯텍스트")]
    public Text curLeftDamage;
    public Text curRightDamage;
    public Text curCriticalChance;
    [SerializeField] Text curStamina;
    [SerializeField] Text curHp;
    public Text atkLate;
    public Text leftPoint;

    Status status;
    GameManager gm;

    int textStamina;
    int textHp;

    public float criticalpt;

    void Start()
    {
        status = GameObject.Find("Status").GetComponent<Status>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        //curLeftDamage.text = "+ " + ((int)(float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true).Attackpt)
        //                             + status.damagePlus * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true).AtkPlusValue))).ToString();
        //curRightDamage.text = "+ " + ((int)(float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).Attackpt)
        //                          + status.damagePlus * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).AtkPlusValue))).ToString();
    }

    public void PlusDamage()
    {
        if (status.leftPointint > 0)
        {
            status.leftPointint--;
            leftPoint.text = status.leftPointint.ToString();
            status.damagePlus += 1;
            damage.text = status.damagePlus.ToString();
            curLeftDamage.text = "+ " + ((int)(float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true).Attackpt) 
                                      + status.damagePlus * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == true).AtkPlusValue))).ToString();
            curRightDamage.text = "+ "+ ((int)(float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).Attackpt)
                                      + status.damagePlus * float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false).AtkPlusValue))).ToString();

            status.MyStatList.Find(x => x.ID == "0");
        }
    }
    public void PlusCriticalChance()
    {
        if (status.leftPointint > 0)
        {
            if (criticalpt < 60)
            {
                status.leftPointint--;
                leftPoint.text = status.leftPointint.ToString();
                criticalpt += 2;
                status.criticalChance = (float.Parse(gm.CurEquipList.Find(x => x.Type == "body").Cirticalpt) + float.Parse(gm.CurEquipList.Find(x => x.Type == "boots").Cirticalpt)) + criticalpt;
                criticalChance.text = (criticalpt / 2).ToString();
                curCriticalChance.text = "+ "+status.criticalChance.ToString() + "%";
            }
        }
    }
    int plusStamina;
    int plusHp;
    public void PlusStamina()
    {
        if (status.leftPointint > 0)
        {
            status.leftPointint--;
            leftPoint.text = status.leftPointint.ToString();
            status.Maxstamina += 5;
            status.stamina += 5;
            plusStamina += 5;
            status.Staminabar.fillAmount = status.stamina / status.Maxstamina;
            textStamina++;
            stamina.text = textStamina.ToString();
            curStamina.text ="+ " + plusStamina.ToString();
            status.Staminafont.text = (int)status.stamina + " / " + (int)status.Maxstamina;
        }
    }
    public void PlusHP()
    {
        if (status.leftPointint > 0)
        {
            status.leftPointint--;
            leftPoint.text = status.leftPointint.ToString();
            status.Maxhp += 5;
            status.hp += 5;
            plusHp += 5;
            status.HPbar.fillAmount = status.hp / status.Maxhp;
            textHp++;
            hp.text = textHp.ToString();
            curHp.text = "+ " + plusHp.ToString();
            status.HPfont.text = (int)status.hp + " / " + (int)status.Maxhp;
        }
    }

}
