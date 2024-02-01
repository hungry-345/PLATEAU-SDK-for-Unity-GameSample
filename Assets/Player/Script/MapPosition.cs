using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    Transform Player;
    Transform PlayerPositionMarker;
    Transform PlayerPositionCamera;
    Transform OverLook;
    int HintNum;
    void Start()
    {
        Player = GameObject.Find("PlayerArmature").transform;
        PlayerPositionMarker = GameObject.Find("PositionMarker").transform;
        PlayerPositionCamera = GameObject.Find("PlayerPositionCamera").transform;
        OverLook = this.transform;

    }
    // Update is called once per frame
    void Update()
    {
        Player.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        // プレイヤーの座標に合わせてカメラの位置を変更する
        PlayerPositionMarker.localEulerAngles = new Vector3(0f,angle+180f,0f);
        PlayerPositionMarker.position = new Vector3(Player.position.x,-490,Player.position.z);
        PlayerPositionCamera.position = new Vector3(Player.position.x,-80,Player.position.z);
    }
}
