using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Tooltip("次に進めるポイント（0ならゴール）")]
    public WaypointNode[] nextNodes;

    public bool IsGoal => nextNodes == null || nextNodes.Length == 0;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = IsGoal ? Color.red : Color.green;
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
