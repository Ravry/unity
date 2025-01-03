using UnityEngine;

public class WeaponIdleState : BaseState<EWeaponStates>
{
    public WeaponIdleState(EWeaponStates eState) : base(eState)
    {
    }

    public static WeaponStateMachine WSM => WeaponStateMachine.instance;
    public static PlayerStateMachine PSM => PlayerStateMachine.instance;
    
    public override EWeaponStates CheckState()
    {
        if (WSM.holdingAimKey)
        {
            return EWeaponStates.Aim;
        }

        if (WSM.holdingShootKey)
        {
            return EWeaponStates.Shoot;   
        }

        return EWeaponStates.Idle;
    }

    public override void EnterState()
    {
        WSM.SetAimRigWeight(0);
    }

    public override void Update()
    {
        WSM.HandleAimRigLogic();
    }

    public override void ExitState()
    {

    }
}
