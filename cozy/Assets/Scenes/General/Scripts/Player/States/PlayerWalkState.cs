using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("walk", .05f);
    }

    public override void Update()
    {
        PSM.HandleKeyboardMovement();
        PSM.HandleGravity();
        PSM.HandleStationaryInput();
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        else if (PSM.currentSpeedMultiplier == PSM.sprintMultiplier)
            return EPlayerStates.Run;

        if (!PSM.grounded)
            return EPlayerStates.Fall;

        return EPlayerStates.Walk;
    }
}