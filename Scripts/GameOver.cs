using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Start()
    {
        Time.timeScale = 0;
    }
    public void TotheMainMenu()
    {
        Debug.Log("���θ޴���");
    }
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
    }
}
