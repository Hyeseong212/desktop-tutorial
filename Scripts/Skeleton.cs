using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skeleton : MonoBehaviour
{
    SpriteRenderer sr;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1f);
    bool isHurt;

    Animator animator;
    public Transform player;
    public float speed;

    public float atkCooltime = 4;
    public float atkDelay;

    public float calculeddamage;
    public float damage = 5;

    public int skeletonExp = 10;


    Status status;
    PlayerController playercontroller;

    [Header("몬스터체력")]
    public float currentHP;
    public float maxHP;

    public GameObject healthBarBack;
    public Image currenthealthBar;

    [Header("데미지텍스트관련")]
    public GameObject hudDamageText;
    public Transform hudPos;

    Vector2 hitPos;

    GameManager gm;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
        status = GameObject.Find("Status").GetComponent<Status>();
        playercontroller = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        maxHP = maxHP * gm.stageNumber * 0.5f;
        damage = damage * gm.stageNumber * 0.25f;
        currentHP = maxHP;
        currenthealthBar.fillAmount = 1f;
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        //if(player.position.x - transform.position.x < 0)
        //{
        //    Debug.Log("플레이어 왼쪽");
        //}
        //else if (player.position.x - transform.position.x > 0)
        //{
        //    Debug.Log("플레이어 오른쪽");
        //}
        if (atkDelay >= 0)
        {
            atkDelay -= Time.deltaTime;
        }
    }
    public void DirectionEnemy(float target, float baseobj)
    {
        if(target < baseobj)
        {
            animator.SetFloat("Direction", -1);
        }
        else
        {
            animator.SetFloat("Direction", 1);
        }
    }

    public Transform boxpos;
    public Vector2 boxSize;
    public AudioClip slashClip;
    public void Attack()
    {
        SoundManager.instance.SFXPlay("Slash", slashClip, 1);
        if(animator.GetFloat("Direction") == -1)
        {
            if(boxpos.localPosition.x > 0)
            {
                boxpos.localPosition = new Vector2(boxpos.localPosition.x * -1, boxpos.localPosition.y);
            }
        }
        else
        {
            if (boxpos.localPosition.x < 0)
            {
                boxpos.localPosition = new Vector2(Mathf.Abs(boxpos.localPosition.x), boxpos.localPosition.y);
            }
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxpos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")//플레이어가 데미지입는부분
            {
                calculeddamage = damage*((1 - status.defpoint));// (1 - 0.712방어력)*100 28.8퍼센트가 들어가야함
                Debug.Log(calculeddamage);
                status.hp -= calculeddamage;
                status.PlayerHP();//플레이어 hp UI에 반영
                status.PlayerHit();// 플레이어 데미지 맞는 소리
                playercontroller.TakeDamage(calculeddamage);//플레이어 데미지 들어가는 택스트
            }
        }
    }
    public AudioClip hitClip;
    public AudioClip deadClip;

    public void Hit(float _playerAtk)
    {
        SoundManager.instance.SFXPlay("Hit", hitClip, 1);
        currentHP -= _playerAtk;
        healthBarBack.SetActive(true);
        if(currentHP <= 0)
        {
            currenthealthBar.fillAmount = 0 / maxHP;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Invoke("DestroyObject", 1f);
            animator.SetTrigger("IsDead");
            SoundManager.instance.SFXPlay("Dead", deadClip, 1);
            status.EXP += skeletonExp * gm.stageNumber * 0.75f;
            gm.ScoreAdd();
        }
        else
        {
            isHurt = true;
            hitPos = transform.position;
            currenthealthBar.fillAmount = (float)currentHP / maxHP;
            StartCoroutine(IsHurt());

        }
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
    public void TakeDamage(float damage)
    {
        GameObject hudText = Instantiate(hudDamageText);
        TextMeshPro text = hudText.GetComponent<TextMeshPro>();
        text.color = Color.white;
        hudText.transform.SetParent(hudPos.transform);
        hudText.GetComponent<DamageText>().damage = (int)damage;
    }
    IEnumerator IsHurt()
    {
        while (isHurt)
        {
            yield return new WaitForSeconds(0.1f);
            sr.color = halfA;
            yield return new WaitForSeconds(0.1f);
            sr.color = fullA;
            isHurt = false;
        }
    }
}
