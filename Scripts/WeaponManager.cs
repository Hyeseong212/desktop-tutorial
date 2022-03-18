using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("공격이펙트관련")]
    Vector3 mousePos, transPos, targetPos, playerPos;
    public GameObject[] Weapon;
    public float atkCooltime = 1;
    public float rightatkCooltime = 1;
    public bool isCooltime;
    public bool rightisCooltime;
    public float z;
    public float z2;

    GameManager gm;
    PlayerController player;
    Status status;

    GameObject fireAttack;
    GameObject energyBolt;

    public float speed;
    Quaternion a;
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        status = GameObject.Find("Status").GetComponent<Status>();
        speed = 1;
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (fireAttack != null)
            {
                fireAttack.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            }
            Vector2 len2 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            z2 = Mathf.Atan2(len2.y, len2.x) * Mathf.Rad2Deg;
            z = Mathf.Atan2(-len2.y, -len2.x) * Mathf.Rad2Deg;
            a = Quaternion.Euler(0, 0, z);
            transform.rotation = Quaternion.Euler(0, 0, z2);
        }
    }
    public void Attack(EquipItem item)
    {
        if (item != null)
        {
            if (item.Name == "신의심판")
            {
                JudgeOfGod();
            }
            else if (item.Name == "용의숨결")
            {
                DragonBreath();
            }
            else if (item.Name == "에너지볼트")
            {
                EnergyBolt();
            }
        }
        else return;
    }
    public void RightAttack(EquipItem item)
    {
        if (item != null)
        {
            if (item.Name == "신의심판")
            {
                RightJudgeOfGod();
            }
            else if (item.Name == "용의숨결")
            {
                RightDragonBreath();
            }
            else if (item.Name == "에너지볼트")
            {
                RightEnergyBolt();
            }
        }
        else return;
    }
    public void JudgeOfGod()
    {
        GameObject atkeft = Instantiate(Weapon[0]);
        mousePos = Input.mousePosition;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y + 5f, 0);
        atkeft.transform.position = targetPos;
        StartCoroutine(AttackCoolTime());
    }
    public void DragonBreath()
    {
        fireAttack = Instantiate(Weapon[1], transform.position, a);
        StartCoroutine(AttackCoolTime());
    }
    public void EnergyBolt()
    {
        energyBolt = Instantiate(Weapon[2], transform.position, transform.rotation);
        StartCoroutine(AttackCoolTime());
    }
    public void RightJudgeOfGod()
    {
        GameObject atkeft = Instantiate(Weapon[0]);
        mousePos = Input.mousePosition;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y + 5f, 0);
        atkeft.transform.position = targetPos;
        StartCoroutine(RightAttackCoolTime());
    }
    public void RightDragonBreath()
    {
        fireAttack = Instantiate(Weapon[1], transform.position, a);
        StartCoroutine(RightAttackCoolTime());
    }


    Vector2 len;
    public void RightEnergyBolt()
    {
        energyBolt = Instantiate(Weapon[2], transform.position, transform.rotation);
        StartCoroutine(RightAttackCoolTime());
    }
    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(atkCooltime);
        isCooltime = false;
    }
    IEnumerator RightAttackCoolTime()
    {
        yield return new WaitForSeconds(rightatkCooltime);
        rightisCooltime = false;
    }
}
