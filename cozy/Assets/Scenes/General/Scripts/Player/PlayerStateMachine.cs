using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerStateMachine : Statemachine<EPlayerStates>
{
    public static PlayerStateMachine instance;

    [Header("Camera")]
    public Transform cam;
    public CinemachineCamera cinemachineCamera;
    public float turnSmoothT = .1f;
    

    [Header("Player")]
    public LayerMask playerLayerMask;
    public CharacterController controller;
    public float speed = 8.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 2.0f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = .2f;
    public float gravity = 30.0f;

    [Header("Animation")]
    public Animator animator;

    public KeyCodes keyCodes;

    [HideInInspector] public Vector3 inputVec;
    [HideInInspector] public float currentSpeedMultiplier;
    [HideInInspector] public bool grounded;
    [HideInInspector] public Vector3 velocity;

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
        instance = this;
        states = new Dictionary<EPlayerStates, IState<EPlayerStates>>();
        states.Add(EPlayerStates.Idle, new PlayerIdleState());
        states.Add(EPlayerStates.Walk, new PlayerWalkState());
        states.Add(EPlayerStates.Run, new PlayerRunState());
        states.Add(EPlayerStates.Fall, new PlayerFallState());
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
        HandleCameraShake();
    }
    
    private float turnSmoothVelocity;
    public void HandleKeyboardMovement() {
        Vector3 normalizedInput = inputVec.normalized;
        
        if (normalizedInput.magnitude <= 0)
            return;

        float targetAngle = Mathf.Atan2(normalizedInput.x, normalizedInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothT);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        Vector3 moveForwardDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        Vector3 move = moveForwardDir;
        currentSpeedMultiplier = Input.GetKey(keyCodes.sprintKey) ? sprintMultiplier : 1f;
        move *= Time.deltaTime * speed * currentSpeedMultiplier;
        controller.Move(move);
    }

    public void HandleStationaryInput() {
        if (Input.GetKeyDown(keyCodes.jumpKey))
        {
            velocity.y = Mathf.Sqrt(gravity * 2f * jumpHeight);
        }
    }

    public void HandleGroundCheck() {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public void HandleGravity() {
        velocity.y += -gravity * Time.deltaTime;
        if (grounded && velocity.y < 0)
            velocity.y = -2f;
        controller.Move(velocity * Time.deltaTime);
    }

    public System.Type GetCurrentState() {
        return currentState.GetType();
    }

    struct CineShake {
        public float startTime;
        public float shakeTimer;
        public float shakeIntensity;
    }

    private CineShake shakeParams;

    public void CameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cineNoise.AmplitudeGain = intensity;
        shakeParams.shakeTimer = time;
        shakeParams.startTime = time;
        shakeParams.shakeIntensity = intensity;
    }

    public void HandleCameraShake() {
        if (shakeParams.shakeTimer > 0)
        {
            CinemachineBasicMultiChannelPerlin cineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            shakeParams.shakeTimer -= Time.deltaTime;
            cineNoise.AmplitudeGain = Mathf.Lerp(0, shakeParams.shakeIntensity, shakeParams.shakeTimer / shakeParams.startTime);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}