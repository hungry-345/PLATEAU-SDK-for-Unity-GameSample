using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PLATEAU.Samples;
using UnityEngine.AI;
using UnityEngine.Scripting;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Stroll,//巡回する
        Wait,//待機する（キャラクターを見失った/倒した）
        Chase,//追いかける
        hit//攻撃を受けた
    };
    //走るスピード
    [SerializeField] private float runSpeed = 5f;
    //歩くスピード
    [SerializeField] private float walkSpeed = 1f;
    //視界の範囲
    [SerializeField] private float sightAngle = 90f;


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
    private float chaseOffsetTime = 0f;
    //経過時間
    private float elapsedTime;
    //見失うフラグ
    private bool isLost;
    //目的地
    private Vector3 enemyDestination;
    //追いかける相手
    private Transform target;
    //速度
    private Vector3 velocity;
    //移動方向
    private Vector3 direction;
    private float distance;
    private Contact contact;
    //マテリアル
    private GameObject meshObj;
    private Transform meshTrans;
    private Material material;
    

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        strollPosObjects = GameObject.Find("RoadObjects");
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        //色変更
        //meshTrans = transform.Find("Armature_Mesh");
        //meshObj = meshTrans.gameObject;
        //Renderer render = meshTrans.GetComponent<Renderer>();
        //if(render != null )
        //{
        //    for(int i = 0;i< render.materials.Length; i++)
        //    {
        //        render.materials[i].SetColor("_BaseColor", Color.red);
        //    }
        //}
        meshObj = GameObject.Find("Armature_Mesh");
        if(meshObj != null)
        {
            Renderer render = meshObj.GetComponent<Renderer>();
            for (int i = 0; i < render.sharedMaterials.Length; i++)
           {
               render.materials[i].SetColor("_BaseColor", Color.red);
           }
        }
        //velocity = Vector3.zero;
        SetState(EnemyState.Wait);
    }

    void FixedUpdate()
    {
        currentDistance = Vector3.Distance(this.transform.position, enemyDestination);
        if (state == EnemyState.Chase)//追いかける
        {
            this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));

            SetEnemyDestination(target.position);
            //navMeshAgent.SetDestination(enemyDestination);
            elapsedTime += Time.deltaTime;
               
            if (isLost == true)//見失ったら待機する
            {
                if (elapsedTime > waitTime)
                {
                    SetState(EnemyState.Wait);
                }
            }
            else //追いかける
            {
                if (elapsedTime > chaseOffsetTime)
                {
                    velocity = Vector3.zero;
                    animator.SetFloat("MoveSpeed", runSpeed);
                    direction = new Vector3(enemyDestination.x - transform.position.x, 0f, enemyDestination.z - transform.position.z).normalized;
                    velocity = direction * runSpeed;                
                    //navMeshAgent.speed = runSpeed;
                }

            }

            //キャラクターを倒す
            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (distance < 1f)
            {
                contact.GameOverFunc();
                SetState(EnemyState.Stroll);
            }

            //重力の適用
            //velocity.y += (Physics.gravity.y) * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

        }
        else if (state == EnemyState.Stroll)//巡回する
        {

            //巡回地点まである程度ちかづいたら別の地点へ移動
            if (currentDistance<2f)
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
            //navMeshをアクティブにする
            navMeshAgent.enabled = true;

            animator.SetFloat("MoveSpeed", walkSpeed);
            navMeshAgent.speed = walkSpeed;
            //ランダムな目的地へ向かう
            SetStrollDestination();
            //Debug.Log("巡回状態になった");
        }
        else if (tempState == EnemyState.Chase)
        {
            //navMeshを非アクティブにする
            navMeshAgent.enabled = false;
            //追いかけるターゲットを設定
            isLost = false;
            target = targetObj;
        }
        else if (tempState == EnemyState.Wait)
        {
            isLost = false;
            animator.SetFloat("MoveSpeed", 0f);
            //Debug.Log("待機状態になった");
        }
        else if(tempState == EnemyState.hit)
        {
            isLost = true;
            //animator.SetBool(Animator.StringToHash("Dying"), true);
            animator.SetFloat("MoveSpeed", 0f);
            

        }
    }
    //　敵キャラクターの状態取得メソッド
    public EnemyState GetState()
    {
        return state;
    }

    //索敵範囲に入ったら
    public void OnCharacterEnter(Collider collider)
    {
        //キャラクターを発見(Playerは優先的に追いかける)
        if (collider.CompareTag("Player") || (!target.CompareTag("Player")&&(collider.CompareTag("NPC"))))
        {

            //NPCの状態を取得
            state = GetState();
            
            if(state!= EnemyState.Chase)
            {
                if (state == EnemyState.hit)
                {
                    isLost = true;
                }
                else
                {
                    //視界に入っているかの判定
                    //Vector3 posDelta = collider.transform.position - transform.position;
                    //float targetAngle = Vector3.Angle(transform.forward, posDelta);
                    //if(targetAngle<sightAngle)
                    //{
                    //追いかける状態にする
                    SetState(EnemyController.EnemyState.Chase, collider.transform);
                    isLost = false;
                    //}
                }
            }
        }
    }
    //索敵範囲から出たら見失う
    public void OnCharacterExit(Collider collider)
    {
        if (collider.transform==target)
        {
            //Debug.Log("見失う");
            isLost = true;
            //経過時間をリセット
            elapsedTime = 0;
            target = null;
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
