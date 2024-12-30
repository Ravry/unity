using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class PlayerIdleState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("idle", .2f);
    }

    public override void Update()
    {
        PSM.HandleStationaryInput();
        PSM.HandleGravity();
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.inputVec.magnitude > 0)
            return EPlayerStates.Walk;

        if (!PSM.grounded)
            return EPlayerStates.Fall;

        return EPlayerStates.Idle;    
    }
}
