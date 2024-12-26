using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    private float bobbingTimer = 0.0f;

    public override void Update()
    {
        PSM.HandleMouseInput();
        PSM.HandleKeyboardMovement();
        HandleViewBobbing();
        PSM.HandleStationaryInput();
        PSM.HandleGravity();
    }

    public override void ExitState()
    {
        bobbingTimer = 0f;
    }

    private void HandleViewBobbing() {
        bobbingTimer += Time.deltaTime * PSM.bobFrequency;
        float bobOffsetY = Mathf.Sin(bobbingTimer) * PSM.bobAmplitude;
        float bobOffsetX = Mathf.Cos(bobbingTimer / 2) * PSM.bobAmplitude / 2;
        PSM.cam.position = new Vector3(
            PSM.cam.position.x + bobOffsetX,
            PSM.cam.position.y + bobOffsetY,
            PSM.cam.position.z
        );
    }

    public override EPlayerStates CheckState()
    {
        if (PSM.inputVec.magnitude == 0)
            return EPlayerStates.Idle;
        if (!PSM.grounded)
            return EPlayerStates.Fall;

        return EPlayerStates.Walk;
    }
}