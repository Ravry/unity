#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RaymarchArea : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public float noiseScale = .8f;
    public int noiseOctaves = 1;
    public float stepSize = .1f;
    public int stepCount = 100;
    public Color volumeColor = Color.white;
    [HideInInspector] public Matrix4x4 bound;

    public void Start() {
        bound = Matrix4x4.identity;
    }

    public void Update() {
        UpdateBounds();
    }

    private void UpdateBounds() {
        bound.SetRow(0, new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
        bound.SetRow(1, new Vector4(size.x, size.y, size.z, 0) * .5f);
        bound.SetRow(2, new Vector4(stepSize, stepCount, noiseScale, noiseOctaves));
        bound.SetRow(3, new Vector4(volumeColor.r, volumeColor.g, volumeColor.b, 0));
    }
    
    #if UNITY_EDITOR
    public void OnDrawGizmos() {
        UpdateBounds();
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
        GUIStyle style = new();
        style.normal.textColor  = Gizmos.color;
        style.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position, "Raymarched!", style);
    }
    #endif
}
