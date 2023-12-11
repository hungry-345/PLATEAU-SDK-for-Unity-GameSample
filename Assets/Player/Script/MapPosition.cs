using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    Transform Player;
    Transform OverLook;
    int HintNum;
    void Start()
    {
        Player = GameObject.Find("PlayerArmature").transform;
        OverLook = this.transform;

    }
    // Update is called once per frame
    void Update()
    {
        // プレイヤーの座標に合わせてカメラの位置を変更する
        OverLook.position = new Vector3(Player.position.x,0,Player.position.z);
    }
}
