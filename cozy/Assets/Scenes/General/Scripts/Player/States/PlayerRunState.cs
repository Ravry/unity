using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : BaseState<EPlayerStates>
{
    public PlayerRunState(EPlayerStates eState) : base(eState)
    {
    }

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.currentSpeedMultiplier = PSM.sprintMultiplier;
        PSM.animator.CrossFade("run", .05f);
    }

    public override void Update()
    {
        PSM.HandleRotation(true);
        PSM.HandleStationaryInput();
        PSM.HandleSpeedControl();
    }

    public override void FixedUpdate() {
        PSM.HandleKeyboardMovement();
    }

    public override void ExitState()
    {
        PSM.currentSpeedMultiplier = 1;
    }

    public override EPlayerStates CheckState()
    {
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        else if (!PSM.holdingSprintKey)
            return EPlayerStates.Walk;

        return EPlayerStates.Run;
    }
}