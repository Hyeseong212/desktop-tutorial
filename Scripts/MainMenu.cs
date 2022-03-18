using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Text valueText;
    [SerializeField] Scrollbar SoundBar;
    [SerializeField] GameObject GameStartBtn;
    [SerializeField] GameObject OptionBackBtn;
    [SerializeField] GameObject OptionBtn;
    [SerializeField] GameObject m_SettinUi;

    SoundAdjust soundadjust;
    public void Start()
    {
        SoundBar = GameObject.Find("SoundBar").GetComponent<Scrollbar>();
        soundadjust = GameObject.Find("ParameterSaver").GetComponent<SoundAdjust>();
    }
    public void ValueUpdate()
    {
        valueText.text = Mathf.RoundToInt(SoundBar.value * 100) + "%";
        soundadjust.soundControllInt = SoundBar.value * 100;
    }
    public void GameStartButton()
    {
        iTween.PunchScale(GameStartBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            SceneManager.LoadScene(2);
        }));
    }
    public void OptionButton()
    {
        iTween.PunchScale(OptionBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            iTween.MoveTo(m_SettinUi, iTween.Hash("x", 0, "time", 1, "islocal", true));
        }));
    }
    public void OptionBackButton()
    {
        iTween.PunchScale(OptionBackBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            iTween.MoveTo(m_SettinUi, iTween.Hash("x", 2000, "time", 1, "islocal", true));
        }));
    }
    public void ExitButton()
    {
        iTween.PunchScale(OptionBackBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            Application.Quit();
        }));
    }
    IEnumerator WaitAndCall(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
