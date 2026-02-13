using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("次に進めるポイント（0ならゴール）")]
    public WaypointNode[] nextNodes;

    // ゴール判定
    public bool IsGoal => nextNodes == null || nextNodes.Length == 0;

    // ------------------------------
    // 信号設定
    // ------------------------------
    [Header("信号設定")]
    public bool hasSignal = false;

    [Header("進行時間帯①（青）")]
    public float goStart1 = 0f;
    public float goEnd1   = 15f;

    [Header("進行時間帯②（青）")]
    public float goStart2 = 20f;
    public float goEnd2   = 35f;

    /// <summary>
    /// 今の時間が「進める（青信号）」か
    /// </summary>
    public bool CanGo(float t)
    {
        if (!hasSignal) return true; // 信号なしは常に進める

        return
            (t >= goStart1 && t < goEnd1) ||
            (t >= goStart2 && t < goEnd2) ;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // ゴール：赤 / 信号：黄 / 通常：緑
        if (IsGoal)
            Gizmos.color = Color.red;
        else if (hasSignal)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position, 0.3f);

        if (nextNodes == null) return;

        Gizmos.color = Color.cyan;
        foreach (var n in nextNodes)
        {
            if (n != null)
                Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
#endif
}
