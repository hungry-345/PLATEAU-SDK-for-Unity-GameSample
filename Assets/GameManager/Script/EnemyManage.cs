using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敵を管理するクラス
public class EnemyManage : MonoBehaviour
{
    //生成する敵
    [SerializeField, Tooltip("ゾンビ")] private GameObject Zombie;
    //生成範囲
    private GameObject spawnTransformObjects;  //敵のスポーン位置の候補が入ったオブジェクト
    //生成数
    private int enemyNum = 50; //敵の数
    private Vector3 center; //スポーン範囲の中心

    //初期化処理
    public void InitializeEnemy()
    {
        //スポーン位置）を取得
        spawnTransformObjects = GameObject.Find("RoadObjects");
        for (int i = 0; i < enemyNum; i++)
        {
            GenerateEnemy();
        }
    }
    //円の内側のランダムな位置に敵を生成する
    void GenerateEnemy()
    {
        //ランダムな子オブジェクトの位置を取得する
        int n = Random.Range(0, spawnTransformObjects.transform.childCount);
        center = spawnTransformObjects.transform.GetChild(n).gameObject.GetComponent<Renderer>().bounds.center;
        // 円の半径
        float radius = 1;
        // 指定された半径の円内のランダム位置を取得
        var circlePos = radius * Random.insideUnitCircle;
        //円内のランダム位置を計算
        var spawnPos = new Vector3(circlePos.x,10f, circlePos.y) + center;
        //EnamyManagerの配下に敵を生成
        GameObject zombie = Instantiate(Zombie, this.gameObject.transform) as GameObject;
        zombie.transform.position = spawnPos;

    }
    //敵の削除
    public void DestroyEnemy()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }
}
