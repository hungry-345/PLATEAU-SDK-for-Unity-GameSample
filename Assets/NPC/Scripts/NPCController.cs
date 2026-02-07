using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;

    private CharacterController characterController;
    private PathManage pathManage;
    private Animator animator;

    private Vector3 velocity;
    private float speed;
    private Vector3 direction;
    private Vector3 NPCDestination;
    private float currentDistance;

    // 到着と判定する距離
    private float arrivedDistance = 1.7f;

    // 道オブジェクト関連
    private GameObject pastRoadObj;
    private GameObject currentRoadObj;
    private GameObject nextRoadObj;

    // 壁衝突関連
    private bool canReverse = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        pathManage = GameObject.Find("RoadObjects").GetComponent<PathManage>();
        animator = GetComponent<Animator>();

        speed = walkSpeed;

        // 最初の目的地を設定
        currentRoadObj = pathManage.GetNearestRoadObject(transform);
        nextRoadObj = pathManage.GetRandomNeighbor(currentRoadObj, null);
        SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
    }

    void Update()
    {
        // アニメーション速度
        animator.SetFloat("MoveSpeed", speed);

        // 移動方向計算
        direction = (NPCDestination - transform.position).normalized;
        velocity = direction * speed;

        currentDistance = Vector3.Distance(transform.position, NPCDestination);

        // 目的地に到着したら次の目的地へ
        if (currentDistance < arrivedDistance)
        {
            pastRoadObj = currentRoadObj;
            currentRoadObj = nextRoadObj;
            nextRoadObj = pathManage.GetRandomNeighbor(currentRoadObj, pastRoadObj);
            SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
        }

        // 重力を適用
        velocity.y += (Physics.gravity.y * 10f) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // 進行方向を向く
        Vector3 lookPos = new Vector3(NPCDestination.x, transform.position.y, NPCDestination.z);
        transform.LookAt(lookPos);
    }

    // CharacterController専用衝突検知
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall") && canReverse)
        {
            ReverseDirection();
            StartCoroutine(ReverseCooldown());
        }
    }

    // 逆方向へ進路変更
    private void ReverseDirection()
    {
        Vector3 reverseDir = -direction;
        Vector3 newDestination = transform.position + reverseDir * 5f;
        SetNPCDestination(newDestination);
    }

    // 連続反転防止
    private IEnumerator ReverseCooldown()
    {
        canReverse = false;
        yield return new WaitForSeconds(0.5f);
        canReverse = true;
    }

    // 目的地をセット
    private void SetNPCDestination(Vector3 destination)
    {
        NPCDestination = destination;
    }
}
