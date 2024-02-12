using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObject : MonoBehaviour
{
    // Start is called before the first frame update
    //隣接するRoadObjectを管理するクラス
    public List<Transform> neighborObjects = new List<Transform>();
}
