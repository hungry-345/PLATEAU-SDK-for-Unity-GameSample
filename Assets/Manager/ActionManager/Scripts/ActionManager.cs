using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public enum State
    {
        Wait,
        Normal,
        //HookshotFlyingPlayer,
        Attack,
        Died

    }
    public State state;

    //inputsystem
    private PlayerInput playerInput;
    

    //アニメーション管理
    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDDying;

    private StarterAssets.ThirdPersonController thirdPersonController;

    private StarterAssets.HookshotHandle hookshotHandle;

    private bool hookshotAble;

    private bool attackAble;

    private attackHandler attackHandler;

    private void Awake()
    {
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();

        //hookshotHandle = GetComponent<StarterAssets.HookshotHandle>();

        attackHandler = GetComponent<attackHandler>();

        state = State.Wait;

        //hookshotAble = hookshotHandle.hookshotAble;

        attackAble = attackHandler.checkAttack();

        _hasAnimator = TryGetComponent(out _animator);

        _animIDDying = Animator.StringToHash("Dying");

        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    private void Update()
    {

        _hasAnimator = TryGetComponent(out _animator);
        _animIDDying = Animator.StringToHash("Dying");
        //if (hookshotAble == true)
        //{
        //    thirdPersonController.enabled = false;
        //    attackHandler.enabled = false;
        //    state = State.HookshotFlyingPlayer;
        //}
        if(attackAble == true)
        {
            thirdPersonController.enabled = false;
            attackHandler.enabled = false;  
        }
        else if(state == State.Died)
        {
            state = State.Died;
            attackHandler.enabled = false;
        }
        else if(state == State.Wait)
        {
            state = State.Wait;
        }
        else
        {
            thirdPersonController.enabled = true;
            attackHandler.enabled = true;
            state = State.Normal;
        }

        switch (state)
        {
            default:
            case State.Wait:
                playerInput.enabled = false; 
                break;
            case State.Normal:
                playerInput.enabled = true;
                //thirdPersonController.enabled = true;
                break;
            //case State.HookshotFlyingPlayer:
            //    thirdPersonController.enabled = false;
            //    break;
            //case State.Attack:
            //    thirdPersonController.enabled = false;
            //    break;
            case State.Died:
                _animator.SetBool(_animIDDying, true);
                playerInput.enabled = false;
                break;

        }

        //Debug.Log(state);
    }

    public void changeNormal ()
    {
        state = State.Normal ;
    }

}     
    

