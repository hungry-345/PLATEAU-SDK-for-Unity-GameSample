using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using StarterAssets;
using PLATEAU.Samples;

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

    private CharacterController characterController;
    private Animator animator;
    private GameObject player;

    // 敵の状態
    private EnemyState state;
    //待ち時間
    private float waitTime = 0.5f;
    //経過時間
    private float elapsedTime;
    //見失うフラグ
    private bool isLost;
    //速度
    private Vector3 velocity;
    //移動方向
    private Vector3 direction;
    //目的地
    private Vector3 enemyDestination;

    private float distance;
    private Contact contact;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        velocity = Vector3.zero;
        SetState(EnemyState.Stroll);
    }

    void FixedUpdate()
    {
        if (state == EnemyState.Chase)//追いかける
        {
            this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            //transform.position -= transform.forward * 0.1f;
            SetEnemyDestination(player.transform.position);

            velocity = Vector3.zero;
            elapsedTime += Time.deltaTime;
               
            
            if (elapsedTime > waitTime)
            {
                if (isLost == true)//待機時間を超えたら待機する
                {
                    SetState(EnemyState.Wait);
                }
                else //待機時間を超えたら追いかける
                {
                    animator.SetFloat("MoveSpeed", runSpeed);
                    direction = (enemyDestination - transform.position).normalized;
                    velocity = direction * runSpeed;
                }
            }

            //キャラクターが死んだら待機状態にする
            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (distance < 2f)
            {
                contact.GameOverFunc();
                SetState(EnemyState.Stroll);
            }
        }
        else if (state == EnemyState.Stroll)//巡回する
        {
            this.transform.LookAt(new Vector3(enemyDestination.x, this.transform.position.y, enemyDestination.z));
            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", walkSpeed);
            direction = (enemyDestination - transform.position).normalized;
            velocity = direction * walkSpeed;
        }
        else if (state == EnemyState.Wait) //待機する
        {
            elapsedTime += Time.deltaTime;
            animator.SetFloat("MoveSpeed", 0f);
            velocity = Vector3.zero;
            //　待ち時間を越えたら巡回を始める
            if (elapsedTime > waitTime)
            {
                SetState(EnemyState.Stroll);
            }
        }
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    //　敵キャラクターの状態変更メソッド
    public void SetState(EnemyState tempState, Transform targetObj = null)
    {
        if (tempState == EnemyState.Stroll)
        {
            state = tempState;
            elapsedTime = 0f;
            animator.SetFloat("MoveSpeed", walkSpeed);
            //Debug.Log("巡回状態になった");
        }
        else if (tempState == EnemyState.Chase)
        {
            state = tempState;
            elapsedTime = 0f;
            isLost = false;
            SetEnemyDestination(player.transform.position);
            //Debug.Log("追いかける状態になった");
        }
        else if (tempState == EnemyState.Wait)
        {
            state = tempState;
            elapsedTime = 0f;
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
        if (collider.CompareTag("Player"))
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
        if (collider.CompareTag("Player"))
        {
            //Debug.Log("見失う");
            isLost = true;
            //経過時間をリセット
            elapsedTime = 0;
            //SetState(EnemyController.EnemyState.Wait);
        }
    }
    //NPCの目的地を設定
    public void SetEnemyDestination(Vector3 destination)
    {
        enemyDestination = destination;
    }
}
