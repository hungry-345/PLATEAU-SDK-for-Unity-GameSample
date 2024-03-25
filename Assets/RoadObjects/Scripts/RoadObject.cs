using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scene上の隣接するRoadObjectを管理する
public class RoadObject : MonoBehaviour
{
    public List<Transform> neighborObjects = new List<Transform>();
}
