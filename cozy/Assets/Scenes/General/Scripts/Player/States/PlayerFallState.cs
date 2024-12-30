using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;
    private float startPosY = 0;


    public override void EnterState()
    {
        PSM.animator.CrossFade("fall", .05f);
        startPosY = PSM.transform.position.y;
    }

    public override void Update()
    {
        PSM.HandleKeyboardMovement();
        PSM.HandleGravity();
    }

    public override void ExitState()
    {
        float endPosY = PSM.transform.position.y;
        float fallDistance = endPosY - startPosY;

        if (fallDistance < -2.0f)
            PSM.CameraShake(fallDistance, .5f);
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