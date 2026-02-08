using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCRandomTurnOnHit : MonoBehaviour
{
    [Header("回転角度の範囲（度）")]
    public float minTurnAngle = 90f;
    public float maxTurnAngle = 180f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 地面は無視
        if (hit.normal.y > 0.5f) return;

        // ランダムな角度を決定
        float randomAngle = Random.Range(minTurnAngle, maxTurnAngle);

        // 左右どちらか
        if (Random.value < 0.5f)
        {
            randomAngle *= -1f;
        }

        // Y軸だけ回転
        transform.Rotate(0f, randomAngle, 0f);
    }
}

