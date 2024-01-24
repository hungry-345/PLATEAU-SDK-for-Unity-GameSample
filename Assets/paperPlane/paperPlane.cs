using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paperPlane : MonoBehaviour
{
    // Start is called before the first frame update
    private ItemManage ItemManageScript;
    void Start()
    {
        ItemManageScript = GameObject.Find("ItemManager").GetComponent<ItemManage>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y < 40)
        {
            ItemManageScript.GetItem(this.gameObject);
        }
    }
}
