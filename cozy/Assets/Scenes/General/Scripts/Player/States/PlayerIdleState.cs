using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class PlayerIdleState : BaseState<EPlayerStates>
{
    public PlayerIdleState(EPlayerStates eState) : base(eState)
    {
    }

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        PSM.animator.CrossFade("idle", .2f);
    }

    public override void Update()
    {
        PSM.HandleStationaryInput();
    }

    public override void ExitState()
    {
    }

    public override EPlayerStates CheckState()
    {
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        if (PSM.crouching)
            return EPlayerStates.CrouchIdle;

        if (PSM.inputVec.magnitude > 0)
        {
            if (Input.GetKey(PSM.keyCodes.aimKey) || Input.GetKey(PSM.keyCodes.shootKey))
                return EPlayerStates.StrafeWalk;
            else
                return EPlayerStates.Walk;

        }
            
        return EPlayerStates.Idle;    
    }
}