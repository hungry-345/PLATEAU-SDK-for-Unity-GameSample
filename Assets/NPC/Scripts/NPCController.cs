using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using PLATEAU.Samples;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        Wait,//待機する
        Stroll,//巡回する
        Follow,//ついていく
        Goal,//ゴールへ向かう
        Escape //敵から逃げる
    };

    [SerializeField]private float runSpeed=15f;

    private CharacterController characterController;
    private GameManage gameManage;
    private NPCManager npcManager;
    private Animator animator;
    private GameObject player;
    //private NavMeshAgent agent;

    //状態
    private NPCState state;
    //NPCとプレイヤーの現在の距離
    private float currentDistance;
    //プレイヤーがこの距離まで近づいたらついてくるようになる
    private float followDistance=1.5f;
    //他のNPCとこの距離まで接近したらワープする
    private float NPCWarpDistance=10f;
    //目的地に到着したかフラグ
    private bool isArrived;
    //速度
    private Vector3 velocity;
    //移動方向
    private Vector3 direction;
    //目的地
    private Vector3 NPCDestination;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManage = GameObject.Find("GameManager").GetComponent<GameManage>();
        npcManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();

        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        //agent = GetComponent<NavMeshAgent>();
        velocity = Vector3.zero;
        isArrived = false;
        //SetState(NPCState.Wait);
    }

    void FixedUpdate()
    {
        currentDistance = Vector3.Distance(this.transform.position, player.transform.position);

        if (state == NPCState.Follow)//追いかける
        {
            this.transform.LookAt(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z));

            SetNPCDestination(player.transform.position);

            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", runSpeed);
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * runSpeed;

            //プレイヤーに十分近づいたら待機状態にする
            if (currentDistance < followDistance)
            {
                SetState(NPCState.Wait);
            }
            //プレイヤーから離れたらワープする
            if (currentDistance > NPCWarpDistance || !characterController.isGrounded)
            {
                this.transform.position = player.transform.position;
            }
        }
        else if (state == NPCState.Wait) //待機する
        {
            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0f);
        }
        //else if (state == NPCState.Goal) //ゴールに向かう
        //{
        //    direction = (NPCDestination - transform.position).normalized;
        //    velocity = direction * runSpeed;
        //}
        //重力の適用
        velocity.y += (Physics.gravity.y*10f) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    //NPCの状態変更メソッド
    public void SetState(NPCState tempState, Transform targetObj = null)
    {
        state = tempState;
        if (tempState == NPCState.Follow)
        {
            //agent.isStopped = false;
            animator.SetFloat("MoveSpeed", runSpeed);
            animator.SetBool("IsWalking", true);
            SetNPCDestination(player.transform.position);
            //Debug.Log("追いかける状態になった");
        }
        else if (tempState == NPCState.Wait)
        {
            //agent.velocity = Vector3.zero;
            //agent.isStopped = true;
            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0f);
            animator.SetBool("IsWalking", false);
            //Debug.Log("待機状態になった");
        }
        else if (tempState == NPCState.Goal)
        {
            EnterGoal();
        }
        else if (tempState == NPCState.Escape)
        {
            animator.SetFloat("MoveSpeed", runSpeed);
            animator.SetBool("IsWalking", true);

        }
    }
    //ゴールの建物にはいる（救助される）
    private void EnterGoal()
    {
        gameManage.AddRescueNum();
        npcManager.RemoveFollowList(this.gameObject);
        Destroy(this.gameObject);
    }
    //NPCの状態取得メソッド
    public NPCState GetState()
    {
        return state;
    }
    //検知範囲にオブジェクトが入った場合
    public void OnObjectEnter(Collider collider)
    {
        //プレイヤーを発見
        if (collider.CompareTag("Player"))
        {
            //NPCの状態を取得
            state = GetState();
            //NPCが待機状態OR巡回状態であればついていく設定に変更
            if (state == NPCState.Wait|| state == NPCState.Stroll|| state == NPCState.Escape)
            {
                //救助中の人としてカウント
                if(isArrived==false)
                {
                    gameManage.ContactHumanAction();
                    npcManager.AddFollowList(this.gameObject);
                }
                isArrived = true;
                //Debug.Log("プレイヤー発見");
                SetState(NPCState.Follow);
            }
        }
        //敵を発見
        else if(collider.CompareTag("Enemy"))
        {
            if (state != NPCState.Follow)
            {
                //逃げる対象をセット

                //逃げる
                SetState(NPCState.Escape);
            }
        }
    }

    //検知範囲からオブジェクトが出た場合
    public void OnObjectExit(Collider collider)
    {
        //プレイヤー
        if (collider.CompareTag("Player"))
        {
            //NPCの状態を取得
            state = GetState();
            //NPCが逃げる状態であれば巡回状態に変更
            if (state == NPCState.Wait)
            {
                SetState(NPCState.Follow);
            }
        }
        //敵
        if (collider.CompareTag("Enemy"))
        {
            //NPCの状態を取得
            state = GetState();
            //NPCが逃げる状態であれば巡回状態に変更
            if (state == NPCState.Wait)
            {
                SetState(NPCState.Stroll);
            }
        }
    }
    // 衝突があった場合
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ////ゴールの建物
        //if (hit.gameObject.CompareTag("Goal"))
        //{
        //    //自身を助けた人数としてカウントさせる
        //    gameManage.AddRescueNum();
        //    //自身を消す
        //    Destroy(this.gameObject);
        //}
    }

    //NPCの対象を設定
    public void SetNPCDestination(Vector3 destination)
    {
        NPCDestination=destination;
    }
}
