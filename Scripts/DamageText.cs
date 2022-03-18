using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float alphaSpeed; // 투명도 변환속도
    public float destroyTime;

    public float damage;

    TextMeshPro text;

    Skeleton skeleton;

    Color alpha;
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text ="-" + ((int)damage).ToString();
        alpha = text.color;
        Invoke("DestroyObject",destroyTime);
    }

    void Update()
    {
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
