using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public float speed = 8f;
    public float rotateSpeed = 3f;

    public WaypointNode currentNode;

    void Update()
    {
        if (currentNode == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveToNode();
    }

    void MoveToNode()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentNode.transform.position,
            speed * Time.deltaTime
        );

        Vector3 dir = currentNode.transform.position - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotateSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, currentNode.transform.position) < 0.1f)
        {
            // ゴールなら消える
            if (currentNode.IsGoal)
            {
                Destroy(gameObject);
                return;
            }

            // 次をランダム選択
            currentNode = currentNode.nextNodes[
                Random.Range(0, currentNode.nextNodes.Length)
            ];
        }
    }
}

