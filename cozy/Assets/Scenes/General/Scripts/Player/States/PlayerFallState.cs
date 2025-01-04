using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;
    private float startPosY = 0;

    public PlayerFallState(EPlayerStates eState) : base(eState)
    {
    }

    public override void EnterState()
    {
        PSM.SetDrag(0);
        PSM.animator.CrossFade("fall", .05f);
        startPosY = PSM.transform.position.y;
    }

    public override void Update()
    {
        PSM.HandleRotation(false);
        PSM.HandleSpeedControl();
    }

    public override void FixedUpdate()
    {
    }

    private void HandleAirMovement() {

    }

    public override void ExitState()
    {
        PSM.SetDrag(PSM.groundDrag);
        SoundManager.instance.Play("jumpland");
        float endPosY = PSM.transform.position.y;
        float fallDistance = endPosY - startPosY;

        if (fallDistance < -.5f)
            PSM.cameraHandler.CameraShake(fallDistance * 2.0f, .5f);
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