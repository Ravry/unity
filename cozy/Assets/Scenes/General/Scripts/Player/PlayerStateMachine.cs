using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerStateMachine : Statemachine<EPlayerStates>
{
    public static PlayerStateMachine instance;

    [Header("Camera")]
    public CameraHandler cameraHandler;
    

    [Header("Player")]
    public Rigidbody rb;
    public float turnSmoothTime = .1f;
    public float speed = 8.0f;
    public float jumpHeight = 2.0f, groundDistance = .2f;
    public Transform groundCheck; 
    public LayerMask groundMask, playerLayerMask;


    [Header("Animation")]
    public Animator animator;
    public RigBuilder rigBuilder;


    [Header("Weapon")]
    public Weapon weapon;
    public GameObject bulletHolePrefab;


    [Header("KeyCodes")]
    public KeyCodes keyCodes;

    [HideInInspector] public Vector3 inputVec;
    [HideInInspector] public float currentSpeedMultiplier;
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool crouching;
    [HideInInspector] public float turnSmoothVelocity;
    [HideInInspector] public Vector3 normalizedInput;
    
    public override void ReloadStates()
    {
        if (states == null || states.Count == 0)
        {
            SetupStates();
        }

        if (currentState == null && states != null)
        {
            currentState = states[EPlayerStates.Idle];
        }
    }

    public override void SetupStates()
    {
        rb.freezeRotation = true;
        instance = this;
        states = new Dictionary<EPlayerStates, BaseState<EPlayerStates>>();
        states.Add(EPlayerStates.Idle, new PlayerIdleState(EPlayerStates.Idle));
        states.Add(EPlayerStates.Walk, new PlayerWalkState(EPlayerStates.Walk));
        states.Add(EPlayerStates.Run, new PlayerRunState(EPlayerStates.Run));
        states.Add(EPlayerStates.Fall, new PlayerFallState(EPlayerStates.Fall));
        states.Add(EPlayerStates.CrouchIdle, new PlayerCrouchIdleState(EPlayerStates.CrouchIdle));
        states.Add(EPlayerStates.CrouchWalk, new PlayerCrouchWalkState(EPlayerStates.CrouchWalk));
        states.Add(EPlayerStates.StrafeWalk, new PlayerStrafeState(EPlayerStates.StrafeWalk));
        currentState = states[EPlayerStates.Idle];
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void UpdateVars()
    {
        inputVec = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );
        HandleGroundCheck();
        cameraHandler.HandleCameraShake();
        normalizedInput = inputVec.normalized;
    }
    

    public void HandleKeyboardMovement(bool faceTowardsCamera) {
        Vector3 targetVel = Vector3.zero;

        if (faceTowardsCamera)
        {
            float targetAngle = HandleRotation(faceTowardsCamera);
            Vector3 moveForwardDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            targetVel = moveForwardDir * speed;
        }
        targetVel.y = rb.linearVelocity.y;

        Vector3 velDiff = targetVel - rb.linearVelocity;
        velDiff.y = 0;

        rb.AddForce(velDiff, ForceMode.VelocityChange);
    }

    public float HandleRotation(bool faceTowardsCamera) {
        Vector3 forward = Vector3.ProjectOnPlane(cameraHandler.transform.forward, Vector3.up);
        Quaternion desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
        rb.rotation = desiredRotation;
        return 0;
    }

    public void HandleStationaryInput() {
        if (Input.GetKeyDown(keyCodes.jumpKey))
        {    
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(keyCodes.crouchKey))
        {
            crouching = !crouching;
        }
    }

    public void HandleGroundCheck() {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}