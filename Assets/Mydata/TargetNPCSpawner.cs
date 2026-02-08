using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetNPCSpawner : MonoBehaviour
{
    [Header("目標NPC")]
    [SerializeField] private GameObject targetNPCPrefab;

    [Header("生成候補座標（XYZ）")]
    [SerializeField] private Vector3[] spawnPositions;

    public static Vector3 CurrentTargetPosition { get; private set; }
    public static int CurrentTargetSpawnIndex { get; private set; } = -1;

    private void Start()
    {
        SpawnTargetNPC();
    }

    private void SpawnTargetNPC()
    {
        if (spawnPositions.Length < 3)
        {
            Debug.LogError("生成候補座標が3つ以上必要です");
            return;
        }

        int randomIndex = Random.Range(0, spawnPositions.Length);
        Debug.Log(randomIndex);
        CurrentTargetSpawnIndex = randomIndex;
        CurrentTargetPosition = spawnPositions[randomIndex];

        Instantiate(
            targetNPCPrefab,
            CurrentTargetPosition,
            Quaternion.identity
        );
    }
}



