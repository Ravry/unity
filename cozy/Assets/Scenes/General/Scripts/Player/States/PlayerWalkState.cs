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
        PSM.HandleStationaryInput();
        PSM.HandleGravity();
        HandleViewBobbing();
    }

    public override void ExitState()
    {
        bobbingTimer = 0;
    }

    private void HandleViewBobbing() {
        bobbingTimer += Time.deltaTime * PSM.bobFrequency * PSM.currentSpeedMultiplier;
        float bobOffsetY = Mathf.Cos(bobbingTimer) * PSM.bobAmplitude * PSM.currentSpeedMultiplier;
        float bobOffsetX = Mathf.Cos(bobbingTimer / 2) * (PSM.bobAmplitude / 2) * PSM.currentSpeedMultiplier;
        PSM.cam.localPosition = Vector3.Lerp(
            PSM.cam.localPosition, 
            PSM.camOfffset + new Vector3(bobOffsetX, bobOffsetY, 0),
            PSM.camResetTime * Time.deltaTime
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