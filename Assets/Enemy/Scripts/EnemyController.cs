using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PLATEAU.Samples;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Stroll,//巡回する
        Wait,//待機する（キャラクターを見失った/倒した）
        Chase//追いかける
    };
    //走るスピード
    [SerializeField] private float runSpeed = 5f;
    //歩くスピード
    [SerializeField] private float walkSpeed = 1f;
    //巡回地点の親オブジェクト
    private GameObject strollPosObjects;

    private CharacterController characterController;
    private Animator animator;
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    // 状態
    private EnemyState state;
    //目的地との距離
    private float currentDistance;
    //待機時間
    private float waitTime = 1.5f;
    //みつけてから追いかけるまでの時間
    private float chaseOffsetTime = 0.5f;
    //経過時間
    private float elapsedTime;
    //見失うフラグ
    private bool isLost;
    //目的地
    private Vector3 enemyDestination;

    private float distance;
    private Contact contact;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        strollPosObjects = GameObject.Find("RoadObjects");
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        //velocity = Vector3.zero;
        SetState(EnemyState.Wait);
    }

    void FixedUpdate()
    {
        currentDistance = Vector3.Distance(this.transform.position, enemyDestination);
        if (state == EnemyState.Chase)//追いかける
        {
            //this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            SetEnemyDestination(player.transform.position);
            navMeshAgent.SetDestination(enemyDestination);
            elapsedTime += Time.deltaTime;
               
            if (isLost == true)//見失ったら待機する
            {
                if (elapsedTime > waitTime)
                    SetState(EnemyState.Wait);
            }
            else //待機時間を超えたら追いかける
            {
                if (elapsedTime > chaseOffsetTime)
                {
                    animator.SetFloat("MoveSpeed", runSpeed);
                    navMeshAgent.speed = runSpeed;
                }
            }

            //キャラクターが死んだら巡回状態にする
            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (distance < 2f)
            {
                contact.GameOverFunc();
                SetState(EnemyState.Stroll);
            }
        }
        else if (state == EnemyState.Stroll)//巡回する
        {

            //巡回地点まである程度ちかづいたら別の地点へ移動
            if (currentDistance<1f)
            {
               SetStrollDestination();
            }
        }
        else if (state == EnemyState.Wait) //待機する
        {
            elapsedTime += Time.deltaTime;
            animator.SetFloat("MoveSpeed", 0f);
            navMeshAgent.velocity = Vector3.zero;
            //　待ち時間を越えたら巡回を始める
            if (elapsedTime > waitTime)
            {
                SetState(EnemyState.Stroll);
            }
        }
    }
    //ランダムな巡回地点を取得する
    private void SetStrollDestination()
    {
        //ランダムな子オブジェクトの位置を取得する
        int r = Random.Range(0, strollPosObjects.transform.childCount);
        Vector3 newStrollPoint = strollPosObjects.transform.GetChild(r).gameObject.GetComponent<Renderer>().bounds.center;
        //目的地に設定
        SetEnemyDestination(new Vector3(newStrollPoint.x, this.transform.position.y, newStrollPoint.z));
        navMeshAgent.SetDestination(enemyDestination);
    }

    //　敵キャラクターの状態変更メソッド
    public void SetState(EnemyState tempState, Transform targetObj = null)
    {
        state = tempState;
        elapsedTime = 0f;
        if (tempState == EnemyState.Stroll)
        {
            animator.SetFloat("MoveSpeed", walkSpeed);
            navMeshAgent.speed = walkSpeed;
            //ランダムな目的地へ向かう
            SetStrollDestination();
            //Debug.Log("巡回状態になった");
        }
        else if (tempState == EnemyState.Chase)
        {
            isLost = false;
            //SetEnemyDestination(player.transform.position);
            navMeshAgent.SetDestination(player.transform.position);
            //Debug.Log("追いかける状態になった");
        }
        else if (tempState == EnemyState.Wait)
        {
            isLost = false;
            animator.SetFloat("MoveSpeed", 0f);
            //Debug.Log("待機状態になった");
        }
    }
    //　敵キャラクターの状態取得メソッド
    public EnemyState GetState()
    {
        return state;
    }

    //索敵範囲に入ったら追いかける状態にする
    public void OnCharacterEnter(Collider collider)
    {
        //キャラクターを発見
        if (collider.CompareTag("Player")/*|| collider.CompareTag("NPC")*/)
        {
            //　敵キャラクターの状態を取得
            EnemyController.EnemyState state = GetState();
            //Debug.Log("プレイヤー発見");
            SetState(EnemyController.EnemyState.Chase, collider.transform);
            isLost = false;
        }
    }
    //索敵範囲から出たら待機状態にする
    public void OnCharacterExit(Collider collider)
    {
        if (collider.CompareTag("Player")/* || collider.CompareTag("NPC")*/)
        {
            //Debug.Log("見失う");
            isLost = true;
            //経過時間をリセット
            elapsedTime = 0;
        }
    }

    // 衝突があった場合
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.gameObject.CompareTag("Player"))
        //{
        //    contact.GameOverFunc();
        //    SetState(EnemyState.Stroll);
        //}
    }
    //NPCの目的地を設定
    public void SetEnemyDestination(Vector3 destination)
    {
        enemyDestination = destination;
    }
}
