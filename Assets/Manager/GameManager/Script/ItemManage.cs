using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManage : MonoBehaviour
{
    //生成するアイテム
    [SerializeField, Tooltip("高さアイテム")] private GameObject measuredheightItem;
    [SerializeField, Tooltip("用途アイテム")] private GameObject UsageItem;
    [SerializeField, Tooltip("アイテムマーカー")] private GameObject marker;

    //生成範囲
    private GameObject spawnTransformObjects;
    private Vector3 center; //スポーン範囲の中心
    private int itemCount;

    public void InitializeItem()
    {
        itemCount = 0;
        //ステージの範囲を取得
        // spawnTransformObjects = GameObject.Find("StageRange");
        //オブジェクトの中心を設定
    }

    //初期化
    public void GenerateItem()
    {
        if(itemCount < 16)
        {
            //スポーン位置にセット
            GameObject hintItem = Instantiate(UsageItem, this.gameObject.transform) as GameObject;
            Vector3 spawnPos = new Vector3(Random.Range(-140f,470f), 100f, Random.Range(1300f,1700f));
            hintItem.name = itemCount + "hintItem";
            hintItem.transform.position = spawnPos;

            GameObject itemMarker = Instantiate(marker,transform.root.gameObject.transform) as GameObject;
            itemMarker.name = itemCount + "ItemMarker";
            itemMarker.transform.localScale = new Vector3(20f, 1f, 20f);
            itemMarker.transform.position = new Vector3(spawnPos.x,-500,spawnPos.z);
            itemCount += 1;
        }
    }
    public void GetItem(GameObject hitItem)
    {
        itemCount -= 1;
        GameObject tmpMarker = GameObject.Find(hitItem.name[0]+"ItemMarker");
        Destroy(tmpMarker);
        Destroy(hitItem);
    }
    //アイテムの削除
    public void DestroyItem()
    {
        foreach (Transform n in gameObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }
}
