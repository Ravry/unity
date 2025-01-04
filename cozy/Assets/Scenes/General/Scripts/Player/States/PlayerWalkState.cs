using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseState<EPlayerStates>
{
    public PlayerWalkState(EPlayerStates eState) : base(eState)
    {
    }

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("walk", .05f);
        PSM.currentSpeedMultiplier = 1.0f;
    }

    public override void Update()
    {
        PSM.HandleRotation(true);
        PSM.HandleStationaryInput();
        PSM.HandleSpeedControl();
    }

    public override void FixedUpdate()
    {
        PSM.HandleKeyboardMovement();
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        else if (PSM.holdingSprintKey)
            return EPlayerStates.Run;
        else if (Input.GetKey(PSM.keyCodes.aimKey) || Input.GetKey(PSM.keyCodes.shootKey))
                return EPlayerStates.StrafeWalk;

        if (!PSM.grounded)
            return EPlayerStates.Fall;

        return EPlayerStates.Walk;
    }
}