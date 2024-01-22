using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    //NPCのPrefabの種類を管理する配列
    [SerializeField, Tooltip("NPCPrefab")] private GameObject[] NPCPrefabs;
    //NPCのスポーン位置が入ったオブジェクト
    [SerializeField, Tooltip("NPCSpawnPositions")] private Transform[] NPCSpawnPositions;
    //生成数
    [SerializeField, Tooltip("NPCNum")]private int NPCNum = 50;

    //マップにいるNPCを管理するリスト
    public GameObject[] NPCArray = new GameObject[50];




    //初期化処理
    public void InitializeNPC()
    {
        GenerateNPC();
    }
    //NPC生成する
    void GenerateNPC()
    {
        //
        for(int i=0;i<NPCNum;i++)
        {
            //ランダムな種類のNPCを生成
            int r = Random.Range(0, NPCPrefabs.Length);
            Instantiate(NPCPrefabs[r], NPCSpawnPositions[i].position, Quaternion.identity,this.gameObject.transform);
        }

    }
    //ランダムなNPCを建物に向かわせる
    public void SendBuilding(Transform buildingTransform)
    {
        //ランダムなNPCを選択
        int n = Random.Range(0, transform.childCount);
        NPCController npcController = transform.GetChild(n).gameObject.GetComponent<NPCController>();
        //対象のNPCの目的地を設定する
        npcController.SetNPCDestination(buildingTransform.position);
        //NPCをゴールへ向かう状態に変更する
        npcController.SetState(NPCController.NPCState.Goal);
    }
    //NPCの削除
    public void DestroyNPC()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }
}
