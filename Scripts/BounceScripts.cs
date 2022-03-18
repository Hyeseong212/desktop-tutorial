using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceScripts : MonoBehaviour
{
    public PhysicsMaterial2D pm2d;
    Collider2D col;
    float time;
    private void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
    }
    void Update()
    {
        time += Time.deltaTime;
        if(time > 1)
        {
            pm2d.bounciness = Random.Range(0.5f, 0.6f);

            time = 0;
        }
    }
}
