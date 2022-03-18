using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image progressBar;
    void Start()
    {
        StartCoroutine(LoadingScene());
    }
    IEnumerator LoadingScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("InGame");
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (timer <= 3f)
        {
            yield return null;
            timer += Time.deltaTime;
            progressBar.fillAmount = timer/3f;
            if(progressBar.fillAmount >= 1.0f && timer >= 3f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
