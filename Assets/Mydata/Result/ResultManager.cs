using System;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("ElapsedTime"))
        {
            float elapsed = PlayerPrefs.GetFloat("ElapsedTime");
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsed);
            Debug.Log("リザルト画面 経過時間: " + timeSpan.ToString(@"mm\:ss"));
        }
        else
        {
            Debug.Log("残り時間データが保存されていません。");
        }
    }
}


