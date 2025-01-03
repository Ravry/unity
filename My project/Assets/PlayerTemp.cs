using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    [SerializeField] private Transform cameraTRS;
    Rigidbody rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleRotation();
    }

    public void HandleRotation() {
        Vector3 forward = Vector3.ProjectOnPlane(cameraTRS.forward, Vector3.up);
        Quaternion desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
        rb.rotation = desiredRotation;
    }
}