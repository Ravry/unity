using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class PlayerIdleState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void EnterState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        PSM.HandleMouseInput();
        PSM.HandleStationaryInput();
        PSM.HandleGravity();
        PSM.HandleStopViewBobbing();
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
