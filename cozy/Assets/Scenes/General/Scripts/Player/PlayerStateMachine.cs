using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : Statemachine<EPlayerStates>
{
    public static PlayerStateMachine instance;
    
    [Header("Player")]
    public Transform groundCheck;
    public Transform cam;
    public CharacterController controller;
    public float gravity = 20.0f;
    public float speed = 8.0f;
    public float jumpHeight = 2.0f;
    public float groundDistance = .2f;
    public LayerMask groundMask;


    [HideInInspector] public Vector3 inputVec;
    [HideInInspector] public Vector3 mouseVec;
    [HideInInspector] public float pitch, yaw;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public bool grounded;

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
        states.Add(EPlayerStates.Fall, new PlayerFallState());
        currentState = states[EPlayerStates.Idle];
    }

    public override void UpdateVars()
    {
        inputVec = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );

        mouseVec = new Vector3(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"),
            0
        );

        HandleGroundCheck();
    }

    public void HandleMouseInput() {
        pitch -= mouseVec.y;
        pitch = Mathf.Clamp(pitch, -80.0f, 80.0f);
        yaw += mouseVec.x;
        cam.position = transform.position + Vector3.up * 2;
        cam.rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    public void HandleKeyboardMovement() {
        Vector3 normalizedInput = inputVec.normalized;
        Vector3 move = transform.right * normalizedInput.x + transform.forward * normalizedInput.z;
        move *= Time.deltaTime * speed;
        controller.Move(move);
    }

    public void HandleStationaryInput() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    public void HandleGravity() {
        velocity.y += -gravity * Time.deltaTime;
        if (grounded && velocity.y < 0)
            velocity.y = -2f;
        controller.Move(velocity * Time.deltaTime);
    }

    public void HandleGroundCheck() {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public System.Type GetCurrentState() {
        return currentState.GetType();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10f))
        {
            Gizmos.DrawLine(cam.transform.position, hit.point);
        }
        else
        {
            Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * 10f);
        }

    }
}