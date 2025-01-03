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
        PSM.animator.CrossFade("run", .05f);
    }

    public override void Update()
    {
        PSM.HandleRotation(true);
        PSM.HandleStationaryInput();
    }

    public override void FixedUpdate() {
        PSM.HandleKeyboardMovement(true);
    }

    public override EPlayerStates CheckState()
    {
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        else
            return EPlayerStates.Walk;

        return EPlayerStates.Run;
    }
}