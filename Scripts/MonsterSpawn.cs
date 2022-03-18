using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonsterSpawn : MonoBehaviour
{
    public Transform[] points;
    public GameObject monsterPrefab;

    public float createTime;
    public int maxMonster = 10;
    public bool isGameOver = false;
    GameManager gm;


    void Start()
    {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();


        if(points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }
    }
    IEnumerator CreateMonster()
    {
        while (!isGameOver)
        {
            //���� ������ ���� ���� ����
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            if (monsterCount < maxMonster)
            {
                //������ ���� �ֱ� �ð���ŭ ���
                yield return new WaitForSeconds(createTime);

                //�ұ�Ģ���� ��ġ ����
                int idx = Random.Range(1, points.Length);
                //������ ���� ����
                Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
}
