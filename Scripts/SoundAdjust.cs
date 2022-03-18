using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAdjust : MonoBehaviour
{
    public float soundControllInt;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
