using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敵を管理するクラス
public class EnemyManage : MonoBehaviour
{
    //生成する敵
    [SerializeField, Tooltip("敵")] private GameObject enemyPrefab;
    //生成範囲
    private GameObject spawnTransformObjects;  //敵のスポーン位置の候補が入ったオブジェクト
    //生成数
    private int enemyNum = 40; //敵の数
    private Vector3 center; //スポーン範囲の中心
    private int enemyCount;

    void Update()
    {
        
    }
    //初期化処理
    public void InitializeEnemy()
    {
        enemyCount = 0;
        //スポーン位置）を取得
        spawnTransformObjects = GameObject.Find("RoadObjects");
        for (int i = 0; i < enemyNum; i++)
        {
            GenerateEnemy();
        }
    }
    //道路上のランダムな位置にに敵を生成する
    void GenerateEnemy()
    {
        enemyCount += 1;
        //ランダムな子オブジェクトの位置を取得する
        int r = Random.Range(0, spawnTransformObjects.transform.childCount);
        center = spawnTransformObjects.transform.GetChild(r).gameObject.GetComponent<Renderer>().bounds.center;
        // 円の半径
        float radius = 1;
        // 指定された半径の円内のランダム位置を取得
        var circlePos = radius * Random.insideUnitCircle;
        //円内のランダム位置を計算
        //var spawnPos = new Vector3(circlePos.x,0f, circlePos.y) + center;
        var spawnPos = new Vector3(circlePos.x,0f, circlePos.y) + center;
        //EnamyManagerの配下に敵を生成
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity,this.gameObject.transform) as GameObject;
        //Enemy.transform.position = spawnPos;
        enemy.name = enemyCount + "Enemy";


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
