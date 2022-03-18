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
            //현재 생성된 몬스터 개수 산출
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            if (monsterCount < maxMonster)
            {
                //몬스터의 생성 주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);
                //몬스터의 동적 생성
                Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
}
