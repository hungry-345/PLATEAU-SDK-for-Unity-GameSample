using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PLATEAU.Samples;
using UnityEngine.Scripting;
using JetBrains.Annotations;
using System.Drawing.Printing;

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
    private PathManager pathManager;
    private Animator animator;
    private GameObject player;
    //private NavMeshAgent navMeshAgent;

    // 状態
    private EnemyState state;
    //麻痺フラグ
    private bool paralysis = false;
    //目的地との距離
    private float currentDistance;
    //待機時間
    private float waitTime = 1.5f;
    //麻痺時間
    private float paralysisTime = 5f;
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
    //目的地に到着した判定距離
    private float arrivedDistance = 1.0f;
    private Contact contact;
    //麻痺
    [SerializeField] private GameObject kaminari;
    //private GameObject kaminariInstance;
    private ParticleSystem ps;
    //private float emission;
    //ビリビリ音
    [SerializeField] private AudioClip biribiri;


    //現在いる道路オブジェクト
    private GameObject pastRoadObj;
    private GameObject currentRoadObj;
    private GameObject nextRoadObj;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        strollPosObjects = GameObject.Find("RoadObjects");
        pathManager = strollPosObjects.GetComponent<PathManager>();
        animator = GetComponent<Animator>();
        contact = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        //初期状態
        SetState(EnemyState.Wait);

        //gameView = GameObject.Find("GameView").GetComponent<GameView>();
    }

    void FixedUpdate()
    {
        currentDistance = Vector3.Distance(this.transform.position, enemyDestination);
        this.transform.LookAt(new Vector3(enemyDestination.x, this.transform.position.y, enemyDestination.z));
        if (state == EnemyState.Chase)//追いかける
        {

            if(target!=null)
            {
                SetEnemyDestination(target.position);   
            }
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
                }
            }

            //キャラクターを倒す
            float distance = Vector3.Distance(this.transform.position, player.transform.position);
         

           if (distance < arrivedDistance)
          {                 
             contact.GameOverFunc();
             SetState(EnemyState.Stroll);
                   
          }

        }
        else if (state == EnemyState.Stroll)//巡回する
        {
            //巡回地点まである程度ちかづいたら別の地点へ移動
            if (currentDistance< arrivedDistance)
            {
                //かつての目的地を現在地にセット
                pastRoadObj = currentRoadObj;
                currentRoadObj = nextRoadObj;
                //自身の周辺のランダムな道路オブジェクトを新たな目的地として設定する
                nextRoadObj = pathManager.GetRandomNeighbor(currentRoadObj, pastRoadObj);
                SetEnemyDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
            }
            direction = new Vector3(enemyDestination.x - transform.position.x, 0f, enemyDestination.z - transform.position.z).normalized;
            velocity = direction * walkSpeed;
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
        else if(state == EnemyState.hit)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > paralysisTime)
            {
                SetState(EnemyState.Stroll);
                EnemyColorRed();
                
            }
            else
            {
                animator.SetFloat("MoveSpeed", 0f);

                velocity = Vector3.zero;

                ////雷の量を少なく
                //if (ps)
                //{
                //    AudioSource biribiriSound = gameObject.AddComponent<AudioSource>();
                //    biribiriSound.clip = biribiri;
                //    var emission = ps.emission;
                //    if(elapsedTime > 5f)
                //    {
                //        biribiriSound.loop = true;
                //        emission.rateOverTime = new ParticleSystem.MinMaxCurve(10f,20f);

                //    }
                //    else
                //    {
                //        biribiriSound.loop = false;
                //    }
                  
                //}
            }
        }
        //重力の適用
        velocity.y += (Physics.gravity.y) * Time.deltaTime;
        //移動
        characterController.Move(velocity * Time.deltaTime);

        //if (gameView.GetGameEnd() && kaminari)
        //{
        //    Destroy(kaminari);
        //}
    }

    //　敵キャラクターの状態変更メソッド
    public void SetState(EnemyState tempState, Transform targetObj = null)
    {
        state = tempState;
        elapsedTime = 0f;
        if (tempState == EnemyState.Stroll)
        {
            animator.SetFloat("MoveSpeed", walkSpeed);
            //現在いる道路オブジェクトを取得し，中央へ移動する
            nextRoadObj = pathManager.GetNearestRoadObject(transform);
            currentRoadObj = nextRoadObj;
            SetEnemyDestination(nextRoadObj.GetComponent<Renderer>().bounds.center);
        }
        else if (tempState == EnemyState.Chase)
        {
            isLost = false;
        }
        else if (tempState == EnemyState.Wait)
        {
            isLost = false;
            target = null;
            animator.SetFloat("MoveSpeed", 0f);
            velocity = Vector3.zero;
            //Debug.Log("待機状態になった");
        }
        else if(tempState == EnemyState.hit)
        {
            isLost = true;
            
            //animator.SetBool(Animator.StringToHash("Dying"), true);
            animator.SetFloat("MoveSpeed", 0f);
            velocity = Vector3.zero;
            GameObject kaminariInstance = Instantiate(kaminari, new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z), Quaternion.Euler(0, 0, 0),this.transform);
            ps = kaminariInstance.GetComponent<ParticleSystem>();
            //ビリビリサウンド再生
            AudioSource biribiriSound = kaminariInstance.AddComponent<AudioSource>();
            biribiriSound.clip = biribiri;
            biribiriSound.loop = true;
            biribiriSound.Play();
            Destroy(kaminariInstance,paralysisTime);
            
            

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
        //キャラクターを発見
        if (collider.CompareTag("Player") || collider.CompareTag("NPC"))
        {
            //追いかけるターゲットを設定
            if (target != null)
            {
                //Playerは優先的に追いかける
                if (collider.CompareTag("Player"))
                {
                    target = collider.transform;
                }
            }
            else
            {
                target = collider.transform;
            }
            //現在の状態を取得
            state = GetState();        
            //既に追いかける状態でない
            if(state!= EnemyState.Chase)
            {
                if (state == EnemyState.hit)
                {
                    isLost = true;
                }
                else
                {
                    //追いかける状態にする
                    SetState(EnemyController.EnemyState.Chase, collider.transform);
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
        }
    }

    // 衝突があった場合
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }
    //NPCの目的地を設定
    public void SetEnemyDestination(Vector3 destination)
    {
        enemyDestination = destination;
    }

    //黄色にする
    public void EnemyColorYellow(RaycastHit hitAttack)
    {
        Renderer[] renderers = hitAttack.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend.gameObject.name == "Armature_Mesh") // 名前で比較
            {
                Debug.Log(rend.materials.Length); // 色を変更
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.yellow;
                }
                break; // 見つかったらループを抜ける
            }
        }
    }

    //赤色に戻す
    public void EnemyColorRed()
    {
        Renderer[] renderers = this.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend.gameObject.name == "Armature_Mesh") // 名前で比較
            {
                Debug.Log(rend.materials.Length); ; // 色を変更
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.red;
                }
                break; // 見つかったらループを抜ける
            }
        }
    }

    //ゲーム終了時の処理
    //private void OnDestroy()
    //{
    //    if (kaminariInstance) 
    //    {
    //        Destroy(kaminariInstance);
    //    }
        
    //}
}
