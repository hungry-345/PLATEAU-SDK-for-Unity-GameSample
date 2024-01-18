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
        [SerializeField] private LayerMask Hookable;
        [SerializeField] private Transform hookshotTransform;
        [SerializeField] private AnimationCurve AnimationCurve;
        private bool isHookshot;
        private bool isFirstClosed;
        public bool hookshotAble;
        public int quality;
        public float waveCount;
        public float waveHeight;
        private float reachedHookshotPositionDistance = 3f;

        private float hookshotSpeedMin = 10f;
        private float hookshotSpeedMax = 40f;
        private float hookshotSpeed = 0;
        private float hookshotSpeedMultipulier = 2f;
        private float distance;
        private float reachedPosY;
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
                PlayerMove();
                if(distance < Vector3.Distance(transform.position, hookshotPosition))
                {
                    isFirstClosed = true;
                    reachedPosY = player.transform.position.y;
                }
                if(Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
                {
                    HookDelete();
                    if(hookshotDir.y > 0.1f || hookshotPosition.y > 10f);
                    {
                        _controller.Move(new Vector3(0f, 6f, 0f));
                    }
                    isHookshot = false;
                }
                distance = Mathf.Abs(Vector3.Distance(transform.position, hookshotPosition));
            }
            CheckClickRightMouseButton();
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
        //Hookshot
        private void CheckClickRightMouseButton()
        {
            if(Input.GetMouseButtonDown(1))
            {
                if(isHookshot)
                {
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
        private void RemoveHook()
        {
            float jumpSpeed = 40f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            HookDelete();
        }
        private void HangHook()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100f, Hookable))
            {
                isHookshot = true;
                lr.enabled = true;
                hookshotPosition = hit.point;
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
        // public void HandleHookshotStart()
        // {
        //     if (Input.GetMouseButtonDown(1))
        //     {
        //         if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100f, Hookable))
        //         {
        //             hookshotAble = true;
        //             hookshotPosition = hit.point;
        //         }
                
        //     }
        // }

        // public void HandleHookshotMovement()
        // {
        //     lr.enabled = true;

        //     if(isHookshot == false)
        //     {
        //         isHookshot = true;
        //         //フックショットのポイント計算
        //         Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;
        //         if (hookshotDir.magnitude > 0.01)
        //         {
        //             // Playerの姿勢が崩れるためy軸削除
        //             player.transform.rotation = Quaternion.LookRotation(new Vector3(hookshotDir.x,0,hookshotDir.z));
        //         }
        //         // 移動処理
        //         float hookshotSpeedMin = 10f;
        //         float hookshotSpeedMax = 40f;
        //         float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        //         float hookshotSpeedMultipulier = 2f;
        //         // _controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultipulier * Time.deltaTime);
        //     }
        //    else
        //    {
        //         isHookshot = false;
        //         // フックショットが終わるときの処理
        //         float reachedHookshotPositionDistance = 3f;
        //         isreached = Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance;
        //         if (isreached)
        //         {
        //             HookDelete();
        //             _controller.Move(new Vector3(0.4f, 3f, 0.4f));
        //             hookshotAble = false;
        //             return;
        //         }

        //         // フックショットが途中で解除されたときの処理
        //         if (Input.GetMouseButtonDown(1))
        //         {
        //             float momentumExtraSpeed = 7f;
        //             characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
        //             float jumpSpeed = 40f;
        //             characterVelocityMomentum += Vector3.up * jumpSpeed;
        //             HookDelete();
        //             return;
        //         }
        //    }
        // }

    }
}