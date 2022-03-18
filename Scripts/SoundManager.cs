using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int energyBoltSoundObjectNumber = 0;
    
    public void SFXPlay(string sfxName, AudioClip clip, float objectLifeTime)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.clip = clip;
        if (sfxName == "Thunder" && sfxName == "Hit")
        {
            audiosource.volume = 0.5f;
        }
        else
        {
            audiosource.volume = 0.1f;
        }
        audiosource.Play();

        Destroy(go, objectLifeTime);
    }
}
