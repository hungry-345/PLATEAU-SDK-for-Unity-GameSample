using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    // Start is called before the first frame update
    ReflectionProbe probe;
    private Transform Player;

    void Start()
    {
        this.probe = GetComponent<ReflectionProbe>();
        Player = GameObject.Find("PlayerArmature").transform;
    }

    void Update()
    {
        //y軸は-1をかけて逆側に配置する
        if(Player.position.y < 64f)
        {
            this.probe.cullingMask = 1 << 9;
        }
        else
        {
            this.probe.cullingMask = 0;
        }
        this.probe.transform.position = new Vector3(Camera.main.transform.position.x,
                                                60f,
                                                Camera.main.transform.position.z);

    }
}
