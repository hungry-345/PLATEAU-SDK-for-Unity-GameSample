using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PLATEAU.Samples;

public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        Stroll,//
        Follow,//ついていく
        Goal,//ゴールへ向かう
        Escape //敵から逃げる
    };

    [SerializeField]private float walkSpeed=3f;
    [SerializeField]private float runSpeed=15f;
    [SerializeField]private Renderer renderer;
    private CharacterController characterController;
    private GameManage gameManage;
    private NPCManage npcManage;
    private PathManager pathManager;
    private Animator animator;
    private GameObject player;

    //状態
    private NPCState state;
    //目的地との距離
    private float currentDistance;
    //待機時間
    private float waitTime = 3f;
    //目的地に到着した判定距離
    private float arrivedDistance=1.7f;
    //他のNPCとこの距離まで接近したらワープする
    private float NPCWarpDistance=10f;
    //目的地に到着したかフラグ
    private bool isArrived;
    //速度
    private Vector3 velocity;
    //スピード
    private float speed;
    //移動方向
    private Vector3 direction;
    //経過時間
    private float elapsedTime;
    //敵を見失ったフラグ
    private bool isLost;
    //目的地
    private Vector3 NPCDestination;
    //逃げる相手
    private Transform target;
    //particle
    [SerializeField]private GameObject particle;
    private GameObject particleInstance;
    //particleを消すタイミング
    private float duration = 2f;

    //現在いる道路オブジェクト
    private GameObject pastRoadObj;
    private GameObject currentRoadObj;
    private GameObject nextRoadObj;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManage = GameObject.Find("GameManager").GetComponent<GameManage>();
        pathManager = GameObject.Find("RoadObjects").GetComponent<PathManager>();
        npcManage = GameObject.Find("NPCManager").GetComponent<NPCManage>();

        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        velocity = Vector3.zero;
        isArrived = false;
        SetState(NPCState.Stroll);
    }

    void Update()
    {
        this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));

        if (state == NPCState.Follow)//ついていく
        {
            if(Mathf.Abs(Camera.main.transform.forward.x - player.transform.forward.x) < 1.2f && Mathf.Abs(Camera.main.transform.forward.z - player.transform.forward.z) < 1.2f)
            {
                if(gameManage.rescuingNum < 11)
                {
                    SetNPCDestination(player.transform.position + player.transform.right*(1.1f + gameManage.rescuingNum*0.075f));
                }
                else
                {
                    SetNPCDestination(player.transform.position + player.transform.right*(1.85f));
                }
            }
            else
            {
                SetNPCDestination(player.transform.position + player.transform.right* (-1.1f));
            }
            animator.SetFloat("MoveSpeed", runSpeed);

            velocity = Vector3.zero;
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * speed;

            currentDistance = Vector3.Distance(this.transform.position, NPCDestination);
            //プレイヤーに十分近づいたら止まる
            if (currentDistance < arrivedDistance)
            {
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
        else if(state == NPCState.Escape|| state == NPCState.Stroll) //逃げる,巡回する
        {
            animator.SetFloat("MoveSpeed", speed);


            if (isLost == true)//敵からまいた場合
            {
                elapsedTime += Time.deltaTime;  
                if (elapsedTime > waitTime)
                {
                    SetState(NPCState.Stroll);
                }
            }
            //SetNPCDestination(new Vector3(transform.position.x - target.position.x, transform.position.y, transform.position.z - target.position.z));

            //animator.SetFloat("MoveSpeed", runSpeed);
            //    this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));
            //    direction = (NPCDestination - transform.position).normalized;
            //    velocity = direction * runSpeed;

            //    //一定時間経過または目的地に十分近づいたら目的地を更新する
            //    if (/*elapsedTime > waitTime||*/currentDistance < arrivedDistance)
            //    {
            //        //自身の周辺のランダムな道路オブジェクトを新たな目的地として設定する
            //        nextRoadObj = pathManager.GetRandomNeighbor(nextRoadObj, currentRoadObj);
            //        currentRoadObj = nextRoadObj;
            //        SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
            //    }
            //}
            //else if (state == NPCState.Stroll) //巡回する
            //{
            //animator.SetFloat("MoveSpeed", walkSpeed);
            this.transform.LookAt(new Vector3(NPCDestination.x, this.transform.position.y, NPCDestination.z));
            direction = (NPCDestination - transform.position).normalized;
            velocity = direction * speed;

            currentDistance = Vector3.Distance(this.transform.position, NPCDestination);
            //一定時間経過または目的地に十分近づいたら目的地を更新する
            if (/*elapsedTime > waitTime||*/currentDistance < arrivedDistance)
            {
                //かつての目的地を現在地にセット
                pastRoadObj = currentRoadObj;
                currentRoadObj = nextRoadObj;
                //自身の周辺のランダムな道路オブジェクトを新たな目的地として設定する
                nextRoadObj = pathManager.GetRandomNeighbor(currentRoadObj,pastRoadObj);
                SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
            }
        }

        //重力の適用
        velocity.y += (Physics.gravity.y*10f) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    //NPCの状態変更メソッド
    public void SetState(NPCState tempState, Transform targetObj = null)
    {
        state = tempState;
        if (tempState == NPCState.Follow)//ついていく
        {
            animator.SetFloat("MoveSpeed", runSpeed);
            speed = runSpeed;
            SetNPCDestination(player.transform.position);
        }
        else if (tempState == NPCState.Stroll)//巡回する
        {
            isLost = false;
            animator.SetFloat("MoveSpeed", walkSpeed);
            speed = walkSpeed;
            //現在いる道路オブジェクトを取得し，中央へ移動する
            nextRoadObj =pathManager.GetNearestRoadObject(transform);
            currentRoadObj = nextRoadObj;
            SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);

        }
        else if (tempState == NPCState.Goal)//ゴール建物に入る
        {
            EnterGoal();
        }
        else if (tempState == NPCState.Escape)//逃げる
        {
            //１つ前に来た道に戻る
            pastRoadObj = nextRoadObj;
            nextRoadObj = currentRoadObj;
            currentRoadObj = pastRoadObj;

            //逃げる相手を設定
            //target = targetObj;
            //Vector3 escapeDestination = new Vector3(transform.position.x - targetObj.position.x, transform.position.y, transform.position.z - targetObj.position.z);
            SetNPCDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
            animator.SetFloat("MoveSpeed", runSpeed);
            speed = runSpeed;
        }
    }
    //ゴールの建物にはいる（救助される）
    private void EnterGoal()
    {
        gameManage.AddRescueNum();
        npcManage.RemoveFollowList(this.gameObject);
        particleInstance =  Instantiate(particle,this.gameObject.transform.position,Quaternion.Euler(-90,0,0),this.gameObject.transform);
        Destroy(this.gameObject);
        Destroy(particleInstance,duration);
    }
    //NPCの状態取得メソッド
    public NPCState GetState()
    {
        return state;
    }
    //検知範囲にオブジェクトが入った場合
    public void OnObjectStay(Collider collider)
    {
        //プレイヤーを発見
        if (collider.CompareTag("Player"))
        {
            //NPCの状態を取得
            state = GetState();
            //NPCが待機状態or逃げる状態であればついていく設定に変更
            if (state == NPCState.Stroll|| state == NPCState.Escape)
            {
                //救助中の人としてカウント
                if(isArrived==false)
                {
                    gameManage.ContactHumanAction();
                    npcManage.AddFollowList(this.gameObject);
                }
                isArrived = true;
                //Debug.Log("プレイヤー発見");
                SetState(NPCState.Follow);
            }
        }
        //敵
        else if(collider.CompareTag("Enemy"))
        {
            if (state == NPCState.Stroll)
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
            isLost = true;
            //経過時間をリセット
            elapsedTime = 0;
        }

    }
    // 衝突があった場合
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }


    //NPCの目的地を設定
    public void SetNPCDestination(Vector3 destination)
    {
        NPCDestination=destination;
    }
}
