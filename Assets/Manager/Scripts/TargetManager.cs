using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField, Tooltip("目的となるNPCプレハブ")] private GameObject targetNPC;

    private GameObject spawnTransformObjects;  // スポーン位置候補が入った親オブジェクト

    private void Start()
    {
        // RoadObjects を探す（ItemManage と同じ）
        spawnTransformObjects = GameObject.Find("RoadObjects");

        if (spawnTransformObjects == null)
        {
            Debug.LogError("RoadObjects がシーン内に見つかりません！");
            return;
        }

        SpawnTargetNPC();
    }

    private void SpawnTargetNPC()
    {
        // ランダムな子オブジェクトの位置を取得
        int r = Random.Range(0, spawnTransformObjects.transform.childCount);
        Transform child = spawnTransformObjects.transform.GetChild(r);

        // Renderer がある場合はその中心を使う（ItemManageと同様）
        Vector3 spawnPos;
        Renderer rend = child.GetComponent<Renderer>();
        if (rend != null)
        {
            spawnPos = rend.bounds.center;
        }
        else
        {
            // Rendererが無い場合は単純にTransformの位置を利用
            spawnPos = child.position;
        }

        // NPCを生成
        GameObject npc = Instantiate(targetNPC, spawnPos, Quaternion.identity);
        npc.name = "TargetNPC";
    }
}
