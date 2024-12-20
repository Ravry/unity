using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GizmoDemo : MonoBehaviour
{

    [Header("Noise Parameters")]
    public int octaves = 1;
    public float scalar = .1f;
    public float threshold = 1.0f;
    public int size = 10;

    [Header("Gizmo Parameters")]
    [SerializeField] private float radius = .1f;
    [SerializeField] private bool cubes = false;


    private float[,,] perlinValues;
    
    void OnValidate() { 
        Validate();
    }

    void Validate() {
        perlinValues = new float[size, size, size];
        perlinValues = Perlin3D();
    }

    float[,,] Perlin3D() {
        float[,,] _perlinValues = new float[size, size, size];

        for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                for (int x = 0; x < size; x++)
                {
                    float perlinValue = 0;
                    float scale = 1.0f;
                    for (int o = 0; o < (octaves + 1); o++)
                    {
                        perlinValue += scale * Perlin.Noise3D(transform.position.x + x * scalar * (float)o, transform.position.y +  y * scalar * (float)o, transform.position.z + z * scalar * (float)o);
                        scale /= 2.0f;
                    }
                    perlinValue += 1.0f;
                    perlinValue /= 2.0f;
                    
                    _perlinValues[x, y, z] = perlinValue;
                    // Debug.Log(perlinValue);
                }
        
        return _perlinValues;
    }

    void OnDrawGizmos() {
        if (!this.enabled)
            return;

        for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                for (int x = 0; x < size; x++)
                {
                    float perlinValue = perlinValues[x, y, z];
                    if (perlinValue <= threshold)
                    {
                        Gizmos.color = new Color(perlinValue, perlinValue, perlinValue);
                        Gizmos.DrawSphere(transform.position + new Vector3(x, y, z), radius);
                    }
                }
    }

    private void Start() {
        if (!Application.isPlaying || !cubes) return;

        for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                for (int x = 0; x < size; x++)
                {
                    float perlinValue = perlinValues[x, y, z];
                    if (perlinValue <= threshold)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = transform.position + new Vector3(x, y, z);
                    }
                }
    }
}