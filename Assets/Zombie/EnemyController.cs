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

    private Animator animator;
    private GameObject player;

    // 敵の状態
    private EnemyState state;
    //　待ち時間
    private float waitTime = 3f;
    //　経過時間
    private float elapsedTime;
    //　見失うフラグ
    private bool isLost;

    private float distance;
    private Contact contact;
    private ThirdPersonController ThirdPersonController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        ThirdPersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {
        if (state == EnemyState.Chase)//追いかける
        {
            this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            transform.position -= transform.forward * 0.1f;

            if(isLost==true)
            {
                elapsedTime += Time.deltaTime;

                //　見失うまでの時間を超えたら待機する
                if (elapsedTime > waitTime)
                {
                    SetState(EnemyState.Wait);
                    
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
            //巡回
        }
        else if (state == EnemyState.Wait) //待機する
        {
            elapsedTime += Time.deltaTime;

            //　待ち時間を越えたら巡回を始める
            if (elapsedTime > waitTime)
            {
                SetState(EnemyState.Stroll);
            }
        }
    }

    //　敵キャラクターの状態変更メソッド
    public void SetState(EnemyState tempState, Transform targetObj = null)
    {
        if (tempState == EnemyState.Stroll)
        {
            state = tempState;
            elapsedTime = 0f;
            animator.SetBool("IsChase", false);
            //Debug.Log("巡回状態になった");
        }
        else if (tempState == EnemyState.Chase)
        {
            state = tempState;
            elapsedTime = 0f;
            isLost = false;
            //★追いかける対象をセット
            //playerTransform = targetObj;
            animator.SetBool("IsChase", true);
            //Debug.Log("追いかける状態になった");
        }
        else if (tempState == EnemyState.Wait)
        {
            state = tempState;
            elapsedTime = 0f;
            isLost = false;

            //★敵のスピードを変更させる
            animator.SetBool("IsChase", false);
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
            ////　敵キャラクターが追いかける状態でなければ追いかける設定に変更
            //if (state != EnemyController.EnemyState.Chase)
            //{
                //Debug.Log("プレイヤー発見");
             SetState(EnemyController.EnemyState.Chase, collider.transform);
            //}
        }
    }
    //索敵範囲から出たら待機状態にする
    public void OnCharacterExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            //Debug.Log("見失う");
            isLost = true;
            //SetState(EnemyController.EnemyState.Wait);
        }
    }
}
