using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;   // ★ 車プレファブ（複数）
    public Transform spawnPoint;      // 生成位置
    public float spawnInterval = 5f;  // 何秒ごとにスポーンするか

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            // ★ ランダムに1つ選ぶ
            int r = Random.Range(0, carPrefabs.Length);

            // ★ 車を生成
            Instantiate(carPrefabs[r], spawnPoint.position, spawnPoint.rotation);
        }
    }
}
