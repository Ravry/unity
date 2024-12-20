using UnityEngine;

public class RaymarchArea : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public float noiseScale = .8f;
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

    public void OnDrawGizmos() {
        UpdateBounds();
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
    }

    private void UpdateBounds() {
        bound.SetRow(0, new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
        bound.SetRow(1, new Vector4(size.x, size.y, size.z, 0) * .5f);
        bound.SetRow(2, new Vector4(stepSize, stepCount, 0, 0));
        bound.SetRow(3, new Vector4(volumeColor.r, volumeColor.g, volumeColor.b, 0));
    }
}
