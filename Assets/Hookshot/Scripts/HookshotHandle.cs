using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;



namespace StarterAssets
{
    public class HookshotHandle : MonoBehaviour
    {
        private Vector3 hookshotPosition;
        //private float hookshotSize;
        private Vector3 characterVelocityMomentum;
        public LineRenderer lr;
        private GameObject player;
        private CharacterController _controller;
        private ActionManager actionManager;
        private ThirdPersonController _thirdPersonController;
        private float gravity;


        [Header("Hookshot")]
        //[SerializeField] private Transform debugHitPosition;
        //HookshotできるLayerを設定
        [SerializeField] private LayerMask Hookable;
        //敵の判別用Layerを設定
        [SerializeField] private LayerMask attackable;
        [SerializeField] private Transform hookshotTransform;
        [SerializeField] private AnimationCurve AnimationCurve;
        //移動するときの判別
        private bool isHookshotMove;
        //攻撃するときの判別
        private bool isHookshotAttack;
        private bool isHookshot;
        private bool isFirstClosed;
        public bool hookshotAble;
        public int quality;
        public float waveCount;
        public float waveHeight;

        //攻撃する敵の情報取得設定
        private EnemyController enemyController;

        //Hookshotの調整
        private float reachedHookshotPositionDistance = 3f;
        private float hookshotSpeedMin = 10f;
        private float hookshotSpeedMax = 40f;
        private float hookshotSpeed = 0;
        private float hookshotSpeedMultipulier = 2f;
        private float distance;
        private float reachedPosY;
        private float hookshotAngleY;
        private Vector3 hookshotDir;
        private Vector3 moveDirection;
        private Vector3 currentHookshot;




        private void Awake()
        {
            player = GameObject.Find("PlayerArmature");
            _controller = GetComponent<CharacterController>();
            actionManager = GetComponent<ActionManager>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
            isHookshotAttack = false;
            isHookshotMove = false;
            isHookshot = false;
            isFirstClosed = false;
            hookshotDir = Vector3.zero;
            distance = 1000f;
        }

        private void Start()
        {
            gravity = _thirdPersonController.Gravity;
        }

        private void Update()
        {
            if(isHookshot)
            {
                if (isHookshotMove)
                {
                    PlayerMove();
                    if (distance < Vector3.Distance(transform.position, hookshotPosition))
                    {
                        isFirstClosed = true;
                        reachedPosY = player.transform.position.y;
                    }
                    if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
                    {
                        HookDelete();
                        if (hookshotAngleY > 0)
                        {
                            _controller.Move(new Vector3(0f, 4f, 0f));
                        }
                        isHookshot = false;
                    }
                    distance = Mathf.Abs(Vector3.Distance(transform.position, hookshotPosition));
                }
                else if (isHookshotAttack)
                {

                }
            }
            CheckClickRightMouseButton();
            CheckClickLeftMouseButton();
        }

        private void LateUpdate()
        {
            DrawRope();
        }

        private void PlayerMove()
        {
            // フックショット(プレイヤー)の向き(y軸0 -> プレイヤーの姿勢が崩れるから)
            hookshotDir = (hookshotPosition - transform.position).normalized;
            if (hookshotDir.magnitude > 0.01)
            {
                player.transform.rotation = Quaternion.LookRotation(new Vector3(hookshotDir.x,0,hookshotDir.z));
            }
            // フックショット時の移動
            if(isFirstClosed)
            {
                hookshotSpeed = Mathf.Clamp(Vector3.Distance(new Vector3(transform.position.x,0,transform.position.z),new Vector3(hookshotPosition.x,0,hookshotPosition.z)),hookshotSpeedMin, hookshotSpeedMax);
                moveDirection = new Vector3(hookshotDir.x,0,hookshotDir.z);
            }
            else
            {
                hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
                moveDirection = hookshotDir;
            }
            _controller.Move(moveDirection * hookshotSpeed * hookshotSpeedMultipulier * Time.deltaTime);
        }

        //Hookshotしたか確認
        private void CheckClickRightMouseButton()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isHookshot)
                {
                    isHookshotMove = false;
                    isHookshotAttack = false;
                    isHookshot = false;

                    RemoveHook();
                }
                else
                {
                    isFirstClosed = false;
                    distance = 1000f;
                    HangHook();
                }
            }
        }

        private void CheckClickLeftMouseButton()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isHookshot)
                {
                    isHookshotMove = false;
                    isHookshotAttack = false;
                    isHookshot = false;

                    RemoveHook();
                }
                else
                {
                    isFirstClosed = false;
                    distance = 1000f;
                    AttackHook();
                }
            }
        }

        private void RemoveHook()
        {
            float jumpSpeed = 40f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            HookDelete();
        }
        //攻撃する場合
        private void AttackHook()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitAttack, 50f, attackable))
            //Hookshotで攻撃する場合
            {
                isHookshotAttack = true;
                isHookshot = true;
                lr.enabled = true;
                hookshotPosition = hitAttack.point;
                //enemyのstate変更
                enemyController = hitAttack.collider.GetComponent<EnemyController>();
                if(enemyController != null )
                {
                    enemyController.SetState(EnemyController.EnemyState.hit);
                }
                else
                {
                    Transform parent = hitAttack.transform.parent;
                    enemyController = parent.GetComponent<EnemyController>();
                    enemyController.SetState(EnemyController.EnemyState.hit);
                }

                //色変更
                // Armature_Meshが見つからない場合、子孫オブジェクトを再帰的に検索
                Renderer[] renderers = hitAttack.transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                {
                    if (rend.gameObject.name == "Armature_Mesh") // 名前で比較
                    {
                        Debug.Log(rend.materials.Length); ; // 色を変更
                        foreach (Material mat in rend.materials)
                        {
                            mat.color = Color.yellow;
                        }
                        break; // 見つかったらループを抜ける
                    }
                }

                RemoveHook();
                //UnityEditor.EditorApplication.isPaused = true;

            }
        }
        //フックショットで移動する場合
        private void HangHook()
        {
              
           if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50f, Hookable))
            //Hookshotで移動する場合
            {
               
                isHookshotMove = true;
                isHookshot = true;
                lr.enabled = true;
                hookshotPosition = hit.point;
                hookshotAngleY = Camera.main.transform.forward.y;
            }
        }
        public void DrawRope()
        {
            currentHookshot = Vector3.Lerp(currentHookshot, hookshotTransform.position, Time.deltaTime * 8f);
            lr.SetPosition(0, hookshotTransform.position);
            lr.SetPosition(1, hookshotPosition);
        }
        private void HookDelete()
        {
            if (lr != null)
            {
                lr.enabled = false;
            }
        }
    }
}