using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //生成する敵
    [SerializeField, Tooltip("ゾンビ")] private GameObject Zombie;
    private int enemyNum = 50; //敵の数

    //初期化処理
    public void InitializeEnemy()
    {
        for (int i = 0; i < enemyNum; i++)
        {
            GenerateEnemy();
        }
    }
    void GenerateEnemy()
    {
        //EnamyManagerの配下に敵を生成
        GameObject zombie = Instantiate(Zombie, this.gameObject.transform) as GameObject;
        //zombie.name = "zombie";
        float itemPosX = Random.Range(-400f, 400f);
        float itemPosZ = Random.Range(-200f, 200f);
        zombie.transform.position = new Vector3(itemPosX, 100, itemPosZ);
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
