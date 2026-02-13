using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public enum State
    {
        Wait,
        Normal
    }

    public State state;
    private PlayerInput playerInput;
    private StarterAssets.ThirdPersonController thirdPersonController;

    private void Awake()
    {
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();
        playerInput = GetComponent<PlayerInput>();

        state = State.Wait;
        playerInput.enabled = false;
        thirdPersonController.enabled = false;
    }

    private void Update()
    {
        switch(state)
        {
            case State.Wait:
                playerInput.enabled = false;
                thirdPersonController.enabled = false;
                break;

            case State.Normal:
                playerInput.enabled = true;
                thirdPersonController.enabled = true;
                break;
        }
    }

    public void ChangeNormal()
    {
        state = State.Normal;
    }
}
