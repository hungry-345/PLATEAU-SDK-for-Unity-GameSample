using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;



namespace StarterAssets
{
    public class HookshotHandle : MonoBehaviour
    {
        private Vector3 hookshotPosition;
        private Vector3 characterVelocityMomentum;
        public LineRenderer lr;
        private GameObject player;
        private CharacterController _controller;
        private ActionManager actionManager;
        private ThirdPersonController _thirdPersonController;
        private float gravity;


        [Header("Hookshot")]
        //[SerializeField] private Transform debugHitPosition;
        //攻撃用Hookshotの有効化
        [SerializeField] private bool hookshotAttackable;
        //HookshotできるLayerを設定
        [SerializeField] private LayerMask Hookable;
        //敵の判別用Layerを設定
        [SerializeField] private LayerMask attackable;
        [SerializeField] private LayerMask CityMaterials;
        [SerializeField] private Transform hookshotTransform;
        [SerializeField] private AnimationCurve AnimationCurve;
        [SerializeField] private Material black;
        [SerializeField] private Material orange;
        //攻撃するときの判別
        private bool isHookshotAttack;
        private bool isHookshot;
        private bool isFirstClosed;
        public bool hookshotAble;
        
       
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

        // アタック関連
        [SerializeField] private Texture[] ElectricTextures;
        private int ElectricAnimationStep;
        [SerializeField] private float fps = 30f;
        private float fpsCounter;

        //経過時間
        private float elapsedTime = 0f;
        //麻痺させた場合のロープ解除
        private float lorpRelease = 0.1f;

        //サウンドエフェクト
        [SerializeField] private AudioClip spark;
        private AudioSource sparkSound;

        //アニメーション
        private Animator _animator;
        private bool _hasAnimator;
        private int _animIDRailgun;


        private void Awake()
        {
            player = GameObject.Find("PlayerArmature");
            _controller = GetComponent<CharacterController>();
            actionManager = GetComponent<ActionManager>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
            isHookshotAttack = false;
            isHookshot = false;
            isFirstClosed = false;
            hookshotDir = Vector3.zero;
            distance = 1000f;
            sparkSound = gameObject.AddComponent<AudioSource>();
            sparkSound.clip = spark;
            sparkSound.loop = false;
            _hasAnimator = TryGetComponent(out  _animator);
            _animIDRailgun = Animator.StringToHash("Railgun");
        }

        private void Start()
        {
            gravity = _thirdPersonController.Gravity;
        }

        private void Update()
        {
            if(isHookshot)
            {
                if (isHookshotAttack)
                {

                    //isHookshot = false;
                    elapsedTime += Time.deltaTime;
                    fpsCounter += Time.deltaTime;

                    if(fpsCounter >= 1 / fps)
                    {
                        ElectricAnimationStep++;

                    }
                    if (elapsedTime > lorpRelease)
                    {
                        RemoveHook();
                        isHookshotAttack = false;
                        elapsedTime = 0f;
                        _animator.SetBool(_animIDRailgun, false);
                    }
                }
            }
            if (hookshotAttackable)
            {
                CheckClickLeftMouseButton();
            }

            _hasAnimator = TryGetComponent(out _animator);
            _animIDRailgun = Animator.StringToHash("Railgun");
        }

        private void LateUpdate()
        {
            //DrawRope();
            DrawBeam();
        }

        //左クリック
        private void CheckClickLeftMouseButton()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isHookshot = true;
                //isHookshotAttack = true;
                AttackHook();
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
            // if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitAttack, 50f, attackable))
            if (Physics.Raycast(player.transform.position, player.transform.forward, out RaycastHit hitAttack, 50f, attackable))
            //Hookshotで攻撃する場合
            {
                isHookshotAttack = true;
                isHookshot = true;
                lr.enabled = true;
                
                //hookをArmature_Meshにくっつける
                Transform geometry = hitAttack.transform.Find("Geometry");
                Transform mesh = geometry.Find("Armature_Mesh");
                hookshotPosition = new Vector3(mesh.position.x,mesh.position.y + 1f,mesh.position.z);

                //enemyのstate変更
                enemyController = hitAttack.collider.GetComponent<EnemyController>();
                if(enemyController != null )
                {
                    enemyController.SetState(EnemyController.EnemyState.hit);
                    enemyController.EnemyColorYellow(hitAttack);
                }
                else
                {
                    Transform parent = hitAttack.transform.parent;
                    enemyController = parent.GetComponent<EnemyController>();
                    enemyController.SetState(EnemyController.EnemyState.hit);
                    enemyController.EnemyColorYellow(hitAttack);
                }
                    //se再生
                // sparkSound.Play();
            }
            else if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50f,CityMaterials))
            {
                lr.enabled = true;

                isHookshotAttack = true;
                hookshotPosition = hit.point;
                sparkSound.Play(); 
            }
            else
            {
                lr.enabled = true;
                if (isHookshot)
                {
                    if (!isHookshotAttack) 
                    {
                        isHookshotAttack = true;
                        Vector3 forwardDirection = Camera.main.transform.forward;
                        Vector3 targetPosition = Camera.main.transform.position + forwardDirection * 100f;
                        hookshotPosition = targetPosition;
                        sparkSound.Play();
                    }
                }
            }
            _animator.SetBool(_animIDRailgun, true);
        }
        //旧フックショットの線描画
        public void DrawRope()
        {
            if (isHookshotAttack)
            {
                lr.material = orange;
            }
            lr.SetPosition(0, hookshotTransform.position);
            lr.SetPosition(1, hookshotPosition);
        }

        //攻撃用の線描画
        public void DrawBeam()
        {
            lr.material = orange;
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