using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public enum State
    {
        Normal,
        //HookshotFlyingPlayer,
        Attack,
        Died

    }
    public State state;

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

        state = State.Normal;

        //hookshotAble = hookshotHandle.hookshotAble;

        attackAble = attackHandler.checkAttack();

        _hasAnimator = TryGetComponent(out _animator);

        _animIDDying = Animator.StringToHash("Dying");


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
        else
        {
            thirdPersonController.enabled = true;
            attackHandler.enabled = true;
            state = State.Normal;
        }

        switch (state)
        {
            default:
            case State.Normal:
                thirdPersonController.enabled = true;
                break;
            //case State.HookshotFlyingPlayer:
            //    thirdPersonController.enabled = false;
            //    break;
            case State.Attack:
                thirdPersonController.enabled = false;
                break;
            case State.Died:
                _animator.SetBool(_animIDDying, true);
                break;

        }

        //Debug.Log(state);
    }

}     
    

