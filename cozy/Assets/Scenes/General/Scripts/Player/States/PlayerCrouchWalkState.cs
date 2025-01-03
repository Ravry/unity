using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchWalkState : BaseState<EPlayerStates>
{
    public PlayerCrouchWalkState(EPlayerStates eState) : base(eState)
    {
    }

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("crouchwalk", .05f);
    }

    public override void ExitState()
    {
    }

    public override void Update()
    {
        PSM.HandleKeyboardMovement(true);
        PSM.HandleStationaryInput();
    }

    public override EPlayerStates CheckState()
    {
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        if (!PSM.crouching)
            return EPlayerStates.Idle;

        if (PSM.inputVec.normalized.magnitude == 0)
            return EPlayerStates.CrouchIdle;

        return EPlayerStates.CrouchWalk;
    }
}