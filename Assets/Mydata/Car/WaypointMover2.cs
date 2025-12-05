using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PLATEAU.Samples;

public class WaypointMover2 : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float rotateSpeed = 3f;

    private int index = 0;
    private bool stopAtSignal = false;

    private TimeManage timeManage;

    void Start()
    {
        timeManage = FindObjectOfType<TimeManage>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        float t = timeManage?.ElapsedTime ?? 0f;

        //---------------------------------------------------------
        // ★ 信号状態で止める（信号地点：index==2）
        //---------------------------------------------------------
        if (index == 2 && IsRedSignal(t))
        {
            stopAtSignal = true;
        }

        // ★ 赤信号が終わった瞬間に再開
        if (!IsRedSignal(t) && stopAtSignal)
        {
            stopAtSignal = false;
        }

        if (stopAtSignal) return;

        //---------------------------------------------------------
        // ★ すでに最後を超えていたら削除
        //---------------------------------------------------------
        if (index >= waypoints.Length)
        {
            Destroy(gameObject);
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
        // 到着したら次のポイントへ
        //---------------------------------------------------------
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            index++;

            // ★ 最終 WayPoint に到達したら削除
            if (index >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    // ★ 信号の「赤」範囲
    private bool IsRedSignal(float t)
    {
        return
            !(
                (t >= 0f && t < 15f) ||
                (t >= 31f && t < 45f) ||
                (t >= 61f && t < 75f)
            );
    }
}
