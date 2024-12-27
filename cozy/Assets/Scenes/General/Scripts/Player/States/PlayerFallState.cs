using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;
    private float fallDistance = 0;


    public override void EnterState()
    {
        Physics.Raycast(PSM.groundCheck.position, Vector3.down, out RaycastHit hit, 500.0f, PSM.groundMask);
        fallDistance = hit.distance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        PSM.HandleMouseInput();
        PSM.HandleKeyboardMovement();
        PSM.HandleGravity();
        PSM.HandleStopViewBobbing();
    }

    public override void ExitState()
    {
        // PSM.cam.localPosition = PSM.camOfffset + Vector3.down * .2f;
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.grounded)
        {
            if (PSM.inputVec.magnitude > 0)
                return EPlayerStates.Walk;
            else
                return EPlayerStates.Idle;
        }
            
        return EPlayerStates.Fall;    
    }
}