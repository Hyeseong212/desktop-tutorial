using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeOfGod : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public Transform pos;
    public Vector2 boxSize;


    int attackonce = 0;

    public float objectLifeTime = 1f;

    Skeleton skeleton;
    Status status;
    GameManager gm;
    private void Start()
    {
        status = GameObject.Find("Status").GetComponent<Status>();
        gm = GameObject.Find("UI").GetComponent<GameManager>();
        SoundManager.instance.SFXPlay("Thunder", clip, objectLifeTime);
        Invoke("DestroyObject",1f);
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
                    if (gm.CurEquipList != null)
                    {
                        int damage = (int)((float.Parse(gm.CurEquipList.FindAll(x => x.Type == "Weapon").Find(x => x.Index == "0").Attackpt) + (float)(status.damagePlus * 2)) * (float)status.CriticalHit());
                        skeleton = collider.GetComponent<Skeleton>();
                        skeleton.Hit(damage);
                        skeleton.TakeDamage(damage);
                    }
                    else return;
                }
            }
            attackonce++;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    void SoundPlay()
    {
        SoundManager.instance.SFXPlay("Thunder", clip, objectLifeTime);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

}
