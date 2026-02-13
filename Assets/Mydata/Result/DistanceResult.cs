using UnityEngine;

public class DistanceResult : MonoBehaviour
{
    void Start()
    {
        float distance = DistanceChecker.LastDistanceAtTimeUp;
        Debug.Log("前のシーンのタイマーゼロ時の距離: " + distance);

        // ここで距離をUI表示したり、ゲームロジックに使用可能
    }
}
