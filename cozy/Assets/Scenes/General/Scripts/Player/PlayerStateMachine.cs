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
    public float rotationSpeed = 20f;
    public float moveSpeed = 3.0f, sprintMultiplier = 1.5f;
    public float jumpForce = 2.0f, groundDistance = .2f, groundDrag = 2f;
    public Transform orientation, playerObj, groundCheck; 
    public LayerMask groundMask, playerLayerMask;


    [Header("Animation")]
    public Animator animator;
    public RigBuilder rigBuilder;


    [Header("Weapon")]
    public Weapon weapon;
    public GameObject bulletHolePrefab;


    [Header("KeyCodes")]
    public KeyCodes keyCodes;

    [HideInInspector] public Vector3 inputVec, inputDir;
    [HideInInspector] public float currentSpeedMultiplier;
    [HideInInspector] public bool grounded, holdingSprintKey;
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
        holdingSprintKey = Input.GetKey(keyCodes.sprintKey);
        inputVec = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );
        inputDir = orientation.forward * inputVec.z + orientation.right * inputVec.x;
        HandleGroundCheck();
        cameraHandler.HandleCameraShake();
        normalizedInput = inputVec.normalized;

        Temp();
    }

    private void Temp() {
        Vector3 debugPos = transform.position + Vector3.up * 1.4f;
        Vector3 currentVelocity = rb.linearVelocity;
        currentVelocity.y = 0;
        Debug.DrawRay(debugPos, currentVelocity, Color.black);
    }
    

    public void HandleKeyboardMovement() {
        Vector3 moveDir = inputDir.normalized * moveSpeed * currentSpeedMultiplier * 10f;
        rb.AddForce(moveDir, ForceMode.Force);
    }

    public void HandleSpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed * currentSpeedMultiplier)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed * currentSpeedMultiplier;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    public void HandleRotation(bool move) {
        Vector3 viewDir = transform.position - new Vector3(cameraHandler.transform.position.x, transform.position.y, cameraHandler.transform.position.z);
        orientation.forward = viewDir.normalized;
        if (move) {
            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        else {
            playerObj.forward = Vector3.Slerp(playerObj.forward, orientation.forward, Time.deltaTime * rotationSpeed);
        }
    }

    public void SetDrag(float drag)
    {
        rb.linearDamping = drag;
    }

    public void HandleStationaryInput() {
        if (Input.GetKeyDown(keyCodes.jumpKey))
        {    
            SetDrag(0);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    private void OnGUI()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0, 100, 500, 50), $"Speed: {flatVelocity.magnitude}", style);
    }
}