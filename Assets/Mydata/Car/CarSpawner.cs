using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    // ★ シーン上にある車オブジェクトを登録
    public GameObject[] sceneCars;

    // ★ スタート地点
    public WaypointNode[] startNodes;

    public float spawnInterval = 2f;
    private float timer;
    private int carIndex = 0;

    void Start()
    {
        // 最初は全て非表示
        foreach (GameObject car in sceneCars)
        {
            car.SetActive(false);
        }
    }

    void Update()
    {
        //timer += Time.deltaTime;

        //if (timer >= spawnInterval)
        //{
            //timer = 0f;
            SpawnCars();
        //}
    }

    void SpawnCars()
    {
        foreach (WaypointNode start in startNodes)
        {
            if (carIndex >= sceneCars.Length) return;

            GameObject car = sceneCars[carIndex];
            carIndex++;

            // 位置・回転をセット
            car.transform.position = start.transform.position;
            car.transform.rotation = start.transform.rotation;

            // Waypoint設定
            WaypointMover mover = car.GetComponent<WaypointMover>();
            mover.currentNode = start;

            // 表示
            car.SetActive(true);
        }
    }
}

