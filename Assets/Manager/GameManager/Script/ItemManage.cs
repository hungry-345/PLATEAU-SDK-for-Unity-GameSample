using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManage : MonoBehaviour
{
    //生成するアイテム
    [SerializeField, Tooltip("高さアイテム")] private GameObject measuredheightItem;
    [SerializeField, Tooltip("用途アイテム")] private GameObject UsageItem;

    //生成範囲
    private GameObject spawnTransformObjects;
    private System.Random rnd;
    private Vector3 center; //スポーン範囲の中心

    public void InitializeItem()
    {
        //ステージの範囲を取得
        spawnTransformObjects = GameObject.Find("StageRange");
        //オブジェクトの中心を設定
        // center = spawnTransformObjects.GetComponent<Renderer>().bounds.center;

        rnd = new System.Random();
    }

    //初期化
    public void GenerateItem()
    {
        //円の半径
        float radius = 100f;
        //指定された半径の円内のランダム位置を取得
        var circlePos = radius * Random.insideUnitCircle;
        //円内のランダム位置を計算
        var spawnPos = new Vector3(circlePos.x, 100f, circlePos.y) + center;

        GameObject hintItem = Instantiate(UsageItem, this.gameObject.transform) as GameObject;
        hintItem.name = "Usage";
        hintItem.transform.position = spawnPos;

        circlePos = radius * Random.insideUnitCircle;
        //スポーン位置にセット
        // spawnPos = new Vector3(circlePos.x, 100f, circlePos.y) + center;
        spawnPos = new Vector3(Random.Range(-140f,470f), 100f, Random.Range(1300f,1700f));
        hintItem = Instantiate(measuredheightItem, this.gameObject.transform) as GameObject;
        hintItem.name = "measuredheight";
        hintItem.transform.position = spawnPos;
    }
    //アイテムの削除
    public void DestroyItem()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }
}
