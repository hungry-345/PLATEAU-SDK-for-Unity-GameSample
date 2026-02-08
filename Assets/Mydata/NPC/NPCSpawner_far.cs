using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner_far : MonoBehaviour
{
    [Header("生成するNPCのプレハブリスト（1パターン）")]
    public GameObject[] npcPattern1;  // 配列の要素番号が TargetNPCSpawner の数字と対応

    [Header("出現位置（Waypointなど）")]
    public Transform[] spawnPoints;

    [Header("生成上限")]
    public int npcCount = 2;

    [Header("生成間隔（秒）")]
    public float spawnInterval = 1f;


    private float timer = 0f;
    private int spawnedCount = 0;

    void Update()
    {
        if (spawnedCount >= npcCount) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnNPC();
        }
    }

    void SpawnNPC()
    {
        if (spawnedCount >= npcCount) return;

        // TargetNPCSpawner から取得した数字をインデックスとして使用
        int index = TargetNPCSpawner.CurrentTargetSpawnIndex;

        // 配列に存在しない場合は生成しない
        if (index < 0 || index >= npcPattern1.Length)
        {
            Debug.Log("CurrentTargetSpawnIndex が配列範囲外なので生成をスキップします。");
            return;
        }

        // ランダムに出現位置を選択
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject npcPrefab = npcPattern1[index];

        // ★Y座標だけ固定する
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = 58.3f; // 固定

        // NPCを生成
        Instantiate(npcPrefab, spawnPos, spawnPoint.rotation);


        spawnedCount++;
    }
}
