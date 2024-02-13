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
    private GameObject spawnTransformObjects;  //敵のスポーン位置の候補が入ったオブジェクト
    private Vector3 center; //スポーン範囲の中心
    private int itemCount;

    private void Start()
    {
        //スポーン位置）を取得
        spawnTransformObjects = GameObject.Find("RoadObjects");
    }
    public void InitializeItem()
    {
        itemCount = 0;
    }

    //初期化
    public void GenerateItem()
    {
        if(itemCount < 16)
        {

            //ランダムな子オブジェクトの位置を取得する
            int r = Random.Range(0, spawnTransformObjects.transform.childCount);
            center = spawnTransformObjects.transform.GetChild(r).gameObject.GetComponent<Renderer>().bounds.center;
            // 円の半径
            float radius = 1;
            // 指定された半径の円内のランダム位置を取得
            Vector3 circlePos = radius * Random.insideUnitCircle;
            //円内のランダム位置を計算
            Vector3 spawnPos = new Vector3(circlePos.x, 50f, circlePos.y) + center;
            //アイテムを生成
            GameObject item = Instantiate(UsageItem, spawnPos, Quaternion.Euler(-90f, 0f, 0f), this.gameObject.transform);
            item.name = itemCount + "hintItem";
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
        Debug.Log(tmpMarker.name);
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
