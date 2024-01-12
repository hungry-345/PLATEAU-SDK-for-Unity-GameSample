using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using PLATEAU.Samples;

public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        //Stroll,//巡回する
        Wait,//待機する
        Follow//ついていく
    };

    private Animator animator;
    private GameObject player;

    // NPCの状態
    private NPCState state;

    private float distance;
    private Contact contact;
    //　到着フラグ
    private bool isArrived;
    private ThirdPersonController ThirdPersonController;

    void Start()
    {
        isArrived = false;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        ThirdPersonController = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();
    }

    void FixedUpdate()
    {

        if (state == NPCState.Follow)//追いかける
        {
            this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));
            transform.position += transform.forward * 0.25f;
            animator.SetFloat("MoveSpeed", 10f);

            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (distance < 2f)
            {
                isArrived = true;
                SetState(NPCState.Wait);
            }
        }
        else if (state == NPCState.Wait) //待機する
        {
            if(isArrived == true)
            {
                distance = Vector3.Distance(this.transform.position, player.transform.position);
                if (distance > 2f)
                {
                    isArrived = false;
                    SetState(NPCState.Follow);
                }
            }
        }
    }

    //　敵キャラクターの状態変更メソッド
    public void SetState(NPCState tempState, Transform targetObj = null)
    {
        if (tempState == NPCState.Follow)
        {
            state = tempState;
            isArrived = false;
            animator.SetBool("IsWalking", true);
            Debug.Log("追いかける状態になった");
        }
        else if (tempState == NPCState.Wait)
        {
            state = tempState;
            
            animator.SetBool("IsWalking", false);
            Debug.Log("待機状態になった");
        }
    }
    //　敵キャラクターの状態取得メソッド
    public NPCState GetState()
    {
        return state;
    }

    //プレイヤーが範囲に入ったらついていく
    public void OnCharacterEnter(Collider collider)
    {
        //キャラクターを発見
        if (collider.CompareTag("Player"))
        {
            //　NPCの状態を取得
            NPCController.NPCState state = GetState();
            //　NPCがついていく状態でなければついていく設定に変更
            if (state != NPCController.NPCState.Follow)
            {
                Debug.Log("プレイヤー発見");
                SetState(NPCController.NPCState.Follow, collider.transform);
            }
        }
    }
}
