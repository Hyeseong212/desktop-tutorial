using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBolt : MonoBehaviour
{

    public Vector2 boxSize;

    public Transform pos;

    int attackonce = 0;

    public float objectLifeTime = 5f;
    public float dotCooltime = 0.5f;
    public float speed = 5f;

    Skeleton skeleton;
    Status status;
    GameManager gm;
    SoundManager sm;

    AudioSource audio;

    private void Start()
    {
        status = GameObject.Find("Status").GetComponent<Status>();
        sm = GameObject.Find("AudioManager").GetComponent<SoundManager>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        audio = GetComponent<AudioSource>();

        Invoke("DestroyObject", objectLifeTime);
    }
    private void Update()
    {
        if (attackonce == 0)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.tag == "MONSTER")
                {
                    int damage = (int)((float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.Index == "7").Attackpt) + (float)(status.damagePlus*1)) * (float)status.CriticalHit());
                    skeleton = collider.GetComponent<Skeleton>();
                    skeleton.Hit(damage);
                    skeleton.TakeDamage(damage);
                    StartCoroutine(DotDamage());
                    attackonce++;
                }
            }
        }
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        if (gm.isEquipmentPanelOn)
        {
            audio.volume = 0;
        }
        else
        {
            audio.volume = 0.1f;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
        sm.energyBoltSoundObjectNumber--;
    }
    IEnumerator DotDamage()
    {
        yield return new WaitForSeconds(dotCooltime);
        attackonce = 0;
    }
}
