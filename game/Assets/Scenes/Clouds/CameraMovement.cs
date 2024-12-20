using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float pitch, yaw;

    void Update()
    {
        float up = 0;
        if (Input.GetKey(KeyCode.Space))
            up = 1;
        if (Input.GetKey(KeyCode.LeftControl))
            up = -1;
        float speed = 10f;
        transform.position += transform.forward * Input.GetAxisRaw("Vertical") * Time.deltaTime * speed + transform.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed + Vector3.up * up * Time.deltaTime * speed;
        pitch -= Input.GetAxisRaw("Mouse Y");
        yaw += Input.GetAxisRaw("Mouse X");
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        if (Input.GetKeyDown(KeyCode.R))
        {
            pitch = 0; yaw = 0;
        }
    }
}
