using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip movingClip; // 動いている時の音
    public AudioClip idleClip;   // 停まっている時の音

    [Header("Settings")]
    public float moveThreshold = 0.01f; // この距離以上動いたら動いていると判断
    [Range(0f,1f)]
    public float movingVolume = 0.5f; // 動いている音の音量
    [Range(0f,1f)]
    public float idleVolume = 0.3f;   // 停止音の音量

    private AudioSource movingAudio;
    private AudioSource idleAudio;

    private Vector3 lastPosition;

    void Awake()
    {
        // AudioSourceをスクリプトで追加
        movingAudio = gameObject.AddComponent<AudioSource>();
        movingAudio.clip = movingClip;
        movingAudio.loop = true;
        movingAudio.volume = movingVolume; // 初期音量設定

        idleAudio = gameObject.AddComponent<AudioSource>();
        idleAudio.clip = idleClip;
        idleAudio.loop = true;
        idleAudio.volume = idleVolume; // 初期音量設定

        // 初期位置記録
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);

        if(distanceMoved > moveThreshold)
        {
            // 動いている
            if(!movingAudio.isPlaying) movingAudio.Play();
            if(idleAudio.isPlaying) idleAudio.Stop();
        }
        else
        {
            // 停止中
            if(movingAudio.isPlaying) movingAudio.Stop();
            if(!idleAudio.isPlaying) idleAudio.Play();
        }

        lastPosition = currentPosition;
    }
}
