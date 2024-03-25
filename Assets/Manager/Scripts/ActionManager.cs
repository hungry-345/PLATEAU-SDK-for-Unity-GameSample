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
        Attack,
        Died
    }
    public State state;
    private PlayerInput playerInput;
    
    //アニメーション管理
    private Animator animator;
    private int animIDDying;
    private StarterAssets.ThirdPersonController thirdPersonController;
    private bool attackAble;
    private AttackHandler attackHandler;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();
        attackHandler = GetComponent<AttackHandler>();
        state = State.Wait;
        attackAble = attackHandler.CheckAttack();
        animIDDying = Animator.StringToHash("Dying");
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    private void Update()
    {
        animIDDying = Animator.StringToHash("Dying");
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
                break;
            case State.Died:
                animator.SetBool(animIDDying, true);
                playerInput.enabled = false;
                break;
        }

    }

    public void ChangeNormal ()
    {
        state = State.Normal ;
    }

}     
    

