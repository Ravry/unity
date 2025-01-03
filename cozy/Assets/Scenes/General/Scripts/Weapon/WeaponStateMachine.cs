using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponStateMachine : Statemachine<EWeaponStates>
{

    public static PlayerStateMachine PSM => PlayerStateMachine.instance;

    public static WeaponStateMachine instance;
    
    public Rig aimRig;


    [HideInInspector] public bool holdingShootKey, holdingAimKey, duringAim;
    public float shootTime, aimTime, desiredRigWeight, desiredCamFOV;

    public override void ReloadStates()
    {
        if (states == null || states.Count == 0)
        {
            SetupStates();
        }

        if (currentState == null && states != null)
        {
            currentState = states[EWeaponStates.Idle];
        }
    }

    public override void SetupStates()
    {
        instance = this;
        states = new Dictionary<EWeaponStates, BaseState<EWeaponStates>>();
        states.Add(EWeaponStates.Idle, new WeaponIdleState(EWeaponStates.Idle));
        states.Add(EWeaponStates.Aim, new WeaponAimState(EWeaponStates.Aim));
        states.Add(EWeaponStates.Shoot, new WeaponShootState(EWeaponStates.Shoot));
        currentState = states[EWeaponStates.Idle];
    }

    public override void UpdateVars()
    {
        aimTime -= Time.deltaTime;
        shootTime -= Time.deltaTime;
        holdingShootKey = Input.GetKey(PSM.keyCodes.shootKey);
        holdingAimKey = Input.GetKey(PSM.keyCodes.aimKey);
    }

    public void HandleWeaponShootLogic() {
        if (duringAim)
            return;

        if (shootTime <= 0)
        {
            shootTime = PSM.weapon.shootDelay;
            if (Physics.Raycast(PSM.cameraHandler.transform.position, PSM.cameraHandler.transform.forward, out RaycastHit hitInfo, PSM.weapon.distance, PSM.groundMask))
                {
                    if (hitInfo.transform.tag == "Target")
                    {
                        Target target = hitInfo.transform.GetComponent<Target>();
                        target.Dissolve();
                    }
                    Debug.DrawLine(PSM.cameraHandler.transform.position, hitInfo.point, Color.red, .5f);
                    Instantiate(PSM.bulletHolePrefab, hitInfo.point + hitInfo.normal * .05f, Quaternion.LookRotation(-hitInfo.normal));
                }

                PSM.cameraHandler.cinemachineCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value -= PSM.weapon.shootRecoil;
                PSM.cameraHandler.CameraShake(PSM.weapon.shootRecoil, PSM.weapon.shootDelay);
                // SoundManager.instance.Play("shot", .8f, 1.2f);
        }
    }

    public void SetAimRigWeight(float weight) {
        aimTime = PSM.weapon.aimDuration;
        desiredRigWeight = weight;
    }

    public void HandleAimRigLogic() {
        duringAim = aimRig.weight != desiredRigWeight;
        if (duringAim)
        {
            aimRig.weight = Mathf.Lerp(desiredRigWeight, aimRig.weight, aimTime/PSM.weapon.aimDuration);
            PSM.cameraHandler.cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(desiredCamFOV, PSM.cameraHandler.cinemachineCamera.Lens.FieldOfView, aimTime/PSM.weapon.aimDuration);
        }
    }

    public void HandlePlayerRotation() {
        if (PSM.currentState.eState == EPlayerStates.Idle)
        {
            PSM.HandleRotation(true);
        }
    }

}