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
        Wait,//待機
        Stroll,//
        Follow,//ついていく
        Goal,//ゴールへ向かう
        Escape //敵から逃げる
    };

    [SerializeField]private float walkSpeed=5f;
    [SerializeField]private float runSpeed=15f;
    [SerializeField]private Renderer renderer;
    private CharacterController characterController;
    private GameManage gameManage;
    private NPCManager npcManager;
    private Animator animator;
    private GameObject player;
    //private NavMeshAgent navMeshAgent;

    //状態
    private NPCState state;
    //目的地との距離
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
    //particle
    private GameObject particle;
    //particleを消すタイミング
    private float duration = 5f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManage = GameObject.Find("GameManager").GetComponent<GameManage>();
        npcManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();
        particle = GameObject.Find("SaveParticle");

        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        velocity = Vector3.zero;
        isArrived = false;
        SetState(NPCState.Wait);
    }

    void Update()
    {
        //currentDistance = Vector3.Distance(this.transform.position, player.transform.position);

        if (state == NPCState.Follow)//追いかける
        {
            SetNPCDestination(player.transform.position);

            this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));

            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", runSpeed);
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * runSpeed;

            currentDistance = Vector3.Distance(this.transform.position, NPCDestination);

            //プレイヤーに十分近づいたら止まる
            if (currentDistance < followDistance)
            {
                //SetState(NPCState.Stroll);
                velocity = Vector3.zero;
                animator.SetFloat("MoveSpeed", 0f);
            }

            //プレイヤーから離れたらワープする
            if (currentDistance > NPCWarpDistance || !characterController.isGrounded)
            {
                //空中にいるときはNPCを表示しない
                renderer.enabled = false;
                this.transform.Translate(player.transform.position - this.transform.position);
            }
            else
            {
                renderer.enabled = true;
            }
        }
        else if(state == NPCState.Escape) //逃げる
        {
            this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));
            animator.SetFloat("MoveSpeed", runSpeed);
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * runSpeed;
        }
        else if (state == NPCState.Stroll) //巡回する
        {
            this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));
            animator.SetFloat("MoveSpeed",walkSpeed);
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * walkSpeed;

            currentDistance = Vector3.Distance(this.transform.position, NPCDestination);
            //目的地に十分近づいたら目的地を更新する
            if (currentDistance < followDistance)
            {
                SetState(NPCState.Stroll);
            }
        }
        else if(state == NPCState.Wait)
        {
            velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0f);
        }

        //重力の適用
        velocity.y += (Physics.gravity.y) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    //NPCの状態変更メソッド
    public void SetState(NPCState tempState, Transform targetObj = null)
    {
        state = tempState;
        if (tempState == NPCState.Follow)
        {
            animator.SetFloat("MoveSpeed", runSpeed);
            SetNPCDestination(player.transform.position);
            //Debug.Log("追いかける状態になった");
        }
        else if (tempState == NPCState.Stroll)
        {
            Vector3 randomDestination= new Vector3( Random.Range(0, 180), this.transform.position.y, Random.Range(0, 180));
            SetNPCDestination(randomDestination);
            //Debug.Log("待機状態になった");
        }
        else if (tempState == NPCState.Goal)
        {
            EnterGoal();
        }
        else if (tempState == NPCState.Escape)
        {
            //逃げる先を設定
            Vector3 escapeDestination = new Vector3(transform.position.x - targetObj.position.x, transform.position.y, transform.position.z - targetObj.position.z);
            SetNPCDestination(escapeDestination);
            animator.SetFloat("MoveSpeed", runSpeed);

        }
    }
    //ゴールの建物にはいる（救助される）
    private void EnterGoal()
    {
        gameManage.AddRescueNum();
        npcManager.RemoveFollowList(this.gameObject);
        GameObject particleInstance =  Instantiate(particle, this.gameObject.transform.position,Quaternion.Euler(-90,0,0));
        Destroy(this.gameObject);
        Destroy(particleInstance,duration);
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
            //NPCが待機状態or逃げる状態であればついていく設定に変更
            if (state == NPCState.Wait || state == NPCState.Stroll|| state == NPCState.Escape)
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
        //敵
        else if(collider.CompareTag("Enemy"))
        {
            if (state == NPCState.Stroll || state == NPCState.Escape)
            {
                //逃げる
                SetState(NPCState.Escape, collider.gameObject.transform);
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
            if (state == NPCState.Stroll)
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
            if (state == NPCState.Escape)
            {
                SetState(NPCState.Stroll);
            }
        }
    }
    // 衝突があった場合
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            //NPCの状態を取得
            state = GetState();
            //敵に当たったら死ぬ
            if (state != NPCState.Follow)
            {
                Destroy(this.gameObject);
            }
        }
    }

    //NPCの目的地を設定
    public void SetNPCDestination(Vector3 destination)
    {
        NPCDestination=destination;
    }
}
