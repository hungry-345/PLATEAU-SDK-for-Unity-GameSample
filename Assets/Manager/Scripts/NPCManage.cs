using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManage : MonoBehaviour
{
    //NPCのPrefabの種類を管理する配列
    [SerializeField, Tooltip("NPCPrefab")] private GameObject[] NPCPrefabs;
    //NPCのスポーン位置が入ったオブジェクト
    [SerializeField, Tooltip("NPCSpawnPositions")] private Transform[] NPCSpawnPositions;
    //生成数
    [SerializeField, Tooltip("NPCNum")]private int NPCNum = 5;
    //連れているNPCを管理するリスト
    [SerializeField]List<GameObject> followNPCList = new List<GameObject>(); 

    //初期化処理
    public void InitializeNPC()
    {
        GenerateNPC();
    }
    //NPC生成する
    void GenerateNPC()
    {
        for (int i = 0; i < NPCNum; i++)
        {
            //ランダムな種類のNPCを生成
            int r = Random.Range(0, NPCPrefabs.Length);
            Instantiate(NPCPrefabs[r], NPCSpawnPositions[i].position, Quaternion.identity, this.gameObject.transform);
        }

    }
    //ランダムなNPCを建物に向かわせる
    public void SendBuilding(int NPCNum)
    {
        for(int i=0;i<NPCNum;i++)
        {
            //ランダムなNPCを選択
            int n = Random.Range(0, followNPCList.Count);
            NPCController npcController = followNPCList[n].GetComponent<NPCController>();
        }
    }
    //NPCの削除
    public void DestroyNPC()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }

    //followNPCListの追加
    public void AddFollowList(GameObject NPC)
    {
        followNPCList.Add(NPC);
    }
    //followNPCListの除外
    public void RemoveFollowList(GameObject NPC)
    {
        followNPCList.Remove(NPC);
    }
}
