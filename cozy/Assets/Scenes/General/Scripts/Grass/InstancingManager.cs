using Unity.VisualScripting;
using UnityEngine;

public class InstancingManager : MonoBehaviour
{
    struct InstanceData {
        public Matrix4x4 matrix;
    };

    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private int size = 8;
    [SerializeField] private float density = 2.0f;

    private ComputeBuffer instanceBuffer;
    private Matrix4x4[] matrices;


    void Start() {
        int densSize = (int)(size * density);
        int totalInstances = densSize * densSize;
        matrices = new Matrix4x4[totalInstances];
        
        computeShader.SetVector("_CenterPosition", transform.position);
        computeShader.SetInt("_Resolution", size);
        computeShader.SetFloat("_Density", density);
        
        instanceBuffer = new ComputeBuffer(totalInstances, sizeof(float) * 16);
        computeShader.SetBuffer(0, "instanceBuffer", instanceBuffer);

        int kernel = computeShader.FindKernel("CSMain");
        computeShader.Dispatch(kernel, densSize / 8, densSize / 8, 1);

        InstanceData[] instanceData = new InstanceData[totalInstances];
        instanceBuffer.GetData(instanceData);

        for (int i = 0; i < totalInstances; i++)
        {
            matrices[i] = instanceData[i].matrix;
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }

    private void OnDestroy()
    {
        instanceBuffer.Release();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(size, 2, size));
    }
}