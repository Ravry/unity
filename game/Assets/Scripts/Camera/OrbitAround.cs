using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private Vector3 centeroffset;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private AnimationCurve easingCurve;

    private float angle;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            offset.y += 10f * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftControl))
            offset.y -= 10f * Time.deltaTime;

        radius -= Input.mouseScrollDelta.y;
        radius = Mathf.Clamp(radius, 0, 100);
        float scalar = ease(angle / 360f);
        angle += speed * Time.deltaTime * scalar;
        angle %= 360f;
        transform.position = center.position + centeroffset + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0, radius * Mathf.Sin(angle * Mathf.Deg2Rad)) + offset;
        transform.LookAt(center.position + centeroffset);
    }

    private float ease(float t)
    {
        return easingCurve.Evaluate(t);
    }
}