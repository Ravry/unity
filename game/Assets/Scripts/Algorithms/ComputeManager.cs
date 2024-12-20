using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComputeManager : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private float threshold = .5f;
    private const int SIZE = 10;
    private ComputeBuffer voxelBuffer;
    private float[] voxelData;

    void Update() {
        ComputeNoise();
    }

    void ComputeNoise()
    {
        int totalVoxels = SIZE * SIZE * SIZE;
        voxelBuffer = new ComputeBuffer(totalVoxels, sizeof(float));
        voxelData = new float[totalVoxels];
        int kernelHandle = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelHandle, "VoxelBuffer", voxelBuffer);
        computeShader.SetInts("GridSize", SIZE, SIZE, SIZE);
        computeShader.SetInts("BaseVec", (int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        int threadGroupsX = Mathf.CeilToInt(SIZE / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(SIZE / 8.0f);
        int threadGroupsZ = Mathf.CeilToInt(SIZE / 8.0f);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, threadGroupsZ);
        voxelBuffer.GetData(voxelData);
    }

    void OnDestroy()
    {
        // Release the buffer when done
        if (voxelBuffer != null)
        {
            voxelBuffer.Release();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(SIZE/2, SIZE/2, SIZE/2), Vector3.one * SIZE);
        
        if (voxelData == null || voxelData.Length == 0) return;

        for (int y = 0; y < SIZE; y++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    int index = z * SIZE * SIZE + y * SIZE + x;
                    float value = voxelData[index];
                    Gizmos.color = new Color(value , value, value);
                    Vector3 position = new Vector3(x, y, z) + Vector3.one * .5f;
                    if (value >= threshold)
                        Gizmos.DrawWireCube(transform.position + position, Vector3.one);
                }
            }
        }
    }
}
