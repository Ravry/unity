using UnityEngine;

public class WeaponShootState : BaseState<EWeaponStates>
{
    public WeaponShootState(EWeaponStates eState) : base(eState)
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

        if (!WSM.holdingShootKey)
        {
            return EWeaponStates.Idle;            
        }

        return EWeaponStates.Shoot;
    }

    public override void EnterState()
    {
        WSM.SetAimRigWeight(1);
    }
    
    public override void Update()
    {
        WSM.HandleAimRigLogic();
        WSM.HandlePlayerRotation();
        WSM.HandleWeaponShootLogic();
    }

    public override void ExitState()
    {
    }
}
