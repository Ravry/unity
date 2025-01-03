using UnityEngine;

public class WeaponAimState : BaseState<EWeaponStates>
{
    public WeaponAimState(EWeaponStates eState) : base(eState)
    {
    }

    public static WeaponStateMachine WSM => WeaponStateMachine.instance;
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;
    
    public override EWeaponStates CheckState()
    {
        if (!WSM.holdingAimKey)
        {
            if (WSM.holdingShootKey)
            {
                return EWeaponStates.Shoot;
            }
            else
            {
                return EWeaponStates.Idle;
            }
        }

        return EWeaponStates.Aim;
    }

    public override void EnterState()
    {
        WSM.SetAimRigWeight(1);
        WSM.desiredCamFOV = PSM.cameraHandler.aimedFOV;
    }

    public override void Update()
    {
        WSM.HandlePlayerRotation();
        WSM.HandleAimRigLogic();
        if (WSM.holdingShootKey)
            WSM.HandleWeaponShootLogic();
    }

    public override void ExitState()
    {
        WSM.desiredCamFOV = PSM.cameraHandler.normalFOV;
    }
}
