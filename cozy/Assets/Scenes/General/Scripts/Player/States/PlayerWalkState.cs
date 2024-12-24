using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseState<EPlayerStates>
{
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public override void Update()
    {
        PSM.HandleMouseInput();
        PSM.HandleKeyboardMovement();
        // HandleViewBob(.1f, PSM.speed);
        PSM.HandleStationaryInput();
        PSM.HandleGravity();
    }


    private float moveTime = 0f;
    private void HandleViewBob(float intensity, float speed) {
        moveTime += Time.deltaTime * speed;
        float absSinY = -Mathf.Abs(intensity * Mathf.Sin(moveTime));
        PSM.cam.transform.position += Vector3.up * absSinY;
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