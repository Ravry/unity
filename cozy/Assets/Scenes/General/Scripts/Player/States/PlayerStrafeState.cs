using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStrafeState : BaseState<EPlayerStates>
{
    public PlayerStrafeState(EPlayerStates eState) : base(eState)
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
        PSM.HandleStationaryInput();
    }

    public override void FixedUpdate()
    {
        PSM.HandleKeyboardMovement();
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        else if (Input.GetKey(PSM.keyCodes.sprintKey))
            return EPlayerStates.Run;
        else if (!Input.GetKey(PSM.keyCodes.shootKey) && !Input.GetKey(PSM.keyCodes.aimKey))
            return EPlayerStates.Walk;

        if (!PSM.grounded)
            return EPlayerStates.Fall;

        return EPlayerStates.StrafeWalk;
    }
}