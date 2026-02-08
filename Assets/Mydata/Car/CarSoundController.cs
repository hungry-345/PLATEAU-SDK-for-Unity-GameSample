using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CarSoundController : MonoBehaviour
{
    [Header("音声クリップ")]
    public AudioClip movingClip;    // 走行中
    public AudioClip stopClip;      // 停止
    public AudioClip collisionClip; // 接触

    [Header("判定設定")]
    public float moveSpeedThreshold = 0.2f; // これ以上で走行中

    private Rigidbody rb;
    private AudioSource audioSource;

    private bool isMoving = false;
    private bool wasMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;

        isMoving = speed > moveSpeedThreshold;

        // 走行開始
        if (isMoving && !wasMoving)
        {
            PlaySound(movingClip);
        }

        // 停止
        if (!isMoving && wasMoving)
        {
            PlaySound(stopClip);
        }

        wasMoving = isMoving;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 接触音は最優先
        PlaySound(collisionClip);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
