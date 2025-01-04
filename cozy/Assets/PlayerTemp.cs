using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform orientation;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;
    public float moveSpeed;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.linearDamping = 10f; 
    }

    private Vector3 inputDir;

    void Update()
    {
        float horizontalInput, verticalInput;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        HandleRotation(inputDir);
    }

    void FixedUpdate() {
        HandleMovement(inputDir);
        HandleSpeedControl();
    }

    private void HandleRotation(Vector3 inputDir) {
        Vector3 viewDir = transform.position - new Vector3(cam.position.x, transform.position.y, cam.position.z);
        orientation.forward = viewDir.normalized;
        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    private void HandleMovement(Vector3 inputDir) {
        Vector3 moveDir = inputDir.normalized * moveSpeed;
        rb.AddForce(moveDir, ForceMode.Force);
    }

    private void HandleSpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}
