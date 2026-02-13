using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public static DistanceChecker Instance;
    public static float LastDistanceAtTimeUp = -1f; // タイマーゼロ時の距離を保持

    [Header("プレイヤー")]
    public Transform player;

    [Header("ターゲット(NPCなど)")]
    public Transform target;

    public float CurrentDistance { get; private set; } = -1f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject obj = GameObject.Find("PlayerArmature");
            if (obj != null)
            {
                player = obj.transform;
                Debug.Log("PlayerArmature を取得しました");
            }
            else return;
        }

        if (target == null) return;

        CurrentDistance = Vector3.Distance(player.position, target.position);
    }

    // タイマーゼロ時に呼んで距離を保存
    public void SaveDistanceAtTimeUp()
    {
        LastDistanceAtTimeUp = CurrentDistance;
        Debug.Log("タイマーゼロ時の距離を保存: " + LastDistanceAtTimeUp);
    }
}
