using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PLATEAU.Samples;

public class WaypointMover3 : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float rotateSpeed = 3f;

    private int index = 0;
    private bool stopAtSignal = false;

    private TimeManage timeManage;

    void Start()
    {
        timeManage = FindAnyObjectByType<TimeManage>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        float t = timeManage?.ElapsedTime ?? 0f;

        //---------------------------------------------------------
        // ★ 信号が赤の間（停止時間帯）は止まる
        //---------------------------------------------------------
        if (IsRedSignal(t) && index == 3)
        {
            stopAtSignal = true;
        }

        // ★ 赤信号終了と同時に動き出す
        if (!IsRedSignal(t) && stopAtSignal)
        {
            stopAtSignal = false;
        }

        if (stopAtSignal) return;

        //---------------------------------------------------------
        // 既に WayPoint 最後を超えていたら削除
        //---------------------------------------------------------
        if (index >= waypoints.Length)
        {
            Destroy(gameObject);   // ← 車を削除！
            return;
        }

        //---------------------------------------------------------
        // 通常移動
        //---------------------------------------------------------
        Transform target = waypoints[index];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime * 2
        );

        Vector3 dir = target.position - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        //---------------------------------------------------------
        // 到着判定
        //---------------------------------------------------------
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            index++;

            // ★ 最終点に到達した瞬間に削除する
            if (index >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    private bool IsRedSignal(float t)
    {
        return
            (t >= 0f && t < 19f) ||
            (t >= 30f && t < 49f) ||
            (t >= 60f && t < 79f);
    }
}

