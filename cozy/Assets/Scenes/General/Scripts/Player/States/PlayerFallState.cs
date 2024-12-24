using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        PSM.HandleMouseInput();
        PSM.HandleKeyboardMovement();
        PSM.HandleGravity();
    }

    public override void ExitState()
    {
        
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.grounded)
        {
            if (PSM.inputVec.magnitude > 0)
                return EPlayerStates.Walk;
            else
                return EPlayerStates.Idle;
        }
            
        return EPlayerStates.Fall;    
    }
}