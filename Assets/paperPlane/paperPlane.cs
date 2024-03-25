using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlane : MonoBehaviour
{
    private ItemManage itemManage;
    void Start()
    {
        itemManage = GameObject.Find("ItemManager").GetComponent<ItemManage>();
    }

    void Update()
    {
        if(this.transform.position.y < 40)
        {
            itemManage.GetItem(this.gameObject);
        }
    }
}
