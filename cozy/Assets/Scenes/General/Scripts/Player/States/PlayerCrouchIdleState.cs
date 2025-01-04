using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : BaseState<EPlayerStates>
{
    public PlayerCrouchIdleState(EPlayerStates eState) : base(eState)
    {
    }

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("crouchidle", .05f);
    }

    
    public override void ExitState()
    {
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
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        if (!PSM.crouching)
            return EPlayerStates.Idle;

        if (PSM.inputVec.normalized.magnitude > 0)
            return EPlayerStates.CrouchWalk;

        return EPlayerStates.CrouchIdle;
    }
}