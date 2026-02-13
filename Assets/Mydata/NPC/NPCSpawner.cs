using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("生成するNPCのプレハブリスト")]
    public GameObject[] npcPrefabs; // 複数種類のNPCを格納

    [Header("出現位置（Waypointなど）")]
    public Transform[] spawnPoints;

    [Header("生成上限")]
    public int npcCount = 5;

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

        // ランダムに出現位置を選択
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // ランダムにNPCを選択
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // ★Y座標だけ固定する
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = 58.3f; // 固定
        
        // NPCを生成
        Instantiate(npcPrefab, spawnPos, spawnPoint.rotation);

        spawnedCount++;
    }
}
