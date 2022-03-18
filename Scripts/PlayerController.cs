using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Animator animationControll;
    Status status;

    [Header("이동관련")]
    Rigidbody2D rb;
    Vector3 movement;
    float h, v;
    public float fixedspeed;
    public float speed;
    float staminaCoolTime = 0;
    bool isRun;
    
    [Header("데미지텍스트관련")]
    public GameObject hudDamageText;
    public Transform hudPos;

    GameManager gm;
    WeaponManager wm;
    void Start()
    {
        animationControll = GetComponent<Animator>();
        status = GameObject.Find("Status").GetComponent<Status>();
        rb = GetComponent<Rigidbody2D>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        wm = GameObject.Find("WeaponManager").GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (!gm.isEquipmentPanelOn)
            {
                h = Input.GetAxisRaw("Horizontal");
                v = Input.GetAxisRaw("Vertical");
                if (Input.GetKey(KeyCode.W))
                {
                    animationControll.SetInteger("MovePara", 1);
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    animationControll.SetInteger("MovePara", 5);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    animationControll.SetInteger("MovePara", 2);
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    animationControll.SetInteger("MovePara", 5);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    animationControll.SetInteger("MovePara", 3);
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    animationControll.SetInteger("MovePara", 5);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    animationControll.SetInteger("MovePara", 4);
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    animationControll.SetInteger("MovePara", 5);
                }

                // 이동부분
                if (Input.GetMouseButton(0))
                {
                    if (!wm.isCooltime)
                    {
                        wm.isCooltime = true;
                        if (gm.CurEquipList != null)
                        {
                            if (gm.CurEquipList.Contains(gm.CurEquipList.Find(x => x.Type == "Weapon")))
                            {
                                wm.Attack(gm.CurEquipList.Find(x => x.IsLeft == true));
                            }
                        }
                        else return;
                    }
                }
                if (Input.GetMouseButton(1))
                {
                    if (!wm.rightisCooltime)
                    {
                        wm.rightisCooltime = true;
                        if (gm.CurEquipList != null)
                      {
                            if (gm.CurEquipList.Contains(gm.CurEquipList.Find(x => x.Type == "Weapon")))
                            {
                                wm.RightAttack(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.IsLeft == false));
                            }
                        }
                        else return;
                    }
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    staminaCoolTime = 0;
                    speed = fixedspeed;
                    animationControll.speed = 1.5f;
                    isRun = false;
                }
                if (!isRun)
                {
                    staminaCoolTime += Time.deltaTime;
                    if (staminaCoolTime >= 1)
                    {
                        status.PlayerStaminaRestore();
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        movement.Set(h, v, 0f);
        movement = movement.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
        if (Input.GetKey(KeyCode.LeftShift) && status.stamina >= 0)
        {
            speed = fixedspeed * 1.5f;
            animationControll.speed = 2.5f;
            status.PlayerStamina();
            isRun = true;
            if (status.stamina <= 0)
            {
                staminaCoolTime = 0;
                speed = fixedspeed;
                animationControll.speed = 1.5f;
                isRun = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            staminaCoolTime = 0;
            speed = fixedspeed;
            animationControll.speed = 1.5f;
            isRun = false;
        }
        if (!isRun)
        {
            staminaCoolTime += Time.deltaTime;
            if (staminaCoolTime >= 1)
            {
                status.PlayerStaminaRestore();
            }
        }
    }
    public void TakeDamage(float damage)//데미지 텍스트 출력부분
    {
        GameObject hudText = Instantiate(hudDamageText);
        hudText.transform.SetParent(hudPos.transform);
        hudText.transform.position = new Vector3 (hudPos.position.x, hudPos.position.y + 2, -90);
        hudText.GetComponent<DamageText>().damage = damage;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("무언가에 부딪힘");
    }
}
