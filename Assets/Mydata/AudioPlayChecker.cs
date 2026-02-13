using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayChecker : MonoBehaviour
{
    public float hearDistance = 10f;

    AudioSource src;
    AudioListener listener;
    bool hasRestarted = false;

    void Start()
    {
        src = GetComponent<AudioSource>();
        listener = FindObjectOfType<AudioListener>();

        void Start()
{
    var src = GetComponent<AudioSource>();
    src.spatialBlend = 0f; // ★ 完全2D
    src.volume = 1f;
    src.Play();

    Debug.Log("🔥 強制2D再生テスト");
}

    }

    void Update()
    {
        if (listener == null) return;
        if (hasRestarted) return;

        float dist = Vector3.Distance(transform.position, listener.transform.position);

        if (dist <= hearDistance)
        {
            src.Stop();
            src.Play();
            hasRestarted = true;

            Debug.Log("🎧 近距離に入ったので音を再生し直しました");
        }

        Debug.Log("Listener volume:" + AudioListener.volume);

        Debug.Log(
    $"mute:{src.mute} " +
    $"volume:{src.volume} " +
    $"spatial:{src.spatialBlend}"
);


    }
}
