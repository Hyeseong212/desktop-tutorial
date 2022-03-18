using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonsBreath : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public Vector2 boxSize;

    public GameObject playerpos;

    public int attackDamage = 5;
    public float dotCooltime = 0.2f;
    public float objectLifeTime = 1.2f;

    int attackonce = 0;

    Skeleton skeleton;
    Status status;
    PlayerController Player;
    WeaponManager wm;
    private void Start()
    {
        status = GameObject.Find("Status").GetComponent<Status>();
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        wm = GameObject.Find("WeaponManager").GetComponent<WeaponManager>();
        SoundManager.instance.SFXPlay("Dragon'sBreath", clip, objectLifeTime);
        Invoke("DestroyObject", 1.2f);
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(playerpos.transform.position, boxSize, wm.z2);
            if (attackonce == 0)
            {
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "MONSTER")
                    {
                        float damage = (attackDamage + (status.damagePlus) * 0.5f) * status.CriticalHit();
                        skeleton = collider.GetComponent<Skeleton>();
                        skeleton.Hit(damage);
                        skeleton.TakeDamage(damage);
                        StartCoroutine(DotDamage());
                        attackonce++;
                    }
                }
            }
            transform.position = Player.transform.position;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(playerpos.transform.position.x, playerpos.transform.position.y,0), boxSize);
    }
    void SoundPlay()
    {
        SoundManager.instance.SFXPlay("Fire", clip, objectLifeTime);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);

    }
    IEnumerator DotDamage()
    {
        yield return new WaitForSeconds(dotCooltime);
        attackonce = 0;
    }
}
