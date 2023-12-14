using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    //生成する敵
    [SerializeField, Tooltip("高さアイテム")] private GameObject measuredheightItem;
    [SerializeField, Tooltip("用途アイテム")] private GameObject UsageItem;

    //初期化処
    public void GenerateItem()
    {
        //★GameViewの子として生成
        GameObject hintItem = Instantiate(measuredheightItem, this.gameObject.transform) as GameObject;
        hintItem.name = "measuredheight";
        float itemPosX = Random.Range(0, 550f);
        float itemPosZ = Random.Range(0, 700f);
        hintItem.transform.position = new Vector3(itemPosX, 100, itemPosZ);

        hintItem = Instantiate(UsageItem, this.gameObject.transform) as GameObject;
        hintItem.name = "Usage";
        itemPosX = Random.Range(-400f, 450f);
        itemPosZ = Random.Range(-200f, 200f);
        hintItem.transform.position = new Vector3(itemPosX, 100, itemPosZ);
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
